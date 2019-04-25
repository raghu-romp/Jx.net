using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.ValueMap
{
    public delegate dynamic LambdaValuePipeFunc(dynamic source, params string[] args);

    public class LambdaPipe : IValuePipe
    {
        public string Name { get; private set; }
        public LambdaValuePipeFunc MapFunction { get; set; }

        public LambdaPipe(string name, LambdaValuePipeFunc func)
        {
            this.Name = name;
            this.MapFunction = func;
        }

        public dynamic Process(dynamic fromValue)
        {
            return this.MapFunction(fromValue);
        }
    }
}
