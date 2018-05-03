using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScatterGather.Request;
using ScatterGather.Aggregation;
using ScatterGather.Workloads;

namespace ScatterGather
{
    public class WorkloadManager : IWorkload
    {
        public Task<List<TRslt>> MulticastAsync<TRslt, TReq, TRspns>(AbstractRequestInstructions<TRslt, TReq, TRspns>[] lstRequests,
            TimeSpan gatherTimeout, IAggregator<TRslt> gatherer) where TReq: IRequest
        {
            if (GetTypeOfWorkload(lstRequests) == WorkloadType.Http)
            {
                return new HttpWorkload().DoSeveralHttpRequestsAsync(lstRequests.Select(x => x as HttpRequestInstructions<TRslt>).ToList(), gatherTimeout, gatherer);
            }
            throw new NotImplementedException("Unable to parse the type of workload");
        }

        public async Task<List<TRslt>> MulticastAsync<TRslt, TReq, TRspns>(AbstractRequestInstructions<TRslt, TReq, TRspns> request,
            List<IHost> hosts, IAggregator<TRslt> gatherer) where TReq: IRequest
        {
            if (GetTypeOfWorkload(request) == WorkloadType.Http)
            {
                if (hosts?.All(x => x.GetType() == typeof(HttpHost)) ?? false)
                {
                    return await (new HttpWorkload().DoHttpRequestForSeveralRecipientsAsync(request as HttpRequestInstructions<TRslt>,
                        hosts.Select(x => new Uri(x.Host.ToString())).ToList(), gatherer));
                }
            }
            else
            {
                return await (new BaseWorkload<TRslt, TReq, TRspns>().DoAsync(request, hosts, gatherer).ConfigureAwait(false));
            }
            throw new NotImplementedException("Unable to parse the type of workload");
        }

        private WorkloadType GetTypeOfWorkload<TRslt, TReq, TRspns>(IEnumerable<AbstractRequestInstructions<TRslt, TReq, TRspns>> lstRequests) where TReq: IRequest
        {
            var abstractRequestInstructionses = lstRequests.ToList();
            if (abstractRequestInstructionses.Any())
            {
                return GetTypeOfWorkload(abstractRequestInstructionses.First());
            }
            return WorkloadType.Unsupported;
        }

        private WorkloadType GetTypeOfWorkload<TRslt, TReq, TRspns>(AbstractRequestInstructions<TRslt, TReq, TRspns> request) where TReq: IRequest
        {
            if (request is HttpRequestInstructions<TRslt>)
            {
                return WorkloadType.Http;
            }
            return WorkloadType.Unsupported;
        }
    }

    public enum WorkloadType
    {
        Unsupported,
        Http
    }
}
