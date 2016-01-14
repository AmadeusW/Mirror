using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Weather
{
    internal class WeatherModel_fake : WeatherModel
    {
        private Random random = new Random();

        public override TimeSpan Interval => TimeSpan.FromSeconds(10);

        public override async Task Update()
        {
            Sunrise = DateTime.Parse("2015-11-26 07:30:00") + TimeSpan.FromMinutes(random.NextDouble() * 30);
            Sunset = DateTime.Parse("2015-11-26 16:30:00") + TimeSpan.FromMinutes(random.NextDouble() * 30);
            var dailyForecast = new List<WeatherDetailsModel>();
            var hourlyForecast = new List<WeatherDetailsModel>();
            for (int i = 0; i < 24; i++)
            {
                hourlyForecast.Add(getRandomConditions(DateTime.Parse("2015-11-26 07:30:00") + TimeSpan.FromHours(i)));
            }
            for (int i = 0; i < 10; i++)
            {
                dailyForecast.Add(getRandomConditions(DateTime.Parse("2015-11-26 07:30:00") + TimeSpan.FromDays(i)));
            }
            DailyForecast = dailyForecast;
            HourlyForecast = hourlyForecast;
        }

        private WeatherDetailsModel getRandomConditions(DateTime time)
        {
            var temperature = 15 + getRandomInt(15);
            var rainfall = Math.Max(0, getRandomInt(50) - 40);
            var snowfall = Math.Max(0, getRandomInt(20) - 18);
            return new WeatherDetailsModel()
            {
                Conditions = snowfall > 0 ? "Snow" : rainfall > 0 ? "Rain" : "Sunny",
                Time = time,
                Rainfall = rainfall,
                Snowfall = snowfall,
                Temperature = temperature,
                TemperatureHigh = temperature + getRandomInt(2),
                TemperatureLow = temperature - getRandomInt(2),
            };
        }
        
        private int getRandomInt(int maxValue)
        {
            return (int)(random.NextDouble() * maxValue);
        }
    }
}
