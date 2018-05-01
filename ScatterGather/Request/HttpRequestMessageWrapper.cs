using System.Net.Http;

namespace ScatterGather.Request
{
    public abstract class HttpRequestMessageWrapper : HttpRequestMessage, IRequest
    {
        public IHost Host => new HttpHost(RequestUri?.ToString() ?? string.Empty);
    }
}
