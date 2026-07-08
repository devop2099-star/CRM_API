using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using CRM.WebFrontend.Client.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register named HttpClient pointing to the host (WebFrontend) for API requests
builder.Services.AddHttpClient("BackendApi", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

await builder.Build().RunAsync();
