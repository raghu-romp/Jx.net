using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Jx.net.Transformer
{
    public static class Patterns
    {
        public static readonly Regex Interpolation = new Regex("{{(?<placeholder>.*?)}}");
        public static readonly Regex JxFor = new Regex(@"^\*jx-for\((?<placeholder>.*?)\)( as (?<alias>[a-zA-Z][a-zA-Z0-9]{0,9}))?$");
    }
}
