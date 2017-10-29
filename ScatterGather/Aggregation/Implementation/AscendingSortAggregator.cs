using ScatterGather.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScatterGather.Aggregation.Implementation
{
    public class AscendingSortAggregator<T> : IAggregator<T>
    {
        public List<ResultEnvelope<T>> Aggregate(IReadOnlyList<ResultEnvelope<T>> results)
        {
            return results.OrderBy(x => x.SortWeight).ToList();
        }
    }
}
