using ScatterGather.Aggregation;
using ScatterGather.Request;
using ScatterGather.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterGather.Workloads
{
    public class HttpWorkload
    {
        private IClientManager<HttpResponseMessage, HttpRequestMessageWrapper> clientManager;
        public HttpWorkload(IClientManager<HttpResponseMessage, HttpRequestMessageWrapper> hcm = null)
        {
            clientManager = hcm ?? new HttpClientManager();
        }

        public async Task<List<T>> DoHttpRequestForSeveralRecipientsAsync<T>(HttpRequestInstructions<T> request, List<Uri> hosts, IAggregator<T> gatherer)
        {
            var listResults = new List<ResultEnvelope<T>>();
            var listTasksToAwait = new List<Task>();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(request.Timeout);
            foreach (var host in hosts)
            {
                var client = clientManager.GetClient(new HttpHost(host.OriginalString));
                listTasksToAwait.Add(client.SendAsync(request.Request, cts.Token).ContinueWith(
                    x => listResults.Add(request.ResponseTransformer(x.Result)), cts.Token));
            }
            await Task.WhenAll(listTasksToAwait).ConfigureAwait(false);
            var result = gatherer.Aggregate(listResults);
            return result.Select(x => x.Result).ToList();

        }

        public async Task<List<T>> DoSeveralHttpRequestsAsync<T>(List<HttpRequestInstructions<T>> requests, TimeSpan gatherTimeout, IAggregator<T> gatherer)
        {
            //TODO null checks all around
            var listResults = new List<ResultEnvelope<T>>();
            var listTasksToAwait = new List<Task>();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(gatherTimeout);
            foreach (var request in requests)
            {
                var client = clientManager.GetClient(new HttpHost(request.Request.RequestUri.ToString()));
                if (client != null)
                {
                    listTasksToAwait.Add(client.SendAsync(request.Request, cts.Token).ContinueWith(
                        x => listResults.Add(request.ResponseTransformer(x.Result)), cts.Token));
                }
            }
            await Task.WhenAll(listTasksToAwait).ConfigureAwait(false);
            var result = gatherer.Aggregate(listResults);
            return result.Select(x => x.Result).ToList();

        }
    }
}
