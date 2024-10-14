using AWSV2.Models;
using AWSV2.Services;
using FluentValidation;
using Stylet;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.IO.Ports;
using System.Text;
using Newtonsoft.Json;
using Aspose.Cells;
using System.Collections;
using System.Net.NetworkInformation;
using System.Net;
using System.Drawing;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using Common.Utility.AJ;
using Common.Model;
using AWSV2.Views;
using Common.Utility.AJ.MobileConfiguration;
using Common.Utility.AJ.MobileConfiguration.WeightSystem;
using Common.Utility.AJ.MobileConfiguration.Intelligence;
using Common.Model.ChargeInfo;
using Common.Utility.AJ.Extension;
using NPOI.SS.Formula.Functions;
using Common.HardwareSDKS.Models;
using Common.HardwareSDKS.IDCardReader;
using System.ComponentModel;
using Common.LPR;
using Common.Models;
using static Common.Utility.AJDatabaseService;
using Common.Utility;
using System.Windows.Data;
using System.Globalization;
using Common.HardwareSDKS.RS232Reader;
using static Common.Utility.AJ.CloudAPI;

namespace AWSV2.ViewModels
{
    public class SettingViewModel : Screen
    {
        private bool _enableSameCameraTriggerCloseGate;
        /// <summary>
        /// 称重完成IO2开闸, A相机识别后,不用B相机开闸,而是触发A相机关闸,来实现A相机同时控制两个道闸 --阿吉 2024年10月9日11点22分
        /// </summary>
        public bool EnableSameCameraTriggerCloseGate
        {
            get => _enableSameCameraTriggerCloseGate;
            set => SetAndNotify(ref _enableSameCameraTriggerCloseGate, value);
        }

        #region 激活码新增锁定硬件配置 --阿吉 2024年8月17日14点14分

        private CloudAPI.ActiveCode _activeCode;
        public CloudAPI.ActiveCode ActiveCode
        {
            get => _activeCode;
            set => SetAndNotify(ref _activeCode, value);
        }

        #endregion

        #region 新增RS232reader设备配置 --阿吉 2024年8月13日15点38分

        private RS232ReaderCfg _rs232ReaderCfg;
        public RS232ReaderCfg RS232ReaderCfg
        {
            get => _rs232ReaderCfg;
            set => SetAndNotify(ref _rs232ReaderCfg, value);
        }

        #endregion

        #region 新增扫码一体机设备 --阿吉 2024年4月30日11点12分

        public YunMengZhiHuiDeviceInfo YunMengZhiHuiDevice { get; set; }

        #endregion

        #region 新增西门子道闸串口配置 --阿吉 2024年5月13日08点42分

        public SIEMENSSerialPortCfg SIEMENSSerialPortCfg { get; set; }
        public Dictionary<string, Parity> ParitiesDropDown { get; set; }
        public Dictionary<string, StopBits> StopBitsDropDown { get; set; }

        #endregion

        #region 新增身份证读卡器配置 --阿吉 2024年5月12日17点51分

        public IDCardReaderCfg IDCardReaderCfg { get; set; }

        public Dictionary<int, string> USBDevices { get; set; }

        #endregion

        #region 新的配置调整 --阿吉 2023年11月7日09点24分

        private InstrumentCfg _instrument = new InstrumentCfg();
        private WeightingCfg _weightingCfg = new WeightingCfg();
        private PoundCfg _poundCfg = new PoundCfg();
        private PrintCfg _printCfg = new PrintCfg();

        #endregion

        //加载其他窗口
        private IWindowManager windowManager;
        #region 页面上的属性


        public IEnumerable<string> SerialPorts { get; set; }
        public IEnumerable<string> WZList { get; set; }

        public IEnumerable<string> BaudRates { get; set; }
        public IEnumerable<CustomDeviceConfig> ProtocolTypes { get; set; }
        public IEnumerable<string> AutoBackupFrequency { get; set; }
        public ObservableCollection<Common.AJControls.AJMultiSelect.AJMultiSelectOptionItem> PrinterList { get; set; }
        public Dictionary<string, DeviceType> DeviceTypes { get; set; }

        public IEnumerable<string> ChargeModles { get; set; }//闸道启用相机控制选择项
        public bool EnableWeighFormTemplate { get; set; } = false; //权限管理：允许修改称重表单
        public bool EnableBackup { get; set; } = false; //权限管理：允许进行备份还原

        public IEnumerable<string> GoodsList { get; set; }

        #region 各种属性

        private AJSystemParamsCfg _ajSystemParamsCfg;
        public AJSystemParamsCfg AJSystemParamsCfg
        {
            get
            {
                return _ajSystemParamsCfg;
            }
            set
            {
                SetAndNotify(ref _ajSystemParamsCfg, value);
            }
        }

        private bool _enableScanQRCodeToWeigh;
        /// <summary>
        /// 扫码上磅称重是否启用
        /// </summary>
        public bool EnableScanQRCodeToWeigh
        {
            get
            {
                return _enableScanQRCodeToWeigh;
            }
            set
            {
                SetAndNotify(ref _enableScanQRCodeToWeigh, value);
                _mainConfig.Settings[nameof(EnableScanQRCodeToWeigh)].Value = value.ToString();
            }
        }

        private bool _enableNotBookedCarWeigh;
        /// <summary>
        /// 未预约车辆称重是否启用
        /// </summary>
        public bool EnableNotBookedCarWeigh
        {
            get
            {
                return _enableNotBookedCarWeigh;
            }
            set
            {
                SetAndNotify(ref _enableNotBookedCarWeigh, value);
                _mainConfig.Settings[nameof(EnableNotBookedCarWeigh)].Value = value.ToString();
            }
        }

        private CleanupConfig _cleanupConfig;
        /// <summary>
        /// 数据清理配置
        /// </summary>
        public CleanupConfig CleanupConfig
        {
            get
            {
                return _cleanupConfig;
            }
            set
            {
                SetAndNotify(ref _cleanupConfig, value);
            }
        }

        private AJPrintConfig _aJPrintConfig;
        /// <summary>
        /// 打印配置 --阿吉 --2023年10月17日08点52分
        /// </summary>
        public AJPrintConfig AJPrintConfig
        {
            get
            {
                return _aJPrintConfig;
            }
            set
            {
                SetAndNotify(ref _aJPrintConfig, value);
            }
        }

        private TTSConfig _ttsConfig;
        /// <summary>
        /// TTS语音文本配置 --阿吉 -- 2024年5月31日17点18分
        /// </summary>
        public TTSConfig TTSConfig
        {
            get
            {
                return _ttsConfig;
            }
            set
            {
                SetAndNotify(ref _ttsConfig, value);
            }
        }

        private FastWeigthCorrectConfig _fastWeigthCorrectConfig;
        /// <summary>
        /// 称重修正功能配置 --阿吉 -- 2023年10月13日17点07分
        /// </summary>
        public FastWeigthCorrectConfig FastWeigthCorrectConfig
        {
            get
            {
                return _fastWeigthCorrectConfig;
            }
            set
            {
                SetAndNotify(ref _fastWeigthCorrectConfig, value);
            }
        }

        private SnapWatermarkConfig _snapWatermarkConfig;
        /// <summary>
        /// 过磅抓拍水印配置
        /// </summary>
        public SnapWatermarkConfig SnapWatermarkConfig
        {
            get
            {
                return _snapWatermarkConfig;
            }
            set
            {
                SetAndNotify(ref _snapWatermarkConfig, value);
            }
        }


        public string DownloadBarCodeTips { get; set; }

        public string GenerationTypeTips { get; set; }
        public IEnumerable<string> PrefixList { get; set; }
        public IEnumerable<string> GenerationTypes { get; set; } //磅单编号生成规则
        private string selectedPrefix;
        public string SelectedPrefix
        {
            get { return selectedPrefix; }
            set
            {
                selectedPrefix = value;
                _mainConfig.Settings["Prefix"].Value = value;

                var dateVal = DateTime.Now;
                if (!Enum.TryParse<WeighCodeCreateType>(SelectedGenerationType, out var type))
                {
                    type = WeighCodeCreateType.当日递增;
                }
                var format = type == WeighCodeCreateType.当日递增 ? "yyyyMMdd" : "yyyyMM";
                var sortNoFormat = type == WeighCodeCreateType.当日递增 ? "000" : "00000";
                var sortNo = 1;

                GenerationTypeTips = $"预览效果：{value}{dateVal.ToString(format)}{sortNo.ToString(sortNoFormat)}";
                Log("磅单编号前缀", value);
            }
        }

        private string selectedGenerationType;
        public string SelectedGenerationType
        {
            get { return selectedGenerationType; }
            set
            {
                selectedGenerationType = value;
                _mainConfig.Settings["GenerationType"].Value = value;

                var dateVal = DateTime.Now;
                if (!Enum.TryParse<WeighCodeCreateType>(SelectedGenerationType, out var type))
                {
                    type = WeighCodeCreateType.当日递增;
                }
                var format = type == WeighCodeCreateType.当日递增 ? "yyyyMMdd" : "yyyyMM";
                var sortNoFormat = type == WeighCodeCreateType.当日递增 ? "000" : "00000";
                var sortNo = 1;

                GenerationTypeTips = $"预览效果：{selectedPrefix}{dateVal.ToString(format)}{sortNo.ToString(sortNoFormat)}";

                Log("磅单编号生成规则", value);
            }
        }

        private string gblxXS;
        public string GblxXS
        {
            get { return gblxXS; }
            set
            {
                gblxXS = value;
                _mainConfig.Settings["GblxXS"].Value = value;
                Log("销售过磅", value);
            }
        }

        private string gblxCG;
        public string GblxCG
        {
            get { return gblxCG; }
            set
            {
                gblxCG = value;
                _mainConfig.Settings["GblxCG"].Value = value;
                Log("采购过磅", value);
            }
        }

        private string gblxQT;
        public string GblxQT
        {
            get { return gblxQT; }
            set
            {
                gblxQT = value;
                _mainConfig.Settings["GblxQT"].Value = value;
                Log("其他过磅", value);
            }
        }

        private string _companyName;
        public string CompanyName
        {
            get { return _companyName; }
            set
            {
                _companyName = value;
                _mainConfig.Settings["CompanyName"].Value = value;

            }
        }

        private string fhdw_mrz;
        public string Fhdw_mrz
        {
            get { return fhdw_mrz; }
            set
            {
                fhdw_mrz = value;
                _mainConfig.Settings["Fhdw_mrz"].Value = value;
                Log("发货单位", value);
            }
        }


        //----- DI状态 3-8 的设置值
        private string di3State;
        public string DI3State
        {
            get { return di3State; }
            set
            {
                di3State = value;
                _mainConfig.Settings["DI3State"].Value = value.Trim();
                Log("DI3状态", value);
            }
        }

        private string di4State;
        public string DI4State
        {
            get { return di4State; }
            set
            {
                di4State = value;
                _mainConfig.Settings["DI4State"].Value = value.Trim();
                Log("DI4状态", value);
            }
        }
        private string di5State;
        public string DI5State
        {
            get { return di5State; }
            set
            {
                di5State = value;
                _mainConfig.Settings["DI5State"].Value = value.Trim();
                Log("DI5状态", value);
            }
        }
        private string di6State;
        public string DI6State
        {
            get { return di6State; }
            set
            {
                di6State = value;
                _mainConfig.Settings["DI6State"].Value = value.Trim();
                Log("DI6状态", value);
            }
        }
        private string di7State;
        public string DI7State
        {
            get { return di7State; }
            set
            {
                di7State = value;
                _mainConfig.Settings["DI7State"].Value = value.Trim();
                Log("DI7状态", value);
            }
        }
        private string di8State;
        public string DI8State
        {
            get { return di8State; }
            set
            {
                di8State = value;
                _mainConfig.Settings["DI8State"].Value = value.Trim();
                Log("DI8状态", value);
            }
        }
        //----- 备用字段 6-20 的设置值

        private string byzd_Mess;
        public string Byzd_Mess
        {
            get { return byzd_Mess; }
            set
            {
                byzd_Mess = value;
            }
        }

        private string byzd_Key1;
        public string Byzd_Key1
        {
            get { return byzd_Key1; }
            set
            {
                byzd_Key1 = value;
                _mainConfig.Settings["Byzd_Key1"].Value = value;
                CheckByzd();
                Log("备用字段1-别名", value);
            }
        }

        private string byzd_Value1;
        public string Byzd_Value1
        {
            get { return byzd_Value1; }
            set
            {
                byzd_Value1 = value;
                _mainConfig.Settings["Byzd_Value1"].Value = value;
                Log("备用字段1-结果", value);
            }
        }

        private string byzd_Key2;
        public string Byzd_Key2
        {
            get { return byzd_Key2; }
            set
            {
                byzd_Key2 = value;
                _mainConfig.Settings["Byzd_Key2"].Value = value;
                CheckByzd();
                Log("备用字段2-别名", value);
            }
        }

        private string byzd_Value2;
        public string Byzd_Value2
        {
            get { return byzd_Value2; }
            set
            {
                byzd_Value2 = value;
                _mainConfig.Settings["Byzd_Value2"].Value = value;
                Log("备用字段1-结果", value);
            }
        }

        private string byzd_Key3;
        public string Byzd_Key3
        {
            get { return byzd_Key3; }
            set
            {
                byzd_Key3 = value;
                _mainConfig.Settings["Byzd_Key3"].Value = value;
                CheckByzd();
                Log("备用字段3-别名", value);
            }
        }

        private string byzd_Value3;
        public string Byzd_Value3
        {
            get { return byzd_Value3; }
            set
            {
                byzd_Value3 = value;
                _mainConfig.Settings["Byzd_Value3"].Value = value;
                Log("备用字段3-结果", value);
            }
        }

        private string byzd_Key4;
        public string Byzd_Key4
        {
            get { return byzd_Key4; }
            set
            {
                byzd_Key4 = value;
                _mainConfig.Settings["Byzd_Key4"].Value = value;
                CheckByzd();
                Log("备用字段4-别名", value);
            }
        }

        private string byzd_Value4;
        public string Byzd_Value4
        {
            get { return byzd_Value4; }
            set
            {
                byzd_Value4 = value;
                _mainConfig.Settings["Byzd_Value4"].Value = value;
                Log("备用字段4-结果", value);
            }
        }

        //-----
        private string weighName;
        public string WeighName
        {
            get { return weighName; }
            set
            {
                weighName = value;
                _mainConfig.Settings["WeighName"].Value = value;
                Log("地磅1名称", value);
            }
        }

        private string _effectiveWeight;
        public string EffectiveWeight
        {
            get { return _effectiveWeight; }
            set
            {
                _effectiveWeight = value;
                _mainConfig.Settings[nameof(EffectiveWeight)].Value = value.TryGetDecimal().ToString();
            }
        }

        private string weigh2Name;
        public string Weigh2Name
        {
            get { return weigh2Name; }
            set
            {
                weigh2Name = value;
                _mainConfig.Settings["Weigh2Name"].Value = value;
                Log("地磅2名称", value);
            }
        }

        private string weighProtocolType;      //称重仪表协议
        public string WeighProtocolType
        {
            get { return weighProtocolType; }
            set
            {
                weighProtocolType = value;

                _mainConfig.Settings["WeighProtocolType"].Value = value;
                Log("称重仪表协议", value);
            }
        }

        private string weighSerialPortName;      //称重仪表串口号
        public string WeighSerialPortName
        {
            get { return weighSerialPortName; }
            set
            {
                weighSerialPortName = value;

                _mainConfig.Settings["WeighSerialPortName"].Value = value;
                Log("称重仪表串口号", value);
            }
        }

        private string weighSerialPortBaudRate;      //称重仪表串口波特率
        public string WeighSerialPortBaudRate
        {
            get { return weighSerialPortBaudRate; }
            set
            {
                weighSerialPortBaudRate = value;

                _mainConfig.Settings["WeighSerialPortBaudRate"].Value = value;
                Log("称重仪表串口波特率", value);
            }
        }

        private bool enableSecondDevice;                  //是否开启副设备
        public bool EnableSecondDevice
        {
            get { return enableSecondDevice; }
            set
            {
                enableSecondDevice = value;

                _mainConfig.Settings["EnableSecondDevice"].Value = value.ToString();
                Log("开启副设备", value);
            }
        }

        private string weigh2ProtocolType;      //称重仪表2协议
        public string Weigh2ProtocolType
        {
            get { return weigh2ProtocolType; }
            set
            {
                weigh2ProtocolType = value;

                _mainConfig.Settings["Weigh2ProtocolType"].Value = value;
            }
        }

        private string weigh2SerialPortName;      //称重仪表2串口号
        public string Weigh2SerialPortName
        {
            get { return weigh2SerialPortName; }
            set
            {
                weigh2SerialPortName = value;

                _mainConfig.Settings["Weigh2SerialPortName"].Value = value;
            }
        }

        private string weigh2SerialPortBaudRate;      //称重仪表2串口波特率
        public string Weigh2SerialPortBaudRate
        {
            get { return weigh2SerialPortBaudRate; }
            set
            {
                weigh2SerialPortBaudRate = value;

                _mainConfig.Settings["Weigh2SerialPortBaudRate"].Value = value;
            }
        }

        private string weighFormDisplayMode;        //称重表单显示模式：List，Preview
        public string WeighFormDisplayMode
        {
            get { return weighFormDisplayMode; }
            set
            {
                weighFormDisplayMode = value;

                _mainConfig.Settings["WeighFormDisplayMode"].Value = value;
            }
        }


        private bool withPrinting;                  //称重后是否打印
        public bool WithPrinting
        {
            get { return withPrinting; }
            set
            {
                withPrinting = value;

                _mainConfig.Settings["WithPrinting"].Value = value.ToString();
                Log("称重后打印", value);
            }
        }

        private string printingMode;                  //打印模式，立即打印还是先预览再打印
        public string PrintingMode
        {
            get { return printingMode; }
            set
            {
                printingMode = value;
                _mainConfig.Settings["PrintingMode"].Value = value;

                Log("打印模式", value);
            }
        }

        private string printingType;                  //打印类别，一次过磅打印，二次过磅打印，一次二次都打印。。。。
        public string PrintingType
        {
            get { return printingType; }
            set
            {
                printingType = value;
                _mainConfig.Settings["PrintingType"].Value = value;
                Log("打印类别", value);
            }
        }


        private string printer;

        public string Printer
        {
            get { return printer; }
            set
            {
                printer = value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SelectedPrinter = value.Split(',').Select(p => new Common.AJControls.AJMultiSelect.AJMultiSelectOptionItem
                    {
                        Label = p,
                        Value = p
                    }).ToList();
                }
                else
                {
                    SelectedPrinter = new List<Common.AJControls.AJMultiSelect.AJMultiSelectOptionItem>();
                }

                Log("打印机", value);
            }
        }


        private string pageSizeHeight;              //页面高度（mm）
        public string PageSizeHeight
        {
            get { return pageSizeHeight; }
            set
            {
                pageSizeHeight = value;

                if (ValidateProperty("PageSizeHeight"))
                {
                    _mainConfig.Settings["PageSizeHeight"].Value = value;
                    Log("打印页面高度（mm）", value);
                }
            }
        }

        private string pageSizeWidth;               //页面宽度（mm）
        public string PageSizeWidth
        {
            get { return pageSizeWidth; }
            set
            {
                pageSizeWidth = value;

                if (ValidateProperty("PageSizeHeight"))
                {
                    _mainConfig.Settings["PageSizeWidth"].Value = value;
                    Log("打印页面宽度（mm）", value);
                }
            }
        }

        private string dyrqgs;               //打印日期格式
        public string Dyrqgs
        {
            get { return dyrqgs; }
            set
            {
                dyrqgs = value;

                if (ValidateProperty("Dyrqgs"))
                {
                    _mainConfig.Settings["Dyrqgs"].Value = value;
                    Log("打印日期格式", value);
                }
            }
        }

        private string mzrqgs;               //毛重日期格式
        public string Mzrqgs
        {
            get { return mzrqgs; }
            set
            {
                mzrqgs = value;

                if (ValidateProperty("Mzrqgs"))
                {
                    _mainConfig.Settings["Mzrqgs"].Value = value;
                    Log("毛重日期格式", value);
                }
            }
        }

        private string pzrqgs;               //皮重日期格式
        public string Pzrqgs
        {
            get { return pzrqgs; }
            set
            {
                pzrqgs = value;

                if (ValidateProperty("Pzrqgs"))
                {
                    _mainConfig.Settings["Pzrqgs"].Value = value;
                    Log("皮重日期格式", value);
                }
            }
        }

        private string jzrqgs;               //净重日期格式
        public string Jzrqgs
        {
            get { return jzrqgs; }
            set
            {
                jzrqgs = value;

                if (ValidateProperty("Jzrqgs"))
                {
                    _mainConfig.Settings["Jzrqgs"].Value = value;
                    Log("净重日期格式", value);
                }
            }
        }

        private List<Common.AJControls.AJMultiSelect.AJMultiSelectOptionItem> selectedPrinter;
        public List<Common.AJControls.AJMultiSelect.AJMultiSelectOptionItem> SelectedPrinter
        {
            get
            {
                return selectedPrinter;
            }
            set
            {
                selectedPrinter = value;
                if (value?.Count > 0)
                {
                    _mainConfig.Settings[nameof(Printer)].Value = string.Join(",", value.Select(p => p.Label));
                }
            }
        }

        private int _weightValueDisplayFormat;
        public int WeightValueDisplayFormat
        {
            get
            {
                return _weightValueDisplayFormat;
            }
            set
            {
                SetAndNotify(ref _weightValueDisplayFormat, value);
                _mainConfig.Settings[nameof(WeightValueDisplayFormat)].Value = value.ToString();
            }
        }

        private List<DropdownOption> _weightValueDisplayFormatOptions;
        public List<DropdownOption> WeightValueDisplayFormatOptions
        {
            get
            {
                return _weightValueDisplayFormatOptions;
            }
            set
            {
                SetAndNotify(ref _weightValueDisplayFormatOptions, value);
            }
        }

        private string weighingUnit;                //重量单位 kg / t
        public string WeighingUnit
        {
            get { return weighingUnit; }
            set
            {
                weighingUnit = value;

                _mainConfig.Settings["WeighingUnit"].Value = value;
                Log("重量单位", value);
            }
        }

        private string weighingMode;                //称重模式 Once / Twice
        public string WeighingMode
        {
            get { return weighingMode; }
            set
            {
                weighingMode = value;

                _mainConfig.Settings["WeighingMode"].Value = value;
                Log("称重模式", value);
            }
        }

        private string weighingControl;             //称重控制方式 Auto / Hand 
        public string WeighingControl
        {
            get { return weighingControl; }
            set
            {
                weighingControl = value;

                _mainConfig.Settings["WeighingControl"].Value = value;

                if (value == "Auto") EnableSecondDevice = false;
                if (value == "Btn")
                {
                    CheckGrating = 0;
                }
                //if (value == "Hand") 车牌识别 = "0";
                Log("称重控制方式", value);
            }
        }

        private string stableDelay;                 //自动称重模式的稳定延时
        public string StableDelay
        {
            get { return stableDelay; }
            set
            {
                stableDelay = value;

                if (ValidateProperty("StableDelay"))
                {
                    _mainConfig.Settings["StableDelay"].Value = value;
                    Log("自动称重模式的稳定延时长", value);
                }

            }
        }

        private string minSlotWeight;               //自动模式的最小称重重量
        public string MinSlotWeight
        {
            get { return minSlotWeight; }
            set
            {
                minSlotWeight = value;

                if (ValidateProperty("MinSlotWeight"))
                {
                    _mainConfig.Settings["MinSlotWeight"].Value = value;

                }
                //Common.SyncData.SetMinSlotWeight(value);
                Log("自动模式的最小称重重量", value);

            }
        }

        private bool tableRFEnable;             //启用明华桌面读卡器
        public bool TableRFEnable
        {
            get { return tableRFEnable; }
            set
            {
                tableRFEnable = value;
                _mainConfig.Settings["TableRFEnable"].Value = value.ToString();
                Log("启用明华桌面读卡器", value);
            }
        }

        private string tableRFPortName;              //明华桌面读卡器串口号
        public string TableRFPortName
        {
            get { return tableRFPortName; }
            set
            {
                tableRFPortName = value;
                _mainConfig.Settings["TableRFPortName"].Value = value;
                Log("明华桌面读卡器串口号", value);
            }
        }

        private bool qrEnable;             //启用二维码扫码器
        public bool QREnable
        {
            get { return qrEnable; }
            set
            {
                qrEnable = value;
                _mainConfig.Settings["QREnable"].Value = value.ToString();
                Log("启用二维码扫码器", value);
            }
        }

        private string qrPortName;              //二维码扫码器串口号
        public string QRPortName
        {
            get { return qrPortName; }
            set
            {
                qrPortName = value;
                _mainConfig.Settings["QRPortName"].Value = value;
                Log("二维码扫码器串口号", value);
            }
        }

        private bool qr2Enable;             //启用二维码扫码器2
        public bool QR2Enable
        {
            get { return qr2Enable; }
            set
            {
                qr2Enable = value;
                _mainConfig.Settings["QR2Enable"].Value = value.ToString();
                Log("启用二维码扫码器2", value);
            }
        }

        private string qrPort2Name;              //二维码扫码器2串口号
        public string QRPort2Name
        {
            get { return qrPort2Name; }
            set
            {
                qrPort2Name = value;
                _mainConfig.Settings["QRPort2Name"].Value = value;
                Log("二维码扫码器2串口号", value);
            }
        }

        private string discount;                    //扣重扣率 0：无 1：扣重 2：扣率
        private string discountWeight;
        private string discountRate;
        public string Discount
        {
            get { return discount; }
            set
            {
                discount = value;

                _mainConfig.Settings["Discount"].Value = value;
                Log("扣重扣率", value);
            }
        }
        public string DiscountWeight
        {
            get { return discountWeight; }
            set
            {
                discountWeight = value;

                if (ValidateProperty("DiscountWeight"))
                {
                    _mainConfig.Settings["DiscountWeight"].Value = value;
                }
                Log("扣重", value);
            }
        }
        public string DiscountRate
        {
            get { return discountRate; }
            set
            {
                discountRate = value;

                if (ValidateProperty("DiscountRate"))
                {
                    _mainConfig.Settings["DiscountRate"].Value = value;
                    Log("扣率", value);
                }
            }
        }

        private string lpr;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string LPR
        {
            get { return lpr; }
            set
            {
                lpr = value;

                _mainConfig.Settings["车牌识别"].Value = value;
                Log("车辆识别方式", value);
            }
        }

        private string barrier;

        public string Barrier
        {
            get { return barrier; }
            set
            {
                barrier = value;

                //以下这段逻辑判断为2023-04-18日新增
                if (barrier == "1")//单向道闸模式
                {
                    CameraLEDMode = "0";//LPR自带的LED的为单向模式
                }
                else
                {
                    // 不是单向模式, 则禁用  RelayType-3-相机控制红绿灯
                    if (RelayType == "3")
                    {
                        RelayType = ((int)Common.LPR.RelayType.通用控制器).ToString();
                    }
                }
                if (barrier == "2" || barrier == "3")//双向道闸模式
                {
                    CameraLEDMode = "1";//LPR自带的LED的为双向模式
                }
                //   新增逻辑结束
                // 3 混合双向 --阿吉 2023年12月14日15点44分
                _mainConfig.Settings["Barrier"].Value = value;

                Log("单双向道闸", value);
            }
        }

        private string lightType;

        public string LightType
        {
            get { return lightType; }
            set
            {
                lightType = value;
                _mainConfig.Settings["LightType"].Value = value;
                Log("红绿灯控制", value);
            }
        }

        private string openBarrierB;

        public string OpenBarrierB
        {
            get { return openBarrierB; }
            set
            {
                openBarrierB = value;

                _mainConfig.Settings["OpenBarrierB"].Value = value;
                Log("称重完成开后闸", value);
            }
        }

        private string lightSwitch;

        public string LightSwitch
        {
            get { return lightSwitch; }
            set
            {
                lightSwitch = value;

                _mainConfig.Settings["LightSwitch"].Value = value;

            }
        }

        private Dictionary<string, LEDType> _ledTypes;
        public Dictionary<string, LEDType> LEDTypes
        {
            get
            {
                return _ledTypes;
            }
            set
            {
                SetAndNotify(ref _ledTypes, value);
            }
        }

        private LEDType _ledType;
        public LEDType LEDType
        {
            get { return _ledType; }
            set
            {
                _ledType = value;
                _mainConfig.Settings[nameof(LEDType)].Value = value.ToString();
            }
        }

        private string ledxsnr;

        public string Ledxsnr
        {
            get { return ledxsnr; }
            set
            {
                ledxsnr = value;

                _mainConfig.Settings["Ledxsnr"].Value = value;
                Log("闲时内容", value);
            }
        }

        private string ledsbcps;

        public string Ledsbcps
        {
            get { return ledsbcps; }
            set
            {
                ledsbcps = value;

                _mainConfig.Settings["Ledsbcps"].Value = value;
                Log("忙时内容-识别到车牌时", value);
            }
        }

        private string ledzlwds;

        public string Ledzlwds
        {
            get { return ledzlwds; }
            set
            {
                ledzlwds = value;

                _mainConfig.Settings["Ledzlwds"].Value = value;
                Log("忙时内容-重量稳定时", value);
            }
        }

        private string leddyc;

        public string Leddyc
        {
            get { return leddyc; }
            set
            {
                leddyc = value;

                _mainConfig.Settings["Leddyc"].Value = value;
                Log("忙时内容-第一次称重完成时", value);
            }
        }

        private string leddec;

        public string Leddec
        {
            get { return leddec; }
            set
            {
                leddec = value;

                _mainConfig.Settings["Leddec"].Value = value;
                Log("忙时内容-第二次称重完成时", value);
            }
        }

        private bool monitorEnable;                         //启用监控摄像头
        public bool MonitorEnable
        {
            get { return monitorEnable; }
            set
            {
                monitorEnable = value;

                _mainConfig.Settings["MonitorEnable"].Value = value.ToString();
                Log("启用监控摄像头", value);
            }
        }

        private bool monitorCaptureEnable;                         //启用监控摄像头抓拍
        public bool MonitorCaptureEnable
        {
            get
            {

                CaptureIcon = monitorCaptureEnable ? Visibility.Visible : Visibility.Hidden;
                return monitorCaptureEnable;
            }
            set
            {
                monitorCaptureEnable = value;
                _mainConfig.Settings["MonitorCaptureEnable"].Value = value.ToString();
                CaptureIcon = value ? Visibility.Visible : Visibility.Hidden;
                Log("启用监控摄像头抓拍", value);
            }
        }

        public System.Windows.Visibility CaptureIcon { set; get; } = Visibility.Hidden;

        private bool syncDataEnable;                         //启用数据同步
        public bool SyncDataEnable
        {
            get { return syncDataEnable; }
            set
            {
                syncDataEnable = value;

                _mainConfig.Settings["SyncDataEnable"].Value = value.ToString();
                Log("启用数据同步", value);
            }
        }

        private bool syncYycz;                         //数据同步的方式 预约称重
        public bool SyncYycz
        {
            get { return syncYycz; }
            set
            {
                syncYycz = value;

                _mainConfig.Settings["SyncYycz"].Value = value.ToString();
                Log("预约称重", value);
            }

        }

        private bool syncYyzdsh;                         //数据同步的方式 预约自动审核
        public bool SyncYyzdsh
        {
            get { return syncYyzdsh; }
            set
            {
                syncYyzdsh = value;

                _mainConfig.Settings["SyncYyzdsh"].Value = value.ToString();
                Log("平台数据纠正", value);
            }
        }

        private bool enableVideo;                         //启用称重过磅
        public bool EnableVideo
        {
            get { return enableVideo; }
            set
            {
                enableVideo = value;

                _mainConfig.Settings["EnableVideo"].Value = value.ToString();
                Log("启用称重过磅", value);
            }
        }

        private bool ledEnable;                         //启用大屏幕
        public bool LEDEnable
        {
            get { return ledEnable; }
            set
            {
                ledEnable = value;

                _mainConfig.Settings["LEDEnable"].Value = value.ToString();
                Log("启用大屏幕1", value);
            }
        }

        private string ledPortName;               //大屏幕串口
        public string LEDPortName
        {
            get { return ledPortName; }
            set
            {
                ledPortName = value;

                _mainConfig.Settings["LEDPortName"].Value = value;
                Log("大屏幕串口", value);

            }
        }

        private bool led2Enable;                         //启用大屏幕
        public bool LED2Enable
        {
            get { return led2Enable; }
            set
            {
                led2Enable = value;

                _mainConfig.Settings["LED2Enable"].Value = value.ToString();
                Log("启用大屏幕2", value);
            }
        }

        private bool led3Enable;                         //启用大屏幕
        public bool LED3Enable
        {
            get { return led3Enable; }
            set
            {
                led3Enable = value;

                _mainConfig.Settings["LED3Enable"].Value = value.ToString();
                Log("启用大屏幕3", value);
            }
        }

        private double speechSpeed;                 //语音语速

        public double SpeechSpeed
        {
            get { return speechSpeed; }
            set
            {
                speechSpeed = value;

                _mainConfig.Settings["SpeechSpeed"].Value = value.ToString();
                Log("语音速度", value);
            }
        }


        private string overloadWarning;             //超载报警 0:无 1:毛重报警 2:净重报警
        public string OverloadWarning
        {
            get { return overloadWarning; }
            set
            {
                overloadWarning = value;

                _mainConfig.Settings["OverloadWarning"].Value = value;
                Log("超载报警", value);
            }
        }

        private string overloadAction;             //超载动作 0:开后闸 1:不开闸
        public string OverloadAction
        {
            get { return overloadAction; }
            set
            {
                overloadAction = value;

                _mainConfig.Settings["OverloadAction"].Value = value;
                Log("超载动作", value);
            }
        }

        private string overloadLog;             //超载动作 0:开后闸 1:不开闸
        public string OverloadLog
        {
            get { return overloadLog; }
            set
            {
                overloadLog = value;

                _mainConfig.Settings["OverloadLog"].Value = value;
                Log("超载存记录", value);
            }
        }

        private string overloadAxle2;
        public string OverloadAxle2
        {
            get { return overloadAxle2; }
            set
            {
                overloadAxle2 = value;
                _mainConfig.Settings["OverloadAxle2"].Value = value;
                Log("超载-两轴车", value);
            }
        }

        private string overloadAxle3;
        public string OverloadAxle3
        {
            get { return overloadAxle3; }
            set
            {
                overloadAxle3 = value;
                _mainConfig.Settings["OverloadAxle3"].Value = value;
                Log("超载-三轴车", value);
            }
        }

        private string overloadAxle4;
        public string OverloadAxle4
        {
            get { return overloadAxle4; }
            set
            {
                overloadAxle4 = value;
                _mainConfig.Settings["OverloadAxle4"].Value = value;
                Log("超载-四轴车", value);
            }
        }

        private string overloadAxle5;
        public string OverloadAxle5
        {
            get { return overloadAxle5; }
            set
            {
                overloadAxle5 = value;
                _mainConfig.Settings["OverloadAxle5"].Value = value;
                Log("超载-五轴车", value);
            }
        }

        private string overloadAxle6;
        public string OverloadAxle6
        {
            get { return overloadAxle6; }
            set
            {
                overloadAxle6 = value;
                _mainConfig.Settings["OverloadAxle6"].Value = value;
                Log("超载-六轴车", value);
            }
        }

        private string overloadWarningWeight;
        public string OverloadWarningWeight           //超载报警重量
        {
            get { return overloadWarningWeight; }
            set
            {
                overloadWarningWeight = value;

                if (ValidateProperty("OverloadWarningWeight"))
                {
                    _mainConfig.Settings["OverloadWarningWeight"].Value = value;
                }
                Log("超载-超载报警重量", value);
            }
        }

        private string overloadWarningText;
        public string OverloadWarningText           //超载报警语音
        {
            get { return overloadWarningText; }
            set
            {
                overloadWarningText = value;

                if (ValidateProperty("OverloadWarningText"))
                {
                    _mainConfig.Settings["OverloadWarningText"].Value = value;
                    Log("超载-超载报警语音", value);
                }
            }
        }

        private string backupPath;                  //备份文件路径
        public string BackupPath
        {
            get { return backupPath; }
            set
            {
                backupPath = value;

                if (ValidateProperty("BackupPath"))
                {
                    _mainConfig.Settings["BackupPath"].Value = value;
                    Log("备份文件路径", value);
                }
            }
        }

        private string monitorSavePath;
        public string MonitorSavePath
        {
            get { return monitorSavePath; }
            set
            {
                monitorSavePath = value;

                if (ValidateProperty("MonitorSavePath"))
                {
                    _mainConfig.Settings["MonitorSavePath"].Value = value;
                    Log("视频抓拍路径", value);
                }

            }
        }

        private string lPRSavePath;
        public string LPRSavePath
        {
            get { return lPRSavePath; }
            set
            {
                lPRSavePath = value;

                if (ValidateProperty("LPRSavePath"))
                {
                    _mainConfig.Settings["LPRSavePath"].Value = value;
                    Log("图片抓拍路径", value);
                }
            }
        }

        private DeviceType _lprCameraType;
        public DeviceType LPRCameraType
        {
            get
            {
                return _lprCameraType;
            }
            set
            {
                if (SetAndNotify(ref _lprCameraType, value))
                {
                    _mainConfig.Settings[nameof(LPRCameraType)].Value = value.ToString();
                    if (value == DeviceType.华夏)
                    {
                        Camera1Username
                            = Camera2Username
                            = Camera1Password
                            = Camera2Password = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 2022-11-25 新增。相机的控制器类型集合
        /// </summary>
        private List<CustomDeviceConfig> controllers;
        public List<CustomDeviceConfig> Controllers
        {
            get { return controllers; }
            set => SetAndNotify(ref controllers, value);
        }

        /// <summary>
        /// 2022-11-25 新增。相机的控制器类型集合
        /// </summary>
        private string selectedController;
        public string SelectedController
        {
            get { return selectedController; }
            set
            {
                selectedController = value;
                _mainConfig.Settings["SelectedController"].Value = value;
                Log("相机的控制器类型", value);
            }
        }

        private string currentAutoBackupFrequency;  //备份频率
        public string CurrentAutoBackupFrequency
        {
            get { return currentAutoBackupFrequency; }
            set
            {
                currentAutoBackupFrequency = value;

                _mainConfig.Settings["CurrentAutoBackupFrequency"].Value = value;
                Log("备份频率", value);
            }
        }

        private List<int> _rasterIOStateOptions;
        public List<int> RasterIOStateOptions
        {
            get => _rasterIOStateOptions;
            set => SetAndNotify(ref _rasterIOStateOptions, value);
        }

        private string camera1IP;
        public string Camera1IP
        {
            get { return camera1IP; }
            set
            {
                camera1IP = value;
                _mainConfig.Settings["Camera1IP"].Value = value;
                Log("相机1的IP", value);
            }
        }

        private string camera2IP;
        public string Camera2IP
        {
            get { return camera2IP; }
            set
            {
                camera2IP = value;
                _mainConfig.Settings["Camera2IP"].Value = value;
                Log("相机2的IP", value);
            }
        }



        private string camera1Username;
        public string Camera1Username
        {
            get { return camera1Username; }
            set
            {
                camera1Username = value;
                _mainConfig.Settings["Camera1Username"].Value = value;
                Log("相机1的用户名", value);
            }
        }


        private string camera2Username;
        public string Camera2Username
        {
            get { return camera2Username; }
            set
            {
                camera2Username = value;
                _mainConfig.Settings["Camera2Username"].Value = value;
                Log("相机2的用户名", value);
            }
        }


        private string camera1Password;
        public string Camera1Password
        {
            get { return camera1Password; }
            set
            {
                camera1Password = value;
                _mainConfig.Settings["Camera1Password"].Value = value;
                Log("相机1密码", value);
            }
        }

        private string camera2Password;
        public string Camera2Password
        {
            get { return camera2Password; }
            set
            {
                camera2Password = value;
                _mainConfig.Settings["Camera2Password"].Value = value;
                Log("相机2密码", value);
            }
        }

        private bool camera1Enable;
        public bool Camera1Enable
        {
            get { return camera1Enable; }
            set
            {
                camera1Enable = value;
                _mainConfig.Settings[nameof(Camera1Enable)].Value = (value ? 1 : 0).ToString();

            }
        }

        private bool _camera1DisableVideo;
        public bool Camera1DisableVideo
        {
            get { return _camera1DisableVideo; }
            set
            {
                _camera1DisableVideo = value;
                _mainConfig.Settings[nameof(Camera1DisableVideo)].Value = value.ToString();
            }
        }

        private int _camera1RasterCoveredIOState;
        public int Camera1RasterCoveredIOState
        {
            get { return _camera1RasterCoveredIOState; }
            set
            {
                _camera1RasterCoveredIOState = value;
                _mainConfig.Settings[nameof(Camera1RasterCoveredIOState)].Value = value.ToString();
            }
        }

        private bool camera2Enable;
        public bool Camera2Enable
        {
            get { return camera2Enable; }
            set
            {
                camera2Enable = value;
                _mainConfig.Settings[nameof(Camera2Enable)].Value = (value ? 1 : 0).ToString();

            }
        }

        private bool _camera2DisableVideo;
        public bool Camera2DisableVideo
        {
            get { return _camera2DisableVideo; }
            set
            {
                _camera2DisableVideo = value;
                _mainConfig.Settings[nameof(Camera2DisableVideo)].Value = value.ToString();
            }
        }

        private int _camera2RasterCoveredIOState;
        public int Camera2RasterCoveredIOState
        {
            get { return _camera2RasterCoveredIOState; }
            set
            {
                _camera2RasterCoveredIOState = value;
                _mainConfig.Settings[nameof(Camera2RasterCoveredIOState)].Value = value.ToString();
            }
        }

        private bool camera1LEDEnable;
        public bool Camera1LEDEnable
        {
            get { return Convert.ToBoolean(camera1LEDEnable); }
            set
            {
                camera1LEDEnable = Convert.ToBoolean(value);
                _mainConfig.Settings["Camera1LEDEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool camera2LEDEnable;
        public bool Camera2LEDEnable
        {
            get { return Convert.ToBoolean(camera2LEDEnable); }
            set
            {
                camera2LEDEnable = Convert.ToBoolean(value);
                _mainConfig.Settings["Camera2LEDEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool camera1GPIO;
        public bool Camera1GPIO
        {
            get { return Convert.ToBoolean(camera1GPIO); }
            set
            {
                camera1GPIO = Convert.ToBoolean(value);
                _mainConfig.Settings["Camera1GPIO"].Value = Convert.ToInt32(value).ToString();
                Log("自动称重", value);
            }
        }

        private bool rF1Enable;
        public bool RF1Enable
        {
            get { return Convert.ToBoolean(rF1Enable); }
            set
            {
                rF1Enable = Convert.ToBoolean(value);
                _mainConfig.Settings["RF1Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool rF2Enable;
        public bool RF2Enable
        {
            get { return Convert.ToBoolean(rF2Enable); }
            set
            {
                rF2Enable = Convert.ToBoolean(value);
                _mainConfig.Settings["RF2Enable"].Value = Convert.ToInt32(value).ToString();

            }
        }

        private string rF1Uart;
        public string RF1Uart
        {
            get { return rF1Uart; }
            set
            {
                rF1Uart = value;
                _mainConfig.Settings["RF1Uart"].Value = value;
            }
        }

        private string rF2Uart;
        public string RF2Uart
        {
            get { return rF2Uart; }
            set
            {
                rF2Uart = value;
                _mainConfig.Settings["RF2Uart"].Value = value;
            }
        }

        private bool _rf3Enable;
        public bool RF3Enable
        {
            get { return _rf3Enable; }
            set
            {
                _rf3Enable = value;
                _mainConfig.Settings[nameof(RF3Enable)].Value = (value ? 1 : 0).ToString();

            }
        }

        private string rF3Ip;
        public string RF3Ip
        {
            get { return rF3Ip; }
            set
            {
                rF3Ip = value;
                _mainConfig.Settings["RF3IP"].Value = value;
            }
        }


        private bool _rf4Enable;
        public bool RF4Enable
        {
            get { return _rf4Enable; }
            set
            {
                _rf4Enable = value;
                _mainConfig.Settings[nameof(RF4Enable)].Value = (value ? 1 : 0).ToString();

            }
        }

        private string rF4Ip;
        public string RF4Ip
        {
            get { return rF4Ip; }
            set
            {
                rF4Ip = value;
                _mainConfig.Settings["RF4IP"].Value = value;
            }
        }

        /*private string rF1Power;
        public string RF1Power
        {
            get { return rF1Power; }
            set
            {
                rF1Power = value;
                LPRSection.Settings["RF1Power"].Value = value;
            }
        }

        private string rF2Power;
        public string RF2Power
        {
            get { return rF2Power; }
            set
            {
                rF2Power = value;
                LPRSection.Settings["RF2Power"].Value = value;
            }
        }
        */

        private bool relayEnable;
        public bool RelayEnable
        {
            get { return Convert.ToBoolean(relayEnable); }
            set
            {
                relayEnable = Convert.ToBoolean(value);
                _mainConfig.Settings["RelayEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool relay2Enable;
        public bool Relay2Enable
        {
            get { return Convert.ToBoolean(relay2Enable); }
            set
            {
                relay2Enable = Convert.ToBoolean(value);
                _mainConfig.Settings["Relay2Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private string relayType = "0";
        /// <summary>
        /// 0 通用控制器, 2 相机开闸, 3 相机控制红绿灯, 4 读头开闸, 5 读头控制红绿灯
        /// </summary>
        public string RelayType
        {
            get
            {
                return relayType;
            }
            set
            {
                relayType = value;

                var intVal = value.TryGetInt();

                RelayEnable = intVal == 0;
                Relay2Enable = intVal == 1;
                // 相机开闸
                Plate2Enable = intVal == 2 ? 1 : 0;

                _mainConfig.Settings[nameof(RelayType)].Value = value;
            }
        }

        private string relayUart;
        public string RelayUart
        {
            get { return relayUart; }
            set
            {
                relayUart = value;
                _mainConfig.Settings["RelayUart"].Value = value;
            }
        }

        private string cameraLEDMode;
        public string CameraLEDMode
        {
            get { return cameraLEDMode; }
            set
            {
                cameraLEDMode = value;
                _mainConfig.Settings["CameraLEDMode"].Value = value;
            }
        }

        private int plate2Enable;
        public int Plate2Enable
        {
            get { return plate2Enable; }
            set
            {
                plate2Enable = value;
                _mainConfig.Settings["Plate2Enable"].Value = value.ToString();
            }
        }

        private bool monitor1Enable;
        public bool Monitor1Enable
        {
            get { return Convert.ToBoolean(monitor1Enable); }
            set
            {
                monitor1Enable = Convert.ToBoolean(value);
                _mainConfig.Settings["Monitor1Enable"].Value = Convert.ToInt32(value).ToString();
                Log("影像抓拍-启用相机1", value);
            }
        }

        private bool monitor2Enable;
        public bool Monitor2Enable
        {
            get { return Convert.ToBoolean(monitor2Enable); }
            set
            {
                monitor2Enable = Convert.ToBoolean(value);
                _mainConfig.Settings["Monitor2Enable"].Value = Convert.ToInt32(value).ToString();
                Log("影像抓拍-启用相机2", value);
            }
        }

        private bool monitor3Enable;
        public bool Monitor3Enable
        {
            get { return Convert.ToBoolean(monitor3Enable); }
            set
            {
                monitor3Enable = Convert.ToBoolean(value);
                _mainConfig.Settings["Monitor3Enable"].Value = Convert.ToInt32(value).ToString();
                Log("影像抓拍-启用相机3", value);
            }
        }


        private bool monitor4Enable;
        public bool Monitor4Enable
        {
            get { return Convert.ToBoolean(monitor4Enable); }
            set
            {
                monitor4Enable = Convert.ToBoolean(value);
                _mainConfig.Settings["Monitor4Enable"].Value = Convert.ToInt32(value).ToString();
                Log("影像抓拍-启用相机4", value);
            }
        }

        private string monitor1IP;
        public string Monitor1IP
        {
            get { return monitor1IP; }
            set
            {
                monitor1IP = value;
                _mainConfig.Settings["Monitor1IP"].Value = value;
                Log("影像抓拍-相机1的IP", value);
            }
        }

        private string monitor2IP;
        public string Monitor2IP
        {
            get { return monitor2IP; }
            set
            {
                monitor2IP = value;
                _mainConfig.Settings["Monitor2IP"].Value = value;
                Log("影像抓拍-相机2的IP", value);
            }
        }

        private string monitor3IP;
        public string Monitor3IP
        {
            get { return monitor3IP; }
            set
            {
                monitor3IP = value;
                _mainConfig.Settings["Monitor3IP"].Value = value;
                Log("影像抓拍-相机3的IP", value);
            }
        }


        private string monitor4IP;
        public string Monitor4IP
        {
            get { return monitor4IP; }
            set
            {
                monitor4IP = value;
                _mainConfig.Settings["Monitor4IP"].Value = value;
                Log("影像抓拍-相机4的IP", value);
            }
        }


        private string monitor1Username;
        public string Monitor1Username
        {
            get { return monitor1Username; }
            set
            {
                monitor1Username = value;
                _mainConfig.Settings["Monitor1Username"].Value = value;
                Log("影像抓拍-相机1的用户名", value);
            }
        }


        private string monitor2Username;
        public string Monitor2Username
        {
            get { return monitor2Username; }
            set
            {
                monitor2Username = value;
                _mainConfig.Settings["Monitor2Username"].Value = value;
                Log("影像抓拍-相机2的用户名", value);
            }
        }


        private string monitor3Username;
        public string Monitor3Username
        {
            get { return monitor3Username; }
            set
            {
                monitor3Username = value;
                _mainConfig.Settings["Monitor3Username"].Value = value;
                Log("影像抓拍-相机3的用户名", value);
            }
        }

        private string monitor4Username;
        public string Monitor4Username
        {
            get { return monitor4Username; }
            set
            {
                monitor4Username = value;
                _mainConfig.Settings["Monitor4Username"].Value = value;
                Log("影像抓拍-相机4的用户名", value);
            }
        }

        private string monitor1Password;
        public string Monitor1Password
        {
            get { return monitor1Password; }
            set
            {
                monitor1Password = value;
                _mainConfig.Settings["Monitor1Password"].Value = value;
                Log("影像抓拍-相机1的密码", value);
            }
        }


        private string monitor2Password;
        public string Monitor2Password
        {
            get { return monitor2Password; }
            set
            {
                monitor2Password = value;
                _mainConfig.Settings["Monitor2Password"].Value = value;
                Log("影像抓拍-相机2的密码", value);
            }
        }


        private string monitor3Password;
        public string Monitor3Password
        {
            get { return monitor3Password; }
            set
            {
                monitor3Password = value;
                _mainConfig.Settings["Monitor3Password"].Value = value;
                Log("影像抓拍-相机3的密码", value);
            }
        }

        private string monitor4Password;
        public string Monitor4Password
        {
            get { return monitor4Password; }
            set
            {
                monitor4Password = value;
                _mainConfig.Settings["Monitor4Password"].Value = value;
                Log("影像抓拍-相机4的密码", value);
            }
        }


        private string _monitor1RTSPUrl;
        public string Monitor1RTSPUrl
        {
            get { return _monitor1RTSPUrl; }
            set
            {
                _monitor1RTSPUrl = value;
                _mainConfig.Settings[nameof(Monitor1RTSPUrl)].Value = value;
            }
        }


        private string _monitor2RTSPUrl;
        public string Monitor2RTSPUrl
        {
            get { return _monitor2RTSPUrl; }
            set
            {
                _monitor2RTSPUrl = value;
                _mainConfig.Settings[nameof(Monitor2RTSPUrl)].Value = value;
            }
        }


        private string _monitor3RTSPUrl;
        public string Monitor3RTSPUrl
        {
            get { return _monitor3RTSPUrl; }
            set
            {
                _monitor3RTSPUrl = value;
                _mainConfig.Settings[nameof(Monitor3RTSPUrl)].Value = value;
            }
        }

        private string _monitor4RTSPUrl;
        public string Monitor4RTSPUrl
        {
            get { return _monitor4RTSPUrl; }
            set
            {
                _monitor4RTSPUrl = value;
                _mainConfig.Settings[nameof(Monitor4RTSPUrl)].Value = value;
            }
        }


        private string lEDIP;
        public string LEDIP
        {
            get { return lEDIP; }
            set
            {
                lEDIP = value;
                _mainConfig.Settings["LEDIP"].Value = value;

            }
        }

        private string _ledFontSize;
        public string LEDFontSize
        {
            get { return _ledFontSize; }
            set
            {
                _ledFontSize = value;
                _mainConfig.Settings[nameof(LEDFontSize)].Value = value;

            }
        }

        private string _led1FontSize;
        public string LED1FontSize
        {
            get { return _led1FontSize; }
            set
            {
                _led1FontSize = value;
                _mainConfig.Settings[nameof(LED1FontSize)].Value = value;

            }
        }

        private string _ledPlaySpeed;
        public string LEDPlaySpeed
        {
            get { return _ledPlaySpeed; }
            set
            {
                _ledPlaySpeed = value;
                _mainConfig.Settings[nameof(LEDPlaySpeed)].Value = value;

            }
        }

        private string _led1PlaySpeed;
        public string LED1PlaySpeed
        {
            get { return _led1PlaySpeed; }
            set
            {
                _led1PlaySpeed = value;
                _mainConfig.Settings[nameof(LED1PlaySpeed)].Value = value;

            }
        }

        private string lED1IP;
        public string LED1IP
        {
            get { return lED1IP; }
            set
            {
                lED1IP = value;
                _mainConfig.Settings["LED1IP"].Value = value;

            }
        }

        private string lEDWIDTH;
        public string LEDWIDTH
        {
            get { return lEDWIDTH; }
            set
            {
                lEDWIDTH = value;
                _mainConfig.Settings["LEDWIDTH"].Value = value;

            }
        }

        private string lED1WIDTH;
        public string LED1WIDTH
        {
            get { return lED1WIDTH; }
            set
            {
                lED1WIDTH = value;
                _mainConfig.Settings["LED1WIDTH"].Value = value;
                Log("室外大屏-设备2宽度", value);
            }
        }

        private string lEDHEIGHT;
        public string LEDHEIGHT
        {
            get { return lEDHEIGHT; }
            set
            {
                lEDHEIGHT = value;
                _mainConfig.Settings["LEDHEIGHT"].Value = value;
                Log("室外大屏-设备1高度", value);
            }
        }

        private string lED1HEIGHT;
        public string LED1HEIGHT
        {
            get { return lED1HEIGHT; }
            set
            {
                lED1HEIGHT = value;
                _mainConfig.Settings["LED1HEIGHT"].Value = value;
                Log("室外大屏-设备2高度", value);
            }
        }

        //--------以下属性为收费程序的参数配置--------------
        private bool chargeEnable;
        public bool ChargeEnable
        {
            get { return Convert.ToBoolean(chargeEnable); }
            set
            {
                chargeEnable = Convert.ToBoolean(value);
                _mainConfig.Settings["ChargeEnable"].Value = Convert.ToInt32(value).ToString();
                Log("过磅收费-启用收费功能", value);
            }
        }

        private bool multipleChargeEnable;
        public bool MultipleChargeEnable
        {
            get { return Convert.ToBoolean(multipleChargeEnable); }
            set
            {
                multipleChargeEnable = Convert.ToBoolean(value);
                _mainConfig.Settings["MultipleChargeEnable"].Value = Convert.ToInt32(value).ToString();
                Log("过磅收费-启用两次收费模式", value);
            }
        }

        private bool eChargeEnable;
        public bool EChargeEnable
        {
            get { return Convert.ToBoolean(eChargeEnable); }
            set
            {
                eChargeEnable = Convert.ToBoolean(value);
                _mainConfig.Settings["EChargeEnable"].Value = Convert.ToInt32(value).ToString();
                Log("过磅收费-启用电子支付", value);
            }
        }

        private string parkingID;
        public string ParkingID
        {
            get { return parkingID; }
            set
            {
                parkingID = value;
                _mainConfig.Settings["ParkingID"].Value = value;
                Log("过磅收费-电子支付配置-商户号", value);
            }
        }

        private string pServerPath;
        public string PServerPath
        {
            get { return pServerPath; }
            set
            {
                pServerPath = value;
                _mainConfig.Settings["PServerPath"].Value = value;
                Log("过磅收费-电子支付配置-服务号", value);
            }
        }

        private string uuid;
        public string UUID
        {
            get { return uuid; }
            set
            {
                uuid = value;
                _mainConfig.Settings["UUID"].Value = value;
                Log("过磅收费-电子支付配置-UUID", value);
            }
        }

        private string mac;
        public string MAC
        {
            get { return mac; }
            set
            {
                mac = value;
                _mainConfig.Settings["MAC"].Value = value;
                Log("过磅收费-电子支付配置-MAC", value);
            }
        }

        private bool chargeByMaterial;
        public bool ChargeByMaterial
        {
            get { return Convert.ToBoolean(chargeByMaterial); }
            set
            {
                chargeByMaterial = Convert.ToBoolean(value);
                _mainConfig.Settings["ChargeByMaterial"].Value = Convert.ToInt32(value).ToString();
                Log("过磅收费-按物资编号收费", value);
            }
        }

        private string chargeType;
        /// <summary>
        /// 收费类型 1 按单位收费, 2 按范围收费, 3 按单价收费
        /// </summary>
        public string ChargeType
        {
            get { return chargeType; }
            set
            {
                chargeType = value;
                _mainConfig.Settings["ChargeType"].Value = value;
                Log("过磅收费-按单位收费", value);
            }
        }

        private string chargeWay;
        public string ChargeWay
        {
            get { return chargeWay; }
            set
            {
                chargeWay = value;
                _mainConfig.Settings["ChargeWay"].Value = value;
                Log("过磅收费-缴费后下磅", value);
            }
        }

        private string lowestFees;
        public string LowestFees
        {
            get { return lowestFees; }
            set
            {
                lowestFees = value;
                _mainConfig.Settings["LowestFees"].Value = value;
                Log("过磅收费-保底收费", value);
            }
        }

        private string chargeTypes;
        public string ChargeTypes
        {
            get { return chargeTypes; }
            set
            {
                chargeTypes = value;
                _mainConfig.Settings["ChargeTypes"].Value = value;
                Log("过磅收费-收费模式", value);
            }
        }

        private bool chargeStorage;
        public bool ChargeStorage
        {
            get { return chargeStorage; }
            set
            {
                chargeStorage = value;
                _mainConfig.Settings["ChargeStorage"].Value = Convert.ToInt32(value).ToString();
                //Log("电子围栏-超出授信车辆禁止出场", value);
            }
        }





        #region 电子围栏
        //----------以下为电子围栏的参数------------

        private string virtualWall;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VirtualWall
        {
            get { return virtualWall; }
            set
            {
                virtualWall = value;
                _virtualWallSection.Settings["VirtualWall"].Value = value;
                //Log("电子围栏-是否启用电子围栏", value);
            }
        }

        private string vWCamera1IP;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera1IP
        {
            get { return vWCamera1IP; }
            set
            {
                vWCamera1IP = value;
                _virtualWallSection.Settings["VWCamera1IP"].Value = value;
                //Log("电子围栏-进口1相机", value);
            }
        }

        private string vWCamera1Username;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera1Username
        {
            get { return vWCamera1Username; }
            set
            {
                vWCamera1Username = value;
                _virtualWallSection.Settings["VWCamera1Username"].Value = value;
                //Log("电子围栏-进口1相机用户名", value);
            }
        }

        private string vWCamera1Password;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera1Password
        {
            get { return vWCamera1Password; }
            set
            {
                vWCamera1Password = value;
                _virtualWallSection.Settings["VWCamera1Password"].Value = value;
                //Log("电子围栏-进口1相机密码", value);
            }
        }

        private string vWCamera2IP;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera2IP
        {
            get { return vWCamera2IP; }
            set
            {
                vWCamera2IP = value;
                _virtualWallSection.Settings["VWCamera2IP"].Value = value;
                //Log("电子围栏-进口2相机", value);
            }
        }

        private string vWCamera2Username;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera2Username
        {
            get { return vWCamera2Username; }
            set
            {
                vWCamera2Username = value;
                _virtualWallSection.Settings["VWCamera2Username"].Value = value;
                //Log("电子围栏-进口2相机用户名", value);
            }
        }

        private string vWCamera2Password;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera2Password
        {
            get { return vWCamera2Password; }
            set
            {
                vWCamera2Password = value;
                _virtualWallSection.Settings["VWCamera2Password"].Value = value;
                //Log("电子围栏-进口2相机密码", value);
            }
        }

        private string vWCamera3IP;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera3IP
        {
            get { return vWCamera3IP; }
            set
            {
                vWCamera3IP = value;
                _virtualWallSection.Settings["VWCamera3IP"].Value = value;
                //Log("电子围栏-进口3相机", value);
            }
        }

        private string vWCamera3Username;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera3Username
        {
            get { return vWCamera3Username; }
            set
            {
                vWCamera3Username = value;
                _virtualWallSection.Settings["VWCamera3Username"].Value = value;
                //Log("电子围栏-进口3相机用户名", value);
            }
        }

        private string vWCamera3Password;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera3Password
        {
            get { return vWCamera3Password; }
            set
            {
                vWCamera3Password = value;
                _virtualWallSection.Settings["VWCamera3Password"].Value = value;
                Log("电子围栏-进口3相机密码", value);
            }
        }

        private string vWCamera4IP;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera4IP
        {
            get { return vWCamera4IP; }
            set
            {
                vWCamera4IP = value;
                _virtualWallSection.Settings["VWCamera4IP"].Value = value;
                Log("电子围栏-进口4相机", value);
            }
        }

        private string vWCamera4Username;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera4Username
        {
            get { return vWCamera4Username; }
            set
            {
                vWCamera4Username = value;
                _virtualWallSection.Settings["VWCamera4Username"].Value = value;
                Log("电子围栏-进口4相机用户名", value);
            }
        }

        private string vWCamera4Password;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera4Password
        {
            get { return vWCamera4Password; }
            set
            {
                vWCamera4Password = value;
                _virtualWallSection.Settings["VWCamera4Password"].Value = value;
                Log("电子围栏-进口4相机密码", value);
            }
        }

        private string vWRF3Ip;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWRF3Ip
        {
            get { return vWRF3Ip; }
            set
            {
                vWRF3Ip = value;
                _virtualWallSection.Settings["VWRF3Ip"].Value = value;
                Log("电子围栏-高频读头-进口", value);
            }
        }

        private string vWRF4Ip;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWRF4Ip
        {
            get { return vWRF4Ip; }
            set
            {
                vWRF4Ip = value;
                _virtualWallSection.Settings["VWRF4Ip"].Value = value;
                Log("电子围栏-高频读头-出口", value);
            }
        }

        private bool enableWeighFinish;
        public bool EnableWeighFinish
        {
            get { return Convert.ToBoolean(enableWeighFinish); }
            set
            {
                enableWeighFinish = Convert.ToBoolean(value);
                _virtualWallSection.Settings["EnableWeighFinish"].Value = Convert.ToInt32(value).ToString();
                Log("电子围栏-功能设置-未称重完成禁止出场", value);
            }
        }

        private bool vWWhiteListeEnable;
        public bool VWWhiteListeEnable
        {
            get { return Convert.ToBoolean(vWWhiteListeEnable); }
            set
            {
                vWWhiteListeEnable = Convert.ToBoolean(value);
                _virtualWallSection.Settings["VWWhiteListeEnable"].Value = Convert.ToInt32(value).ToString();
                Log("电子围栏-功能设置-临时车禁止入场", value);
            }
        }

        private bool vWStorageOverflow;
        public bool VWStorageOverflow
        {
            get { return vWStorageOverflow; }
            set
            {
                vWStorageOverflow = value;
                _virtualWallSection.Settings["VWStorageOverflow"].Value = Convert.ToInt32(value).ToString();
                Log("电子围栏-功能设置-超出授信车辆禁止出场", value);
            }
        }
        private bool vWOpenBarrier;
        public bool VWOpenBarrier
        {
            get { return vWOpenBarrier; }
            set
            {
                vWOpenBarrier = value;
                _virtualWallSection.Settings["VWOpenBarrier"].Value = Convert.ToInt32(value).ToString();
                Log("电子围栏-功能设置-控制开闸", value);
            }
        }
        #endregion

        #region 称重模式设置

        private int checkGrating;
        public int CheckGrating
        {
            get { return checkGrating; }
            set
            {
                checkGrating = value;
                _mainConfig.Settings["CheckGrating"].Value = value.ToString();
                Log("称重设置-功能设置-光栅防作弊", value);
            }
        }

        private int lPRWhiteList;
        public int LPRWhiteList
        {
            get { return lPRWhiteList; }
            set
            {
                lPRWhiteList = value;
                _mainConfig.Settings["LPRWhiteList"].Value = value.ToString();
                Log("称重设置-功能设置-白名单功能", value);
            }
        }

        private int autoWeighingType;
        public int AutoWeighingType
        {
            get { return autoWeighingType; }
            set
            {
                autoWeighingType = value;
                _mainConfig.Settings["AutoWeighingType"].Value = value.ToString();
                Log("称重设置-功能设置-自动过磅类型", value);
            }
        }

        private int timelyRefresh;
        public int TimelyRefresh
        {
            get { return timelyRefresh; }
            set
            {
                timelyRefresh = value;
                _mainConfig.Settings["TimelyRefresh"].Value = value.ToString();
                Log("称重设置-功能设置-立即刷新磅单", value);
            }
        }

        private bool _manualPZOrMZ;
        /// <summary>
        ///  一次过磅人工判断皮毛 --阿吉 2023年10月6日15点50分
        /// </summary>
        public bool ManualPZOrMZ
        {
            get { return _manualPZOrMZ; }
            set
            {
                _manualPZOrMZ = value;
                _mainConfig.Settings["ManualPZOrMZ"].Value = value.ToString();
            }
        }

        private bool _autoConvertUnit;
        public bool AutoConvertUnit
        {
            get { return _autoConvertUnit; }
            set
            {
                _autoConvertUnit = value;
                _mainConfig.Settings["AutoConvertUnit"].Value = value.ToString();
                Log("称重设置-功能设置-立即刷新磅单", value);
            }
        }

        private string cancelWeighingTime;
        public string CancelWeighingTime
        {
            get { return cancelWeighingTime; }
            set
            {
                cancelWeighingTime = value;
                _mainConfig.Settings["CancelWeighingTime"].Value = value;
                Log("称重设置-功能设置-超时复位(秒)", value);
            }
        }

        private string lastPlateTimer;
        public string LastPlateTimer
        {
            get { return lastPlateTimer; }
            set
            {
                lastPlateTimer = value;
                _mainConfig.Settings["LastPlateTimer"].Value = value;
                Log("称重设置-功能设置-重复过磅间隔(秒)", value);
            }
        }
        private string weighingValidTime;
        public string WeighingValidTime
        {
            get { return weighingValidTime; }
            set
            {
                weighingValidTime = value;
                _mainConfig.Settings["WeighingValidTime"].Value = value;
                Log("称重设置-功能设置-一次过磅有效时间", value);
            }
        }

        private string pageCount;
        public string PageCount
        {
            get { return pageCount; }
            set
            {
                pageCount = value;
                _mainConfig.Settings["PageCount"].Value = value;
                Log("打印设置-功能设置-打印份数", value);
            }
        }


        private bool pzbjzt;
        public bool Pzbjzt
        {
            get { return Convert.ToBoolean(pzbjzt); }
            set
            {
                pzbjzt = Convert.ToBoolean(value);
                _syncSection.Settings["Pzbjzt"].Value = value.ToString();
                Log("数据同步-皮重报警公众号通知", value);
                //Common.SyncData.set_alarm_status(value);
            }
        }
        private string serverPath;
        public string ServerPath
        {
            get { return serverPath; }
            set
            {
                serverPath = value;
                _syncSection.Settings["ServerPath"].Value = value;
                Log("数据同步-服务器地址", value);
            }
        }
        private string pzbjfz;
        public string Pzbjfz
        {
            get { return pzbjfz; }
            set
            {
                pzbjfz = value;
                _syncSection.Settings["Pzbjfz"].Value = value;
                Log("数据同步-报警值±", value);
                //Common.SyncData.set_pz_weight_alarm_threshold(value);
            }
        }

        public object WeighFormVM { get; set; } //过磅单表单
        public List<string> WeighFromTemplateSheetsName { get; private set; } = new List<string>();
        public string SelectedWeighFromTemplateSheet { get; set; }
        #endregion


        #endregion

        private string barCodeUrl;
        public string BarCodeUrl
        {
            get { return barCodeUrl; }
            set
            {
                barCodeUrl = value;
            }
        }

        #endregion
        public string ProvicerTitle
        {
            get
            {
                return AWSV2.Globalspace.ProvicerTitle;
            }
        }

        private bool _jieGuanZha;  //是否有接关闸 --阿吉 2023年8月3日09点24分
        public bool JieGuanZha
        {
            get { return _jieGuanZha; }
            set
            {
                _jieGuanZha = value;

                _mainConfig.Settings["JieGuanZha"].Value = value.ToString();
            }
        }

        protected AppSettingsSection _mainConfig;

        protected AppSettingsSection _virtualWallSection;

        protected AppSettingsSection _syncSection;

        private Visibility _hiddenInStandardVersion;
        public Visibility HiddenInStandardVersion
        {
            get { return _hiddenInStandardVersion; }
            set { _hiddenInStandardVersion = value; }
        }

        private Common.Socket.NiRen_UDP nirenUDP;
        private MobileConfigurationMgr _mobileConfigurationMgr;

        public SettingViewModel(IWindowManager windowManager, IModelValidator<SettingViewModel> validator, ref MobileConfigurationMgr mobileConfigurationMgr) : base(validator)
        {
            _mobileConfigurationMgr = mobileConfigurationMgr;
            _mainConfig = _mobileConfigurationMgr.SettingList[SettingNameKey.Main];

            this.ActiveCode = Common.Platform.PlatformManager.ActiveCode;

            _instrument.InitPickerSource();
            ProtocolTypes = CloudAPI.CustomDeviceMaps[CustomDeviceGroupKey.仪表];
            SerialPorts = MobileConfigurationMgr.SerialPorts;
            BaudRates = _instrument.pickerSource[nameof(_instrument.rate)];

            _weightingCfg.InitPickerSource();
            WZList = _weightingCfg.pickerSource[nameof(_weightingCfg.btns)].Prepend(string.Empty);

            _poundCfg.InitPickerSource();
            PrefixList = _poundCfg.pickerSource[nameof(_poundCfg.codePrefix)];
            GenerationTypes = _poundCfg.codeTypeOptions;

            AutoBackupFrequency = DataBackupCfg.FrequencyOptions;
            ChargeModles = ChargeCfg.ChargeModeOptions;

            HiddenInStandardVersion = Common.Share.VersionControl.CurrentVersion == Common.Share.VersionType.标准版
            ? Visibility.Collapsed : Visibility.Visible;

            canWriteLog = false;

            nirenUDP = new Common.Socket.NiRen_UDP();
            nirenUDP.OnResult += NirenUDP_OnResult;

            this.windowManager = windowManager;

            ResetPrinters();

            AJSystemParamsCfg = AJUtil.TryGetJSONObject<AJSystemParamsCfg>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(AJSystemParamsCfg)].TryGetString()) ?? new AJSystemParamsCfg();

            TTSConfig = AJUtil.TryGetJSONObject<TTSConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(TTSConfig)]?.Value ?? string.Empty) ?? new TTSConfig();
            if ((TTSConfig.Items?.Count).GetValueOrDefault() == 0)
            {
                TTSConfig.InitDefault();
            }

            EnableSameCameraTriggerCloseGate = _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(EnableSameCameraTriggerCloseGate)].TryGetBoolean();

            FastWeigthCorrectConfig = AJUtil.TryGetJSONObject<FastWeigthCorrectConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(FastWeigthCorrectConfig)]?.Value ?? string.Empty) ?? new FastWeigthCorrectConfig();

            SnapWatermarkConfig = AJUtil.TryGetJSONObject<SnapWatermarkConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(SnapWatermarkConfig)]?.Value ?? string.Empty) ?? new SnapWatermarkConfig();

            AJPrintConfig = AJUtil.TryGetJSONObject<AJPrintConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(AJPrintConfig)]?.Value ?? string.Empty) ?? new AJPrintConfig();

            CleanupConfig = AJUtil.TryGetJSONObject<CleanupConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(CleanupConfig)]?.Value ?? string.Empty) ?? new CleanupConfig();

            JieGuanZha = (_mainConfig.Settings["JieGuanZha"]?.Value ?? "False").Equals("True");

            EnableNotBookedCarWeigh = _mainConfig.Settings[nameof(EnableNotBookedCarWeigh)].TryGetBoolean();
            EnableScanQRCodeToWeigh = _mainConfig.Settings[nameof(EnableScanQRCodeToWeigh)].TryGetBoolean();

            YunMengZhiHuiDevice = AJUtil.TryGetJSONObject<YunMengZhiHuiDeviceInfo>(_mainConfig.Settings[nameof(YunMengZhiHuiDevice)].TryGetString()) ?? new YunMengZhiHuiDeviceInfo();

            IDCardReaderCfg = AJUtil.TryGetJSONObject<IDCardReaderCfg>(_mainConfig.Settings[nameof(IDCardReaderCfg)].TryGetString()) ?? new IDCardReaderCfg();
            //IDCardReaderCfg.EnableChanged += (_, val) =>
            //{
            //    if (val)
            //    {
            //        RS232ReaderCfg.Enable = false;
            //    }
            //};

            RS232ReaderCfg = AJUtil.TryGetJSONObject<RS232ReaderCfg>(_mainConfig.Settings[nameof(RS232ReaderCfg)].TryGetString()) ?? new RS232ReaderCfg();

            this.RS232ReaderCfg.EnableChanged += (_, val) =>
            {
                if (val)
                {
                    IDCardReaderCfg.Enable = false;
                    RF3Enable = false;
                    RF4Enable = false;
                    Camera1Enable = false;
                    Camera2Enable = false;
                    Barrier = "0";
                }
            };

            RasterIOStateOptions = new List<int>() { 0, 1 };

            // 有点卡页面，使用worker处理
            var usbDevicesFindWorker = new BackgroundWorker();
            usbDevicesFindWorker.DoWork += (_, e) =>
            {
                var tree = new DeviceTree();
                e.Result = tree.DeviceNodes.Where(p => p.Description.Contains("SDT")).ToList();
            };
            usbDevicesFindWorker.RunWorkerCompleted += (_, e) =>
            {
                if (e.Error == null)
                {
                    var devices = new Dictionary<int, string>();
                    var usbDevices = e.Result as List<DeviceNode>;

                    for (int i = 0; i < usbDevices.Count; i++)
                    {
                        // Port_#0001.Hub_#0001
                        var code = usbDevices[i].LocationInfo?.Split('.').LastOrDefault();
                        if (string.IsNullOrWhiteSpace(code))
                        {
                            continue;
                        }

                        devices.Add(1000 + (code.Replace("Hub_#", "").TryGetInt()), usbDevices[i].Description);
                    }
                    USBDevices = devices;
                }
            };
            usbDevicesFindWorker.RunWorkerAsync();

            SIEMENSSerialPortCfg = AJUtil.TryGetJSONObject<SIEMENSSerialPortCfg>(_mainConfig.Settings[nameof(SIEMENSSerialPortCfg)].TryGetString()) ?? new SIEMENSSerialPortCfg();
            ParitiesDropDown = AJUtil.EnumToDictionary<Parity>(null);
            StopBitsDropDown = AJUtil.EnumToDictionary<StopBits>((val) =>
            {
                switch (val)
                {
                    case StopBits.None:
                        return "0";
                    case StopBits.One:
                        return "1";
                    case StopBits.Two:
                        return "2";
                    case StopBits.OnePointFive:
                        return "1.5";
                    default:
                        return "0";
                }
            });

            LoadConfig();
            LoadZXLPRConfig();
            LoadLEDConfig();
            LoadMONITORConfig();
            LoadChargeConfig();


            //检查用户 修改磅单样式、数据备份恢复 权限
            if (!string.IsNullOrWhiteSpace(Globalspace._currentUser.Permission))
            {
                if (Globalspace._currentUser.Permission.Contains("修改磅单样式")) EnableWeighFormTemplate = true;
                if (Globalspace._currentUser.Permission.Contains("数据备份恢复")) EnableBackup = true;
            }

            //搜索控制器的下拉框默认选中之前选择的IP和MAC
            SelectIp = new KeyValuePair<string, string>(Relay2IP, Relay2Mac);
            //打开窗口就发送广播获取所有的控制器，并且选中默认的设置数据
            EnumComputers();

            //自助过磅码赋值
            var barCodePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\自助过磅码.jpg";
            if (File.Exists(barCodePath))
            {
                BarCodeUrl = barCodePath;
            }


            //});

            ////称重记录、数据管理应用模块启动时太耗时，此处延时启动，点击按钮时，直接显示
            //var delayRunTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(2000) };
            //delayRunTimer.Start();
            //delayRunTimer.Tick += (sender, args) =>
            //{
            //    delayRunTimer.Stop();
            //    LoadWeighFormView();
            //};

            //标记此后，允许记录日志
            canWriteLog = true;
        }

        private void LoadZXLPRConfig()
        {
            LPRCameraType = (DeviceType)Enum.Parse(typeof(DeviceType), _mainConfig.Settings[nameof(LPRCameraType)]
                .TryGetString(DeviceType.臻识.ToString
                ()));

            // 目前只有车牌识别相机那里需要这个下拉框数据源,  所以要移除读头
            DeviceTypes = AJUtil.EnumToDictionary<DeviceType>((enumVal) =>
            {
                var defaultStr = enumVal.ToString();
                var custom = CloudAPI.CustomDeviceMaps[CustomDeviceGroupKey.LPR].FirstOrDefault(p => p.Key == defaultStr);
                if (custom != null)
                {
                    defaultStr = custom.Name;
                }
                return defaultStr;
            }).Where(p => p.Value != DeviceType.UHF读头)
                .ToDictionary(k => k.Key, v => v.Value);

            Controllers = CloudAPI.CustomDeviceMaps[CustomDeviceGroupKey.LPRController];

            Camera1IP = _mainConfig.Settings["Camera1IP"].Value;
            Camera2IP = _mainConfig.Settings["Camera2IP"].Value;
            Camera1Username = _mainConfig.Settings["Camera1Username"].Value;
            Camera2Username = _mainConfig.Settings["Camera2Username"].Value;
            Camera1Password = _mainConfig.Settings["Camera1Password"].Value;
            Camera2Password = _mainConfig.Settings["Camera2Password"].Value;
            Camera1Enable = _mainConfig.Settings[nameof(Camera1Enable)].TryGetInt() == 1;
            Camera2Enable = _mainConfig.Settings[nameof(Camera2Enable)].TryGetInt() == 1;
            Camera1DisableVideo = _mainConfig.Settings[nameof(Camera1DisableVideo)].TryGetBoolean();
            Camera2DisableVideo = _mainConfig.Settings[nameof(Camera2DisableVideo)].TryGetBoolean();
            Camera1RasterCoveredIOState = _mainConfig.Settings[nameof(Camera1RasterCoveredIOState)].TryGetInt();
            Camera2RasterCoveredIOState = _mainConfig.Settings[nameof(Camera2RasterCoveredIOState)].TryGetInt();
            Camera1LEDEnable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["Camera1LEDEnable"].Value));
            Camera2LEDEnable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["Camera2LEDEnable"].Value));
            Camera1GPIO = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["Camera1GPIO"].Value));
            RF1Enable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["RF1Enable"].Value));
            RF2Enable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["RF2Enable"].Value));
            RF1Uart = _mainConfig.Settings["RF1Uart"].Value;
            RF2Uart = _mainConfig.Settings["RF2Uart"].Value;
            RF3Ip = _mainConfig.Settings["RF3IP"].Value;
            RF4Ip = _mainConfig.Settings["RF4IP"].Value;
            RF3Enable = _mainConfig.Settings[nameof(RF3Enable)].TryGetInt() == 1;
            RF4Enable = _mainConfig.Settings[nameof(RF4Enable)].TryGetInt() == 1;
            //RF1Power = LPRSection.Settings["RF1Power"].Value;
            //RF2Power = LPRSection.Settings["RF2Power"].Value;
            RelayEnable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["RelayEnable"].Value));
            Relay2Enable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["Relay2Enable"].Value));

            RelayUart = _mainConfig.Settings["RelayUart"].Value;
            Plate2Enable = _mainConfig.Settings["Plate2Enable"].TryGetInt();

            RelayType = _mainConfig.Settings[nameof(RelayType)].TryGetString();
            if (string.IsNullOrEmpty(RelayType))
            {
                // 兼容之前的旧逻辑 --阿吉 2024年6月20日09点20分
                RelayType = RelayEnable ? "0" : Relay2Enable ? "1" : Plate2Enable == 1 ? "2" : "0";
            };

            LPRSavePath = _mainConfig.Settings["LPRSavePath"].Value;
            selectedController = _mainConfig.Settings["SelectedController"].Value;

            Relay2IP = _mainConfig.Settings["Relay2IP"].Value;
            Relay2Mac = _mainConfig.Settings["Relay2Mac"].Value;

        }

        private void LoadLEDConfig()
        {
            LED2Enable = _mainConfig.Settings["LED2Enable"].TryGetBoolean();
            LED3Enable = _mainConfig.Settings["LED3Enable"].TryGetBoolean();
            LEDIP = _mainConfig.Settings["LEDIP"].Value;
            LEDWIDTH = _mainConfig.Settings["LEDWIDTH"].Value;
            LEDHEIGHT = _mainConfig.Settings["LEDHEIGHT"].Value;
            LED1IP = _mainConfig.Settings["LED1IP"].Value;
            LED1WIDTH = _mainConfig.Settings["LED1WIDTH"].Value;
            LED1HEIGHT = _mainConfig.Settings["LED1HEIGHT"].Value;

            LEDFontSize = _mainConfig.Settings[nameof(LEDFontSize)].TryGetString("12");
            LED1FontSize = _mainConfig.Settings[nameof(LED1FontSize)].TryGetString("12");

            LEDPlaySpeed = _mainConfig.Settings[nameof(LEDPlaySpeed)].TryGetString("20");
            LED1PlaySpeed = _mainConfig.Settings[nameof(LED1PlaySpeed)].TryGetString("20");
        }

        private void LoadMONITORConfig()
        {
            var defaultName = "admin";
            var defaultPsw = "123456";

            var defaultRtspUrl = "rtsp://{0}:{1}@{2}:554/h264/ch1/sub/av_stream";

            Monitor1Enable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["Monitor1Enable"].Value));
            Monitor2Enable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["Monitor2Enable"].Value));
            Monitor3Enable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["Monitor3Enable"].Value));
            Monitor4Enable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["Monitor4Enable"].Value));
            Monitor1IP = _mainConfig.Settings["Monitor1IP"].TryGetString("192.168.1.100");
            Monitor2IP = _mainConfig.Settings["Monitor2IP"].TryGetString("192.168.1.101");
            Monitor3IP = _mainConfig.Settings["Monitor3IP"].TryGetString("192.168.1.102");
            Monitor4IP = _mainConfig.Settings["Monitor4IP"].TryGetString("192.168.1.103");
            Monitor1Username = _mainConfig.Settings["Monitor1Username"].TryGetString(defaultName);
            Monitor2Username = _mainConfig.Settings["Monitor2Username"].TryGetString(defaultName);
            Monitor3Username = _mainConfig.Settings["Monitor3Username"].TryGetString(defaultName);
            Monitor4Username = _mainConfig.Settings["Monitor4Username"].TryGetString(defaultName);
            Monitor1Password = _mainConfig.Settings["Monitor1Password"].TryGetString(defaultPsw);
            Monitor2Password = _mainConfig.Settings["Monitor2Password"].TryGetString(defaultPsw);
            Monitor3Password = _mainConfig.Settings["Monitor3Password"].TryGetString(defaultPsw);
            Monitor4Password = _mainConfig.Settings["Monitor4Password"].TryGetString(defaultPsw);


            Monitor1RTSPUrl = _mainConfig.Settings[nameof(Monitor1RTSPUrl)].TryGetString(string.Format(defaultRtspUrl, Monitor1Username, monitor1Password, Monitor1IP));
            Monitor2RTSPUrl = _mainConfig.Settings[nameof(Monitor2RTSPUrl)].TryGetString(string.Format(defaultRtspUrl, Monitor2Username, monitor2Password, Monitor2IP));
            Monitor3RTSPUrl = _mainConfig.Settings[nameof(Monitor3RTSPUrl)].TryGetString(string.Format(defaultRtspUrl, Monitor3Username, monitor3Password, Monitor3IP));
            Monitor4RTSPUrl = _mainConfig.Settings[nameof(Monitor4RTSPUrl)].TryGetString(string.Format(defaultRtspUrl, Monitor4Username, monitor4Password, Monitor4IP));

            MonitorSavePath = _mainConfig.Settings["MonitorSavePath"].Value;

            //2022-12-15 add
            MonitorCaptureEnable = Convert.ToBoolean(_mainConfig.Settings["MonitorCaptureEnable"].Value);
        }

        private void LoadChargeConfig()
        {
            //收费基数配置数据加载
            ChargeEnable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["ChargeEnable"].Value));
            //多次收费模式
            MultipleChargeEnable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["MultipleChargeEnable"].Value));

            //是否启用称重处的电子支付功能
            EChargeEnable = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["EChargeEnable"].Value));
            //是否按照物资收费
            ChargeByMaterial = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["ChargeByMaterial"].Value));
            ChargeWay = _mainConfig.Settings["ChargeWay"].Value;
            //收费方式：1、按单位收费。2、按范围收费
            ChargeType = _mainConfig.Settings["ChargeType"].Value;

            ParkingID = _mainConfig.Settings["ParkingID"].Value;
            PServerPath = _mainConfig.Settings["PServerPath"].Value;
            UUID = _mainConfig.Settings["UUID"].Value;
            MAC = _mainConfig.Settings["MAC"].Value;

            //最低消费
            LowestFees = _mainConfig.Settings["LowestFees"].Value;
            //称重结束收费?
            ChargeTypes = _mainConfig.Settings["ChargeTypes"].Value;

            //是否支持用户存储充值
            ChargeStorage = Convert.ToBoolean(Convert.ToInt32(_mainConfig.Settings["ChargeStorage"].Value));

        }

        private void LoadSyncConfig()
        {
            Pzbjzt = Convert.ToBoolean(_syncSection.Settings["Pzbjzt"].Value);
            ServerPath = _syncSection.Settings["ServerPath"].Value;
            Pzbjfz = _syncSection.Settings["Pzbjfz"].Value;
        }

        private void LoadVirtualWallConfig()
        {
            VirtualWall = _virtualWallSection.Settings["VirtualWall"].Value;
            VWCamera1IP = _virtualWallSection.Settings["VWCamera1IP"].Value;
            VWCamera1Username = _virtualWallSection.Settings["VWCamera1Username"].Value;
            VWCamera1Password = _virtualWallSection.Settings["VWCamera1Password"].Value;
            VWCamera2IP = _virtualWallSection.Settings["VWCamera2IP"].Value;
            VWCamera2Username = _virtualWallSection.Settings["VWCamera2Username"].Value;
            VWCamera2Password = _virtualWallSection.Settings["VWCamera2Password"].Value;

            VWCamera3IP = _virtualWallSection.Settings["VWCamera3IP"].Value;
            VWCamera3Username = _virtualWallSection.Settings["VWCamera3Username"].Value;
            VWCamera3Password = _virtualWallSection.Settings["VWCamera3Password"].Value;

            VWCamera4IP = _virtualWallSection.Settings["VWCamera4IP"].Value;
            VWCamera4Username = _virtualWallSection.Settings["VWCamera4Username"].Value;
            VWCamera4Password = _virtualWallSection.Settings["VWCamera4Password"].Value;

            VWRF3Ip = _virtualWallSection.Settings["VWRF3Ip"].Value;
            VWRF4Ip = _virtualWallSection.Settings["VWRF4Ip"].Value;
            VWWhiteListeEnable = Convert.ToBoolean(Convert.ToInt32(_virtualWallSection.Settings["VWWhiteListeEnable"].Value));
            EnableWeighFinish = Convert.ToBoolean(Convert.ToInt32(_virtualWallSection.Settings["EnableWeighFinish"].Value));
            VWStorageOverflow = Convert.ToBoolean(Convert.ToInt32(_virtualWallSection.Settings["VWStorageOverflow"].Value));
            VWOpenBarrier = Convert.ToBoolean(Convert.ToInt32(_virtualWallSection.Settings["VWOpenBarrier"].Value));
        }

        private void LoadConfig()
        {
            SelectedPrefix = _mainConfig.Settings["Prefix"].TryGetString();
            SelectedGenerationType = _mainConfig.Settings["GenerationType"].TryGetString();

            GblxCG = _mainConfig.Settings["GblxCG"].TryGetString();
            GblxXS = _mainConfig.Settings["GblxXS"].TryGetString();
            GblxQT = _mainConfig.Settings["GblxQT"].TryGetString();
            CompanyName = _mainConfig.Settings["CompanyName"].TryGetString();
            Fhdw_mrz = _mainConfig.Settings["Fhdw_mrz"].TryGetString();

            DI3State = _mainConfig.Settings["DI3State"].TryGetString();
            DI4State = _mainConfig.Settings["DI4State"].TryGetString();
            DI5State = _mainConfig.Settings["DI5State"].TryGetString();
            DI6State = _mainConfig.Settings["DI6State"].TryGetString();
            DI7State = _mainConfig.Settings["DI7State"].TryGetString();
            DI8State = _mainConfig.Settings["DI8State"].TryGetString();

            Byzd_Key1 = _mainConfig.Settings["Byzd_Key1"].TryGetString();
            Byzd_Key2 = _mainConfig.Settings["Byzd_Key2"].TryGetString();
            Byzd_Key3 = _mainConfig.Settings["Byzd_Key3"].TryGetString();
            Byzd_Key4 = _mainConfig.Settings["Byzd_Key4"].TryGetString();

            Byzd_Value1 = _mainConfig.Settings["Byzd_Value1"].TryGetString();
            Byzd_Value2 = _mainConfig.Settings["Byzd_Value2"].TryGetString();
            Byzd_Value3 = _mainConfig.Settings["Byzd_Value3"].TryGetString();
            Byzd_Value4 = _mainConfig.Settings["Byzd_Value4"].TryGetString();

            WeighName = _mainConfig.Settings[nameof(WeighName)].TryGetString();
            EffectiveWeight = _mainConfig.Settings[nameof(EffectiveWeight)].TryGetString("0");
            Weigh2Name = _mainConfig.Settings["Weigh2Name"].TryGetString();
            WeighProtocolType = _mainConfig.Settings["WeighProtocolType"].TryGetString();
            WeighSerialPortName = _mainConfig.Settings["WeighSerialPortName"].TryGetString();
            WeighSerialPortBaudRate = _mainConfig.Settings["WeighSerialPortBaudRate"].TryGetString();
            EnableSecondDevice = _mainConfig.Settings["EnableSecondDevice"].TryGetBoolean();
            Weigh2ProtocolType = _mainConfig.Settings["Weigh2ProtocolType"].TryGetString();
            Weigh2SerialPortName = _mainConfig.Settings["Weigh2SerialPortName"].TryGetString();
            Weigh2SerialPortBaudRate = _mainConfig.Settings["Weigh2SerialPortBaudRate"].TryGetString();
            WeighFormDisplayMode = _mainConfig.Settings["WeighFormDisplayMode"].TryGetString();
            WithPrinting = _mainConfig.Settings["WithPrinting"].TryGetBoolean();
            PrintingMode = _mainConfig.Settings["PrintingMode"].TryGetString();
            PrintingType = _mainConfig.Settings["PrintingType"].TryGetString();
            Printer = _mainConfig.Settings["Printer"].TryGetString();
            PageSizeHeight = _mainConfig.Settings["PageSizeHeight"].TryGetString();

            Dyrqgs = _mainConfig.Settings["Dyrqgs"].TryGetString();
            Pzrqgs = _mainConfig.Settings["Pzrqgs"].TryGetString();
            Mzrqgs = _mainConfig.Settings["Mzrqgs"].TryGetString();
            Jzrqgs = _mainConfig.Settings["Jzrqgs"].TryGetString();

            PageSizeWidth = _mainConfig.Settings["PageSizeWidth"].TryGetString();

            WeighingUnit = _mainConfig.Settings["WeighingUnit"].TryGetString();
            WeightValueDisplayFormatOptions = new List<DropdownOption>()
            {
                new DropdownOption
                {
                    label = "无小数",
                    value = 0
                },
                new DropdownOption
                {
                    label = "0.0",
                    value = 1
                },
                new DropdownOption
                {
                    label = "0.00",
                    value = 2
                },
                new DropdownOption
                {
                    label = "0.000",
                    value = 3
                }
            };
            WeightValueDisplayFormat = _mainConfig.Settings["WeightValueDisplayFormat"].TryGetInt();

            WeighingMode = _mainConfig.Settings["WeighingMode"].TryGetString();
            TableRFEnable = _mainConfig.Settings["TableRFEnable"].TryGetBoolean();
            TableRFPortName = _mainConfig.Settings["TableRFPortName"].TryGetString();
            QREnable = _mainConfig.Settings["QREnable"].TryGetBoolean();
            QRPortName = _mainConfig.Settings["QRPortName"].TryGetString();
            QR2Enable = _mainConfig.Settings["QR2Enable"].TryGetBoolean();
            QRPort2Name = _mainConfig.Settings["QRPort2Name"].TryGetString();
            WeighingControl = _mainConfig.Settings["WeighingControl"].TryGetString();
            StableDelay = _mainConfig.Settings["StableDelay"].TryGetString();
            MinSlotWeight = _mainConfig.Settings["MinSlotWeight"].TryGetString();
            Discount = _mainConfig.Settings["Discount"].TryGetString();
            DiscountWeight = _mainConfig.Settings["DiscountWeight"].TryGetString();
            DiscountRate = _mainConfig.Settings["DiscountRate"].TryGetString();
            LPR = _mainConfig.Settings["车牌识别"].TryGetString();
            Barrier = _mainConfig.Settings["Barrier"].TryGetString();
            LightType = _mainConfig.Settings["LightType"].TryGetString();
            OpenBarrierB = _mainConfig.Settings["OpenBarrierB"].TryGetString();

            MonitorEnable = _mainConfig.Settings["MonitorEnable"].TryGetBoolean();

            SyncDataEnable = _mainConfig.Settings["SyncDataEnable"].TryGetBoolean();
            LEDEnable = _mainConfig.Settings["LEDEnable"].TryGetBoolean();
            LEDPortName = _mainConfig.Settings["LEDPortName"].TryGetString();

            SpeechSpeed = Convert.ToDouble(_mainConfig.Settings["SpeechSpeed"].TryGetString("0"));

            OverloadWarning = _mainConfig.Settings["OverloadWarning"].TryGetString();
            OverloadAction = _mainConfig.Settings["OverloadAction"].TryGetString();
            OverloadLog = _mainConfig.Settings["OverloadLog"].TryGetString();

            OverloadAxle2 = _mainConfig.Settings["OverloadAxle2"].TryGetString();
            OverloadAxle3 = _mainConfig.Settings["OverloadAxle3"].TryGetString();
            OverloadAxle4 = _mainConfig.Settings["OverloadAxle4"].TryGetString();
            OverloadAxle5 = _mainConfig.Settings["OverloadAxle5"].TryGetString();
            OverloadAxle6 = _mainConfig.Settings["OverloadAxle6"].TryGetString();

            OverloadWarningWeight = _mainConfig.Settings["OverloadWarningWeight"].TryGetString();
            OverloadWarningText = _mainConfig.Settings["OverloadWarningText"].TryGetString();
            BackupPath = _mainConfig.Settings["BackupPath"].TryGetString();
            CurrentAutoBackupFrequency = _mainConfig.Settings["CurrentAutoBackupFrequency"].TryGetString();

            CheckGrating = _mainConfig.Settings["CheckGrating"].TryGetInt();
            lPRWhiteList = _mainConfig.Settings["lPRWhiteList"].TryGetInt();
            AutoWeighingType = _mainConfig.Settings["AutoWeighingType"].TryGetInt();
            TimelyRefresh = _mainConfig.Settings["TimelyRefresh"].TryGetInt();

            ManualPZOrMZ = _mainConfig.Settings["ManualPZOrMZ"].TryGetBoolean(false);

            AutoConvertUnit = _mainConfig.Settings["AutoConvertUnit"].TryGetBoolean();
            CancelWeighingTime = _mainConfig.Settings["CancelWeighingTime"].TryGetString();
            LastPlateTimer = _mainConfig.Settings["LastPlateTimer"].TryGetString();
            WeighingValidTime = _mainConfig.Settings["WeighingValidTime"].TryGetString();

            PageCount = _mainConfig.Settings["PageCount"].TryGetString();

            SyncDataEnable = _mainConfig.Settings["SyncDataEnable"].TryGetBoolean();
            SyncYycz = _mainConfig.Settings["SyncYycz"].TryGetBoolean();
            SyncYyzdsh = _mainConfig.Settings["SyncYyzdsh"].TryGetBoolean();

            LightSwitch = _mainConfig.Settings["LightSwitch"].TryGetString();

            Ledxsnr = _mainConfig.Settings["Ledxsnr"].TryGetString();
            Ledsbcps = _mainConfig.Settings["Ledsbcps"].TryGetString();
            Ledzlwds = _mainConfig.Settings["Ledzlwds"].TryGetString();
            Leddyc = _mainConfig.Settings["Leddyc"].TryGetString();
            Leddec = _mainConfig.Settings["Leddec"].TryGetString();

            LEDTypes = AJUtil.EnumToDictionary<LEDType>((enumVal) =>
            {
                var defaultStr = enumVal.ToString();
                var custom = CloudAPI.CustomDeviceMaps[CustomDeviceGroupKey.LED].FirstOrDefault(p => p.Key == defaultStr);
                if (custom != null)
                {
                    defaultStr = custom.Name;
                }
                return defaultStr;
            });
            LEDType = (LEDType)Enum.Parse(typeof(LEDType), _mainConfig.Settings[nameof(LEDType)].TryGetString(Common.LPR.LEDType.灵信T系列.ToString()));

        }

        /// <summary>
        /// 此标记，用于确定是否可以写日志，比如在加载参数设置的时候，不加这个标记会把所有的设置都写一次日志，
        /// 很明显，这是不对的，因为这些不是人为操作的记录，所以需要加这个标记用以区分是否是有效的日志行为。
        /// </summary>
        bool canWriteLog = true;
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="key">修改的项名称</param>
        /// <param name="value">修改的结果</param>
        private void Log(string key, object value)
        {
            if (canWriteLog)
                AJLog4NetLogger.Instance().Info($"用户{Globalspace._currentUser.UserName}修改了配置项：{key} 的结果为：{value}");
        }

        public void OpenTemplateExeclFile()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Globalspace._weightFormTemplatePath,
                UseShellExecute = true,
            });

        }

        public void EditWeighFormList()
        {
            var viewModel = new WeighFormListViewModel(ref _mainConfig);
            this.windowManager.ShowWindow(viewModel);
        }

        public void EditByFieldFormula()
        {
            var viewModel = new ByFieldFormulaViewModel(windowManager, ref _mainConfig);
            this.windowManager.ShowWindow(viewModel);
        }

        public void SetBackupPath()
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                InitialDirectory = BackupPath
            };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                BackupPath = dialog.SafeFolderName;
            }
        }

        public void SetLPRSavePath()
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                InitialDirectory = LPRSavePath,
            };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                LPRSavePath = dialog.SafeFolderName;
            }
        }

        public void SetMonitorSavePath()
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                InitialDirectory = MonitorSavePath,
            };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                MonitorSavePath = dialog.SafeFolderName;
            }
        }

        public void ManualBackup()
        {
            BackupHelper.ManualBackupAsync(windowManager);

        }

        public void OpenBackupFolder()
        {
            Process.Start("explorer.exe", BackupPath);
        }

        public void DBRestore()
        {
            BackupHelper.DBRestoreAsync(windowManager);
        }

        public async void DBClean()
        {
            try
            {
                await BackupHelper.DbCleanAsync(this.windowManager);
            }
            catch (Exception e)
            {
                windowManager.ShowMessageBox(e.Message);
            }
        }

        //public void showEChargeEdit(string type)
        //{
        //    bool? result = windowManager.ShowDialog(new ChargeEditViewModel(this));
        //}


        //public void LPRFuzzyMatching()
        //{
        //    var viewModel = new LPRFuzzyMatchingViewModel();
        //    this.windowManager.ShowWindow(viewModel);
        //}

        //public void BackupSettings()
        //{
        //    BackupHelper.BackupSettings();
        //}

        //public void RestoreSettings()
        //{
        //    BackupHelper.RestoreSettings();
        //}

        public void ReUpload()
        {
            try
            {
                Common.SyncData.Instal.ReUpload();
            }
            catch { }
        }

        public void EnumComputers()
        {
            try
            {
                count = 0;
                ipList.Clear();
                FontColor = Brushes.Black;
                //SearchTips = "正在搜索.....";
                nirenUDP.Radio();
            }
            catch { }
        }

        int max = 0;
        int count = 0;
        public Brush FontColor { get; set; } = Brushes.Black;
        public string searchTips;
        public string SearchTips
        {
            get { return searchTips; }
            set
            {

                searchTips = value;
                OnPropertyChanged("Content");
            }
        }

        public void NirenUDP_OnResult(string ip, string mac, string gateway, string mask)
        {
            //如果是网关指令返回的结果
            if (!string.IsNullOrWhiteSpace(gateway))
            {
                TRelay2Gateway = gateway;
            }
            else if (!string.IsNullOrWhiteSpace(mask))//如果是子网掩码指令返回的结果
            {
                TRelay2Mask = mask;
            }
            else//网关、子网掩码为空说明这个动作是获取IP和MAC的命令
            {
                _myPing_PingCompleted(ip, mac);
            }
        }

        private KeyValuePair<string, string> selectIp;
        public KeyValuePair<string, string> SelectIp
        {
            get
            {
                return selectIp;
            }
            set
            {
                selectIp = value;
                _mainConfig.Settings["Relay2IP"].Value = value.Key.ToString();
                _mainConfig.Settings["Relay2Mac"].Value = value.Value.ToString();
                OnPropertyChanged("SelectIp");
            }
        }
        public ObservableCollection<KeyValuePair<string, string>> ipList { get; set; } = new ObservableCollection<KeyValuePair<string, string>>();

        public void _myPing_PingCompleted(string ip, string mac)
        {
            if (!ipList.Any(p => p.Key == ip))
            {

                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));

                SynchronizationContext.Current.Post(pl =>
                {

                    var item = new KeyValuePair<string, string>(ip, mac);

                    //不添加重复的
                    if (!ipList.Any(a => a.Key == ip))
                    {
                        ipList.Add(item);
                    }
                    count++;
                    //if (count >= max)
                    {
                        if (ipList.Count <= 0)
                        {
                            FontColor = Brushes.Red;
                            SearchTips = "未找到设备.....";
                        }
                        else
                        {
                            //FontColor = Brushes.Green;
                            //SearchTips = $"{count}";
                            SearchTips = $"";
                        }
                    }

                }, null);

            }


            if (TRelay2IP == ip)
            {
                //保存后重新搜索后，选中之前修改的那个ip
                SelectIp = ipList.FirstOrDefault(a => a.Key == TRelay2IP);
            }
        }

        private string relay2IP;
        public string Relay2IP
        {
            get { return relay2IP; }
            set
            {
                relay2IP = value;

                if (ValidateProperty("Relay2IP"))
                {
                    _mainConfig.Settings["Relay2IP"].Value = value;
                }
            }
        }
        public string TRelay2IP { get; set; }
        public string TRelay2Mac { get; set; }

        private string relay2Mac;
        public string Relay2Mac
        {
            get { return relay2Mac; }
            set
            {
                relay2Mac = value;

                if (ValidateProperty("Relay2Mac"))
                {
                    _mainConfig.Settings["Relay2Mac"].Value = value;
                }
            }
        }

        private string relay2Mask;
        public string Relay2Mask
        {
            get { return relay2Mask; }
            set
            {
                relay2Mask = value;

                if (ValidateProperty("Relay2Mask"))
                {
                    _mainConfig.Settings["Relay2Mask"].Value = value;
                }
            }
        }
        public string TRelay2Mask { get; set; }

        private string relay2Gateway;
        public string Relay2Gateway
        {
            get { return relay2Gateway; }
            set
            {
                relay2Gateway = value;

                if (ValidateProperty("Relay2Gateway"))
                {
                    _mainConfig.Settings["Relay2Gateway"].Value = value;
                }
            }
        }
        public string TRelay2Gateway { get; set; }

        public void SaveNiRenInfo()
        {
            if (!string.IsNullOrWhiteSpace(SelectIp.Key))
            {
                Relay2IP = TRelay2IP;
                Relay2Mac = TRelay2Mac;
                //Relay2Mask = TRelay2Mask;
                //Relay2Gateway = TRelay2Gateway;

                nirenUDP.SetDevice(SelectIp.Value, TRelay2IP, TRelay2Mask, TRelay2Gateway);
                //Thread.Sleep(50);//间隔一下，不然有可能获取不到数据
                EnumComputers();
                //Thread.Sleep(100);//间隔一下，不然有可能获取不到数据
            }
            HideEdit();
        }

        public bool ShowEditPanel { get; set; }
        public void ShowEdit()
        {
            if (!string.IsNullOrWhiteSpace(SelectIp.Key))
            {
                TRelay2IP = SelectIp.Key;
                TRelay2Mac = SelectIp.Value;
                //查询出ip、mac、网关
                nirenUDP.GetGateway(SelectIp.Value);
                nirenUDP.GetMask(SelectIp.Value);
            }
            ShowEditPanel = true;
        }

        public void HideEdit()
        {
            ShowEditPanel = false;
        }

        public void ResetPrinters()
        {
            _printCfg.InitPickerSource();
            PrinterList = new ObservableCollection<Common.AJControls.AJMultiSelect.AJMultiSelectOptionItem>(_printCfg.pickerSource[nameof(_printCfg.printer)].Select(p => new Common.AJControls.AJMultiSelect.AJMultiSelectOptionItem
            {
                Label = p,
                Value = p
            }).ToList());

            Printer = string.Empty;
        }

        public void DownloadBarCode()
        {
            //try
            //{
            //    string result = Common.SyncData.GetBarCode();
            //    BarCodeUrl = result;

            //    var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //    var fileName = $"{desktopPath}\\自助过磅码.jpg";
            //    Common.SyncData.FileDownload(result, fileName);
            //    if (string.IsNullOrEmpty(BarCodeUrl))
            //    {

            //        DownloadBarCodeTips = "下载失败请激活软件，或者联系服务商";
            //    }
            //    else
            //    {
            //        DownloadBarCodeTips = string.Empty;
            //    }
            //}
            //catch (Exception e)
            //{
            //    BarCodeUrl = string.Empty;
            //}

        }

        public void CheckByzd()
        {
            try
            {
                if (IsError(Byzd_Key1) && IsError(Byzd_Key2) && IsError(Byzd_Key3) && IsError(Byzd_Key4))
                {
                    Byzd_Mess = string.Empty;
                }
                else
                {
                    Byzd_Mess = ("温馨提示：只允许_by5至_by20字段定义");
                }
            }
            catch
            {
                Byzd_Mess = ("温馨提示：只允许_by5至_by20字段定义");
            }
        }

        private bool IsError(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt)) return false;

            if (!txt.ToLower().Contains("_by"))
            {
                return false;
            }

            int index = 0;
            if (!int.TryParse(txt.ToLower().Replace("_by", string.Empty), out index))
            {
                return false;
            }

            if (index > 20 || index <= 5)
            {
                return false;
            }

            return !((txt.ToLower() == "_by1" || txt.ToLower() == "_by2" || txt.ToLower() == "_by3" || txt.ToLower() == "_by4" || txt.ToLower() == "_by5"));
        }

        public void OpenToolsFolder(string param)
        {
            OpenToolsFolderCore(param);
        }

        public static void OpenToolsFolderCore(string param)
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            string path = $"{root}\\Resources\\Tools\\CarmeraTools\\LPRConfigTool.exe";

            switch (param)
            {
                case "xj":
                    path = $"{root}\\Resources\\Tools\\CarmeraTools\\LPRConfigTool.exe";
                    break;
                case "kzq":
                    path = $"{root}\\Resources\\Tools\\ControllerTools\\参数配置V4.42.36.exe";
                    break;
            }

            if (File.Exists(path))
            {
                try
                {
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true,
                    });
                }
                catch { }
            }
        }

        public void CleanupProcess()
        {
            try
            {
                BackupHelper.CleanupByMonthCfg(this.windowManager, _cleanupConfig);
            }
            catch (Exception e)
            {
                windowManager.ShowMessageBox(e.Message);
            }
        }

        protected override void OnClose()
        {
            var errMsg = new List<string>();

            Func<ProcessResult> mainConfigSaveHandler = () =>
            {
                ////始终是开启同步模式
                //_mainConfig.Settings["SyncDataEnable"].Value = true.ToString();
                //版本之间逻辑检测和处理
                if (Common.Share.VersionControl.CurrentVersion == Common.Share.VersionType.标准版)
                {
                    _mainConfig.Settings["Barrier"].Value = "1";
                }

                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(TTSConfig)].Value = AJUtil.AJSerializeObject(new
                {
                    Items = TTSConfig.Items.Select(p => new
                    {
                        p.Enable,
                        p.Type,
                        p.Text
                    }),
                });

                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(AJSystemParamsCfg)].Value = AJUtil.AJSerializeObject(new
                {
                    AJSystemParamsCfg.ConnectTimeout,
                    AJSystemParamsCfg.HeartbeatInterval,
                    AJSystemParamsCfg.OfflineCheck
                });

                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(FastWeigthCorrectConfig)].Value = AJUtil.AJSerializeObject(new
                {
                    FastWeigthCorrectConfig.Enable,
                    FastWeigthCorrectConfig.Limit,
                    FastWeigthCorrectConfig.F11FunValue,
                    FastWeigthCorrectConfig.F12FunValue,
                });

                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(SnapWatermarkConfig)].Value = AJUtil.AJSerializeObject(new
                {
                    SnapWatermarkConfig.Enable,
                    SnapWatermarkConfig.CHEnable,
                    SnapWatermarkConfig.MZEnable,
                    SnapWatermarkConfig.PZEnable,
                    SnapWatermarkConfig.JZEnable,
                    SnapWatermarkConfig.WeighbridgeNameEnable,
                    SnapWatermarkConfig.CustomTextEnable,
                    SnapWatermarkConfig.CustomText
                });

                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(AJPrintConfig)].Value = AJUtil.AJSerializeObject(new
                {
                    AJPrintConfig.IsDateUpper,
                    AJPrintConfig.IsWeightNumberUpper
                });

                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(CleanupConfig)].Value = AJUtil.AJSerializeObject(new
                {
                    CleanupConfig.Month,
                    CleanupConfig.EnableAuto
                });

                _mainConfig.Settings[nameof(YunMengZhiHuiDevice)].Value = AJUtil.AJSerializeObject(new
                {
                    YunMengZhiHuiDevice.Enable,
                    YunMengZhiHuiDevice.IP,
                    YunMengZhiHuiDevice.Port,
                    YunMengZhiHuiDevice.Code,
                    YunMengZhiHuiDevice.Message
                });

                _mainConfig.Settings[nameof(RS232ReaderCfg)].Value = AJUtil.AJSerializeObject(RS232ReaderCfg);

                _mainConfig.Settings[nameof(IDCardReaderCfg)].Value = AJUtil.AJSerializeObject(new
                {
                    IDCardReaderCfg.Enable,
                    IDCardReaderCfg.Port,
                });

                _mainConfig.Settings[nameof(SIEMENSSerialPortCfg)].Value = AJUtil.AJSerializeObject(new
                {
                    SIEMENSSerialPortCfg.PortName,
                    SIEMENSSerialPortCfg.BaudRate,
                    SIEMENSSerialPortCfg.Parity,
                    SIEMENSSerialPortCfg.StopBits,
                    SIEMENSSerialPortCfg.DataBits,
                    SIEMENSSerialPortCfg.ConfirmCmd,
                    SIEMENSSerialPortCfg.Gate1OpenCmd,
                    SIEMENSSerialPortCfg.Gate2OpenCmd,
                    SIEMENSSerialPortCfg.Gate1CloseCmd,
                    SIEMENSSerialPortCfg.Gate2CloseCmd,
                    SIEMENSSerialPortCfg.GreenLight1Cmd,
                    SIEMENSSerialPortCfg.GreenLight2Cmd,
                    SIEMENSSerialPortCfg.Raster1Cmd,
                    SIEMENSSerialPortCfg.Raster2Cmd,
                    SIEMENSSerialPortCfg.Raster1CoverValueCmd,
                    SIEMENSSerialPortCfg.Raster2CoverValueCmd
                });

                _mainConfig.Settings[nameof(EnableSameCameraTriggerCloseGate)].Value = EnableSameCameraTriggerCloseGate.ToString();

                return _mobileConfigurationMgr.SaveSetting();
            };

            var sections = new Dictionary<string, Func<ProcessResult>>
            {
                {"主程序", mainConfigSaveHandler},
            };

            foreach (var item in sections)
            {
                var ret = item.Value.Invoke();
                if (!ret.Success)
                {
                    errMsg.Add($"{item.Key}配置保存失败:{ret.Message}");
                }
            }

            try
            {
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception e)
            {
                errMsg.Add($"配置节点刷新失败:{e.Message}");
            }

            if (errMsg.Count > 0)
            {
                MessageBox.Show(string.Join(";\r\n", errMsg), "配置保存异常");
            }

            base.OnClose();
        }
    }



    public class SettingViewModelValidator : AbstractValidator<SettingViewModel>
    {
        public SettingViewModelValidator()
        {
            RuleFor(x => x.PageSizeHeight).Custom((text, context) =>
            {
                try
                {
                    int i = Convert.ToInt32(text);
                    if (i > 300)
                    {
                        context.AddFailure("纸张高度不能超过300mm");
                    }
                }
                catch
                {
                    context.AddFailure("请输入数字");
                }
            });

            RuleFor(x => x.Dyrqgs).Custom((text, context) =>
            {
                try
                {
                    DateTime.Now.ToString(text);
                }
                catch
                {
                    context.AddFailure("请输入正确的打印日期格式");
                }
            });

            RuleFor(x => x.Mzrqgs).Custom((text, context) =>
            {
                try
                {
                    DateTime.Now.ToString(text);
                }
                catch
                {
                    context.AddFailure("请输入正确的毛重日期格式");
                }
            });

            RuleFor(x => x.Pzrqgs).Custom((text, context) =>
            {
                try
                {
                    DateTime.Now.ToString(text);
                }
                catch
                {
                    context.AddFailure("请输入正确的皮重日期格式");
                }
            });

            RuleFor(x => x.Jzrqgs).Custom((text, context) =>
            {
                try
                {
                    DateTime.Now.ToString(text);
                }
                catch
                {
                    context.AddFailure("请输入正确的净重日期格式");
                }
            });

            RuleFor(x => x.PageSizeWidth).Custom((text, context) =>
            {
                try
                {
                    int i = Convert.ToInt32(text);
                    if (i > 250)
                    {
                        context.AddFailure("纸张宽度不能超过210mm");
                    }
                }
                catch
                {
                    context.AddFailure("请输入数字");
                }
            });

            RuleFor(x => x.StableDelay).Custom((text, context) =>
            {
                try
                {
                    int i = Convert.ToInt32(text);
                    if (i > 60)
                    {
                        context.AddFailure("稳定延时必须小于60秒");
                    }
                    if (i < 1)
                    {
                        context.AddFailure("稳定延时最少1秒");
                    }
                }
                catch
                {
                    context.AddFailure("请输入正整数");
                }
            });

            RuleFor(x => x.MinSlotWeight).Custom((text, context) =>
            {
                try
                {
                    var i = Convert.ToDecimal(text);

                    if (i < 0)
                    {
                        context.AddFailure("请输入正数");
                    }
                }
                catch
                {
                    context.AddFailure("请输入合法的数字");
                }
            });

            RuleFor(x => x.DiscountWeight).Custom((text, context) =>
            {
                try
                {
                    var i = Convert.ToDouble(text);
                }
                catch
                {
                    context.AddFailure("请输入数字");
                }
            });

            RuleFor(x => x.DiscountRate).Custom((text, context) =>
            {
                try
                {
                    int i = Convert.ToInt32(text);
                    if (i > 100)
                    {
                        context.AddFailure("扣率不能大于100%");
                    }
                    if (i < 0)
                    {
                        context.AddFailure("扣率不能小于0");
                    }
                }
                catch
                {
                    context.AddFailure("请输入0-100的整数");
                }
            });



            RuleFor(x => x.OverloadWarningWeight).Custom((text, context) =>
            {
                try
                {
                    var i = Convert.ToDecimal(text);

                    if (i < 0)
                    {
                        context.AddFailure("请输入正数");
                    }
                }
                catch
                {
                    context.AddFailure("请输入合法的数字");
                }
            });

            RuleFor(x => x.OverloadAxle2).Custom((text, context) =>
            {
                try
                {
                    var i = Convert.ToDecimal(text);

                    if (i < 0)
                    {
                        context.AddFailure("请输入正数");
                    }
                }
                catch
                {
                    context.AddFailure("请输入合法的数字");
                }
            });

            RuleFor(x => x.OverloadAxle3).Custom((text, context) =>
            {
                try
                {
                    var i = Convert.ToDecimal(text);

                    if (i < 0)
                    {
                        context.AddFailure("请输入正数");
                    }
                }
                catch
                {
                    context.AddFailure("请输入合法的数字");
                }
            });

            RuleFor(x => x.OverloadAxle4).Custom((text, context) =>
            {
                try
                {
                    var i = Convert.ToDecimal(text);

                    if (i < 0)
                    {
                        context.AddFailure("请输入正数");
                    }
                }
                catch
                {
                    context.AddFailure("请输入合法的数字");
                }
            });

            RuleFor(x => x.OverloadAxle5).Custom((text, context) =>
            {
                try
                {
                    var i = Convert.ToDecimal(text);

                    if (i < 0)
                    {
                        context.AddFailure("请输入正数");
                    }
                }
                catch
                {
                    context.AddFailure("请输入合法的数字");
                }
            });

            RuleFor(x => x.OverloadAxle6).Custom((text, context) =>
            {
                try
                {
                    var i = Convert.ToDecimal(text);

                    if (i < 0)
                    {
                        context.AddFailure("请输入正数");
                    }
                }
                catch
                {
                    context.AddFailure("请输入合法的数字");
                }
            });

            RuleFor(x => x.OverloadWarningText).NotNull();

            RuleFor(x => x.BackupPath).NotEmpty().WithMessage("路径不能为空");

            RuleFor(x => x.TRelay2IP).NotEmpty().WithMessage("IP地址不能为空");
            RuleFor(x => x.TRelay2Gateway).NotEmpty().WithMessage("网关不能为空");
            RuleFor(x => x.TRelay2Mask).NotEmpty().WithMessage("子网掩码不能为空");



            RuleFor(x => x.Byzd_Key1).Custom((text, context) =>
            {
                try
                {
                    if (!CheckByzd(text))
                    {
                        context.AddFailure("只允许_by5至_by20字段定义");
                    }
                }
                catch
                {
                    context.AddFailure("只允许_by5至_by20字段定义");
                }
            });

            RuleFor(x => x.Byzd_Key2).Custom((text, context) =>
            {
                try
                {
                    if (!CheckByzd(text))
                    {
                        context.AddFailure("只允许_by5至_by20字段定义");
                    }
                }
                catch
                {
                    context.AddFailure("只允许_by5至_by20字段定义");
                }
            });

            RuleFor(x => x.Byzd_Key3).Custom((text, context) =>
            {
                try
                {
                    if (!CheckByzd(text))
                    {
                        context.AddFailure("只允许_by5至_by20字段定义");
                    }
                }
                catch
                {
                    context.AddFailure("只允许_by5至_by20字段定义");
                }
            });

            RuleFor(x => x.Byzd_Key4).Custom((text, context) =>
            {
                try
                {
                    if (!CheckByzd(text))
                    {
                        context.AddFailure("只允许_by5至_by20字段定义");
                    }
                }
                catch
                {
                    context.AddFailure("只允许_by5至_by20字段定义");
                }
            });

            #region 收费设置校验

            RuleFor(x => x.LowestFees).Custom((text, context) =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        var i = Convert.ToDecimal(text);

                        if (i < 0)
                        {
                            context.AddFailure("请输入正数");
                        }
                    }
                }
                catch
                {
                    context.AddFailure("请输入合法的数字");
                }
            });

            RuleFor(x => x.Pzbjfz).Custom((text, context) =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        var i = Convert.ToDecimal(text);

                        if (i < 0)
                        {
                            context.AddFailure("请输入正数");
                        }
                    }
                }
                catch
                {
                    context.AddFailure("请输入合法的数字");
                }
            });



            #endregion

        }

        public bool CheckByzd(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text)) return false;

                if (!text.ToLower().Contains("_by"))
                {
                    return false;
                }

                int index = 0;
                if (!int.TryParse(text.ToLower().Replace("_by", string.Empty), out index))
                {
                    return false;
                }

                if (index > 20 || index <= 5)
                {
                    return false;
                }


                if (text.ToLower() == "_by1" || text.ToLower() == "_by2" || text.ToLower() == "_by3" || text.ToLower() == "_by4" || text.ToLower() == "_by5")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


    }
}
