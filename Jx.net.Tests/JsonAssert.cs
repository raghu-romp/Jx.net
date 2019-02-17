using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net.Tests
{
    public static class JsonAssert
    {
        public static void AreEqual(string expected, string actual)
        {
            expected = JsonConvert.DeserializeObject(expected).ToString();
            actual = JsonConvert.DeserializeObject(actual).ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
