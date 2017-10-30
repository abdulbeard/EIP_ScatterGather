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
    public class BaseWorkload<Rslt, Req, Rspns> where Req : IRequest
    {
        public async Task<List<Rslt>> DoAsync(AbstractRequestInstructions<Rslt, Req, Rspns> request,
            List<IHost> hosts, IAggregator<Rslt> gatherer)
        {
            if (request.ClientManager == null)
            {
                throw new ArgumentNullException($"{nameof(request.ClientManager)}");
            }
            var listResults = new List<ResultEnvelope<Rslt>>();
            var taskBag = new List<Task>();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(request.Timeout);
            foreach (var host in hosts)
            {
                var client = request.ClientManager.GetClient(host);
                if (client != null)
                {
                    taskBag.Add(client.SendAsync(request.Request, cts.Token).ContinueWith(x => listResults.Add(request.ResponseTransformer(x.Result))));
                }
            }
            await Task.WhenAll(taskBag).ConfigureAwait(false);
            var result = gatherer.Aggregate(listResults);
            return result.Select(x => x.Result).ToList();
        }

        public async Task<List<Rslt>> DoAsync(IEnumerable<AbstractRequestInstructions<Rslt, Req, Rspns>> requests, TimeSpan gatherTimeout, IAggregator<Rslt> gatherer)
        {
            if (!requests?.Any() ?? true || gatherTimeout == TimeSpan.MinValue || gatherTimeout == null)
            {
                throw new ArgumentNullException($"{nameof(requests)} or {nameof(gatherTimeout)} or {nameof(gatherer)}");
            }
            var taskBag = new List<Task>();
            var listResults = new List<ResultEnvelope<Rslt>>();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(gatherTimeout);
            foreach (var request in requests)
            {
                var client = request.ClientManager.GetClient(request.Request.Host);
                if (client != null)
                {
                    taskBag.Add(client.SendAsync(request.Request, cts.Token).ContinueWith(x => listResults.Add(request.ResponseTransformer(x.Result))));
                }
            }
            var nowPlusGatherTimeout = DateTime.Now.Add(gatherTimeout);
            while (!cts.Token.IsCancellationRequested || nowPlusGatherTimeout < DateTime.Now)
            {
                await Task.Delay(100);
            }
            var result = gatherer.Aggregate(listResults);
            return result.Select(x => x.Result).ToList();
        }
    }
}
