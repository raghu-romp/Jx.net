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

        public static JArray FlattenArray(this IEnumerable<JToken> tokens) {
            JArray array = null;

            void IfTargetArrayNull(JToken t, Action a = null) {
                if (array == null) {
                    array = (JArray)t;
                } else {
                    a?.Invoke();
                }
            }

            foreach (var token in tokens) {
                if (token.Type == JTokenType.Array) {
                    IfTargetArrayNull(token, () => {
                        var newArray = (JArray)token;
                        newArray.Each(elm => array.Add(elm));
                    });
                } else {
                    IfTargetArrayNull(new JArray());
                    array.Add(token);
                }
            }

            return array;
        }

        internal static JArray FindBlockStatement(this JToken token, out Match match) {
            return token.FindBlockTemplate(Patterns.JxBlockStatement, out match);
        }

        //internal static JArray FindForTemplate(this JToken token, out Match match) {
        //    return token.FindBlockTemplate(Patterns.JxFor, out match);
        //}

        //internal static JArray FindIfTemplate(this JToken token, out Match match) {
        //    return token.FindBlockTemplate(Patterns.JxIf, out match);
        //}

        internal static JArray FindBlockTemplate(this JToken token, Regex pattern, out Match match)
        {
            if (token.Type == JTokenType.Object) {
                var jObj = (JObject)token;
                foreach (var prop in jObj.Properties()) {
                    var array = prop.Value.FindBlockTemplate(pattern, out match);
                    if (array != null) {
                        return array;
                    }
                }
            }
            else if (token.Type == JTokenType.Array) {
                var array = (JArray)token;
                if (IsBlockTemplateArray(array, out match)) {
                    return array;
                }

                foreach (var item in array) {
                    var forEachArray = item.FindBlockTemplate(pattern, out match);
                    if (forEachArray != null) {
                        return forEachArray;
                    }
                }
            }

            match = null;
            return null;

            bool IsBlockTemplateArray(JArray array, out Match templateMatch)
            {
                var firstItem = array.Count > 0 ? array[0] : null;
                if (firstItem != null && firstItem.Type == JTokenType.String) {
                    var value = firstItem.Value<string>();
                    var patternMatch = pattern.Match(value);
                    if (patternMatch.Success) {
                        templateMatch = patternMatch;
                        return true;
                    }
                }

                templateMatch = null;
                return false;
            }
        }
    }
}
