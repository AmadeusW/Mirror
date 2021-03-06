﻿using AmadeusW.Mirror.GUI.Clock;
using AmadeusW.Mirror.GUI.Controllers;
using AmadeusW.Mirror.GUI.Transit;
using AmadeusW.Mirror.GUI.Weather;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AmadeusW.Mirror.GUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.PageView |
                Microsoft.ApplicationInsights.WindowsCollectors.UnhandledException |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private void launchScreenCallback(Type screenToLaunch, bool navigatingRight)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(screenToLaunch, navigatingRight);
        }

        private List<Type> availableScreens = new List<Type>();

        private void TimeOfDayChangedHandler(bool nightFall)
        {
            (Window.Current.Content as ThemeAwareFrame).AppTheme = nightFall ? ElementTheme.Dark : ElementTheme.Light;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            var tc = new Microsoft.ApplicationInsights.TelemetryClient();

            var navigation = new NavigationController(launchScreenCallback);
            CoreWindow.GetForCurrentThread().KeyDown += navigation.GlobalKeyDown;

            await SettingsController.LoadSettings();

            try
            {
                var clockModel = new ClockModel();
                Task.Run(() => clockModel.Update());
                (Resources["clockViewModel"] as ClockViewModel).Initialize(clockModel);
                TimerController.RegisterModel(clockModel);
                navigation.RegisterView(typeof(ClockView));
                clockModel.NightFallDelegate += TimeOfDayChangedHandler;
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<String, string> { { "Module", "Clock" } };
                tc.TrackException(ex, properties);
                System.Diagnostics.Debugger.Break();
            }

            try
            { 
                var weatherModel = new WeatherModel_wunderground();
                Task.Run(() => weatherModel.Update());
                TimerController.RegisterModel(weatherModel);
                (Resources["weatherThisWeekViewModel"] as WeatherThisWeekViewModel).Initialize(weatherModel);
                (Resources["weatherTodayViewModel"] as WeatherTodayViewModel).Initialize(weatherModel);
                navigation.RegisterView(typeof(WeatherThisWeekView));
                navigation.RegisterView(typeof(WeatherTodayView));
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<String, string> { { "Module", "Weather" } };
                tc.TrackException(ex, properties);
                System.Diagnostics.Debugger.Break();
            }

            try
            { 
                var transitModel = new TransitModel_translink();
                Task.Run(() => transitModel.Update());
                TimerController.RegisterModel(transitModel);
                (Resources["transitViewModel"] as TransitViewModel).Initialize(transitModel);
                TimerController.RegisterViewModel((Resources["transitViewModel"] as TransitViewModel));
                navigation.RegisterView(typeof(TransitView));
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<String, string> { { "Module", "Transit" } };
                tc.TrackException(ex, properties);
                System.Diagnostics.Debugger.Break();
            }

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new ThemeAwareFrame(ElementTheme.Light);

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(ClockView), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
            tc.TrackEvent("Smart Mirror has loaded.");

            setupAutoScroll(navigation);
        }

        private void setupAutoScroll(NavigationController navigation)
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += (s, e) => { navigation.NavigateNext(); };
            timer.Start();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            // TODO: Handle gracefully and log
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
