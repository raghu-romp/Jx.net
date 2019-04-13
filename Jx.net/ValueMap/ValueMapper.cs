using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net
{
    public class ValueMapper
    {
        public Dictionary<string, IValuePipe> Mappers { get; }
        public bool ReturnNullIfNoMappingFound { get; set; } = false;

        public ValueMapper()
        {
            this.Mappers = new Dictionary<string, IValuePipe>();
        }
        
        public void RegisterMapper(IValuePipe map)
        {
            if (Mappers.ContainsKey(map.MapperName)) {
                throw new ArgumentException($"Mapper with name '{map.MapperName}' already registered");
            }

            Mappers.Add(map.MapperName, map);
        }

        public void UnregisterMapper(string mapperName)
        {
            if (Mappers.ContainsKey(mapperName)) {
                Mappers.Remove(mapperName);
            }
        }

        public dynamic MapValue(string mapperName, string fromValue)
        {
            if (!this.Mappers.ContainsKey(mapperName)) {
                if (ReturnNullIfNoMappingFound) {
                    return null;
                }
                throw new KeyNotFoundException($"Mapper with name '{mapperName}' is not found in registered mappers");
            }

            return this.Mappers[mapperName].MapValue(fromValue);
        }
    }
}
