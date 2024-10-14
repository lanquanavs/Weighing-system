using Aspose.Cells;
using AWSV2.Models;
using AWSV2.Services;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Model;
using System.Reflection;
using System.Runtime.InteropServices;
using Common.Utility.AJ.Extension;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Common.Utility.AJ;

namespace AWSV2.ViewModels
{
    public class WeighFormListViewModel : Screen
    {
        private AppSettingsSection _mainSettings;

        private string _search;
        public string Search
        {
            get => _search;
            set
            {
                if (SetAndNotify(ref _search, value))
                {
                    DyanmicPoundFieldItem = new ObservableCollection<DynamicPoundFieldItem>(_source.Where(p => p.Label.Contains(_search) || p.Field.ToLower().Contains(_search)));
                }
            }
        }

        private List<DynamicPoundFieldItem> _source;
        private ObservableCollection<DynamicPoundFieldItem> _dyanmicPoundFieldItems;
        public ObservableCollection<DynamicPoundFieldItem> DyanmicPoundFieldItem
        {
            get
            {
                return _dyanmicPoundFieldItems;
            }
            set
            {
                SetAndNotify(ref _dyanmicPoundFieldItems, value);
            }
        }

        public WeighFormListViewModel(ref AppSettingsSection mainSettings)
        {
            _mainSettings = mainSettings;

            _source = Common.Models.WeighingRecord.GetDyanmicPoundFieldItems(_mainSettings, false, false);

            DyanmicPoundFieldItem = new ObservableCollection<DynamicPoundFieldItem>(_source);
        }

        public bool CanSave
        {
            get
            {
                var hasItems = _source.Any();
                var hasLabels = _source.Count(p => !string.IsNullOrWhiteSpace(p.Label)) == _source.Count;

                return hasItems && hasLabels;
            }
        }
        public void Save()
        {
            var items = _source.Select(p => new
            {
                p.Field,
                p.Label,
                p.SortNo,
                p.IsColumnDisplay,
                p.IsFormDisplay
            });

            var configFieldKeys = _mainSettings.Settings.AllKeys.Where(p => p.StartsWith("_")).ToArray();

            foreach (var item in items)
            {
                if (configFieldKeys.Any(p => p.Equals(item.Field, StringComparison.OrdinalIgnoreCase)))
                {
                    _mainSettings.Settings[item.Field].Value = item.Label;
                }
                else
                {
                    _mainSettings.Settings.Add(item.Field, item.Label);
                }
            }
            _mainSettings.Settings["WeightRecordDataGridOrderedColumns"].Value = AJUtil.AJSerializeObject(items);

            RequestClose();
        }

    }
}
