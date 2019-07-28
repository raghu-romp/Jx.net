using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.ValueMap.BuildIn
{
    public class ToUpper : IValuePipe
    {
        public string Name => "ToUpper";

        public dynamic Process(dynamic fromValue) {

            if (!fromValue is string) {
                throw new ArgumentException($"{nameof(fromValue)} should be a string value");
            }

            return ((string)fromValue).ToUpper();
        }
    }

    public class ToLower : IValuePipe
    {
        public string Name => "ToLower";

        public dynamic Process(dynamic fromValue) {

            if (!fromValue is string) {
                throw new ArgumentException($"{nameof(fromValue)} should be a string value");
            }

            return ((string)fromValue).ToLower();
        }
    }
}
