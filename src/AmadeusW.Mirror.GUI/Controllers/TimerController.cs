using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmadeusW.Mirror.GUI.Transit;
using Windows.UI.Xaml;

namespace AmadeusW.Mirror.GUI.Controllers
{
    static class TimerController
    {
        public static void RegisterModel(BaseModel model)
        {
            var timer = new DispatcherTimer();
            timer.Interval = model.Interval;
            timer.Tick += model.TimerTick;
            timer.Start();
        }

        /// <summary>
        /// ViewModels are registered to be updated every minute
        /// </summary>
        /// <param name="transitViewModel"></param>
        internal static void RegisterViewModel(IPeriodicallyUpdate viewModel)
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += viewModel.TimerTick;
            timer.Start();
        }
    }
}
