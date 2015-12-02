using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Weather
{
    abstract class WeatherModel : BaseModel 
    {
        private IEnumerable<WeatherDetailsModel> dailyForecast;
        public IEnumerable<WeatherDetailsModel> DailyForecast
        {
            get
            {
                return dailyForecast;
            }
            set
            {
                if (dailyForecast != value)
                {
                    dailyForecast = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private IEnumerable<WeatherDetailsModel> hourlyForecast;
        public IEnumerable<WeatherDetailsModel> HourlyForecast
        {
            get
            {
                return hourlyForecast;
            }
            set
            {
                if (hourlyForecast != value)
                {
                    hourlyForecast = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime sunrise;
        public DateTime Sunrise
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

        private DateTime sunset;
        public DateTime Sunset
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
    }
}
