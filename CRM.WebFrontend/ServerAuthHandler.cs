using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace CRM.WebFrontend;

public class ServerAuthHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerAuthHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            var tokenClaim = context.User.FindFirst("access_token");
            if (tokenClaim != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenClaim.Value);
            }
        }
        return base.SendAsync(request, cancellationToken);
    }
}
