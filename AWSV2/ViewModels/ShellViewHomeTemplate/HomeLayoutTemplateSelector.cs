using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace AWSV2.ViewModels.ShellViewHomeTemplate
{
    public class HomeLayoutTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Default { get; set; }
        public DataTemplate Monitor { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ShellViewModel model && model.TemplateType != null)
            {
                return model.TemplateType.GetValueOrDefault() == HomeTemplateType.Default ? Default : Monitor;
            }
            return Monitor;
        }
    }

    public enum HomeTemplateType
    {
        [Description("页面一")]
        Default,
        [Description("页面二")]
        Monitor
    }
}
