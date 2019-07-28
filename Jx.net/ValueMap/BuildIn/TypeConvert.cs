using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.ValueMap.BuildIn
{
    public class ToString : IValuePipe
    {
        public string Name => "ToString";

        public dynamic Process(dynamic fromValue) {
            return fromValue == null ?
                string.Empty : fromValue.ToString();
        }
    }
}
