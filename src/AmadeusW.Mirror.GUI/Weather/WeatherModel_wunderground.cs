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

        public override TimeSpan Interval => TimeSpan.FromMinutes(15);

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
            dynamic apiToken = SettingsController.Settings.WundergroundApi;
            var requestHourly = WebRequest.Create($"http://api.wunderground.com/api/{apiToken}/hourly/q/Canada/Vancouver.json");
            using (var response = await requestHourly.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    _rawResponseHourly = await reader.ReadToEndAsync();
                }
            }
            var request10Day = WebRequest.Create($"http://api.wunderground.com/api/{apiToken}/forecast10day/q/Canada/Vancouver.json");
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
            var allDaily = json["forecast"]["simpleforecast"]["forecastday"].Children();
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
            var allHourly = json["hourly_forecast"];
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

        private static async Task Alive()
        {
            var x = new WeatherModel_wunderground();
            x.updateWithHourlyData(_cached1);
            x.updateWith10DayData(_cached2);
        }

        static string _cached1 = @"
{
  ""response"": {
  ""version"":""0.1"",
  ""termsofService"":""http://www.wunderground.com/weather/api/d/terms.html"",
  ""features"": {
  ""hourly"": 1
  }
}
		,
	""hourly_forecast"": [
		{
		""FCTTIME"": {
		""hour"": ""12"",""hour_padded"": ""12"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450900800"",""pretty"": ""12:00 PM PST on December 23, 2015"",""civil"": ""12:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""41"", ""metric"": ""5""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""98"",
		""wspd"": {""english"": ""12"", ""metric"": ""19""},
		""wdir"": {""dir"": ""SE"", ""degrees"": ""140""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""89"",
		""windchill"": {""english"": ""34"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""34"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.0"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""50"",
		""mslp"": {""english"": ""29.39"", ""metric"": ""995""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""13"",""hour_padded"": ""13"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450904400"",""pretty"": ""1:00 PM PST on December 23, 2015"",""civil"": ""1:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""42"", ""metric"": ""6""},
		""dewpoint"": {""english"": ""39"", ""metric"": ""4""},
		""condition"": ""Rain"",
		""icon"": ""rain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/rain.gif"",
		""fctcode"": ""13"",
		""sky"": ""100"",
		""wspd"": {""english"": ""14"", ""metric"": ""23""},
		""wdir"": {""dir"": ""SSE"", ""degrees"": ""157""},
		""wx"": ""Rain"",
		""uvi"": ""0"",
		""humidity"": ""89"",
		""windchill"": {""english"": ""36"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""36"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""59"",
		""mslp"": {""english"": ""29.38"", ""metric"": ""995""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""14"",""hour_padded"": ""14"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450908000"",""pretty"": ""2:00 PM PST on December 23, 2015"",""civil"": ""2:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""42"", ""metric"": ""6""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Rain"",
		""icon"": ""rain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/rain.gif"",
		""fctcode"": ""13"",
		""sky"": ""100"",
		""wspd"": {""english"": ""15"", ""metric"": ""24""},
		""wdir"": {""dir"": ""SSE"", ""degrees"": ""167""},
		""wx"": ""Rain"",
		""uvi"": ""0"",
		""humidity"": ""87"",
		""windchill"": {""english"": ""35"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""35"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.04"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""80"",
		""mslp"": {""english"": ""29.37"", ""metric"": ""995""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""15"",""hour_padded"": ""15"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450911600"",""pretty"": ""3:00 PM PST on December 23, 2015"",""civil"": ""3:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""42"", ""metric"": ""6""},
		""dewpoint"": {""english"": ""39"", ""metric"": ""4""},
		""condition"": ""Overcast"",
		""icon"": ""cloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/cloudy.gif"",
		""fctcode"": ""4"",
		""sky"": ""91"",
		""wspd"": {""english"": ""13"", ""metric"": ""21""},
		""wdir"": {""dir"": ""S"", ""degrees"": ""170""},
		""wx"": ""Cloudy"",
		""uvi"": ""0"",
		""humidity"": ""86"",
		""windchill"": {""english"": ""36"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""36"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.0"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""22"",
		""mslp"": {""english"": ""29.35"", ""metric"": ""994""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""16"",""hour_padded"": ""16"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450915200"",""pretty"": ""4:00 PM PST on December 23, 2015"",""civil"": ""4:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""41"", ""metric"": ""5""},
		""dewpoint"": {""english"": ""39"", ""metric"": ""4""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""90"",
		""wspd"": {""english"": ""14"", ""metric"": ""23""},
		""wdir"": {""dir"": ""SSE"", ""degrees"": ""166""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""91"",
		""windchill"": {""english"": ""33"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""44"",
		""mslp"": {""english"": ""29.34"", ""metric"": ""994""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""17"",""hour_padded"": ""17"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450918800"",""pretty"": ""5:00 PM PST on December 23, 2015"",""civil"": ""5:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""41"", ""metric"": ""5""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""90"",
		""wspd"": {""english"": ""16"", ""metric"": ""26""},
		""wdir"": {""dir"": ""SSE"", ""degrees"": ""166""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""92"",
		""windchill"": {""english"": ""33"", ""metric"": ""0""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""0""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""50"",
		""mslp"": {""english"": ""29.35"", ""metric"": ""994""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""18"",""hour_padded"": ""18"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450922400"",""pretty"": ""6:00 PM PST on December 23, 2015"",""civil"": ""6:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""41"", ""metric"": ""5""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""92"",
		""wspd"": {""english"": ""13"", ""metric"": ""21""},
		""wdir"": {""dir"": ""SSE"", ""degrees"": ""160""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""91"",
		""windchill"": {""english"": ""34"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""34"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""59"",
		""mslp"": {""english"": ""29.36"", ""metric"": ""994""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""19"",""hour_padded"": ""19"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450926000"",""pretty"": ""7:00 PM PST on December 23, 2015"",""civil"": ""7:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""40"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of a Thunderstorm"",
		""icon"": ""chancetstorms"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancetstorms.gif"",
		""fctcode"": ""14"",
		""sky"": ""95"",
		""wspd"": {""english"": ""11"", ""metric"": ""18""},
		""wdir"": {""dir"": ""SSE"", ""degrees"": ""155""},
		""wx"": ""Scattered Thunderstorms"",
		""uvi"": ""0"",
		""humidity"": ""90"",
		""windchill"": {""english"": ""32"", ""metric"": ""0""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""32"", ""metric"": ""0""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""58"",
		""mslp"": {""english"": ""29.37"", ""metric"": ""995""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""20"",""hour_padded"": ""20"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450929600"",""pretty"": ""8:00 PM PST on December 23, 2015"",""civil"": ""8:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""40"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""97"",
		""wspd"": {""english"": ""10"", ""metric"": ""16""},
		""wdir"": {""dir"": ""SE"", ""degrees"": ""139""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""92"",
		""windchill"": {""english"": ""33"", ""metric"": ""0""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""0""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""44"",
		""mslp"": {""english"": ""29.37"", ""metric"": ""995""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""21"",""hour_padded"": ""21"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450933200"",""pretty"": ""9:00 PM PST on December 23, 2015"",""civil"": ""9:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""40"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""95"",
		""wspd"": {""english"": ""8"", ""metric"": ""13""},
		""wdir"": {""dir"": ""SE"", ""degrees"": ""140""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""93"",
		""windchill"": {""english"": ""34"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""34"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""43"",
		""mslp"": {""english"": ""29.38"", ""metric"": ""995""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""22"",""hour_padded"": ""22"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450936800"",""pretty"": ""10:00 PM PST on December 23, 2015"",""civil"": ""10:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""40"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""97"",
		""wspd"": {""english"": ""7"", ""metric"": ""11""},
		""wdir"": {""dir"": ""SE"", ""degrees"": ""127""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""93"",
		""windchill"": {""english"": ""34"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""34"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""43"",
		""mslp"": {""english"": ""29.39"", ""metric"": ""995""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""23"",""hour_padded"": ""23"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""23"",""mday_padded"": ""23"",""yday"": ""356"",""isdst"": ""0"",""epoch"": ""1450940400"",""pretty"": ""11:00 PM PST on December 23, 2015"",""civil"": ""11:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Wednesday"",""weekday_name_night"": ""Wednesday Night"",""weekday_name_abbrev"": ""Wed"",""weekday_name_unlang"": ""Wednesday"",""weekday_name_night_unlang"": ""Wednesday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""39"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""37"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""97"",
		""wspd"": {""english"": ""6"", ""metric"": ""10""},
		""wdir"": {""dir"": ""ESE"", ""degrees"": ""107""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""93"",
		""windchill"": {""english"": ""35"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""35"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""50"",
		""mslp"": {""english"": ""29.4"", ""metric"": ""996""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""0"",""hour_padded"": ""00"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450944000"",""pretty"": ""12:00 AM PST on December 24, 2015"",""civil"": ""12:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""39"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""98"",
		""wspd"": {""english"": ""7"", ""metric"": ""11""},
		""wdir"": {""dir"": ""ESE"", ""degrees"": ""103""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""96"",
		""windchill"": {""english"": ""34"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""34"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""51"",
		""mslp"": {""english"": ""29.41"", ""metric"": ""996""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""1"",""hour_padded"": ""01"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450947600"",""pretty"": ""1:00 AM PST on December 24, 2015"",""civil"": ""1:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""39"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""97"",
		""wspd"": {""english"": ""7"", ""metric"": ""11""},
		""wdir"": {""dir"": ""ESE"", ""degrees"": ""123""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""96"",
		""windchill"": {""english"": ""34"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""34"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""54"",
		""mslp"": {""english"": ""29.43"", ""metric"": ""997""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""2"",""hour_padded"": ""02"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450951200"",""pretty"": ""2:00 AM PST on December 24, 2015"",""civil"": ""2:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""39"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""96"",
		""wspd"": {""english"": ""7"", ""metric"": ""11""},
		""wdir"": {""dir"": ""ESE"", ""degrees"": ""119""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""97"",
		""windchill"": {""english"": ""34"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""34"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""52"",
		""mslp"": {""english"": ""29.45"", ""metric"": ""997""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""3"",""hour_padded"": ""03"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450954800"",""pretty"": ""3:00 AM PST on December 24, 2015"",""civil"": ""3:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""39"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""97"",
		""wspd"": {""english"": ""6"", ""metric"": ""10""},
		""wdir"": {""dir"": ""ESE"", ""degrees"": ""110""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""97"",
		""windchill"": {""english"": ""35"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""35"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""50"",
		""mslp"": {""english"": ""29.46"", ""metric"": ""998""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""4"",""hour_padded"": ""04"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450958400"",""pretty"": ""4:00 AM PST on December 24, 2015"",""civil"": ""4:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""38"", ""metric"": ""3""},
		""dewpoint"": {""english"": ""38"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""93"",
		""wspd"": {""english"": ""6"", ""metric"": ""10""},
		""wdir"": {""dir"": ""E"", ""degrees"": ""96""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""97"",
		""windchill"": {""english"": ""33"", ""metric"": ""0""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""0""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""49"",
		""mslp"": {""english"": ""29.47"", ""metric"": ""998""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""5"",""hour_padded"": ""05"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450962000"",""pretty"": ""5:00 AM PST on December 24, 2015"",""civil"": ""5:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""38"", ""metric"": ""3""},
		""dewpoint"": {""english"": ""37"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""92"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""E"", ""degrees"": ""80""},
		""wx"": ""Light Rain"",
		""uvi"": ""0"",
		""humidity"": ""98"",
		""windchill"": {""english"": ""33"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""63"",
		""mslp"": {""english"": ""29.49"", ""metric"": ""999""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""6"",""hour_padded"": ""06"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450965600"",""pretty"": ""6:00 AM PST on December 24, 2015"",""civil"": ""6:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""37"", ""metric"": ""3""},
		""dewpoint"": {""english"": ""37"", ""metric"": ""3""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""94"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""ENE"", ""degrees"": ""64""},
		""wx"": ""Light Rain"",
		""uvi"": ""0"",
		""humidity"": ""97"",
		""windchill"": {""english"": ""33"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""67"",
		""mslp"": {""english"": ""29.51"", ""metric"": ""999""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""7"",""hour_padded"": ""07"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450969200"",""pretty"": ""7:00 AM PST on December 24, 2015"",""civil"": ""7:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""37"", ""metric"": ""3""},
		""dewpoint"": {""english"": ""36"", ""metric"": ""2""},
		""condition"": ""Rain"",
		""icon"": ""rain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_rain.gif"",
		""fctcode"": ""13"",
		""sky"": ""100"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""ENE"", ""degrees"": ""59""},
		""wx"": ""Rain"",
		""uvi"": ""0"",
		""humidity"": ""95"",
		""windchill"": {""english"": ""33"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.03"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""62"",
		""mslp"": {""english"": ""29.53"", ""metric"": ""1000""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""8"",""hour_padded"": ""08"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450972800"",""pretty"": ""8:00 AM PST on December 24, 2015"",""civil"": ""8:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""37"", ""metric"": ""3""},
		""dewpoint"": {""english"": ""36"", ""metric"": ""2""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""95"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""NE"", ""degrees"": ""56""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""96"",
		""windchill"": {""english"": ""33"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.03"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""53"",
		""mslp"": {""english"": ""29.55"", ""metric"": ""1001""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""9"",""hour_padded"": ""09"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450976400"",""pretty"": ""9:00 AM PST on December 24, 2015"",""civil"": ""9:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""38"", ""metric"": ""3""},
		""dewpoint"": {""english"": ""36"", ""metric"": ""2""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""92"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""NE"", ""degrees"": ""53""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""93"",
		""windchill"": {""english"": ""33"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.03"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""49"",
		""mslp"": {""english"": ""29.58"", ""metric"": ""1002""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""10"",""hour_padded"": ""10"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450980000"",""pretty"": ""10:00 AM PST on December 24, 2015"",""civil"": ""10:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""39"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""36"", ""metric"": ""2""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""89"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""NE"", ""degrees"": ""53""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""90"",
		""windchill"": {""english"": ""36"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""36"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""49"",
		""mslp"": {""english"": ""29.61"", ""metric"": ""1003""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""11"",""hour_padded"": ""11"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450983600"",""pretty"": ""11:00 AM PST on December 24, 2015"",""civil"": ""11:00 AM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""AM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""40"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""36"", ""metric"": ""2""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""96"",
		""wspd"": {""english"": ""6"", ""metric"": ""10""},
		""wdir"": {""dir"": ""NE"", ""degrees"": ""40""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""87"",
		""windchill"": {""english"": ""35"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""35"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""48"",
		""mslp"": {""english"": ""29.62"", ""metric"": ""1003""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""12"",""hour_padded"": ""12"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450987200"",""pretty"": ""12:00 PM PST on December 24, 2015"",""civil"": ""12:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""40"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""36"", ""metric"": ""2""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""97"",
		""wspd"": {""english"": ""6"", ""metric"": ""10""},
		""wdir"": {""dir"": ""NNE"", ""degrees"": ""23""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""86"",
		""windchill"": {""english"": ""35"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""35"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.02"", ""metric"": ""1""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""48"",
		""mslp"": {""english"": ""29.64"", ""metric"": ""1004""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""13"",""hour_padded"": ""13"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450990800"",""pretty"": ""1:00 PM PST on December 24, 2015"",""civil"": ""1:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""40"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""36"", ""metric"": ""2""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""97"",
		""wspd"": {""english"": ""6"", ""metric"": ""10""},
		""wdir"": {""dir"": ""NNE"", ""degrees"": ""17""},
		""wx"": ""Showers"",
		""uvi"": ""0"",
		""humidity"": ""85"",
		""windchill"": {""english"": ""35"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""35"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""47"",
		""mslp"": {""english"": ""29.65"", ""metric"": ""1004""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""14"",""hour_padded"": ""14"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450994400"",""pretty"": ""2:00 PM PST on December 24, 2015"",""civil"": ""2:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""40"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""36"", ""metric"": ""2""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""95"",
		""wspd"": {""english"": ""6"", ""metric"": ""10""},
		""wdir"": {""dir"": ""N"", ""degrees"": ""4""},
		""wx"": ""Few Showers"",
		""uvi"": ""0"",
		""humidity"": ""84"",
		""windchill"": {""english"": ""35"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""35"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""32"",
		""mslp"": {""english"": ""29.67"", ""metric"": ""1005""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""15"",""hour_padded"": ""15"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1450998000"",""pretty"": ""3:00 PM PST on December 24, 2015"",""civil"": ""3:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""40"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""35"", ""metric"": ""2""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""91"",
		""wspd"": {""english"": ""7"", ""metric"": ""11""},
		""wdir"": {""dir"": ""N"", ""degrees"": ""352""},
		""wx"": ""Few Showers"",
		""uvi"": ""0"",
		""humidity"": ""82"",
		""windchill"": {""english"": ""34"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""34"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.01"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""31"",
		""mslp"": {""english"": ""29.69"", ""metric"": ""1005""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""16"",""hour_padded"": ""16"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1451001600"",""pretty"": ""4:00 PM PST on December 24, 2015"",""civil"": ""4:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""39"", ""metric"": ""4""},
		""dewpoint"": {""english"": ""35"", ""metric"": ""2""},
		""condition"": ""Chance of Rain"",
		""icon"": ""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""fctcode"": ""12"",
		""sky"": ""86"",
		""wspd"": {""english"": ""6"", ""metric"": ""10""},
		""wdir"": {""dir"": ""NNW"", ""degrees"": ""344""},
		""wx"": ""Few Showers"",
		""uvi"": ""0"",
		""humidity"": ""86"",
		""windchill"": {""english"": ""35"", ""metric"": ""2""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""35"", ""metric"": ""2""},
		""qpf"": {""english"": ""0.0"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""31"",
		""mslp"": {""english"": ""29.72"", ""metric"": ""1006""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""17"",""hour_padded"": ""17"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1451005200"",""pretty"": ""5:00 PM PST on December 24, 2015"",""civil"": ""5:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""38"", ""metric"": ""3""},
		""dewpoint"": {""english"": ""35"", ""metric"": ""2""},
		""condition"": ""Mostly Cloudy"",
		""icon"": ""mostlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_mostlycloudy.gif"",
		""fctcode"": ""3"",
		""sky"": ""78"",
		""wspd"": {""english"": ""6"", ""metric"": ""10""},
		""wdir"": {""dir"": ""N"", ""degrees"": ""350""},
		""wx"": ""Mostly Cloudy"",
		""uvi"": ""0"",
		""humidity"": ""89"",
		""windchill"": {""english"": ""33"", ""metric"": ""0""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""0""},
		""qpf"": {""english"": ""0.0"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""24"",
		""mslp"": {""english"": ""29.76"", ""metric"": ""1008""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""18"",""hour_padded"": ""18"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1451008800"",""pretty"": ""6:00 PM PST on December 24, 2015"",""civil"": ""6:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""37"", ""metric"": ""3""},
		""dewpoint"": {""english"": ""34"", ""metric"": ""1""},
		""condition"": ""Mostly Cloudy"",
		""icon"": ""mostlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_mostlycloudy.gif"",
		""fctcode"": ""3"",
		""sky"": ""71"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""N"", ""degrees"": ""358""},
		""wx"": ""Mostly Cloudy"",
		""uvi"": ""0"",
		""humidity"": ""88"",
		""windchill"": {""english"": ""33"", ""metric"": ""1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""33"", ""metric"": ""1""},
		""qpf"": {""english"": ""0.0"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""24"",
		""mslp"": {""english"": ""29.79"", ""metric"": ""1009""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""19"",""hour_padded"": ""19"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1451012400"",""pretty"": ""7:00 PM PST on December 24, 2015"",""civil"": ""7:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""36"", ""metric"": ""2""},
		""dewpoint"": {""english"": ""34"", ""metric"": ""1""},
		""condition"": ""Mostly Cloudy"",
		""icon"": ""mostlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_mostlycloudy.gif"",
		""fctcode"": ""3"",
		""sky"": ""66"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""N"", ""degrees"": ""4""},
		""wx"": ""Mostly Cloudy"",
		""uvi"": ""0"",
		""humidity"": ""91"",
		""windchill"": {""english"": ""31"", ""metric"": ""-0""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""31"", ""metric"": ""-0""},
		""qpf"": {""english"": ""0.0"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""24"",
		""mslp"": {""english"": ""29.82"", ""metric"": ""1010""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""20"",""hour_padded"": ""20"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1451016000"",""pretty"": ""8:00 PM PST on December 24, 2015"",""civil"": ""8:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""36"", ""metric"": ""2""},
		""dewpoint"": {""english"": ""34"", ""metric"": ""1""},
		""condition"": ""Mostly Cloudy"",
		""icon"": ""mostlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_mostlycloudy.gif"",
		""fctcode"": ""3"",
		""sky"": ""68"",
		""wspd"": {""english"": ""6"", ""metric"": ""10""},
		""wdir"": {""dir"": ""NNE"", ""degrees"": ""20""},
		""wx"": ""Mostly Cloudy"",
		""uvi"": ""0"",
		""humidity"": ""94"",
		""windchill"": {""english"": ""31"", ""metric"": ""-1""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""31"", ""metric"": ""-1""},
		""qpf"": {""english"": ""0.0"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""18"",
		""mslp"": {""english"": ""29.85"", ""metric"": ""1011""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""21"",""hour_padded"": ""21"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1451019600"",""pretty"": ""9:00 PM PST on December 24, 2015"",""civil"": ""9:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""36"", ""metric"": ""2""},
		""dewpoint"": {""english"": ""34"", ""metric"": ""1""},
		""condition"": ""Mostly Cloudy"",
		""icon"": ""mostlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_mostlycloudy.gif"",
		""fctcode"": ""3"",
		""sky"": ""71"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""NNE"", ""degrees"": ""13""},
		""wx"": ""Mostly Cloudy"",
		""uvi"": ""0"",
		""humidity"": ""94"",
		""windchill"": {""english"": ""31"", ""metric"": ""-0""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""31"", ""metric"": ""-0""},
		""qpf"": {""english"": ""0.0"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""17"",
		""mslp"": {""english"": ""29.88"", ""metric"": ""1012""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""22"",""hour_padded"": ""22"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1451023200"",""pretty"": ""10:00 PM PST on December 24, 2015"",""civil"": ""10:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""36"", ""metric"": ""2""},
		""dewpoint"": {""english"": ""34"", ""metric"": ""1""},
		""condition"": ""Mostly Cloudy"",
		""icon"": ""mostlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_mostlycloudy.gif"",
		""fctcode"": ""3"",
		""sky"": ""74"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""NE"", ""degrees"": ""43""},
		""wx"": ""Mostly Cloudy"",
		""uvi"": ""0"",
		""humidity"": ""91"",
		""windchill"": {""english"": ""31"", ""metric"": ""-0""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""31"", ""metric"": ""-0""},
		""qpf"": {""english"": ""0.0"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""17"",
		""mslp"": {""english"": ""29.9"", ""metric"": ""1013""}
		}
		,
		{
		""FCTTIME"": {
		""hour"": ""23"",""hour_padded"": ""23"",""min"": ""00"",""min_unpadded"": ""0"",""sec"": ""0"",""year"": ""2015"",""mon"": ""12"",""mon_padded"": ""12"",""mon_abbrev"": ""Dec"",""mday"": ""24"",""mday_padded"": ""24"",""yday"": ""357"",""isdst"": ""0"",""epoch"": ""1451026800"",""pretty"": ""11:00 PM PST on December 24, 2015"",""civil"": ""11:00 PM"",""month_name"": ""December"",""month_name_abbrev"": ""Dec"",""weekday_name"": ""Thursday"",""weekday_name_night"": ""Thursday Night"",""weekday_name_abbrev"": ""Thu"",""weekday_name_unlang"": ""Thursday"",""weekday_name_night_unlang"": ""Thursday Night"",""ampm"": ""PM"",""tz"": """",""age"": """",""UTCDATE"": """"
		},
		""temp"": {""english"": ""35"", ""metric"": ""2""},
		""dewpoint"": {""english"": ""33"", ""metric"": ""1""},
		""condition"": ""Mostly Cloudy"",
		""icon"": ""mostlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_mostlycloudy.gif"",
		""fctcode"": ""3"",
		""sky"": ""70"",
		""wspd"": {""english"": ""5"", ""metric"": ""8""},
		""wdir"": {""dir"": ""NNE"", ""degrees"": ""25""},
		""wx"": ""Mostly Cloudy"",
		""uvi"": ""0"",
		""humidity"": ""93"",
		""windchill"": {""english"": ""31"", ""metric"": ""-0""},
		""heatindex"": {""english"": ""-9999"", ""metric"": ""-9999""},
		""feelslike"": {""english"": ""31"", ""metric"": ""-0""},
		""qpf"": {""english"": ""0.0"", ""metric"": ""0""},
		""snow"": {""english"": ""0.0"", ""metric"": ""0""},
		""pop"": ""8"",
		""mslp"": {""english"": ""29.93"", ""metric"": ""1014""}
		}
	]
}
";




        private static string _cached2 = @"
{
  ""response"": {
  ""version"":""0.1"",
  ""termsofService"":""http://www.wunderground.com/weather/api/d/terms.html"",
  ""features"": {
  ""forecast10day"": 1
  }
}
		,
	""forecast"":{
		""txt_forecast"": {
		""date"":""9:19 AM PST"",
		""forecastday"": [
		{
		""period"":0,
		""icon"":""chancetstorms"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancetstorms.gif"",
		""title"":""Wednesday"",
		""fcttext"":""A few showers in the morning with thunderstorms developing for the afternoon.High 42F. Winds SE at 10 to 20 mph.Chance of rain 80%."",
		""fcttext_metric"":""Showers this morning, becoming a steady rain during the afternoon hours.High 6C.Winds SSE at 15 to 30 km/h.Chance of rain 80%. Rainfall around 6mm."",
		""pop"":""80""
		}
		,
		{
		""period"":1,
		""icon"":""nt_chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""title"":""Wednesday Night"",
		""fcttext"":""Showers this evening becoming a steady light rain overnight.Low 36F. Winds ESE at 10 to 15 mph.Chance of rain 70%."",
		""fcttext_metric"":""Rain showers early becoming a steady light rain overnight.Low 2C.Winds ESE at 15 to 25 km/h.Chance of rain 70%."",
		""pop"":""70""
        }
		,
		{
		""period"":2,
		""icon"":""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""title"":""Thursday"",
		""fcttext"":""Showers early becoming less numerous later in the day.High around 40F. Winds NNE at 5 to 10 mph.Chance of rain 60%."",
		""fcttext_metric"":""Showers early becoming less numerous later in the day. High around 5C.Winds N at 10 to 15 km/h.Chance of rain 60%."",
		""pop"":""60""
		}
		,
		{
		""period"":3,
		""icon"":""nt_partlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_partlycloudy.gif"",
		""title"":""Thursday Night"",
		""fcttext"":""Mostly cloudy skies early, then partly cloudy after midnight.Low 31F. Winds light and variable."",
		""fcttext_metric"":""Considerable clouds early.Some decrease in clouds late. Low around 0C.Winds light and variable."",
		""pop"":""20""
		}
		,
		{
		""period"":4,
		""icon"":""partlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/partlycloudy.gif"",
		""title"":""Friday"",
		""fcttext"":""Partly cloudy skies.High 37F. Winds light and variable."",
		""fcttext_metric"":""Intervals of clouds and sunshine.High 3C.Winds light and variable."",
		""pop"":""20""
		}
		,
		{
		""period"":5,
		""icon"":""nt_clear"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_clear.gif"",
		""title"":""Friday Night"",
		""fcttext"":""Mainly clear skies.Low 29F. Winds ENE at 5 to 10 mph."",
		""fcttext_metric"":""Clear skies with a few passing clouds.Low -2C.Winds E at 10 to 15 km/h."",
		""pop"":""0""
		}
		,
		{
		""period"":6,
		""icon"":""cloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/cloudy.gif"",
		""title"":""Saturday"",
		""fcttext"":""Overcast.High 38F. Winds ENE at 5 to 10 mph."",
		""fcttext_metric"":""Cloudy.High 4C.Winds ENE at 10 to 15 km/h."",
		""pop"":""10""
		}
		,
		{
		""period"":7,
		""icon"":""nt_partlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_partlycloudy.gif"",
		""title"":""Saturday Night"",
		""fcttext"":""Cloudy skies early, then partly cloudy after midnight.Low near 35F. Winds ENE at 5 to 10 mph."",
		""fcttext_metric"":""Cloudy skies early, then partly cloudy after midnight.Low 2C.Winds ENE at 10 to 15 km/h."",
		""pop"":""10""
		}
		,
		{
		""period"":8,
		""icon"":""cloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/cloudy.gif"",
		""title"":""Sunday"",
		""fcttext"":""Cloudy.High 41F. Winds E at 5 to 10 mph."",
		""fcttext_metric"":""Cloudy.High near 5C.Winds E at 10 to 15 km/h."",
		""pop"":""20""
		}
		,
		{
		""period"":9,
		""icon"":""nt_mostlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_mostlycloudy.gif"",
		""title"":""Sunday Night"",
		""fcttext"":""Mostly cloudy skies.Low 36F. Winds E at 5 to 10 mph."",
		""fcttext_metric"":""A few showers early with overcast skies late.Low 2C.Winds E at 10 to 15 km/h.Chance of rain 30%."",
		""pop"":""10""
		}
		,
		{
		""period"":10,
		""icon"":""partlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/partlycloudy.gif"",
		""title"":""Monday"",
		""fcttext"":""Cloudy skies early, then partly cloudy in the afternoon.High 42F. Winds N at 5 to 10 mph."",
		""fcttext_metric"":""Cloudy early with partial sunshine expected late.High 6C.Winds NE at 10 to 15 km/h."",
		""pop"":""10""
		}
		,
		{
		""period"":11,
		""icon"":""nt_partlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_partlycloudy.gif"",
		""title"":""Monday Night"",
		""fcttext"":""A few clouds.Low around 35F. Winds ENE at 5 to 10 mph."",
		""fcttext_metric"":""Partly cloudy.Low 2C.Winds E at 10 to 15 km/h."",
		""pop"":""10""
		}
		,
		{
		""period"":12,
		""icon"":""partlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/partlycloudy.gif"",
		""title"":""Tuesday"",
		""fcttext"":""Considerable clouds early.Some decrease in clouds later in the day.High 43F. Winds NE at 5 to 10 mph."",
		""fcttext_metric"":""Cloudy early with partial sunshine expected late.High 6C.Winds ENE at 10 to 15 km/h."",
		""pop"":""20""
		}
		,
		{
		""period"":13,
		""icon"":""nt_partlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_partlycloudy.gif"",
		""title"":""Tuesday Night"",
		""fcttext"":""Partly cloudy.Low 36F. Winds ENE at 5 to 10 mph."",
		""fcttext_metric"":""A few clouds.Low 2C.Winds E at 10 to 15 km/h."",
		""pop"":""20""
		}
		,
		{
		""period"":14,
		""icon"":""clear"",
		""icon_url"":""http://icons.wxug.com/i/c/k/clear.gif"",
		""title"":""Wednesday"",
		""fcttext"":""Except for a few afternoon clouds, mainly sunny.High 43F. Winds E at 5 to 10 mph."",
		""fcttext_metric"":""Partly cloudy.High 6C.Winds ENE at 10 to 15 km/h."",
		""pop"":""20""
		}
		,
		{
		""period"":15,
		""icon"":""nt_chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""title"":""Wednesday Night"",
		""fcttext"":""Partly cloudy skies early will give way to occasional showers later during the night.Low 39F. Winds ENE at 5 to 10 mph.Chance of rain 40%."",
		""fcttext_metric"":""Partly cloudy skies early followed by increasing clouds with showers developing later at night. Low 4C.Winds ENE at 10 to 15 km/h.Chance of rain 40%."",
		""pop"":""40""
		}
		,
		{
		""period"":16,
		""icon"":""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""title"":""Thursday"",
		""fcttext"":""A shower or two around the area in the morning, then partly cloudy in the afternoon.High 44F. Winds NE at 5 to 10 mph.Chance of rain 30%."",
		""fcttext_metric"":""Rain showers in the morning becoming more intermittent in the afternoon. High 7C.Winds ENE at 10 to 15 km/h.Chance of rain 40%."",
		""pop"":""30""
		}
		,
		{
		""period"":17,
		""icon"":""nt_chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""title"":""Thursday Night"",
		""fcttext"":""Partly cloudy skies early followed by increasing clouds with showers developing later at night.Low 38F. Winds ENE at 5 to 10 mph.Chance of rain 40%."",
		""fcttext_metric"":""A few clouds.Low 4C.Winds ENE at 10 to 15 km/h."",
		""pop"":""40""
		}
		,
		{
		""period"":18,
		""icon"":""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""title"":""Friday"",
		""fcttext"":""Overcast with showers at times.High 44F. Winds E at 5 to 10 mph.Chance of rain 50%."",
		""fcttext_metric"":""Partly cloudy skies during the morning hours will give way to occasional showers in the afternoon.High 7C.Winds ENE at 10 to 15 km/h.Chance of rain 40%."",
		""pop"":""50""
		}
		,
		{
		""period"":19,
		""icon"":""nt_chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/nt_chancerain.gif"",
		""title"":""Friday Night"",
		""fcttext"":""Overcast with rain showers at times.Low 41F. Winds E at 5 to 10 mph.Chance of rain 50%."",
		""fcttext_metric"":""Rain showers in the evening becoming more intermittent overnight. Low around 5C.Winds ENE at 10 to 15 km/h.Chance of rain 40%."",
		""pop"":""50""
		}
		]
		},
		""simpleforecast"": {
		""forecastday"": [
		{""date"":{
	""epoch"":""1450926000"",
	""pretty"":""7:00 PM PST on December 23, 2015"",
	""day"":23,
	""month"":12,
	""year"":2015,
	""yday"":356,
	""hour"":19,
	""min"":""00"",
	""sec"":0,
	""isdst"":""0"",
	""monthname"":""December"",
	""monthname_short"":""Dec"",
	""weekday_short"":""Wed"",
	""weekday"":""Wednesday"",
	""ampm"":""PM"",
	""tz_short"":""PST"",
	""tz_long"":""America/Vancouver""
},
		""period"":1,
		""high"": {
		""fahrenheit"":""42"",
		""celsius"":""6""
		},
		""low"": {
		""fahrenheit"":""36"",
		""celsius"":""2""
		},
		""conditions"":""Chance of a Thunderstorm"",
		""icon"":""chancetstorms"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancetstorms.gif"",
		""skyicon"":"""",
		""pop"":80,
		""qpf_allday"": {
		""in"": 0.28,
		""mm"": 7
		},
		""qpf_day"": {
		""in"": 0.11,
		""mm"": 3
		},
		""qpf_night"": {
		""in"": 0.18,
		""mm"": 5
		},
		""snow_allday"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_day"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_night"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""maxwind"": {
		""mph"": 20,
		""kph"": 32,
		""dir"": ""SE"",
		""degrees"": 146
		},
		""avewind"": {
		""mph"": 15,
		""kph"": 24,
		""dir"": ""SE"",
		""degrees"": 146
		},
		""avehumidity"": 91,
		""maxhumidity"": 0,
		""minhumidity"": 0
		}
		,
		{""date"":{
	""epoch"":""1451012400"",
	""pretty"":""7:00 PM PST on December 24, 2015"",
	""day"":24,
	""month"":12,
	""year"":2015,
	""yday"":357,
	""hour"":19,
	""min"":""00"",
	""sec"":0,
	""isdst"":""0"",
	""monthname"":""December"",
	""monthname_short"":""Dec"",
	""weekday_short"":""Thu"",
	""weekday"":""Thursday"",
	""ampm"":""PM"",
	""tz_short"":""PST"",
	""tz_long"":""America/Vancouver""
},
		""period"":2,
		""high"": {
		""fahrenheit"":""40"",
		""celsius"":""4""
		},
		""low"": {
		""fahrenheit"":""31"",
		""celsius"":""-1""
		},
		""conditions"":""Chance of Rain"",
		""icon"":""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""skyicon"":"""",
		""pop"":60,
		""qpf_allday"": {
		""in"": 0.17,
		""mm"": 4
		},
		""qpf_day"": {
		""in"": 0.17,
		""mm"": 4
		},
		""qpf_night"": {
		""in"": 0.00,
		""mm"": 0
		},
		""snow_allday"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_day"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_night"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""maxwind"": {
		""mph"": 10,
		""kph"": 16,
		""dir"": ""NNE"",
		""degrees"": 21
		},
		""avewind"": {
		""mph"": 6,
		""kph"": 10,
		""dir"": ""NNE"",
		""degrees"": 21
		},
		""avehumidity"": 89,
		""maxhumidity"": 0,
		""minhumidity"": 0
		}
		,
		{""date"":{
	""epoch"":""1451098800"",
	""pretty"":""7:00 PM PST on December 25, 2015"",
	""day"":25,
	""month"":12,
	""year"":2015,
	""yday"":358,
	""hour"":19,
	""min"":""00"",
	""sec"":0,
	""isdst"":""0"",
	""monthname"":""December"",
	""monthname_short"":""Dec"",
	""weekday_short"":""Fri"",
	""weekday"":""Friday"",
	""ampm"":""PM"",
	""tz_short"":""PST"",
	""tz_long"":""America/Vancouver""
},
		""period"":3,
		""high"": {
		""fahrenheit"":""37"",
		""celsius"":""3""
		},
		""low"": {
		""fahrenheit"":""29"",
		""celsius"":""-2""
		},
		""conditions"":""Partly Cloudy"",
		""icon"":""partlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/partlycloudy.gif"",
		""skyicon"":"""",
		""pop"":20,
		""qpf_allday"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_day"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_night"": {
		""in"": 0.00,
		""mm"": 0
		},
		""snow_allday"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_day"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_night"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""maxwind"": {
		""mph"": 10,
		""kph"": 16,
		""dir"": ""NNE"",
		""degrees"": 30
		},
		""avewind"": {
		""mph"": 5,
		""kph"": 8,
		""dir"": ""NNE"",
		""degrees"": 30
		},
		""avehumidity"": 87,
		""maxhumidity"": 0,
		""minhumidity"": 0
		}
		,
		{""date"":{
	""epoch"":""1451185200"",
	""pretty"":""7:00 PM PST on December 26, 2015"",
	""day"":26,
	""month"":12,
	""year"":2015,
	""yday"":359,
	""hour"":19,
	""min"":""00"",
	""sec"":0,
	""isdst"":""0"",
	""monthname"":""December"",
	""monthname_short"":""Dec"",
	""weekday_short"":""Sat"",
	""weekday"":""Saturday"",
	""ampm"":""PM"",
	""tz_short"":""PST"",
	""tz_long"":""America/Vancouver""
},
		""period"":4,
		""high"": {
		""fahrenheit"":""38"",
		""celsius"":""3""
		},
		""low"": {
		""fahrenheit"":""35"",
		""celsius"":""2""
		},
		""conditions"":""Overcast"",
		""icon"":""cloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/cloudy.gif"",
		""skyicon"":"""",
		""pop"":10,
		""qpf_allday"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_day"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_night"": {
		""in"": 0.00,
		""mm"": 0
		},
		""snow_allday"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_day"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_night"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""maxwind"": {
		""mph"": 10,
		""kph"": 16,
		""dir"": ""ENE"",
		""degrees"": 75
		},
		""avewind"": {
		""mph"": 8,
		""kph"": 13,
		""dir"": ""ENE"",
		""degrees"": 75
		},
		""avehumidity"": 80,
		""maxhumidity"": 0,
		""minhumidity"": 0
		}
		,
		{""date"":{
	""epoch"":""1451271600"",
	""pretty"":""7:00 PM PST on December 27, 2015"",
	""day"":27,
	""month"":12,
	""year"":2015,
	""yday"":360,
	""hour"":19,
	""min"":""00"",
	""sec"":0,
	""isdst"":""0"",
	""monthname"":""December"",
	""monthname_short"":""Dec"",
	""weekday_short"":""Sun"",
	""weekday"":""Sunday"",
	""ampm"":""PM"",
	""tz_short"":""PST"",
	""tz_long"":""America/Vancouver""
},
		""period"":5,
		""high"": {
		""fahrenheit"":""41"",
		""celsius"":""5""
		},
		""low"": {
		""fahrenheit"":""36"",
		""celsius"":""2""
		},
		""conditions"":""Overcast"",
		""icon"":""cloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/cloudy.gif"",
		""skyicon"":"""",
		""pop"":20,
		""qpf_allday"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_day"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_night"": {
		""in"": 0.00,
		""mm"": 0
		},
		""snow_allday"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_day"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_night"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""maxwind"": {
		""mph"": 10,
		""kph"": 16,
		""dir"": ""E"",
		""degrees"": 86
		},
		""avewind"": {
		""mph"": 7,
		""kph"": 11,
		""dir"": ""E"",
		""degrees"": 86
		},
		""avehumidity"": 77,
		""maxhumidity"": 0,
		""minhumidity"": 0
		}
		,
		{""date"":{
	""epoch"":""1451358000"",
	""pretty"":""7:00 PM PST on December 28, 2015"",
	""day"":28,
	""month"":12,
	""year"":2015,
	""yday"":361,
	""hour"":19,
	""min"":""00"",
	""sec"":0,
	""isdst"":""0"",
	""monthname"":""December"",
	""monthname_short"":""Dec"",
	""weekday_short"":""Mon"",
	""weekday"":""Monday"",
	""ampm"":""PM"",
	""tz_short"":""PST"",
	""tz_long"":""America/Vancouver""
},
		""period"":6,
		""high"": {
		""fahrenheit"":""42"",
		""celsius"":""6""
		},
		""low"": {
		""fahrenheit"":""35"",
		""celsius"":""2""
		},
		""conditions"":""Partly Cloudy"",
		""icon"":""partlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/partlycloudy.gif"",
		""skyicon"":"""",
		""pop"":10,
		""qpf_allday"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_day"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_night"": {
		""in"": 0.00,
		""mm"": 0
		},
		""snow_allday"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_day"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_night"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""maxwind"": {
		""mph"": 10,
		""kph"": 16,
		""dir"": ""N"",
		""degrees"": 10
		},
		""avewind"": {
		""mph"": 6,
		""kph"": 10,
		""dir"": ""N"",
		""degrees"": 10
		},
		""avehumidity"": 83,
		""maxhumidity"": 0,
		""minhumidity"": 0
		}
		,
		{""date"":{
	""epoch"":""1451444400"",
	""pretty"":""7:00 PM PST on December 29, 2015"",
	""day"":29,
	""month"":12,
	""year"":2015,
	""yday"":362,
	""hour"":19,
	""min"":""00"",
	""sec"":0,
	""isdst"":""0"",
	""monthname"":""December"",
	""monthname_short"":""Dec"",
	""weekday_short"":""Tue"",
	""weekday"":""Tuesday"",
	""ampm"":""PM"",
	""tz_short"":""PST"",
	""tz_long"":""America/Vancouver""
},
		""period"":7,
		""high"": {
		""fahrenheit"":""43"",
		""celsius"":""6""
		},
		""low"": {
		""fahrenheit"":""36"",
		""celsius"":""2""
		},
		""conditions"":""Partly Cloudy"",
		""icon"":""partlycloudy"",
		""icon_url"":""http://icons.wxug.com/i/c/k/partlycloudy.gif"",
		""skyicon"":"""",
		""pop"":20,
		""qpf_allday"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_day"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_night"": {
		""in"": 0.00,
		""mm"": 0
		},
		""snow_allday"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_day"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_night"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""maxwind"": {
		""mph"": 10,
		""kph"": 16,
		""dir"": ""NE"",
		""degrees"": 37
		},
		""avewind"": {
		""mph"": 7,
		""kph"": 11,
		""dir"": ""NE"",
		""degrees"": 37
		},
		""avehumidity"": 79,
		""maxhumidity"": 0,
		""minhumidity"": 0
		}
		,
		{""date"":{
	""epoch"":""1451530800"",
	""pretty"":""7:00 PM PST on December 30, 2015"",
	""day"":30,
	""month"":12,
	""year"":2015,
	""yday"":363,
	""hour"":19,
	""min"":""00"",
	""sec"":0,
	""isdst"":""0"",
	""monthname"":""December"",
	""monthname_short"":""Dec"",
	""weekday_short"":""Wed"",
	""weekday"":""Wednesday"",
	""ampm"":""PM"",
	""tz_short"":""PST"",
	""tz_long"":""America/Vancouver""
},
		""period"":8,
		""high"": {
		""fahrenheit"":""43"",
		""celsius"":""6""
		},
		""low"": {
		""fahrenheit"":""39"",
		""celsius"":""4""
		},
		""conditions"":""Clear"",
		""icon"":""clear"",
		""icon_url"":""http://icons.wxug.com/i/c/k/clear.gif"",
		""skyicon"":"""",
		""pop"":20,
		""qpf_allday"": {
		""in"": 0.04,
		""mm"": 1
		},
		""qpf_day"": {
		""in"": 0.00,
		""mm"": 0
		},
		""qpf_night"": {
		""in"": 0.04,
		""mm"": 1
		},
		""snow_allday"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_day"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_night"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""maxwind"": {
		""mph"": 10,
		""kph"": 16,
		""dir"": ""E"",
		""degrees"": 82
		},
		""avewind"": {
		""mph"": 8,
		""kph"": 13,
		""dir"": ""E"",
		""degrees"": 82
		},
		""avehumidity"": 84,
		""maxhumidity"": 0,
		""minhumidity"": 0
		}
		,
		{""date"":{
	""epoch"":""1451617200"",
	""pretty"":""7:00 PM PST on December 31, 2015"",
	""day"":31,
	""month"":12,
	""year"":2015,
	""yday"":364,
	""hour"":19,
	""min"":""00"",
	""sec"":0,
	""isdst"":""0"",
	""monthname"":""December"",
	""monthname_short"":""Dec"",
	""weekday_short"":""Thu"",
	""weekday"":""Thursday"",
	""ampm"":""PM"",
	""tz_short"":""PST"",
	""tz_long"":""America/Vancouver""
},
		""period"":9,
		""high"": {
		""fahrenheit"":""44"",
		""celsius"":""7""
		},
		""low"": {
		""fahrenheit"":""38"",
		""celsius"":""3""
		},
		""conditions"":""Chance of Rain"",
		""icon"":""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""skyicon"":"""",
		""pop"":30,
		""qpf_allday"": {
		""in"": 0.07,
		""mm"": 2
		},
		""qpf_day"": {
		""in"": 0.03,
		""mm"": 1
		},
		""qpf_night"": {
		""in"": 0.04,
		""mm"": 1
		},
		""snow_allday"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_day"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_night"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""maxwind"": {
		""mph"": 10,
		""kph"": 16,
		""dir"": ""NE"",
		""degrees"": 54
		},
		""avewind"": {
		""mph"": 9,
		""kph"": 14,
		""dir"": ""NE"",
		""degrees"": 54
		},
		""avehumidity"": 90,
		""maxhumidity"": 0,
		""minhumidity"": 0
		}
		,
		{""date"":{
	""epoch"":""1451703600"",
	""pretty"":""7:00 PM PST on January 01, 2016"",
	""day"":1,
	""month"":1,
	""year"":2016,
	""yday"":0,
	""hour"":19,
	""min"":""00"",
	""sec"":0,
	""isdst"":""0"",
	""monthname"":""January"",
	""monthname_short"":""Jan"",
	""weekday_short"":""Fri"",
	""weekday"":""Friday"",
	""ampm"":""PM"",
	""tz_short"":""PST"",
	""tz_long"":""America/Vancouver""
},
		""period"":10,
		""high"": {
		""fahrenheit"":""44"",
		""celsius"":""7""
		},
		""low"": {
		""fahrenheit"":""41"",
		""celsius"":""5""
		},
		""conditions"":""Chance of Rain"",
		""icon"":""chancerain"",
		""icon_url"":""http://icons.wxug.com/i/c/k/chancerain.gif"",
		""skyicon"":"""",
		""pop"":50,
		""qpf_allday"": {
		""in"": 0.13,
		""mm"": 3
		},
		""qpf_day"": {
		""in"": 0.07,
		""mm"": 2
		},
		""qpf_night"": {
		""in"": 0.06,
		""mm"": 2
		},
		""snow_allday"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_day"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""snow_night"": {
		""in"": 0.0,
		""cm"": 0.0
		},
		""maxwind"": {
		""mph"": 10,
		""kph"": 16,
		""dir"": ""E"",
		""degrees"": 80
		},
		""avewind"": {
		""mph"": 8,
		""kph"": 13,
		""dir"": ""E"",
		""degrees"": 80
		},
		""avehumidity"": 97,
		""maxhumidity"": 0,
		""minhumidity"": 0
		}
		]
		}
	}
}
";
    }
}
