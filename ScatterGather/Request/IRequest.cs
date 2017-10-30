using System.Collections.Generic;

namespace ScatterGather.Request
{
    public interface IRequest
    {
        IHost Host { get; }
    }
}
