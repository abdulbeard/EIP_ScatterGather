namespace ScatterGather.Result
{
    public class ResultEnvelope<T>
    {
        /// <summary>
        /// Used for sequence-preserving aggregation. Signifies the sequence of this result in a list of results
        /// </summary>
        public int Sequence { get; set; }
        /// <summary>
        /// Used for sorting aggregator. Signifies the sorting weight to use for ascending/descending sort
        /// </summary>
        public int SortWeight { get; set; }
        /// <summary>
        /// The result object
        /// </summary>
        public T Result { get; set; }
    }
}
