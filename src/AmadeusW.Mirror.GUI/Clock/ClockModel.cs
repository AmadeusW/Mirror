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
        public DateTime CurrentTime { get; set; }

        public override TimeSpan Interval => TimeSpan.FromSeconds(1);

        public override void Update()
        {
            CurrentTime = DateTime.Now;
        }
    }
}
