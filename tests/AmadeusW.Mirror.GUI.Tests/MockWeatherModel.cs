using AmadeusW.Mirror.GUI.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Tests
{
    internal class MockWeatherModel : WeatherModel
    {
        public override TimeSpan Interval => TimeSpan.FromMinutes(15);

        public IEnumerable<WeatherDetailsModel> MockDailyForecast { get; set; }
        public IEnumerable<WeatherDetailsModel> MockHourlyForecast { get; set; }
        public DateTime MockSunrise { get; set; }
        public DateTime MockSunset { get; set; }

        public override async Task Update()
        {
            Sunrise = MockSunrise;
            Sunset = MockSunset;
            DailyForecast = MockDailyForecast;
            HourlyForecast = MockHourlyForecast;
        }
    }
}
