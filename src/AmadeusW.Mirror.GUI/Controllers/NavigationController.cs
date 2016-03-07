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
        private Action<Type> launchCallback;
        int screenId;
        int maxScreenId;

        public NavigationController(Action<Type> launchScreenCallback)
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
            launchCallback(screens[screenId]);
        }

        internal void NavigateNext()
        {
            screenId++;
            if (screenId > maxScreenId)
            {
                screenId = 0;
            }
            launchCallback(screens[screenId]);
        }

        internal void RegisterView(Type viewType)
        {
            screens.Add(viewType);
            maxScreenId = screens.Count - 1;
        }
    }
}
