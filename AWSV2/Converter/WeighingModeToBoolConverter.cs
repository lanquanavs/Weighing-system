using System;
using System.Globalization;
using System.Windows.Data;

namespace AWSV2.Converter
{
    class WeighingModeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter.ToString() == value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (!isChecked)
            {
                return null;
            }
            return parameter.ToString();
        }
    }
}
