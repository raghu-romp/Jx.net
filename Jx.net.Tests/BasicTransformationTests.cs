using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jx.net.Tests
{
    [TestClass]
    public class BasicTransformationTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mapper = new JsonMapper();
            var input = "{ \"menu\": { \"popup\": { \"menuitem\": [ { \"value\": \"Open\", \"onclick\": \"OpenDoc()\" }, { \"value\": \"Close\", \"onclick\": \"CloseDoc()\" } ] } } }";
            var transformer = "{ \"result\": { \"Open\": \"#valueof($.menu.popup.menuitem[?(@.value=='Open')].onclick)\", \"Close\": \"#valueof($.menu.popup.menuitem[?(@.value=='Close')].onclick)\" } }";
            var expected = "{ \"result\":{\"Open\": \"OpenDoc()\", \"Close\": \"CloseDoc()\"} }";
            var actual = mapper.Map(input, transformer);
            AssertAreEqual(expected, actual);
        }

        private void AssertAreEqual(string expected, string actual)
        {
            expected = JsonConvert.DeserializeObject(expected).ToString();
            actual = JsonConvert.DeserializeObject(actual).ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
