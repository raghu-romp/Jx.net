using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jx.net.Tests
{
    [TestClass]
    public class BasicMapperTests
    {
        [TestMethod]
        public void MapFromJson()
        {
            var input = "{ \"menu\": { \"popup\": { \"menuitem\": [ { \"value\": \"Open\", \"onclick\": \"OpenDoc()\" }, { \"value\": \"Close\", \"onclick\": \"CloseDoc()\" } ] } } }";
            var transformer = "{ \"result\": { \"Open\": \"#valueof($.menu.popup.menuitem[?(@.value=='Open')].onclick)\", \"Close\": \"#valueof($.menu.popup.menuitem[?(@.value=='Close')].onclick)\" } }";
            var expected = "{ \"result\":{\"Open\": \"OpenDoc()\", \"Close\": \"CloseDoc()\"} }";
            var actual = JsonMapper.Map(input, transformer);
            JsonAssert.AreEqual(expected, actual);
        }
    }
}
