using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AWSV2.Services
{
    public static class AutoCompleteBoxHelper
    {
        private static void OnIsOtherPathChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var acb = obj as AutoCompleteBox;
            if (!string.IsNullOrEmpty(e.NewValue.ToString()))
            {
                acb.ItemFilter = (s, item) => {
                    var objType = item.GetType();
                    string path = GetOtherPaths(acb);
                    var pathes = path.Split(',');
                    bool result = false;
                    foreach (string p in pathes)
                    {
                        var propertyInfo = objType.GetProperty(p);
                        string value = propertyInfo.GetValue(item, null).ToString();
                        result |= value.Contains(s);
                    }
                    return result;
                };
            }
            else
                acb.ItemFilter = null;
        }

        public static string GetOtherPaths(DependencyObject obj)
        {
            return (string)obj.GetValue(OtherPathsProperty);
        }

        public static void SetOtherPaths(DependencyObject obj, string value)
        {
            obj.SetValue(OtherPathsProperty, value);
        }

        public static readonly DependencyProperty OtherPathsProperty =
             DependencyProperty.RegisterAttached("OtherPaths", typeof(string),
             typeof(AutoCompleteBoxHelper),
             new PropertyMetadata("", OnIsOtherPathChanged));
    }
}
