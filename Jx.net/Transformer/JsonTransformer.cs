using Jx.net.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jx.net.Transformer
{
    public class JsonTransformer : IJsonTransformer
    {
        public bool SuppressErrors { get; set; } = false;
        internal Dictionary<string, IValuePipe> Mappers { get; private set; } = new Dictionary<string, IValuePipe>();

        private const int MaxIterations = 100;

        private Context root;
        private Context currentCtx;
        private readonly Dictionary<string, Context> namedContext = new Dictionary<string, Context>();

        public JToken Transform(JToken source, JToken transformer) {
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
                ifTemplate = output.FindIfTemplate(out var match);

                if (ifTemplate != null) {
                    RenderIfTemplate(ifTemplate, ifTemplate.Parent, match);
                }
            } while (ifTemplate != null && iterations++ < MaxIterations);
        }

        private void ProcessForEachTemplates(JToken output) {
            var iterations = 0;
            JArray forEachArray;
            do {
                forEachArray = output.FindForTemplate(out var match);

                if (forEachArray != null) {
                    RenderForTemplate(forEachArray, match);
                }
            } while (forEachArray != null && iterations++ < MaxIterations);
        }

        private void RenderIfTemplate(JArray ifTemplate, JToken parent, Match match) {
            var query = match.Groups["query"].Value;
            var condition = match.Groups["condition"].Value;
            var context = FindContext(query, out var jPath);
            var queryTokens = context.Node.SelectTokens(jPath);
            var exists = queryTokens.Any();
            var truthy = (exists && condition == "exists") || (!exists && condition == "not-exists");
            var template = truthy ? ifTemplate[1]
                : ifTemplate.Count > 2 ? ifTemplate[2]
                : null;
            if (template == null) {
                if (ifTemplate.Parent.Type == JTokenType.Property)
                    ifTemplate.Parent.Remove();
            } else {
                var rendered = RenderNode(template, currentCtx);
                if (ifTemplate.Parent.Type == JTokenType.Property)
                    ((JProperty)ifTemplate.Parent).Value = rendered;
            }
        }

        private void RenderForTemplate(JArray jsForArray, Match match) {
            var query = match.Groups["query"].Value;
            var alias = match.Groups["alias"].Value;
            var context = FindContext(query, out var jPath);
            var tokens = context.Node.SelectTokens(jPath, !SuppressErrors);

            if (tokens == null) {
                return;
            }

            var sourceArray = tokens.FlattenArray();

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

        internal dynamic Resolve(string str) {
            var match = Patterns.Interpolation.Match(str);
            if (match.Success && match.Value == str) {
                return Interpolate(match);
            }
            else {
                return Patterns.Interpolation.Replace(str, m => {
                    var val = Interpolate(m);
                    return val is null ? string.Empty : val.ToString();
                });
            }
        }

        private dynamic Interpolate(Match match) {
            var query = match.Groups["query"].Value;
            var pipes = GetPipes(match);
            var ctx = FindContext(query, out var jPath);

            if (ctx == null || !ctx.Node.TrySelectToken(jPath, out var matchToken)) {
                return $"[{query}]";
            }

            var val = matchToken.Value<dynamic>();
            if (val is JValue) {
                val = ((JValue)val).Value;
            }
            val = ProcessIntoPipes(pipes, val);

            return val;
        }

        private dynamic ProcessIntoPipes(List<string> pipeNames, dynamic val) {

            pipeNames.ForEach(pipeName => {
                if (!this.Mappers.TryGetValue(pipeName, out var pipe)) {
                    throw new NullReferenceException($"{pipeName} not a registered {nameof(IValuePipe)}");
                }

                val = pipe.MapValue(val);
            });

            return val;
        }

        private List<string> GetPipes(Match match) {
            var pipes = new List<string>();
            if (match.Groups["pipes"].Success) {
                foreach (Capture capture in match.Groups["pipe"].Captures) {
                    pipes.Add(capture.Value);
                }
            }

            return pipes;
        }

        private Context FindContext(string query, out string jPath) {
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

            if (Check("$", root) || CheckNamed(firstPart)) {
                jPath = string.Join(".", new[] { "$", rest });
            } else {
                jPath = query;
            }

            return context;
        }
    }
}
