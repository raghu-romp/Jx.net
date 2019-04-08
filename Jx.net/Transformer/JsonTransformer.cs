using Jx.net.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Jx.net.Transformer
{
    public class JsonTransformer
    {
        private const string MESSAGE_ID_TOKEN_START = @"::";
        private const string MESSAGE_ID_TOKEN_END = @"::";
        private Context rootCtx;
        private Context currentCtx;
        
        public JToken Transform(JToken source, JToken transformer)
        {
            rootCtx = new Context { Node = source };
            var output = transformer.DeepClone();

            RenderNode(source, transformer, rootCtx);

            output.AllStrings((value, token, parent) => {
                if (value.StartsWith("*")) {
                    
                }
                if (parent.Type == JTokenType.Property)
                {
                    ((JProperty)parent).Value = Resolve(value, rootCtx);
                }
            });

            return output;
        }

        private void RenderNode(JToken source, JToken transformer, Context context)
        {
            JArray forEachArray = null;
            currentCtx = context;
            int iterations = 0;
            do
            {
                forEachArray = transformer.FindForEach(out var expression);
                if (forEachArray != null)
                {
                    RenderForEach(forEachArray, forEachArray.Parent, expression);
                    iterations++;
                }
            } while (forEachArray != null && iterations < 100);
            
        }

        private void RenderForEach(JArray arrayTemplate, JToken parentToken, string forEachExpression)
        {
            
        }

        internal dynamic Resolve(string str, Context ctx)
        {
            var regex = new Regex(string.Format(@"{0}(.*?){1}", MESSAGE_ID_TOKEN_START, MESSAGE_ID_TOKEN_END), RegexOptions.Compiled);
            return regex.Replace(str, match => {
                var matchToken = ctx.Node.SelectToken(match.Groups[1].Value);
                if (matchToken == null) {
                    return $"[{match.Groups[1].Value}]";
                }


                var val = string.Empty;
                val = matchToken.Value<dynamic>();

                return val;
            });
        }
    }
}
