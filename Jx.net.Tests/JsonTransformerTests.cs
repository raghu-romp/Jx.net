using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Jx.net.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Jx.net.Transformer;
using Jx.net.ValueMap;

namespace Jx.net.Tests
{
    [TestClass]
    public class JsonTransformerTests
    {
        string testsRootPath;

        [TestInitialize]
        public void Setup()
        {
            testsRootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tests");
        }

        [TestMethod]
        public void BasicJPath()
        {
            TestUseCase("basic");
        }

        [TestMethod]
        public void JxFor()
        {
            TestUseCase("jx-for");
        }

        [TestMethod]
        public void NestedJxFor()
        {
            TestUseCase("multiple-jx-for");
        }

        [TestMethod]
        public void JxIf() {
            TestUseCase("jx-if-exists");
        }

        [TestMethod]
        public void Pipe_BoolYesNo() {
            TestUseCase("value-map");
        }

        [TestMethod]
        public void Pipe_DictTest() {
            var map = new Dictionary<dynamic, dynamic> {
                { "USD", "$" }, { "AUD", "A$" },
                { "CAD", "C$" }, { "GBP", "£" }
            };

            var pipe = new DictionaryMapPipe("CcySymbol", map);
            TestUseCase("value-map-dict", pipe);
        }

        [TestMethod]
        public void Pipe_StringPipes() {
            TestUseCase("stringpipes");
        }

        [TestMethod]
        public void FormulaEvaluation() {
            TestUseCase("formula-eval");
        }

        private void TestUseCase(string name, IValuePipe pipe = null)
        {
            var testPath = Path.Combine(testsRootPath, name);
            var source = ReadFile(Path.Combine(testPath, "source.json"));
            var transformer = ReadFile(Path.Combine(testPath, "transformer.json"));
            var expected = ReadFile(Path.Combine(testPath, "expected.json"));

            var jx = JxFactory.Create();
            if (pipe != null) {
                jx.AddPipe(pipe);
            }

            var actual = jx.Transform(source, transformer);

            Assert.IsTrue(JToken.DeepEquals(expected, actual), $"Expected: {expected}\nActual: {actual}");
        }

        private JToken ReadFile(string fileName)
        {
            var fileText = File.ReadAllText(fileName, Encoding.UTF8);
            return JsonConvert.DeserializeObject<JToken>(fileText);
        }
    }
}
