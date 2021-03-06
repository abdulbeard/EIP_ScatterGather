﻿using System;
using System.Collections.Generic;
using ScatterGather.Result;

namespace ScatterGather.Aggregation.Implementation
{
    public class ExpressionEvaluatingAggregator<T> : IAggregator<T>
    {
        private Func<IReadOnlyList<ResultEnvelope<T>>, List<ResultEnvelope<T>>> evaluator;
        public ExpressionEvaluatingAggregator(Func<IReadOnlyList<ResultEnvelope<T>>, List<ResultEnvelope<T>>> expression)
        {
            evaluator = expression;
        }

        public List<ResultEnvelope<T>> Aggregate(IReadOnlyList<ResultEnvelope<T>> results)
        {
            return evaluator(results);
        }
    }
}
