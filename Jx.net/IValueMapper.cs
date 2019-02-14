using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net
{
    public interface IValueMapper<FT, TT>
    {
        IDictionary<FT,TT> ValueMapping { get; }
        TT Map(FT value);
    }

    public interface IValueMapper<T> : IValueMapper<T, T>
    {
    }
}
