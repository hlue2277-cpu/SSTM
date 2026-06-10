using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SSTMTerminal.Converters
{

    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BooleanToColorConverter : IValueConverter
    {
        public SolidColorBrush Default { get; set; } = new SolidColorBrush(Colors.White);
        public SolidColorBrush Opposite { get; set; } = new SolidColorBrush(Colors.Black);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return DependencyProperty.UnsetValue;

            bool valueBool = (bool)value;

            if (valueBool)
            {
                return Default;
            }
            else
            {
                return Opposite;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
