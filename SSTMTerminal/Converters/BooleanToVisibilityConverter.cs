using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SSTMTerminal.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        // Decide True Changing To Visibility or Collapsed
        public bool IsOpposite { get; set; }

        // Decide False Changing To Collapsed or Hidden
        public bool IsHidden { get; set; }

        public static readonly BooleanToVisibilityConverter Instance = new BooleanToVisibilityConverter();

        public BooleanToVisibilityConverter()
        {
            IsOpposite = false;
            IsHidden = false;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return DependencyProperty.UnsetValue;

            bool valueBool = (bool)value;

            if ((valueBool && IsOpposite && !IsHidden) || (!valueBool && !IsOpposite && !IsHidden))
            {
                return Visibility.Collapsed;
            }

            if ((valueBool && IsOpposite && IsHidden) || (!valueBool && !IsOpposite && IsHidden))
            {
                return Visibility.Hidden;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
