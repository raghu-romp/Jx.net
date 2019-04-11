using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.Transformer
{
    internal class Context
    {
        public JToken Node { get; internal set; }
        public int? Index { get; set; }
        public Context Parent { get; internal set; }

        public bool isLast()
        {
            return this.Node.Next == null;
        }

        public static Context New(JToken node, Context parent, int? index) {
            return new Context { Node = node, Parent = parent, Index = index };
        }
    }
}
