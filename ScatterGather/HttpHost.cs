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

    /// <summary>
    /// Uniquely represents a host that will receive a message via scatter
    /// </summary>
    public interface IHost: IEqualityComparer<object>
    {
        object Host { get; }
    }
}
