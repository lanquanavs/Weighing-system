using AWSV2.ViewModels.ShellViewHomeTemplate;
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
    public class HomeTemplateTypeMenuConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values?.Length).GetValueOrDefault() == 0)
            {
                return string.Empty;
            }

            var convertSuccess = Enum.TryParse<HomeTemplateType>(values.FirstOrDefault()?.ToString(), out var tagEnum);
            var type = values.LastOrDefault() as HomeTemplateType?;

            if (!convertSuccess || type == null)
            {
                return string.Empty;
            }

            return type.GetValueOrDefault() == tagEnum ? $"（当前）{tagEnum.GetDescription()}" : tagEnum.GetDescription();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
