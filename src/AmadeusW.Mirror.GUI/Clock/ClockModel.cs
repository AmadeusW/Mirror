using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Clock
{
    class ClockModel : BaseModel
    {
        DateTime currentTime;
        public DateTime CurrentTime
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

        public override TimeSpan Interval => TimeSpan.FromSeconds(1);

        Boolean? NightMode;
        public Action<bool> NightFallDelegate;

        public override async Task Update()
        {
            CurrentTime = DateTime.Now;
            changeAppThemeIfNeeded();
        }

        private void changeAppThemeIfNeeded()
        {
            if (NightFallDelegate == null)
                return;

            if ((!NightMode.HasValue || NightMode.Value)
            && (CurrentTime.Hour >= 7 && CurrentTime.Hour < 22))
            {
                NightMode = false;
                NightFallDelegate?.Invoke(false);
            }
            else if ((!NightMode.HasValue || !NightMode.Value)
            && (CurrentTime.Hour < 7 || CurrentTime.Hour >= 22))
            {
                NightMode = true;
                NightFallDelegate?.Invoke(true);
            }
        }
    }
}
