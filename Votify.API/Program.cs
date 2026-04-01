using DotNetEnv;
using Votify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Votify.Application.Interfaces;
using Votify.Application.Services;
using Votify.Infrastructure.Repositories;
using Votify.Domain.Interfaces;

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
builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<IVotoRepository, VotoRepository>();
builder.Services.AddScoped<IVotoService, VotoService>();

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
app.UseHttpsRedirection();
app.MapControllers();

app.Run();