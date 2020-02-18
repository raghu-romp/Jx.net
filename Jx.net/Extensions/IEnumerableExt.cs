using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var e in ie) action(e);
        }
    }
}
