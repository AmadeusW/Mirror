using AmadeusW.Mirror.GUI.Converters;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Tests
{
    [TestClass]
    public class ConverterTests
    {
        [TestMethod]
        public void DateTimeToShortStringConverterTest()
        {
            var converter = new DateTimeToShortStringConverter();
            var converted = converter.Convert(DateTime.Parse("2016.02.01 16:52"), typeof(string), null, null);
            Assert.AreEqual("4:52", converted);
        }

        [TestMethod]
        public void DateTimeToShortStringConverterFailsGracefully()
        {
            var converter = new DateTimeToShortStringConverter();
            var converted = converter.Convert(1652, typeof(string), null, null);
            Assert.AreEqual("", converted);
        }
    }
}
