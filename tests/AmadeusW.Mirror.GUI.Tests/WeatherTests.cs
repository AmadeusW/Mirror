using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AmadeusW.Mirror.GUI.Clock;
using AmadeusW.Mirror.GUI.Weather;
using System.Collections.Generic;
using System.Linq;

namespace AmadeusW.Mirror.GUI.Tests
{
    [TestClass]
    public class WeatherTests
    {
        [TestMethod]
        public void WeatherCorrectlyIdentifiesIncomingRain()
        {
            // Arrange
            var wm = new MockWeatherModel();
            mockSunnyThenRainyWeather(wm);
            wm.Update();

            var dayVm = new WeatherTodayViewModel();
            dayVm.Initialize(wm);

            Assert.AreEqual("Sunny", dayVm.CurrentWeather.Conditions);
            Assert.AreEqual(0, dayVm.CurrentWeather.Rainfall);
            Assert.AreEqual(0, dayVm.CurrentWeather.Snowfall);
            Assert.AreEqual(26, dayVm.CurrentWeather.Temperature);

            Assert.AreEqual("Rainy", dayVm.Forecast.First().Conditions);
            Assert.AreEqual(20, dayVm.Forecast.First().Rainfall);
            Assert.AreEqual(5, dayVm.Forecast.First().Snowfall);
            Assert.AreEqual(16, dayVm.Forecast.First().Temperature);

            Assert.AreEqual("Rain starts at 1pm", dayVm.RainForecast);
        }

        [TestMethod]
        public void WeatherCorrectlyIdentifiesOutcomingRain()
        {
            // Arrange
            var wm = new MockWeatherModel();
            mockRainyThenSunnyWeather(wm);
            wm.Update();

            var dayVm = new WeatherTodayViewModel();
            dayVm.Initialize(wm);

            Assert.AreEqual("Rainy", dayVm.CurrentWeather.Conditions);
            Assert.AreEqual(20, dayVm.CurrentWeather.Rainfall);
            Assert.AreEqual(5, dayVm.CurrentWeather.Snowfall);
            Assert.AreEqual(15, dayVm.CurrentWeather.Temperature);

            Assert.AreEqual("Sunny", dayVm.Forecast.First().Conditions);
            Assert.AreEqual(0, dayVm.Forecast.First().Rainfall);
            Assert.AreEqual(0, dayVm.Forecast.First().Snowfall);
            Assert.AreEqual(26, dayVm.Forecast.First().Temperature);

            Assert.AreEqual("Rain ends at 1pm", dayVm.RainForecast);
        }

        [TestMethod]
        public void WeatherViewModelsUpdate()
        {
            // Arrange
            var wm = new MockWeatherModel();
            var weekVm = new WeatherThisWeekViewModel();
            var dayVm = new WeatherTodayViewModel();

            mockSunnyWeather(wm);
            wm.Update();
            weekVm.Initialize(wm);
            dayVm.Initialize(wm);

            // Act
            mockRainyWeather(wm);
            wm.Update();

            // Assert that viewmodels updated correctly to the "rainyWeather" values
            Assert.AreEqual("Rainy", weekVm.CurrentWeather.Conditions);
            Assert.AreEqual(20, weekVm.CurrentWeather.Rainfall);
            Assert.AreEqual(5, weekVm.CurrentWeather.Snowfall);
            Assert.AreEqual(15, weekVm.CurrentWeather.Temperature);

            Assert.AreEqual("Rainy", weekVm.Forecast.First().Conditions);
            Assert.AreEqual("Friday 27", weekVm.Forecast.First().Date);
            Assert.AreEqual(16, weekVm.Forecast.First().TemperatureHigh);
            Assert.AreEqual(14, weekVm.Forecast.First().TemperatureLow);

            Assert.AreEqual("Rainy", dayVm.CurrentWeather.Conditions);
            Assert.AreEqual(20, dayVm.CurrentWeather.Rainfall);
            Assert.AreEqual(5, dayVm.CurrentWeather.Snowfall);
            Assert.AreEqual(15, dayVm.CurrentWeather.Temperature);

            Assert.AreEqual("7:31", dayVm.Sunrise);
            Assert.AreEqual("5:29", dayVm.Sunset);

            Assert.AreEqual("Rainy", dayVm.Forecast.First().Conditions);
            Assert.AreEqual(20, dayVm.Forecast.First().Rainfall);
            Assert.AreEqual(5, dayVm.Forecast.First().Snowfall);
            Assert.AreEqual(16, dayVm.Forecast.First().Temperature);
            Assert.AreEqual(1, dayVm.Forecast.First().Time); // 1pm
        }


        private void mockSunnyWeather(MockWeatherModel wm)
        {
            wm.MockSunrise = DateTime.Parse("2015-11-26 07:30:00");
            wm.MockSunset = DateTime.Parse("2015-11-26 17:30:00");
            wm.MockDailyForecast = new List<WeatherDetailsModel>()
                {
                    new WeatherDetailsModel()
                    {
                        Conditions = "Sunny",
                        Rainfall = 0,
                        Snowfall = 0,
                        TemperatureHigh = 26,
                        TemperatureLow = 24,
                        Time = DateTime.Parse("2015-11-26 8:00:00"),
                    }
                };
            wm.MockHourlyForecast = new List<WeatherDetailsModel>()
                {
                    new WeatherDetailsModel()
                    {
                        Conditions = "Sunny",
                        Rainfall = 0,
                        Snowfall = 0,
                        Temperature = 25,
                        Time = DateTime.Parse("2015-11-26 8:00:00"),
                    },
                    new WeatherDetailsModel()
                    {
                        Conditions = "Sunny",
                        Rainfall = 0,
                        Snowfall = 0,
                        Temperature = 26,
                        Time = DateTime.Parse("2015-11-26 13:00:00"),
                    }
                };
        }

        private void mockRainyWeather(MockWeatherModel wm)
        {
            wm.MockSunrise = DateTime.Parse("2015-11-27 07:31:00");
            wm.MockSunset = DateTime.Parse("2015-11-27 17:29:00");
            wm.MockDailyForecast = new List<WeatherDetailsModel>()
                {
                    new WeatherDetailsModel()
                    {
                        Conditions = "Rainy",
                        Rainfall = 20,
                        Snowfall = 5,
                        TemperatureHigh = 16,
                        TemperatureLow = 14,
                        Time = DateTime.Parse("2015-11-27 8:00:00"),
                    }
                };
            wm.MockHourlyForecast = new List<WeatherDetailsModel>()
                {
                    new WeatherDetailsModel()
                    {
                        Conditions = "Rainy",
                        Rainfall = 20,
                        Snowfall = 5,
                        Temperature = 15,
                        Time = DateTime.Parse("2015-11-27 8:00:00"),
                    },
                    new WeatherDetailsModel()
                    {
                        Conditions = "Rainy",
                        Rainfall = 20,
                        Snowfall = 5,
                        Temperature = 16,
                        Time = DateTime.Parse("2015-11-27 13:00:00"),
                    }
                };
        }

        private void mockSunnyThenRainyWeather(MockWeatherModel wm)
        {
            wm.MockHourlyForecast = new List<WeatherDetailsModel>()
                {
                    new WeatherDetailsModel()
                    {
                        Conditions = "Sunny",
                        Rainfall = 0,
                        Snowfall = 0,
                        Temperature = 26,
                        Time = DateTime.Parse("2015-11-27 8:00:00"),
                    },
                    new WeatherDetailsModel()
                    {
                        Conditions = "Rainy",
                        Rainfall = 20,
                        Snowfall = 5,
                        Temperature = 16,
                        Time = DateTime.Parse("2015-11-27 13:00:00"),
                    }
                };
        }

        private void mockRainyThenSunnyWeather(MockWeatherModel wm)
        {
            wm.MockHourlyForecast = new List<WeatherDetailsModel>()
                {
                    new WeatherDetailsModel()
                    {
                        Conditions = "Rainy",
                        Rainfall = 20,
                        Snowfall = 5,
                        Temperature = 15,
                        Time = DateTime.Parse("2015-11-27 8:00:00"),
                    },
                    new WeatherDetailsModel()
                    {
                        Conditions = "Sunny",
                        Rainfall = 0,
                        Snowfall = 0,
                        Temperature = 26,
                        Time = DateTime.Parse("2015-11-27 13:00:00"),
                    }
                };
        }

    }
}
