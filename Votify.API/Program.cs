using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using Votify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Votify.Application.Interfaces;
using Votify.Application.Services;
using Votify.Application.Services.Fachadas;
using Votify.Infrastructure.Repositories;
using Votify.Domain.Interfaces;
using Votify.Infrastructure.Configuration;
using Votify.Infrastructure.Adapters;
using Votify.Infrastructure.Storage;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
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
builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
builder.Services.AddScoped<IVotacionService, VotacionService>();
builder.Services.AddScoped<EquipoService>();
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
builder.Services.AddScoped<ICriterioRepository, CriterioRepository>();
builder.Services.AddScoped<IValoracionCriterioRepository, ValoracionCriterioRepository>();
builder.Services.AddScoped<Votify.Domain.Interfaces.IEventoRepository, Votify.Infrastructure.Repositories.EventoRepository>();
builder.Services.AddScoped<Votify.Domain.Interfaces.IParticipanteEventoRepository, Votify.Infrastructure.Repositories.ParticipanteEventoRepository>();
builder.Services.AddScoped<Votify.Application.Interfaces.IEventoService, Votify.Application.Services.EventoService>();

// New service interfaces (prerequisites for facades)
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IParticipanteService, ParticipanteService>();

// Facade registrations
builder.Services.AddScoped<IEventoFachada, EventoFachada>();
builder.Services.AddScoped<IVotacionFachada, VotacionFachada>();
builder.Services.AddScoped<IVotoFachada, VotoFachada>();
builder.Services.AddScoped<IProyectoFachada, ProyectoFachada>();
builder.Services.AddScoped<IEquipoFachada, EquipoFachada>();
builder.Services.AddScoped<IParticipanteFachada, ParticipanteFachada>();

// Resumidor de comentarios con IA (Patron Adapter)
builder.Services.Configure<OpcionesResumidorIA>(options =>
{
    builder.Configuration.GetSection("AISummarizer").Bind(options);

    var envEnabled = Environment.GetEnvironmentVariable("AI_SUMMARIZER_ENABLED");
    var envBaseUrl = Environment.GetEnvironmentVariable("AI_SUMMARIZER_BASE_URL");
    var envApiKey = Environment.GetEnvironmentVariable("AI_SUMMARIZER_API_KEY");
    var envModel = Environment.GetEnvironmentVariable("AI_SUMMARIZER_MODEL");

    if (!string.IsNullOrEmpty(envEnabled) && bool.TryParse(envEnabled, out var enabled))
        options.Enabled = enabled;
    if (!string.IsNullOrEmpty(envBaseUrl)) options.BaseUrl = envBaseUrl;
    if (!string.IsNullOrEmpty(envApiKey)) options.ApiKey = envApiKey;
    if (!string.IsNullOrEmpty(envModel)) options.Model = envModel;
});

var aiOptions = builder.Configuration.GetSection("AISummarizer").Get<OpcionesResumidorIA>()
                ?? new OpcionesResumidorIA();

var envEnabled = Environment.GetEnvironmentVariable("AI_SUMMARIZER_ENABLED");
var envBaseUrl = Environment.GetEnvironmentVariable("AI_SUMMARIZER_BASE_URL");
var envApiKey = Environment.GetEnvironmentVariable("AI_SUMMARIZER_API_KEY");
var envModel = Environment.GetEnvironmentVariable("AI_SUMMARIZER_MODEL");

if (!string.IsNullOrEmpty(envEnabled) && bool.TryParse(envEnabled, out var enabled))
    aiOptions.Enabled = enabled;
if (!string.IsNullOrEmpty(envBaseUrl)) aiOptions.BaseUrl = envBaseUrl;
if (!string.IsNullOrEmpty(envApiKey)) aiOptions.ApiKey = envApiKey;
if (!string.IsNullOrEmpty(envModel)) aiOptions.Model = envModel;

builder.Services.AddScoped<ResumidorComentariosFallback>();

if (aiOptions.Enabled && !string.IsNullOrEmpty(aiOptions.BaseUrl) && !string.IsNullOrEmpty(aiOptions.ApiKey))
{
    builder.Services.AddHttpClient<AdaptadorClienteIA, AdaptadorClienteIA>();
    builder.Services.AddScoped<IResumidorComentariosIA>(sp =>
        new ResumidorComentariosResiliente(
            sp.GetRequiredService<AdaptadorClienteIA>(),
            sp.GetRequiredService<ResumidorComentariosFallback>(),
            sp.GetRequiredService<ILogger<ResumidorComentariosResiliente>>()));
}
else
{
    builder.Services.AddScoped<IResumidorComentariosIA, ResumidorComentariosFallback>();
}

Console.WriteLine($"Resumidor IA => Enabled={aiOptions.Enabled}, BaseUrl={aiOptions.BaseUrl}, Model={aiOptions.Model}, ApiKeyPresente={!string.IsNullOrEmpty(aiOptions.ApiKey)}");
Console.WriteLine($"DB => {host}:{port}/{db} USER => {user}");

var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? "";
var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_KEY") ?? "";
builder.Services.AddHttpClient<IStorageService, SupabaseStorageService>(client =>
{
    client.DefaultRequestHeaders.Add("apikey", supabaseKey);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());
builder.Services.AddScoped<SupabaseStorageService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient(nameof(SupabaseStorageService));
    var logger = sp.GetRequiredService<ILogger<SupabaseStorageService>>();
    return new SupabaseStorageService(httpClient, supabaseUrl, supabaseKey, logger);
});
builder.Services.AddScoped<IStorageService>(sp => sp.GetRequiredService<SupabaseStorageService>());

Console.WriteLine($"Supabase Storage => Url={supabaseUrl}, KeyPresente={!string.IsNullOrEmpty(supabaseKey)}");

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

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
app.MapHealthChecks("/healthz");

app.Run();
