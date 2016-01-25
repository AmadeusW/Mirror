using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Transit
{
    internal interface IPeriodicallyUpdate
    {
        void TimerTick(object sender, object e);
    }
}