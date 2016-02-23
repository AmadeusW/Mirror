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

        public Action<bool> NightFallDelegate;

        public override async Task Update()
        {
            CurrentTime = DateTime.Now;
            NightFallDelegate?.Invoke(CurrentTime.Second % 2 == 0);
        }
    }
}
