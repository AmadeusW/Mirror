using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace AmadeusW.Mirror.GUI.Controllers
{
    internal class NavigationController
    {
        private List<Type> screens { get; set; }
        private Action<Type, bool> launchCallback;
        int screenId;
        int maxScreenId;

        public NavigationController(Action<Type, bool> launchScreenCallback)
        {
            screens = new List<Type>();
            screenId = 0;
            launchCallback = launchScreenCallback;
        }

        internal void GlobalKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            try
            {
                if (args.VirtualKey == Windows.System.VirtualKey.Right)
                {
                    NavigateNext();
                }
                else if (args.VirtualKey == Windows.System.VirtualKey.Left)
                {
                    NavigatePrevious();
                }
            }
            catch (Exception ex)
            {
                var tc = new Microsoft.ApplicationInsights.TelemetryClient();
                var properties = new Dictionary<String, string> { { "Module", "Navigation" } };
                tc.TrackException(ex, properties);
                System.Diagnostics.Debugger.Break();
            }
        }

        internal void NavigatePrevious()
        {
            screenId--;
            if (screenId < 0)
            {
                screenId = maxScreenId;
            }
            launchCallback(screens[screenId], false);
        }

        internal void NavigateNext()
        {
            screenId++;
            if (screenId > maxScreenId)
            {
                screenId = 0;
            }
            launchCallback(screens[screenId], true);
        }

        internal void RegisterView(Type viewType)
        {
            screens.Add(viewType);
            maxScreenId = screens.Count - 1;
        }

        internal void ProximityMeasurement(int measurement1, int measurement2, int average1, int average2, ref bool shouldDebounce)
        {
            if (measurement1 > 410)
            {
                NavigatePrevious();
                shouldDebounce = true;
                return;
            }
            else if (measurement2 > 410)
            {
                NavigateNext();
                shouldDebounce = true;
                return;
            }
            if (average1 > 320)
            {
                NavigatePrevious();
                shouldDebounce = true;
                return;
            }
            else if (average2 > 320)
            {
                NavigateNext();
                shouldDebounce = true;
                return;
            }
        }
    }
}
