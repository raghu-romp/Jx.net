using Jx.net.Transformer;
using Jx.net.ValueMap.BuiltIn;
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
            jx.pipes.Add(nameof(ValueMap.BuiltIn.ToString), new ToString());
            jx.pipes.Add(nameof(ValueMap.BuiltIn.ToUpper), new ToUpper());
            jx.pipes.Add(nameof(ValueMap.BuiltIn.ToLower), new ToLower());
            jx.pipes.Add(nameof(ToBool), new ToBool());
            jx.pipes.Add(nameof(ToInt), new ToInt());
        }
    }
}
