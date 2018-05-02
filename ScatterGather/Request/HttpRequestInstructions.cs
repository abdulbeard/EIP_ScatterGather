using ScatterGather.Result;
using System;
using System.Net.Http;

namespace ScatterGather.Request
{
    /// <inheritdoc />
    public class HttpRequestInstructions<T> : AbstractRequestInstructions<T, HttpRequestMessageWrapper, HttpResponseMessage>
    {
        public HttpRequestInstructions(HttpRequestMessageWrapper request, TimeSpan timeout, Func<HttpResponseMessage, ResultEnvelope<T>> responseTransformer,
            Guid id, HttpClientManager clientManager = null)
        {
            Request = request;
            Timeout = timeout;
            ResponseTransformer = responseTransformer;
            Id = id;
            ClientManager = clientManager;
        }
        public override HttpRequestMessageWrapper Request { get; }

        public override TimeSpan Timeout { get; }

        public override Func<HttpResponseMessage, ResultEnvelope<T>> ResponseTransformer { get; }

        public override Guid Id { get; }

        public override IClientManager<HttpResponseMessage, HttpRequestMessageWrapper> ClientManager { get; }
    }
}
