using Jx.net.Extensions;
using Jx.net.Formulas;
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
        internal Dictionary<string, IValuePipe> pipes = new Dictionary<string, IValuePipe>();
        public TransformOptions Options { get; set; }

        private Context root;
        private Context currentCtx;
        private readonly Dictionary<string, Context> namedContext = new Dictionary<string, Context>();

        private readonly IFormulaSolver formulaSolver = new FormulaSolver();

        internal JsonTransformer(TransformOptions options) {
            this.Options = options;
        }

        public JToken Transform(JToken source, JToken transformer) {
            root = new Context { Node = source };
            var output = RenderNode(transformer, root);
            return output;
        }

        public void AddPipe(IValuePipe pipe) {
            this.pipes.Add(pipe.Name, pipe);
        }

        private JToken RenderNode(JToken transformer, Context context) {
            var output = transformer.DeepClone();
            currentCtx = context;

            ProcessBlockStatements(output);

            output = ResolveInterpolations(output);

            return output;
        }

        private void ProcessBlockStatements(JToken output) {
            var iterations = 0;
            JArray blockStatement;
            do {
                blockStatement = output.FindBlockStatement(out var match);
                if (blockStatement != null) {
                    RenderBlockTemplate(blockStatement, match);
                }
            } while (blockStatement != null && iterations++ < Options.MaxIterations);
        }

        private void RenderBlockTemplate(JArray blockStatement, Match match) {
            var blockType = match.Groups["blocktype"].Value;
            if (blockType == "if" && Patterns.JxIf.IsMatch(match.Value, out var ifMatch)) {
                RenderIfTemplate(blockStatement, blockStatement.Parent, ifMatch);
            } else if (blockType == "for" && Patterns.JxFor.IsMatch(match.Value, out var forMatch)) {
                RenderForTemplate(blockStatement, forMatch);
            } else {
                throw new InvalidOperationException($"Unable to process the statement {match.Value}");
            }
        }

        private JToken ResolveInterpolations(JToken output) {
            output.AllStrings((value, token, parent) => {
                var resolvedValue = Resolve(value);
                if (parent != null) {
                    if (parent.Type == JTokenType.Property)
                        ((JProperty)parent).Value = resolvedValue;
                } else {
                    output = resolvedValue == null 
                            ? null 
                            : JToken.FromObject(resolvedValue);
                }
            });
            return output;
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
            var tokens = context.Node.SelectTokens(jPath, false);

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
            dynamic returnVal = Resolve(str, Patterns.Interpolation, (m) => Interpolate(m));

            if (returnVal is string) {
                str = returnVal;
                returnVal = Resolve(str, Patterns.Formula, (m) => Eval(m));
            }

            return returnVal;
        }

        private dynamic Resolve(string str, Regex pattern, Func<Match, dynamic> func) {
            var match = pattern.Match(str);
            dynamic returnVal = str;
            if (match.Success && match.Value == str) {
                returnVal = func(match);
            } else {
                returnVal = pattern.Replace(str, m => {
                    var val = func(m);
                    return val is null ? string.Empty : val.ToString();
                });
            }

            return returnVal;
        }

        private dynamic Eval(Match m) {
            var expression = m.Groups["expression"].Value;
            return formulaSolver.Solve<dynamic>(expression, null);
        }

        private dynamic Interpolate(Match match) {
            try {
                var query = match.Groups["query"].Value;
                var pipes = GetPipes(match);
                var ctx = FindContext(query, out var jPath);
                var hasMatchToken = ctx.Node.TrySelectToken(jPath, out var matchToken);

                if (pipes.Count == 0 && (ctx == null || !hasMatchToken) && !Options.NullToNoPath) {
                    throw new Exception($"Match token not found for expression '{query}'");
                }

                var val = matchToken?.Value<dynamic>();
                if (val is JValue) {
                    val = ((JValue)val).Value;
                }
                val = ProcessPipes(pipes, val);

                return val;
            }
            catch (Exception ex) {
                throw new Exception($"Unable to interpolate expression {match.Value}", ex);
            }
        }

        private dynamic ProcessPipes(List<string> pipeNames, dynamic val) {

            pipeNames.ForEach(pipeName => {
                if (!this.pipes.TryGetValue(pipeName, out var pipe)) {
                    throw new NullReferenceException($"{pipeName} not a registered {nameof(IValuePipe)}");
                }

                val = pipe.Process(val);
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
                jPath = string.IsNullOrEmpty(rest) ? "$" : string.Join(".", new[] { "$", rest });
            } else {
                jPath = query;
            }

            return context;
        }
    }
}
