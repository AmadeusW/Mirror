using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Transit
{
    public class TransitViewModel : PropertyChangedBase
    {
        private bool initialized;
        public bool Initialized
        {
            get
            {
                return initialized;
            }
            set
            {
                if (initialized != value)
                {
                    initialized = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string lastUpdated;
        public string LastUpdated
        {
            get
            {
                return lastUpdated;
            }
            set
            {
                if (lastUpdated != value)
                {
                    lastUpdated = value;
                    NotifyPropertyChanged();
                }
            }
        }

        ObservableCollection<TransitLineViewModel> lines;
        public ObservableCollection<TransitLineViewModel> Lines
        {
            get
            {
                return lines;
            }
            set
            {
                if (lines != value)
                {
                    lines = value;
                    NotifyPropertyChanged();
                }
            }
        }



        private TransitModel model;
        internal void Initialize(TransitModel model)
        {
            this.model = model;
            prepareLines();
            model.PropertyChanged += ModelPropertyChanged;
        }

        private void ModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.Lines))
            {
                prepareLines();
            }
        }

        private void prepareLines()
        {
            var newLines = new ObservableCollection<TransitLineViewModel>();
            foreach (var line in model.Lines)
            {
                line.Arrivals.CollectionChanged += (s, e) => Arrivals_CollectionChanged(s, e, line);
                newLines.Add(new TransitLineViewModel()
                {
                    RouteName = line.RouteName,
                    StopName = line.StopName,
                    Arrivals = new ObservableCollection<ArrivalViewModel>()
                });
            }
            Lines = newLines;
        }

        private void updateTimestamp()
        {
            Initialized = true;
            LastUpdated = DateTime.Now.ToString("h:mm");
        }

        private void Arrivals_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e, TransitLine updatedLine)
        {
            updateTimestamp();
            updateLine(updatedLine, e.NewItems);
        }

        private void updateLine(TransitLine updatedLine, IList newArrivals)
        {
            var lineToUpdate = Lines.Single(line => line.RouteName == updatedLine.RouteName
                                                 && line.StopName == updatedLine.StopName);
            foreach (var newArrival in newArrivals)
            {
                var arrival = DateTime.Parse(newArrival.ToString());
                var whenINeedToLeave = (arrival - DateTime.Now - updatedLine.WalkTime + TimeSpan.FromSeconds(1)).Minutes;

                lineToUpdate.Arrivals.Add(new ArrivalViewModel()
                {
                    ArrivalTime = arrival.ToString("h:mm"),
                    WhenINeedToLeave = whenINeedToLeave + " min",
                });
            }
        }
    }
}
