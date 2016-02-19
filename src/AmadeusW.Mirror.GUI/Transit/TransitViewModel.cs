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
        private const int BUS_CUTOFF = -1;
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
                line.PropertyChanged += ModelLinePropertyChanged;
                return true;
            });
        }

        private void ModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TransitModel.Lines))
            {
                updateLines();
            }
        }

        private void ModelLinePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TransitLine.Arrivals))
            {
                updateLine(sender as TransitLine);
            }
        }

        private void updateLine(TransitLine updatedLine, TransitLineViewModel lineToUpdate = null)
        {
            if (lineToUpdate == null)
                lineToUpdate = Lines.Single(line => line.Equals(updatedLine));

            // Remove arrivals that are not there in the model
            // .ToList() makes a copy of the collection to iterate over
            foreach (var existingArrival in lineToUpdate.Arrivals.ToList())
            {
                if (!updatedLine.Arrivals.Contains(existingArrival.ArrivalTime))
                {
                    lineToUpdate.Arrivals.Remove(existingArrival);
                }
            }
            // Add arrivals that are not there in the viewmodel
            foreach (var arrivalInModel in updatedLine.Arrivals)
            {
                if (!lineToUpdate.Arrivals.Any(a => a.ArrivalTime == arrivalInModel))
                {
                    var newArrival = getNewArrival(arrivalInModel, updatedLine.WalkTime);
                    if (newArrival.WhenINeedToLeave < BUS_CUTOFF)
                    {
                        continue;
                    }
                    var insertionPoint = lineToUpdate.Arrivals.Count(a => a.ArrivalTime < arrivalInModel);
                    lineToUpdate.Arrivals.Insert(insertionPoint, newArrival);
                }
            }

            updateTimestamp();
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
                ArrivalTime = arrivalTime,
                WhenINeedToLeave = (int)((arrivalTime - DateTime.Now - walkTime + TimeSpan.FromSeconds(1)).TotalMinutes),
            };
        }

        private void updateArrival(ArrivalViewModel arrival, TimeSpan walkTime)
        {
            arrival.WhenINeedToLeave = (int)((arrival.ArrivalTime - DateTime.Now - walkTime + TimeSpan.FromSeconds(1)).TotalMinutes);
        }

        /// <summary>
        /// The first time a viewmodel is hooked up to the model,
        /// populate the Lines collection with lines from the model
        /// </summary>
        private void updateLines()
        {
            var newLines = new ObservableCollection<TransitLineViewModel>();
            foreach (var line in model.Lines.OrderBy(l => l.RouteName).OrderBy(l => l.Direction))
            {
                var newLineViewModel = new TransitLineViewModel()
                {
                    RouteName = line.RouteName,
                    StopName = line.StopName,
                    Direction = line.Direction,
                    Arrivals = new ObservableCollection<ArrivalViewModel>(),
                };
                updateLine(line, newLineViewModel);
                newLines.Add(newLineViewModel);
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
                    var arrivalInViewModel = lineInViewModel.Arrivals.FirstOrDefault(l => l.ArrivalTime == arrival);
                    if (arrivalInViewModel != null)
                    {
                        updateArrival(arrivalInViewModel, line.WalkTime);
                        // Remove obsolete information
                        if (arrivalInViewModel.WhenINeedToLeave < BUS_CUTOFF)
                        {
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
