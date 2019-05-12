using Jx.net.Transformer;
using Jx.net.ValueMap.BuildIn;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net
{
    public static class JxFactory
    {
        public static IJsonTransformer Create(TransformOptions options = null) {
            if (options == null) {
                options = new TransformOptions();
            }
            var jx = new JsonTransformer(options);
            RegisterBuiltInMappers(jx);

            return jx;
        }

        private static void RegisterBuiltInMappers(JsonTransformer jx) {
            jx.pipes.Add(nameof(BoolYesNo), new BoolYesNo());
        }
    }
}
