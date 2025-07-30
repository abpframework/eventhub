using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace EventHub.Web;

public class LoggingHandler : DelegatingHandler
{
    public LoggingHandler()
        : base(new HttpClientHandler())
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Log.Error($"[Request] {request.Method} {request.RequestUri}");
        if (request.Method == HttpMethod.Post && request.Content != null)
        {
            var content = await request.Content.ReadAsStringAsync(cancellationToken);
            Log.Error($"[Request Content Headers] {request.Content.Headers}");
            Log.Error($"[Request Content] {content}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
