namespace AmadeusW.Mirror.GUI.Weather
{
    public class HourlyForecastViewModel
    {
        public int Time { get; set; }
        public string AmPm { get; set; }
        public int Temperature { get; set; }
        public string Conditions { get; set; }
        public int Rainfall { get; set; }
        public int Snowfall { get; set; }
    }
}
