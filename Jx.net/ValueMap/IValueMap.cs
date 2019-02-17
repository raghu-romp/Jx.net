using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net
{
    public interface IValueMap
    {
        string MappingName { get; }
        dynamic MapValue(string fromValue);
    }
}
