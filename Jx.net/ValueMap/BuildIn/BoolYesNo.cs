using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.ValueMap.BuildIn
{
    public class BoolYesNo : IValuePipe
    {
        public string MapperName => nameof(BoolYesNo);

        public dynamic MapValue(dynamic fromValue) {
            if (fromValue is null) {
                return null;
            }

            if (fromValue is bool) {
                return (bool)fromValue
                ? "Yes" : "No";
            }

            return "*Can't infer*";
        }
    }
}
