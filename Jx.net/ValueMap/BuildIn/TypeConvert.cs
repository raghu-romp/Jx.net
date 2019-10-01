using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.ValueMap.BuiltIn
{
    public class ToString : IValuePipe
    {
        public string Name => nameof(BuiltIn.ToString);

        public dynamic Process(dynamic fromValue) {
            return fromValue == null ?
                string.Empty : fromValue.ToString();
        }
    }

    public class ToBool : IValuePipe
    {
        public string Name => nameof(ToBool);

        public dynamic Process(dynamic fromValue) {
            if (!(fromValue is string)) {
                fromValue = fromValue.ToString();
            }

            return string.Equals(fromValue, "true", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
