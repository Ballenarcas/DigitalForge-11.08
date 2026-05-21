using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Votify.Client;
using Votify.Client.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<VotacionesService>();
builder.Services.AddScoped<ProyectosService>();
builder.Services.AddScoped<EventosService>();
builder.Services.AddScoped<EquiposService>();
builder.Services.AddScoped<ParticipantesService>();
builder.Services.AddScoped<NotificacionesService>();
builder.Services.AddScoped<AppState>();

// Servicios de Autenticación
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5154/")
});

await builder.Build().RunAsync();
