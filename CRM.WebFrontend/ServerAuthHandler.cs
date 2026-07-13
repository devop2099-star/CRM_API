using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace CRM.WebFrontend;

public class ServerAuthHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerAuthHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null && httpContext.User.Identity?.IsAuthenticated == true)
        {
            var tokenClaim = httpContext.User.FindFirst("access_token");
            if (tokenClaim != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenClaim.Value);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
