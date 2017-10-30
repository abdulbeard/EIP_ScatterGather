using ScatterGather.Aggregation.Implementation;
using ScatterGather.Request;
using ScatterGather.Result;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterGather.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var main = new WorkloadManager();
            var yolo = main.MulticastAsync(new HttpRequestInstructions<string>(
                new HttpRequestMessageWrapper(), TimeSpan.FromMinutes(5), ResponseTransformer, Guid.Empty, new HttpClientManager()),
                new List<IHost> { new HttpHost("asdf"), new HttpHost("jkll") }, new AscendingSortAggregator<string>()).Result;
            var nolo = main.MulticastAsync(new List<HttpRequestInstructions<string>> {
                new HttpRequestInstructions<string>(new HttpRequestMessageWrapper
                    {
                        RequestUri = new Uri("http://arrrrrrrdfasdfasdfuioui.com:8080")
                    }, TimeSpan.FromMinutes(5), ResponseTransformer, Guid.Empty, new HttpClientManager())
                }.ToArray(),
                TimeSpan.FromMinutes(5), new DescendingSortAggregator<string>()).Result;
        }

        [Fact]
        public void Test2()
        {
            var testClientManager = new TestClientManager();
            testClientManager.AddClient(new TestHost("qwer"), new TestClient("qwer"));
            testClientManager.AddClient(new TestHost("uiop"), new TestClient("uiop"));
            var wlm = new WorkloadManager();
            var yolo = wlm.MulticastAsync(new TestAbstractRequestInstructions(new TestRequest(new TestHost("requestASDF")), TimeSpan.FromMilliseconds(10000),
                TestResponseTransformer, Guid.NewGuid(), testClientManager), new List<IHost> { new TestHost("qwer"), new TestHost("uiop") },
                new AscendingSortAggregator<string>()).Result;
            Assert.Equal(yolo.Count, 2);
            Assert.Equal(yolo[0], "ScatterGather.Tests.TestHost_sklfjlksdjklflksdkflsldf_qwer_ResponseTransformed");
            Assert.Equal(yolo[1], "ScatterGather.Tests.TestHost_sklfjlksdjklflksdkflsldf_uiop_ResponseTransformed");
        }

        private static ResultEnvelope<string> ResponseTransformer(HttpResponseMessage arg)
        {
            throw new NotImplementedException();
        }

        private static ResultEnvelope<string> TestResponseTransformer(string arg)
        {
            return new ResultEnvelope<string>
            {
                Result = $"{arg}_ResponseTransformed",
                Sequence = Environment.TickCount,
                SortWeight = Environment.TickCount
            };
        }
    }

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
            this.Host = host;
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
