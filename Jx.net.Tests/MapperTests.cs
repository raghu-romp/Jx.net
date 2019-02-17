using Jx.net.ValueMap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Jx.net.Tests
{
    [TestClass]
    public class MapperTests
    {
        string input = "{\r\n  \"currencies\": [ \"US Dollar\", \"Euro\" ]\r\n}";
        string transformer = "{\r\n\t\"transformed\": {\r\n\t\t\"#loop($.currencies)\": {\r\n\t\t\t\"Name\": \"#currentvalueatpath($)\",\r\n\t\t\t\"Code\": \"#MapValue(CurrencyCodeMapper,#currentvalueatpath($))\"\r\n\t\t}\r\n\t}\r\n}";
        string expected = "{\r\n  \"transformed\": [\r\n    {\r\n      \"Name\": \"US Dollar\",\r\n      \"Code\": \"USD\"\r\n    },\r\n    {\r\n      \"Name\": \"Euro\",\r\n      \"Code\": \"EUR\"\r\n    }\r\n  ]\r\n}";

        [TestMethod]
        public void TestDictionaryMapper()
        {
            var input = "{\r\n  \"currencies\": [ \"US Dollar\", \"Euro\" ]\r\n}";
            var transformer = "{\r\n\t\"transformed\": {\r\n\t\t\"#loop($.currencies)\": {\r\n\t\t\t\"Name\": \"#currentvalueatpath($)\",\r\n\t\t\t\"Code\": \"#MapValue(CurrencyCodeMapper,#currentvalueatpath($))\"\r\n\t\t}\r\n\t}\r\n}";
            var expected = "{\r\n  \"transformed\": [\r\n    {\r\n      \"Name\": \"US Dollar\",\r\n      \"Code\": \"USD\"\r\n    },\r\n    {\r\n      \"Name\": \"Euro\",\r\n      \"Code\": \"EUR\"\r\n    }\r\n  ]\r\n}";
            var dictionaryMapper = new DictionaryMapper("CurrencyCodeMapper", new Dictionary<string, dynamic>
            {
                { "US Dollar", "USD" }, { "Indian Rupee", "INR" }, { "Euro", "EUR" }
            });
            JsonMapper.Mapper.RegisterMapper(dictionaryMapper);
            var actual = JsonMapper.Map(input, transformer);
            JsonAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDataReaderMapper()
        {
            var table = new DataTable();
            table.Columns.AddRange(new[]
            {
                new DataColumn("CurrencyName", typeof(string)),
                new DataColumn("CurrencyCode", typeof(string))
            });

            table.Rows.Add("US Dollar", "USD");
            table.Rows.Add("Indian Rupee", "INR");
            table.Rows.Add("Euro", "EUR");
            JsonMapper.FactoryReset();
            JsonMapper.Mapper.RegisterMapper(
                    new DataReaderValueMap("CurrencyCodeMapper", table.CreateDataReader(), "CurrencyName", "CurrencyCode")
            );
            var actual = JsonMapper.Map(input, transformer);
            JsonAssert.AreEqual(expected, actual);
        }
    }
}
