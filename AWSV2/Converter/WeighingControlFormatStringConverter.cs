using Common.LPR;
using Masuit.Tools.Reflection;
using Masuit.Tools.Systems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AWSV2.Converter
{
    public class WeighingControlFormatStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string val && !string.IsNullOrWhiteSpace(val))
            {
                return ((WeighingControl)Enum.Parse(typeof(WeighingControl), val)).GetDescription();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = WeighingControl.Hand.ToString();
            if (value is string val && !string.IsNullOrWhiteSpace(val))
            {
                switch (val)
                {
                    case "自动称重":
                        result = WeighingControl.Auto.ToString();
                        break;
                    case "手动称重":
                        result = WeighingControl.Hand.ToString();
                        break;
                    case "按钮称重":
                        result = WeighingControl.Btn.ToString();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
    }
}
