using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IParticipanteRepository _participanteRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IParticipanteRepository participanteRepository, IConfiguration configuration)
        {
            _participanteRepository = participanteRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Validar si el email ya existe
            var usuarioExistente = await _participanteRepository.GetByEmailAsync(request.Email);
            if (usuarioExistente != null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Email ya registrado", Token = "" };
            }

            // 2. Encriptar contraseña
            string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var nuevoUsuario = new Participante(request.Nombre, request.Email, hash)
            {
                Rol = "Participante" // Por defecto
            };

            // 3. Guardar en Base de Datos
            await _participanteRepository.AddAsync(nuevoUsuario);

            return new AuthResponseDto { IsSuccess = true, Message = "Registro exitoso", Token = "" };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            // 1. Buscar usuario por email
            var usuario = await _participanteRepository.GetByEmailAsync(request.Email);
            if (usuario == null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Credenciales inválidas", Token = "" };
            }

            // 2. Verificar contraseña con BCrypt
            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash);
            if (!isValid)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Credenciales inválidas", Token = "" };
            }

            // 3. Generar JWT si es válido
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyInfo = _configuration["Jwt:Key"] ?? "ClaveSecretaSuperLargaParaQueFuncioneElJWT32Caracteres";
            var key = Encoding.UTF8.GetBytes(keyInfo);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new AuthResponseDto { IsSuccess = true, Message = "Login exitoso", Token = tokenString };
        }
    }
}