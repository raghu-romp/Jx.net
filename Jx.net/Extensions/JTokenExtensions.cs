using Jx.net.Transformer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Jx.net.Extensions
{
    public delegate void StringJtokenAction(string propertyValue, JToken stringProperty, JToken parentToken);

    public static class JTokenExtensions
    {

        public static bool TrySelectToken(this JToken node, string jPath, out JToken token)
        {
            token = node.SelectToken(jPath);
            return token != null;
        }

        public static void AllStrings(this JToken token, StringJtokenAction action)
        {
            if (token.Type == JTokenType.Object) {
                ((JObject)token).Properties().Each(property => property.Value.AllStrings(action));
            }
            else if (token.Type == JTokenType.Array) {
                ((JArray)token).Each(item => item.AllStrings(action));
            }
            else if (token.Type == JTokenType.String) {
                action(token.Value<string>(), token, token.Parent);
            }
        }

        public static JArray ConvertToJArray(this IEnumerable<JToken> tokens) {
            JArray array = null;

            void IfNullAssign(JToken t, Action a = null) {
                if (array == null) {
                    array = (JArray)t;
                } else {
                    a?.Invoke();
                }
            }

            foreach (var token in tokens) {
                if (token.Type == JTokenType.Array) {
                    IfNullAssign(token, () => {
                        var newArray = (JArray)token;
                        newArray.Each(elm => array.Add(elm));
                    });
                } else {
                    IfNullAssign(new JArray());
                    array.Add(token);
                }
            }

            return array;
        }

        internal static JArray FindForEach(this JToken token, out string expression) {
            return token.FindBlockTemplate(Patterns.JxFor, out expression);
        }

        internal static JArray FindBlockTemplate(this JToken token, Regex pattern, out string expression)
        {
            if (token.Type == JTokenType.Object) {
                var jObj = (JObject)token;
                foreach (var prop in jObj.Properties()) {
                    var array = prop.Value.FindBlockTemplate(pattern, out expression);
                    if (array != null) {
                        return array;
                    }
                }
            }
            else if (token.Type == JTokenType.Array) {
                var array = (JArray)token;
                if (IsJxForArray(array, out expression)) {
                    return array;
                }

                foreach (var item in array) {
                    var forEachArray = item.FindBlockTemplate(pattern, out expression);
                    if (forEachArray != null) {
                        return forEachArray;
                    }
                }
            }

            expression = string.Empty;
            return null;

            bool IsJxForArray(JArray array, out string templateExpression)
            {
                var firstItem = array.Count > 0 ? array[0] : null;
                if (firstItem != null && firstItem.Type == JTokenType.String) {
                    var value = firstItem.Value<string>();
                    if (pattern.IsMatch(value)) {
                        templateExpression = value;
                        return true;
                    }
                }

                templateExpression = string.Empty;
                return false;
            }
        }
    }
}
