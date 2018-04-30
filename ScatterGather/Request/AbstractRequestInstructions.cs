using ScatterGather.Result;
using System;

namespace ScatterGather.Request
{
    public abstract class AbstractRequestInstructions<TRslt, TRqst, TRspns> where TRqst : IRequest
    {
        public abstract TRqst Request { get; }
        public abstract TimeSpan Timeout { get; }
        public abstract Func<TRspns, ResultEnvelope<TRslt>> ResponseTransformer { get; }
        public abstract Guid Id { get; }
        public abstract IClientManager<TRspns, TRqst> ClientManager { get; }
    }
}
