using ScatterGather.Aggregation.Implementation;
using ScatterGather.Request;
using ScatterGather.Result;
using System;
using System.Collections.Generic;
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

        public UnitTest1()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }
        [Fact]
        public void Test1()
        {
            var main = new WorkloadManager();
            var clientManager = new HttpClientManager();
            clientManager.AddClient(new HttpHost(_client.BaseAddress.Host), new SgHttpClient(_client));
            var result = main.MulticastAsync(
                new HttpRequestInstructions<string>(new HttpRequestMessageWrapper(), TimeSpan.FromSeconds(30),
                    ResponseTransformer, Guid.NewGuid(), clientManager),
                new List<IHost>()
                {
                    new HttpHost(_client.BaseAddress.AbsoluteUri),
                    //new HttpHost(_client.BaseAddress.AbsoluteUri),
                    //new HttpHost(_client.BaseAddress.AbsoluteUri)
                }, new AscendingSortAggregator<string>()).Result;
            var asdfsdafasdf = result;
            //var yolo = main.MulticastAsync(new HttpRequestInstructions<string>(
            //    new HttpRequestMessageWrapper(), TimeSpan.FromMinutes(5), ResponseTransformer, Guid.Empty, new HttpClientManager()),
            //    new List<IHost> { new HttpHost("asdf"), new HttpHost("jkll") }, new AscendingSortAggregator<string>()).Result;
            //var nolo = main.MulticastAsync(
            //    new List<HttpRequestInstructions<string>>
            //    {
            //        new HttpRequestInstructions<string>(
            //            new HttpRequestMessageWrapper {RequestUri = new Uri("http://arrrrrrrdfasdfasdfuioui.com:8080")},
            //            TimeSpan.FromMinutes(5), ResponseTransformer, Guid.Empty, new HttpClientManager())
            //    }.ToArray(), TimeSpan.FromMinutes(5), new DescendingSortAggregator<string>()).Result;
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
            return new ResultEnvelope<string>()
            {
                Result = arg.Content.ReadAsStringAsync().Result,
                Sequence = 0,
                SortWeight = 1
            };
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
}
