using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace AmadeusW.Mirror.GUI
{
    public class ThemeAwareFrame : Frame
    {
        private static readonly ThemeProxyClass _themeProxyClass = new ThemeProxyClass();

        public static readonly DependencyProperty AppThemeProperty = DependencyProperty.Register(
            "AppTheme", typeof(ElementTheme), typeof(ThemeAwareFrame), new PropertyMetadata(default(ElementTheme), (d, e) => _themeProxyClass.Theme = (ElementTheme)e.NewValue));


        public ElementTheme AppTheme
        {
            get { return (ElementTheme)GetValue(AppThemeProperty); }
            set { SetValue(AppThemeProperty, value); }
        }

        public ThemeAwareFrame(ElementTheme appTheme)
        {
            var themeBinding = new Binding { Source = _themeProxyClass, Path = new PropertyPath("Theme"), Mode = BindingMode.OneWay };
            SetBinding(RequestedThemeProperty, themeBinding);
            AppTheme = appTheme;

        }
        sealed class ThemeProxyClass : INotifyPropertyChanged
        {
            private ElementTheme _theme;

            public ElementTheme Theme
            {
                get { return _theme; }
                set
                {
                    _theme = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
