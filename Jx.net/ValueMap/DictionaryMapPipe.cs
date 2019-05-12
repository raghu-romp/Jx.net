using System;
using System.Collections.Generic;

namespace Jx.net.ValueMap
{
    public class DictionaryMapPipe : IValuePipe
    {
        public string Name { get; }
        public IDictionary<dynamic, dynamic> Mapping { get; private set; }
        public dynamic Default { get; set; }

        public DictionaryMapPipe(string name, IDictionary<dynamic, dynamic> mapping)
        {
            this.Name = name;
            this.Mapping = mapping;
        }

        public dynamic Process(dynamic fromValue)
        {
            if (fromValue == null) {
                return null;
            }

            this.Mapping.TryGetValue(fromValue, out dynamic value);
            return value;
        }
    }
}
