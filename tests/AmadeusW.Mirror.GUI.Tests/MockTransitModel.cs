using AmadeusW.Mirror.GUI.Transit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Tests
{
    internal class MockTransitModel : TransitModel
    {
        public MockTransitModel(int walkTime) : base()
        {
            this.Lines = new ObservableCollection<TransitLine>()
            {
                new TransitLine
                {
                    RouteName = "1",
                    StopName = "Broadway westbound",
                    WalkTime = TimeSpan.FromMinutes(walkTime),
                    Arrivals = new ObservableCollection<DateTime>(),
                },
                new TransitLine
                {
                    RouteName = "1",
                    StopName = "Broadway eastbound",
                    WalkTime = TimeSpan.FromMinutes(walkTime),
                    Arrivals = new ObservableCollection<DateTime>(),
                },
                new TransitLine
                {
                    RouteName = "99",
                    StopName = "Broadway eastbound",
                    WalkTime = TimeSpan.FromMinutes(walkTime),
                    Arrivals = new ObservableCollection<DateTime>(),
                },
                new TransitLine
                {
                    RouteName = "99",
                    StopName = "Broadway westbound",
                    WalkTime = TimeSpan.FromMinutes(walkTime),
                    Arrivals = new ObservableCollection<DateTime>(),
                },
            };
        }

        public void AddArrival(int minutesAway)
        {
            Lines.First().Arrivals.Add(DateTime.Now + TimeSpan.FromMinutes(minutesAway));
        }
    }
}
