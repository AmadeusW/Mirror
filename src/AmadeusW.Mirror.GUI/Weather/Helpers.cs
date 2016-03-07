using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Weather
{
    internal class Helpers
    {
        internal static HourlyForecastViewModel GetCurrentWeather(WeatherModel model)
        {
            var currentWeather = model.HourlyForecast.FirstOrDefault();
            if (currentWeather == null)
            {
                return null;
            }
            var hourlyForecast = new HourlyForecastViewModel()
            {
                Conditions = currentWeather.Conditions,
                Time = Int32.Parse(currentWeather.Time.ToString("hh")),
                AmPm = currentWeather.Time.ToString("tt").ToLower(),
                Temperature = currentWeather.Temperature.Value,
                Rainfall = currentWeather.Rainfall.Value,
                Snowfall = currentWeather.Snowfall.Value,
            };
            return hourlyForecast;
        }

        internal static List<DailyForecastViewModel> GetDailyForecast(WeatherModel model)
        {
            var dailyForecast = new List<DailyForecastViewModel>();
            foreach (var forecast in model.DailyForecast.Take(6))
            {
                dailyForecast.Add(new DailyForecastViewModel()
                {
                    Conditions = forecast.Conditions,
                    Date = forecast.Time.ToString("%d"),
                    DayOfWeek = forecast.Time.ToString("dddd"),
                    TemperatureHigh = forecast.TemperatureHigh.Value,
                    TemperatureLow = forecast.TemperatureLow.Value
                });
            }
            return dailyForecast;
        }

        private void test()
        {
            var x = DateTime.Now;
            var z = x.ToString("%d");
            var zz = x.ToString("tt");
        }

        internal static string GetRainForecast(WeatherModel model)
        {
            var currentWeather = model.HourlyForecast.FirstOrDefault();
            if (currentWeather == null)
            {
                return String.Empty;
            }
            var futureWeather = model.HourlyForecast.Skip(1);
            var rainsNow = currentWeather.Rainfall > 0;
            if (rainsNow)
            {
                var firstSunnyWeather = futureWeather.FirstOrDefault(w => w.Rainfall == 0);
                // later, change First to FirstAndDefault and add this line:
                if (firstSunnyWeather == null)
                    return String.Empty;
                return $"Rain ends at {firstSunnyWeather.Time.ToString("h tt").ToLower()}";
            }
            else
            {
                var firstRainyWeather = futureWeather.FirstOrDefault(w => w.Rainfall > 0);
                // later, change First to FirstAndDefault and add this line:
                if (firstRainyWeather == null)
                    return String.Empty;
                return $"Rain starts at {firstRainyWeather.Time.ToString("h tt").ToLower()}";
            }
        }

        internal static List<HourlyForecastViewModel> GetHourlyForecast(WeatherModel model)
        {
            var hourlyForecast = new List<HourlyForecastViewModel>();
            foreach (var forecast in model.HourlyForecast.Skip(1).Where((t, i) => i % 2 == 0)) // the first entry is current weather. Get only every other hour
            {
                hourlyForecast.Add(new HourlyForecastViewModel()
                {
                    Conditions = forecast.Conditions,
                    Time = Int32.Parse(forecast.Time.ToString("hh")),
                    AmPm = forecast.Time.ToString("tt").ToLower(),
                    Temperature = forecast.Temperature.Value,
                    Rainfall = forecast.Rainfall.Value,
                    Snowfall = forecast.Snowfall.Value,
                });
            }
            return hourlyForecast;
        }
    }
}
