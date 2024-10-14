using AWSV2.Models;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AWSV2.Converter
{
    public class DeviceNoticeIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var online = values.FirstOrDefault();

            var key = values.LastOrDefault();

            if (online is bool onlineValue && key is DeviceNoticeIcon.DeviceKey keyV)
            {
                return onlineValue ? $"{keyV}运行中" : $"{keyV}已离线";
            }
            return string.Empty;
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
