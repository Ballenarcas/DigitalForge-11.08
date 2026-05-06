using Moq;
using Xunit;
using Votify.Application.DTOs;
using Votify.Application.Services;
using Votify.Domain.Entities;
using Votify.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Votify.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IParticipanteRepository> _mockParticipanteRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockParticipanteRepository = new Mock<IParticipanteRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Configurar la clave JWT
            _mockConfiguration
                .Setup(x => x["Jwt:Key"])
                .Returns("ClaveSecretaSuperLargaParaQueFuncioneElJWT32Caracteres");

            _authService = new AuthService(_mockParticipanteRepository.Object, _mockConfiguration.Object);
        }

        #region RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "SecurePassword123",
                Nombre = "New User"
            };

            _mockParticipanteRepository
                .Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync((Participante)null);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Registro exitoso", result.Message);
            Assert.Empty(result.Token);
            _mockParticipanteRepository.Verify(x => x.AddAsync(It.IsAny<Participante>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldReturnFailure()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "existing@example.com",
                Password = "SecurePassword123",
                Nombre = "Existing User"
            };

            var existingUser = new Participante("Existing User", "existing@example.com", "hash");
            _mockParticipanteRepository
                .Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Email ya registrado", result.Message);
            _mockParticipanteRepository.Verify(x => x.AddAsync(It.IsAny<Participante>()), Times.Never);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("user@")]
        [InlineData("")]
        public async Task RegisterAsync_WithInvalidEmail_ShouldReturnFailure(string email)
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = email,
                Password = "SecurePassword123",
                Nombre = "User"
            };

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El formato del correo electrónico no es válido.", result.Message);
            _mockParticipanteRepository.Verify(x => x.AddAsync(It.IsAny<Participante>()), Times.Never);
        }

        #endregion

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var password = "SecurePassword123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var request = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = password
            };

            var user = new Participante("User", "user@example.com", hashedPassword);
            _mockParticipanteRepository
                .Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Login exitoso", result.Message);
            Assert.NotEmpty(result.Token);
        }

        [Fact]
        public async Task LoginAsync_WithNonexistentEmail_ShouldReturnFailure()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "nonexistent@example.com",
                Password = "SomePassword"
            };

            _mockParticipanteRepository
                .Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync((Participante)null);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Credenciales inválidas", result.Message);
            Assert.Empty(result.Token);
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldReturnFailure()
        {
            // Arrange
            var password = "SecurePassword123";
            var wrongPassword = "WrongPassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var request = new LoginRequestDto
            {
                Email = "user@example.com",
                Password = wrongPassword
            };

            var user = new Participante("User", "user@example.com", hashedPassword);
            _mockParticipanteRepository
                .Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Credenciales inválidas", result.Message);
            Assert.Empty(result.Token);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("")]
        public async Task LoginAsync_WithInvalidEmail_ShouldReturnFailure(string email)
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = email,
                Password = "SomePassword"
            };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El formato del correo electrónico no es válido.", result.Message);
        }

        #endregion
    }
}
