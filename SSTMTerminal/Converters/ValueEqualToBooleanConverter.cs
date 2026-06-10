using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SSTMTerminal.Converters
{
    public class ValueEqualToBooleanConverter : IMultiValueConverter
    {
        private static ValueEqualToBooleanConverter _instance = new ValueEqualToBooleanConverter();

        /// <summary>
        /// Initializes a new instance of <see cref="ValueEqualToBooleanConverter"/>.
        /// </summary>
        private ValueEqualToBooleanConverter()
        {
            // Empty.
        }

        /// <summary>
        /// Gets an instance of <see cref="ValueEqualToBooleanConverter"/>.
        /// </summary>
        public static ValueEqualToBooleanConverter Instance
        {
            get
            {
                return _instance;
            }
        }

        private const string PARAMETER_RESERVE = "reverse";

        #region IMultiValueConverter Members

        /// <summary>
        /// Compare with two value.
        /// </summary>
        /// <param name="values">
        /// values[0], value[1]: Two value need to be compared.
        /// </param>
        /// <param name="parameter">
        /// observe: Return true then two value is equal.
        /// reserve: Return false then two value is equal.
        /// </param>
        /// <returns>
        /// Return true then two value is equal if parameter is "observe".
        /// Return false then two value is equal if parameter is "reserve".
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isReverse = false;

            if (null != parameter)
            {
                isReverse = string.Equals(parameter.ToString(), PARAMETER_RESERVE, StringComparison.OrdinalIgnoreCase);
            }

            bool result;

            double doubleVarLeft = double.Parse(values[0].ToString());
            double doubleVarRight = double.Parse(values[1].ToString());

            bool isEqual = (doubleVarLeft == doubleVarRight);

            if (isReverse)
            {
                result = !isEqual;
            }
            else
            {
                result = isEqual;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
