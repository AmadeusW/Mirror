using AmadeusW.Mirror.GUI.Clock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Tests
{
    class MockClockModel : ClockModel
    {
        internal DateTime MockDateTime { get; set; }

        public override async Task Update()
        {
            CurrentTime = MockDateTime;
        }
    }
}
