using System.Collections.ObjectModel;

namespace AmadeusW.Mirror.GUI.Transit
{
    public class TransitLineViewModel : PropertyChangedBase
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
    }
}