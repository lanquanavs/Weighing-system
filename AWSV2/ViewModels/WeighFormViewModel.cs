using Aspose.Cells;
using AWSV2.Models;
using System;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;
using AWSV2.Services;
using System.Configuration;
using System.Reflection;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Common.Utility.AJ.Extension;
using MaterialDesignExtensions.Controls;
using Common.Utility.AJ.MobileConfiguration;
using Common.Converters;
using Common.Utility.AJ;
using Stylet;
using Common.Utility;
using AWSV2.ViewModels.ShellViewHomeTemplate;
using Masuit.Tools;
using Common.Model;
using LiveChartsCore.VisualElements;
using Common.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Common.EF.Controllers;
using Masuit.Tools.Models;

namespace AWSV2.ViewModels
{
    public class WeighFormViewModel : PropertyChangedBase
    {
        private List<DynamicPoundFieldItem> _fields;

        public event EventHandler ManualInputCarNoCompleted;

        private static readonly SolidColorBrush _borderBrush = Application.Current.FindResource("MaterialDesignBody") as SolidColorBrush;

        private static readonly SolidColorBrush _moduleBrush = Application.Current.FindResource("PrimaryHueDarkBrush") as SolidColorBrush;

        //页面上绑定的属性
        public Grid WeighFormGrid { get; set; } = new Grid()
        {
            Focusable = false,
            FocusVisualStyle = null
        };

        public StackPanel WeighFormStackPanel { get; set; } = new StackPanel()
        {
            Focusable = false,
            FocusVisualStyle = null
        };


        public class PoundFormControlData
        {
            public DynamicPoundFieldItem Left { get; set; }
            public DynamicPoundFieldItem Right { get; set; }
        }
        private List<PoundFormControlData> _formItems;

        private static readonly string[] _textBoxNameArray = new string[] { "Dj", "Je", "Driver", "DriverPhone" };


        private static readonly string[] _notAutoModeTextboxReadonlyFileds = new string[] {
            nameof(WeighingRecord.Bz),
            nameof(WeighingRecord.Je),
            "Dj",
            nameof(WeighingRecord.Driver),
            nameof(WeighingRecord.DriverPhone),
        };

        private static readonly string[] _valueConverterParam = new string[] {
            nameof(WeighingRecord.AxleNum),
            nameof(WeighingRecord.IsLimit),
            nameof(WeighingRecord.IsCover),
            nameof(WeighingRecord.IsPay),
            nameof(WeighingRecord.Gblx),
            nameof(WeighingRecord.Kh2),
            nameof(WeighingRecord.Kh3),
            nameof(WeighingRecord.Fhdw),
            "By"
        };

        private static readonly string[] _autocompleteNameArray = new string[]
        {
            nameof(WeighingRecord.GoodsSpec),
            nameof(WeighingRecord.AxleNum),
            nameof(WeighingRecord.Gblx),
            nameof(WeighingRecord.Ch),
            nameof(WeighingRecord.Wz),
            nameof(WeighingRecord.Kh),
            "By",
            nameof(WeighingRecord.IsLimit),
            nameof(WeighingRecord.IsCover),
            nameof(WeighingRecord.IsPay),
            nameof(WeighingRecord.Fhdw),
        };

        private static readonly string[] _weightFields = new string[]
        {
            nameof(WeighingRecord.Mz),
            nameof(WeighingRecord.Pz),
            nameof(WeighingRecord.Jz),
            nameof(WeighingRecord.Sz),
        };

        private static readonly IEnumerable<PropertyInfo> _props = typeof(WeighingRecord).GetRuntimeProperties();

        private static readonly string[] _booleanStrArray = new string[] { "是", "对", "Y", "Yes", "OK", "True", "已完成" };

        private static readonly List<DropdownOption> _isPaySource = new List<DropdownOption>
                                                    {
                                                        new DropdownOption
                                                        {
                                                            label = "未支付",
                                                            value = 0
                                                        },
                                                        new DropdownOption
                                                        {
                                                            label = "电子支付",
                                                            value = 1
                                                        },
                                                        new DropdownOption
                                                        {
                                                            label = "线下支付",
                                                            value = 2
                                                        },
                                                        new DropdownOption
                                                        {
                                                            label = "储值支付",
                                                            value = 3
                                                        },
                                                        new DropdownOption
                                                        {
                                                            label = "免费放行",
                                                            value = 4
                                                        }
                                                    };

        private static readonly List<DropdownOption> _axleNumSource = new List<DropdownOption> {
                                                new DropdownOption {
                                                    label = "2",
                                                    value = "2",
                                                },
                                                new DropdownOption {
                                                    label = "3",
                                                    value = "3",
                                                },
                                                new DropdownOption
                                                {
                                                    label = "4",
                                                    value = "4",
                                                },
                                                new DropdownOption
                                                {
                                                    label = "5",
                                                    value = "5",
                                                },
                                                new DropdownOption
                                                {
                                                    label = "6",
                                                    value = "6",
                                                }
                                            };

        private static readonly List<DropdownOption> _booleanDropdownOptions = new List<DropdownOption>
                                        {
                                            new DropdownOption
                                            {
                                                label = "是",
                                                value = true
                                            },
                                            new DropdownOption
                                            {
                                                label = "否",
                                                value = false
                                            }
                                        };

        private List<string> _goodsSource = new List<string>();
        private List<string> _carSource = new List<string>();
        private List<string> _customerSource = new List<string>();
        private List<string> _gblxSource = new List<string>();

        private readonly DataTemplate _simpleAutocompleteDataTemplate = new DataTemplate();

        private string _currentCustomer;
        public string CurrentCustomer
        {
            get { return _currentCustomer; }
            set
            {
                if (SetAndNotify(ref _currentCustomer, value))
                {
                    CurrentCustomerChangedAsync();
                }
            }
        }

        private string _goodsItem;
        public string GoodsItem
        {
            get { return _goodsItem; }
            set
            {
                if (SetAndNotify(ref _goodsItem, value))
                {
                    GoodsItemChangedAsync();
                }
            }
        }


        private string _goodsPrice;
        public string GoodsPrice
        {
            get { return _goodsPrice; }
            set
            {
                _goodsPrice = value;
                Source.GoodsPrice = value.TryGetDecimal();
            }
        }

        WeighingRecord _source;
        public WeighingRecord Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }
        public List<Common.Models.SysField> SysFields = new List<Common.Models.SysField>();

        private string _weighFormDisplayMode;

        private readonly Dictionary<string, Func<bool, Autocomplete, Task<object>>> AutocompleteCtrlSourceProcessMaps;

        private HomeTemplateType _homeTemplateType;

        private AppSettingsSection _mainSetting;

        public WeighFormViewModel(AppSettingsSection mainSetting)
        {
            _mainSetting = mainSetting;
            _formItems = new List<PoundFormControlData>();
            Source = new WeighingRecord();

            var fef = new FrameworkElementFactory(typeof(Label));
            fef.SetValue(Label.ContentProperty, new Binding("label"));
            _simpleAutocompleteDataTemplate.VisualTree = fef;

            AutocompleteCtrlSourceProcessMaps = new Dictionary<string, Func<bool, Autocomplete, Task<object>>>
            {
                {"AxleNum", (reset,ctrl) => {
                    object axleNum = Source?.AxleNum ?? 0;
                    return Task.FromResult(axleNum);
                } },
                {"IsLimit", (reset,ctrl) => {
                    var val = Source?.IsLimit;
                    object ret = val.HasValue ? _booleanDropdownOptions.Find(c => (bool)c.value == Source.IsLimit) : null;
                    return Task.FromResult(ret);
                } },
                {"IsCover", (reset,ctrl) => {
                    var val = Source?.IsCover;
                    object ret = val.HasValue ? _booleanDropdownOptions.Find(c => (bool)c.value == val.GetValueOrDefault()) : null;
                    return Task.FromResult(ret);
                }},
                {"IsPay", (reset,ctrl) => {
                    var val = Source?.IsPay;
                    object ret = val.HasValue ? _isPaySource.FirstOrDefault(P => (int)P.value == Source.IsPay) : null;
                    return Task.FromResult(ret);
                } },
                {"Gblx", ProcessGblx },
                {"Wz", ProcessWz },
                {"GoodsSpec", ProcessGoodsSpec },
                {"Ch",ProcessCh },
                {"Kh", ProcessKh },
                {"Kh2", ProcessKh2OrKh3 },
                {"Kh3", ProcessKh2OrKh3 },
                {"Fhdw",ProcessFhdw },
                //{"Je",ProcessJe },
                {"By",ProcessBy },
        };

            WeighFormGrid.Width = double.NaN;
            _weighFormDisplayMode = _mainSetting.Settings["WeighFormDisplayMode"].TryGetString("Priview");
        }

        public void SetFormField(string propName, string value)
        {
            if (propName == nameof(Source.Wz))
            {
                GoodsItem = value;
                return;
            }
            if (propName == nameof(Source.Kh))
            {
                CurrentCustomer = value;
                return;
            }
            if (propName == nameof(Source.GoodsPrice))
            {
                GoodsPrice = value;
                return;
            }

            var props = Source.GetType().GetRuntimeProperties();
            var prop = props.FirstOrDefault(p => p.Name == propName);
            if (prop == null)
            {
                return;
            }

            if (prop.PropertyType == typeof(decimal))
            {
                prop.SetValue(Source, value.TryGetDecimal());
                return;
            }

            if (prop.PropertyType == typeof(bool))
            {
                prop.SetValue(Source, _booleanStrArray.Contains(value));
                return;
            }

            if (prop.PropertyType == typeof(DateTime)
                || prop.PropertyType == typeof(DateTime?))
            {
                prop.SetValue(Source, value.TryGetDateTime().GetValueOrDefault());
                return;
            }

            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(Source, value);
            }

        }

        private async Task<object> ProcessBy(bool reset, Autocomplete ctrl)
        {
            var val = string.Empty;
            if (Source != null)
            {
                var prop = Source.GetType().GetRuntimeProperties().FirstOrDefault(p => p.Name == ctrl.Name);
                val = prop?.GetValue(Source)?.ToString();
            }
            var binding = BindingOperations.GetBinding(ctrl, Autocomplete.SelectedItemProperty);
            var converter = binding.Converter as AutocompleteDropdownOptionConverter;
            if (reset)
            {
                if (SysFields == null || SysFields.Count == 0)
                {
                    using var db = AJDatabaseService.GetDbContext();
                    var paged = await db.SysFields.AsNoTracking().ToListAsync();
                    SysFields = AJAutoMapperService.Instance()
                        .Mapper.Map<List<Common.EF.Tables.SysField>, List<SysField>>(paged);
                }
                converter.Source = SysFields.Where(p => p.FieldType == ctrl.Name)
                                           .Select(s => new DropdownOption
                                           {
                                               label = s.FieldValue,
                                               value = s.FieldValue
                                           }).ToList();//SQLDataAccess.LoadFildValue(bdName);

                ctrl.AutocompleteSource = new Common.Models
                    .DropdownOptionAutocompleteSource(converter.Source);
            }

            if (!string.IsNullOrWhiteSpace(val) && !SysFields.Any(p => p.FieldValue == val))
            {
                converter.Source.Insert(0, ((Common.Models.DropdownOptionAutocompleteSource)ctrl.AutocompleteSource).AddIfNotExist(val));
            }

            return converter.Source.FirstOrDefault(p => p.label == val);
        }

        //private object ProcessJe(bool reset, Autocomplete ctrl)
        //{
        //    var val = Source?.Je ?? 0m;

        //    var binding = BindingOperations.GetBinding(ctrl, Autocomplete.SelectedItemProperty);
        //    var converter = binding.Converter as AutocompleteDropdownOptionConverter;
        //    if (reset)
        //    {
        //        if ((_customerSource?.Count).GetValueOrDefault() == 0)
        //        {
        //            _customerSource = SQLDataAccess.LoadByxValue(nameof(Common.Models.WeighingRecord.Kh)).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
        //        }

        //        converter.Source = _customerSource.Select(p => new DropdownOption
        //        {
        //            label = p,
        //            value = p
        //        }).ToList();

        //        ctrl.AutocompleteSource = new Common.Models
        //            .DropdownOptionAutocompleteSource(converter.Source)
        //        {

        //        };

        //    }

        //    if (!string.IsNullOrWhiteSpace(val)
        //            && !converter.Source.Any(p => p.label == val))
        //    {
        //        converter.Source.Insert(0, ((Common.Models.DropdownOptionAutocompleteSource)ctrl.AutocompleteSource).AddIfNotExist(val));
        //    }

        //    return converter.Source.FirstOrDefault(c => c.label.Equals(val));
        //}

        private async Task<object> ProcessFhdw(bool reset, Autocomplete ctrl)
        {
            var val = Source?.Fhdw;

            var binding = BindingOperations.GetBinding(ctrl, Autocomplete.SelectedItemProperty);
            var converter = binding.Converter as AutocompleteDropdownOptionConverter;
            if (reset)
            {
                if (SysFields == null || SysFields.Count == 0)
                {
                    using var db = AJDatabaseService.GetDbContext();
                    var paged = await db.SysFields.AsNoTracking().ToListAsync();
                    SysFields = AJAutoMapperService.Instance()
                        .Mapper.Map<List<Common.EF.Tables.SysField>, List<SysField>>(paged);
                }
                converter.Source = SysFields.Where(p => p.FieldType == ctrl.Name && !string.IsNullOrWhiteSpace(p.FieldValue))
                                            .Select(s => new DropdownOption
                                            {
                                                label = s.FieldValue,
                                                value = s.FieldValue
                                            }).ToList();// SQLDataAccess.LoadFildValue(bdName);
                ctrl.AutocompleteSource = new Common.Models
                    .DropdownOptionAutocompleteSource(converter.Source);

                //设置发货单位的默认值
                var fhdw_mrz = SettingsHelper.AWSV2Settings.Settings["Fhdw_mrz"]?.Value ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(fhdw_mrz) && string.IsNullOrWhiteSpace(val))
                {
                    Source.Fhdw = fhdw_mrz;
                }
            }

            if (!string.IsNullOrWhiteSpace(val)
                    && !converter.Source.Any(p => p.label == val))
            {
                converter.Source.Insert(0, ((Common.Models.DropdownOptionAutocompleteSource)ctrl.AutocompleteSource).AddIfNotExist(val));
            }

            return string.IsNullOrEmpty(val) ? null : converter.Source.FirstOrDefault(p => p.label == val);
        }

        private async Task<object> ProcessGblx(bool reset, Autocomplete ctrl)
        {
            var val = Source?.Gblx ?? string.Empty;
            var binding = BindingOperations.GetBinding(ctrl, Autocomplete.SelectedItemProperty);
            var converter = binding.Converter as AutocompleteDropdownOptionConverter;
            if (reset)
            {
                using var commonCtrl = new CommonController();
                _gblxSource = await commonCtrl.DropdownOptionsByField(nameof(Common.Models.WeighingRecord.Gblx), MobileConfigurationMgr.DropdownOptionSource.WeighingRecord) ?? new List<string>();

                var source = _gblxSource
                .Select(p => new DropdownOption
                {
                    label = p,
                    value = p
                }).ToList();
                converter.Source = source;

                ctrl.AutocompleteSource = new Common.Models
                    .DropdownOptionAutocompleteSource(source)
                {
                    DefaultValueGenerateHandle = (label) =>
                    {
                        return new Random(Guid.NewGuid().GetHashCode()).Next(10, int.MaxValue);
                    }
                };
            }

            if (!string.IsNullOrWhiteSpace(val)
                    && !converter.Source.Any(p => p.label == val))
            {
                converter.Source.Insert(0, ((Common.Models.DropdownOptionAutocompleteSource)ctrl.AutocompleteSource).AddIfNotExist(val));
            }

            return string.IsNullOrWhiteSpace(val) ? null : converter.Source.FirstOrDefault(p => p.label == val);
        }

        private async Task<object> ProcessWz(bool reset, Autocomplete ctrl)
        {
            var val = reset ? string.Empty : Source?.Wz ?? string.Empty;
            var binding = BindingOperations.GetBinding(ctrl, Autocomplete.SelectedItemProperty);
            var converter = binding.Converter as AutocompleteDropdownOptionConverter;

            if (reset)
            {
                using var db = AJDatabaseService.GetDbContext();
                var pages = await db.Goods.AsNoTracking().OrderBy(p => p.Id).ToPagedListAsync(1, 100);
                _goodsSource = pages.Data != null
                    ? pages.Data.Select(p => p.Name).ToList() : new List<string>();
                converter.Source = _goodsSource.Select(p => new DropdownOption
                {
                    label = p,
                    value = p,
                    key = "0"
                }).ToList();

                ctrl.AutocompleteSource = new Common.Models
                    .DropdownOptionAutocompleteSource(converter.Source)
                {

                };

            }

            if (!string.IsNullOrWhiteSpace(val)
                    && !converter.Source.Any(p => p.label == val))
            {
                converter.Source.Insert(0, ((Common.Models.DropdownOptionAutocompleteSource)ctrl.AutocompleteSource).AddIfNotExist(val));
            }

            GoodsItem = converter.Source.FirstOrDefault(g => g.label.Equals(val))?.value as string;
            return converter.Source.FirstOrDefault(p => p.label == val);
        }

        private async Task<object> ProcessGoodsSpec(bool reset, Autocomplete ctrl)
        {
            var val = Source?.GoodsSpec ?? string.Empty;
            var binding = BindingOperations.GetBinding(ctrl, Autocomplete.SelectedItemProperty);
            var converter = binding.Converter as AutocompleteDropdownOptionConverter;
            if (reset)
            {
                await Task.Delay(600);

                converter.Source = new List<DropdownOption>();

                ctrl.AutocompleteSource = new Common.Models
                    .DropdownOptionAutocompleteSource(converter.Source)
                {

                };
            }

            if (!string.IsNullOrWhiteSpace(val)
                    && !converter.Source.Any(p => p.label == val))
            {
                converter.Source.Insert(0, ((Common.Models.DropdownOptionAutocompleteSource)ctrl.AutocompleteSource).AddIfNotExist(val));
            }

            return string.IsNullOrWhiteSpace(val) ? null : converter.Source.FirstOrDefault(g => g.label.Equals(val));
        }

        private async Task<object> ProcessCh(bool reset, Autocomplete ctrl)
        {
            var val = Source?.Ch ?? string.Empty;
            var binding = BindingOperations.GetBinding(ctrl, Autocomplete.SelectedItemProperty);
            var converter = binding.Converter as AutocompleteDropdownOptionConverter;
            if (reset)
            {
                using var commonCtrl = new CommonController();
                _carSource = await commonCtrl.DropdownOptionsByField(nameof(Common.Models.WeighingRecord.Ch), MobileConfigurationMgr.DropdownOptionSource.WeighingRecord) ?? new List<string>();

                converter.Source = _carSource.Select(p => new DropdownOption
                {
                    label = p,
                    value = p
                }).ToList();

                ctrl.AutocompleteSource = new Common.Models
                    .DropdownOptionAutocompleteSource(converter.Source)
                {

                };
            }

            if (!string.IsNullOrWhiteSpace(val)
                    && !converter.Source.Any(p => p.label == val))
            {
                converter.Source.Insert(0, ((Common.Models.DropdownOptionAutocompleteSource)ctrl.AutocompleteSource).AddIfNotExist(val));
            }

            return string.IsNullOrEmpty(val) ? null : converter.Source.FirstOrDefault(c => c.label.Equals(val));
        }

        private async Task<object> ProcessKh(bool reset, Autocomplete ctrl)
        {
            var val = Source?.Kh ?? string.Empty;
            var binding = BindingOperations.GetBinding(ctrl, Autocomplete.SelectedItemProperty);
            var converter = binding.Converter as AutocompleteDropdownOptionConverter;
            if (reset)
            {
                using var db = AJDatabaseService.GetDbContext();
                _customerSource = await db.Customers.AsNoTracking()
                    .Where(p => p.Name != null && p.Name.Length > 0).OrderBy(p => p.Id).Select(p => p.Name).Take(1000).ToListAsync();

                converter.Source = _customerSource
                    .Select(p => new DropdownOption
                    {
                        label = p,
                        value = p
                    }).ToList();

                ctrl.AutocompleteSource = new Common.Models
                    .DropdownOptionAutocompleteSource(converter.Source)
                {

                };
            }

            if (!string.IsNullOrWhiteSpace(val)
                    && !converter.Source.Any(p => p.label == val))
            {
                converter.Source.Insert(0, ((Common.Models.DropdownOptionAutocompleteSource)ctrl.AutocompleteSource).AddIfNotExist(val));
            }

            var selected = converter.Source.FirstOrDefault(c => c.label.Equals(val));
            CurrentCustomer = selected?.value as string;
            return selected;
        }
        private async Task<object> ProcessKh2OrKh3(bool reset, Autocomplete ctrl)
        {
            var val = ctrl.Name == "Kh2" ? (Source?.Kh2 ?? string.Empty) : (Source?.Kh3 ?? string.Empty);
            var binding = BindingOperations.GetBinding(ctrl, Autocomplete.SelectedItemProperty);
            var converter = binding.Converter as AutocompleteDropdownOptionConverter;
            if (reset)
            {
                if (SysFields == null || SysFields.Count == 0)
                {
                    using var db = AJDatabaseService.GetDbContext();
                    var paged = await db.SysFields.AsNoTracking().ToListAsync();
                    SysFields = AJAutoMapperService.Instance()
                        .Mapper.Map<List<Common.EF.Tables.SysField>, List<SysField>>(paged);
                }
                converter.Source = SysFields.Where(p => p.FieldType == ctrl.Name)
                                    .Select(p => new DropdownOption
                                    {
                                        label = p.FieldValue,
                                        value = p.FieldValue
                                    }).ToList();

                ctrl.AutocompleteSource = new Common.Models
                    .DropdownOptionAutocompleteSource(converter.Source);
            }

            if (!string.IsNullOrWhiteSpace(val)
                    && !converter.Source.Any(p => p.label == val))
            {
                converter.Source.Insert(0, ((Common.Models.DropdownOptionAutocompleteSource)ctrl.AutocompleteSource).AddIfNotExist(val));
            }

            return converter.Source.FirstOrDefault(c => c.label.Equals(val));
        }

        /// <summary>
        /// 每次称重按钮点击前，手动提交所有Autocomplete控件数据，防止有些字段数据没保存上
        /// </summary>
        public void Submit()
        {
            var (autocompleteCtrl, _) = GetFormItems();

            foreach (var control in autocompleteCtrl)
            {
                if (!string.IsNullOrWhiteSpace(control.SearchTerm))
                {
                    control.ForceCompleteSearchTermInput();
                }
            }
        }

        public void Refresh(ref WeighingRecord weighingRecord, bool reset = false)
        {
            Source = weighingRecord;
            WeighFormGrid.Dispatcher.Invoke(SetGridDataSource);
            WeighFormStackPanel.Dispatcher.Invoke(SetStackPanelDataSource);
            //WeighFormGrid.DataContext = weighingRecord;
            //WeighFormStackPanel.DataContext = weighingRecord;
            if (reset)
            {
                GoodsItem = null;
                CurrentCustomer = null;
            }

            var (autocompleteCtrls, textboxCtrls) = GetFormItems();

            foreach (var item in autocompleteCtrls)
            {
                AutocomplteCtrlRefresh(item, reset);
            }
            foreach (var item in textboxCtrls)
            {
                TextBoxCtrlRefresh(item, reset);
            }
        }

        private void TextBoxCtrlRefresh(TextBox ctrl, bool reset)
        {
            if (reset)
            {
                if (!string.IsNullOrWhiteSpace(ctrl.Text))
                {
                    ctrl.Text = string.Empty;
                }
                return;
            }

        }

        private async void AutocomplteCtrlRefresh(Autocomplete ctrl, bool reset)
        {
            if (reset)
            {
                if (!string.IsNullOrWhiteSpace(ctrl.SearchTerm))
                {
                    ctrl.SearchTerm = string.Empty;
                    ctrl.SelectedItem = null;
                }
            }
            // 根据Source重新绑值
            var ctrlName = ctrl.Name;
            var key = AutocompleteCtrlSourceProcessMaps.Keys.FirstOrDefault(p => ctrlName == p || ctrlName.StartsWith(p));
            if (!string.IsNullOrWhiteSpace(key))
            {
                ctrl.SelectedItem = await AutocompleteCtrlSourceProcessMaps[key].Invoke(reset, ctrl);
            }
            else
            {
                ctrl.SelectedItem = null;
            }

        }

        private void SetGridDataSource()
        {
            WeighFormGrid.DataContext = Source;
        }
        private void SetStackPanelDataSource()
        {
            WeighFormStackPanel.DataContext = Source;
        }

        public void SwitchTemplate(AppSettingsSection mainSettings, IEnumerable<DynamicPoundFieldItem> fields,
            HomeTemplateType homeTemplateType = HomeTemplateType.Default)
        {
            _mainSetting = mainSettings;
            // 只取出来是表单显示的
            _fields = fields.Where(p => p.IsFormDisplay).ToList();
            // 这个目前没什么用了,  重构后都是渲染的两列 --阿吉 2024年5月24日12点34分
            _homeTemplateType = homeTemplateType;

            LoadExcelToDT();
            CreateControlsForMonitorLayout();
        }

        private void LoadExcelToDT()
        {
            _formItems.Clear();

            var index = 0;
            var colIndex = 0;

            foreach (var item in _fields)
            {
                if (colIndex == 0)
                {
                    _formItems.Add(new PoundFormControlData
                    {
                        Left = item
                    });
                    colIndex++;
                    continue;
                }
                if (colIndex == 1)
                {
                    _formItems[index].Right = item;
                    index++;
                    colIndex = 0;
                }

            }

        }

        /// <summary>
        /// 如果首页是监控布局的,使用该方法创建表单
        /// </summary>
        private async void CreateControlsForMonitorLayout()
        {
            WeighFormGrid.Children.Clear();
            WeighFormGrid.RowDefinitions.Clear();
            WeighFormGrid.ColumnDefinitions.Clear();

            var mainStackPanel = new StackPanel();
            var weighingControl = _mainSetting.Settings["WeighingControl"].TryGetString();

            var autoMode = !(weighingControl.Equals("Hand") || weighingControl.Equals("Btn"));

            foreach (var item in _formItems)
            {
                var grid = new Grid
                {
                    Margin = new Thickness(8, 8, 8, 8)
                };
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                if (item.Left != null)
                {
                    var ctrl = await ProcessFormControl(item.Left, autoMode);
                    if (ctrl != null)
                    {
                        grid.Children.Add(ctrl);
                        Grid.SetColumn(ctrl, 0);
                    }

                }
                if (item.Right != null)
                {
                    var ctrl = await ProcessFormControl(item.Right, autoMode);
                    if (ctrl != null)
                    {
                        grid.Children.Add(ctrl);
                        Grid.SetColumn(ctrl, 1);
                    }
                }
                mainStackPanel.Children.Add(grid);
            }

            WeighFormGrid.Children.Add(mainStackPanel);
        }

        private async Task<DockPanel> ProcessFormControl(DynamicPoundFieldItem item, bool autoMode)
        {
            if (string.IsNullOrEmpty(item.Field) || string.IsNullOrEmpty(item.Label))
            {
                return null;
            }

            var bdName = ProcessPoundField(item.ToConfigFieldKey(), out var prop);

            if (string.IsNullOrWhiteSpace(bdName))
            {
                return null;
            }

            var dockPanel = new DockPanel();

            // 不需要label了 --阿吉 2024年8月19日16点28分
            //var label = CreateLabel(item.Label, autoMode);
            //dockPanel.Children.Add(label);

            // 如果是自动模式或者不是自动完成控件, 则创建输入框
            if (autoMode || !_autocompleteNameArray.Any(p => p == bdName || bdName.StartsWith(p)))
            {
                // 自动称重模式全部是只读TextBox
                var ctrl = CreateTextBox(item, bdName, autoMode, prop);
                dockPanel.Children.Add(ctrl);
            }
            else
            {
                // 创建自动完成控件
                var ctrl = await CreateAutoComplete(item, bdName);
                dockPanel.Children.Add(ctrl);
            }

            return dockPanel;
        }

        //private void DisplayToGrid()
        //{
        //    WeighFormGrid.Children.Clear();
        //    WeighFormGrid.RowDefinitions.Clear();
        //    WeighFormGrid.ColumnDefinitions.Clear();
        //    //画grid的行列
        //    for (int i = 0; i < _dataTable.Rows.Count; i++)
        //    {
        //        WeighFormGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(55, GridUnitType.Pixel) });
        //    }
        //    for (int i = 0; i < _dataTable.Columns.Count; i++)
        //    {
        //        WeighFormGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength((i == 0 || i == 2) ? 105 : 428, GridUnitType.Star) });
        //    }
        //    var weighingControl = _mainSetting.Settings["WeighingControl"].TryGetString();
        //    var autoMode = !(weighingControl.Equals("Hand") || weighingControl.Equals("Btn"));
        //    //给grid填充数据
        //    for (int i = 0; i < _dataTable.Rows.Count; i++)
        //    {
        //        for (int j = 0; j < _dataTable.Columns.Count; j++)
        //        {
        //            string gridContent = _dataTable.Rows[i][j].ToString();

        //            if (!gridContent.Equals("")) //datatable不为空填充数据，else为空合并到前一个单元格
        //            {
        //                var bdName = ProcessPoundField(gridContent, out var prop);

        //                if (gridContent.StartsWith("_")) //以 _ 开头为变量，else为textblock
        //                {

        //                    if (string.IsNullOrEmpty(bdName))
        //                    {
        //                        continue;
        //                    }

        //                    if (!autoMode)
        //                    {
        //                        if (_autocompleteNameArray.Any(p => p == bdName || bdName.StartsWith(p)))
        //                        {
        //                            var acb = CreateAutoComplete(bdName);

        //                            WeighFormGrid.Children.Add(acb);
        //                            acb.SetValue(Grid.RowProperty, i);
        //                            acb.SetValue(Grid.ColumnProperty, j);
        //                        }
        //                        else
        //                        {
        //                            var tb = CreateTextBox(bdName, autoMode, prop);
        //                            WeighFormGrid.Children.Add(tb);
        //                            tb.SetValue(Grid.RowProperty, i);
        //                            tb.SetValue(Grid.ColumnProperty, j);

        //                        }
        //                    }
        //                    else //自动称重模式，全部为readonly的textbox
        //                    {
        //                        var tb = CreateTextBox(bdName, autoMode, prop);

        //                        //添加到grid对应格子中
        //                        WeighFormGrid.Children.Add(tb);
        //                        tb.SetValue(Grid.RowProperty, i);
        //                        tb.SetValue(Grid.ColumnProperty, j);
        //                        tb.MinWidth = 90;
        //                    }

        //                } //以 _ 开头为变量，else为textblock
        //                else //静态内容，textblock
        //                {
        //                    var tb = CreateLabel(gridContent);

        //                    WeighFormGrid.Children.Add(tb);
        //                    tb.SetValue(Grid.RowProperty, i);
        //                    tb.SetValue(Grid.ColumnProperty, j);
        //                } //静态内容，textblock
        //            } //datatable不为空填充数据，else为空合并到前一个单元格
        //            else //grid中显示单元格合并
        //            {
        //                if (WeighFormGrid.Children.Count <= 0) continue;
        //                var item = WeighFormGrid.Children[WeighFormGrid.Children.Count - 1];
        //                int columnSpanNum = Convert.ToInt32(item.GetValue(Grid.ColumnSpanProperty)) + 1;
        //                item.SetValue(Grid.ColumnSpanProperty, columnSpanNum);
        //            } //grid中显示单元格合并
        //        }
        //    } //给grid填充数据

        //    GridHelper.SetShowBorder(WeighFormGrid, false);
        //}

        private string ProcessPoundField(string content, out PropertyInfo prop)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                prop = null;
                return string.Empty;
            }
            var bdName = content.Substring(1);
            bdName = bdName[0].ToString().ToUpper() + bdName.Substring(1);
            prop = _props.FirstOrDefault(p => p.Name.Equals(bdName, StringComparison.OrdinalIgnoreCase));

            if (prop == null)
            {
                // 这里做下Dj 和 GoodsPrice 的兼容
                if (!bdName.Equals("dj", StringComparison.OrdinalIgnoreCase))
                {
                    return string.Empty;
                }
            }
            else
            {
                bdName = prop.Name;
            }

            return bdName;
        }

        private async Task<Autocomplete> CreateAutoComplete(DynamicPoundFieldItem fieldItem, string bdName)
        {
            var acb = new Autocomplete
            {
                Name = bdName,
                SearchOnInitialFocus = true,
                //FontSize = 20,
                ItemTemplate = _simpleAutocompleteDataTemplate,
                BorderBrush = _borderBrush,
                Background = _moduleBrush,
                Padding = new Thickness(6, 8, 6, 8),
                Hint = fieldItem.Label,
                IsFloating = true,
                HintBackground = _moduleBrush,
                Margin = new Thickness(8, 8, 8, 8),
            };

            MaterialDesignThemes.Wpf.TextFieldAssist.SetTextFieldCornerRadius(acb, new CornerRadius(0));

            if (_homeTemplateType == HomeTemplateType.Default)
            {
                acb.Margin = new Thickness(0, 8, 0, 8);
            }

            acb.SearchTermInputCompleted += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(e.Search))
                {
                    return;
                }
                acb.Dispatcher.Invoke(() =>
                {
                    var source = acb.AutocompleteSource as Common.Models.DropdownOptionAutocompleteSource;
                    var item = source.AddIfNotExist(e.Search);
                    if (acb.SelectedItem == null)
                    {
                        acb.SelectedItem = item;
                    }
                });
            };

            acb.SelectedItemChanged += (s, e) =>
            {
                if (bdName.Equals(nameof(Common.Models.WeighingRecord.Ch)))
                {
                    var carNo = string.Empty;
                    if (e.SelectedItem is DropdownOption option)
                    {
                        carNo = option.label;
                    }

                    ManualInputCarNoCompleted?.Invoke(carNo, new EventArgs());
                }

            };

            //var style = new System.Windows.Style(typeof(TextBox));
            //style.Setters.Add(new Setter(TextBox.VerticalContentAlignmentProperty, System.Windows.VerticalAlignment.Center));

            var converter = new AutocompleteDropdownOptionConverter();

            var binding = new Binding(bdName)
            {
                Mode = BindingMode.TwoWay,
                Converter = converter,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                //Source = this.Source
            };

            if (_valueConverterParam.Any(p => p == bdName || bdName.StartsWith(p)))
            {
                binding.ConverterParameter = nameof(DropdownOption.value);
            }

            if (bdName == nameof(WeighingRecord.AxleNum))
            {
                converter.Source = _axleNumSource;
                acb.AutocompleteSource = new Common.Models
                    .DropdownOptionAutocompleteSource(converter.Source);

                acb.SelectedItem = Source.AxleNum;
                acb.ItemTemplate = _simpleAutocompleteDataTemplate;

            }
            if (bdName == nameof(WeighingRecord.IsLimit))
            {
                var curr = _booleanDropdownOptions
                    .Find(c => (bool)c.value == (Source.IsLimit));
                converter.Source = _booleanDropdownOptions;
                acb.AutocompleteSource
                    = new Common.Models.DropdownOptionAutocompleteSource(_booleanDropdownOptions)
                    {
                        DefaultValueGenerateHandle = (label) =>
                        {
                            return _booleanStrArray
                            .Any(p => p.Equals(label, StringComparison.OrdinalIgnoreCase));
                        }
                    };
                //acb.ValueMemberPath = "KeyStr";
                acb.SelectedItem = curr;
                acb.ItemTemplate = _simpleAutocompleteDataTemplate;
                acb.IsEnabled = false;

            }
            if (bdName == nameof(WeighingRecord.IsCover))
            {
                var curr = _booleanDropdownOptions.Find(c => (bool)c.value == Source.IsCover);
                converter.Source = _booleanDropdownOptions;
                acb.AutocompleteSource = new Common.Models
                    .DropdownOptionAutocompleteSource(_booleanDropdownOptions)
                {
                    DefaultValueGenerateHandle = (label) =>
                    {
                        return _booleanStrArray
                        .Any(p => p.Equals(label, StringComparison.OrdinalIgnoreCase));
                    }
                };
                //acb.ValueMemberPath = "KeyStr";
                acb.SelectedItem = curr;
                acb.ItemTemplate = _simpleAutocompleteDataTemplate;
                acb.IsEnabled = false;
            }
            if (bdName == nameof(WeighingRecord.IsPay))
            {
                converter.Source = _isPaySource;

                acb.AutocompleteSource = new Common.Models.DropdownOptionAutocompleteSource(converter.Source);
                acb.SelectedItem = converter.Source.FirstOrDefault(P => (int)P.value == Source.IsPay);
                acb.ItemTemplate = _simpleAutocompleteDataTemplate;
                acb.IsEnabled = false;
            }
            if (bdName == nameof(WeighingRecord.Wz))
            {
                binding.Path = new PropertyPath(nameof(GoodsItem));
                binding.Source = this;
                binding.ConverterParameter = nameof(DropdownOption.value);
            }
            if (bdName == nameof(WeighingRecord.GoodsSpec))
            {
                binding.ConverterParameter = nameof(DropdownOption.label);
                acb.KeyDown += Acb_KeyDown;
                acb.KeyUp += Acb_KeyUp;

            }
            if (bdName == nameof(WeighingRecord.Ch))
            {
                acb.HelperText = "输入车牌后请按回车键确认";
                acb.IsFloating = true;

                acb.KeyDown += Acb_CH_KeyDown;
                acb.MouseDoubleClick += (sender, args) =>
                {
                    IPCHelper.OpenApp("QuickPlate");
                };
                binding.ConverterParameter = nameof(DropdownOption.label);
                acb.SelectedItemChanged += Acb_SelectionChanged;
            }
            if (bdName == nameof(WeighingRecord.Kh))
            {
                binding.Path = new PropertyPath(nameof(CurrentCustomer));
                binding.Source = this;
                binding.ConverterParameter = nameof(DropdownOption.value);

            }
            acb.SetBinding(Autocomplete.SelectedItemProperty, binding);
            var key = AutocompleteCtrlSourceProcessMaps.Keys.FirstOrDefault(p => p == bdName || bdName.StartsWith(p));
            if (!string.IsNullOrEmpty(key))
            {
                acb.SelectedItem = await AutocompleteCtrlSourceProcessMaps[key](true, acb);
            }

            return acb;
        }

        private TextBox CreateTextBox(DynamicPoundFieldItem fieldItem, string bdName, bool autoMode, PropertyInfo prop)
        {
            var tb = new TextBox
            {
                //FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = double.NaN,
                IsReadOnly = autoMode,
                BorderBrush = _borderBrush,
                Margin = new Thickness(8, 8, 8, 8),
                Opacity = autoMode ? 0.4 : 1
            };

            MaterialDesignThemes.Wpf.TextFieldAssist.SetTextFieldCornerRadius(tb, new CornerRadius(0));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb, fieldItem.Label);
            MaterialDesignThemes.Wpf.HintAssist.SetBackground(tb, _moduleBrush);

            if (_homeTemplateType == HomeTemplateType.Default)
            {
                tb.Margin = new Thickness(0, 8, 0, 8);
            }

            var binding = new Binding(bdName)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };

            if (prop != null)
            {
                var isDecimal = prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?);
                var isInt = prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?);
                if (isDecimal || isInt)
                {
                    binding.Converter = new NumberToStringConverter
                    {
                        NumberDataType = isDecimal ? NumberDataType.Decimal : NumberDataType.Int,
                    };
                }

                if (prop.PropertyType == typeof(DateTime)
                    || prop.PropertyType == typeof(DateTime?))
                {
                    binding.Converter = new DatetimeToStringConverter();
                }
            }

            //设置值转换器
            if (bdName == nameof(WeighingRecord.IsLimit) || bdName == nameof(WeighingRecord.IsCover))
            {
                binding.Converter = new Converter.BoolToStringConverters();
            }
            else if (bdName == nameof(WeighingRecord.IsPay))
            {
                binding.Converter = new Converter.PayToStringConverters();
            }

            if (bdName == nameof(WeighingRecord.Fhdw))
            {
                //设置发货单位的默认值
                var fhdw_mrz = _mainSetting.Settings["Fhdw_mrz"].TryGetString();
                if (!string.IsNullOrWhiteSpace(fhdw_mrz) && string.IsNullOrWhiteSpace(Source.Fhdw))
                {
                    Source.Fhdw = fhdw_mrz;
                }
                if (!autoMode)
                {
                    tb.IsReadOnly = false;
                }
            }

            if (bdName == "Dj")
            {
                binding = new Binding
                {
                    Mode = BindingMode.TwoWay,
                    Path = new PropertyPath(nameof(Source.GoodsPrice)),
                    //Source = this,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Converter = new NumberToStringConverter() { NumberDataType = NumberDataType.Decimal }
                };
            }

            if (!autoMode)
            {
                if (bdName == nameof(WeighingRecord.Kz))
                {
                    tb.IsReadOnly = _mainSetting.Settings["Discount"].TryGetInt() == 1;
                }

                if (bdName == nameof(WeighingRecord.Kl))
                {
                    tb.IsReadOnly = _mainSetting.Settings["Discount"].TryGetInt() == 2;
                }

                if (_notAutoModeTextboxReadonlyFileds.Contains(bdName))
                {
                    tb.IsReadOnly = false;
                }

                if (_mainSetting.Settings["WeighingMode"].TryGetString().Equals("Once")
                    && bdName == nameof(WeighingRecord.Pz))
                {
                    tb.IsReadOnly = false;
                }

                if (_textBoxNameArray.Any(p => p == bdName))
                {
                    tb.Name = bdName;
                }
            }

            // 重量值字段强制只读
            if (_weightFields.Contains(bdName))
            {
                tb.IsReadOnly = true;
            }

            tb.SetBinding(TextBox.TextProperty, binding);

            return tb;
        }

        private Label CreateLabel(object content, bool autoMode)
        {
            var label = new Label
            {
                //设置静态文本的内容和格式
                Margin = new Thickness(8, 0, 8, 0),
                Content = content,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right,
                Foreground = _borderBrush,
                Opacity = autoMode ? 0.4 : 1,
                MinWidth = 120
                //FontSize = 20,
                //VerticalContentAlignment = VerticalAlignment.Center,

            };

            return label;
        }

        private (List<Autocomplete> autocompleteCtrls, List<TextBox> textBoxCtrls) GetFormItems()
        {
            var autocompleteCtrls = new List<Autocomplete>();
            var textBoxCtrls = new List<TextBox>();

            if ((WeighFormGrid.Children?.Count).GetValueOrDefault() == 0)
            {
                return (autocompleteCtrls, textBoxCtrls);
            }

            var mainStackPanel = (StackPanel)WeighFormGrid.Children[0];

            /*
                <Grid>
                    <StackPanel>
                        <Grid>
                            <dockpanel>
                        </Grid>
                    </StackPanel>
                </Grid>
             */

            foreach (Grid grid in mainStackPanel.Children)
            {
                foreach (DockPanel item in grid.Children)
                {
                    foreach (var item2 in item.Children)
                    {
                        if (item2 is Autocomplete comboBox1)
                        {
                            autocompleteCtrls.Add(comboBox1);
                        }
                        if (item2 is TextBox textBox1)
                        {
                            textBoxCtrls.Add(textBox1);
                        }
                    }
                }
            }

            return (autocompleteCtrls, textBoxCtrls);
        }

        private void Acb_SelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            //try
            //{
            //    Globalspace._isManual = true;
            //    var comboBox = (sender as Autocomplete);

            //    if (!comboBox.IsLoaded)
            //        return;
            //    if (comboBox.SelectedItem is DropdownOption opt)
            //    {
            //        SetValues(opt.label);
            //    }

            //}
            //catch { }
        }

        private void SetValues(string value)
        {
            //if (!string.IsNullOrEmpty(value))
            //{
            //    var carInfo = Common.Data.SQLDataAccess.GetCarModel(value);
            //    if (carInfo != null)
            //    {

            //        foreach (var control in this.WeighFormGrid.Children)
            //        {
            //            var txtbox = control as Control;
            //            if (txtbox != null && txtbox.Name == "Driver")
            //            {
            //                (this.WeighFormGrid.DataContext as WeighingRecord).Driver = carInfo.Driver;

            //                //WeighFormStackPanel.DataContext = Source;
            //                (txtbox as TextBox).Text = carInfo.Driver;
            //            }
            //            else if (txtbox != null && txtbox.Name == "DriverPhone")
            //            {
            //                (this.WeighFormGrid.DataContext as WeighingRecord).DriverPhone = carInfo.DriverPhone;
            //                (txtbox as TextBox).Text = carInfo.DriverPhone;
            //            }
            //        }

            //    }
            //}
        }

        private void Acb_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Back && e.Key != System.Windows.Input.Key.Delete)
            {

                if (sender is Autocomplete ac)
                {
                    ac.SearchTerm = string.Empty;
                    ac.SelectedItem = null;
                }

                e.Handled = true;
            }
        }

        private void Acb_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Back && e.Key != System.Windows.Input.Key.Delete)
            {
                if (sender is Autocomplete ac)
                {
                    ac.SearchTerm = string.Empty;
                    ac.SelectedItem = null;
                }
                e.Handled = true;
            }
        }
        private void Acb_CH_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key != System.Windows.Input.Key.Back && e.Key != System.Windows.Input.Key.Delete)
            //{
            //    Globalspace._isManual = true;
            //    (sender as AutoCompleteBox).Text = String.Empty;
            //    e.Handled = true;
            //}
            Globalspace._isManual = true;
        }

        private async void CurrentCustomerChangedAsync()
        {
            if (!string.IsNullOrWhiteSpace(_currentCustomer))
            {
                Source.Kh = _customerSource.FirstOrDefault(p => p == _currentCustomer);
                var goodsId = -1L;
                var goodsPrice = 0m;
                using var db = AJDatabaseService.GetDbContext();
                if (!string.IsNullOrWhiteSpace(GoodsItem))
                {
                    var name = GoodsItem;
                    var goodsItem = await db.Goods.AsNoTracking().FirstOrDefaultAsync(p => p.Name == name);
                    goodsId = (goodsItem?.Id).GetValueOrDefault();
                    Source.GoodsPrice = goodsPrice = (goodsItem?.Price).GetValueOrDefault();
                }

                var currentCustomer = await db.Customers.AsNoTracking().FirstOrDefaultAsync(p => p.Name == _currentCustomer);
                var currentCustomerId = (currentCustomer?.Id).GetValueOrDefault();

                var goodsVSCustomerPrice = await db.GoodsVsCustomerPrices
                    .FirstOrDefaultAsync(p => p.CustomerId == currentCustomerId && p.GoodsId == goodsId);

                Source.GoodsPrice = (goodsVSCustomerPrice?.Price ?? goodsPrice);
            }
            else
            {
                Source.Kh = string.Empty;
            }

            var (autocompleteCtrls, textboxCtrls) = GetFormItems();

            var khAuto = autocompleteCtrls.FirstOrDefault(p => p.Name == nameof(WeighingRecord.Kh));
            if (khAuto != null && string.IsNullOrWhiteSpace(Source.Kh))
            {
                if (khAuto.SelectedItem == null)
                {
                    Source.Kh = khAuto.SearchTerm;
                }
                else
                {
                    Source.Kh = ((DropdownOption)khAuto.SelectedItem).label;
                }
            }

            var jeTxt = textboxCtrls.FirstOrDefault(p => p.Name == nameof(WeighingRecord.Je));
            if (jeTxt != null)
            {
                Source.ComputeCost(2);
                jeTxt.Text = Source.Je.ToString();
            }
        }

        private async void GoodsItemChangedAsync()
        {
            IList<Common.EF.Tables.GoodsSpec> specList = null;
            if (!string.IsNullOrWhiteSpace(GoodsItem))
            {
                using var db = AJDatabaseService.GetDbContext();

                Source.Wz = _goodsSource.FirstOrDefault(p => p == GoodsItem);

                var name = GoodsItem;
                var detail = await db.Goods.Include(p => p.SpecList).AsNoTracking().FirstOrDefaultAsync(p => p.Name == name);

                specList = detail?.SpecList ?? new List<Common.EF.Tables.GoodsSpec>();

                var cusomerId = -1L;
                if (!string.IsNullOrWhiteSpace(CurrentCustomer))
                {
                    var customer = await db.Customers.AsNoTracking().FirstOrDefaultAsync(p => p.Name == _currentCustomer);

                    cusomerId = (customer?.Id).GetValueOrDefault();
                }

                var goodsId = detail?.Id;

                var goodsVSCustomerPrice = await db.GoodsVsCustomerPrices
                    .FirstOrDefaultAsync(p => p.CustomerId == cusomerId && p.GoodsId == goodsId);


                Source.GoodsPrice = (goodsVSCustomerPrice?.Price ?? detail?.Price).GetValueOrDefault();
            }
            else
            {
                Source.Wz = string.Empty;
                Source.GoodsSpec = string.Empty;
            }

            var (autocompleteCtrls, textboxCtrls) = GetFormItems();

            var goodsSpecAuto = autocompleteCtrls.FirstOrDefault(p => p.Name == nameof(WeighingRecord.GoodsSpec));
            var wzAuto = autocompleteCtrls.FirstOrDefault(p => p.Name == nameof(WeighingRecord.Wz));

            if (goodsSpecAuto != null)
            {
                var options = new List<DropdownOption>();
                if (specList?.Count > 0)
                {
                    options = specList.Select(p => new DropdownOption
                    {
                        label = p.Name,
                        value = p.Name
                    }).ToList();
                }
                var binding = BindingOperations.GetBinding(goodsSpecAuto, Autocomplete.SelectedItemProperty);
                var converter = binding.Converter as AutocompleteDropdownOptionConverter;
                converter.Source = options;

                goodsSpecAuto.AutocompleteSource = new Common.Models.DropdownOptionAutocompleteSource(options);
                var firstOption = options.FirstOrDefault();
                Source.GoodsSpec = firstOption?.label;
                goodsSpecAuto.SelectedItem = firstOption;
            }

            if (wzAuto != null && string.IsNullOrWhiteSpace(Source.Wz))
            {
                if (wzAuto.SelectedItem == null)
                {
                    Source.Wz = wzAuto.SearchTerm;
                }
                else
                {
                    Source.Wz = ((DropdownOption)wzAuto.SelectedItem).label;
                }
            }
        }
    }

    public class KeyValue
    {
        public string KeyStr { get; set; }
        public bool ValuePath { get; set; }
    }

}
