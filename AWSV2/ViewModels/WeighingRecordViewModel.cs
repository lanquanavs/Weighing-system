using Common.Model;
using Common.Utility;
using Common.Utility.AJ.Extension;
using FluentValidation;
using Newtonsoft.Json;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Common.Utility.AJ.MobileConfiguration;
using NPOI.Util;
using System.Reflection;
using AWSV2.Models;
using AWSV2.Services;
using Common.Models;
using Masuit.Tools.Systems;
using Yitter.IdGenerator;
using Common.Utility.AJ;
using Common.Platform;

namespace AWSV2.ViewModels
{
    public class FormItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Combox { get; set; }
        public DataTemplate Date { get; set; }
        public DataTemplate TextBox { get; set; }
        public DataTemplate InputNumber { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }
            if (item is DynamicPoundFieldItem fieldItem)
            {
                if (Enum.TryParse(fieldItem.FormType, true, out VbenAdminFormType fType))
                {
                    switch (fType)
                    {
                        case VbenAdminFormType.Select:
                        case VbenAdminFormType.AutoComplete:
                            return Combox;
                        case VbenAdminFormType.Input:
                            return TextBox;
                        case VbenAdminFormType.InputNumber:
                            return InputNumber;
                        case VbenAdminFormType.RangePicker:
                        case VbenAdminFormType.DatePicker:
                            return Date;
                        default:
                            break;
                    }
                }

                return null;
            }
            return null;
        }
    }

    public class WeighingRecordViewModel : Screen
    {
        public string Title { get; set; }

        public Common.Models.WeighingRecord Wrm { get; set; }
        /// <summary>
        /// 新增一个备份数据，用于编辑数据 --阿吉 2023年7月5日15点44分
        /// </summary>
        public Common.Models.WeighingRecord Copy { get; set; }

        private IWindowManager windowManager;

        public AppSettingsSection _mainSettings;

        private bool _editMode;

        private ObservableCollection<DynamicPoundFieldItem> _dyanmicPoundFieldItems;
        public ObservableCollection<DynamicPoundFieldItem> DyanmicPoundFieldItems
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

        private List<DropdownOption> _fieldSource;

        public WeighingRecordViewModel(IModelValidator<WeighingRecordViewModel> validator,
            List<DropdownOption> fieldSource,
            ICollection<DynamicPoundFieldItem> dyanmicPoundFieldItems,
            IWindowManager _windowManager, Common.Models.WeighingRecord wrm = null) : base(validator)
        {
            windowManager = _windowManager;
            _mainSettings = SettingsHelper.AWSV2Settings;
            var notEditableFields = new List<string>();
            _fieldSource = fieldSource;
            var props = typeof(Common.Models.WeighingRecord).GetRuntimeProperties();

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(bool)
                    || prop.Name == nameof(Common.Models.WeighingRecord.Bh)
                    || prop.Name == nameof(Common.Models.WeighingRecord.Gblx))
                {
                    notEditableFields.Add(prop.Name);
                }
            }
            var by20Field = nameof(Common.Models.WeighingRecord.By20);
            
            var weightValueFields = Common.Models.WeighingRecord.GetWeightValueFields();
            var weightUnit = _mainSettings.Settings["WeighingUnit"].TryGetString("kg");

            var dateFields = Common.Models.WeighingRecord.GetWeightWeighDateFields();

            var itemsSource = dyanmicPoundFieldItems
                .Where(p => p.IsColumnDisplay && p.Field != by20Field)
                .Select(p => new DynamicPoundFieldItem
                {
                    Field = p.Field,
                    Label = p.Field == nameof(Common.Models.WeighingRecord.Bh) ? (wrm == null ? $"根据毛重日期生成" : p.Label) : p.Label,
                    FormType = p.Field == nameof(Common.Models.WeighingRecord.Bh)
                            ? VbenAdminFormType.Input.ToString() : p.FormType,
                    IsFilterDisplay = p.Field != nameof(Common.Models.WeighingRecord.EditReason),
                    IsColumnDisplay = p.IsColumnDisplay,
                    Editable = !notEditableFields.Contains(p.Field),
                    Disabled = p.Field == nameof(Common.Models.WeighingRecord.Bh),
                    Options = p.Options,
                    SuffixText = weightValueFields.Contains(p.Field) ? weightUnit : string.Empty
                }).ToList();

            _editMode = wrm != null;

            if (!_editMode)
            {
                itemsSource.RemoveAll(p => dateFields.Contains(p.Field));
            }

            DyanmicPoundFieldItems = new ObservableCollection<DynamicPoundFieldItem>(itemsSource);

            var rolePermission = Globalspace._currentUser.Permission;
            if (!string.IsNullOrWhiteSpace(rolePermission))
            {

                if (!rolePermission.Contains("修改皮重"))
                {
                    var field = DyanmicPoundFieldItems.FirstOrDefault(p => p.Field == nameof(Common.Models.WeighingRecord.Pz));
                    if (field != null)
                    {
                        field.IsFilterDisplay = false;
                    }
                }
                if (!rolePermission.Contains("修改毛重"))
                {
                    var field = DyanmicPoundFieldItems.FirstOrDefault(p => p.Field == nameof(Common.Models.WeighingRecord.Mz));
                    if (field != null)
                    {
                        field.IsFilterDisplay = false;
                    }
                }
                if (!rolePermission.Contains("修改净重"))
                {
                    var field = DyanmicPoundFieldItems.FirstOrDefault(p => p.Field == nameof(Common.Models.WeighingRecord.Jz));
                    if (field != null)
                    {
                        field.IsFilterDisplay = false;
                    }
                }
                if (!rolePermission.Contains("修改实重"))
                {
                    var field = DyanmicPoundFieldItems.FirstOrDefault(p => p.Field == nameof(Common.Models.WeighingRecord.Sz));
                    if (field != null)
                    {
                        field.IsFilterDisplay = false;
                    }
                }
            }

            if (!_editMode)
            {
                var now = DateTime.Now;
                Wrm = new Common.Models.WeighingRecord
                {
                    Mzrq = now,
                };
                var task = Task.Run(() =>
                {
                    return Common.Models.WeighingRecord.CreateBhAsync();
                });
                task.Wait();
                Wrm.Bh = task.Result;
                Title = "新增称重记录";
            }
            else
            {
                Wrm = wrm;
                Copy = JsonConvert.DeserializeObject<Common.Models.WeighingRecord>(JsonConvert.SerializeObject(wrm));
                Title = "修改称重记录";
            }

            //系统设置了扣重为固定值，不可以由用户输入
            if (_mainSettings.Settings["Discount"].Value.Equals("1"))
            {
                Wrm.Kz = Common.Utility.SettingsHelper.AWSV2Settings.Settings["DiscountWeight"].TryGetDecimal();
            }
            //系统设置了扣率为固定值，不可以由用户输入
            if (Common.Utility.SettingsHelper.AWSV2Settings.Settings["Discount"].Value.Equals("2"))
            {
                Wrm.Kz = Common.Utility.SettingsHelper.AWSV2Settings.Settings["DiscountRate"].TryGetDecimal();
            }

            var formModelProps = Wrm.GetType().GetRuntimeProperties();

            var jzItem = DyanmicPoundFieldItems
                .FirstOrDefault(p => p.Field == nameof(Common.Models.WeighingRecord.Jz));

            foreach (var item in DyanmicPoundFieldItems)
            {
                var formProp = formModelProps.FirstOrDefault(p => p.Name == item.Field);
                if (formProp != null)
                {
                    if (jzItem != null
                        &&
                            (formProp.Name == nameof(Common.Models.WeighingRecord.Mz)
                            || formProp.Name == nameof(Common.Models.WeighingRecord.Pz)))
                    {
                        item.ValueChanged += (s, _) =>
                        {
                            var pzItem = DyanmicPoundFieldItems.FirstOrDefault(p => p.Field == nameof(Common.Models.WeighingRecord.Pz));

                            var pz = pzItem.Value.TryGetDecimal();

                            if (pz == 0)
                            {
                                return;
                            }

                            var mzItem = DyanmicPoundFieldItems.FirstOrDefault(p => p.Field == nameof(Common.Models.WeighingRecord.Mz));


                            jzItem.Value = Math.Round(mzItem.Value.TryGetDecimal() - pzItem.Value.TryGetDecimal(), 2, MidpointRounding.AwayFromZero).ToString();
                        };
                    }

                    if (formProp.PropertyType == typeof(bool))
                    {
                        var val = (bool)formProp.GetValue(Wrm);
                        var sourceFd = _fieldSource.FirstOrDefault(p => p.key == item.Field);
                        if (sourceFd != null)
                        {
                            var label = sourceFd.children.FirstOrDefault(p => (int)p.value == (val ? 1 : 0))?.label;
                            item.Value = label;
                            continue;
                        }
                    }
                    if (formProp.PropertyType == typeof(decimal))
                    {
                        item.Value = ((decimal)formProp.GetValue(Wrm)).ToString();
                        continue;
                    }
                    if (formProp.PropertyType == typeof(DateTime)
                        || formProp.PropertyType == typeof(DateTime?))
                    {
                        var val = formProp.GetValue(Wrm);
                        item.Value = val == null ? string.Empty : (val.ToString()).TryGetDateTime().GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                        continue;
                    }
                    item.Value = formProp.GetValue(Wrm) as string;
                }
            }
        }


        public async void SaveItem()
        {
            if (Validate())
            {
                using var db = AJDatabaseService.GetDbContext();

                var mzSbyField = DyanmicPoundFieldItems
                    .FirstOrDefault(p => p.Field == nameof(Wrm.Mzsby));
                if (mzSbyField != null && string.IsNullOrWhiteSpace(mzSbyField.Value))
                {
                    mzSbyField.Value = Globalspace._currentUser.UserName;
                }

                var pzSbyField = DyanmicPoundFieldItems
                    .FirstOrDefault(p => p.Field == nameof(Wrm.Pzsby));
                if (pzSbyField != null && string.IsNullOrWhiteSpace(pzSbyField.Value))
                {
                    pzSbyField.Value = Globalspace._currentUser.UserName;
                }

                ////先处理用户输入的车牌号和物资，因为它们是输入的，不是选择的，数据库里没有。
                ////此时为了保持上下文正常，需要再插入前先保存它们。
                //var chField = DyanmicPoundFieldItems
                //    .FirstOrDefault(p => p.Field == nameof(Wrm.Ch));
                //if (chField != null && !string.IsNullOrWhiteSpace(chField.Value))
                //{
                //    var ch = chField.Value;
                //    if (!db.Cars.Any(p => p.PlateNo == ch))
                //    {
                //        //新增车辆信息
                //        await new Common.EF.Tables.Car
                //        {
                //            PlateNo = ch,
                //        }.SaveAsync(db);
                //    }
                //}

                //此时为了保持上下文正常，需要再插入前先保存它们。
                var wzField = DyanmicPoundFieldItems
                    .FirstOrDefault(p => p.Field == nameof(Wrm.Wz));
                if (wzField != null && !string.IsNullOrWhiteSpace(wzField.Value))
                {
                    var wz = wzField.Value;
                    if (!db.Goods.Any(p => p.Name == wz))
                    {
                        //新增物资信息
                        await new Common.EF.Tables.Goods
                        {
                            Num = YitIdHelper.NextId().ToString(),
                            Name = wz,
                        }.SaveAsync(db);
                    }
                }

                var fromProps = Wrm.GetType().GetRuntimeProperties();

                foreach (var field in DyanmicPoundFieldItems)
                {
                    var prop = fromProps.FirstOrDefault(p => p.Name == field.Field);
                    if (prop == null)
                    {
                        continue;
                    }
                    if (prop.PropertyType == typeof(bool))
                    {
                        var fieldS = _fieldSource.FirstOrDefault(p => p.key == field.Field);
                        if (fieldS != null)
                        {
                            var boolVal = ((fieldS.children.FirstOrDefault(p => p.label == field.Value)?.value) as bool?).GetValueOrDefault();
                            prop.SetValue(Wrm, boolVal);
                        }
                        continue;
                    }
                    if (prop.PropertyType == typeof(decimal))
                    {
                        prop.SetValue(Wrm, field.Value.TryGetDecimal());
                        continue;
                    }
                    if (prop.PropertyType == typeof(DateTime)
                        || prop.PropertyType == typeof(DateTime?))
                    {
                        prop.SetValue(Wrm, field.Value.TryGetDateTime());
                        continue;
                    }

                    prop.SetValue(Wrm, field.Value);
                }

                var pz = Wrm.Pz;
                var jz = Wrm.Jz;
                var mz = Wrm.Mz;

                var kz = Wrm.Kz;
                var kl = Wrm.Kl;

                Wrm.IsFinish = jz > 0;

                VildOverLoad(Wrm);//超载验证处理

                Wrm.WeighingTimes = Wrm.IsFinish ? 2 : 1;
                Wrm.WeighName = _mainSettings.Settings["WeighName"].Value;
                // 默认实重 等于 净重
                Wrm.Sz = Wrm.Jz;

                if (Wrm.IsFinish)
                {
                    Wrm.Weigh2Name = _mainSettings.Settings["WeighName"].Value;
                    if (pz > 0)
                    {
                        if (pz > mz)
                        {
                            windowManager.ShowMessageBox("皮重不能大于毛重，请修改！");
                            return;
                        }
                        Wrm.Jz = mz - pz;
                        Wrm.Jzrq = DateTime.Now;
                    }

                    //判断扣重、扣率是否都填写了。如果都填写了。需要提示。
                    if (kz != 0 && kl != 0)
                    {
                        windowManager.ShowMessageBox("扣重、扣率只允许有一个，请处理！");
                        return;
                    }
                    else
                    {
                        //计算实重多少
                        if (kz != 0)
                        {
                            Wrm.Sz = jz - kz;
                        }
                        else if (kl != 0)
                        {
                            Wrm.Sz = jz - (jz) * (kl / 100);

                        }
                    }
                }

                if (Title == "新增称重记录")
                {
                    Wrm.EntryTime = DateTime.Now;
                    Wrm.IsPay = 0;
                    Wrm.SerialNumber = YitIdHelper.NextId().ToString();
                }

                if (Title == "修改称重记录")
                {
                    // 新增一个备份 --阿吉 2023年7月5日15点12分
                    Copy.By20 = "1";
                    Copy.Bh = $"X{Copy.Bh}";
                    Copy.EntryTime = DateTime.Now;
                    Copy.AutoNo = 0;

                    var dbCopy = AJAutoMapperService.Instance().Mapper.Map<Common.Models.WeighingRecord, Common.EF.Tables.WeighingRecord>(Copy);
                    try
                    {
                        await dbCopy.SaveAsync(db);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"原数据备份失败:{e.Message}", "提示", MessageBoxButton.OK);
                    }
                }

                var dbWrm = AJAutoMapperService.Instance().Mapper.Map<Common.Models.WeighingRecord, Common.EF.Tables.WeighingRecord>(Wrm);
                try
                {
                    await dbWrm.SaveAsync(db);
                    this.RequestClose(true);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"保存数据失败:{e.Message}", "提示", MessageBoxButton.OK);
                }

                if (Common.Platform.PlatformManager.Instance.Current is PlatformBase platform)
                {
                    ProcessResult apiRet;
                    if (_editMode)
                    {
                        apiRet = await platform.UpdateWeighRecordAsync(new PlatformBase.UploadWeighRecordParams
                        {
                            WeighRecord = Wrm
                        });
                    }
                    else
                    {
                        apiRet = await platform.UploadWeighRecordAsync(new PlatformBase.UploadWeighRecordParams
                        {
                            WeighRecord = Wrm
                        });
                    }

                    if (!apiRet.Success)
                    {
                        MessageBox.Show(apiRet.Message, "提示", MessageBoxButton.OK);
                    }
                }
            }
        }

        private void VildOverLoad(Common.Models.WeighingRecord weighingRecord)
        {
            var overloadLog = _mainSettings.Settings["OverloadLog"].Value;
            var OverloadWarning = _mainSettings.Settings["OverloadWarning"].Value;
            var OverloadWarningWeight = _mainSettings.Settings["OverloadWarningWeight"].TryGetDecimal();
            var wighingTimes = weighingRecord.WeighingTimes == 2 ? "Second" : "First";

            if (OverloadWarning == "1") //毛重超载报警
            {

                if (OverloadWarningWeight > 0)
                {
                    if (weighingRecord.Mz > OverloadWarningWeight)
                    {
                        weighingRecord.IsLimit = true;
                        weighingRecord.LimitedValue = OverloadWarningWeight;
                        weighingRecord.LimitType = "毛重超载";


                        //SQLDataAccess.SavOverloadLog(new OverloadLog()
                        //{
                        //    PlateNo = weighingRecord.Ch,
                        //    AxleCount = weighingRecord.AxleNum,
                        //    Constraints = "毛重超载",
                        //    OverloadWeight = WeightStr,
                        //    StandardWeight = maxWeight.ToString(),
                        //    Times = wighingTimes,
                        //    CreateDate = DateTime.Now
                        //}, overloadLog);
                    }
                }
            }
            else if (OverloadWarning == "2") //净重超载报警
            {
                if (weighingRecord.Jz > Convert.ToDecimal(OverloadWarningWeight))
                {
                    weighingRecord.IsLimit = true;
                    weighingRecord.LimitedValue = OverloadWarningWeight;
                    weighingRecord.LimitType = "净重超载";

                    //SQLDataAccess.SavOverloadLog(new OverloadLog()
                    //{
                    //    PlateNo = weighingRecord.Ch,
                    //    AxleCount = weighingRecord.AxleNum,
                    //    Constraints = "净重超载",
                    //    OverloadWeight = weighingRecord.Jz,
                    //    StandardWeight = OverloadWarningWeight,
                    //    Times = wighingTimes,
                    //    CreateDate = DateTime.Now
                    //}, overloadLog);
                }
            }
            else if (OverloadWarning == "3") //车轴计算超载报警
            {
                var apps = _mainSettings.Settings;
                var standard =
                     weighingRecord.AxleNum == 2 ? apps["OverloadAxle2"].Value.TryGetDecimal() :
                     weighingRecord.AxleNum == 3 ? apps["OverloadAxle3"].Value.TryGetDecimal() :
                     weighingRecord.AxleNum == 4 ? apps["OverloadAxle4"].Value.TryGetDecimal() :
                     weighingRecord.AxleNum == 5 ? apps["OverloadAxle5"].Value.TryGetDecimal() :
                     weighingRecord.AxleNum == 6 ? apps["OverloadAxle6"].Value.TryGetDecimal() : 0;

                if (weighingRecord.Mz > standard)
                {
                    weighingRecord.IsLimit = true;
                    weighingRecord.LimitedValue = standard;
                    weighingRecord.LimitType = "车轴计算超载";

                    //SQLDataAccess.SavOverloadLog(new OverloadLog()
                    //{
                    //    PlateNo = weighingRecord.Ch,
                    //    AxleCount = weighingRecord.AxleNum,
                    //    Constraints = "车轴计算超载",
                    //    OverloadWeight = WeightStr,
                    //    StandardWeight = standard,
                    //    Times = wighingTimes,
                    //    CreateDate = DateTime.Now
                    //}, overloadLog);
                }

            }
        }

        public void GenerateNow(string s)
        {
            var props = Wrm.GetType().GetRuntimeProperties();
            foreach (var field in DyanmicPoundFieldItems)
            {
                if (!field.Field.Contains("rq"))
                {
                    continue;
                }
                if (s == field.Field)
                {
                    var now = DateTime.Now;
                    field.Value = now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Title == "新增称重记录")
                    {
                        var task = Task.Run(() =>
                        {
                            return Common.Models.WeighingRecord.CreateBhAsync(now);
                        });
                        task.Wait();
                        props.FirstOrDefault(p => p.Name == nameof(Common.Models.WeighingRecord.Bh)).SetValue(Wrm, task.Result);
                    }
                }
            }

        }



    }


    public class WeighingRecordViewModelValidator : AbstractValidator<WeighingRecordViewModel>
    {
        public WeighingRecordViewModelValidator()
        {

        }
    }
}
