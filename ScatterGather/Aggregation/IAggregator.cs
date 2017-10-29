using ScatterGather.Result;
using System.Collections.Generic;

namespace ScatterGather.Aggregation
{
    public interface IAggregator<T>
    {
        List<ResultEnvelope<T>> Aggregate(IReadOnlyList<ResultEnvelope<T>> results);
    }
}
