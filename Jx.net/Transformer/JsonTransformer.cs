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

        private Context root;
        private Context currentCtx;

        public bool SuppressErrors { get; set; } = false;

        private readonly Dictionary<string, Context> namedContext = new Dictionary<string, Context>();
        
        public JToken Transform(JToken source, JToken transformer)
        {
            root = new Context { Node = source };
            var output = RenderNode(transformer, root);
            return output;
        }

        private JToken RenderNode(JToken transformer, Context context) {
            var output = transformer.DeepClone();
            currentCtx = context;

            ProcessForEachTemplates(output);
            ProcessIfTemplates(output);
            output = ResolveInterpolations(output);

            return output;
        }

        private JToken ResolveInterpolations(JToken output) {
            output.AllStrings((value, token, parent) => {
                var resolvedValue = Resolve(value);
                if (parent != null) {
                    if (parent.Type == JTokenType.Property)
                        ((JProperty)parent).Value = resolvedValue;
                } else {
                    output = JToken.FromObject(resolvedValue);
                }
            });
            return output;
        }

        private void ProcessIfTemplates(JToken output) {
            var iterations = 0;
            JArray ifTemplate;
            do {
                ifTemplate = output.FindForEach(out var expression);

                if (ifTemplate != null) {
                    RenderForEach(ifTemplate, expression);
                }
            } while (ifTemplate != null && iterations++ < MaxIterations);
        }

        private void ProcessForEachTemplates(JToken output) {
            var iterations = 0;
            JArray forEachArray;
            do {
                forEachArray = output.FindForEach(out var expression);

                if (forEachArray != null) {
                    RenderForEach(forEachArray, expression);
                }
            } while (forEachArray != null && iterations++ < MaxIterations);
        }

        private void RenderForEach(JArray jsForArray, string expression) {
            var parsedExpression = Patterns.JxFor.Match(expression);
            var query = parsedExpression.Groups["query"].Value;
            var alias = parsedExpression.Groups["alias"].Success ? parsedExpression.Groups["alias"].Value : "";
            var context = FindContext(query, out var jPath);
            var tokens = context.Node.SelectTokens(jPath, !SuppressErrors);

            if (tokens == null) {
                return;
            }

            var sourceArray = tokens.ConvertToJArray();

            var template = jsForArray[1];
            jsForArray.RemoveAll();

            if (sourceArray != null) {
                var outerContext = currentCtx;
                for (int index = 0; index < sourceArray.Count; index++) {
                    var iterationNode = sourceArray[index];
                    var newContext = Context.New(iterationNode, outerContext, index);
                    this.namedContext[alias] = newContext;
                    var rendered = RenderNode(template, newContext);
                    jsForArray.Add(rendered);
                }

                currentCtx = outerContext;
                this.namedContext.Remove(alias);
            }
        }

        internal dynamic Resolve(string str)
        {
            return Patterns.Interpolation.Replace(str, match => {
                var query = match.Groups["query"].Value;
                var ctx = FindContext(query, out var jPath);

                if (ctx == null || !ctx.Node.TrySelectToken(jPath, out var matchToken)) {
                    return $"[{query}]";
                }

                var val = string.Empty;
                val = matchToken.Value<dynamic>();

                return val;
            });
        }

        private Context FindContext(string query, out string jPath)
        {
            var firstPart = query.SplitFirst(".", out var rest);
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

            if (Check("$", root) || Check("@ctx", currentCtx) || CheckNamed(firstPart)) {
                jPath = string.Join(".", new []{ "$", rest });
            } else {
                jPath = query;
            }

            return context;
        }
    }
}
