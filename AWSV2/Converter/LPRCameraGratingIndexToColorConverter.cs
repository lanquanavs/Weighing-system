using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AWSV2.Converter
{
    internal class LPRCameraGratingIndexToColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush _gray = Application.Current.FindResource("AJGray") as SolidColorBrush;
        private static readonly SolidColorBrush _olive = Application.Current.FindResource("AJGreen") as SolidColorBrush;
        private static readonly SolidColorBrush _red = Application.Current.FindResource("AJRed") as SolidColorBrush;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return _gray;
            }

            return (int)value != -1 ? _olive : _red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
