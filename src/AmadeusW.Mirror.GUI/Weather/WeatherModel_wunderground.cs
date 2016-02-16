using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using AmadeusW.Mirror.GUI.Controllers;

namespace AmadeusW.Mirror.GUI.Weather
{
    /// <summary>
    /// WeatherModel that takes its data from http://www.wunderground.com/
    /// </summary>
    internal class WeatherModel_wunderground : WeatherModel
    {
        private string _rawResponse10Day;
        private string _rawResponseHourly;
        private string _apiToken;

        public override TimeSpan Interval => TimeSpan.FromMinutes(15);

        public WeatherModel_wunderground() : base()
        {
            _apiToken = SettingsController.Settings.WundergroundApi.ToString();
            DailyForecast = new List<WeatherDetailsModel>();
            HourlyForecast = new List<WeatherDetailsModel>();
        }

        public override async Task Update()
        {
            // Decide which APIs to hit.
            // Astronomy can be checked once a day
            // Hourly forecast should be checked frequently
            // 10 day forecase can be checked every couple hours
            await getWeatherData();
            updateWithHourlyData(_rawResponseHourly);
            updateWith10DayData(_rawResponse10Day);
        }

        private async Task getWeatherData()
        {
            var requestHourly = WebRequest.Create($"http://api.wunderground.com/api/{_apiToken}/hourly/q/Canada/Vancouver.json");
            using (var response = await requestHourly.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    _rawResponseHourly = await reader.ReadToEndAsync();
                }
            }
            var request10Day = WebRequest.Create($"http://api.wunderground.com/api/{_apiToken}/forecast10day/q/Canada/Vancouver.json");
            using (var response = await request10Day.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    _rawResponse10Day = await reader.ReadToEndAsync();
                }
            }
        }

        private void updateWith10DayData(string response)
        {
            var json = JObject.Parse(response);
            JToken forecastToken;
            if (!json.TryGetValue("forecast", out forecastToken))
            {
                var tc = new Microsoft.ApplicationInsights.TelemetryClient();
                var properties = new Dictionary<String, string> { { "response", response } };
                tc.TrackEvent($"Unexpected response in {nameof(updateWith10DayData)}", properties);
                return;
            }
            var allDaily = forecastToken["simpleforecast"]["forecastday"].Children();
            var dailyForecast = new List<WeatherDetailsModel>();
            foreach (var daily in allDaily.Take(10))
            {
                var rawEpoch = Int64.Parse(daily["date"]["epoch"].ToString());
                var epoch = DateTimeOffset.FromUnixTimeSeconds(rawEpoch);
                var forecast = new WeatherDetailsModel()
                {
                    Conditions = daily["conditions"].ToString(),
                    TemperatureHigh = Int32.Parse(daily["high"]["celsius"].ToString()),
                    TemperatureLow = Int32.Parse(daily["low"]["celsius"].ToString()),
                    Rainfall = Int32.Parse(daily["qpf_allday"]["mm"].ToString()),
                    Snowfall = Int32.Parse(daily["snow_allday"]["cm"].ToString()),
                    Time = epoch.DateTime,
                };
                dailyForecast.Add(forecast);
            }
            DailyForecast = dailyForecast;
        }

        private void updateWithHourlyData(string response)
        {
            var json = JObject.Parse(response);
            JToken allHourly;
            if (!json.TryGetValue("hourly_forecast", out allHourly))
            {
                var tc = new Microsoft.ApplicationInsights.TelemetryClient();
                var properties = new Dictionary<String, string> { { "response", response } };
                tc.TrackEvent($"Unexpected response in {nameof(updateWithHourlyData)}", properties);
                return;
            }
            var hourlyForecast = new List<WeatherDetailsModel>();
            foreach (var hourly in allHourly.Take(24))
            {
                var rawEpoch = Int64.Parse(hourly["FCTTIME"]["epoch"].ToString());
                var epoch = DateTimeOffset.FromUnixTimeSeconds(rawEpoch);
                var forecast = new WeatherDetailsModel()
                {
                    Conditions = hourly["condition"].ToString(),
                    Temperature = Int32.Parse(hourly["temp"]["metric"].ToString()),
                    Rainfall = Int32.Parse(hourly["qpf"]["metric"].ToString()),
                    Snowfall = Int32.Parse(hourly["snow"]["metric"].ToString()),
                    Time = epoch.DateTime,
                };
                hourlyForecast.Add(forecast);
            }
            HourlyForecast = hourlyForecast;
        }
    }
}
