using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace AmadeusW.Mirror.GUI.Transit
{
    public class TransitViewModel : PropertyChangedBase, IPeriodicallyUpdate
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
            });
        }

        private void Arrivals_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e, TransitLine line)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                resetLine(line);
            }
            else if (e.NewItems != null)
            {
                updateLine(line, e.NewItems);
            }
            else
            {
                // We don't know how to handle this.
                System.Diagnostics.Debugger.Break();
            }
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
                lineToUpdate.Arrivals.Add(getNewArrival(arrival, updatedLine.WalkTime));
            }
        }

        private void resetLine(TransitLine targetLine)
        {
            var lineToReset = Lines.Single(line => line.Equals(targetLine));
            lineToReset.Arrivals.Clear();
        }

        private ArrivalViewModel getNewArrival(DateTime arrivalTime, TimeSpan walkTime)
        {
            if (arrivalTime + TimeSpan.FromHours(12) < DateTime.Now)
            {
                // It's late at night and the bus arrives after midnight.
                arrivalTime += TimeSpan.FromDays(1);
            }
            var test = (arrivalTime - DateTime.Now - walkTime + TimeSpan.FromSeconds(1));
            var test2 = (arrivalTime - DateTime.Now - walkTime + TimeSpan.FromSeconds(1)).TotalMinutes;
            var test3 = (int)((arrivalTime - DateTime.Now - walkTime + TimeSpan.FromSeconds(1)).TotalMinutes);
            return new ArrivalViewModel()
            {
                ArrivalTime = arrivalTime.ToString("h:mm"),
                WhenINeedToLeave = (int)((arrivalTime - DateTime.Now - walkTime + TimeSpan.FromSeconds(1)).TotalMinutes),
            };
        }

        private void updateArrival(ArrivalViewModel arrival, DateTime arrivalTime, TimeSpan walkTime)
        {
            arrival.ArrivalTime = arrivalTime.ToString("h:mm");
            arrival.WhenINeedToLeave = (int)((arrivalTime - DateTime.Now - walkTime + TimeSpan.FromSeconds(1)).TotalMinutes);
        }

        /// <summary>
        /// The first time a viewmodel is hooked up to the model,
        /// populate the Lines collection with lines from the model
        /// </summary>
        private void updateLines()
        {
            var newLines = new ObservableCollection<TransitLineViewModel>();
            foreach (var line in model.Lines)
            {
                var allArrivals = new ObservableCollection<ArrivalViewModel>();
                foreach (var arrival in line.Arrivals)
                {
                    var newArrival = getNewArrival(arrival, line.WalkTime);
                    if (newArrival.WhenINeedToLeave >= 0)
                    {
                        allArrivals.Add(newArrival);
                    }
                }
                newLines.Add(new TransitLineViewModel()
                {
                    RouteName = line.RouteName,
                    StopName = line.StopName,
                    Arrivals = allArrivals,
                });
            }
            Lines = newLines;
            updateTimestamp();
        }

        private void updateTimestamp()
        {
            Initialized = true;
            LastUpdated = DateTime.Now.ToString("h:mm");
        }

        /// <summary>
        /// Every minute, we update the "time to leave" for all lines
        /// </summary>
        /// <returns></returns>
        public void TimerTick(object sender, object e)
        {
            foreach (var line in model.Lines)
            {
                var lineInViewModel = Lines.Single(l => l.Equals(line));
                foreach (var arrival in line.Arrivals)
                {
                    var arrivalInViewModel = lineInViewModel.Arrivals.FirstOrDefault(l => l.ArrivalTime == arrival.ToString("h:mm"));
                    if (arrivalInViewModel != null)
                    {
                        updateArrival(arrivalInViewModel, arrival, line.WalkTime);
                        if (arrivalInViewModel.WhenINeedToLeave < 0)
                        {
                            // Remove obsolete information
                            lineInViewModel.Arrivals.Remove(arrivalInViewModel);
                        }
                    }
                    else
                    {
                        // We probably already removed this line
                    }
                }
            }
        }
    }
}
