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

namespace Jx.net.Tests
{
    [TestClass]
    public class JsonTransformerTests
    {
        string testsRootPath;
        List<string> testFolders = new List<string>();

        [TestInitialize]
        public void Setup()
        {
            testsRootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tests");
            Directory.GetDirectories(testsRootPath).Each(dir => testFolders.Add(Path.Combine(testsRootPath, dir)));
        }

        [TestMethod]
        public void Test()
        {
            foreach (var testPath in testFolders) {
                TestUseCase(testPath);
            }
        }

        private void TestUseCase(string testPath)
        {
            var source = ReadFile(Path.Combine(testPath, "source.json"));
            var transformer = ReadFile(Path.Combine(testPath, "transformer.json"));
            var expected = ReadFile(Path.Combine(testPath, "expected.json"));

            var jx = new JsonTransformer();
            var actual = jx.Transform(source, transformer);

            Assert.IsTrue(JToken.DeepEquals(expected, actual), $"Test Case: {testPath}");
        }

        private JToken ReadFile(string fileName)
        {
            var fileText = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<JToken>(fileText);
        }
    }
}
