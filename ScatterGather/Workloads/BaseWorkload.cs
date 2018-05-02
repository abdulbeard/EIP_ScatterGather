using ScatterGather.Aggregation;
using ScatterGather.Request;
using ScatterGather.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterGather.Workloads
{
    public class BaseWorkload<TRslt, TReq, TRspns> where TReq : IRequest
    {
        public async Task<List<TRslt>> DoAsync(AbstractRequestInstructions<TRslt, TReq, TRspns> request,
            List<IHost> hosts, IAggregator<TRslt> gatherer)
        {
            if (request.ClientManager == null)
            {
                throw new ArgumentNullException($"{nameof(request.ClientManager)}");
            }
            var listResults = new List<ResultEnvelope<TRslt>>();
            var taskBag = new List<Task>();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(request.Timeout);
            foreach (var host in hosts)
            {
                var client = request.ClientManager.GetClient(host);
                if (client != null)
                {
                    taskBag.Add(client.SendAsync(request.Request, cts.Token)
                        .ContinueWith(x => listResults.Add(request.ResponseTransformer(x.Result)), cts.Token));
                }
            }
            await Task.WhenAll(taskBag).ConfigureAwait(false);
            var result = gatherer.Aggregate(listResults);
            return result.Select(x => x.Result).ToList();
        }

        public async Task<List<TRslt>> DoAsync(IEnumerable<AbstractRequestInstructions<TRslt, TReq, TRspns>> requests, TimeSpan gatherTimeout, IAggregator<TRslt> gatherer)
        {
            var requestsList = requests?.ToList();
            if (requestsList == null || requestsList.Count <= 0 || gatherTimeout == TimeSpan.MinValue ||
                gatherTimeout == null)
            {
                throw new ArgumentNullException($"{nameof(requests)} or {nameof(gatherTimeout)} or {nameof(gatherer)}");
            }

            var taskBag = new List<Task>();
            var listResults = new List<ResultEnvelope<TRslt>>();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(gatherTimeout);
            foreach (var request in requestsList)
            {
                var client = request.ClientManager.GetClient(request.Request.Host);
                if (client != null)
                {
                    taskBag.Add(client.SendAsync(request.Request, cts.Token)
                        .ContinueWith(x => listResults.Add(request.ResponseTransformer(x.Result)), cts.Token));
                }
            }
            var nowPlusGatherTimeout = DateTime.Now.Add(gatherTimeout);
            while (!cts.Token.IsCancellationRequested || nowPlusGatherTimeout < DateTime.Now)
            {
                await Task.Delay(100, cts.Token);
            }
            var result = gatherer.Aggregate(listResults);
            return result.Select(x => x.Result).ToList();
        }
    }
}
