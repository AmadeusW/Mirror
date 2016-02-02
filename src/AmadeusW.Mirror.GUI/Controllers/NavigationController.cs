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
            if (args.VirtualKey == Windows.System.VirtualKey.Right)
            {
                navigateNext();
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.Left)
            {
                navigatePrevious();
            }
        }

        private void navigatePrevious()
        {
            screenId--;
            if (screenId < 0)
            {
                screenId = maxScreenId;
            }
            launchCallback(screens[screenId]);
        }

        private void navigateNext()
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
