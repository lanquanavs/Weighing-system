using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AWSV2.Converter
{
    class BoolToVisibilityConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "Collapsed";
            else
                return "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToStringConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "是";
            else
                return "否";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return false;
            }
            else
            {
                switch (value.ToString().ToLower())
                {
                    case "true":
                        return true;
                    case "false":
                        return false;
                    default:
                        break;
                }
            }
            return false;
        }
    }

    public class PayToStringConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "未支付";
            }
            switch ((int)value)
            {
                case 0:
                    return "未支付";
                case 1:
                    return "电子支付";
                case 2:
                    return "线下支付";
                case 3:
                    return "储值支付";
                case 4:
                    return "免费放行";
                default:
                    return "未支付";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return 0;
            }
            else
            {
                switch (value.ToString().ToLower())
                {
                    case "未支付":
                        return 0;
                    case "电子支付":
                        return 1;
                    case "线下支付":
                        return 2;
                    case "储值支付":
                        return 3;
                    case "免费放行":
                        return 4;
                    default:
                        return 0;

                }
            }
        }
    }
}
