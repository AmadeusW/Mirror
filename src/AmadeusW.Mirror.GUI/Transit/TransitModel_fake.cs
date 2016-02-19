using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Transit
{
    internal class TransitModel_fake : TransitModel
    {
        private Random random = new Random();

        public override TimeSpan Interval => TimeSpan.FromSeconds(2);

        public TransitModel_fake() : base()
        {
            this.Lines = new List<TransitLine>()
            {
                new TransitLine
                {
                    RouteName = "1",
                    Direction = "Westbound",
                    StopName = "Broadway",
                    WalkTime = TimeSpan.FromMinutes(5)
                },
                new TransitLine
                {
                    RouteName = "1",
                    Direction = "Eastbound",
                    StopName = "Broadway",
                    WalkTime = TimeSpan.FromMinutes(5)
                },
                new TransitLine
                {
                    RouteName = "99",
                    Direction = "Westbound",
                    StopName = "Broadway",
                    WalkTime = TimeSpan.FromMinutes(5)
                },
                new TransitLine
                {
                    RouteName = "99",
                    Direction = "Eastbound",
                    StopName = "Broadway",
                    WalkTime = TimeSpan.FromMinutes(5)
                },
            };
        }

        public override async Task Update()
        {
            var line = Lines[getRandomInt(this.Lines.Count())]; // required changing Lines to List
            (line.Arrivals as List<DateTime>).Add(DateTime.Now + TimeSpan.FromMinutes(getRandomInt(10)));
        }

        private int getRandomInt(int maxValue)
        {
            return (int)(random.NextDouble() * maxValue);
        }
    }
}
