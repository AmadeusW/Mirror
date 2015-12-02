using AmadeusW.Mirror.GUI.Transit;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Tests
{
    [TestClass]
    public class TransitTests
    {
        [TestMethod]
        public void TransitNewArrivalsAreAddedAndCalculated()
        {
            var distanceOfStop = 5;
            var nextBus = 7;
            var needToLeave = nextBus - distanceOfStop;

            var tm = new MockTransitModel(distanceOfStop);

            var tvm = new TransitViewModel();
            tvm.Initialize(tm);

            tm.AddArrival(nextBus);

            Assert.AreEqual($"{needToLeave} min", tvm.Lines.First().Arrivals.First().WhenINeedToLeave);
        }
    }
}
