using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Jx.net.Transformer
{
    public static class Patterns
    {
        public static readonly Regex Interpolation = new Regex(
                "{{(?<query>.*?)(?<pipes> => ((?<pipe>.*?)))*}}");

        public static readonly Regex Formula = new Regex(
            @"=\[(?<expression>.*?)\]"
        );

        public static readonly Regex JxBlockStatement = new Regex(
            @"^\*jx-(?<blocktype>for|if)(.*?)$"
        );

        public static readonly Regex JxFor = new Regex(
            @"^\*jx-for\((?<query>.*?)\) as (?<alias>[a-z][a-z0-9]{0,9})$"
        );

        public static readonly Regex JxIf = new Regex(@"^\*jx-if\((?<query>.*?) (?<condition>((not-)?exists)|(not-)null)\)$");

        public static bool IsMatch(this Regex pattern, string str, out Match match) {
            match = pattern.Match(str);
            return match.Success;
        }
    }
}
