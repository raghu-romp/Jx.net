using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.Extensions
{
    public static class StringExtensions
    {
        public static string SplitFirst(this string str, string separator, out string rest)
        {
            var index = str.IndexOf(separator);
            if (index < 0) {
                rest = string.Empty;
                return str;
            }

            var firstPart = str.Substring(0, index);
            rest = str.Substring(index + separator.Length);

            return firstPart;
        }
    }
}
