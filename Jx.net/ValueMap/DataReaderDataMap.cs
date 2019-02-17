using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Jx.net.ValueMap
{
    public class DataReaderValueMap : IValueMap
    {
        public string MappingName { get; }
        private DictionaryMapper dictionaryMapper;

        public DataReaderValueMap(string name, IDataReader reader, string fromColumn, string toColumn)
        {
            this.MappingName = name;
            this.BuildMap(reader, fromColumn, toColumn);
        }

        private void BuildMap(IDataReader reader, string fromColumn, string toColumn)
        {
            var valueMapping = new Dictionary<string, dynamic>();

            while (reader.Read())
            {
                var fromValue = reader[fromColumn].ToString();
                var toValue = reader[toColumn];
                if (valueMapping.ContainsKey(fromValue))
                {
                    throw new NotSupportedException($"Duplicate mapping found for '{fromValue}' in SqlSourceDataMap");
                }
                valueMapping.Add(fromValue, toValue);
            }

            this.dictionaryMapper = new DictionaryMapper(this.MappingName, valueMapping);
        }

        public dynamic MapValue(string fromValue)
        {
            return this.dictionaryMapper.MapValue(fromValue);
        }
    }
}
