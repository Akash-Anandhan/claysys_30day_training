using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

public class JwtAuthHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authHeader = request.Headers.Authorization;
        if (authHeader != null && authHeader.Scheme == "Bearer")
        {
            try
            {
                var principal = JwtHelper.ValidateToken(authHeader.Parameter);
                Thread.CurrentPrincipal = principal;
                System.Web.HttpContext.Current.User = principal;
            }
            catch
            {
                // Token invalid — let [Authorize] reject the request
            }
        }
        return await base.SendAsync(request, cancellationToken);
    }
}