using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AmadeusW.Mirror.GUI.Transit
{
    public class TransitLine : PropertyChangedBase
    {
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

        private ObservableCollection<DateTime> arrivals;
        public ObservableCollection<DateTime> Arrivals
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

        public IEnumerable<DateTime> ArrivalsEnumerable { get; set; }

        public override string ToString()
        {
            return $"{RouteName} {StopName}";
        }
    }
}