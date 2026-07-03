using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace CRM.WebFrontend.Client.Providers;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    public CustomAuthenticationStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Consultamos al BFF en lugar de leer el localStorage. El HttpClient incluirá la cookie automáticamente gracias al CookieHandler.
            var response = await _httpClient.GetAsync("api/auth/userinfo");
            
            if (response.IsSuccessStatusCode)
            {
                var claimsDict = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
                if (claimsDict != null)
                {
                    var claims = new List<Claim>();
                    foreach (var kvp in claimsDict)
                    {
                        var claimType = kvp.Key == "role" ? ClaimTypes.Role : 
                                        kvp.Key == "unique_name" ? ClaimTypes.Name : kvp.Key;
                                        
                        if (kvp.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in kvp.Value.EnumerateArray())
                            {
                                claims.Add(new Claim(claimType, item.ToString()));
                            }
                        }
                        else
                        {
                            claims.Add(new Claim(claimType, kvp.Value.ToString()));
                        }
                    }
                    var identity = new ClaimsIdentity(claims, "BffAuth");
                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }
            }
        }
        catch
        {
            // Fallback silencioso: no está autenticado o la red falló.
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public void NotifyUserAuthentication()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
