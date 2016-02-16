using System;
using System.Collections.Generic;

namespace AmadeusW.Mirror.GUI.Weather
{
    public class WeatherThisWeekViewModel : PropertyChangedBase
    {
        private bool initialized;
        public bool Initialized
        {
            get
            {
                return initialized;
            }
            set
            {
                if (initialized != value)
                {
                    initialized = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string lastUpdated;
        public string LastUpdated
        {
            get
            {
                return lastUpdated;
            }
            set
            {
                if (lastUpdated != value)
                {
                    lastUpdated = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private HourlyForecastViewModel currentWeather;
        public HourlyForecastViewModel CurrentWeather
        {
            get
            {
                return currentWeather;
            }
            set
            {
                if (currentWeather != value)
                {
                    currentWeather = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string rainForecast;
        public string RainForecast
        {
            get
            {
                return rainForecast;
            }
            set
            {
                if (rainForecast != value)
                {
                    rainForecast = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private IEnumerable<DailyForecastViewModel> forecast;
        public IEnumerable<DailyForecastViewModel> Forecast
        {
            get
            {
                return forecast;
            }
            set
            {
                if (forecast != value)
                {
                    forecast = value;
                    NotifyPropertyChanged();
                }
            }
        }



        private WeatherModel model;
        internal void Initialize(WeatherModel model)
        {
            this.model = model;
            model.PropertyChanged += ModelPropertyChanged;
            if (model.Ready)
            {
                updateDailyForecast();
                updateCurrentWeather();
                updateTimestamp();
            }
        }

        private void ModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.DailyForecast))
            {
                updateDailyForecast();
                updateTimestamp();
            }
            else if (e.PropertyName == nameof(model.HourlyForecast))
            {
                updateCurrentWeather();
                updateTimestamp();
            }
        }

        private void updateTimestamp()
        {
            Initialized = true;
            LastUpdated = DateTime.Now.ToString("h:mm");
        }

        private void updateCurrentWeather()
        {
            CurrentWeather = Helpers.GetCurrentWeather(model);
            RainForecast = Helpers.GetRainForecast(model);
        }

        private void updateDailyForecast()
        {
            Forecast = Helpers.GetDailyForecast(model);
        }

    }
}
