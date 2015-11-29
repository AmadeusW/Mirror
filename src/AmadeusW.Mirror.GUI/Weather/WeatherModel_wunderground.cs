using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Weather
{
    /// <summary>
    /// WeatherModel that takes its data from http://www.wunderground.com/
    /// </summary>
    internal class WeatherModel_wunderground : WeatherModel
    {
        public override TimeSpan Interval => TimeSpan.FromMinutes(15);

        public override void Update()
        {
            // Decide which APIs to hit.
            // Astronomy can be checked once a day
            // Hourly forecast should be checked frequently
            // 10 day forecase can be checked every couple hours
        }
    }
}
