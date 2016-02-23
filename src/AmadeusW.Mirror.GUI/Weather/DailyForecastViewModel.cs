namespace AmadeusW.Mirror.GUI.Weather
{
    public class DailyForecastViewModel : PropertyChangedBase
    {
        public string Date { get; set; }
        public string DayOfWeek { get; set; }

        public int TemperatureHigh { get; set; }
        public int TemperatureLow { get; set; }
        public string Conditions { get; set; }
    }
}
