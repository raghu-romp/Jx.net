using Jx.net.Transformer;
using Jx.net.ValueMap.BuildIn;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net
{
    public static class JxFactory
    {
        public static IJsonTransformer Create() {
            var jx = new JsonTransformer();
            RegisterBuiltInMappers(jx);

            return jx;
        }

        private static void RegisterBuiltInMappers(JsonTransformer jx) {
            jx.Mappers.Add(nameof(BoolYesNo), new BoolYesNo());
        }
    }
}
