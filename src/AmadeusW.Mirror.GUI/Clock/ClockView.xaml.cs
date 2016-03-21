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

namespace AmadeusW.Mirror.GUI.Clock
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClockView : Page
    {
        public ClockView()
        {
            this.InitializeComponent();
        }

        private void Proximity_OnMeasurement(int measurement1, int measurement2, ref bool shouldDebounce)
        {
            Measurement1.Text = measurement1.ToString();
            Measurement2.Text = measurement2.ToString();
            Interval.Text = Controllers.ProximityController.Instance.Interval.TotalMilliseconds.ToString() + " ms";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Controllers.ProximityController.Instance != null)
                Controllers.ProximityController.Instance.OnMeasurement += Proximity_OnMeasurement;

            if (!(e.Parameter is bool))
                return;
            bool navigatingRight = (bool)e.Parameter;
            EntranceAnimation.FromHorizontalOffset = navigatingRight ? 300 : -300;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (Controllers.ProximityController.Instance != null)
                Controllers.ProximityController.Instance.OnMeasurement -= Proximity_OnMeasurement;

            if (!(e.Parameter is bool))
                return;
            bool navigatingRight = (bool)e.Parameter;
            EntranceAnimation.FromHorizontalOffset = navigatingRight ? -300 : 300;
        }
    }
}
