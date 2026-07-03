using CRM.WebFrontend.Server.Components;
using Microsoft.AspNetCore.Components.Authorization;
using CRM.WebFrontend.Client.Providers;
using MudBlazor.Services;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Configuración BFF: Handler para propagar la cookie en SSR y configuración de HttpClient
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CookieHandler>();
builder.Services.AddHttpClient("BFF").AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped(sp => 
{
    var nav = sp.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
    var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("BFF");
    client.BaseAddress = new Uri(nav.BaseUri);
    return client;
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

// Endpoints BFF para Autenticación con Cookies HttpOnly
app.MapPost("/api/auth/login", (LoginRequest req, HttpContext ctx) =>
{
    // Simulamos validación y parseo de roles (En un entorno real, esto haría un HTTP POST al API Hub externo)
    var isSupervisor = req.Email.Contains("supervisor", StringComparison.OrdinalIgnoreCase);
    var role = isSupervisor ? "SUPERVISOR" : "ASESOR";
    
    // Generación de un JWT Mockeado para demostrar el almacenamiento seguro
    var payload = new {
        unique_name = req.Email,
        role = role,
        exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
    };
    var base64Payload = Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(payload)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    var mockJwt = $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{base64Payload}.mock_signature";
    
    ctx.Response.Cookies.Append("authToken", mockJwt, new CookieOptions 
    { 
        HttpOnly = true, 
        Secure = true, 
        SameSite = SameSiteMode.Strict 
    });
    
    return Results.Ok(new { success = true, role = role });
});

app.MapGet("/api/auth/userinfo", (HttpContext ctx) =>
{
    if (ctx.Request.Cookies.TryGetValue("authToken", out var token))
    {
        try
        {
            var payloadStr = token.Split('.')[1];
            switch (payloadStr.Length % 4)
            {
                case 2: payloadStr += "=="; break;
                case 3: payloadStr += "="; break;
            }
            var jsonBytes = Convert.FromBase64String(payloadStr.Replace('-', '+').Replace('_', '/'));
            var claimsDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);
            return Results.Ok(claimsDict);
        }
        catch { }
    }
    return Results.Unauthorized();
});

app.MapPost("/api/auth/logout", (HttpContext ctx) =>
{
    ctx.Response.Cookies.Delete("authToken");
    return Results.Ok();
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CookieHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CookieHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var cookie = _httpContextAccessor.HttpContext?.Request.Cookies["authToken"];
        if (!string.IsNullOrEmpty(cookie))
        {
            request.Headers.Add("Cookie", $"authToken={cookie}");
        }
        return base.SendAsync(request, cancellationToken);
    }
}
