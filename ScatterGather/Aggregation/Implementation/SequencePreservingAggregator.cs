using ScatterGather.Result;
using System.Collections.Generic;
using System.Linq;

namespace ScatterGather.Aggregation.Implementation
{
    public class SequencePreservingAggregator<T> : IAggregator<T>
    {
        public List<ResultEnvelope<T>> Aggregate(IReadOnlyList<ResultEnvelope<T>> results)
        {
            return results.OrderBy(x => x.Sequence).ToList();
        }
    }
}
