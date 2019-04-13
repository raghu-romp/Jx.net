using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.ValueMap
{
    public class LambdaPipe : IValuePipe
    {
        public string MapperName { get; }
        public Func<dynamic, string> MapFunction { get; set; }

        public LambdaPipe(string name, Func<dynamic, string> func)
        {
            this.MapFunction = func;
        }

        public dynamic MapValue(dynamic fromValue)
        {
            return this.MapFunction(fromValue);
        }
    }
}
