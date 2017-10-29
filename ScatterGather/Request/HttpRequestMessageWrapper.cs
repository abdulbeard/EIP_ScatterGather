using System.Net.Http;

namespace ScatterGather.Request
{
    public class HttpRequestMessageWrapper : HttpRequestMessage, IRequest
    {
        public string Host => RequestUri?.ToString() ?? string.Empty;
    }
}
