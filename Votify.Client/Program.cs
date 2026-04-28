using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Votify.Client;
using Votify.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<VotacionesService>();
builder.Services.AddScoped<ProyectosService>();
builder.Services.AddScoped<EventosService>();
builder.Services.AddScoped<AppState>();

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5154/") 
});

await builder.Build().RunAsync();
