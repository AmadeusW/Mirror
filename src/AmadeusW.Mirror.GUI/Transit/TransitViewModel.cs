using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

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
            updateLines();
            model.PropertyChanged += ModelPropertyChanged;
            model.Lines.All(line =>
            {
                line.Arrivals.CollectionChanged += (s, e) => Arrivals_CollectionChanged(s, e, line);
                return true;
            }
                );
        }

        private void Arrivals_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e, TransitLine line)
        {
            updateLine(line, e.NewItems);
        }

        private void ModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.Lines))
            {
                updateLines();
            }
            else
            {
                updateLines();
            }
        }

        private void updateLine(TransitLine updatedLine, System.Collections.IList newArrivals)
        {
            var lineToUpdate = Lines.Single(line => line.Equals(updatedLine));
            foreach (var newArrival in newArrivals)
            {
                var arrival = DateTime.Parse(newArrival.ToString());
                
                lineToUpdate.Arrivals.Add(new ArrivalViewModel()
                {
                    ArrivalTime = arrival.ToString("h:mm"),
                    WhenINeedToLeave = (arrival - DateTime.Now - updatedLine.WalkTime + TimeSpan.FromSeconds(1)).Minutes.ToString() + " min",
                }
                );
            }
        }

        private void updateLines()
        {
            var newLines = new ObservableCollection<TransitLineViewModel>();
            foreach (var line in model.Lines)
            {
                newLines.Add(new TransitLineViewModel()
                {
                    RouteName = line.RouteName,
                    StopName = line.StopName,
                    Arrivals = new ObservableCollection<ArrivalViewModel>(),
                });
            }
            Lines = newLines;
        }
    }
}
