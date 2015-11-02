using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Clock
{
    public class ClockViewModel : PropertyChangedBase
    {
        private ClockModel model;
        private string currentTime;
        public string CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                if (currentTime != value)
                {
                    currentTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string currentDate;
        public string CurrentDate
        {
            get
            {
                return currentDate;
            }
            set
            {
                if (currentDate != value)
                {
                    currentDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        internal void Initialize(ClockModel model)
        {
            this.model = model;
            updateTime();
            model.PropertyChanged += ModelPropertyChanged;
        }

        private void ModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.CurrentTime))
            {
                updateTime();
            }
        }

        private void updateTime()
        {
            CurrentTime = model.CurrentTime.ToString("h:mm");
            CurrentDate = model.CurrentTime.ToString("MMMM d");
        }
    }
}
