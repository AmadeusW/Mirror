using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace AmadeusW.Mirror.GUI.Weather
{
    /// <summary>
    /// WeatherModel that takes its data from http://www.wunderground.com/
    /// </summary>
    internal class WeatherModel_wunderground : WeatherModel
    {
        public override TimeSpan Interval => TimeSpan.FromMinutes(15);

        public override async Task Update()
        {
            // Decide which APIs to hit.
            // Astronomy can be checked once a day
            // Hourly forecast should be checked frequently
            // 10 day forecase can be checked every couple hours
            await getWeatherData();
            updateWithHourlyData();
            updateWith10DayData();
        }

        private async Task getWeatherData()
        {
            var request = WebRequest.Create("http://api.wunderground.com/api/9539133219291752/forecast/q/Canada/Vancouver.json");
            var response = await request.GetResponseAsync();
        }

        private void updateWith10DayData()
        {
            throw new NotImplementedException();
        }

        private void updateWithHourlyData()
        {
            throw new NotImplementedException();
        }
    }
}
