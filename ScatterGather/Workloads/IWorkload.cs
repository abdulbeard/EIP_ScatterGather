using ScatterGather.Aggregation;
using ScatterGather.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScatterGather.Workloads
{
    public interface IWorkload
    {
        Task<List<Rslt>> MulticastAsync<Rslt, Req, Rspns>(AbstractRequestInstructions<Rslt, Req, Rspns>[] lstRequests,
            TimeSpan gatherTimeout, IAggregator<Rslt> gatherer) where Req: IRequest;

        Task<List<Rslt>> MulticastAsync<Rslt, Req, Rspns>(AbstractRequestInstructions<Rslt, Req, Rspns> request,
            List<string> hosts, IAggregator<Rslt> gatherer) where Req: IRequest;
    }
}
