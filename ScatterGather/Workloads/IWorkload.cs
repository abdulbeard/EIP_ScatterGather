using ScatterGather.Aggregation;
using ScatterGather.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScatterGather.Workloads
{
    public interface IWorkload
    {
        Task<List<TRslt>> MulticastAsync<TRslt, TReq, TRspns>(AbstractRequestInstructions<TRslt, TReq, TRspns>[] lstRequests,
            TimeSpan gatherTimeout, IAggregator<TRslt> gatherer) where TReq: IRequest;

        Task<List<TRslt>> MulticastAsync<TRslt, TReq, TRspns>(AbstractRequestInstructions<TRslt, TReq, TRspns> request,
            List<IHost> hosts, IAggregator<TRslt> gatherer) where TReq: IRequest;
    }
}
