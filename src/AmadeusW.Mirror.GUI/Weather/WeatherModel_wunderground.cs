using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using AmadeusW.Mirror.GUI.Controllers;
using System.Net.Http;

namespace AmadeusW.Mirror.GUI.Weather
{
    /// <summary>
    /// WeatherModel that takes its data from http://www.wunderground.com/
    /// </summary>
    internal class WeatherModel_wunderground : WeatherModel
    {
        private string _rawResponse10Day;
        private string _rawResponseHourly;
        private string _rawResponseAstronomy;
        private string _apiToken;

        public override TimeSpan Interval => TimeSpan.FromMinutes(15);

        public WeatherModel_wunderground() : base()
        {
            if (SettingsController.Settings != null)
            {
                if (SettingsController.Settings.WundergroundApi == null)
                {
                    throw new InvalidOperationException($"{nameof(WeatherModel_wunderground)} requires \"WundergroundApi\" entry in settings.json");
                }
                _apiToken = SettingsController.Settings.WundergroundApi.ToString();
            }
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
            updateAstronomy(_rawResponseAstronomy);
            Ready = true;
        }

        private async Task getWeatherData()
        {
            using (var client = new HttpClient())
            {
                var requestHourly = new HttpRequestMessage(HttpMethod.Get, $"http://api.wunderground.com/api/{_apiToken}/hourly/q/Canada/Vancouver.json");
                var taskHourly = client.SendAsync(requestHourly);
                
                var request10Day = new HttpRequestMessage(HttpMethod.Get, $"http://api.wunderground.com/api/{_apiToken}/forecast10day/q/Canada/Vancouver.json");
                var task10Day = client.SendAsync(request10Day);

                var requestAstronomy = new HttpRequestMessage(HttpMethod.Get, $"http://api.wunderground.com/api/{_apiToken}/astronomy/q/Canada/Vancouver.json");
                var taskAstronomy = client.SendAsync(requestAstronomy);

                await Task.WhenAll(taskHourly, task10Day, taskAstronomy);
                _rawResponseHourly = await taskHourly.Result.Content.ReadAsStringAsync();
                _rawResponse10Day = await task10Day.Result.Content.ReadAsStringAsync();
                _rawResponseAstronomy = await taskAstronomy.Result.Content.ReadAsStringAsync();
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
                var forecast = new WeatherDetailsModel()
                {
                    Conditions = daily["conditions"].ToString(),
                    TemperatureHigh = Int32.Parse(daily["high"]["celsius"].ToString()),
                    TemperatureLow = Int32.Parse(daily["low"]["celsius"].ToString()),
                    Rainfall = Int32.Parse(daily["qpf_allday"]["mm"].ToString()),
                    Snowfall = Int32.Parse(daily["snow_allday"]["cm"].ToString()),
                    Time = getDateTimeFromDate(daily["date"])
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
                var forecast = new WeatherDetailsModel()
                {
                    Conditions = hourly["condition"].ToString(),
                    Temperature = Int32.Parse(hourly["temp"]["metric"].ToString()),
                    Rainfall = Int32.Parse(hourly["qpf"]["metric"].ToString()),
                    Snowfall = Int32.Parse(hourly["snow"]["metric"].ToString()),
                    Time = getDateTimeFromFCTTIME(hourly["FCTTIME"])
                };
                hourlyForecast.Add(forecast);
            }
            HourlyForecast = hourlyForecast;
        }

        // I swear WUNDERGROUND is the worst weather API ever. The can't even agree on the time format.

        private DateTime getDateTimeFromFCTTIME(JToken rawTime)
        {
            var dataString = $"{rawTime["hour_padded"]}:{rawTime["min"]} {rawTime["year"]}.{rawTime["mon_padded"]}.{rawTime["mday_padded"]}";
            var formatString = "HH:mm yyyy.MM.dd";
            return DateTime.ParseExact(dataString, formatString, System.Globalization.CultureInfo.InvariantCulture);
        }

        private DateTime getDateTimeFromDate(JToken rawDate)
        {
            var fixedHour = rawDate["hour"].ToString().PadLeft(2, '0');
            var fixedMonth = rawDate["month"].ToString().PadLeft(2, '0');
            var fixedDay = rawDate["day"].ToString().PadLeft(2, '0');
            var dataString = $"{fixedHour}:{rawDate["min"]} {rawDate["year"]}.{fixedMonth}.{fixedDay}";
            var formatString = "HH:mm yyyy.MM.dd";
            return DateTime.ParseExact(dataString, formatString, System.Globalization.CultureInfo.InvariantCulture);
        }

        private void updateAstronomy(string response)
        {
            var json = JObject.Parse(response);
            JToken astronomyRoot;
            if (!json.TryGetValue("moon_phase", out astronomyRoot))
            {
                var tc = new Microsoft.ApplicationInsights.TelemetryClient();
                var properties = new Dictionary<String, string> { { "response", response } };
                tc.TrackEvent($"Unexpected response in {nameof(updateAstronomy)}", properties);
                return;
            }
            var rawSunrise = astronomyRoot["sunrise"];
            var rawSunset = astronomyRoot["sunset"];
            // Date doesn't matter, we only care about hours and minutes
            Sunrise = DateTime.Parse(rawSunrise["hour"]+":"+ rawSunrise["minute"]);
            Sunset = DateTime.Parse(rawSunset["hour"] + ":" + rawSunset["minute"]);
        }
    }
}
