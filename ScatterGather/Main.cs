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
        public Task<List<Rslt>> MulticastAsync<Rslt, Req, Rspns>(AbstractRequestInstructions<Rslt, Req, Rspns>[] lstRequests,
            TimeSpan gatherTimeout, IAggregator<Rslt> gatherer) where Req: IRequest
        {
            if (GetTypeOfWorkload(lstRequests) == WorkloadType.Http)
            {
                return new HttpWorkload().DoSeveralHttpRequestsAsync(lstRequests.Select(x => x as HttpRequestInstructions<Rslt>).ToList(), gatherTimeout, gatherer);
            }
            throw new NotImplementedException("Unable to parse the type of workload");
        }

        public async Task<List<Rslt>> MulticastAsync<Rslt, Req, Rspns>(AbstractRequestInstructions<Rslt, Req, Rspns> request,
            List<IHost> hosts, IAggregator<Rslt> gatherer) where Req: IRequest
        {
            if (GetTypeOfWorkload(request) == WorkloadType.Http)
            {
                if (hosts?.All(x => x.GetType() == typeof(HttpHost)) ?? false)
                {
                    return await (new HttpWorkload().DoHttpRequestForSeveralRecipientsAsync(request as HttpRequestInstructions<Rslt>,
                        hosts.Select(x => new Uri(x.Host.ToString())).ToList(), gatherer));
                }
            }
            else
            {
                return await (new BaseWorkload<Rslt, Req, Rspns>().DoAsync(request, hosts, gatherer).ConfigureAwait(false));
            }
            throw new NotImplementedException("Unable to parse the type of workload");
        }

        private WorkloadType GetTypeOfWorkload<Rslt, Req, Rspns>(IEnumerable<AbstractRequestInstructions<Rslt, Req, Rspns>> lstRequests) where Req: IRequest
        {
            if (lstRequests?.Any() ?? false)
            {
                return GetTypeOfWorkload(lstRequests.First());
            }
            return WorkloadType.Unsupported;
        }

        private WorkloadType GetTypeOfWorkload<Rslt, Req, Rspns>(AbstractRequestInstructions<Rslt, Req, Rspns> request) where Req: IRequest
        {
            if (request != null && request is HttpRequestInstructions<Rslt>
                && request.ClientManager != null && request.ClientManager is HttpClientManager)
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
