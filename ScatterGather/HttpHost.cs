using System.Collections.Generic;

namespace ScatterGather
{
    public class HttpHost : IHost
    {
        public HttpHost(string host)
        {
            Host = host;
        }
        public object Host { get; private set; }
        public new bool Equals(object x, object y)
        {
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(object obj)
        {
            return Host.GetHashCode();
        }
    }

    public interface IHost: IEqualityComparer<object>
    {
        object Host { get; }
    }
}
