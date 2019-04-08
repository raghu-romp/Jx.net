﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Jx.net.Extensions
{
    public delegate void StringJtokenAction(string propertyValue, JToken stringProperty, JToken parentToken);
    public delegate void StringPatternMatchJtokenAction(string propertyValue, JToken stringProperty, JToken parentToken, bool isPartialMatch);
    public delegate void ForEachContainerAction(JArray arrayTemplate, JToken parentToken);

    public static class JTokenExtensions
    {
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


        public static JArray FindForEach(this JToken token, out string expression)
        {
            if (token.Type == JTokenType.Object)
            {
                var jObj = (JObject)token;
                foreach (var prop in jObj.Properties()) {
                    var array = prop.Value.FindForEach(out expression);
                    if (array != null) {
                        return array;
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                var array = (JArray)token;
                if (IsJxForArray(array, out expression)) {
                    return array;
                }

                foreach (var item in array)
                {
                    var forEachArray = item.FindForEach(out expression);
                    if (forEachArray != null)
                    {
                        return forEachArray;
                    }
                }
            }

            expression = string.Empty;
            return null;

            bool IsJxForArray(JArray array, out string jxForExpression)
            {
                var firstItem = array.Count > 0 ? array[0] : null;
                if (firstItem != null && firstItem.Type == JTokenType.String)
                {
                    var value = firstItem.Value<string>();
                    if (value.StartsWith("*jx-for"))
                    {
                        jxForExpression = value;
                        return true;
                    }
                }

                jxForExpression = string.Empty;
                return false;
            }
        }

        //public static void AllStringProperties(this JToken jtoken, Regex patternToMatch, stringPatternMatchJtokenAction action)
        //{
        //    jtoken.AllStrings((val, prop, parent) => {
        //        if (patternToMatch.IsMatch(val)) {
        //            var fullMatchPattern = $"^{patternToMatch.ToString()}$";
        //            var fullMatchRegex = new Regex(fullMatchPattern);
        //            action(val, prop, parent, !fullMatchRegex.IsMatch(val));
        //        }
        //    });
        //}
    }
}