using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Jx.net.Extensions
{
    public delegate void stringJtokenAction(string propertyValue, JProperty stringProperty, JToken parentToken);
    public delegate void stringPatternMatchJtokenAction(string propertyValue, JProperty stringProperty, JToken parentToken, bool isPartialMatch);

    public static class JTokenExtensions
    {
        public static void AllStringProperties(this JToken token, stringJtokenAction action)
        {
            if (token.Type == JTokenType.Object) {
                ((JObject)token).Properties().Each(property => {
                    switch (property.Value.Type) {
                        case JTokenType.Array:
                            JArray jArray = property.Value.Value<JArray>();
                            jArray.Each(item => AllStringProperties(item, action));
                            break;
                        case JTokenType.String:
                            action((string)property.Value, property, token);
                            break;
                        case JTokenType.Object:
                            AllStringProperties(property.Value, action);
                            break;
                    }
                });
            }
        }

        public static void AllStringProperties(this JToken jtoken, Regex patternToMatch, stringPatternMatchJtokenAction action)
        {
            jtoken.AllStringProperties((val, prop, parent) => {
                if (patternToMatch.IsMatch(val)) {
                    var fullMatchPattern = $"^{patternToMatch.ToString()}$";
                    var fullMatchRegex = new Regex(fullMatchPattern);
                    action(val, prop, parent, !fullMatchRegex.IsMatch(val));
                }
            });
        }
    }
}
