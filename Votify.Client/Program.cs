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

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

var apiUrl = builder.HostEnvironment.IsDevelopment()
    ? "http://localhost:5154"
    : "https://votify.azurewebsites.net/";

builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();
    var client = new HttpClient(handler) { BaseAddress = new Uri(apiUrl) };
    return client;
});

await builder.Build().RunAsync();
