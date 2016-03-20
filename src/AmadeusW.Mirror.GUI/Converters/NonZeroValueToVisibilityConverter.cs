using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AmadeusW.Mirror.GUI.Converters
{
    class NonZeroValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var valueAsString = value.ToString();
            if (String.IsNullOrWhiteSpace(valueAsString))
            {
                return Visibility.Collapsed;
            }
            if (valueAsString.Trim() == "0")
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Not going to happen
            return String.Empty;
        }
    }
}
