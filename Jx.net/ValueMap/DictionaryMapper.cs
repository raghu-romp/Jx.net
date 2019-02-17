using System.Collections.Generic;

namespace Jx.net.ValueMap
{
    public class DictionaryMapper : IValueMap
    {
        public string MappingName { get; }
        public IDictionary<string, dynamic> Mapping { get; private set; }

        public DictionaryMapper(string name, IDictionary<string, dynamic> mapping)
        {
            this.MappingName = name;
            this.Mapping = mapping;
        }

        public dynamic MapValue(string fromValue)
        {
            this.Mapping.TryGetValue(fromValue, out dynamic value);
            return value;
        }
    }
}
