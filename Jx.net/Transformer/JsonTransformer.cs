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

        public JToken Transform(JToken source, JToken transformer)
        {
            var root = source;
            var output = transformer.DeepClone();

            output.AllStringProperties((value, property, parent) => {
                property.Value = Resolve(value, root);
            });

            return output;
        }

        public dynamic Resolve(string str, JToken source)
        {
            var regex = new Regex(string.Format(@"{0}(.*?){1}", MESSAGE_ID_TOKEN_START, MESSAGE_ID_TOKEN_END), RegexOptions.Compiled);
            return regex.Replace(str, match => {
                var matchToken = source.SelectToken(match.Groups[1].Value);
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
