using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.ValueMap
{
    public class LambdaValueMap : IValueMap
    {
        public string MappingName { get; }
        public Func<dynamic, string> MapFunction { get; set; }

        public LambdaValueMap(string name, Func<dynamic, string> func)
        {
            this.MapFunction = func;
        }

        public dynamic MapValue(string fromValue)
        {
            return this.MapFunction(fromValue);
        }
    }
}
