using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Jx.net.ValueMap
{
    public class DataReaderValueMap : IValuePipe
    {
        public string MapperName { get; }
        private DictionaryMapper dictionaryMapper;

        public DataReaderValueMap(string name, IDataReader reader, string fromColumn, string toColumn)
        {
            this.MapperName = name;
            this.BuildMap(reader, fromColumn, toColumn);
        }

        private void BuildMap(IDataReader reader, string fromColumn, string toColumn)
        {
            var valueMapping = new Dictionary<dynamic, dynamic>();

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

            this.dictionaryMapper = new DictionaryMapper(this.MapperName, valueMapping);
        }

        public dynamic Process(dynamic fromValue)
        {
            return this.dictionaryMapper.Process(fromValue);
        }
    }
}
