﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.ValueMap.BuiltIn
{
    public class BoolYesNo : IValuePipe
    {
        public string Name => nameof(BoolYesNo);

        public dynamic Process(dynamic fromValue) {
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
