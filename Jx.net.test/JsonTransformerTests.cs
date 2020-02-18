using Jx.net.ValueMap;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace Jx.net.test
{
    public class JsonTransformerTests
    {
        string testsRootPath;

        public JsonTransformerTests()
        {
            testsRootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tests");
        }

        [Theory]
        [InlineData("basic")]
        [InlineData("formula-eval")]
        [InlineData("jx-for")]
        [InlineData("jx-if-exists")]
        [InlineData("multiple-jx-for")]
        [InlineData("value-map")]
        public void TestTransformer(string name)
        {
            ExecuteTest(name);
        }

        [Fact]
        public void Pipe_DictTest()
        {
            var map = new Dictionary<dynamic, dynamic> {
                { "USD", "$" }, { "AUD", "A$" },
                { "CAD", "C$" }, { "GBP", "£" }
            };

            var pipe = new DictionaryMapPipe("CcySymbol", map);
            ExecuteTest("value-map-dict", pipe);
        }

        private void ExecuteTest(string name, IValuePipe pipe = null)
        {
            var testPath = Path.Combine(testsRootPath, name);
            var source = ReadFile(Path.Combine(testPath, "source.json"));
            var transformer = ReadFile(Path.Combine(testPath, "transformer.json"));
            var expected = ReadFile(Path.Combine(testPath, "expected.json"));

            var jx = JxFactory.Create();

            if (pipe != null)
            {
                jx.AddPipe(pipe);
            }

            var actual = jx.Transform(source, transformer);

            Assert.True(JToken.DeepEquals(expected, actual), $"Expected: {expected}\nActual: {actual}");
        }

        private JToken ReadFile(string fileName)
        {
            var fileText = File.ReadAllText(fileName, Encoding.UTF8);
            return JsonConvert.DeserializeObject<JToken>(fileText);
        }
    }
}
