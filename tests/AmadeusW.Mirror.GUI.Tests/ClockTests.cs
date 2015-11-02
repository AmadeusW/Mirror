using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AmadeusW.Mirror.GUI.Clock;

namespace AmadeusW.Mirror.GUI.Tests
{
    [TestClass]
    public class ClockTests
    {
        [TestMethod]
        public void ClockViewModelCorrectlyDisplaysPMTime()
        {
            var cm = new MockClockModel();
            cm.MockDateTime = DateTime.Parse("2015-10-30 19:53:13");
            cm.Update();
            var cvm = new ClockViewModel();
            cvm.Initialize(cm);
            var cmTime = cm.CurrentTime;
            var cvmTime = cvm.CurrentTime;  
            Assert.AreEqual("7:53", cvmTime);
        }

        [TestMethod]
        public void ClockViewModelCorrectlyDisplaysAMTime()
        {
            var cm = new MockClockModel();
            cm.MockDateTime = DateTime.Parse("2015-10-30 04:53:13");
            cm.Update();
            var cvm = new ClockViewModel();
            cvm.Initialize(cm);
            var cmTime = cm.CurrentTime;
            var cvmTime = cvm.CurrentTime;
            Assert.AreEqual("4:53", cvmTime);
        }

        [TestMethod]
        public void ClockViewModelCorrectlyUpdatesTime()
        {
            var cm = new MockClockModel();
            cm.MockDateTime = DateTime.Parse("2015-10-30 04:53:13");
            cm.Update();
            var cvm = new ClockViewModel();
            cvm.Initialize(cm);
            var cmTime = cm.CurrentTime;
            var cvmTime = cvm.CurrentTime;
            Assert.AreEqual("4:53", cvmTime);

            cm.MockDateTime = DateTime.Parse("2015-10-30 05:54:13");
            cm.Update();
            var cmTime2 = cm.CurrentTime;
            var cvmTime2 = cvm.CurrentTime;
            Assert.AreEqual("5:54", cvmTime2);
        }
    }
}
