using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScatterGather.Request;
using ScatterGather.Result;

namespace ScatterGather.Tests.TestClasses
{
    public class TestAbstractRequestInstructions : AbstractRequestInstructions<string, TestRequest, string>
    {
        public TestAbstractRequestInstructions(TestRequest request, TimeSpan timeout, Func<string, ResultEnvelope<string>> responseTransformer,
            Guid id, IClientManager<string, TestRequest> clientManager = null)
        {
            Request = request;
            Timeout = timeout;
            ResponseTransformer = responseTransformer;
            Id = id;
            ClientManager = clientManager;
        }

        public override TestRequest Request { get; }

        public override TimeSpan Timeout { get; }

        public override Func<string, ResultEnvelope<string>> ResponseTransformer { get; }

        public override Guid Id { get; }

        public override IClientManager<string, TestRequest> ClientManager { get; }
    }

    public class TestClientManager : IClientManager<string, TestRequest>
    {
        Dictionary<int, TestClient> dict;
        public TestClientManager()
        {
            dict = dict ?? new Dictionary<int, TestClient>();
        }

        public void AddClient(IHost host, IClient<string, TestRequest> client)
        {
            if (!dict.ContainsKey(((TestHost)host).Host.GetHashCode()))
            {
                dict.Add(((TestHost)host).Host.GetHashCode(), client as TestClient);
            }
        }

        public IClient<string, TestRequest> GetClient(IHost host)
        {
            if (dict.ContainsKey(((TestHost)host).Host.GetHashCode()))
            {
                return dict[((TestHost)host).Host.GetHashCode()];
            }
            return null;
        }
    }

    public class TestClient : IClient<string, TestRequest>
    {
        private string clientId;
        public TestClient(string clientId)
        {
            this.clientId = clientId;
        }
        public Task<string> SendAsync(TestRequest request, CancellationToken ct)
        {
            //if(clientId == "uiop")
            //{
            //    Task.Delay(100).Wait();
            //}
            return Task.FromResult($"{request.Host}_sklfjlksdjklflksdkflsldf_{clientId}");
        }
    }

    public class TestRequest : IRequest
    {
        private IHost host;
        public TestRequest(IHost host)
        {
            this.host = host;
        }
        public IHost Host => host;
    }

    public class TestHost : IHost
    {
        public TestHost(string host)
        {
            Host = host;
        }
        public object Host { get; private set; }

        public new bool Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
