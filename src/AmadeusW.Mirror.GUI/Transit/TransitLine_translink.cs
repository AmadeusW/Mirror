using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AmadeusW.Mirror.GUI.Transit
{
    public class TransitLine_translink : TransitLine
    {
        public string ApiStopNumber { get; }
        public string ApiRouteNumber { get; }

        public TransitLine_translink(string apiStopNumber, string apiRouteNumber, string stopName, string routeNumber, int walkTime) : base()
        {
            ApiStopNumber = apiStopNumber;
            ApiRouteNumber = apiRouteNumber;
            StopName = stopName;
            RouteName = routeNumber;
            WalkTime = TimeSpan.FromMinutes(walkTime);
            Arrivals = new ObservableCollection<DateTime>();
        }

        internal void UpdateWithRawData(string data)
        {
            try
            {
                var array = JArray.Parse(data);
                var schedule = array.First["Schedules"];

                Arrivals.Clear();
                foreach (var element in schedule)
                {
                    var rawTime = element["ExpectedLeaveTime"].ToString();
                    var time = DateTime.Parse(rawTime);
                    Arrivals.Add(time);
                }
            }
            catch (Exception ex)
            {
                // There might be no posted schedule at this time,
                // ignore this.
            }
        }
    }
}
