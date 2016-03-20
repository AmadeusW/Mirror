using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AmadeusW.Mirror.GUI.Transit
{
    public class TransitLine : PropertyChangedBase
    {
        public TransitLine()
        {
            Arrivals = new List<DateTime>();
        }

        private string routeName;
        public string RouteName
        {
            get
            {
                return routeName;
            }
            set
            {
                if (routeName != value)
                {
                    routeName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string direction;
        public string Direction
        {
            get
            {
                return direction;
            }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string stopName;
        public string StopName
        {
            get
            {
                return stopName;
            }
            set
            {
                if (stopName != value)
                {
                    stopName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private TimeSpan walkTime;
        public TimeSpan WalkTime
        {
            get
            {
                return walkTime;
            }
            set
            {
                if (walkTime != value)
                {
                    walkTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private IEnumerable<DateTime> arrivals;
        public IEnumerable<DateTime> Arrivals
        {
            get
            {
                return arrivals;
            }
            set
            {
                if (arrivals != value)
                {
                    arrivals = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override string ToString() => $"{RouteName} {Direction} {StopName}";
    }
}