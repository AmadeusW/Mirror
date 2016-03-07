using System;
using System.Collections.ObjectModel;

namespace AmadeusW.Mirror.GUI.Transit
{
    public class TransitStopViewModel : PropertyChangedBase
    {
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

        private ObservableCollection<ArrivalViewModel> arrivals;
        public ObservableCollection<ArrivalViewModel> Arrivals
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

        public override string ToString() => $"{Direction} {StopName}";

        internal bool ServesLine(TransitLine line)
        {
            return StopName == line.StopName && Direction == line.Direction;
        }
    }
}