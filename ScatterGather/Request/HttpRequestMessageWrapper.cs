using System.Net.Http;

namespace ScatterGather.Request
{
    public class HttpRequestMessageWrapper : HttpRequestMessage, IRequest
    {
        public IHost Host => new HttpHost(RequestUri?.ToString() ?? string.Empty);

    }
}
