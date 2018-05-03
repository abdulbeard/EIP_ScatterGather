using ScatterGather.Aggregation.Implementation;
using ScatterGather.Request;
using ScatterGather.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using ScatterGather.Tests.TestClasses;

namespace ScatterGather.Tests
{
    public class UnitTest1
    {
        private TestServer _server;
        private HttpClient _client;
        private TestServer _server1;
        private HttpClient _client1;

        public UnitTest1()
        {
            _server = new TestServer(new WebHostBuilder().UseUrls("http://localhost:5050").UseStartup<Startup>());
            _client = _server.CreateClient();
            _server1 = new TestServer(new WebHostBuilder().UseUrls("http://localhost:5050").UseStartup<Startup>());
            _client1 = _server1.CreateClient();
        }
        [Fact]
        public void Test1()
        {
            var main = new WorkloadManager();
            var clientManager = new HttpClientManager();
            clientManager.AddClient(new HttpHost(_client.BaseAddress.AbsoluteUri), new SgHttpClient(_client));
            var result = main.MulticastAsync(
                new HttpRequestInstructions<string>(new HttpRequestMessageWrapper(), TimeSpan.FromSeconds(30),
                    ResponseTransformer, Guid.NewGuid()),
                new List<IHost>()
                {
                    new HttpHost("http://localhost:1111"),
                    new HttpHost("http://localhost:51073"),
                    new HttpHost("http://localhost:51073"),
                    new HttpHost("http://localhost:1111"),
                }, new AscendingSortAggregator<string>()).Result;
            Assert.Equal(2, result.Count);
            Assert.True(result.All(x => x == "All is well"));
        }

        [Fact]
        public void Test2()
        {
            var testClientManager = new TestClientManager();
            var qwer = "qwer";
            var uiop = "uiop";
            testClientManager.AddClient(new TestHost(qwer), new TestClient(qwer));
            testClientManager.AddClient(new TestHost(uiop), new TestClient(uiop));
            var wlm = new WorkloadManager();
            var yolo = wlm.MulticastAsync(new TestAbstractRequestInstructions(new TestRequest(new TestHost("requestASDF")), TimeSpan.FromMilliseconds(10000),
                TestResponseTransformer, Guid.NewGuid(), testClientManager), new List<IHost> { new TestHost(qwer), new TestHost(uiop) },
                new AscendingSortAggregator<string>()).Result;
            Assert.Equal(yolo.Count, 2);
            Assert.True(yolo.Contains($"{typeof(TestHost).FullName}_sklfjlksdjklflksdkflsldf_{uiop}_ResponseTransformed"));
            Assert.True(yolo.Contains($"{typeof(TestHost).FullName}_sklfjlksdjklflksdkflsldf_{qwer}_ResponseTransformed"));
        }

        private static ResultEnvelope<string> ResponseTransformer(HttpResponseMessage arg)
        {
            return new ResultEnvelope<string>()
            {
                Result = arg?.Content?.ReadAsStringAsync().Result ?? string.Empty,
                Sequence = 0,
                SortWeight = 1
            };
        }

        private static ResultEnvelope<string> TestResponseTransformer(string arg)
        {
            return new ResultEnvelope<string>
            {
                Result = $"{arg}_ResponseTransformed",
                Sequence = arg.GetHashCode(),
                SortWeight = arg.GetHashCode()
            };
        }
    }
}
