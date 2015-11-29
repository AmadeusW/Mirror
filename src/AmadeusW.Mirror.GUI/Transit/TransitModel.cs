using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Transit
{
    class TransitModel : BaseModel
    {
        private IEnumerable<TransitLine> lines;
        public IEnumerable<TransitLine> Lines
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

        public override TimeSpan Interval => TimeSpan.FromMinutes(5);

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
