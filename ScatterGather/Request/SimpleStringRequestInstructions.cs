using System;
using ScatterGather.Result;
using System.Threading.Tasks;
using System.Threading;

namespace ScatterGather.Request
{
    public class SimpleStringRequestInstructions : AbstractRequestInstructions<object, SimpleRequest<object>, object>
    {
        public SimpleStringRequestInstructions(SimpleRequest<object>  request, TimeSpan timeout, Func<object, ResultEnvelope<object>> responseTransformer,
    Guid id, IClientManager<object, SimpleRequest<object>> clientManager = null)
        {
            Request = request;
            Timeout = timeout;
            ResponseTransformer = responseTransformer;
            Id = id;
            ClientManager = clientManager;
        }

        public override SimpleRequest<object> Request { get; }

        public override TimeSpan Timeout { get; }

        public override Func<object, ResultEnvelope<object>> ResponseTransformer { get; }

        public override Guid Id { get; }

        public override IClientManager<object, SimpleRequest<object>> ClientManager { get; }
    }

    public class SimpleRequest<T> : IRequest
    {
        private IHost host;
        public SimpleRequest(IHost host)
        {
            this.host = host;
        }
        public IHost Host => host;
    }

    public class SimpleHost<T> : IHost
    {
        public SimpleHost(T host)
        {
            this.Host = host;
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
        private string clientId;
        public SimpleClient(string clientId)
        {
            this.clientId = clientId;
        }
        public Task<TOut> SendAsync(IRequest request, CancellationToken ct)
        {
            return Task.FromResult(default(TOut));
            //return Task.FromResult($"{request.Host}_sklfjlksdjklflksdkflsldf_{clientId}");
        }
    }
}
