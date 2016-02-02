using System;
using Windows.UI.Xaml.Data;

namespace AmadeusW.Mirror.GUI.Converters
{
    public class DateTimeToShortStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dateTime;
            try
            {
                dateTime = (DateTime)value;
            }
            catch
            {
                return String.Empty;
            }
            return dateTime.ToString("h:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Not going to happen
            return default(DateTime);
        }
    }
}
