using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AmadeusW.Mirror.GUI.Weather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherTodayView : Page
    {
        public WeatherTodayView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter is bool))
                return;
            bool navigatingRight = (bool)e.Parameter;
            EntranceAnimation.FromHorizontalOffset = navigatingRight ? 300 : -300;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (!(e.Parameter is bool))
                return;
            bool navigatingRight = (bool)e.Parameter;
            EntranceAnimation.FromHorizontalOffset = navigatingRight ? -300 : 300;
        }
    }
}
