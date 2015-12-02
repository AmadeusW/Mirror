using System;
using System.Collections.Generic;

namespace AmadeusW.Mirror.GUI.Weather
{
    public class WeatherTodayViewModel : PropertyChangedBase
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

        private IEnumerable<HourlyForecastViewModel> forecast;
        public IEnumerable<HourlyForecastViewModel> Forecast
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

        private string sunrise;
        public string Sunrise
        {
            get
            {
                return sunrise;
            }
            set
            {
                if (sunrise != value)
                {
                    sunrise = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string sunset;
        public string Sunset
        {
            get
            {
                return sunset;
            }
            set
            {
                if (sunset != value)
                {
                    sunset = value;
                    NotifyPropertyChanged();
                }
            }
        }



        private WeatherModel model;
        internal void Initialize(WeatherModel model)
        {
            this.model = model;
            updateTimestamp();
            updateCurrentAndHourlyForecast();
            updateAstronomy();
            model.PropertyChanged += ModelPropertyChanged;
        }

        private void ModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.HourlyForecast))
            {
                updateTimestamp();
                updateCurrentAndHourlyForecast();
            }
            else if (e.PropertyName == nameof(model.Sunrise) || e.PropertyName == nameof(model.Sunset))
            {
                updateTimestamp();
                updateAstronomy();
            }
        }

        private void updateTimestamp()
        {
            Initialized = true;
            LastUpdated = DateTime.Now.ToString("h:mm");
        }

        private void updateCurrentAndHourlyForecast()
        {
            CurrentWeather = Helpers.GetCurrentWeather(model);
            RainForecast = Helpers.GetRainForecast(model);
            Forecast = Helpers.GetHourlyForecast(model);
        }

        private void updateAstronomy()
        {
            Sunrise = model.Sunrise.ToString("h:mm");
            Sunset = model.Sunset.ToString("h:mm");
        }
    }
}
