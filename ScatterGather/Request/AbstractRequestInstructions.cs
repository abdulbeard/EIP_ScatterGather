using ScatterGather.Result;
using System;

namespace ScatterGather.Request
{
    public abstract class AbstractRequestInstructions<Rslt, Rqst, Rspns> where Rqst : IRequest
    {
        public abstract Rqst Request { get; }
        public abstract TimeSpan Timeout { get; }
        public abstract Func<Rspns, ResultEnvelope<Rslt>> ResponseTransformer { get; }
        public abstract Guid Id { get; }
        public abstract IClientManager<Rspns, Rqst> ClientManager { get; }
    }
}
