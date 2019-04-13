using System;
using System.Collections.Generic;

namespace Jx.net.ValueMap
{
    public class DictionaryMapper : IValuePipe
    {
        public string MapperName { get; }
        public IDictionary<dynamic, dynamic> Mapping { get; private set; }
        public dynamic Default { get; set; }

        public DictionaryMapper(string name, IDictionary<dynamic, dynamic> mapping)
        {
            this.MapperName = name;
            this.Mapping = mapping;
        }

        public dynamic MapValue(dynamic fromValue)
        {
            if (fromValue == null) {
                return null;
            }

            this.Mapping.TryGetValue(fromValue, out dynamic value);
            return value;
        }
    }
}
