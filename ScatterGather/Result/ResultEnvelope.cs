using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterGather.Result
{
    public class ResultEnvelope<T>
    {
        public int Sequence { get; set; }
        public int SortWeight { get; set; }
        public T Result { get; set; }
    }
}
