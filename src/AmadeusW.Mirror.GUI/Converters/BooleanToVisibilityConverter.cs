using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AmadeusW.Mirror.GUI.Converters
{
    class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
                return Visibility.Visible;

            var boolValue = (bool)value;

            if (parameter.ToString() == "reverse")
                boolValue = !boolValue;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!(value is Visibility))
                return false;

            return (Visibility)value == Visibility.Visible;
        }
    }
}
