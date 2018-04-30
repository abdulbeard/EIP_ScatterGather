using System;
using ScatterGather.Result;
using System.Threading.Tasks;
using System.Threading;

namespace ScatterGather.Request
{
    public class SimpleStringRequestInstructions : AbstractRequestInstructions<object, SimpleRequest, object>
    {
        public SimpleStringRequestInstructions(SimpleRequest  request, TimeSpan timeout, Func<object, ResultEnvelope<object>> responseTransformer,
    Guid id, IClientManager<object, SimpleRequest> clientManager = null)
        {
            Request = request;
            Timeout = timeout;
            ResponseTransformer = responseTransformer;
            Id = id;
            ClientManager = clientManager;
        }

        public override SimpleRequest Request { get; }

        public override TimeSpan Timeout { get; }

        public override Func<object, ResultEnvelope<object>> ResponseTransformer { get; }

        public override Guid Id { get; }

        public override IClientManager<object, SimpleRequest> ClientManager { get; }
    }

    public abstract class SimpleRequest : IRequest
    {
        protected SimpleRequest(IHost host)
        {
            Host = host;
        }
        public IHost Host { get; }
    }

    public class SimpleHost<T> : IHost
    {
        public SimpleHost(T host)
        {
            Host = host;
        }
        public object Host { get; private set; }

        public new bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }

    public class SimpleClient<TOut> : IClient<TOut, IRequest>
    {
        private string _clientId;
        public SimpleClient(string clientId)
        {
            _clientId = clientId;
        }
        public Task<TOut> SendAsync(IRequest request, CancellationToken ct)
        {
            return Task.FromResult(default(TOut));
            //return Task.FromResult($"{request.Host}_sklfjlksdjklflksdkflsldf_{clientId}");
        }
    }
}
