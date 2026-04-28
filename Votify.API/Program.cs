using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using Votify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Votify.Application.Interfaces;
using Votify.Application.Services;
using Votify.Infrastructure.Repositories;
using Votify.Domain.Interfaces;
using System.Text;

static string? FindEnvFile()
{
    foreach (var startDir in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
    {
        var dir = startDir;
        while (dir != null)
        {
            var candidate = Path.Combine(dir, ".env");
            if (File.Exists(candidate)) return candidate;
            dir = Directory.GetParent(dir)?.FullName;
        }
    }
    return null;
}


var envFile = FindEnvFile();
if (envFile != null) Env.Load(envFile);

var builder = WebApplication.CreateBuilder(args);

var host = Environment.GetEnvironmentVariable("DB_HOST");
var db = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");
var port = Environment.GetEnvironmentVariable("DB_PORT");

var connectionString = 
    $"Host={host};Port={port};Database={db};Username={user};Password={pass};SslMode=Require";


builder.Services.AddDbContext<VotifyDbContext>(options =>
{
    options.UseNpgsql(connectionString, o =>
        o.EnableRetryOnFailure());
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine);    
});

builder.Services.AddScoped<IVotacionRepository, VotacionRepository>();
builder.Services.AddScoped<IVotacionService, VotacionService>();
builder.Services.AddScoped<IProyectoRepository, ProyectoRepository>();
builder.Services.AddScoped<IParticipanteRepository, ParticipanteRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "ClaveSecretaSuperLargaParaQueFuncioneElJWT32Caracteres");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = true,
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<IVotoRepository, VotoRepository>();
builder.Services.AddScoped<IVotoService, VotoService>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<Votify.Domain.Interfaces.IEventoRepository, Votify.Infrastructure.Repositories.EventoRepository>();
builder.Services.AddScoped<Votify.Domain.Interfaces.IParticipanteEventoRepository, Votify.Infrastructure.Repositories.ParticipanteEventoRepository>();
builder.Services.AddScoped<Votify.Application.Interfaces.IEventoService, Votify.Application.Services.EventoService>();

Console.WriteLine($"DB => {host}:{port}/{db} USER => {user}");

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseCors("AllowBlazor");
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();