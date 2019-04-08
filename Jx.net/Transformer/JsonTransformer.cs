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
        internal const int MaxIterations = 100;

        private Context rootCtx;
        private Context currentCtx;

        public bool SuppressErrors { get; set; } = false;

        private readonly Dictionary<string, Context> namedContext = new Dictionary<string, Context>();
        
        public JToken Transform(JToken source, JToken transformer)
        {
            rootCtx = new Context { Node = source };
            var output = RenderNode(source, transformer, rootCtx);
            return output;
        }

        private JToken RenderNode(JToken source, JToken transformer, Context context)
        {
            var output = transformer.DeepClone();
            currentCtx = context;

            JArray forEachArray = null;
            int iterations = 0;
            do {
                forEachArray = output.FindForEach(out var expression);
                
                if (forEachArray != null) {
                    RenderForEach(forEachArray, forEachArray.Parent, expression);
                }
            } while (forEachArray != null && iterations++ < MaxIterations);

            output.AllStrings((value, token, parent) => {
                if (parent.Type == JTokenType.Property) {
                    ((JProperty)parent).Value = Resolve(value);
                }
            });

            return output;
        }

        private void RenderForEach(JArray jsForArray, JToken parentToken, string expression)
        {
            var parsedExpression = Patterns.JxFor.Match(expression);
            var jPath = parsedExpression.Groups["placeholder"].Value;
            var alias = parsedExpression.Groups["alias"].Success ? parsedExpression.Groups["alias"].Value : "";
            var sourceArray = this.currentCtx.Node.SelectToken(jPath, !SuppressErrors) as JArray;

            var template = jsForArray[1];
            jsForArray.RemoveAll();

            if (sourceArray != null)
            {
                var outerContext = currentCtx;
                for (int index = 0; index < sourceArray.Count; index++)
                {
                    var iterationNode = sourceArray[index];
                    var newContext = new Context { index = index, Node = iterationNode, Parent = outerContext };
                    var rendered = RenderNode(iterationNode, template, newContext);
                    jsForArray.Add(rendered);
                }

                currentCtx = outerContext;
            }
        }

        internal dynamic Resolve(string str)
        {
            return Patterns.Interpolation.Replace(str, match => {
                var placeholder = match.Groups["placeholder"].Value;
                var ctx = FindContext(placeholder, out var jPath);

                if (ctx == null || !ctx.Node.TrySelectToken(jPath, out var matchToken)) {
                    return $"[{placeholder}]";
                }

                var val = string.Empty;
                val = matchToken.Value<dynamic>();

                return val;
            });
        }

        private Context FindContext(string placeholder, out string jPath)
        {
            var parts = placeholder.Split('.');
            var firstPart = placeholder.SplitFirst(".", out var rest);
            Context context = null;

            bool Check(string prefix, Context c) {
                if (firstPart == prefix) {
                    context = c;
                    return true;
                }
                return false;
            }

            bool CheckNamed(string name) {
                return this.namedContext.TryGetValue(name, out context);
            }

            if (Check("$", rootCtx) || Check("@ctx", currentCtx) || CheckNamed(firstPart)) {
                jPath = string.Join(".", new []{ "$", rest });
            } else {
                jPath = placeholder;
            }

            return context;
        }
    }
}
