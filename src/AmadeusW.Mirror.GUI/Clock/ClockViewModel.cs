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

        internal void Initialize(ClockModel model)
        {
            this.model = model;
            updateTime();
        }

        private void updateTime()
        {
            CurrentTime = $"It is {model.CurrentTime.ToString("hh:mm:ss tt")}.";
        }
    }
}
