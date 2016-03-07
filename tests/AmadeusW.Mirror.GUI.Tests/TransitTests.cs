using AmadeusW.Mirror.GUI.Transit;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            Assert.AreEqual($"{needToLeave} min", tvm.Stops.First().Arrivals.First().WhenINeedToLeave);
        }

        [TestMethod]
        public void TranslinkLineIsCorrectlyUpdated()
        {
            var line = new TransitLine_translink("59997", "099", "Broadway at Arbutus", "Eastbound", "99", 5);
            var data = @"
[{ ""RouteNo"":""099"",""RouteName"":""COMMERCIAL-BROADWAY\/UBC (B-LINE)  "",""Direction"":""EAST"",""RouteMap"":{ ""Href"":""http:\/\/nb.translink.ca\/geodata\/099.kmz""},""Schedules"":[{""Pattern"":""EB1"",""Destination"":""COMM'L-BDWAY STN"",""ExpectedLeaveTime"":""7:28pm 2016-01-14"",""ExpectedCountdown"":6,""ScheduleStatus"":""+"",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""06:12:07 pm""},{""Pattern"":""EB1"",""Destination"":""COMM'L-BDWAY STN"",""ExpectedLeaveTime"":""7:35pm 2016-01-14"",""ExpectedCountdown"":13,""ScheduleStatus"":""+"",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""06:17:07 pm""},{""Pattern"":""EB1"",""Destination"":""COMM'L-BDWAY STN"",""ExpectedLeaveTime"":""7:42pm 2016-01-14"",""ExpectedCountdown"":20,""ScheduleStatus"":"" "",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""06:22:03 pm""},{""Pattern"":""EB1"",""Destination"":""COMM'L-BDWAY STN"",""ExpectedLeaveTime"":""7:47pm 2016-01-14"",""ExpectedCountdown"":25,""ScheduleStatus"":"" "",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""06:27:10 pm""},{""Pattern"":""EB1"",""Destination"":""COMM'L-BDWAY STN"",""ExpectedLeaveTime"":""7:57pm 2016-01-14"",""ExpectedCountdown"":35,""ScheduleStatus"":""*"",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""06:37:02 pm""},{""Pattern"":""EB1"",""Destination"":""COMM'L-BDWAY STN"",""ExpectedLeaveTime"":""8:02pm 2016-01-14"",""ExpectedCountdown"":40,""ScheduleStatus"":""*"",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""06:42:03 pm""},{""Pattern"":""EB3"",""Destination"":""TO BOUNDARY B-LINE"",""ExpectedLeaveTime"":""7:23pm 2016-01-14"",""ExpectedCountdown"":1,""ScheduleStatus"":""+"",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""06:07:01 pm""},{""Pattern"":""EB3"",""Destination"":""TO BOUNDARY B-LINE"",""ExpectedLeaveTime"":""7:52pm 2016-01-14"",""ExpectedCountdown"":30,""ScheduleStatus"":""*"",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""06:32:08 pm""},{""Pattern"":""EB3"",""Destination"":""TO BOUNDARY B-LINE"",""ExpectedLeaveTime"":""8:07pm 2016-01-14"",""ExpectedCountdown"":45,""ScheduleStatus"":""*"",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""06:47:02 pm""},{""Pattern"":""EB3"",""Destination"":""TO BOUNDARY B-LINE"",""ExpectedLeaveTime"":""8:23pm 2016-01-14"",""ExpectedCountdown"":61,""ScheduleStatus"":"" "",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""07:03:07 pm""},{""Pattern"":""EB3"",""Destination"":""TO BOUNDARY B-LINE"",""ExpectedLeaveTime"":""8:47pm 2016-01-14"",""ExpectedCountdown"":85,""ScheduleStatus"":""*"",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""08:38:20 pm""},{""Pattern"":""EB3"",""Destination"":""TO BOUNDARY B-LINE"",""ExpectedLeaveTime"":""8:51pm 2016-01-14"",""ExpectedCountdown"":89,""ScheduleStatus"":""*"",""CancelledTrip"":false,""CancelledStop"":false,""AddedTrip"":false,""AddedStop"":false,""LastUpdate"":""08:38:20 pm""}]}]
            ";
            line.UpdateWithRawData(data);

            var x = line.Arrivals;
        }
    }
}
