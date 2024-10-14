using Aspose.Cells;
using AutoUpdaterDotNET;
using AWSV2.Models;
using AWSV2.Services;
using AWSV2.ViewModels.ShellViewHomeTemplate;
using Common.AJControls;
using Common.EF.Controllers;
using Common.HardwareSDKS.HIKVision;
using Common.HardwareSDKS.HuaXia;
using Common.HardwareSDKS.IDCardReader;
using Common.HardwareSDKS.VzClient;
using Common.LPR;
using Common.Model;
using Common.Models;
using Common.Platform;
using Common.Utility;
using Common.Utility.AJ;
using Common.Utility.AJ.EventAgregators;
using Common.Utility.AJ.Extension;
using Common.Utility.AJ.MobileConfiguration;
using Common.Utility.AJ.MobileConfiguration.WeightSystem;
using Common.ViewModels;
using Flee.PublicTypes;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Masuit.Tools;
using Masuit.Tools.Reflection;
using Masuit.Tools.Security;
using Masuit.Tools.Systems;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using SkiaSharp;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Yitter.IdGenerator;
using static Common.AJControls.AJMultiSelect;
using static Common.Platform.PlatformBase;
using static Common.Utility.AJ.MQTTService;
using static Common.Utility.PrintHelper;



namespace AWSV2.ViewModels
{

    public class ShellViewModel : Screen, IHandle<MainShellViewEvent>
    {
        #region 全局激活信息 --阿吉 2024年8月25日20点17分

        private CloudAPI.AppLicenseInfo _appLicenseInfo;
        public CloudAPI.AppLicenseInfo AppLicenseInfo
        {
            get => _appLicenseInfo;
            set => SetAndNotify(ref _appLicenseInfo, value);
        }

        #endregion

        #region 重量曲线图 --阿吉 2024年7月6日09点48分

        private WeightLineChartModel _weightLineChart;
        public WeightLineChartModel WeightLineChart
        {
            get => _weightLineChart;
            set => SetAndNotify(ref _weightLineChart, value);
        }

        #endregion

        #region 新增LPR集成 --阿吉2024年5月12日12点19分

        private LPRService _lprSvc;
        private GateService _gateSvc;
        private LEDService _ledSvc;
        private TTSService _ttsSvc;

        #endregion

        #region 新增扫码一体机设备 --阿吉 2024年4月30日11点12分

        public Common.HardwareSDKS.Models.YunMengZhiHuiDeviceInfo YunMengZhiHuiDevice { get; set; }

        #endregion

        #region 监控首页模板绑定 --阿吉 2024年4月4日08点35分


        public List<Common.HardwareSDKS.Models.DeviceInfo> LPRCameras;

        private Common.HardwareSDKS.Models.DeviceInfo _lprCameraOne;
        /// <summary>
        /// LPR识别相机1
        /// </summary>
        public Common.HardwareSDKS.Models.DeviceInfo LPRCameraOne
        {
            get
            {
                return _lprCameraOne;
            }
            set
            {
                SetAndNotify(ref _lprCameraOne, value);
            }
        }

        private Common.HardwareSDKS.Models.DeviceInfo _lprCameraTwo;
        /// <summary>
        /// LPR识别相机2
        /// </summary>
        public Common.HardwareSDKS.Models.DeviceInfo LPRCameraTwo
        {
            get
            {
                return _lprCameraTwo;
            }
            set
            {
                SetAndNotify(ref _lprCameraTwo, value);
            }
        }


        private ObservableCollection<Common.HardwareSDKS.Models.DeviceInfo> _monitorList;
        public ObservableCollection<Common.HardwareSDKS.Models.DeviceInfo> MonitorList
        {
            get
            {
                return _monitorList;
            }
            set
            {
                SetAndNotify(ref _monitorList, value);
            }
        }

        private HomeTemplateType? _templateType;
        public HomeTemplateType? TemplateType
        {
            get { return _templateType; }
            set
            {
                SetAndNotify(ref _templateType, value);
            }
        }

        #endregion

        #region 绑定道闸开关,和红绿灯图标按钮 --阿吉 2024年3月11日11点05分

        private bool _signalLightGreenOne;
        public bool SignalLightGreenOne
        {
            get
            {
                return _signalLightGreenOne;
            }
            set
            {
                SetAndNotify(ref _signalLightGreenOne, value);

            }
        }

        private bool _signalLightGreenTwo;
        public bool SignalLightGreenTwo
        {
            get
            {
                return _signalLightGreenTwo;
            }
            set
            {
                SetAndNotify(ref _signalLightGreenTwo, value);

            }
        }

        private bool _gateSwitchOne;
        public bool GateSwitchOne
        {
            get
            {
                return _gateSwitchOne;
            }
            set
            {
                SetAndNotify(ref _gateSwitchOne, value);

            }
        }

        private bool _gateSwitchTow;
        public bool GateSwitchTwo
        {
            get
            {
                return _gateSwitchTow;
            }
            set
            {
                SetAndNotify(ref _gateSwitchTow, value);
            }
        }

        #endregion

        #region 新增设备面板数据绑定  --阿吉 2024年3月5日18点09分

        private DeviceNoticeIcon _instrumentDeviceIcon;
        /// <summary>
        /// 仪表状态通知图标
        /// </summary>
        public DeviceNoticeIcon InstrumentDeviceIcon
        {
            get => _instrumentDeviceIcon;
            set => SetAndNotify(ref _instrumentDeviceIcon, value);
        }

        private static readonly SemaphoreSlim _deviceNotifyLocker = new SemaphoreSlim(1, 1);

        private ObservableCollection<DeviceNoticeIcon> _warningIcons = new ObservableCollection<DeviceNoticeIcon>();
        public ObservableCollection<DeviceNoticeIcon> WarningIcons
        {
            get
            {
                return _warningIcons;
            }
            set
            {
                SetAndNotify(ref _warningIcons, value);
            }
        }

        public bool CarImageVisible { get; set; }

        #endregion

        #region 基本参数

        private IEnumerable<PropertyInfo> _weighRecordModelProps;
        private List<DropdownOption> _sourceFields;
        private int _BhColumnIndex;
        private ObservableCollection<DynamicPoundFieldItem> _dynamicPoundFieldItems;
        public ObservableCollection<DynamicPoundFieldItem> DynamicPoundFieldItems
        {
            get
            {
                return _dynamicPoundFieldItems;
            }
            set
            {
                SetAndNotify(ref _dynamicPoundFieldItems, value);
            }
        }

        private string _weighingControl;
        public string WeighingControl
        {
            get
            {
                return _weighingControl;
            }
            set
            {
                SetAndNotify(ref _weighingControl, value);
            }
        }

        /// <summary>
        /// 称重修正功能配置 --阿吉 2023年10月13日17点21分
        /// </summary>
        private FastWeigthCorrectConfig _fastWeigthCorrectConfig;

        /// <summary>
        /// 过磅抓拍水印配置 --阿吉 2023年10月16日实践
        /// </summary>
        private SnapWatermarkConfig _snapWatermarkConfig;

        //log
        private static readonly AJLog4NetLogger log = AJLog4NetLogger.Instance();

        //加载其他窗口
        private IWindowManager _windowManager;


        ////页面上绑定的属性
        //private bool light1;

        //public bool Light1
        //{
        //    get { return light1; }
        //    set 
        //    { 
        //        light1 = value;
        //        if (value)
        //        {
        //            //控制继电器
        //            Console.WriteLine("继电器1号打开");
        //        }
        //        else
        //        {
        //            //控制继电器
        //            Console.WriteLine("继电器1号关闭");
        //        }
        //    }
        //}


        private string _weightStr1 = "";
        public string WeightStr1
        {
            get { return _weightStr1; }
            set
            {
                if (SetAndNotify(ref _weightStr1, value) && StateLabConnected)
                {
                    _eventAggregator?.Publish(new MainShellViewEvent(MainShellViewEvent.EventType.重量值更新)
                    {
                        Data = Tuple.Create(value, _stateLabConnected)
                    });
                }
            }
        } //重量数字
        public string WeightStr2 { get; set; } = ""; //重量数字
        public WeighFormViewModel WeighFormVM { get; set; } //过磅单表单
        public bool TwiceWeighing { get; set; } = true; //显示一个或者二个称重按钮
        public string StatusBar { get; set; } //状态栏

        public bool SelectedDevice1 { get; set; }
        public bool SelectedDevice2 { get; set; }

        //public string _TitleMessage = string.Empty;
        //public string TitleMessage
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(_TitleMessage))
        //        {
        //            _TitleMessage = Common.Utility.IOHelper.ReadTitleTxt();
        //        }
        //        return _TitleMessage;
        //    }

        //}

        BindableCollection<string> _LogContent = new BindableCollection<string>();
        public BindableCollection<string> LogContent
        {
            get
            {

                return _LogContent;
            }
            set
            {
                _LogContent = value;
            }

        }

        bool _EnableWeighing = false;
        public bool EnableWeighing
        {
            get
            {

                return _EnableWeighing;
            }
            set
            {

                _EnableWeighing = value;

            }
        }
        public bool EnableCheckSysLog { get; set; } = false;
        public bool EnableSetting { get; set; } = false;
        public bool EnableDataManage { get; set; } = true;
        public bool ManualWeighing { get; set; } // 手动称重(true)/自动称重标记(false)
        public string WeighModeIcon { get; set; }


        /// <summary>
        /// 是否显示 紧急过磅
        /// </summary>
        public Visibility ShowKeepOpen
        {
            get
            {
                var bol = Common.Share.VersionControl.CurrentVersion == Common.Share.VersionType.智能版;
                return bol ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 闸道常开
        /// </summary>
        private bool _keepOpen;
        public bool KeepOpen
        {
            get
            {
                return _keepOpen;
            }
            set
            {
                _keepOpen = value;
                if (_mobileConfigurationMgr != null)
                {
                    _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["KeepOpen"].Value = value.ToString();
                    _mobileConfigurationMgr.SaveSetting();
                }

                Properties.Settings.Default.Reload();

                if (value)
                {
                    HandToBarrier("upall");
                }
                else
                {
                    HandToBarrier("downall");
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
            }
        }

        private string _weighingUnit;
        public string WeighingUnit
        {
            get
            {
                return _weighingUnit;
            }
            set
            {
                _weighingUnit = value;
                _mainSetting.Settings["WeighingUnit"].Value = value;

            }
        }

        private string _weighName1;
        public string WeighName1
        {
            get
            {
                return _weighName1;
            }
            set
            {
                SetAndNotify(ref _weighName1, value);
            }
        }

        private string _weighName2;
        public string WeighName2
        {
            get
            {
                return _weighName2;
            }
            set
            {
                SetAndNotify(ref _weighName2, value);
            }
        }

        #region  IsStable 注释段
        ////私有
        //private bool isStable; //重量稳定标记
        //System.Timers.Timer isStable_timer = null;
        //System.Timers.Timer isStable_timer1 = null;

        ////在上面2个定时器值没变后，立即启用下面2个延迟指定定时器。这2个定时器仅仅是维持以上2个定时器执行期间，不允许串口继续传值过来。
        ////以免影响正常逻辑
        //System.Timers.Timer isDelay_timer = null;
        //System.Timers.Timer isDelay_timer1 = null;

        ////称重串口是否继续接收数据
        //bool SerialSleep = true;

        //public bool IsStable
        //{
        //    get { return isStable; }
        //    private set
        //    {

        //        isStable = value;

        //        if (value)
        //        {
        //            string StableWeightStr = WeightStr;
        //            System.Timers.Timer delayStableTimer = new System.Timers.Timer(Convert.ToDouble(ConfigurationManager.AppSettings["StableDelay"]) * 1000);
        //            isStable_timer = delayStableTimer;

        //            delayStableTimer.AutoReset = false;
        //            delayStableTimer.Start();
        //            delayStableTimer.Elapsed += (sender, args) =>
        //            {
        //                if (WeightStr.Equals(StableWeightStr))
        //                {
        //                    IsDelayStable = true;
        //                    log.Debug("延时稳定");

        //                    //稳定后立即阻止称重串口继续发送数据
        //                    SerialSleep = false;
        //                    //延时稳定后，要设置接收串口传递参数的阈值为不可接收状态，并且启用定时器在3秒钟之后恢复继续接收
        //                    isDelay_timer = new System.Timers.Timer(3 * 1000);
        //                    isDelay_timer.AutoReset = false;
        //                    isDelay_timer.Start();
        //                    isDelay_timer.Elapsed += (sender1, args1) =>
        //                    {
        //                        //3秒钟后允许串口继续接收数据
        //                        SerialSleep = true;
        //                    };
        //                }
        //                else
        //                {
        //                    IsDelayStable = false;
        //                }
        //            };
        //        }
        //        else
        //        {

        //            ////增加Timer的停止操作。防止上一次为true的时候延迟发出的错误的状态
        //            ////2022-09-04 wanghu
        //            //if (isStable_timer != null)
        //            //{
        //            //    isStable_timer.Stop();//立马停止计时器，防止错误状态的产生
        //            //    isStable_timer = null;
        //            //}

        //            //if (isDelay_timer != null)
        //            //{
        //            //    isDelay_timer.Stop();//立马停止计时器，恢复串口继续传值
        //            //    isDelay_timer = null;
        //            //}

        //            IsDelayStable = false;

        //        }
        //        //非零稳定后创建计时器线程开始计时【延时非零稳定】的时间
        //        if (value
        //            && (Convert.ToDecimal(WeightStr) > Convert.ToDecimal(ConfigurationManager.AppSettings["MinSlotWeight"])))
        //        {
        //            string StableWeightStr = WeightStr;
        //            System.Timers.Timer delayNZStableTimer = new System.Timers.Timer(Convert.ToDouble(ConfigurationManager.AppSettings["StableDelay"]) * 1000);
        //            isStable_timer1 = delayNZStableTimer;

        //            delayNZStableTimer.AutoReset = false;
        //            delayNZStableTimer.Start();
        //            delayNZStableTimer.Elapsed += (sender, args) =>
        //            {
        //                if (WeightStr.Equals(StableWeightStr))
        //                {
        //                    IsDelayNZStable = true;
        //                    if (WeighingTimes != 0)
        //                    {
        //                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["TTS3Enable"]) && ManualWeighing)
        //                            TTSHelper.TTS(_weighingRecord, ConfigurationManager.AppSettings["TTS3Text"]);

        //                        //2022-09-01被注释
        //                        if (ConfigurationManager.AppSettings["LED2Enable"] == "True" && ManualWeighing)
        //                        {
        //                            using (StreamWriter sw = new StreamWriter("./Data/LedText/isDelayNZStable.txt", false, Encoding.Default))
        //                            {
        //                                if (_weighingRecord.Ch != null) sw.WriteLine(_weighingRecord.Ch);
        //                                sw.Write("请停稳车辆并且熄火");
        //                            }
        //                            IPCHelper.SendMsgToApp("大屏幕", IPCHelper.IS_STABLE);

        //                        }

        //                        IPCHelper.SendMsgToApp("车牌识别", IPCHelper.IS_STABLE, StableWeightStr);

        //                    }
        //                    log.Debug("延时非零稳定");

        //                    //稳定后立即阻止称重串口继续发送数据
        //                    SerialSleep = false;
        //                    //延时稳定后，要设置接收串口传递参数的阈值为不可接收状态，并且启用定时器在3秒钟之后恢复继续接收
        //                    isDelay_timer1 = new System.Timers.Timer(3 * 1000);
        //                    isDelay_timer1.AutoReset = false;
        //                    isDelay_timer1.Start();
        //                    isDelay_timer1.Elapsed += (sender1, args1) =>
        //                    {
        //                        //3秒钟后允许串口继续接收数据
        //                        SerialSleep = true;
        //                    };


        //                }
        //                else
        //                {
        //                    ////增加Timer的停止操作。防止上一次为true的时候延迟发出的错误的状态
        //                    ////2022-09-04 wanghu
        //                    //if (isStable_timer1 != null)
        //                    //{
        //                    //    isStable_timer1.Stop();//立马停止计时器，防止错误状态的产生
        //                    //    isStable_timer1 = null;
        //                    //}

        //                    //if (isDelay_timer1 != null)
        //                    //{
        //                    //    isDelay_timer1.Stop();//立马停止计时器，恢复串口继续传值
        //                    //    isDelay_timer1 = null;
        //                    //}

        //                    IsDelayNZStable = false;
        //                }
        //            };
        //        }
        //    }
        //}
        #endregion 

        //私有
        private bool isStable; //重量稳定标记
        public bool IsStable
        {
            get { return isStable; }
            private set
            {
                isStable = value;

                if (value)
                {
                    //Light1 = true;

                    string StableWeightStr = WeightStr;
                    System.Timers.Timer delayStableTimer = new System.Timers.Timer(Convert.ToDouble(_mainSetting.Settings["StableDelay"]?.Value ?? "1") * 1000);
                    delayStableTimer.AutoReset = false;
                    delayStableTimer.Start();
                    delayStableTimer.Elapsed += (sender, args) =>
                    {
                        if (WeightStr.Equals(StableWeightStr))
                        {
                            SetPlate();
                            IsDelayStable = true;
                            log.Debug("延时稳定");
                        }
                        else
                        {
                            IsDelayStable = false;
                            //log.info()...
                        }
                    };
                }
                else
                {
                    //Light1 = false;

                    IsDelayStable = false;
                }
                //非零稳定后创建计时器线程开始计时【延时非零稳定】的时间
                if (value
                    && (Convert.ToDecimal(WeightStr) > Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0")))
                {
                    CarImageVisible = true;
                    string StableWeightStr = WeightStr;
                    System.Timers.Timer delayNZStableTimer = new System.Timers.Timer(Convert.ToDouble(_mainSetting.Settings["StableDelay"]?.Value ?? "1") * 1000);
                    delayNZStableTimer.AutoReset = false;
                    delayNZStableTimer.Start();
                    delayNZStableTimer.Elapsed += (sender, args) =>
                    {
                        if (WeightStr.Equals(StableWeightStr))
                        {
                            SetPlate();
                            IsDelayNZStable = true;

                            log.Debug("延时非零稳定");
                        }
                        else
                        {
                            IsDelayNZStable = false;
                        }
                    };
                }
                else
                {
                    CarImageVisible = false;
                }
            }
        }
        public bool IsDelayStable { get; private set; } //重量延时稳定标记
        public bool IsDelayNZStable { get; private set; } //重量延时非零稳定标记
        public List<string> WeightDataList { get; private set; } = new List<string>(); //判断重量稳定的数组

        /// <summary>
        /// 称重串口
        /// </summary>
        private WeighSerialPortService _weighSerialPortService;

        /// <summary>
        /// 二维码串口1
        /// </summary>
        private QRCodeSerialPortService _qrCodeSerialPortService;
        /// <summary>
        /// 二维码串口2
        /// </summary>
        private QRCodeSerialPortService _qrCodeSerialPortService2;


        public SerialPort LEDSerialPort { get; private set; } = new SerialPort();//大屏幕串口
        public bool LEDSerialPortEnable { get; private set; } //大屏幕串口已打开的标识，用于判断timer中是否发送串口数据，不要每次都读配置文件
        public SerialPort QRPort { get; private set; } = new SerialPort();//二维码串口
        public SerialPort QRPort2 { get; private set; } = new SerialPort();//二维码串口
        public string _qrCodeRawData { get; private set; } = string.Empty;

        private int _weighingTimes;
        // 0:称重完成, ==1:第一次过磅, ==2:第二次过磅 
        public int WeighingTimes
        {
            get { return _weighingTimes; }
            private set
            {
                _weighingTimes = value;

            }
        }
        //public ICCard IC { get; private set; } = new ICCard(); //桌面刷卡器
        public bool IsICBusy { get; private set; } = false;
        public bool NoZeroFlag { get; private set; } = true;
        public bool OverWeight { get; private set; } = false;
        public double OverWeightCount { get; private set; } = 0;
        public bool SendCmdOnce { get; private set; } = true;

        private List<string> _weightFormTemplateSheetsNameSource;

        private ObservableCollection<string> _weightFormTemplateSheetsName;
        public ObservableCollection<string> WeightFormTemplateSheetNames
        {
            get
            {
                return _weightFormTemplateSheetsName;
            }
            set
            {
                SetAndNotify(ref _weightFormTemplateSheetsName, value);
            }
        }


        private string _selectedWeightFormTemplateSheet;
        public string SelectedWeightFormTemplateSheet
        {
            get { return _selectedWeightFormTemplateSheet; }
            set
            {
                _selectedWeightFormTemplateSheet = value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _worksheet = _wss[_selectedWeightFormTemplateSheet];
                }
            }
        }



        public string LastPlate { get; private set; }
        /// <summary>
        /// 当前磅秤的状态：Waiting 等待称重，ReadyWeigh 准备称重，WeighBegin 开始称重，Weighing 称重中，WeighEnd 称重结束
        /// </summary>
        public Common.Model.Custom.WeighStatus CurrentStatus { get; private set; } = Common.Model.Custom.WeighStatus.Waiting;

        /// <summary>
        /// 首页是否显示日志，默认显示。显示日志则不显示进场图片那些。
        /// </summary>
        private bool _showLog;
        public bool ShowLog
        {
            get
            {
                return _showLog;
            }
            set
            {
                _showLog = value;
                if (_mobileConfigurationMgr != null)
                {
                    _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["ShowLog"].Value = value.ToString();
                    _mobileConfigurationMgr.SaveSetting();
                }
            }
        }
        /// <summary>
        /// 首页是否显示图片，显示图片，就不显示日志。同理相反
        /// </summary>
        public bool ShowPic
        {
            get
            {
                return !ShowLog;
            }
        }

        private string weightStr = string.Empty;//重量数字，等于WeightStr1或WeightStr2
        public string WeightStr
        {
            get
            {
                return weightStr;
            }
            private set
            {
                //两次重量数据不同即不稳定
                if (value != weightStr)
                {
                    IsStable = false;
                }
                //3次重量数据相同即稳定
                WeightDataList.Add(value);
                if (WeightDataList.Count > 2)
                {
                    if (WeightDataList[0].Equals(WeightDataList[1]) &&
                        WeightDataList[0].Equals(WeightDataList[2]))
                        IsStable = true;

                    WeightDataList.Clear();
                }

                //2022-12-06 新增，超载启用报警，并且不记录超载日志的设定下，获取超载阈值匹配当前的磅秤重量，如果超载了，就禁用称重按钮。
                //无论自动模式还是手动模式下
                try
                {
                    if (((_mainSetting.Settings["OverloadWarning"]?.Value ?? string.Empty) != "0")) //启用超载报警，并且不记录超载日志数据
                    {
                        var overloadLog = _mainSetting.Settings["OverloadLog"]?.Value ?? string.Empty;//不记录超载日志
                        if (overloadLog == "0")
                        {
                            var OverloadWarningWeight = _mainSetting.Settings["OverloadWarningWeight"]?.Value ?? "0";
                            EnableWeighing = Convert.ToDecimal(value) < Convert.ToDecimal(OverloadWarningWeight);
                        }
                    }
                }
                catch
                {
                    EnableWeighing = true;
                }


                //重量大于最小称重重量时取消自动复位计时器
                try
                {

                    if (Convert.ToDecimal(value) > Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0"))
                    {
                        var ch = _weighingRecord.Ch;//Globalspace._plateNo
                        CarImageVisible = true;

                        //当车号不存在的时候，并且磅秤又在称重，那么就要显示日志界面，让日志显示 称重未准备就绪，请检查仪表重量是否大于回零重量
                        if (!IsStable & string.IsNullOrWhiteSpace(ch))
                        {
                            WeighFormViewVisible = Visibility.Visible;
                            DataFormViewVisible = Visibility.Hidden;
                        }
                        else
                        {

                            if ((_mainSetting.Settings["Barrier"]?.Value ?? string.Empty) == "1")
                            {
                                //当前磅秤稳定的时候，且正在称重，那么在首页日志里要显示 重量稳定
                                if (IsStable &&
                                (CurrentStatus == Common.Model.Custom.WeighStatus.Weighing || CurrentStatus == Common.Model.Custom.WeighStatus.ReadyWeigh) &&
                                !EditLogs($"检测到车牌：{ch} 正在等待稳定", $"检测到车牌：{ch} 重量稳定"))//稳定
                                {
                                    // ShowLogs($"检测到车牌：{Globalspace._plateNo} 重量稳定");//这里只提示一次
                                }
                                //当前磅秤不稳定的时候，且正在称重，那么在首页日志里要显示 等待稳定。
                                if (!IsStable &&
                                    (CurrentStatus == Common.Model.Custom.WeighStatus.Weighing || CurrentStatus == Common.Model.Custom.WeighStatus.ReadyWeigh) &&
                                    !EditLogs($"检测到车牌：{ch} 重量稳定", $"检测到车牌：{ch} 正在等待稳定"))//不稳定
                                {
                                    ShowLogs($"检测到车牌：{ch} 正在等待稳定");//这里只提示一次
                                }
                            }
                            else
                            {
                                //当前磅秤稳定的时候，且正在称重，那么在首页日志里要显示 重量稳定
                                if (IsStable &&
                                (CurrentStatus == Common.Model.Custom.WeighStatus.Weighing) &&
                                !EditLogs($"检测到车牌：{ch} 正在等待稳定", $"检测到车牌：{ch} 重量稳定"))//稳定
                                {
                                    // ShowLogs($"检测到车牌：{Globalspace._plateNo} 重量稳定");//这里只提示一次
                                }
                                //当前磅秤不稳定的时候，且正在称重，那么在首页日志里要显示 等待稳定。
                                if (!IsStable &&
                                    (CurrentStatus == Common.Model.Custom.WeighStatus.Weighing) &&
                                    !EditLogs($"检测到车牌：{ch} 重量稳定", $"检测到车牌：{ch} 正在等待稳定"))//不稳定
                                {
                                    ShowLogs($"检测到车牌：{ch} 正在等待稳定");//这里只提示一次
                                }
                            }

                            //当前磅秤不稳定的时候，且已经称重结束，那么在首页日志里要显示 正在下磅
                            if (!IsStable & CurrentStatus == Common.Model.Custom.WeighStatus.WeighEnd)//不稳定
                            {
                                ShowLogs($"检测到车牌：{ch} 正在下磅");//这里只提示一次

                            }
                        }

                        if (cancelWeighingTimer != null)
                        {
                            cancelWeighingTimer.Stop();
                            //cancelWeighingTimer = null;
                        }
                    }
                    else
                    {
                        CarImageVisible = false;
                    }
                }
                catch { }

                weightStr = value;
            }
        }

        private string getPlateNo; //通过车牌号检测称重按钮是否有效的标记
        public string GetPlateNo
        {
            get
            {
                return getPlateNo;
            }
            set
            {

                if (!string.IsNullOrEmpty(value))
                {
                    WeighFormViewVisible = Visibility.Visible;
                    DataFormViewVisible = Visibility.Hidden;
                }

                if (getPlateNo != value) //页面上车号栏值有改动
                {
                    getPlateNo = value;

                    //页面上改动为非空时进行查询
                    if (getPlateNo != null)
                    {
                        //点击时，getPlateNo为""，删除输入的字符串后，getPlateNo为"",此时需要禁用称重按钮
                        //但不能刷新过磅单页面，所以不能把getPalteNo != ""写到上面的的if中
                        if (getPlateNo == "")
                        {
                            WeighingTimes = 0;
                            return;
                        }

                        RefreshWeightRecordModel(value);

                    }
                }
            }
        }

        #region 新增首页界面定制面板功能  --阿吉 2024年1月2日17点53分

        //目前首页UI已无此功能, 暂时注释 --阿吉 2024年6月25日12点31分
        //public void CarSummaryReUse(CarSummaryModel.CarSummaryItem item)
        //{
        //    var model = Common.Data.SQLDataAccess.LoadWeighingRecordByAutoNo(item.AutoNo);
        //    var vm = AJUtil.TryGetJSONObject<WeighingRecord>(AJUtil.AJSerializeObject(model));

        //    _eventAggregator.PublishOnUIThread(new MainShellViewEvent(MainShellViewEvent.EventType.刷新榜单)
        //    {
        //        Data = vm
        //    });

        //}

        /// <summary>
        /// 目前首页UI已无此功能, 暂时注释 --阿吉 2024年6月25日12点31分
        /// </summary>
        /// <param name="item"></param>
        public void CarSummaryOpenHistory(CarSummaryModel.CarSummaryItem item)
        {

        }

        public void ToggleCarSummaryMode()
        {
            _carSummary.ToggleMode();
        }

        #endregion


        private async void RefreshWeightRecordModel(string value)
        {
            //1.先查询是否有此车号
            using var db = AJDatabaseService.GetDbContext();
            using var wrCtrl = new WeighingRecordController();
            var ch = getPlateNo;
            var cm = await db.Cars.AsNoTracking().FirstOrDefaultAsync(p => p.PlateNo == ch);

            var pages = await wrCtrl.Pages(new WeighingRecordSearchForm
            {
                Limit = 1,
                Filter = new JObject
                    {
                        {nameof(WeighingRecord.By20), false },
                        {nameof(WeighingRecord.Ch), _weighingRecord.Ch },
                        {nameof(WeighingRecord.IsFinish), false }
                    }
            });

            var wrm = AJAutoMapperService.Instance().Mapper
                .Map<Common.EF.Tables.WeighingRecord, Common.Models.WeighingRecord>(pages.Data.FirstOrDefault());

            _weighingRecord.Ch = value;
            if (cm != null) //2.如果有此车号，根据车号查询称重记录表中查未完成的称重记录
            {
                //是否混合模式
                var weiModel = _mainSetting.Settings["WeighingMode"].TryGetString("Twice");
                TwiceWeighing = weiModel.Equals("Twice");

                if (wrm == null) //没有找到未完成的称重记录，是第一次称重
                {
                    //修改车号后，如果是从第二次称重的车号修改为第一次称重的车号，需要清空页面
                    if (_weighingRecord.Bh != null)
                    {
                        _weighingRecord = new WeighingRecord()
                        {
                            Ch = value,
                        };
                    }

                    WeighingTimes = 1;
                    log.Debug("查询到车号，第一次称重:" + cm.PlateNo);
                }
                else //有未完成的称重记录，是第二次称重
                {
                    TwiceWeighing = weiModel.Equals("Twice");
                    if (wrm.EntryTime != DateTime.MinValue)
                    { //如果有记录第一次进入称重场地的时间
                        var validTime = _mainSetting.Settings["WeighingValidTime"]?.Value ?? "0";
                        TimeSpan sp = DateTime.Now.Subtract(wrm.EntryTime);

                        //if (sp.TotalHours >= int.Parse(validTime))
                        if (sp.TotalHours >= double.Parse(validTime))
                        {//有记录，并且已经超时了，那么就将此未完成的记录设置为完成。并且将称重记录重置为1，从第一次开始。
                            wrm.IsFinish = true;
                            var autoNo = wrm.AutoNo;
                            await db.WeighingRecords.Where(p => p.AutoNo == autoNo).ExecuteUpdateAsync(sp => sp.SetProperty(p => p.IsFinish, true));
                            _weighingRecord = new WeighingRecord()
                            {
                                Ch = value,
                            };
                            WeighingTimes = 1;
                            log.Debug($"查询到车号{cm.PlateNo}，第一次称重，但是超过有效期{validTime}，已经重置为第一次称重。");
                        }
                        else
                        {
                            WeighingTimes = 2;
                            _weighingRecord = wrm;
                            log.Debug("查询到车号，第二次称重:" + cm.PlateNo);
                        }

                    }
                    else
                    {

                        WeighingTimes = 2;
                        _weighingRecord = wrm;
                        log.Debug("查询到车号，第二次称重:" + cm.PlateNo);
                    }
                }

                if (cm.CarOwner != 0)
                {
                    var cid = cm.CarOwner.GetValueOrDefault();
                    _weighingRecord.Kh3 = await db.Customers.Where(p => p.Id == cid)
                        .Select(p => p.Name).FirstOrDefaultAsync();
                }
                if (cm.VehicleWeight > 0) _weighingRecord.Pz = cm.VehicleWeight;
                WeighFormVM.Refresh(ref _weighingRecord);
            }
            else //3.如果没此车号，说明是新车，第一次称重    
            {
                //是否混合模式
                var weiModel = _mainSetting.Settings["WeighingMode"].TryGetString("Twice");
                TwiceWeighing = weiModel.Equals("Twice");

                WeighingTimes = 1;

                if ((_mainSetting.Settings["车牌识别"]?.Value ?? string.Empty) != "0" //车牌识别启用时，要更新界面
                    && Globalspace._getPlateFromLpr)    //只有车牌识别传过来的才更新，排除掉手动输入的情况
                {
                    Globalspace._getPlateFromLpr = false;

                }

                if (wrm == null) //没有找到未完成的称重记录，是第一次称重
                {
                    //修改车号后，如果是从第二次称重的车号修改为第一次称重的车号，需要清空页面
                    if (_weighingRecord.Bh != null)
                    {
                        _weighingRecord = new WeighingRecord()
                        {
                            Ch = value,
                        };
                    }
                }
                WeighFormVM.Refresh(ref _weighingRecord);
            }
        }

        private bool readyToWeigh;  //初始化页面，激活车牌识别，可以进行下一次称重
        public bool ReadyToWeigh
        {
            get { return readyToWeigh; }
            private set
            {
                readyToWeigh = value;
                if (value)
                {

                    Globalspace._isManual = false;
                    Globalspace._plateNo = Globalspace._lprDevNo = string.Empty;

                    //SetBtnTExt(1);//重置按钮的显示文本，因为光栅遮挡后 这里的文本会改编成 “一次称重(光栅1遮挡中)”，称完后要改成“一次称重”
                    try
                    {
                        _weighingRecord = new WeighingRecord();
                        WeighFormVM.Refresh(ref _weighingRecord, true);
                        WeighingTimes = 0;
                        IsICBusy = false;
                        IsDelayNZStable = false;
                        OverWeight = false;
                        SendCmdOnce = true;
                        StatusBar = "";
                        //清空
                        LogContent.Clear();
                        //隐藏日志面板，显示统计面板
                        WeighFormViewVisible = Visibility.Hidden;
                        DataFormViewVisible = Visibility.Visible;

                        //当弹出收费对话框时，标记为是否需要保存称重数据。true不需要保存称重记录，false需要保存收费记录
                        //该状态仅在逃费的时候起作用


                        //2023-05-11 双向模式下，并且小于最小重量，并且启用了LPR识别的时候，每次刷新会重置LPR的可识别车牌状态为可识别。否则不可识别

                        if ((_mainSetting.Settings["车牌识别"]?.Value ?? string.Empty) != "0")
                        {
                            if ((_mainSetting.Settings["Barrier"]?.Value ?? string.Empty) == "1")
                            {
                                // IPCHelper.SendMsgToApp("车牌识别", IPCHelper.READY_TO_WEIGH);
                                LPR_READY_TO_WEIGH();
                            }
                            else if (_mainSetting.Settings["Barrier"].Value.TryGetInt() == 3
                                || Math.Abs(WeightStr.TryGetDecimal()) <= Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0"))
                            {
                                //IPCHelper.SendMsgToApp("车牌识别", IPCHelper.READY_TO_WEIGH);
                                CarImageVisible = false;
                                LPR_READY_TO_WEIGH();
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        ShowLogs($"ReadyToWeigh 失败：{e.Message}");
                        log.Error($"ReadyToWeigh 失败", e);
                    }
                }
                else
                {
                    log.Debug("正在称重中");
                }
            }
        }



        private byte[] hexWeight = new byte[12]; //发送大屏幕串口数据的buffer
        private WeighingRecord _weighingRecord = new WeighingRecord(); //当前称重记录表单
        private DispatcherTimer timer = new DispatcherTimer(); //定义定时器，进行周期的查询车牌号操作
        private DispatcherTimer cancelWeighingTimer; //自动复位计时器，刷卡不上秤倒计时结束后初始化称重操作
        private WorksheetCollection _wss;
        private Worksheet _worksheet; //过磅单模板

        private DataGrid _weightRecordDataGrid;
        public void WeightRecordDataGridLoaded(object sender, RoutedEventArgs e)
        {
            _weightRecordDataGrid = (DataGrid)sender;
        }

        public AJHomeDataGridSummary DataGridSummary
        {
            get; set;
        }
        public AJHomeDataGridSummary TabSummary
        {
            get; set;
        }

        public DataTable RecordDT { get; private set; } //经过转换的称重记录表格，用来显示到列表、导出到excel

        public int SelectedTotalWeighingCount { get; private set; }
        public decimal SelectedTotalJz { get; private set; }
        public decimal SelectedTotalSz { get; private set; }


        public string FirstImg { get; set; } = "/Resources/Img/ce.png";
        public string SecondImg { get; set; } = "/Resources/Img/cf.png";
        public string ThirdImg { get; set; } = "/Resources/Img/cf.png";
        #endregion

        #region 新增首页面板定制功能  --阿吉 2024年1月2日16点20分

        private CarSummaryModel _carSummary;
        public CarSummaryModel CarSummary
        {
            get
            {
                return _carSummary;
            }
            set
            {
                SetAndNotify(ref _carSummary, value);
            }
        }

        #endregion

        #region 系统验证模块参数 by：WH 2022-07-01



        public System.Windows.Visibility WeighFormViewVisible { get; set; } = System.Windows.Visibility.Hidden;

        public System.Windows.Visibility _DataFormViewVisible = System.Windows.Visibility.Visible;

        public System.Windows.Visibility DataFormViewVisible
        {

            get
            {
                return _DataFormViewVisible;
            }
            set
            {
                // 当前版本首页已无该功能,注释调 --阿吉 2024年6月25日17点12分
                //if (_DataFormViewVisible == System.Windows.Visibility.Visible)
                //{
                //    //刷新模型
                //    if (DataFormVM == null)
                //    {
                //        DataFormVM = new DataFormViewModel(_eventAggregator, ref _mobileConfigurationMgr);
                //    }
                //}
                _DataFormViewVisible = value;
                // RaisePropertyChanged("Visibility");
            }
        }
        public object DataFormVM { get; set; }
        #endregion

        #region 首页分割控件的位置
        System.Windows.GridLength _SplitterTopPoint = new GridLength(780, GridUnitType.Star);
        public System.Windows.GridLength SplitterTopPoint
        {
            get
            {
                //var point = _mainSetting.Settings["SplitterTopPoint"].Value;
                //if (!string.IsNullOrWhiteSpace(point))
                //{
                //    _SplitterTopPoint = new GridLength(double.Parse(point), GridUnitType.Star);
                //}

                return _SplitterTopPoint;
            }
            set
            {
                _SplitterTopPoint = value;
            }
        }

        System.Windows.GridLength _SplitterBottomPoint = new GridLength(223, GridUnitType.Star);
        public System.Windows.GridLength SplitterBottomPoint
        {

            get
            {
                //var point = _mainSetting.Settings["SplitterBottomPoint"].Value;
                //if (!string.IsNullOrWhiteSpace(point))
                //{
                //    _SplitterBottomPoint = new GridLength(double.Parse(point), GridUnitType.Star);
                //}

                return _SplitterBottomPoint;
            }
            set
            {
                _SplitterBottomPoint = value;

            }
        }
        #endregion

        #region 底部查询列表相关属性 by:WH 2023-05-24
        
        public bool EnablePrintList { get; set; } = false;
        public bool EnableExportList { get; set; } = false;
        public bool EnableDeleteList { get; set; } = false;
        
        public bool EnableDeleteSelected { get; set; } = false;

        #endregion

        DispatcherTimer minSoftTTSTimer = new DispatcherTimer();
        DispatcherTimer normalTTSTimer = new DispatcherTimer();
        DispatcherTimer coverTTSTimer = new DispatcherTimer();
        public bool IsMinsoftCover { get; set; } = false;

        private bool _stateLabConnected;
        public bool StateLabConnected
        {
            get
            {
                return _stateLabConnected;
            }
            set
            {
                if (SetAndNotify(ref _stateLabConnected, value))
                {
                    _eventAggregator?.Publish(new MainShellViewEvent(MainShellViewEvent.EventType.重量值更新)
                    {
                        Data = Tuple.Create(_weightStr1, value)
                    });
                }
            }
        }

        private string _currentUserName;
        public string CurrentUserName
        {
            get => _currentUserName;
            set => SetAndNotify(ref _currentUserName, value);
        }

        //public string ProvicerTitle
        //{
        //    get
        //    {
        //        return AWSV2.Globalspace.ProvicerTitle;
        //    }
        //}
        private string weighingMode;                //称重模式 Once / Twice
        public string WeighingMode
        {
            get { return weighingMode; }
            set
            {
                weighingMode = value;
                if (_mobileConfigurationMgr != null)
                {
                    _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["WeighingMode"].Value = value;
                    _mobileConfigurationMgr.SaveSetting();
                }

                //称重模式：单次称重/二次称重
                TwiceWeighing = _mainSetting.Settings["WeighingMode"].TryGetString().Equals("Twice");
            }
        }
        //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11100);


        // 手动皮重毛重功能 --阿吉 2023年10月6日18点02分
        private bool _manualPZOrMZ;
        private bool _manualPZOrMZBtnsVisible;
        public bool ManualPZOrMZBtnsVisible
        {
            get
            {
                return _manualPZOrMZBtnsVisible;
            }
            set
            {
                _manualPZOrMZBtnsVisible = value;
                NotifyOfPropertyChange(nameof(ManualPZOrMZBtnsVisible));
            }
        }
        private AppSettingsSection _mainSetting;
        private MQTTService _mqttSvc;
        private MobileConfigurationMgr _mobileConfigurationMgr;
        private AJMQTTAppointProcessor _ajmqttAppointProcessor;

        private IEventAggregator _eventAggregator;


        private System.Timers.Timer _reefRetryTimer;
        private static readonly object _locker = new object();
        private static bool _restartRequested;
        private Grid _mainGrid;

        private DependencyPropertyDescriptor _descriptor
            = DependencyPropertyDescriptor.FromProperty(Button.IsEnabledProperty, typeof(Button));

        //构造函数，初始化
        public ShellViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            this.AppLicenseInfo = CloudAPI.APPLICENSEINFO;
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            _windowManager = windowManager;

            Globalspace.ShellViewModel = this;
            CurrentUserName = AWSV2.Globalspace._currentUser.LoginId;

            // 必须初始化的时候就得确定模板类型,不然空白
            TemplateType = (HomeTemplateType)Enum.Parse(typeof(HomeTemplateType),
                SettingsHelper.AWSV2Settings.Settings["HomeTemplateType"]?.Value
                ?? HomeTemplateType.Default.ToString());

            ShellViewModelSetup();

            // 配置在线更新
            AutoUpdater.Synchronous = true;
            AutoUpdater.ReportErrors = true;
            AutoUpdater.DownloadPath = Path.Combine(Path.GetTempPath(), "avsUpdateTemp",
                DateTime.Now.ToString("MMdd_HHmmss"));
            //AutoUpdater.SetOwner(Application.Current.MainWindow);
            AutoUpdater.ApplicationExitEvent += () =>
            {
                if (_mainSetting.Settings["AutoLogin"].TryGetBoolean() == false)
                {
                    SettingsHelper.UpdateAWSV2("AutoLogin", true.ToString());
                }

                var updateAssistantDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateAssistant");
                var porcess = AJUtil.CreateCommand(Path.Combine(updateAssistantDir, "UpdateAssistant.exe"), "uninstall", workingDirectory: updateAssistantDir, noWindow: false);

                porcess.Start();

                porcess.WaitForExit();

                ConfirmClose();

                porcess = AJUtil.CreateCommand(Path.Combine(updateAssistantDir, "UpdateAssistant.exe"), "kill", workingDirectory: updateAssistantDir, noWindow: false);

                porcess.Start();

            };
        }

        private void ShellViewModelSetup()
        {
            WList = new List<WeighingRecord>();
            WeightLineChart = new WeightLineChartModel(() =>
            {
                return WeightStr1.TryGetDecimal();
            });
            _weighRecordModelProps = typeof(Common.Models.WeighingRecord).GetRuntimeProperties();
            DataGridSummary = new AJHomeDataGridSummary();
            TabSummary = new AJHomeDataGridSummary();

            InitWeightStableWorker();

            InitMinSlotWeightCheckWorker();

            InitGateRasterCheckWorker();

            var worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                _mobileConfigurationMgr = new MobileConfigurationMgr();
                _ajmqttAppointProcessor = new AJMQTTAppointProcessor(Globalspace._weightFormTemplatePath);
            };
            worker.RunWorkerCompleted += (s, e) =>
            {
                ConnectToMQTTAsync();
                MobileConfigurationMgrSetup();

                _lprSvc = new LPRService(_eventAggregator);
                _gateSvc = new GateService(_eventAggregator);
                _ledSvc = new LEDService(_eventAggregator);
                _ttsSvc = new TTSService(_eventAggregator);

                _weighSerialPortService = new WeighSerialPortService(_eventAggregator);
                _qrCodeSerialPortService = new QRCodeSerialPortService(_eventAggregator);
                _qrCodeSerialPortService2 = new QRCodeSerialPortService(_eventAggregator);

                RefreshAWSV2Setting(false);

                // 加载完配置后才开启回零重量检测worker
                _minSlotWeightCheckWorker.RunWorkerAsync();

                Common.Platform.PlatformManager.WindowManager = this._windowManager;
                var platformName = _mainSetting.Settings["PlatformName"].TryGetString("Default");
                Common.Platform.PlatformManager.Instance.Init(platformName);

                Common.Platform.PlatformManager.ActiveCode = CloudAPI.GetLocalActiveCode(_mainSetting, out _);

                if (_manualPZOrMZ)
                {
                    Btn1Text = "毛重";
                    Btn1Text = "皮重";
                }

                CarSummary = new CarSummaryModel();

                Init();

                //StartQueryListTimer();

            };

            worker.RunWorkerAsync();

        }

        private void MobileConfigurationMgrSetup()
        {
            var worker = new BackgroundWorker();

            worker.DoWork += (ws, we) =>
            {

                _mobileConfigurationMgr.GetLatestVersionHandler
                = () => Application.ResourceAssembly.GetName().Version.ToString();

                _mobileConfigurationMgr.OnConfigUpdated += (s, e) =>
                {
                    RefreshAWSV2Setting(false);
                };

                _mobileConfigurationMgr.LoginConfigChangedHandler = (e) =>
                {
                    if (e.autoLogin)
                    {
                        Properties.Settings.Default.LastLoginId = Globalspace._currentUser.LoginId;
                        Properties.Settings.Default.LastUserPwd = Globalspace._currentUser.LoginPwd;
                        Properties.Settings.Default.Save();
                    }
                };

                _mobileConfigurationMgr.WeightFormTemplatePath = Globalspace._weightFormTemplatePath;
                InitMobileConfigCMDMaps();
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    ShowLogs($"MobileConfigurationMgrSetup 异常:{e.Error.Message}");
                }
            };

            worker.RunWorkerAsync();

        }

        private void Init()
        {
            _currentLPRCameraDevice = new Common.HardwareSDKS.Models.DeviceInfo()
            {
                CarIdentificationPlateResult = new Common.HardwareSDKS.Models.CarIdentificationResult()
            };

            #region 超时取消称重定时器初始化

            _weighCancelTimer = new System.Timers.Timer
            {
                Interval = TimeSpan
                .FromSeconds(_mainSetting.Settings["CancelWeighingTime"].TryGetDouble(60)).TotalMilliseconds
            };
            _weighCancelTimer.Elapsed += (_, __) =>
            {
                _weighCancelTimer.Stop();
                _weightStableWorker.CancelAsync();
            };

            #endregion

            var weightFormWorker = new BackgroundWorker();
            weightFormWorker.DoWork += (s, e) =>
            {
                var result = new ProcessResult();

                if (!File.Exists(Globalspace._weightFormTemplatePath))
                {
                    result.SetError("表单模板不存在，请联系软件供应商。");
                    return;
                }

                //加载过磅单到内存
                _wss = new Workbook(Globalspace._weightFormTemplatePath).Worksheets;

                _weightFormTemplateSheetsNameSource = _wss.Select(p => p.Name).ToList();
                result.SetSuccess();

                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["PrintTemplate"].Value = "1";
                _mobileConfigurationMgr.SaveSetting();

                e.Result = result;

            };

            weightFormWorker.RunWorkerCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    ShowLogs($"Init Fail:{e.Error.Message}");
                }
                var result = e.Result as ProcessResult;
                if (result.Success)
                {
                    if (result.Message != "ok")
                    {
                        ShowLogs(result.Message);
                    }

                    WeightFormTemplateSheetNames = new ObservableCollection<string>();

                    foreach (var item in _weightFormTemplateSheetsNameSource)
                    {
                        WeightFormTemplateSheetNames.Add(item);
                    }

                    SelectedWeightFormTemplateSheet = WeightFormTemplateSheetNames.FirstOrDefault();
                }

                //WarningIcons.Add(new DeviceNoticeIcon
                //{
                //    Key = DeviceNoticeIcon.DeviceKey.车牌识别,
                //    Online = false,
                //    Icon = DeviceNoticeIcon.ICONMAPS[DeviceNoticeIcon.DeviceKey.车牌识别]
                //});

                if (!_ttsSvc.Available)
                {
                    _eventAggregator?.PublishOnUIThread(new MainShellViewEvent(MainShellViewEvent.EventType.设备检测)
                    {
                        Data = new KeyValuePair<DeviceNoticeIcon.DeviceKey, bool>(DeviceNoticeIcon.DeviceKey.TTS, false)
                    });
                }

                WeighFormVM = new WeighFormViewModel(_mainSetting);
                WeighFormVM.ManualInputCarNoCompleted += OnManualInputCarNoResult;

                SwitchWeighingControl(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(WeighingControl)].TryGetString("Auto"));

                StartTimerThreads();

            };
            weightFormWorker.RunWorkerAsync();

            PrepareMonitors();

            UserAndLicenceCheck();

            if (YunMengZhiHuiDevice.Enable)
            {
                //TODO: gRPC
                #region 启动grpc --阿吉 2024年4月30日14点12分



                #endregion
            }

            // 关闭所有道闸
            _gateSvc.CloseAll();
        }

        private void PrepareMonitors()
        {

            var allGroupedLPRCameras = _mobileConfigurationMgr.Config.intelligenceCfg
                    .carIdentification.cameras.GroupBy(p => p.ip).ToList();

            var allLPRCameras = new List<Common.Utility.AJ.MobileConfiguration.DeviceInfo>();

            foreach (var group in allGroupedLPRCameras)
            {
                allLPRCameras.Add(group.FirstOrDefault());
            }

            var first = allLPRCameras.FirstOrDefault() ?? new DeviceInfo
            {
                ip = "127.0.0.1",
                username = string.Empty,
                password = string.Empty
            };

            var second = allLPRCameras.ElementAtOrDefault(1) ?? new DeviceInfo
            {
                ip = "127.0.0.2",
                username = string.Empty,
                password = string.Empty,
            };
            LPRCameras = new List<Common.HardwareSDKS.Models.DeviceInfo>();
            if (_lprSvc.LPRCameraType == Common.HardwareSDKS.Models.DeviceType.海康)
            {
                LPRCameraOne = new Common.HardwareSDKS.Models.HIKVisionDevice
                {
                    IP = first.ip,
                    LoginName = first.username,
                    LoginPassword = first.password,
                    Enable = first.enable,
                    DisplayIndex = 0,
                    RTSPUrl = $"rtsp://{first.username}:{first.password}@{first.ip}:554/h264/ch1/sub/av_stream",
                    IsCarIdentification = true,
                    DisableVideo = first.disableVideo,
                    RasterCoveredIOState = first.rasterCoveredIOState
                };

                LPRCameraTwo = new Common.HardwareSDKS.Models.HIKVisionDevice
                {
                    IP = second.ip,
                    LoginName = second.username,
                    LoginPassword = second.password,
                    Enable = second.enable,
                    DisplayIndex = 3,
                    RTSPUrl = $"rtsp://{second.username}:{second.password}@{second.ip}:554/h264/ch1/sub/av_stream",
                    IsCarIdentification = true,
                    DisableVideo = second.disableVideo,
                    RasterCoveredIOState = second.rasterCoveredIOState
                };

            }
            else if (_lprSvc.LPRCameraType == Common.HardwareSDKS.Models.DeviceType.华夏)
            {
                LPRCameraOne =
                new Common.HardwareSDKS.Models.HuaXiaCameraDevice
                {
                    IP = first.ip,
                    LoginName = first.username,
                    LoginPassword = first.password,
                    Enable = first.enable,
                    DisplayIndex = 0,
                    IsCarIdentification = true,
                    DisableVideo = first.disableVideo,
                    RasterCoveredIOState = first.rasterCoveredIOState
                };
                LPRCameraTwo = new Common.HardwareSDKS.Models.HuaXiaCameraDevice
                {
                    IP = second.ip,
                    LoginName = second.username,
                    LoginPassword = second.password,
                    Enable = second.enable,
                    DisplayIndex = 3,
                    IsCarIdentification = true,
                    DisableVideo = second.disableVideo,
                    RasterCoveredIOState = second.rasterCoveredIOState
                };
            }
            else
            {
                LPRCameraOne =
                new Common.HardwareSDKS.Models.VzCarIdentificationDevice
                {
                    IP = first.ip,
                    LoginName = first.username,
                    LoginPassword = first.password,
                    Enable = first.enable,
                    DisplayIndex = 0,
                    IsCarIdentification = true,
                    DisableVideo = first.disableVideo,
                    RasterCoveredIOState = first.rasterCoveredIOState
                };
                LPRCameraTwo = new Common.HardwareSDKS.Models.VzCarIdentificationDevice
                {
                    IP = second.ip,
                    LoginName = second.username,
                    LoginPassword = second.password,
                    Enable = second.enable,
                    DisplayIndex = 3,
                    IsCarIdentification = true,
                    DisableVideo = second.disableVideo,
                    RasterCoveredIOState = second.rasterCoveredIOState
                };

            }

            LPRCameras.Add(LPRCameraOne);
            LPRCameras.Add(LPRCameraTwo);

            var captureCameras = _mobileConfigurationMgr.Config.intelligenceCfg.capture.cameras;

            // 这里赋值已经生成好的车牌识别相机列表,后续要用
            _ledSvc.LPRCameras = _lprSvc.LPRCameras = LPRCameras;

            if (LPRCameras.Any(p => p.Enable && p.Type == Common.HardwareSDKS.Models.DeviceType.海康))
            {
                CHCNetSDK.NET_DVR_Init();

                // 设置海康监听回调, 只需要设置一次
                CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(Common.LPR.LPRService.MSGCALLBACK, IntPtr.Zero);

            }

            if (LPRCameras.Any(p => p.Enable && p.Type == Common.HardwareSDKS.Models.DeviceType.臻识))
            {
                VzClientSDK.VzLPRClient_Setup();
            }

            if (LPRCameras.Any(p => p.Enable && p.Type == Common.HardwareSDKS.Models.DeviceType.华夏))
            {
                HuaXiaICESDK.ICE_IPCSDK_Init();
            }

            MonitorList = new ObservableCollection<Common.HardwareSDKS.Models.DeviceInfo>()
                {
                    LPRCameraOne,
                    new Common.HardwareSDKS.Models.HIKVisionDevice{
                        IP = captureCameras.ElementAtOrDefault(0)?.ip ?? string.Empty,
                        LoginName = captureCameras.ElementAtOrDefault(0)?.username ?? string.Empty,
                        LoginPassword = captureCameras.ElementAtOrDefault(0)?.password ?? string.Empty,
                        Enable = captureCameras.ElementAtOrDefault(0)?.enable ?? false,
                        DisplayIndex = 1,
                        RTSPUrl = captureCameras.ElementAtOrDefault(0)?.rtspUrl,
                    },
                    new Common.HardwareSDKS.Models.HIKVisionDevice{
                        IP = captureCameras.ElementAtOrDefault(2)?.ip ?? string.Empty,
                        LoginName = captureCameras.ElementAtOrDefault(2)?.username ?? string.Empty,
                        LoginPassword = captureCameras.ElementAtOrDefault(2)?.password ?? string.Empty,
                        Enable = captureCameras.ElementAtOrDefault(2)?.enable ?? false,
                        DisplayIndex = 3,
                        RTSPUrl = captureCameras.ElementAtOrDefault(2)?.rtspUrl,
                    },
                    LPRCameraTwo,
                    new Common.HardwareSDKS.Models.HIKVisionDevice{
                        IP = captureCameras.ElementAtOrDefault(1)?.ip ?? string.Empty,
                        LoginName = captureCameras.ElementAtOrDefault(1)?.username ?? string.Empty,
                        LoginPassword = captureCameras.ElementAtOrDefault(1)?.password ?? string.Empty,
                        Enable = captureCameras.ElementAtOrDefault(1)?.enable ?? false,
                        DisplayIndex = 2,
                        RTSPUrl = captureCameras.ElementAtOrDefault(1)?.rtspUrl,
                    },
                    new Common.HardwareSDKS.Models.HIKVisionDevice{
                        IP = captureCameras.ElementAtOrDefault(3)?.ip ?? string.Empty,
                        LoginName = captureCameras.ElementAtOrDefault(3)?.username ?? string.Empty,
                        LoginPassword = captureCameras.ElementAtOrDefault(3)?.password ?? string.Empty,
                        Enable = captureCameras.ElementAtOrDefault(3)?.enable ?? false,
                        DisplayIndex = 4,
                        RTSPUrl = captureCameras.ElementAtOrDefault(3)?.rtspUrl,
                    },
                };

            foreach (var monitor in MonitorList)
            {
                monitor.SetEventAggregator(_eventAggregator);
            }
        }

        public void ShowActiveDialog()
        {
            var dialog = new ActiveCodeDialogViewModel();
            var result = this._windowManager.ShowDialog(dialog);
            if (result.GetValueOrDefault())
            {
                _mobileConfigurationMgr.SettingList[SettingNameKey.Main]
                    .Settings[CloudAPI.AppActiveConfigKey].Value = dialog.ActiveCodeData.RegisterCode.Base64Encrypt();

                MessageBox.Show($"感谢您的支持,{dialog.ActiveCodeData.CompanyName} !请重新启动以保证所有功能正常使用", "激活成功");
            }
        }

        private void UserAndLicenceCheck()
        {
            var userAndLicenceCheckWorker = new BackgroundWorker();

            userAndLicenceCheckWorker.DoWork += (s, e) =>
            {
                var result = new ProcessResult();

                //检查用户称重操作权限
                result.SetSuccess(Globalspace._currentUser.Permission);

                //检测是否注册系统
                if (!CloudAPI.APPLICENSEINFO.IsActive)
                {
                    using var db = AJDatabaseService.GetDbContext();

                    //StatusBar = "系统未注册，无法使用称重功能！";
                    if (db.WeighingRecords.Count() > 200)
                    {
                        result.SetError("", Globalspace._currentUser.Permission);
                    }
                }
                e.Result = result;
            };

            userAndLicenceCheckWorker.RunWorkerCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    ShowLogs($"userAndLicenceCheck Fail:{e.Error.Message}");
                    return;
                }
                var result = e.Result as ProcessResult;

                if (!result.Success)
                {
                    StatusBar = "试用期已结束，请注册后使用！";
                    EnableWeighing = false;
                }

                var rolePermission = result.Data?.ToString() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(rolePermission))
                {
                    if (rolePermission.Contains("称重操作")) EnableWeighing = true;
                    if (rolePermission.Contains("系统设置")) EnableSetting = true;
                    //if (rolePermission.Contains("数据管理")) EnableDataManage = true;
                    if (rolePermission.Contains("系统日志查询")) EnableCheckSysLog = true;

                    //底部查询列表相关权限

                    //if (rolePermission.Contains("导出列表")) EnableExportList = true;
                    
                    //if (rolePermission.Contains("修改选中")) EnableUpdateSelected = true;
                    if (rolePermission.Contains("删除选中")) EnableDeleteSelected = true;
                    
                }

            };
            userAndLicenceCheckWorker.RunWorkerAsync();
        }

        private void StartTimerThreads()
        {
            try
            {

                //这里扫描 道闸常开 的值是否被小程序或者其他程序该变，一旦改变就立即刷新界面上的按钮状态
                var propertyChange = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(5000) };

                propertyChange.Start();
                //-var serverPath = Common.Utility.SettingsHelper.ZXSyncSettings.Settings["ServerPath"].Value;
                propertyChange.Tick += (sender, args) =>
                {
                    var isConnected = false; // Common.Encrt.WebApi.Ping(serverPath);
                    string flag = Convert.ToBoolean(KeepOpen).ToString();
                    //var keepOpen = 
                    var keepOpen = _mainSetting.Settings["KeepOpen"].Value;
                    if (flag != keepOpen && isConnected)
                    {
                        KeepOpen = Convert.ToBoolean(keepOpen);
                    }

                    //判断MinfoftWeiging是否改变，一定COnfig文件改变了，这里就要及时刷新
                    // 多线程轮询注释掉， 预防断电后文配置文件失败 --阿吉 2023年8月1日17点03分
                    //if (ConfigurationManager.AppSettings["MinSlotWeight"] != _mainSetting.Settings["MinSlotWeight"].Value)
                    //{
                    //    ConfigurationManager.RefreshSection("appSettings");
                    //}
                };

                //加载首页分割控件的位置
                var point = _mainSetting.Settings["SplitterTopPoint"]?.Value ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(point))
                {
                    _SplitterTopPoint = new GridLength(double.Parse(point), GridUnitType.Star);
                }

                point = _mainSetting.Settings["SplitterBottomPoint"].Value ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(point))
                {
                    _SplitterBottomPoint = new GridLength(double.Parse(point), GridUnitType.Star);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

        }

        private void InitMobileConfigCMDMaps()
        {
            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.平台数据纠正,
                (@params) =>
                {
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.手动备份数据库,
                (@params) =>
                {
                    BackupHelper.ManualBackupAsync(_windowManager);
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.手动数据库恢复,
                (@params) =>
                {
                    BackupHelper.DBRestoreAsync(_windowManager);
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.清理全部数据,
                async (@params) =>
                {
                    var ret = await BackupHelper.DbCleanAsync(_windowManager, @params, false);
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.按月份清理数据,
                (@params) =>
                {
                    var config = AJUtil.TryGetJSONObject<CleanupConfig>(@params);

                    if (!BackupHelper.CleanupByMonthCfg(_windowManager, config, false))
                    {
                        throw new NotSupportedException("管理员密码错误");
                    }

                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.臻识相机工具,
                (@params) =>
                {
                    SettingViewModel.OpenToolsFolderCore("xj");
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.控制器工具,
                (@params) =>
                {
                    SettingViewModel.OpenToolsFolderCore("kzq");
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.重启系统,
                (@params) =>
                {
                    _eventAggregator.PublishOnUIThread(new MainShellViewEvent(MainShellViewEvent.EventType.重启));
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.更新系统,
                (@params) =>
                {
                    AutoUpdater.Mandatory = true;
                    AutoUpdater.UpdateMode = AutoUpdaterDotNET.Mode.ForcedDownload;

                    AutoUpdater.Start(CloudAPI.UpdateXMLUrl);

                    AutoUpdater.Mandatory = false;
                    AutoUpdater.UpdateMode = AutoUpdaterDotNET.Mode.Normal;

                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.下载数据库,
                 async (@params) =>
                {
                    var file = await BackupHelper.AutoBackupAsync();
                    if (string.IsNullOrEmpty(file))
                    {
                        throw new NotSupportedException("备份数据库失败");
                    }
                    var result = await _mqttSvc.UploadFileAsync(file);
                    if (!result.Success)
                    {
                        throw new NotSupportedException(result.Message);
                    }
                    return new
                    {
                        file = result.Data.ToString()
                    };
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.获取VZ设备,
                 async (@params) =>
                 {
                     await Task.Delay(100);
                     return new
                     {
                         devices = _mobileConfigurationMgr.VzDevices
                     };
                 });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.更新VZ设备,
                 (@params) =>
                 {
                     _mobileConfigurationMgr.UpdateVzDevice(AJUtil.TryGetJSONObject<DeviceInfo>(@params));

                     //var bytes = Encoding.Default.GetBytes(@params);

                     //var sm = new ShareMem();
                     //sm.Init("deviceInfo", bytes.Length);
                     //sm.Write(bytes, 0, bytes.Length);

                     //IPCHelper.SendMsgToApp("车牌识别", IPCHelper.UPDATEDEVICE,
                     //    new IntPtr(bytes.Length), IntPtr.Zero);
                     object ret = null;
                     return Task.FromResult(ret);
                 });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.磅单图片,
                 async (@params) =>
                 {
                     // 前端传过来的是编号和车号组成的数组, [Bh,Ch]
                     var array = AJUtil.TryGetJSONObject<string[]>(@params);
                     if (array == null || array.Length != 2)
                     {
                         throw new NotSupportedException("获取图片失败:参数错误");
                     }
                     var images = await Common.SyncData.UploadCarNoImagesAsync(array[0], array[1]);

                     return new
                     {
                         images
                     };
                 });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.磅单打印,
                async (@params) =>
                {
                    var result = new ProcessResult();
                    var weighRecord = AJUtil.TryGetJSONObject<WeighingRecord>(@params);
                    if (weighRecord == null)
                    {
                        result.SetError("磅单参数解析失败,请检查传入数据");
                        return result;
                    }
                    var printWeighType = PrintWeighType.FirstWeighing;
                    if (weighRecord.WeighingTimes == 0)
                    {
                        printWeighType = PrintWeighType.OnceWeighing;
                    }
                    else if (weighRecord.WeighingTimes == 2)
                    {
                        printWeighType = PrintWeighType.SecondWeighing;
                    }
                    weighRecord.Dyrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    result = await Common.Utility.PrintHelper.PrintWeighRecordAsync(weighRecord, _mainSetting, _worksheet, printWeighType, true);

                    return result;
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.道闸全开,
                 (@params) =>
                {
                    object result = null;
                    if (@params.TryGetBoolean())
                    {
                        _gateSvc.Open(null);
                    }
                    else
                    {
                        _gateSvc.CloseAll();
                    }

                    return Task.FromResult(result);
                });

        }

        private async void ConnectToMQTTAsync()
        {
            _mqttSvc = new MQTTService();

            _mobileConfigurationMgr.SetMQTTService(_mqttSvc);
            _ajmqttAppointProcessor.SetMQTTService(_mqttSvc);

            var cfg = AJUtil.TryGetJSONObject<MQTTConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(MQTTConfig)].TryGetString())
                ?? new MQTTConfig();

            var sysParamsCfg = AJUtil.TryGetJSONObject<AJSystemParamsCfg>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(AJSystemParamsCfg)].TryGetString())
                ?? new AJSystemParamsCfg();

            var locker = new SemaphoreSlim(1, 1);

            _mqttSvc.ConnectionChanged += async (s, e) =>
            {
                await locker.WaitAsync();

                _eventAggregator.PublishOnUIThread(new MainShellViewEvent(MainShellViewEvent.EventType.设备检测)
                {
                    Data = new KeyValuePair<DeviceNoticeIcon.DeviceKey, bool>(DeviceNoticeIcon.DeviceKey.云服务器, e)
                });

                await _mqttSvc.UpdateApplicationAsync(CloudAPI.MACHINEMD5, e);

                if (e)
                {
                    await _mqttSvc.SubscribeAsync(MQTTService.MAINAPPLICATION, (mqttMsg) =>
                    {
                        return _mobileConfigurationMgr.ProcessMessageAsync(mqttMsg.ApplicationMessage);
                    });
                }

                locker.Release();
            };

            var localServerResult = await _mqttSvc.StartLocalServer();
            if (!localServerResult.Success)
            {
                log.Error($"mqtt 本地服务启动异常:{localServerResult.Message}\r\n:{localServerResult.Data}");
            }
            else
            {
                await _mqttSvc.SubscribeLocalAsync("/appoint/up", (mqttMsg) =>
                {
                    return _ajmqttAppointProcessor.ProcessMessageAsync(mqttMsg.ApplicationMessage);
                });
            }

            try
            {
                await _mqttSvc.ConnectTCPAsync(cfg.Server, cfg.Port,
                    sysParamsCfg.ConnectTimeout,
                    sysParamsCfg.HeartbeatInterval,
                    sysParamsCfg.OfflineCheck);
            }
            catch (Exception e)
            {
                log.Error($"mqtt服务发生异常", e);
                _eventAggregator.PublishOnUIThread(new MainShellViewEvent(MainShellViewEvent.EventType.设备检测)
                {
                    Data = new KeyValuePair<DeviceNoticeIcon.DeviceKey, bool>(DeviceNoticeIcon.DeviceKey.云服务器, false)
                });
                //ShowLogs($"实时通讯服务发生异常:{e.Message}");
            }
        }

        private void StartQueryListTimer()
        {
            //_queryListTimer = new System.Timers.Timer(9999 * 1000);

            //_queryListTimer.Elapsed += (s, e) =>
            //{
            //    _eventAggregator.PublishOnUIThread(new MainShellViewEvent());
            //    //QueryList(string.Empty);
            //};

            //_queryListTimer.Start();
        }

        private bool _dataGridLoading;
        public bool DataGridLoading
        {
            get => _dataGridLoading;
            set => SetAndNotify(ref _dataGridLoading, value);
        }

        public void MainGridLoadedCmd(object sender, RoutedEventArgs e)
        {
            _mainGrid = (System.Windows.Controls.Grid)sender;
        }

        public void ToggleGateSwitch(string gate)
        {
            int param;
            if (gate == "1")
            {
                @param = GateSwitchOne ? 0x08F1 : 0x08F4;
            }
            else
            {
                @param = GateSwitchTwo ? 0x08F2 : 0x08F5;
            }

            IPCHelper.SendMsgToApp("车牌识别", @param);
        }

        public async void SwitchWeightForm(string template)
        {
            if (template != SelectedWeightFormTemplateSheet)
            {
                SelectedWeightFormTemplateSheet = template;

                var index = _weightFormTemplateSheetsNameSource.FindIndex(p => p == template);
                if (index == -1)
                {
                    index = 0;
                }

                SettingsHelper.UpdateAWSV2("PrintTemplate", (index + 1).ToString());
            }

            var poundCfg = new PoundCfg();

            _sourceFields = await poundCfg.InitFieldsAsync(_mobileConfigurationMgr, true);

            var source = Common.Models.WeighingRecord.GetDyanmicPoundFieldItems(_mainSetting, true, true);

            foreach (var item in source)
            {
                var sourceField = _sourceFields.FirstOrDefault(p => p.key == item.Field);
                if (sourceField == null)
                {
                    continue;
                }
                item.Options = sourceField.children?.Count > 0
                ? new ObservableCollection<AJMultiSelectOptionItem>(sourceField.children.Select(q => new AJMultiSelectOptionItem
                {
                    Label = q.label,
                    Value = q.value
                }))
                : new ObservableCollection<AJMultiSelectOptionItem>();
            }
            var bhField = nameof(Common.Models.WeighingRecord.Bh);
            // 索引必须＋1，因为新增加了选择列 --阿吉 2024年7月8日10点42分
            _BhColumnIndex = source.FindIndex(p => p.Field == bhField) + 1;
            DynamicPoundFieldItems = new ObservableCollection<DynamicPoundFieldItem>(source);

            #region 这段代码用来快速生成配置文件里面磅单对应字段，生产环境建议注释掉 --阿吉 2024年05月20日 19:15:42

            //var fieldMaps = new StringBuilder();
            //foreach (var prop in _weighRecordModelProps)
            //{
            //    if (timeFiled == prop.Name || by20Field == prop.Name)
            //    {
            //        continue;
            //    }
            //        var attr = prop.GetCustomAttribute<DisplayAttribute>();
            //    if (attr != null)
            //    {
            //        var key = prop.Name;
            //        if (!string.IsNullOrWhiteSpace(attr.Description))
            //        {
            //            key = attr.Description;
            //        }
            //        key = $"_{key.ElementAtOrDefault(0).ToString().ToLower()}{key.Substring(1)}";
            //        fieldMaps.Append($"<add key=\"{key}\" value=\"{attr.Name}\" />\r\n");
            //    }
            //}
            //var configMaps = fieldMaps.ToString();

            #endregion

            WeighFormVM.SwitchTemplate(_mainSetting, DynamicPoundFieldItems, TemplateType.GetValueOrDefault());
            WeighFormVM.Refresh(ref _weighingRecord, true);

            QueryList(_currentTabParam);
        }

        public void DataGridCellDoubleClickHandle(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid grid && grid.CurrentCell != null)
            {
                e.Handled = true;
                var cell = grid.CurrentCell;
                var column = cell.Column;
                if (column != null)
                {
                    var ctrl = column.GetCellContent(cell.Item);
                    if (ctrl is TextBlock tb && !string.IsNullOrWhiteSpace(tb.Text))
                    {
                        var field = DynamicPoundFieldItems.FirstOrDefault(p => p.Label.Equals(column.Header as string, StringComparison.OrdinalIgnoreCase));
                        if (field == null)
                        {
                            return;
                        }
                        WeighFormVM.SetFormField(field.Field, tb.Text);
                    }
                }

            }
        }

        /// <summary>
        /// 新增用来接收LPR程序发送过来的VZSDK的一些数据 --阿吉 2023年11月30日14点21分
        /// </summary>
        /// <param name="params"></param>
        public async void VZClientSDKHandleAsync(string @params)
        {
            if (string.IsNullOrWhiteSpace(@params))
            {
                return;
            }
            // 目前只有设备查找
            _mobileConfigurationMgr.AddVZDevice(AJUtil.TryGetJSONObject<DeviceInfo>(@params));

            await Task.Delay(100);
        }

        /// <summary>
        /// 新增用来接收LPR程序发送过来的 道闸设备开关状态 --阿吉 2024年03月29日 16:15:56
        /// </summary>
        /// <param name="params"></param>
        public void AJGateStatusNotifyHandleAsync(string @params)
        {
            if (string.IsNullOrWhiteSpace(@params))
            {
                return;
            }
            var data = AJUtil.TryGetJSONObject<AJGateStatusNotifyModel>(@params);
            if (data == null)
            {
                return;
            }
            if (data.Id == null)
            {
                GateSwitchOne = GateSwitchTwo = data.Open;
                return;
            }
            if (data.Id == 1)
            {
                GateSwitchOne = data.Open;
            }
            else
            {
                GateSwitchTwo = data.Open;
            }
        }

        public async void Handle(MainShellViewEvent @event)
        {

            switch (@event.Type)
            {
                case MainShellViewEvent.EventType.车牌识别:

                    OnPlateResultAsync(@event.Data as Common.HardwareSDKS.Models.DeviceInfo);

                    break;

                case MainShellViewEvent.EventType.刷新列表:
                    QueryList(_currentTabParam);
                    break;
                case MainShellViewEvent.EventType.刷新榜单:

                    var model = (WeighingRecord)@event.Data;
                    if (model != null)
                    {
                        ReadyToWeigh = true;
                        GetPlateNo = model.Ch;
                    }

                    break;
                case MainShellViewEvent.EventType.重启:

                    lock (_locker)
                    {
                        if (_restartRequested)
                        {
                            return;
                        }
                        _restartRequested = true;
                        RequestClose();
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }

                    break;
                case MainShellViewEvent.EventType.重量值更新:
                    var retMsg = new MQTTMessagePackage(CloudAPI.MACHINEMD5, MQTTMessageType.实时重量, true);
                    var dic = @event.Data as Tuple<string, string>;
                    if (dic != null)
                    {
                        retMsg.SetSuccess(JObject.FromObject(new
                        {
                            weight = dic.Item1,
                            state = dic.Item2
                        }));
                        await _mqttSvc?.PublishAsync(MAINAPPLICATIONRESPONSE, AJUtil.AJSerializeObject(retMsg));
                    }

                    break;

                case MainShellViewEvent.EventType.设备检测:

                    await _deviceNotifyLocker.WaitAsync();

                    var kv = (KeyValuePair<DeviceNoticeIcon.DeviceKey, bool>)@event.Data;
                    // 仪表通知特殊处理 --阿吉 2024年8月25日19点34分
                    if (kv.Key == DeviceNoticeIcon.DeviceKey.仪表)
                    {
                        InstrumentDeviceIcon = new DeviceNoticeIcon
                        {
                            Key = kv.Key,
                            Online = kv.Value,
                            Icon = DeviceNoticeIcon.ICONMAPS[kv.Key]
                        };
                        _deviceNotifyLocker.Release();
                        return;
                    }
                    // 其他图标都不要了
                    if (kv.Key != DeviceNoticeIcon.DeviceKey.云服务器)
                    {
                        _deviceNotifyLocker.Release();
                        return;
                    }
                    var iconData = _warningIcons.FirstOrDefault(p => p.Key == kv.Key);
                    if (iconData == null)
                    {
                        if (kv.Key == DeviceNoticeIcon.DeviceKey.车牌识别 && kv.Value)
                        {
                            _deviceNotifyLocker.Release();
                            return;
                        }
                        iconData = new DeviceNoticeIcon
                        {
                            Key = kv.Key,
                            Online = kv.Value,
                            Icon = DeviceNoticeIcon.ICONMAPS[kv.Key]
                        };
                        WarningIcons.Add(iconData);
                    }
                    else
                    {
                        //if (kv.Key == DeviceNoticeIcon.DeviceKey.车牌识别 && kv.Value)
                        //{
                        //    WarningIcons.Remove(iconData);
                        //}
                        iconData.Online = kv.Value;
                    }

                    _deviceNotifyLocker.Release();

                    break;

                case MainShellViewEvent.EventType.云盟智慧二维码通知:

                    if (YunMengZhiHuiDevice.Enable)
                    {
                        var str = @event.Data?.ToString();
                        System.Diagnostics.Trace.WriteLine("云盟智慧二维码通知:" + str);
                    }

                    break;

                case MainShellViewEvent.EventType.二维码读头:

                    _qrCodeRawData = @event.Data as string;

                    break;

                case MainShellViewEvent.EventType.称重串口重量值更新:

                    if (@event.Data is WeighSerialPortService.WeighSerialPortEventData data)
                    {
                        WeighSerialPort_DataReceived(data);
                    }

                    break;

                default:
                    break;
            }

        }

        #region 事件驱动称重逻辑 --阿吉 2024年5月12日10点53分

        /// <summary>
        /// 车牌识别相机触发车牌识别线程锁
        /// </summary>
        private static readonly SemaphoreSlim _plateResultLocker = new SemaphoreSlim(1, 1);
        private bool _plateResultLockerRuning;
        /// <summary>
        /// 超时复位定时器, 当磅单显示成功后, 如果超过配置时间,  则取消称重,刷新磅单
        /// </summary>
        private System.Timers.Timer _weighCancelTimer;

        private Common.HardwareSDKS.Models.DeviceInfo _currentLPRCameraDevice;

        /// <summary>
        /// 用于检车重量是否稳定的worker
        /// </summary>
        private BackgroundWorker _weightStableWorker;

        /// <summary>
        /// 称重完成后道闸关闭方式检测worker
        /// </summary>
        private BackgroundWorker _minSlotWeightCheckWorker;
        /// <summary>
        /// 是否大于回零重量
        /// </summary>
        private bool _greaterThanMinSlotWeightValue;

        /// <summary>
        /// 道闸光栅检测worker,
        /// </summary>
        private BackgroundWorker _gateRasterCheckWorker;

        /// <summary>
        /// 初始化道闸光栅检测worker
        /// </summary>
        private void InitGateRasterCheckWorker()
        {
            _gateRasterCheckWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            _gateRasterCheckWorker.DoWork += (_, e) =>
            {
                while (true)
                {
                    if (_gateRasterCheckWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    // 如果是按钮称重,则优先检测固定相机A的GPIO状态,如果为0,则表示触发自动称重
                    if (WeighingControl.Equals("Btn", StringComparison.OrdinalIgnoreCase))
                    {
                        if (LPRCameraOne is Common.HardwareSDKS.Models.VzCarIdentificationDevice vzDevice)
                        {
                            var ioStatus = vzDevice.GetGPIOValue();
                            if (ioStatus == 1)
                            {
                                Thread.Sleep(1000);
                                continue;
                            }
                            e.Result = ioStatus;
                        }
                    }

                    if (_gateSvc.RasterCheckResult != null)
                    {
                        if (!_gateSvc.RasterCheckResult.Success)
                        {
                            ShowLogs($"{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 光栅检测失败:{_gateSvc.RasterCheckResult.Message}");
                            _ttsSvc.Speak(_weighingRecord, TTSConfig.TTSTextType.光栅遮挡);

                            Thread.Sleep(1000);

                            _gateSvc.RasterCheckResult = null;
                            _gateSvc.StartCheckRaster();

                            continue;
                        }
                        break;
                    }

                    Thread.Sleep(1000);
                }
            };
            _gateRasterCheckWorker.RunWorkerCompleted += (_, e) =>
            {
                if (e.Cancelled)
                {
                    return;
                }


                // 双向以上模式因为前阶段已经开了A闸,  所以这里要关闭
                if (_gateSvc.BarrierType >= BarrierType.双向)
                {
                    _gateSvc.Close(_currentLPRCameraDevice);
                }

                var gpIOStatus = e.Result as int?;
                var gpIOAuto = gpIOStatus.HasValue && gpIOStatus == 0;
                var cfgAuto = !ManualWeighing && EnableWeighing;
                // 按钮模式下gpIO是 0 或者 自动称重, 则触发自动称重逻辑
                if (gpIOAuto || cfgAuto)
                {
                    ShowLogs(gpIOAuto
                        ? $"{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 自动按钮称重"
                        : $"{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 开始自动称重");
                    if (TwiceWeighing)
                    {
                        if (_weighingRecord.WeighingTimes == 1)
                        {
                            FirstWeighing();
                        }
                        if (_weighingRecord.WeighingTimes == 2)
                        {
                            SecondWeighing();
                        }
                    }
                    else
                    {
                        if (_weighingRecord.WeighingTimes == 1)
                        {
                            OnceWeighing();
                        }
                    }

                    return;
                }

                ShowLogs($"{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 等待手动操作");
            };
        }

        /// <summary>
        /// 开一个后台线程轮询小于回零重量, 刷新称重表单， 如果是称重完成, 则关闭出口闸 --阿吉 2024年5月13日14点48分
        /// </summary>
        private void InitMinSlotWeightCheckWorker()
        {
            _minSlotWeightCheckWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true,
            };
            // 一个标记，存储是否有一次大于回零重量
            _greaterThanMinSlotWeightValue = false;
            _minSlotWeightCheckWorker.DoWork += (_, e) =>
            {
                while (true)
                {
                    if (_minSlotWeightCheckWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    var minSlotWeight = _mainSetting.Settings["MinSlotWeight"].TryGetDecimal();

                    var weighVal = Math.Abs(WeightStr.TryGetDecimal());

                    if (weighVal > minSlotWeight)
                    {
                        _greaterThanMinSlotWeightValue = true;
                        // 如果大于了回零重量, 则及时停止称重超时定时器 --阿吉 2024年7月6日17点08分
                        _weighCancelTimer.Stop();
                    }

                    if (_greaterThanMinSlotWeightValue && weighVal <= _mainSetting.Settings["MinSlotWeight"].TryGetDecimal())
                    {
                        _minSlotWeightCheckWorker.ReportProgress(0, null);
                        // 触发了刷新了就改回去
                        _greaterThanMinSlotWeightValue = false;
                    }

                    Thread.Sleep(1000);
                }
            };
            _minSlotWeightCheckWorker.ProgressChanged += (_, __) =>
            {
                if (_weightStableWorker.IsBusy)
                {
                    _weightStableWorker.CancelAsync();
                }

                if (CurrentStatus == Common.Model.Custom.WeighStatus.Weighing)
                {
                    ShowLogs($"小于回零重量,即将刷新磅单");
                    // 单向模式，小于回零重量也要关闸
                    _gateSvc.Close(_currentLPRCameraDevice);
                }

                if (CurrentStatus == Common.Model.Custom.WeighStatus.WeighEnd)
                {
                    ShowLogs($"{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 即将开闸");

                    // 读取称重完成后传入的开闸的那个方向
                    _currentLPRCameraDevice.CarIdentificationPlateResult.Direction = _gateSvc.BarrierType >= BarrierType.双向
                        ? _currentLPRCameraDevice.CarIdentificationPlateResult.Direction : CarEntryDirection.In;
                    // 单向模式, 车走了, 要关掉A闸
                    // 如果是双向, 车走了, 要关传入的方向的闸
                    _gateSvc.Close(_currentLPRCameraDevice);

                }

                RefreshWeighForm(true);
                WeightLineChart.Stop();
            };
            _minSlotWeightCheckWorker.RunWorkerCompleted += (_, e) =>
            {

            };

        }

        /// <summary>
        /// 初始化用于检车重量是否稳定的worker
        /// </summary>
        private void InitWeightStableWorker()
        {
            _weightStableWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            _weightStableWorker.DoWork += (_, e) =>
            {
                ShowLogs("等待重量稳定...");
                // 等待重量稳定了才能继续走其余逻辑
                while (true)
                {
                    if (_weightStableWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    if (IsDelayNZStable)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            };

            _weightStableWorker.RunWorkerCompleted += (_, e) =>
            {
                if (_gateRasterCheckWorker.IsBusy)
                {
                    _gateRasterCheckWorker.CancelAsync();
                    while (_gateRasterCheckWorker.IsBusy)
                    {
                        DispatcherHelper.DoEvents();
                        Thread.Sleep(600);
                    }
                }

                if (e.Cancelled)
                {
                    ShowLogs($"{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 取消称重");
                    _currentLPRCameraDevice.CarIdentificationPlateResult.CarNo = string.Empty;
                    _eventAggregator.Publish(new MainShellViewEvent(MainShellViewEvent.EventType.身份证读卡器)
                    {
                        Data = new RunWorkerCompletedEventArgs(null, null, true)
                    });
                    RefreshWeighForm(true);
                    WeightLineChart.Stop();
                    return;
                }

                _gateSvc.StartCheckRaster();
                // 稍微延迟下，再发送LED，
                Thread.Sleep(1500);
                _ledSvc.SendText(_currentLPRCameraDevice, _weighingRecord, Common.HardwareSDKS.Models.LEDDeviceBase.LEDDisplayTextType.重量稳定);

                _gateRasterCheckWorker.RunWorkerAsync();

                ShowLogs($"{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 正在检测光栅...");

                // tts语音播报重量稳定两次 --阿吉 2024年5月31日19点13分
                Task.Run(() =>
                {
                    var prompt = _ttsSvc.Speak(_weighingRecord, TTSConfig.TTSTextType.重量稳定);
                    var playCount = 1;
                    while (playCount == 1)
                    {
                        if (!prompt.IsCompleted)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        prompt = _ttsSvc.Speak(_weighingRecord, TTSConfig.TTSTextType.重量稳定);
                        playCount++;
                    }
                });
            };
        }

        private void RunWeightStableWorker()
        {
            // 超时复位定时器启动
            // 重新赋值间隔, 确保是最新配置的间隔
            _weighCancelTimer.Interval = TimeSpan
                .FromSeconds(_mainSetting.Settings["CancelWeighingTime"].TryGetDouble(60)).TotalMilliseconds;
            _weighCancelTimer.Start();

            if (_weightStableWorker.IsBusy)
            {
                _weightStableWorker.CancelAsync();
                while (_weightStableWorker.IsBusy)
                {
                    DispatcherHelper.DoEvents();
                    Thread.Sleep(600);
                }
            }

            _weightStableWorker.RunWorkerAsync();
        }

        private async void OnManualInputCarNoResult(object sender, EventArgs e)
        {
            var carNo = sender.ToString();
            // 如果车号为空， 说明表单清空了车号， 可能需要取消称重
            if (string.IsNullOrWhiteSpace(carNo))
            {
                if (_weightStableWorker.IsBusy)
                {
                    _weightStableWorker.CancelAsync();
                    while (_weightStableWorker.IsBusy)
                    {
                        DispatcherHelper.DoEvents();
                        Thread.Sleep(600);
                    }
                }

                return;
            }
            if (carNo.Equals(_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo))
            {
                System.Diagnostics.Trace.WriteLine($"当前正在处理:{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo},或输入的车牌与当前车牌相同,跳过");
                return;
            }

            await ProcessCarPlateCoreAsync(new Common.HardwareSDKS.Models.DeviceInfo
            {
                CarIdentificationPlateResult = new Common.HardwareSDKS.Models.CarIdentificationResult
                {
                    // 移除最开始的颜色信息
                    CarNo = carNo,
                    Color = CarPlateColor.蓝色,
                    Direction = CarEntryDirection.None
                }
            });
        }

        private async void OnPlateResultAsync(Common.HardwareSDKS.Models.DeviceInfo deviceInfo)
        {
            await _plateResultLocker.WaitAsync();

            _plateResultLockerRuning = true;

            var result = deviceInfo.CarIdentificationPlateResult;

            //log.Debug($"识别到:{result.CarNo}");

            if (result.CarNo.Equals(_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo))
            {
                log.Debug($"当前正在处理:{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo},相同车牌号,跳过");
                _plateResultLocker.Release();
                return;
            }

            var camera = LPRCameras.FirstOrDefault(p => p.IP == deviceInfo.IP);
            if (camera == null)
            {
                log.Debug($"未找到独对应IP:{deviceInfo.IP}的相机");
                _plateResultLocker.Release();
                return;
            }

            // 如果当前有车号在处理
            if (!string.IsNullOrWhiteSpace(_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo))
            {
                // 判断是否大于回零重量
                if (_greaterThanMinSlotWeightValue)
                {
                    log.Debug($"当前正在处理:{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 正在称重,忽略处理{result.CarNo} ");
                    _plateResultLocker.Release();
                    return;
                }
                // 取消称重,允许新车号称重
                if (_weightStableWorker.IsBusy)
                {
                    _weightStableWorker.CancelAsync();
                    while (_weightStableWorker.IsBusy)
                    {
                        DispatcherHelper.DoEvents();
                        Thread.Sleep(600);
                    }
                }
            }

            _eventAggregator.PublishOnUIThread(new MainShellViewEvent(MainShellViewEvent.EventType.设备检测)
            {
                Data = new KeyValuePair<DeviceNoticeIcon.DeviceKey, bool>(DeviceNoticeIcon.DeviceKey.车牌识别, true)
            });

            //log.Debug($"{result.CarNo} 进入称重流程");
            await ProcessCarPlateCoreAsync(deviceInfo);

            _plateResultLocker.Release();

            _plateResultLockerRuning = false;
        }

        /// <summary>
        /// 处理车牌称重核心方法, 无论是相机识别的还是手动输入的
        /// </summary>
        /// <param name="device">识别设备,识别结果中的方向为None 可能为手动输入的车牌信息</param>
        /// <returns></returns>
        private async Task ProcessCarPlateCoreAsync(Common.HardwareSDKS.Models.DeviceInfo device)
        {
            if (!CloudAPI.APPLICENSEINFO.OnTrial && !CloudAPI.APPLICENSEINFO.IsActive)
            {
                ShowLogs("软件尚未激活无法继续称重");
                return;
            }
            var carPlateResult = device.CarIdentificationPlateResult;
            var currentCarPlateResult = _currentLPRCameraDevice.CarIdentificationPlateResult;

            if (currentCarPlateResult.CarNo == carPlateResult.CarNo)
            {
                return;
            }
            CurrentStatus = Common.Model.Custom.WeighStatus.Waiting;
            LogContent.Clear();

            _currentLPRCameraDevice = device.CarIdentificationPlateResult.Direction != CarEntryDirection.None
                ? LPRCameras.FirstOrDefault(p => p.IP == device.IP && p.Type == device.Type)
                : device;
            if (_currentLPRCameraDevice == null)
            {
                ShowLogs($"指定的车牌识别相机未找到：{device.IP};{device.Type}");
                return;
            }

            _currentLPRCameraDevice.DisplayCapture(currentCarPlateResult);

            _currentLPRCameraDevice.CarIdentificationPlateResult = device.CarIdentificationPlateResult;
            // 谁识别的谁就是车辆入方向
            _currentLPRCameraDevice.CarIdentificationPlateResult.Direction = CarEntryDirection.In;

            var processResult = await CheckCarPlateAsync();
            if (!processResult.Success)
            {
                ShowLogs(processResult.Message);
                _currentLPRCameraDevice.CarIdentificationPlateResult.CarNo = string.Empty;
                return;
            }

            CurrentStatus = Common.Model.Custom.WeighStatus.ReadyWeigh;

            WeightLineChart.Start();

            var gateOpened = false;
            if (processResult.Data is PlatformBase.CheckCarPlateResult checkCarPlateRet
                && checkCarPlateRet.OpenGate)
            {
                gateOpened = true;
                // 只有双向才会提前开闸,谁识别谁开闸
                if (_gateSvc.BarrierType >= BarrierType.双向)
                {
                    _gateSvc.Open(_currentLPRCameraDevice);
                }
                else
                {
                    // 如果是单向， 则关闸， 防止道闸没关
                    _gateSvc.Close(_currentLPRCameraDevice);
                }

                _ledSvc.SendText(_currentLPRCameraDevice, _weighingRecord, Common.HardwareSDKS.Models.LEDDeviceBase.LEDDisplayTextType.识别到车牌);
                _ttsSvc.Speak(_weighingRecord);
            }

            processResult = await CheckWeighFormAsync(gateOpened);
            if (!processResult.Success)
            {
                ShowLogs(processResult.Message);
                _currentLPRCameraDevice.CarIdentificationPlateResult.CarNo = string.Empty;
                return;
            }

            CurrentStatus = Common.Model.Custom.WeighStatus.Weighing;

            RunWeightStableWorker();

        }

        /// <summary>
        /// 称重表单统一检测方法, 在这里定制各个厂商的逻辑
        /// </summary>
        /// <param name="gateOpened">表示是否在上一步的检车车牌那里已经开闸了</param>
        /// <returns></returns>
        private async Task<ProcessResult> CheckWeighFormAsync(bool gateOpened)
        {
            var result = new ProcessResult();

            var carNo = _weighingRecord.Ch = _currentLPRCameraDevice.CarIdentificationPlateResult.CarNo;

            ShowLogs($"等待 {carNo}");

            // 启用了二维码读头, 则等待读取二维码数据
            var qrCodeData = string.Empty;
            if (_mainSetting.Settings["QREnable"].TryGetBoolean()
                || _mainSetting.Settings["QR2Enable"].TryGetBoolean())
            {
                var qrCodeDialog = new AJCommonLoadingDialogViewModel("等待二维码扫码...", () =>
                {
                    return ReadQRCodeWaitResultAsync();
                });

                var dialogRet = _windowManager.ShowDialog(qrCodeDialog);

                if (qrCodeDialog.Result.Success)
                {
                    result.SetError($"未能读取二维码信息:{qrCodeDialog.Result.Message}");
                    return result;
                }
                // 赋值这个数据,传给下面的处理
                qrCodeData = qrCodeDialog.Result.Data as string;
                // 把二维码串口字符串的变量清空
                _qrCodeRawData = string.Empty;

            }

            // 启用了身份证读卡器,则等待读取身份证
            if (_lprSvc.IDCardReaderCfg.Enable)
            {
                var idCardReadDialog = new AJCommonLoadingDialogViewModel("正在读取身份证...", () =>
                {
                    return _lprSvc.IDCardReaderCfg.ReadIDCardAsync();
                });

                var dialogRet = _windowManager.ShowDialog(idCardReadDialog);

                if (!idCardReadDialog.Result.Success)
                {
                    result.SetError($"未能读取身份证信息:{idCardReadDialog.Result.Message}");
                    return result;
                }

            }

            result = await BuildWeighFormAsync();

            if (!result.Success)
            {
                result.SetError($"基础磅单数据获取失败: {result.Message}");
                return result;
            }

            // 只有一次称重需要调用计划下单或第三方接口填充表单数据 --阿吉 2024年05月29日 10:59:51
            if (_weighingRecord.WeighingTimes == 1)
            {
                // 预约表单数据是否已经填充
                var reserveDataFilled = false;
                var reserveTTSMsg = string.Empty;
                var reservdLogMsg = string.Empty;
                // 如果启用云平台扫码预约功能
                if ((PlatformManager.ActiveCode?.EnableReserveWeigh).GetValueOrDefault())
                {
                    // 则访问云平台接口，查询预约信息
                    var reserveRet = ReserveWeighCheck();

                    reserveDataFilled = reserveRet.reserveDataFilled;
                    reserveTTSMsg = reserveRet.ttsMsg;
                    reservdLogMsg = reserveRet.logMsg;
                }

                // 如果没有预约数据， 则调用计划下单或第三方接口填充表单数据
                if (!reserveDataFilled)
                {
                    result = await FillWeighFormAsync(qrCodeData);

                    // 如果计划下单也没有， 则继续判断是否开启了白名单功能
                    if (!result.Success)
                    {
                        reservdLogMsg += $" {result.Message}";
                        // 检查是否是白名单, --阿吉 2024年5月10日14点15分
                        var needError = false;
                        if (_mainSetting.Settings["LPRWhiteList"].TryGetInt() == 0)
                        {
                            needError = true;
                        }
                        else if (_mainSetting.Settings["LPRWhiteList"].TryGetInt() == 1)
                        {
                            using var db = AJDatabaseService.GetDbContext();
                            var hasCar = await db.Cars.AnyAsync(p => p.PlateNo == carNo);
                            needError = !hasCar;
                        }

                        if (needError)
                        {
                            var customTemplate = reserveTTSMsg ?? $"未经授权,禁止上磅";
                            _ttsSvc.Speak(_weighingRecord, customTemplate: customTemplate);
                            // 稍微延迟下，再发送LED，
                            Thread.Sleep(1500);
                            _ledSvc.SendText(_currentLPRCameraDevice, _weighingRecord, customTemplate: customTemplate);

                            ShowLogs(reservdLogMsg);

                            return result;
                        }
                        else
                        {
                            result.Success = true;
                        }
                    }
                }
            }

            if (!gateOpened)
            {
                // 只有双向才会提前开闸,谁识别谁开闸
                if (_gateSvc.BarrierType >= BarrierType.双向)
                {
                    _gateSvc.Open(_currentLPRCameraDevice);
                    _ledSvc.SendText(_currentLPRCameraDevice, _weighingRecord, Common.HardwareSDKS.Models.LEDDeviceBase.LEDDisplayTextType.识别到车牌);
                    _ttsSvc.Speak(_weighingRecord);
                }
                else
                {
                    // 如果是单向， 则关闸， 防止道闸没关
                    _gateSvc.Close(_currentLPRCameraDevice);
                }
            }

            Globalspace.CarPlateColorKey = _currentLPRCameraDevice.CarIdentificationPlateResult.Color;

            if (result.Data != null)
            {
                _weighingRecord = result.Data as Common.Models.WeighingRecord;
            }

            WeighFormVM.Refresh(ref _weighingRecord);

            ShowLogs($"磅单数据更新成功");

            result.SetSuccess();

            return result;
        }

        /// <summary>
        /// 只有第一次称重的时候, 统一从外部或者第三方放接口填充表单数据的方法,在这里定制厂家逻辑
        /// </summary>
        /// <param name="qrCodeData">二维码串口的数据</param>
        /// <returns></returns>
        private async Task<ProcessResult> FillWeighFormAsync(string qrCodeData)
        {
            var result = new ProcessResult();

            ShowLogs($"查询 {_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 数据...");

            if (PlatformManager.Instance.Current is PlatformBase platform)
            {
                result = await platform.MapWeighRecordVerifyAsync(new PlatformBase.MapWeighRecordVerifyParams
                {
                    WeighRecord = _weighingRecord,
                });

                if (!result.Success)
                {
                    return result;
                }

                platform.MapWeighRecordBefore(ref _weighingRecord, new PlatformBase.MapWeighRecordParams
                {
                    LPRSvc = _lprSvc,
                    MapWeighRecordVerificationResult = result
                });

                var enableNotBookedCarWeigh = _mobileConfigurationMgr.Config.systemCfg.weighting.enableNotBookedCarWeigh;

                result = await platform.GetWeighFormDataAsync(new PlatformBase.GetWeighFormDataParams
                {
                    BasicWeighRecord = _weighingRecord,
                    QRCodeData = qrCodeData,
                    // 如果 没有 启用  [未预约车辆称重] 则需要报错阻止称重
                    ErrorWhenPlanEmpty = !enableNotBookedCarWeigh
                });

                if (!result.Success)
                {
                    return result;
                }
                // 重要!!! 这里重新赋值为 各个平台 处理过的合并的数据
                _weighingRecord = result.Data as Common.Models.WeighingRecord;
                // 重要!!! 这里上一步合并数据过程重会导致已经计算好的称重次数被重置， 所以这里重新赋值
                _weighingRecord.WeighingTimes = WeighingTimes;

            }
            ShowLogs($"查询成功!");
            result.SetSuccess(_weighingRecord);
            return result;
        }

        /// <summary>
        /// 构建称重表单
        /// </summary>
        /// <returns></returns>
        private async Task<ProcessResult> BuildWeighFormAsync()
        {
            var result = new ProcessResult();
            try
            {
                using var db = AJDatabaseService.GetDbContext();
                _weighingRecord.PlateColor = _currentLPRCameraDevice.CarIdentificationPlateResult.Color;
                _weighingRecord.Ch = _currentLPRCameraDevice.CarIdentificationPlateResult.CarNo;
                using var ctrl = new WeighingRecordController();
                var pages = await ctrl.Pages(new WeighingRecordSearchForm
                {
                    Limit = 1,
                    Filter = new JObject
                    {
                        {nameof(WeighingRecord.By20), false },
                        {nameof(WeighingRecord.Ch), _weighingRecord.Ch },
                        {nameof(WeighingRecord.IsFinish), false }
                    }
                });
                var ch = _weighingRecord.Ch;
                var unFinishWeighRecordModel = pages.Data.FirstOrDefault();
                var carModel = await db.Cars.FirstOrDefaultAsync(p => p.PlateNo == ch);
                var weighingMode = _mainSetting.Settings["WeighingMode"].TryGetString("Twice");

                // 如果车号有皮重，则直接是二次称重 --阿吉 2024年05月23日 21:08:24
                var carHasPz = (carModel?.VehicleWeight ?? 0) > 0;

                TwiceWeighing = weighingMode.Equals("Twice");

                if (unFinishWeighRecordModel == null)
                {
                    _weighingRecord = new WeighingRecord()
                    {
                        Ch = _currentLPRCameraDevice.CarIdentificationPlateResult.CarNo,
                        WeighingTimes = carHasPz ? 2 : 1,
                        Pz = carHasPz ? carModel.VehicleWeight : 0
                    };

                    WeighingTimes = _weighingRecord.WeighingTimes;
                }
                else
                {
                    var kh3 = string.Empty;

                    if (carModel?.CarOwner > 0)
                    {
                        var cid = carModel.CarOwner;
                        kh3 = await db.Customers.Where(p => p.Id == cid).Select(p => p.Name).FirstOrDefaultAsync();
                    }

                    TwiceWeighing = weighingMode.Equals("Twice");

                    // 如果有记录第一次进入称重场地的时间
                    if (unFinishWeighRecordModel.EntryTime != DateTime.MinValue)
                    {
                        var validTime = _mainSetting.Settings["WeighingValidTime"].TryGetString("0");
                        var sp = DateTime.Now.Subtract(unFinishWeighRecordModel.EntryTime);

                        //有记录，并且已经超时了,或者车有皮重，那么就将此未完成的记录设置为完成。并且将称重记录重置为1，从第一次开始。
                        if (carHasPz || sp.TotalHours >= double.Parse(validTime))
                        {
                            unFinishWeighRecordModel.IsFinish = true;
                            var autoNo = unFinishWeighRecordModel.AutoNo;
                            await db.WeighingRecords.Where(p => p.AutoNo == autoNo)
                                .ExecuteUpdateAsync(sp => sp.SetProperty(p => p.IsFinish, true));
                            _weighingRecord = new WeighingRecord()
                            {
                                Bh = unFinishWeighRecordModel.Bh,
                                Ch = unFinishWeighRecordModel.Ch,
                                Kh3 = kh3,
                                Pz = carHasPz ? carModel.VehicleWeight : unFinishWeighRecordModel.Pz,
                                WeighingTimes = carHasPz ? 2 : 1
                            };

                            WeighingTimes = _weighingRecord.WeighingTimes;
                            result.SetSuccess();
                            return result;
                        }
                    }

                    WeighingTimes = unFinishWeighRecordModel.WeighingTimes = 2;
                    unFinishWeighRecordModel.Pz = carHasPz ? carModel.VehicleWeight : unFinishWeighRecordModel.Pz;
                    // TODO: 如果确实有本地数据, 那还是以本地为准, 但是称重完成后,需要通知预约平台删除改车号的预约记录
                    _weighingRecord = AJAutoMapperService.Instance().Mapper
                        .Map<Common.EF.Tables.WeighingRecord, Common.Models.WeighingRecord>(unFinishWeighRecordModel);


                }

                if (carHasPz && WeighingTimes == 2)
                {
                    ShowLogs($"{_weighingRecord.Ch} 已录入过皮重,自动第二次称重");
                }
                result.SetSuccess();
            }
            catch (Exception e)
            {
                var msg = $"磅单创建失败:{e.Message}";
                result.SetError(msg);
                log.Error(msg, e);
            }

            return result;
        }

        /// <summary>
        /// 检测是否能上榜, 集中处理逻辑,  以后在里面在定制各个厂家的逻辑,返回true表示可以
        /// </summary>
        /// <param name="camera">为空表示是手动输入的车牌</param>
        /// <returns></returns>
        private async Task<ProcessResult> CheckCarPlateAsync()
        {
            var result = new ProcessResult();

            ShowLogs($"正在检测 {_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo}");

            // 检测重复过磅间隔
            using var ctrl = new WeighingRecordController();
            var recordOfThisCarPage = await ctrl.Pages(new WeighingRecordSearchForm
            {
                Limit = 1,
                Filter = new JObject
                {
                    {nameof(WeighingRecord.Ch),_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo }
                },
                Field = nameof(WeighingRecord.EntryTime),
                Order = "descend"
            });
            var recordOfThisCar = recordOfThisCarPage.Data.FirstOrDefault();
            var lastPlateTimerSeconds = _mainSetting.Settings["LastPlateTimer"].TryGetDouble(60);

            if (recordOfThisCar != null
                && DateTime.Now.Subtract(recordOfThisCar.EntryTime)
                .TotalSeconds < lastPlateTimerSeconds)
            {
                result.SetError($"{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 需超过 {lastPlateTimerSeconds} 秒后才能称重");
                return result;
            }

            Globalspace._lprDevNo = _currentLPRCameraDevice.IP == LPRCameraOne.IP ? "A" : "B";

            if (PlatformManager.Instance.Current is PlatformBase platform)
            {
                result = await platform.CheckCarPlateAsync(new PlatformBase.CheckCarPlateParams
                {
                    CarNo = _currentLPRCameraDevice.CarIdentificationPlateResult.CarNo,
                    WeighName = WeighName1,
                    LPRDevNo = Globalspace._lprDevNo
                });

                if (!result.Success)
                {
                    result.SetError(result.Message);
                    return result;
                }
            }

            ShowLogs($"{_currentLPRCameraDevice.CarIdentificationPlateResult.CarNo} 可以称重");

            return result;
        }

        private (bool reserveDataFilled, string ttsMsg, string logMsg) ReserveWeighCheck()
        {
            var carNo = _currentLPRCameraDevice.CarIdentificationPlateResult.CarNo;

            ShowLogs($"{carNo} 正在使用预约功能");

            var reserveDialog = new AJCommonLoadingDialogViewModel("查询预约中...", () =>
            {
                return Task.Run(async () =>
                {
                    ProcessResult ret;
                    using (var api = new Common.Utility.AJ.CloudAPI())
                    {
                        ret = await api.GetAppointRecordByCarNoAsync(carNo);
                    }
                    return ret;
                });
            });

            var dialogRet = _windowManager.ShowDialog(reserveDialog);

            var enableNotBookedCarWeigh = _mobileConfigurationMgr
                .Config.systemCfg.weighting.enableNotBookedCarWeigh;

            var ttsMsg = string.Empty;
            var logsMsg = string.Empty;
            var reserveDataFilled = false;

            if (reserveDialog.Result.Success)
            {
                var resObj = JObject.FromObject(reserveDialog.Result.Data);
                var count = resObj["totalItemCount"].ToObject<int>();
                var obj = count == 0 ? null : resObj["rows"][0];
                if (obj != null)
                {
                    var record = AJUtil
                        .TryGetJSONObject<Common.Utility.AJ.CloudAPI.AppointRecord>(obj.ToString());
                    if (record != null)
                    {
                        _weighingRecord.FillDataFromAppointRecord(record);
                        reserveDataFilled = true;
                    }
                    else
                    {
                        ttsMsg = enableNotBookedCarWeigh ? string.Empty : "未经授权,禁止上磅";
                        logsMsg = $"{carNo} 解析数据失败";
                    }
                }
                else
                {
                    ttsMsg = enableNotBookedCarWeigh ? string.Empty : "未查询预约数据";
                    logsMsg = $"{carNo} 未查询预约数据";
                }
            }
            else
            {
                ttsMsg = enableNotBookedCarWeigh ? string.Empty : "预约查询失败:接口返回错误";
                logsMsg = $"{carNo} 预约查询失败:{reserveDialog.Result.Message}";
            }

            return (reserveDataFilled, ttsMsg, logsMsg);
        }

        /// <summary>
        /// 等待读取二维码数据,和读身份证一样,一直等待直到超时或者出错
        /// </summary>
        /// <returns></returns>
        private Task<ProcessResult> ReadQRCodeWaitResultAsync()
        {
            return Task.Run(() =>
            {
                var readCount = 0;
                while (true)
                {
                    // 大概1分钟后超时退出
                    if (readCount > 120)
                    {
                        return new ProcessResult
                        {
                            Success = false,
                            Message = "读取超时"
                        };
                    }

                    if (!string.IsNullOrWhiteSpace(_qrCodeRawData))
                    {
                        return new ProcessResult
                        {
                            Success = true,
                            Data = _qrCodeRawData
                        };
                    }

                    readCount += 1;
                    Thread.Sleep(500);
                }
            });
        }

        #endregion

        private async void RefreshAWSV2Setting(bool force)
        {
            if (force)
            {
                _mainSetting = SettingsHelper.Get($"{Application.ResourceAssembly.GetName().Name}.dll", true);
                _mobileConfigurationMgr.SettingList[SettingNameKey.Main] = _mainSetting;
            }
            else
            {
                _mainSetting = _mobileConfigurationMgr.SettingList[SettingNameKey.Main];
            }
            if (string.IsNullOrWhiteSpace(WeighingControl))
            {
                WeighingControl = _mainSetting.Settings[nameof(WeighingControl)].TryGetString();
            }
            //称重模式：单次称重/二次称重
            WeighingMode = _mainSetting.Settings["WeighingMode"].TryGetString();
            KeepOpen = Convert.ToBoolean(_mainSetting.Settings["KeepOpen"]?.Value ?? "False");
            WeighingUnit = _mainSetting.Settings["WeighingUnit"].TryGetString();
            WeightValueDisplayFormat = _mainSetting.Settings[nameof(WeightValueDisplayFormat)].TryGetInt(1);
            WeighName1 = _mainSetting.Settings["WeighName"]?.Value ?? string.Empty;
            WeighName2 = _mainSetting.Settings["Weigh2Name"]?.Value ?? string.Empty;
            ShowLog = Convert.ToBoolean(_mainSetting.Settings["ShowLog"].Value);

            _manualPZOrMZ = (_mainSetting.Settings["ManualPZOrMZ"]?.Value ?? "False").Equals("true", StringComparison.OrdinalIgnoreCase);

            _fastWeigthCorrectConfig = AJUtil.TryGetJSONObject<FastWeigthCorrectConfig>(_mainSetting.Settings[nameof(FastWeigthCorrectConfig)]?.Value ?? string.Empty) ?? new FastWeigthCorrectConfig();
            _snapWatermarkConfig = AJUtil.TryGetJSONObject<SnapWatermarkConfig>(_mainSetting.Settings[nameof(SnapWatermarkConfig)]?.Value ?? string.Empty) ?? new SnapWatermarkConfig();

            _lprSvc.Start(_mainSetting);
            _gateSvc.Start(_mainSetting, _lprSvc);
            _ledSvc.Start(_mainSetting);
            _ttsSvc.Start(_mainSetting);

            YunMengZhiHuiDevice?.Close();
            YunMengZhiHuiDevice = AJUtil.TryGetJSONObject<Common.HardwareSDKS.Models.YunMengZhiHuiDeviceInfo>(_mainSetting.Settings[nameof(YunMengZhiHuiDevice)].TryGetString()) ?? new Common.HardwareSDKS.Models.YunMengZhiHuiDeviceInfo();

            // 必要, 否则无法接收二维码设备上传的数据
            if (Common.HardwareSDKS.Models.YunMengZhiHuiDeviceInfo.EventAggregator == null)
            {
                Common.HardwareSDKS.Models.YunMengZhiHuiDeviceInfo.EventAggregator = _eventAggregator;
            }

            if (YunMengZhiHuiDevice.Enable)
            {
                YunMengZhiHuiDevice.Open();

                var res = await YunMengZhiHuiDevice.SetDeviceSettingParamAsync($"{AJUtil.GetLocalIPV4()}/message/");

                if (!res.Success)
                {
                    System.Diagnostics.Trace.WriteLine($"扫码一体机设置通知回调失败:{res.Message}");
                }
            }

            #region Other

            //称重控制：手动称重/自动称重
            ManualWeighing = WeighingControl.Equals("Hand") || WeighingControl.Equals("Btn");

            _weighSerialPortService.Start(_mainSetting,
                AJUtil.TryGetSerialPortName(_mainSetting.Settings["WeighSerialPortName"]?.Value),
                AJUtil.TryGetSerialPortRate(_mainSetting.Settings["WeighSerialPortBaudRate"]?.Value));

            ////2. 外接大屏幕
            //if ((_mainSetting.Settings["LEDEnable"]?.Value ?? string.Empty).Equals("True"))
            //{
            //    //关闭大屏幕串口
            //    if (LEDSerialPort.IsOpen)
            //    {
            //        SerialPortClosing = true;
            //        while (SerialPortListening) ;
            //        LEDSerialPort.Close();
            //        SerialPortClosing = false;
            //        LEDSerialPortEnable = false;
            //    }

            //    //按照设置重新打开大屏幕串口
            //    LEDSerialPort.PortName = AJUtil.TryGetSerialPortName(_mainSetting.Settings["LEDPortName"]?.Value);
            //    LEDSerialPort.BaudRate = 2400;
            //    try
            //    {
            //        LEDSerialPort.Open();
            //        LEDSerialPortEnable = true;
            //    }
            //    catch (Exception e)
            //    {
            //        StatusBar = "大屏幕串口打开失败！" + e.Message;
            //    }
            //}
            //else
            //{
            //    if (LEDSerialPort.IsOpen)
            //    {
            //        SerialPortClosing = true;
            //        while (SerialPortListening) ;
            //        LEDSerialPort.Close();
            //        SerialPortClosing = false;
            //        LEDSerialPortEnable = false;
            //    }
            //}

            // 版本之间逻辑检测和处理
            if (Common.Share.VersionControl.CurrentVersion == Common.Share.VersionType.标准版)
            {
                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["Barrier"].Value = "1";
                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["ChargeEnable"].Value = "0";
                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["KeepOpen"].Value = "False";

                //_mobileConfigurationMgr.SettingList[SettingNameKey.VirtualWall].Settings["VirtualWall"].Value = "0";

                _mobileConfigurationMgr.SaveSetting();
                //_mobileConfigurationMgr.SaveSetting(SettingNameKey.VirtualWall);
            }

            //5. 明华桌面读卡器
            if (_mainSetting.Settings["TableRFEnable"].Value.Equals("True"))
            {
                short port = Convert.ToInt16((_mainSetting.Settings["TableRFPortName"]?.Value ?? "000").Substring(3));
                if (Globalspace._icdev.ToInt32() == 0)
                {

                    try { Globalspace._icdev = AWSV2.Services.ICCard.rf_init(--port, 9600); } catch { }
                }

                if (Globalspace._icdev.ToInt32() > 0)
                {
                    log.Debug("桌面读卡器连接成功!");
                    AWSV2.Services.ICCard.rf_beep(Globalspace._icdev, 15);
                }
                else
                    log.Debug("桌面读卡器连接失败!");
            }
            else
            {
                if (Globalspace._icdev.ToInt32() > 0)
                {
                    if (0 == AWSV2.Services.ICCard.rf_exit(Globalspace._icdev))
                    {
                        log.Debug("明华桌面读卡器关闭");
                        Globalspace._icdev = IntPtr.Zero;
                    }

                }
            }

            //6. 二维码
            if (_mainSetting.Settings["QREnable"].TryGetBoolean())
            {
                _qrCodeSerialPortService.Start(_mainSetting,
                    AJUtil.TryGetSerialPortName(_mainSetting.Settings["QRPortName"]?.Value));
            }
            else
            {
                _qrCodeSerialPortService.Stop();
            }

            if (_mainSetting.Settings["QR2Enable"].TryGetBoolean())
            {
                _qrCodeSerialPortService2.Start(_mainSetting,
                   AJUtil.TryGetSerialPortName(_mainSetting.Settings["QRPort2Name"]?.Value));
            }
            else
            {
                _qrCodeSerialPortService2.Stop();
            }

            //7. 车轴识别
            //if (_mainSetting.Settings["轮轴接收"].Value.Equals("True"))
            //{
            //    IPCHelper.OpenApp("轮轴接收");
            //}
            //else
            //{
            //    IPCHelper.CloseApp("轮轴接收");
            //}

            ////9.电子围栏开启
            //var vwSection = _mobileConfigurationMgr.SettingList[SettingNameKey.VirtualWall];
            //var virtualWallState = vwSection.Settings["VirtualWall"].Value;
            //if (!vwSection.Settings["VirtualWall"].Value.Equals("0"))//0、不启用电子围栏，1、启用但是用高频读头，2、启用但是用视频
            //{
            //    IPCHelper.OpenApp("电子围栏");
            //}
            //else
            //{
            //    IPCHelper.CloseApp("电子围栏");
            //}

            //10.数据备份中图片和视频自定义储存位置初始化，如果在用户没设置的时候这里应该是空路径，但是程序会出错，所以要初始化根目录


            var lprSection = _mobileConfigurationMgr.SettingList[SettingNameKey.Main];

            var lprSavePath = lprSection.Settings["LPRSavePath"]?.Value ?? Path.Combine(Environment.CurrentDirectory, "Snap", "pic");

            if (string.IsNullOrEmpty(lprSavePath))
            {
                lprSection.Settings["LPRSavePath"].Value = lprSavePath;
                _mobileConfigurationMgr.SaveSetting(SettingNameKey.Main);
            }
            else
            {
                try
                {
                    if (!System.IO.Directory.Exists(lprSavePath)) System.IO.Directory.CreateDirectory(lprSavePath);
                }
                catch
                {
                    lprSection.Settings["LPRSavePath"].Value = lprSavePath;
                    _mobileConfigurationMgr.SaveSetting(SettingNameKey.Main);
                }
            }

            var monitorSavePath = _mainSetting.Settings["MonitorSavePath"].TryGetString()
                ?? Path.Combine(Environment.CurrentDirectory, "Snap", "video");

            if (string.IsNullOrEmpty(monitorSavePath))
            {
                _mainSetting.Settings["MonitorSavePath"].Value = Path.Combine(Environment.CurrentDirectory, "Snap", "video");
                _mobileConfigurationMgr.SaveSetting(SettingNameKey.Main);
            }
            else
            {
                try
                {
                    if (!System.IO.Directory.Exists(monitorSavePath)) System.IO.Directory.CreateDirectory(monitorSavePath);
                }
                catch
                {
                    _mainSetting.Settings["MonitorSavePath"].Value = monitorSavePath;
                    _mobileConfigurationMgr.SaveSetting(SettingNameKey.Main);
                }
            }

            //WeighingImg = mode == "Auto" ? "/Resources/Img/xza.png" : mode == "Hand" ? "/Resources/Img/xz.png" : "/Resources/Img/btn.png";

            #endregion

        }

        public DataRowView SelectedRecrod { get; set; } //选中的称重记录，补充打印用

        private List<WeighingRecord> _wlList;
        /// <summary>
        /// 从数据库中查询出的原始称重记录
        /// </summary>
        public List<WeighingRecord> WList
        {
            get => _wlList;
            set => SetAndNotify(ref _wlList, value);
        }

        /// <summary>
        /// 存储筛选条件,  第一位表示 IsFinish, 第二位表示时间范围值 --阿吉 2024年5月8日08点55分
        /// </summary>
        private int?[] _filterParams = new int?[2];

        private string _currentTabParam;

        private bool _showWeightLineChart;
        public bool ShowWeightLineChart
        {
            get => _showWeightLineChart;
            set => SetAndNotify(ref _showWeightLineChart, value);
        }
        public void TabsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (e.AddedItems?.Count > 0)
            {
                if (e.AddedItems[0] is TabItem tabItem)
                {
                    ShowWeightLineChart = tabItem.Tag is bool;
                    if (!ShowWeightLineChart)
                    {
                        _currentTabParam = tabItem.Tag.ToString();
                        if (DynamicPoundFieldItems != null)
                        {
                            QueryList(_currentTabParam);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 查询首页称重记录数据
        /// </summary>
        /// <param name="parameter">不传或者空表示所有数据, IsFinish_xx 表示筛选完成, DateRange_xx 表示筛选entirytime 范围, xx 为空表示所有</param>
        public async void QueryList(string parameter)
        {
            DataGridLoading = true;
            try
            {
                if (string.IsNullOrWhiteSpace(parameter))
                {
                    _filterParams[0] = _filterParams[1] = null;
                    DataGridSummary.IsFinishFilter = null;
                    DataGridSummary.DateFilter = null;
                }
                else
                {
                    var array = parameter.Split('_');
                    var isFinishFlag = "isfinish".Equals(array.FirstOrDefault() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
                    var index = isFinishFlag ? 0 : 1;

                    var value = array.LastOrDefault();
                    if (string.IsNullOrEmpty(value))
                    {
                        _filterParams[index] = null;
                        if (index == 0)
                        {
                            DataGridSummary.IsFinishFilter = null;
                        }
                        else
                        {
                            DataGridSummary.DateFilter = null;
                        }
                    }
                    else
                    {
                        _filterParams[index] = value.TryGetInt();
                        if (index == 0)
                        {
                            DataGridSummary.IsFinishFilter = _filterParams[index] == 1;
                        }
                        else
                        {
                            DataGridSummary.DateFilter = (AJHomeDataGridSummary.DateRangeFilter)_filterParams[index];
                        }

                    }
                }

                var filter = new JObject
                 {
                     {nameof(WeighingRecord.By20),false },
                 };
                if (_filterParams[0].HasValue)
                {
                    filter.Add(nameof(WeighingRecord.IsFinish), _filterParams[0] == 1);
                }
                if (_filterParams[1].HasValue)
                {
                    DateTime start;
                    var end = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);

                    if (_filterParams[1] == 0)
                    {
                        start = end.Date;
                    }
                    else
                    {
                        var days = _filterParams[1].GetValueOrDefault();
                        start = end.Date.AddDays(-days);
                    }

                    filter.Add($"{nameof(WeighingRecord.EntryTime)}Start", start);
                    filter.Add($"{nameof(WeighingRecord.EntryTime)}End", end);
                }

                var searchForm = new WeighingRecordSearchForm
                {
                    Limit = -1,
                    Filter = filter
                };

                using var ctrl = new WeighingRecordController();

                var paged = await ctrl.Pages(searchForm);

                TabSummary = await ctrl.HomeSummary(new WeighingRecordSearchForm
                {
                    Limit = -1,
                    Filter = new JObject
                    {
                        {nameof(WeighingRecord.By20),false }
                    }
                });

                WList = AJAutoMapperService.Instance().Mapper
                    .Map<List<Common.EF.Tables.WeighingRecord>, List<Common.Models.WeighingRecord>>(paged.Data);

                var dt = new DataTable("WeighingRecord");
                dt.Columns.Add(new DataColumn { ColumnName = "选择", DataType = typeof(bool) });
                var customColumns = DynamicPoundFieldItems.Where(p => p.IsColumnDisplay).OrderBy(p => p.SortNo).ToList();

                foreach (var item in customColumns)
                {
                    var prop = _weighRecordModelProps.FirstOrDefault(p => p.Name.Equals(item.Field, StringComparison.OrdinalIgnoreCase));
                    if (prop == null)
                    {
                        continue;
                    }
                    var col = new DataColumn(item.Label);
                    if (prop.PropertyType == typeof(bool)
                        || prop.PropertyType == typeof(DateTime?)
                        || prop.PropertyType == typeof(DateTime))
                    {
                        col.DataType = typeof(string);
                    }
                    if (prop.PropertyType == typeof(int))
                    {
                        if (prop.Name == nameof(WeighingRecord.IsPay))
                        {
                            col.DataType = typeof(string);
                        }

                    }
                    dt.Columns.Add(col);
                }

                //APP打开后，再新增到数据库的数据，列表中没有，所以要再查一次
                //var CustomerList = new BindableCollection<Customer>(SQLDataAccess.LoadActiveCustomer(2));
                //var GoodsList = new BindableCollection<Goods>(SQLDataAccess.LoadActiveGoods(2));
                //var CarList = new BindableCollection<Car>(SQLDataAccess.LoadActiveCar(2));

                //根据选择的显示字段，处理条件筛选后获得的数据
                foreach (var wr in WList)
                {
                    var row = dt.NewRow();
                    row["选择"] = false;
                    var dataProps = wr.GetType().GetRuntimeProperties();

                    foreach (var dataProp in dataProps)
                    {
                        var fieldKeyValue = DynamicPoundFieldItems.FirstOrDefault(p => p.Field.Equals(dataProp.Name, StringComparison.OrdinalIgnoreCase));

                        if (fieldKeyValue == null || dt.Columns.IndexOf(fieldKeyValue.Label) == -1)
                        {
                            continue;
                        }

                        if (dataProp.PropertyType == typeof(bool))
                        {
                            row[fieldKeyValue.Label] = (bool)dataProp.GetValue(wr) ? "是" : "否";
                            continue;
                        }
                        if (dataProp.PropertyType == typeof(DateTime)
                            || dataProp.PropertyType == typeof(DateTime?))
                        {
                            var dateVal = dataProp.GetValue(wr);
                            row[fieldKeyValue.Label] = dateVal == null ? string.Empty : dateVal.ToString().TryGetDateTime().GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                            continue;
                        }
                        if (dataProp.PropertyType == typeof(int))
                        {
                            if (dataProp.Name == nameof(WeighingRecord.IsPay))
                            {
                                row[fieldKeyValue.Label] = wr.IsPay == 1 ? "电子支付" : wr.IsPay == 2 ? "线下支付" : wr.IsPay == 3 ? "储值支付" : wr.IsPay == 4 ? "免费放行" : "未支付";
                                continue;
                            }
                        }

                        row[fieldKeyValue.Label] = dataProp.GetValue(wr);
                    }

                    dt.Rows.Add(row);
                }

                RecordDT = dt;

                DataGridSummary.Count = WList.Count;
                DataGridSummary.Mz = Math.Round(WList.Sum(p => (double)p.Mz), 2, MidpointRounding.AwayFromZero);
                DataGridSummary.Pz = Math.Round(WList.Sum(p => (double)p.Pz), 2, MidpointRounding.AwayFromZero);
                DataGridSummary.Jz = Math.Round(WList.Sum(p => (double)p.Jz), 2, MidpointRounding.AwayFromZero);

                SelectedTotalWeighingCount = RecordDT.Rows.Count;

            }
            catch (Exception e)
            {
                log.Error($"QueryList Error:{e.Message}", e);
            }
            finally
            {
                DataGridLoading = false;
            }
        }

        public void OpenWeighLineChartDialog()
        {
            if (SelectedRecrod == null)
            {
                MessageBox.Show("请选择一条记录", "提示", MessageBoxButton.OK);
                return;
            }

            var Bh = SelectedRecrod.Row.ItemArray.ElementAtOrDefault(_BhColumnIndex)?.ToString();
            var record = WList.FirstOrDefault(w => w.Bh == Bh);

            if (record == null)
            {
                MessageBox.Show("指定的记录不存在", "提示", MessageBoxButton.OK);
                return;
            }

            var viewModel = new WeighLineChartViewModel(record);
            _windowManager.ShowDialog(viewModel);
        }

        #region 首页表格行选择功能 --阿吉 2024年4月25日10点43分

        private CheckBox _allRowCheckbox;
        private bool _allRowCheckboxProcess;

        public void RowCheckedChangedHandle(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_allRowCheckboxProcess)
            {
                return;
            }
            var cbx = ((CheckBox)sender);
            var view = cbx.DataContext as DataRowView;
            if (view == null)
            {
                return;
            }
            var orgiArray = view.Row.ItemArray;
            var copy = new object[orgiArray.Length];
            for (int i = 0; i < orgiArray.Length; i++)
            {
                copy[i] = i == 0 ? cbx.IsChecked : orgiArray[i];
            }

            view.Row.ItemArray = copy;

            var checkedCount = 0;

            foreach (DataRow row in RecordDT.Rows)
            {
                if ((bool)row[0])
                {
                    checkedCount++;
                }
            }

            _allRowCheckbox.IsThreeState = checkedCount > 0 && checkedCount != RecordDT.Rows.Count;
            if (_allRowCheckbox.IsThreeState)
            {
                _allRowCheckbox.IsChecked = null;
            }
            else
            {
                _allRowCheckbox.IsChecked = checkedCount == RecordDT.Rows.Count;
            }

            CanDeleteSelectedRows = checkedCount > 0;

        }

        public void DataGridColumnAutoGeneratingHandle(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (_weightRecordDataGrid == null)
            {
                _weightRecordDataGrid = (DataGrid)sender;
            }
            if (e.PropertyName != "选择")
            {
                e.Column.IsReadOnly = true;
            }
            else
            {
                e.Column.Visibility = Visibility.Collapsed;
            }

        }

        public void AllRowCheckBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _allRowCheckbox = (CheckBox)sender;
        }

        public void AllRowCheckedChangedHandle(object sender, System.Windows.RoutedEventArgs e)
        {
            if (RecordDT == null)
            {
                return;
            }
            _allRowCheckboxProcess = true;
            var cbx = ((CheckBox)sender);
            cbx.IsThreeState = !cbx.IsChecked.HasValue;
            if (cbx.IsChecked != null && cbx.IsChecked.HasValue)
            {
                foreach (DataRow row in RecordDT.Rows)
                {
                    row[0] = cbx.IsChecked;
                }
            }
            _allRowCheckboxProcess = false;
        }

        #endregion

        public async void Reprint()
        {
            if (SelectedRecrod == null)
            {
                StatusBar = "请选择一条记录";
            }
            else
            {
                var bh = SelectedRecrod.Row.ItemArray.ElementAtOrDefault(_BhColumnIndex)?.ToString();
                using var db = AJDatabaseService.GetDbContext();
                var dbRecord = await db.WeighingRecords.AsNoTracking().FirstOrDefaultAsync(p => p.Bh == bh);


                if (dbRecord == null) return;

                var record = AJAutoMapperService.Instance()
                    .Mapper.Map<Common.EF.Tables.WeighingRecord, Common.Models.WeighingRecord>(dbRecord);

                record.Dyrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result = await Common.Utility.PrintHelper.PrintWeighRecordAsync(record, _mainSetting, _worksheet,
                    record.IsFinish ? PrintWeighType.SecondWeighing : PrintWeighType.FirstWeighing, true);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "打印失败", MessageBoxButton.OK);
                }

            }
        }

        public void UpdateSelected()
        {
            if (SelectedRecrod == null)
            {
                MessageBox.Show("请选择一条记录", "提示", MessageBoxButton.OK);
                return;
            }

            var Bh = SelectedRecrod.Row.ItemArray.ElementAtOrDefault(_BhColumnIndex)?.ToString();
            var Record = WList.Find(w => w.Bh == Bh);

            if (Record == null)
            {
                MessageBox.Show("指定的记录不存在", "提示", MessageBoxButton.OK);
                return;
            }

            var reason = "管理员权限修改";

            if (Globalspace._currentUser.LoginId != "admin")
            {

                if (Globalspace._currentUser.Permission.Contains("修改需要密码授权"))
                {
                    var vm = new Common.ViewModels.PasswordViewModel(Globalspace._currentUser.LoginId, Common.ViewModels.PasswordConfirmType.用户授权);

                    if (!_windowManager.ShowDialog(vm).GetValueOrDefault())
                    {
                        return;
                    }

                    var promptDialog = new AJPromptDialogViewModel
                    {
                        Title = "请输入修改原因",
                        Required = true,
                        SingleLine = false,
                        ConfirmText = "确定",
                        Type = ControlStatusType.Info,
                    };

                    var promptResult = _windowManager.ShowDialog(promptDialog);

                    if (!promptResult.GetValueOrDefault())
                    {
                        return;
                    }
                    reason = promptDialog.Text;
                }
            }
            Record.EditReason = reason;

            var validator = new WeighingRecordViewModelValidator();
            var validatorAdapter = new FluentModelValidator<WeighingRecordViewModel>(validator);

            var items = new List<DynamicPoundFieldItem>();
            foreach (var item in DynamicPoundFieldItems)
            {
                if (item.IsColumnDisplay)
                {
                    items.Add(item);
                }
            }
            // 手动加上修改原因和完成状态字段,和称重记录端保持一致 --阿吉 2024年5月27日18点07分
            var editReasonField = nameof(Common.Models.WeighingRecord.EditReason);
            if (!items.Any(p => p.Field == editReasonField))
            {
                var s = _sourceFields.FirstOrDefault(p => p.key == editReasonField);
                if (s != null)
                {
                    items.Add(new DynamicPoundFieldItem
                    {
                        Field = editReasonField,
                        Label = s.value.ToString(),
                        IsColumnDisplay = true,
                        IsFilterDisplay = true,
                        IsFormDisplay = true,
                        Disabled = true,
                        FormType = VbenAdminFormType.Input.ToString()
                    });
                }

            }

            var isFinishReasonFiled = nameof(Common.Models.WeighingRecord.IsFinish);
            if (!items.Any(p => p.Field == isFinishReasonFiled))
            {
                var s = _sourceFields.FirstOrDefault(p => p.key == isFinishReasonFiled);
                if (s != null)
                {
                    items.Add(new DynamicPoundFieldItem
                    {
                        Field = isFinishReasonFiled,
                        Label = s.value.ToString(),
                        IsColumnDisplay = true,
                        IsFilterDisplay = true,
                        IsFormDisplay = true,
                        FormType = VbenAdminFormType.Select.ToString(),
                        Options = s.children?.Count > 0
                        ? new ObservableCollection<AJMultiSelectOptionItem>(s.children.Select(q => new AJMultiSelectOptionItem
                        {
                            Label = q.label,
                            Value = q.value,
                        }))
                        : new ObservableCollection<AJMultiSelectOptionItem>()
                    });
                }
            }

            var viewModel = new WeighingRecordViewModel(validatorAdapter, _sourceFields,
                items,
                _windowManager, Record);
            var result = _windowManager.ShowDialog(viewModel);

            if (result.GetValueOrDefault(true))
            {
                QueryList(_currentTabParam);
            }
        }

        private bool _canDeleteSelectedRows;
        public bool CanDeleteSelectedRows
        {
            get => _canDeleteSelectedRows;
            set => SetAndNotify(ref _canDeleteSelectedRows, value);
        }
        public async void DeleteSelectedRows()
        {
            var ids = new List<string>();
            foreach (DataRow row in RecordDT.Rows)
            {
                if ((bool)row[0])
                {
                    ids.Add(row[_BhColumnIndex].ToString());
                }
            }

            if (ids.Count == 0)
            {
                MessageBox.Show("请先选择要删除的记录");
                return;
            }

            var result = _windowManager.ShowDialog(new Common.ViewModels.PasswordViewModel(Globalspace._currentUser.LoginId,
                Common.ViewModels.PasswordConfirmType.用户验证, true));

            var platform = Common.Platform.PlatformManager.Instance.Current as PlatformBase;
            if (result.GetValueOrDefault())
            {
                var deleted = new List<string>();
                using var db = AJDatabaseService.GetDbContext();
                var mapper = AJAutoMapperService.Instance().Mapper;
                var records = await db.WeighingRecords.Where(p => ids.Contains(p.Bh)).ToListAsync();
                foreach (var bh in ids)
                {
                    var record = records.FirstOrDefault(p => p.Bh == bh);
                    if (record == null)
                    {
                        MessageBox.Show($"{bh} 不存在", "删除失败", MessageBoxButton.OK);
                        break;
                    }

                    if (platform == null)
                    {
                        deleted.Add(bh);
                        continue;
                    }
                    else
                    {
                        var deleteResult = await platform.DeleteWeighRecordAsync(new PlatformBase.DeleteWeighRecordParams
                        {
                            WeighRecord = mapper
                            .Map<Common.EF.Tables.WeighingRecord, Common.Models.WeighingRecord>(record)
                        });
                        if (!deleteResult.Success)
                        {
                            MessageBox.Show(deleteResult.Message, "删除失败", MessageBoxButton.OK);
                            break;
                        }
                        deleted.Add(bh);
                    }

                }

                var delRet = await db.WeighingRecords.Where(p => deleted.Contains(p.Bh)).ExecuteDeleteAsync();

                MessageBox.Show(delRet > 0 ? $"删除成功:{deleted.Count} 条记录" : $"删除失败:未找到任何数据", "提示", MessageBoxButton.OK);
                QueryList(_currentTabParam);
            }
        }

        public async void DeleteSelected()
        {
            try
            {
                if (SelectedRecrod == null)
                {
                    StatusBar = "请选择一条记录";
                    return;
                }

                var result = _windowManager.ShowDialog(new Common.ViewModels.PasswordViewModel(Globalspace._currentUser.LoginId,
                Common.ViewModels.PasswordConfirmType.用户验证, true));

                if (!result.GetValueOrDefault())
                {
                    return;
                }

                var Bh = SelectedRecrod.Row.ItemArray.ElementAtOrDefault(_BhColumnIndex)?.ToString();
                var Record = WList.Find(w => w.Bh == Bh);
                if (Record == null)
                {
                    MessageBox.Show("指定的记录不存在", "提示", MessageBoxButton.OK);
                    return;
                }

                var platform = Common.Platform.PlatformManager.Instance.Current as PlatformBase;
                if (platform != null)
                {
                    var deleteResult = await platform.DeleteWeighRecordAsync(new PlatformBase.DeleteWeighRecordParams
                    {
                        WeighRecord = (Common.Models.WeighingRecord)Record
                    });
                    if (!deleteResult.Success)
                    {
                        MessageBox.Show(deleteResult.Message, "删除失败", MessageBoxButton.OK);
                        return;
                    }
                }
                using var db = AJDatabaseService.GetDbContext();
                await db.WeighingRecords.Where(p => p.Bh == Bh).ExecuteDeleteAsync();
                MessageBox.Show($"删除成功", "提示", MessageBoxButton.OK);
                QueryList(_currentTabParam);

            }
            catch (Exception ex)
            {
                log.Info("DeleteSelected error:" + ex.Message);
            }

        }
        public void InsertItem()
        {
            var validator = new WeighingRecordViewModelValidator();
            var validatorAdapter = new FluentModelValidator<WeighingRecordViewModel>(validator);

            var items = new List<DynamicPoundFieldItem>();
            foreach (var item in DynamicPoundFieldItems)
            {
                if (item.IsColumnDisplay)
                {
                    items.Add(item);
                }
            }
            // 手动加上修改原因和完成状态字段,和称重记录端保持一致 --阿吉 2024年5月27日18点07分
            var editReasonField = nameof(Common.Models.WeighingRecord.EditReason);
            if (!items.Any(p => p.Field == editReasonField))
            {
                var s = _sourceFields.FirstOrDefault(p => p.key == editReasonField);
                if (s != null)
                {
                    items.Add(new DynamicPoundFieldItem
                    {
                        Field = editReasonField,
                        IsColumnDisplay = true,
                        IsFilterDisplay = true,
                        IsFormDisplay = true,
                        Label = s.value.ToString(),
                        FormType = VbenAdminFormType.Input.ToString()
                    });
                }

            }

            var isFinishReasonFiled = nameof(Common.Models.WeighingRecord.IsFinish);
            if (!items.Any(p => p.Field == isFinishReasonFiled))
            {
                var s = _sourceFields.FirstOrDefault(p => p.key == isFinishReasonFiled);
                if (s != null)
                {
                    items.Add(new DynamicPoundFieldItem
                    {
                        Field = isFinishReasonFiled,
                        Label = s.value.ToString(),
                        FormType = VbenAdminFormType.Select.ToString(),
                        IsColumnDisplay = true,
                        IsFilterDisplay = true,
                        IsFormDisplay = true,
                        Options = s.children?.Count > 0
                        ? new ObservableCollection<AJMultiSelectOptionItem>(s.children.Select(q => new AJMultiSelectOptionItem
                        {
                            Label = q.label,
                            Value = q.value,
                        }))
                        : new ObservableCollection<AJMultiSelectOptionItem>()
                    });
                }
            }

            var viewModel = new WeighingRecordViewModel(validatorAdapter, _sourceFields,
                items,
                _windowManager, null);
            var result = _windowManager.ShowDialog(viewModel);
            if (result.GetValueOrDefault(true))
            {
                QueryList(_currentTabParam);
            }
        }

        public async void OpenPicFolder()
        {
            string path = _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["LPRSavePath"].Value;
            //string path = Environment.CurrentDirectory + "/Snap/";
            if (SelectedRecrod != null)
            {
                var bh = SelectedRecrod.Row.ItemArray.ElementAtOrDefault(_BhColumnIndex)?.ToString();
                using var db = AJDatabaseService.GetDbContext();
                var ch = await db.WeighingRecords.Where(p => p.Bh == bh).Select(p => p.Ch).FirstOrDefaultAsync();
                if (string.IsNullOrEmpty(ch))
                {
                    MessageBox.Show("未找到该记录的图片信息", "提示", MessageBoxButton.OK);
                    return;
                }

                string path2 = path + "\\" + ch + "\\" + bh;
                if (Directory.Exists(path2))
                {
                    Globalspace.ImagsSource = path2;
                    //Process.Start(path2);
                    _windowManager.ShowDialog(new PicListViewModel(_windowManager, path2));
                }
                else
                {
                    MessageBox.Show("未找到该记录的图片信息", "提示", MessageBoxButton.OK);
                }
                //else if (Directory.Exists(path))
                //{
                //    Process.Start(path);
                //}
                Globalspace.ImagsSource = string.Empty;
            }
            else
            {
                if (Directory.Exists(path)) Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true,
                });
            }

        }

        /// <summary>
        /// 接收到称重串口数据处理
        /// </summary>
        /// <param name="data"></param>
        private void WeighSerialPort_DataReceived(WeighSerialPortService.WeighSerialPortEventData data)
        {
            if (string.IsNullOrWhiteSpace(data?.WeightValue))
            {
                return;
            }
            var essentialNum = _mainSetting.Settings["EssentialNum"].TryGetDecimal();

            var minSlotWeight = _mainSetting.Settings["MinSlotWeight"].TryGetDecimal();

            data.WeightValue = (data.WeightValue.TryGetDecimal() * essentialNum).ToString("0.##");
            // 新增自动转换单位逻辑  --阿吉 2023年6月12日16点40分
            WeightStr = WeightStr1 = AutoConvertUnit(data.WeightValue).ToString($"F{WeightValueDisplayFormat}");

            //如果小于最小重量，且当前状态为称重结束。那么就要关闭已经打开的收费窗口，因为这个时候车子已经走了
            //收费窗口还在，会影响下一辆车的称重。
            if (Math.Abs(WeightStr1.TryGetDecimal()) <= minSlotWeight
                && CurrentStatus == Common.Model.Custom.WeighStatus.WeighEnd)
            {
                CarImageVisible = false;
            }

        }
        public string CurrentPlate { get; set; }

        /// <summary>
        /// 新增自动转换单位逻辑 --阿吉 2023年6月12日17点13分
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private decimal AutoConvertUnit(string value)
        {
            decimal.TryParse(value, out var number);

            var digits = (decimal)Math.Pow(10, WeightValueDisplayFormat);

            number = (Math.Truncate(number * digits) / digits);

            var autoConvertUnitVal = _mainSetting.Settings["AutoConvertUnit"].Value;
            if ("true".Equals(autoConvertUnitVal, StringComparison.OrdinalIgnoreCase))
            {
                var unitVal = _mainSetting.Settings["WeighingUnit"].Value;

                return "kg".Equals(unitVal, StringComparison.OrdinalIgnoreCase)
                    ? (number * 1000) : (number / 1000);
            }

            return number;
        }

        /// <summary>
        /// 根据LightType 0：称重完成亮绿灯 ，1：车下磅后绿灯 发送红绿灯指令给LPR
        /// </summary>
        private void LPR_READY_TO_WEIGH()
        {
            //IPCHelper.SendMsgToApp("车牌识别", IPCHelper.READY_TO_WEIGH);
            //lprMessage("", "", "", "", IPCHelper.READY_TO_WEIGH);
            //LightByType();
        }

        /// <summary>
        /// type  0：称重完成亮绿灯 ，1：车下磅后绿灯。 发送红绿灯指令给LPR
        /// </summary>
        /// <param name="type"></param>
        private void LightByType(string type = "0")
        {
            if (_mainSetting.Settings["LightType"].Value == type)
            {
                var limit = (_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0").TryGetDecimal();
                if (type == "1" && WeightStr1.TryGetDecimal() > limit)
                {
                    IPCHelper.SendMsgToApp("车牌识别", 0x08FC); //红灯，道闸全落
                    log.Debug("发送命令：红灯，道闸全落");
                    CarImageVisible = true;
                    SignalLightGreenOne = SignalLightGreenTwo = false;
                    return;
                }

                IPCHelper.SendMsgToApp("车牌识别", 0x08F0); //绿灯，道闸全落
                SignalLightGreenOne = SignalLightGreenTwo = true;

                log.Debug("发送命令：绿灯，道闸全落");
            }
            //else
            //{
            //    IPCHelper.SendMsgToApp("车牌识别", 0x08FC); //红灯，道闸全落
            //    log.Debug("发送命令：红灯，道闸全落");
            //}

            CarImageVisible = false;
        }

        /// <summary>
        /// 添加首页日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="allowRepeat">是否允许重复添加相同日志内容。默认:不允许 False</param>
        private void ShowLogs(string content, bool allowRepeat = false)
        {
            try
            {
                if (LogContent.Count > 35)
                {
                    LogContent.Clear();
                }

                if (!LogContent.Any(p => p == content) || allowRepeat)
                {
                    LogContent.Add(content);
                }

            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }
        }

        /// <summary>
        /// 修改首页日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="allowRepeat">是否允许重复添加相同日志内容。默认:不允许 False</param>
        private bool EditLogs(string oldcontent, string newcontent)
        {
            try
            {
                for (int i = 0; i < LogContent.Count; i++)
                {
                    if (LogContent[i] == oldcontent)
                    {
                        LogContent[i] = newcontent;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 修改首页日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="allowRepeat">是否允许重复添加相同日志内容。默认:不允许 False</param>
        private bool DelLogs(string content)
        {
            try
            {
                LogContent.Remove(content);
                return false;
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return false;
            }
        }

        private void CopyValues<T>(T target, T source)
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }


        private bool CanManualWeighBase
        {
            get
            {
                return ManualWeighing
                    && EnableWeighing
                    && !string.IsNullOrWhiteSpace(WeightStr)
                    && (WeightStr.TryGetDecimal() > _mainSetting.Settings["MinSlotWeight"].TryGetDecimal())
                    && _gateSvc.RasterCheckResult?.Success == true;
            }
        }

        public bool CanFirstWeighing
        {
            get
            {
                return WeighingTimes == 1 && CanManualWeighBase;
            }

        }

        public bool CanSecondWeighing
        {
            get
            {
                return WeighingTimes == 2 && CanManualWeighBase;
            }
        }

        public bool CanOnceWeighing
        {
            get
            {
                return CanFirstWeighing;
            }
        }

        public bool CanWeighWithUpdateCar
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_currentLPRCameraDevice?.CarIdentificationPlateResult?.CarNo) && CanManualWeighBase;
            }

        }

        private static string _defaultBtn1Txt = "一次称重";
        private static string _defaultBtn2Txt = "二次称重";

        public string Btn1Text { get; set; } = _defaultBtn1Txt;

        public string Btn2Text { get; set; } = _defaultBtn2Txt;

        public string Btn3Text { get; set; } = "称重";

        private string _manualPZText = "皮重";
        public string ManualPZText
        {
            get { return _manualPZText; }
            set
            {
                _manualPZText = value;
                NotifyOfPropertyChange(nameof(ManualPZText));
            }
        }

        private string _manualMZText = "毛重";
        public string ManualMZText
        {
            get { return _manualMZText; }
            set
            {
                _manualMZText = value;
                NotifyOfPropertyChange(nameof(ManualMZText));
            }
        }

        private bool _cameraGPIOCovered = false;
        private bool _isCheckGrating = false;
        private Prompt _currentPrompt;

        private System.Windows.Controls.Button _firstWeighingBtn;
        public void FirstWeighingLoadedCmd(object sender, RoutedEventArgs e)
        {
            _firstWeighingBtn = (System.Windows.Controls.Button)sender;
            _descriptor.AddValueChanged(_firstWeighingBtn, WeightBtnIsEnableChange);
        }

        /// <summary>
        /// 称重之前检测有效重量，返回false阻止称重
        /// </summary>
        /// <returns></returns>
        private bool WeighPreCheck()
        {
            var array = new decimal[] { WeightStr.TryGetDecimal(),
                WeightStr1.TryGetDecimal(),
                WeightStr2.TryGetDecimal()
            };

            var effectiveWeight = _mainSetting.Settings["EffectiveWeight"].TryGetDecimal();

            var result = true;

            // 没有大于等于有效重量的话, 要阻止称重 --阿吉 2024年6月22日15点18分
            if (!array.Any(p => p >= effectiveWeight))
            {
                ShowLogs($"有效重量检测失败:{effectiveWeight}");
                result = false;
            }
            else if (!array.Any(p => p > 0))
            {
                ShowLogs($"不存在任何可用重量值");
                result = false;
            }

            if (!result)
            {
                // 去除稳定标记和光栅检测标记，重启稳定检测worker
                IsDelayNZStable = false;
                _gateSvc.RasterCheckResult = null;
                RunWeightStableWorker();
            }
            else
            {
                _currentLPRCameraDevice.CarIdentificationPlateResult.Weight = array.FirstOrDefault();
                _currentLPRCameraDevice.CarIdentificationPlateResult.Weight1 = array.ElementAtOrDefault(1);
                _currentLPRCameraDevice.CarIdentificationPlateResult.Weight2 = array.LastOrDefault();
            }

            return result;
        }

        private async Task<ProcessResult> WeighProcessCoreAsync()
        {
            var result = new ProcessResult();
            WeighFormVM.Submit();
            var weighingRecord = AJUtil.TryGetJSONObject<WeighingRecord>(AJUtil.AJSerializeObject(this._weighingRecord));

            ManualPZOrMZBtnsVisible = false;
            ShowLogs($"{weighingRecord.Ch} 正在保存数据");

            #region 表单数据预处理
            //没输入车号，提示并返回，车牌号被禁用，提示并返回，新车牌，先保存到车辆表
            weighingRecord.Ch = weighingRecord.Ch?.Trim();
            if (string.IsNullOrWhiteSpace(weighingRecord.Ch))
            {
                result.SetError("车号数据有误");
                return result;
            }

            //如果客户不存在,则新增
            weighingRecord.Kh = weighingRecord.Kh?.Trim();
            weighingRecord.Kh2 = weighingRecord.Kh2?.Trim();
            weighingRecord.Kh3 = weighingRecord.Kh3?.Trim();

            var customerArray = new string[] {
                weighingRecord.Kh,
                weighingRecord.Kh2,
                weighingRecord.Kh3
            }.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            var customers = customerArray.Length > 0 
                ? new List<Common.EF.Tables.Customer>() : null;

            foreach (var name in customerArray)
            {
                var customerData = await SaveCustomerIfNotExistsAsync(name);
                if (customerData == null)
                {
                    continue;
                }
                customers.Add(customerData);
            }

            // 如果物资不存在则新增
            weighingRecord.Wz = weighingRecord.Wz?.Trim();
            Common.EF.Tables.Goods goods = null;
            if (!string.IsNullOrWhiteSpace(weighingRecord.Wz))
            {
                using var db = AJDatabaseService.GetDbContext();
                var wz = weighingRecord.Wz;
                goods = await db.Goods.AsNoTracking().FirstOrDefaultAsync(p => p.Name == wz);
                if (goods == null)
                {
                    goods = new Common.EF.Tables.Goods
                    {
                        Id  = YitIdHelper.NextId(),
                        Name = weighingRecord.Wz
                    };
                    goods.Num  = goods.Id.ToString();
                    db.Goods.Add(goods);
                    db.SaveChanges();
                }
            }

            // 如果不等于0， 则表示是手动输入的 --阿吉 2023年11月15日11点21分
            if (weighingRecord.GoodsPrice == 0 && (customers?.Any(p => p != null)).GetValueOrDefault())
            {
                weighingRecord.GoodsPrice = await GetGoodsPriceAsync(customers.FirstOrDefault(p => p.Id > 0), goods);
            }

            result.SetSuccess(weighingRecord);
            return result;
        }

        private async Task<Common.EF.Tables.Customer> SaveCustomerIfNotExistsAsync(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return null;
            }
            using var db = AJDatabaseService.GetDbContext();
            var customer = await db.Customers.FirstOrDefaultAsync(p => p.Name == customerName);
            if (customer == null)
            {
                customer = new Common.EF.Tables.Customer
                {
                    Id = YitIdHelper.NextId(),
                    Name = customerName
                };
                customer.Num = customer.Id.ToString();
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
            }
            return customer;
        }

        public async void FirstWeighing()
        {
            if (!WeighPreCheck())
            {
                return;
            }

            var result = await WeighProcessCoreAsync();
            if (!result.Success)
            {
                ShowLogs(result.Message);
                return;
            }

            var plateResult = _currentLPRCameraDevice.CarIdentificationPlateResult;

            var weighingRecord = result.Data as WeighingRecord;

            weighingRecord.Bh = await WeighingRecord.CreateBhAsync();
            // 如果没有启用手动皮重/毛重,或者皮重未赋值(来自SetPZCmd),则毛重赋值,
            var normalWeigh = !_manualPZOrMZ || weighingRecord.Pz == 0;
            if (normalWeigh)
            {
                weighingRecord.Mz = plateResult.Weight;
                weighingRecord.Mzrq = DateTime.Now;
                weighingRecord.Mzsby = Globalspace._currentUser.UserName;
            }

            string discount = _mainSetting.Settings["Discount"].Value;
            if (discount.Equals("1"))
            {
                weighingRecord.Kz = _mainSetting.Settings["DiscountWeight"].TryGetDecimal();
            }
            else if (discount.Equals("2"))
            {
                weighingRecord.Kl = _mainSetting.Settings["DiscountRate"].TryGetDecimal();
            }

            if (plateResult.Weight == plateResult.Weight1)
            {
                weighingRecord.WeighName = _mainSetting.Settings["WeighName"].Value;
            }
            else if (plateResult.Weight == plateResult.Weight2)
            {
                weighingRecord.WeighName = _mainSetting.Settings["Weigh2Name"].Value;
            }
            weighingRecord.WeighingTimes = 1;
            weighingRecord.IsFinish = false;
            weighingRecord.WeighingFormTemplate = _mainSetting.Settings["PrintTemplate"].Value;

            #endregion

            weighingRecord.EntryTime = DateTime.Now;//第一次称重的，记录出称重时间
            weighingRecord.SerialNumber = YitIdHelper.NextId().ToString();

            weighingRecord.ComputeCost(1);
            if (weighingRecord.Je > 0)
            {
                _ttsSvc.Speak(weighingRecord, TTSConfig.TTSTextType.识别到车牌, $"请缴费{weighingRecord.Je.ToString("0.00")}元！");
                ShowLogs($"本次费用：{weighingRecord.Je.ToString("0.00")} 元");
            }
            using var ctrl = new WeighingRecordController();

            try
            {
                await ctrl.Save(AJAutoMapperService.Instance().Mapper.Map<Common.Models.WeighingRecord, Common.EF.Tables.WeighingRecord>(weighingRecord));
            }
            catch (Exception e)
            {
                ShowLogs($"{weighingRecord.Ch} 数据保存失败:{e.Message}");
                return;
            }

            ShowLogs($"{weighingRecord.Ch} 数据保存完成");

            if (_mainSetting.Settings["车牌识别"].Value == "2")
            {
                CopyImg(weighingRecord);
            }

            if (Convert.ToBoolean(_mainSetting.Settings["WithPrinting"].TryGetBoolean()))
            {
                weighingRecord.Dyrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var printResult = await Common.Utility.PrintHelper.PrintWeighRecordAsync(weighingRecord,
                    _mainSetting, _worksheet, PrintWeighType.FirstWeighing);
                if (!printResult.Success)
                {
                    MessageBox.Show(printResult.Message, "打印失败", MessageBoxButton.OK);
                }
            }

            WeighingTimes = 0;
            CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;

            WeightComplete(weighingRecord, true);
        }

        public void SetPZCmd()
        {
            _weighingRecord.Pz = WeightStr.TryGetDecimal();
            _weighingRecord.Pzrq = DateTime.Now;
            _weighingRecord.Pzsby = Globalspace._currentUser.UserName;
            FirstWeighing();
        }


        private System.Windows.Controls.Button _secondWeighingBtn;
        public void SecondWeighingLoadedCmd(object sender, RoutedEventArgs e)
        {
            _secondWeighingBtn = (System.Windows.Controls.Button)sender;
            _descriptor.AddValueChanged(_secondWeighingBtn, WeightBtnIsEnableChange);
        }

        private void ComputeByFieldsValue(WeighingRecord weighingRecord)
        {
            var total = 40;
            var formula = string.Empty;
            var field = string.Empty;

            var props = weighingRecord.GetType().GetRuntimeProperties();


            var context = new ExpressionContext();
            context.Imports.AddType(typeof(Math));

            for (int i = 1; i <= total; i++)
            {
                field = $"By{i}";
                formula = _mainSetting.Settings[$"{field}Formula"].TryGetString();
                if (string.IsNullOrWhiteSpace(formula))
                {
                    continue;
                }
                var prop = props.FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));
                if (prop == null)
                {
                    continue;
                }

                prop.SetValue(weighingRecord, FormulaCalc(weighingRecord, formula, context, props));
            }
        }

        public async void SecondWeighing()
        {
            if (!WeighPreCheck())
            {
                return;
            }

            var result = await WeighProcessCoreAsync();
            if (!result.Success)
            {
                ShowLogs(result.Message);
                return;
            }

            var plateResult = _currentLPRCameraDevice.CarIdentificationPlateResult;

            var weighingRecord = result.Data as WeighingRecord;

            //根据重量大小，设置毛重、皮重的数据
            //这个判断是为了能保存用户手动输入的值
            if (weighingRecord.Mz == 0)
            {
                weighingRecord.Mz = plateResult.Weight;
                weighingRecord.Mzrq = DateTime.Now;
            }

            //这个判断是为了能保存用户手动输入的值
            if (weighingRecord.Pz == 0)
            {
                weighingRecord.Pz = plateResult.Weight;
                weighingRecord.Pzrq = DateTime.Now;
            }

            if (string.IsNullOrWhiteSpace(weighingRecord.Gblx))
            {
                weighingRecord.Gblx = "其他";
            }
            var mz = weighingRecord.Mz;
            var pz = weighingRecord.Pz;
            if (mz < pz)
            {
                var temp = weighingRecord.Mz;
                var temp_rq = weighingRecord.Mzrq;
                weighingRecord.Mz = weighingRecord.Pz;
                //_weighingRecord.Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                weighingRecord.Mzrq = weighingRecord.Pzrq;
                weighingRecord.Mzsby = Globalspace._currentUser.UserName;

                weighingRecord.Pz = temp;
                //_weighingRecord.Pzrq = _weighingRecord.Mzrq;
                weighingRecord.Pzrq = temp_rq;
                weighingRecord.Pzsby = weighingRecord.Mzsby;

                weighingRecord.IsPZAndMZExchanged = true;

            }
            var autoWeighingType = (_mainSetting.Settings["AutoWeighingType"]?.Value ?? "0").Equals("1");
            if (autoWeighingType)
            {
                //这个判断是为了验证，用户或者机器自动生成的 毛重是否小于皮重，小于则互换。不能出现毛重小于皮重的情况
                if (mz < pz)
                {
                    if (!autoWeighingType || Convert.ToDecimal(weighingRecord.Mz) == plateResult.Weight)
                        weighingRecord.Gblx = "其他";
                    else
                        weighingRecord.Gblx = "销售";
                }
                else
                {
                    //_weighingRecord.Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    weighingRecord.Pzsby = Globalspace._currentUser.UserName;
                    weighingRecord.Gblx = "采购";
                }
            }

            weighingRecord.Jz = Math.Abs(weighingRecord.Mz - weighingRecord.Pz);
            weighingRecord.Jzrq = DateTime.Now;

            string discount = _mainSetting.Settings["Discount"].Value;
            if (discount.Equals("1"))
            {
                weighingRecord.Sz = weighingRecord.Jz - weighingRecord.Kz;
            }
            else if (discount.Equals("2"))
            {
                weighingRecord.Sz = weighingRecord.Jz * ((100 - weighingRecord.Kl) / 100);
            }
            else
            {
                weighingRecord.Sz = weighingRecord.Jz;
            }

            //_weighingRecord.By3 = NumFormatHelper.NumToChn(_weighingRecord.Jz) + "吨";

            //备用字段公式运算
            ComputeByFieldsValue(weighingRecord);

            //更新IC卡的备用字段信息
            if (weighingRecord.ICCard != null)
            {
                using var db = AJDatabaseService.GetDbContext();
                var cardNo = weighingRecord.ICCard;
                var ic = await db.ICCards.FirstOrDefaultAsync(p => p.CarNo == cardNo);
                if (ic != null)
                {
                    var props = ic.GetType().GetRuntimeProperties();
                    var icUpdated = false;
                    var recordProps = weighingRecord.GetType().GetRuntimeProperties();

                    foreach (var prop in props)
                    {
                        if (!prop.Name.Contains("By"))
                        {
                            continue;
                        }
                        var byProp = recordProps.FirstOrDefault(p => p.Name == prop.Name);
                        if (byProp == null)
                        {
                            continue;
                        }
                        var cfgKey = $"{prop.Name}Formula";
                        if (_mainSetting.Settings.AllKeys.Any(p => p.Equals(cfgKey, StringComparison.OrdinalIgnoreCase)))
                        {
                            var cfgVal = _mainSetting.Settings[cfgKey].TryGetString();
                            if (!string.IsNullOrWhiteSpace(cfgVal))
                            {
                                prop.SetValue(ic, byProp.GetValue(weighingRecord));
                                icUpdated = true;
                            }
                        }
                    }
                    if (icUpdated)
                    {
                        db.Entry(ic).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
            }

            if (plateResult.Weight == plateResult.Weight1)
            {
                weighingRecord.Weigh2Name = _mainSetting.Settings["WeighName"].Value;
            }
            else if (plateResult.Weight == plateResult.Weight2)
            {
                weighingRecord.Weigh2Name = _mainSetting.Settings["Weigh2Name"].Value;
            }
            weighingRecord.WeighingTimes = 2;
            weighingRecord.IsFinish = true;



            weighingRecord.ComputeCost(2);//第二次收费的金额
            if (weighingRecord.Je > 0)
            {
                _ttsSvc.Speak(weighingRecord, TTSConfig.TTSTextType.识别到车牌, $"请缴费{weighingRecord.Je.ToString("0.00")}元！");
                ShowLogs($"本次费用：{weighingRecord.Je.ToString("0.00")} 元");
            }


            CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;
            var testVal = string.Empty;

            if (_mainSetting.Settings["车牌识别"].Value == "2")
            {
                CopyImg(weighingRecord);
            }

            //判断扣重、扣率是否都填写了。如果都填写了。需要提示。
            if (weighingRecord.Kz != 0 && weighingRecord.Kl != 0)
            {
                _windowManager.ShowMessageBox("扣重、扣率只允许有一个，请处理！");
                return;
            }
            else
            {
                //计算实重多少
                if (weighingRecord.Kz > 0)
                {
                    weighingRecord.Sz = weighingRecord.Jz - weighingRecord.Kz;
                }
                else if (weighingRecord.Kl > 0)
                {
                    //  jz*kl=sz
                    weighingRecord.Sz = weighingRecord.Jz - (weighingRecord.Jz) * (weighingRecord.Kl / 100);
                }
            }
            //2022-09-06 如果是光栅遮挡了，还要保存的，就要将车牌号码后面追加“?”来区分 Globalspace.CurrentBtnContent
            if (Globalspace.CurrentBtnContent.Contains("遮挡") && !weighingRecord.Ch.Contains("?"))
            {
                weighingRecord.IsCover = true;
            }

            if (string.IsNullOrWhiteSpace(weighingRecord.Bh))
            {
                weighingRecord.Bh = await WeighingRecord.CreateBhAsync();
                var now = DateTime.Now;
                weighingRecord.Mzrq = weighingRecord.Pzrq = now;
                weighingRecord.Mzsby = weighingRecord.Pzsby = Globalspace._currentUser.UserName;
                weighingRecord.SerialNumber = YitIdHelper.NextId().ToString();
                weighingRecord.EntryTime = now;
            }

            using var ctrl = new WeighingRecordController();
            try
            {
                await ctrl.Save(AJAutoMapperService.Instance().Mapper.Map<Common.Models.WeighingRecord, Common.EF.Tables.WeighingRecord>(weighingRecord));
            }
            catch (Exception e)
            {
                ShowLogs($"{weighingRecord.Ch} 数据保存失败:{e.Message}");
                return;
            }


            //写图片视频文件地址到数据库
            ShowLogs($"检测到车牌：{weighingRecord.Ch} 数据保存完成");

            if (Convert.ToBoolean(_mainSetting.Settings["WithPrinting"]?.Value ?? "False"))
            {
                weighingRecord.Dyrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var printResult = await Common.Utility.PrintHelper.PrintWeighRecordAsync(weighingRecord,
                    _mainSetting, _worksheet, PrintWeighType.SecondWeighing);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "打印失败", MessageBoxButton.OK);
                }

            }


            StatusBar = "第二次称重完成";
            WeighingTimes = 0;
            //CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;

            //SetImg("second", weighingRecord.Ch, weighingRecord.Bh);

            RemoveICCardAsync(weighingRecord);

            WeightComplete(weighingRecord, true);
        }

        public async void WeighWithUpdateCar()
        {
            var dialogRet = MessageBox.Show("确定存皮下磅吗？", "提示", MessageBoxButton.OKCancel);

            if (dialogRet != MessageBoxResult.OK)
            {
                return;
            }

            var weight = WeightStr1.TryGetDecimal();
            using var db = AJDatabaseService.GetDbContext();
            var ch = _weighingRecord.Ch;
            var carModel = await db.Cars.FirstOrDefaultAsync(p => p.PlateNo == ch);

            if (carModel == null)
            {
                carModel = new Common.EF.Tables.Car
                {
                    PlateNo = _weighingRecord.Ch,
                    VehicleWeight = weight,
                };
            }
            else
            {
                carModel.VehicleWeight = weight;
            }
            db.Entry(carModel).State = EntityState.Modified;
            await db.SaveChangesAsync();

            CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;

            WeightComplete(_weighingRecord, false);
        }

        private System.Windows.Controls.Button _onceWeighingBtn;
        public void OnceWeighingLoadedCmd(object sender, RoutedEventArgs e)
        {
            _onceWeighingBtn = (System.Windows.Controls.Button)sender;
            _descriptor.AddValueChanged(_onceWeighingBtn, WeightBtnIsEnableChange);
        }

        public async void OnceWeighing()
        {
            if (!WeighPreCheck())
            {
                return;
            }

            var result = await WeighProcessCoreAsync();
            if (!result.Success)
            {
                ShowLogs(result.Message);
                return;
            }

            var plateResult = _currentLPRCameraDevice.CarIdentificationPlateResult;

            var weighingRecord = result.Data as WeighingRecord;

            weighingRecord.Bh = await WeighingRecord.CreateBhAsync();

            if (weighingRecord.Mz == 0)
                weighingRecord.Mz = plateResult.Weight;

            weighingRecord.Mzrq = DateTime.Now;
            weighingRecord.Mzsby = Globalspace._currentUser.UserName;

            //皮重首先由输入车号时带出来，然后如果有手动修改，则取手动修改后的值
            weighingRecord.Pzrq = DateTime.Now;
            weighingRecord.Pzsby = Globalspace._currentUser.UserName;

            if (string.IsNullOrWhiteSpace(weighingRecord.Gblx))
            {
                weighingRecord.Gblx = "其他";
            }
            var mz = weighingRecord.Mz;
            var pz = weighingRecord.Pz;
            if (mz < pz)
            {
                var temp = weighingRecord.Mz;
                weighingRecord.Mz = weighingRecord.Pz;
                weighingRecord.Pz = temp;

            }
            var autoWeighingType = (_mainSetting.Settings["AutoWeighingType"]?.Value ?? "0").Equals("1");
            if (autoWeighingType)
            {
                //这个判断是为了验证，用户或者机器自动生成的 毛重是否小于皮重，小于则互换。不能出现毛重小于皮重的情况
                if (mz < pz)
                {
                    weighingRecord.Gblx = mz == plateResult.Weight ? "其他" : "销售";
                }
                else
                {
                    weighingRecord.Gblx = "采购";
                }
            }

            weighingRecord.Jz = Math.Abs(weighingRecord.Mz - weighingRecord.Pz);
            weighingRecord.Jzrq = DateTime.Now;


            string discount = _mainSetting.Settings["Discount"].Value;
            if (discount.Equals("1"))
            {
                weighingRecord.Sz = weighingRecord.Jz - weighingRecord.Kz;
            }
            else if (discount.Equals("2"))
            {
                weighingRecord.Sz = weighingRecord.Jz * (100 - weighingRecord.Kl) / 100;
            }
            else
            {
                weighingRecord.Sz = weighingRecord.Jz;
            }

            //_weighingRecord.By3 = NumFormatHelper.NumToChn(_weighingRecord.Jz) + "吨";

            //备用字段公式运算
            ComputeByFieldsValue(weighingRecord);

            //更新IC卡的备用字段信息
            if (weighingRecord.ICCard != null)
            {
                using var db = AJDatabaseService.GetDbContext();
                var cardNo = weighingRecord.ICCard;
                var ic = await db.ICCards.FirstOrDefaultAsync(p => p.CarNo == cardNo);
                if (ic != null)
                {
                    var props = ic.GetType().GetRuntimeProperties();
                    var icUpdated = false;
                    var recordProps = weighingRecord.GetType().GetRuntimeProperties();

                    foreach (var prop in props)
                    {
                        if (!prop.Name.Contains("By"))
                        {
                            continue;
                        }
                        var byProp = recordProps.FirstOrDefault(p => p.Name == prop.Name);
                        if (byProp == null)
                        {
                            continue;
                        }
                        var cfgKey = $"{prop.Name}Formula";
                        if (_mainSetting.Settings.AllKeys.Any(p => p.Equals(cfgKey, StringComparison.OrdinalIgnoreCase)))
                        {
                            var cfgVal = _mainSetting.Settings[cfgKey].TryGetString();
                            if (!string.IsNullOrWhiteSpace(cfgVal))
                            {
                                prop.SetValue(ic, byProp.GetValue(weighingRecord));
                                icUpdated = true;
                            }
                        }
                    }
                    if (icUpdated)
                    {
                        db.Entry(ic).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
            }

            if (plateResult.Weight == plateResult.Weight1)
            {
                weighingRecord.WeighName = _mainSetting.Settings["WeighName"].Value;
            }
            else if (plateResult.Weight == plateResult.Weight2)
            {
                weighingRecord.WeighName = _mainSetting.Settings["Weigh2Name"].Value;
            }
            weighingRecord.WeighingTimes = 1;
            weighingRecord.IsFinish = true;
            weighingRecord.WeighingFormTemplate = _mainSetting.Settings["PrintTemplate"].Value;


            weighingRecord.ComputeCost(0);
            if (weighingRecord.Je > 0)
            {
                _ttsSvc.Speak(weighingRecord, TTSConfig.TTSTextType.识别到车牌, $"请缴费{weighingRecord.Je.ToString("0.00")}元！");
                ShowLogs($"本次费用：{weighingRecord.Je.ToString("0.00")} 元");
            }

            weighingRecord.EntryTime = DateTime.Now;//记录称重时间
            Globalspace.CurrentOutPlate = weighingRecord.Ch;

            //在计算费用这里标记为称重完成状态，因为下面的收费框一旦弹出来就是模态的，会阻塞其他用到
            //称重完成状态的子线程的逻辑
            CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;

            if (_mainSetting.Settings["车牌识别"].Value == "2")
            {
                CopyImg(weighingRecord);
            }

            //判断扣重、扣率是否都填写了。如果都填写了。需要提示。
            if (weighingRecord.Kz != 0 && weighingRecord.Kl != 0)
            {
                _windowManager.ShowMessageBox("扣重、扣率只允许有一个，请处理！");
                return;
            }
            else
            {
                //计算实重多少
                if (weighingRecord.Kz > 0)
                {
                    // jz-kz=sz
                    weighingRecord.Sz = weighingRecord.Jz - weighingRecord.Kz;
                }
                else if (weighingRecord.Kl > 0)
                {
                    //  jz*kl=sz
                    weighingRecord.Sz = weighingRecord.Jz - (weighingRecord.Jz) * (weighingRecord.Kl / 100);
                }
            }

            weighingRecord.SerialNumber = YitIdHelper.NextId().ToString();

            //2022-09-06 如果是光栅遮挡了，还要保存的，就要将车牌号码后面追加“?”来区分
            if (Globalspace.CurrentBtnContent.Contains("遮挡"))
            {
                weighingRecord.IsCover = true;
            }


            using var ctrl = new WeighingRecordController();
            try
            {
                await ctrl.Save(AJAutoMapperService.Instance().Mapper.Map<Common.Models.WeighingRecord, Common.EF.Tables.WeighingRecord>(weighingRecord));
            }
            catch (Exception e)
            {
                ShowLogs($"{weighingRecord.Ch} 数据保存失败:{e.Message}");
                return;
            }

            ShowLogs($"{weighingRecord.Ch} 数据保存完成");


            if (Convert.ToBoolean(_mainSetting.Settings["WithPrinting"]?.Value ?? "False"))
            {
                weighingRecord.Dyrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var printResult = await Common.Utility.PrintHelper.PrintWeighRecordAsync(weighingRecord,
                    _mainSetting, _worksheet, PrintWeighType.OnceWeighing);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "打印失败", MessageBoxButton.OK);
                }
            }

            StatusBar = "称重完成";
            WeighingTimes = 0;
            //CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;

            //SetImg("once", weighingRecord.Ch, weighingRecord.Bh);

            RemoveICCardAsync(weighingRecord);

            //UploadData(_weighingRecord);

            WeightComplete(weighingRecord, true);
        }

        /// <summary>
        /// 使用图片控件的方式抓图,抓图完成后上传称重记录
        /// </summary>
        /// <param name="bh"></param>
        /// <param name="ch"></param>
        /// <param name="weighingTimes"></param>
        private async void StartCaptureImagesAndUploadRecord(WeighingRecord record, bool needUploadWeighRecord)
        {
            if (record == null
                || string.IsNullOrWhiteSpace(record.Ch)
                || string.IsNullOrWhiteSpace(record.Bh))
            {
                return;
            }

            PlatformBase.UploadWeighRecordParams uploadWeighRecordParams = null;
            if (needUploadWeighRecord)
            {
                var carIdentificationResult = new Common.HardwareSDKS.Models.CarIdentificationResult();
                AJUtil.CopyPropertyValues(carIdentificationResult, _currentLPRCameraDevice.CarIdentificationPlateResult);
                uploadWeighRecordParams = new PlatformBase.UploadWeighRecordParams
                {
                    MainSettings = _mainSetting,
                    LPRSvc = _lprSvc,
                    // 这个参数因为已经复制过了, 可以直接传递
                    WeighRecord = record,
                    // 因为是异步抓图, 这个参数要重新new一个, 
                    CarIdentificationResult = carIdentificationResult,
                    LPRDevNo = Globalspace._lprDevNo,
                    CurrentWeightValue = carIdentificationResult.Weight
                };
            }
            var cameras = MonitorList
                .Where(p => p.Type == Common.HardwareSDKS.Models.DeviceType.海康 && p.Enable).ToList();
            var weighingTimesName = record.WeighingTimes == 0 ? string.Empty : record.WeighingTimes.ToString();

            var lprSavePath = SettingsHelper.LPRSavePath;

            var dir = Path.Combine(lprSavePath, $"{record.Ch}\\{record.Bh}\\");

            // 如果第二次称重毛重皮重发生交换,那么文件名中的称重次数强制为1, 表示这次的称重图片其实是毛重的
            // 然后要把另外一张图片的 文件名中的W1改成W2,表示为皮重 --阿吉 2024年8月9日16点20分
            if (record.IsPZAndMZExchanged)
            {
                weighingTimesName = "1";
                try
                {
                    var dirInfo = new DirectoryInfo(dir);
                    var images = dirInfo.GetFiles("*.jpg");
                    foreach (var img in images)
                    {
                        var nameArray = Path.GetFileNameWithoutExtension(img.Name).Split('_');
                        var first = nameArray.FirstOrDefault();
                        if (!first.Equals(record.Ch) || nameArray.Length < 3)
                        {
                            continue;
                        }
                        var wName = nameArray.ElementAtOrDefault(2);
                        if (string.IsNullOrWhiteSpace(wName) || !wName.StartsWith("W"))
                        {
                            continue;
                        }
                        try
                        {
                            nameArray[2] = "W2";
                            img.CopyTo(Path.Combine(dir, $"{string.Join("_", nameArray)}{Path.GetExtension(img.Name)}"), true);
                            img.Delete();
                        }
                        catch (Exception)
                        {
                        }

                    }
                }
                catch (Exception)
                {
                }

            }
            if (cameras.Count > 0 && _mainSetting.Settings["MonitorEnable"].TryGetBoolean())
            {
                ShowLogs("正在保存图片...");

                var captureWorker = new BackgroundWorker();
                captureWorker.DoWork += (_, __) =>
                {
                    var now = DateTime.Now;

                    foreach (var camera in cameras)
                    {
                        camera.GetCapture(dir, $"{record.Ch}_{now.ToString("yyyyMMddHHmm")}_W{weighingTimesName}_M{camera.DisplayIndex}.jpg");
                    }

                };
                captureWorker.RunWorkerCompleted += async (_, e) =>
                {
                    ProcessResult uploadResult = null;
                    if (needUploadWeighRecord)
                    {
                        uploadResult = await UploadWeighRecordAsync(record, uploadWeighRecordParams);
                    }
                    ConfirmWeightComplete(record, uploadResult);
                };
                captureWorker.RunWorkerAsync();

            }
            else
            {
                ProcessResult uploadResult = null;
                if (needUploadWeighRecord)
                {
                    uploadResult = await UploadWeighRecordAsync(record, uploadWeighRecordParams);
                }
                ConfirmWeightComplete(record, uploadResult);
            }
        }

        private async Task<ProcessResult> UploadWeighRecordAsync(WeighingRecord record, PlatformBase.UploadWeighRecordParams uploadWeighRecordParams)
        {
            var result = new ProcessResult();

            if (Common.Platform.PlatformManager.Instance.Current is PlatformBase platform)
            {
                ShowLogs("上传称重记录...");

                result = await platform.UploadWeighRecordAsync(uploadWeighRecordParams);

                if (!result.Success)
                {
                    ShowLogs($"上传称重记录失败：{result.Message}");
                }
                else
                {
                    ShowLogs("上传称重记录成功！");
                    using var db = AJDatabaseService.GetDbContext();
                    var id = record.AutoNo;
                    var bh = record.Bh;
                    await db.WeighingRecords.Where(p => p.AutoNo == id && p.Bh == bh)
                        .ExecuteUpdateAsync(sp => sp.SetProperty(p => p.IsUpload, true));
                }
            }
            else
            {
                result.SetSuccess();
            }
            return result;
        }

        private void WeightComplete(WeighingRecord record, bool needUpload)
        {
            // 展示称重结果
            WeighFormVM.Refresh(ref record);

            StartCaptureImagesAndUploadRecord(record, needUpload);

        }

        private void ConfirmWeightComplete(WeighingRecord record, ProcessResult uploadResult)
        {
            var openGate = true;
            string customLEDText = null;
            string customTTSMsg = null;
            if (uploadResult != null && uploadResult.Data is UploadWeighRecordResult apiRet)
            {
                customLEDText = apiRet.CustomLEDText;
                customTTSMsg = apiRet.Message;
                openGate = apiRet.OpenGate;
                if (!openGate)
                {
                    ShowLogs(apiRet.Message);
                }
            }

            Globalspace._plateNo = string.Empty;

            QueryList(_currentTabParam);

            Globalspace._diState = "0";

            // 清空光栅结果,好让下次称重继续进入光栅检测
            _gateSvc.RasterCheckResult = null;

            WeightLineChart.Stop(record);

            if (openGate)
            {
                // 双向以上模式, 开配置方向的闸, 好让车出去
                if (_gateSvc.BarrierType >= BarrierType.双向)
                {
                    // 读取开的配置的方向, 0 是开前闸， 对应相机方向是出，  否则就是入
                    var barrierValue = _mainSetting.Settings["OpenBarrierB"].TryGetInt(0);
                    _currentLPRCameraDevice.CarIdentificationPlateResult.Direction = barrierValue == 0
                        ? CarEntryDirection.Out : CarEntryDirection.In;
                    // 称重完成开闸，但是是开另外一个相机，方法里面判断，这里传入入口识别相机为了方法里面方便筛选
                    _gateSvc.Open(_currentLPRCameraDevice);


                }
                else
                {
                    _currentLPRCameraDevice.CarIdentificationPlateResult.Direction = CarEntryDirection.In;
                    _gateSvc.Open(_currentLPRCameraDevice);
                }
            }

            // 稍微延迟下，再发送LED，
            Thread.Sleep(1500);

            _ledSvc.SendText(_currentLPRCameraDevice, record, record.WeighingTimes == 1
                            ? Common.HardwareSDKS.Models.LEDDeviceBase.LEDDisplayTextType.第一次称重完成
                            : Common.HardwareSDKS.Models.LEDDeviceBase.LEDDisplayTextType.第二次称重完成, customLEDText);

            _ttsSvc.Speak(record, record.WeighingTimes == 1
                            ? TTSConfig.TTSTextType.第一次称重完成
                            : TTSConfig.TTSTextType.第二次称重完成, customTemplate: customTTSMsg, forcePlay: true);

            if (openGate)
            {
                RefreshWeighForm();
            }

        }

        public async Task<decimal> GetGoodsPriceAsync(Common.EF.Tables.Customer customerInfo, Common.EF.Tables.Goods goodsInfo)
        {
            using var ctrl = new GoodsVsCustomerPriceController();
            var pricesInfo = await ctrl.List(customerInfo?.Id ?? -1, goodsInfo?.Id ?? -1);

            if (pricesInfo.Any())
            {
                return pricesInfo.FirstOrDefault().Price;
            }
            return goodsInfo?.Price ?? 0m;
        }

        /// <summary>
        /// 删除ICCARD数据，目前在每次称重完成的时候调用此方法
        /// </summary>
        /// <param name="record"></param>
        private async void RemoveICCardAsync(WeighingRecord record)
        {
            //判断是否开启称重完成就删除IC卡的开关状态。
            if ("true".Equals((_mainSetting.Settings["SyncYycz"]?.Value ?? string.Empty), StringComparison.OrdinalIgnoreCase))
            {
                using var db = AJDatabaseService.GetDbContext();
                var ch = record.Ch;
                await db.ICCards.Where(p => p.CarNo == ch).ExecuteDeleteAsync();
            }

        }

        /// <summary>
        /// 2023-04-26 
        /// 根据重量自动称重功能（A车牌 绑定的重量为2-3kg那么稳定时候的重量小于3大于2KG 则磅单自动输入A车牌号码并且称重完成，B车牌  4-5kg 稳定时候的重量大于4小于5kg 则保存为B车牌）
        /// </summary>
        private void SetPlate()
        {
            try
            {
                if (_mainSetting.Settings["AutoID"].Value == "1")
                {
                    var rangs = new List<PlateInfoModel>();
                    var plateList = _mainSetting.Settings["PlateList"].Value;

                    if (!string.IsNullOrEmpty(plateList))
                    {
                        decimal x = decimal.Parse(WeightStr);
                        rangs = JsonConvert.DeserializeObject<List<PlateInfoModel>>(plateList);
                        var firstPlate = rangs.FirstOrDefault(p => x >= p.Min && x <= p.Max);

                        if (firstPlate != null)
                            AWSV2.Globalspace._plateNo = firstPlate.PlateNo;
                    }
                }
            }
            catch (Exception e)
            {
                log.Info($"GetPlate Error:{e.Message}");
            }
        }

        private void SetImg(string saveralTimes, string carNo, string weighingBH)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(carNo)) return;

                var root = Path.Combine(Common.Utility.SettingsHelper.LPRSavePath, carNo, weighingBH);

                if (!Directory.Exists(root)) return;

                DirectoryInfo sDir = new DirectoryInfo(root);
                FileInfo[] fileArray = sDir.GetFiles().OrderByDescending(o => o.CreationTime).ToArray();


                //隐藏数据统计窗口，显示图片窗口
                WeighFormViewVisible = Visibility.Visible;
                DataFormViewVisible = Visibility.Hidden;

                if (saveralTimes == "iconData")
                {
                    FirstImg = Globalspace._lprImgPath;
                }
                else
                {
                    var fname1 = saveralTimes == "first" ? "_w1_m1" : saveralTimes == "second" ? "_w2_m1" : "_w_m1";
                    var fname2 = saveralTimes == "first" ? "_w1_m2" : saveralTimes == "second" ? "_w2_m2" : "_w_m2";

                    var file = fileArray.FirstOrDefault(p => System.IO.Path.GetFileNameWithoutExtension(p.FullName).ToLower().EndsWith("_lpr"));
                    FirstImg = file == null ? String.Empty : file.FullName;

                    file = fileArray.FirstOrDefault(p => System.IO.Path.GetFileNameWithoutExtension(p.FullName).ToLower().EndsWith(fname1));
                    SecondImg = file == null ? String.Empty : file.FullName;

                    file = fileArray.FirstOrDefault(p => System.IO.Path.GetFileNameWithoutExtension(p.FullName).ToLower().EndsWith(fname2));
                    ThirdImg = file == null ? String.Empty : file.FullName;

                }
            }
            catch { }
        }

        private void CopyImg(WeighingRecord weighingRecord)
        {
            try
            {
                var lprPath = _mainSetting.Settings["LPRSavePath"].TryGetString();
                var basePath = Path.Combine(lprPath, weighingRecord.Ch);
                var target = Path.Combine(basePath, weighingRecord.Bh);

                if (!Directory.Exists(target)) Directory.CreateDirectory(target);

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                var dirInfo = new DirectoryInfo(basePath);
                var files = dirInfo.GetFiles();

                foreach (var file in files)
                {
                    var fName = Path.GetFileNameWithoutExtension(file.FullName);
                    var ext = Path.GetExtension(file.FullName);

                    if (!ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                    {
                        file.Delete();
                        continue;
                    }

                    File.Copy(file.FullName, Path.Combine(target, $"{fName}_W{weighingRecord.WeighingTimes}{ext}"), true);
                    file.Delete();
                }

                files = new DirectoryInfo(target).GetFiles(target);

                if (files != null && files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        if (!Path.GetExtension(file.FullName).ToLower().Contains("jpg")
                            || Path.GetFileName(file.FullName).Contains("marked"))
                        {
                            continue;
                        }
                        _snapWatermarkConfig.ProcessSnapWatermark(file.FullName,
                            weighingRecord.Ch,
                            weighingRecord.Mz.ToString(),
                            weighingRecord.Pz.ToString(),
                            weighingRecord.Jz.ToString(),
                            weighingRecord.WeighName);
                    }
                }

            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }
        }

        private void CopyVideo(WeighingRecord weighingRecord)
        {

            try
            {

                var lprPath = _mainSetting.Settings["MonitorSavePath"].TryGetString();
                string basePath = $"{lprPath}\\{weighingRecord.Ch}";
                string root = $"{basePath}\\{weighingRecord.Bh}";

                if (!Directory.Exists(root)) Directory.CreateDirectory(root);

                //把视频根目录车号文件夹下的所有图片都拷贝到编号文件夹里
                var imgs = Directory.GetFiles(basePath).Where(p => System.IO.Path.GetExtension(p).ToLower() == ".mp4");//获取所有视频
                foreach (var img in imgs)
                {
                    string fileName = $"EWD_{System.IO.Path.GetFileName(img)}";
                    File.Copy(img, $"{root}\\{fileName}");
                    File.Delete(img);
                }

            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }

        }

        public void ShowWeighingRecordWindow()
        {
            IPCHelper.OpenApp("WeighingRecord", Globalspace._currentUser.LoginId);
        }

        public void ShowDataManagementWindow()
        {
            IPCHelper.OpenApp("DataManagement", Globalspace._currentUser.LoginId);
        }

        public void OpenWeighPlanWindow()
        {
            var viewModel = new WeighPlanDialogViewModel(_windowManager);
            _windowManager.ShowDialog(viewModel);
        }

        public void ShowSettingWindow()
        {
            try
            {
                //if (Common.Share.VersionControl.CurrentVersion == Common.Share.VersionType.标准版)
                //{
                //    ShowSimpleSettingWindow();
                //}
                //else
                //{
                //    ShowFullSettingWindow();
                //}
                // 全部用一样的页面， 内部判断去掉一些配置 --阿吉 2023年8月18日10点45分
                ShowFullSettingWindow();
            }
            catch (Exception e)
            {
                log.Debug($"打开设置页面-刷新本地AWSV2配置文件失败：{e.Message}");
            }
        }

        public void ShowFullSettingWindow()
        {
            try
            {
                //加载字段验证功能后打开设置窗口
                var validator = new SettingViewModelValidator();
                var validatorAdapter = new FluentModelValidator<SettingViewModel>(validator);
                var viewModel = new SettingViewModel(_windowManager, validatorAdapter, ref _mobileConfigurationMgr);
                _windowManager.ShowDialog(viewModel);

                //修改配置后重载配置
                RefreshAWSV2Setting(true);

                //如果重新设计了磅单，需要重新绘制到页面和加载到内存
                SwitchWeightForm(SelectedWeightFormTemplateSheet);

                Task.Run(async () =>
                {
                    await _mobileConfigurationMgr.NotifyConfigUpdatedAsync();
                    await BackupHelper.InitBackupJobAsync();
                });
            }
            catch (Exception e)
            {
                _windowManager.ShowMessageBox(e.Message);
            }
        }

        //public void ShowSimpleSettingWindow()
        //{
        //    try
        //    {
        //        //加载字段验证功能后打开设置窗口
        //        var validator = new SimpleSettingViewModelValidator();
        //        var validatorAdapter = new FluentModelValidator<SimpleSettingViewModel>(validator);
        //        var viewModel = new SimpleSettingViewModel(WindowManager, validatorAdapter);
        //        WindowManager.ShowDialog(viewModel);

        //        //修改配置后重载配置
        //        RefreshAWSV2Setting(true);
        //        LoadConfig();

        //        //如果重新设计了磅单，需要重新绘制到页面和加载到内存
        //        WeighFormVM = new WeighFormViewModel(_weighingRecord, _selectedWeightFormTemplateSheet);
        //        _worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[0];

        //        //更新前端的 称重单位、地磅名称
        //        //WeighingUnit= ConfigurationManager.AppSettings["WeighingUnit"];
        //        //WeighName1 = ConfigurationManager.AppSettings["WeighName"];
        //        var WeighingUnit_1 = WeighingUnit;
        //        var WeighName1_1 = WeighName1;
        //        WeighingUnit = "";
        //        WeighName1 = "";
        //        WeighingUnit = WeighingUnit_1;
        //        WeighName1 = WeighName1_1;
        //    }
        //    catch (Exception e)
        //    {
        //        WindowManager.ShowMessageBox(e.Message);
        //    }
        //}

        //public void ShowRegInfo()
        //{
        //    bool flag = Globalspace._isRegister;
        //    var viewModel = new QrCodeViewModel();
        //    bool? result = _windowManager.ShowDialog(viewModel);
        //    //等二维码关闭后再验证一次，防止用户通过二维码注册后，不能即时感知
        //    //还必须是在联网的状态下才能即时感知，否则只能认为注册失败
        //    if (Common.Encrt.WebApi.Ping())
        //    {
        //        Globalspace._isRegister = Register.Verify(Register.GetHdInfo(), ref _mobileConfigurationMgr);
        //    }
        //    else
        //    {
        //        Globalspace._isRegister = false;
        //    }

        //    if (flag != Globalspace._isRegister)
        //    {
        //        StatusBar = "注册成功，请重新启动程序！";
        //    }
        //}

        public void ShowHelpChm()
        {
            //打开chm文档
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = Environment.CurrentDirectory + "\\无人值守汽车衡称重系统V2.2 说明书.chm",
                    UseShellExecute = true,
                });
            }
            catch { }
        }

        public void ShowSysLog()
        {
            //var viewModel = new SysLogViewModel(_windowManager);

            //_windowManager.ShowDialog(viewModel);
        }

        public void ShowAboutAWS()
        {
            var viewModel = new AboutAWSViewModel();

            _windowManager.ShowDialog(viewModel);
        }

        public void ManualRefreshWeighForm()
        {
            if (_weightStableWorker.IsBusy)
            {
                _weightStableWorker.CancelAsync();
                while (_weightStableWorker.IsBusy)
                {
                    DispatcherHelper.DoEvents();
                    Thread.Sleep(600);
                }
            }
            else
            {
                RefreshWeighForm(true);
                WeightLineChart.Stop();
            }
            if (_plateResultLockerRuning)
            {
                _plateResultLockerRuning = false;
                _plateResultLocker.Release();
            }
        }

        private void RefreshWeighForm(bool force = false)
        {
            if (force || _mainSetting.Settings["TimelyRefresh"].TryGetInt() == 1)
            {
                if (!string.IsNullOrWhiteSpace(_weighingRecord.Ch))
                {
                    _ledSvc.SendText(_currentLPRCameraDevice, null, Common.HardwareSDKS.Models.LEDDeviceBase.LEDDisplayTextType.空闲);
                    _weighingRecord = new WeighingRecord();

                    WeighFormVM.Refresh(ref _weighingRecord, true);
                }

                CurrentStatus = Common.Model.Custom.WeighStatus.Waiting;

                LogContent.Clear();

                // 清空当前识别的车牌信息，好可以继续输入或识别车牌称重
                _currentLPRCameraDevice.CarIdentificationPlateResult.CarNo = string.Empty;

                WeighingTimes = 0;
                IsICBusy = false;
                IsDelayNZStable = false;
                OverWeight = false;
                SendCmdOnce = true;
                StatusBar = "";

                IsStable = false;
            }

        }

        public void QuickPlate()
        {
            IPCHelper.OpenApp("QuickPlate");
        }

        //public void SwitchWeighMode()
        //{
        //    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    string mode = config.AppSettings.Settings["WeighingMode"].Value;
        //    if (mode == "Once")
        //    {
        //        config.AppSettings.Settings["WeighingMode"].Value = "Twice";
        //        WeighModeIcon = "Numeric1Box";
        //    }
        //    else if (mode == "Twice")
        //    {
        //        config.AppSettings.Settings["WeighingMode"].Value = "Once";
        //        WeighModeIcon = "Numeric2Box";
        //    }

        //    config.Save(ConfigurationSaveMode.Modified);
        //    ConfigurationManager.RefreshSection("appSettings");

        //    LoadConfig();
        //    WeighFormVM = new WeighFormViewModel(_weighingRecord, _selectedWeightFormTemplateSheet);
        //    _worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[0];
        //}

        public void ShowSystemWindow()
        {
            try
            {
                var viewModel = new SystemViewModel(_windowManager);
                _windowManager.ShowWindow(viewModel);
            }
            catch (Exception e)
            {
                _windowManager.ShowMessageBox(e.Message);
            }
        }

        public async void SwitchHomeTemplate(HomeTemplateType target)
        {
            if (TemplateType == target)
            {
                return;
            }

            var cur = TemplateType.GetValueOrDefault().GetDescription();
            var targetName = target.GetDescription();

            var ret = await AJConfirmDialog.ShowConfirmAsync("切换提示", $"是否从{cur} 切换至 {targetName}？切换后需要重启程序", "root");
            if (ret is bool confirm && confirm)
            {
                SettingsHelper.UpdateAWSV2(nameof(HomeTemplateType), target.ToString());

                ConfirmClose(true);
            }
        }

        public void SwitchWeighingControl(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = WeighingControl;
                SwitchWeighingControl(name);
            }

            ToggleFasePopup(false);

            _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(WeighingControl)].Value = name;
            _mainSetting.Settings[nameof(WeighingControl)].Value = name;
            _mobileConfigurationMgr.SaveSetting();
            WeighingControl = name;

            SwitchWeightForm(_selectedWeightFormTemplateSheet);

            ManualWeighing = WeighingControl.Equals("Hand") || WeighingControl.Equals("Btn");
        }



        public void HandToBarrier(string s)
        {
            if (_mainSetting.Settings["Barrier"].Value == "0")
            {
                StatusBar = "未连接道闸！";
                return;
            }

            //var hasJieGuanZha = (_mainSetting.Settings["JieGuanZha"]?.Value ?? "False") == "True";

            if (s == "up1")
            {
                IPCHelper.SendMsgToApp("车牌识别", 0x08F1);
                GateSwitchOne = SignalLightGreenOne = true;
                //if (!hasJieGuanZha)
                //{
                //    Thread.Sleep(500);
                //    IPCHelper.SendMsgToApp("车牌识别", 0X08F0);
                //}
            }
            if (s == "up2")
            {
                IPCHelper.SendMsgToApp("车牌识别", 0x08F2);
                GateSwitchTwo = SignalLightGreenTwo = true;
                //if (!hasJieGuanZha)
                //{
                //    Thread.Sleep(500);
                //    IPCHelper.SendMsgToApp("车牌识别", 0X08F0);
                //}
            }
            if (s == "upall")
            {
                IPCHelper.SendMsgToApp("车牌识别", 0x08F3);
                GateSwitchOne = GateSwitchTwo = SignalLightGreenOne = SignalLightGreenTwo = true;
                //if (!hasJieGuanZha)
                //{
                //    Thread.Sleep(500);
                //    IPCHelper.SendMsgToApp("车牌识别", 0X08F0);
                //}
            }
            if (s == "downall")
            {
                IPCHelper.SendMsgToApp("车牌识别", 0x08F0);
                GateSwitchOne = GateSwitchTwo = false;
                SignalLightGreenOne = SignalLightGreenTwo = true;
                //if (!hasJieGuanZha)
                //{
                //    Thread.Sleep(500);
                //    IPCHelper.SendMsgToApp("车牌识别", 0X08F0);
                //}
            }
        }

        /// <summary>
        /// F11,F12 快捷键 称重修正功能 --阿吉 2023年10月13日17点20分
        /// </summary>
        /// <param name="key"></param>
        public void FastWeigthCorrect(string key)
        {
            var limit = _fastWeigthCorrectConfig.GetLimit();
            if (!_fastWeigthCorrectConfig.Enable || limit <= 0)
            {
                return;
            }

            var isF11 = key.Equals("f11");

            var value = isF11
                ? _fastWeigthCorrectConfig.GetF11FunValue() : _fastWeigthCorrectConfig.GetF12FunValue() * -1;

            if (value == 0)
            {
                return;
            }

            var logTxg = $"使用 {key} 修正称重,剩余次数:{limit}, 修正值:{Math.Abs(value)}, 原始重量:{WeightStr1}";

            var digits = (decimal)Math.Pow(10, WeightValueDisplayFormat);

            var weight = WeightStr1.TryGetDecimal() + value;

            WeightStr1 = (Math.Truncate(weight * digits) / digits).ToString($"F{WeightValueDisplayFormat}");

            logTxg += $",修正后重量:{WeightStr1}";

            limit -= 1;

            _fastWeigthCorrectConfig.Limit = limit.ToString();
            _fastWeigthCorrectConfig.Enable = limit > 0;

            logTxg += $",修正后剩余次数:{limit}";

            if (!_fastWeigthCorrectConfig.Enable)
            {
                logTxg += $",已自动禁用修正功能";
            }

            log.Debug(logTxg);

            // 保存到配置
            _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(FastWeigthCorrectConfig)].Value = AJUtil.AJSerializeObject(new
            {
                _fastWeigthCorrectConfig.Enable,
                _fastWeigthCorrectConfig.Limit,
                _fastWeigthCorrectConfig.F11FunValue,
                _fastWeigthCorrectConfig.F12FunValue
            });

            _mobileConfigurationMgr.SaveSetting();
        }

        public void OpenChangePasswordDialog()
        {
            _windowManager.ShowWindow(new ChangePasswordViewModel(_windowManager, _eventAggregator, false));
        }

        private bool _isFastPopupOpen;
        public bool IsFastPopupOpen
        {
            get => _isFastPopupOpen;
            set => SetAndNotify(ref _isFastPopupOpen, value);
        }
        public void ToggleFasePopup(bool? isOpen)
        {
            IsFastPopupOpen = isOpen.GetValueOrDefault();
        }

        public void Logout()
        {
            //2022-10-27 新增 切换用户之前 要先密码验证。通过才能切换。
            var pwdWindow = new Common.ViewModels.PasswordViewModel(
                Globalspace._currentUser,
                Common.ViewModels.PasswordConfirmType.切换用户);
            bool? result = _windowManager.ShowDialog(pwdWindow);
            if (!result.HasValue || !result.Value)
            {
                return;
            }

            ConfirmClose(true);
        }

        private string FormulaCalc(WeighingRecord weighingRecord,
            string byxFormula, ExpressionContext context, IEnumerable<PropertyInfo> @props)
        {
            //备用字段公式计算
            var rst = string.Empty;

            try
            {
                context.Variables["_mz"] = weighingRecord.Mz;
                context.Variables["_pz"] = weighingRecord.Pz;
                context.Variables["_jz"] = weighingRecord.Jz;

                foreach (var prop in @props)
                {
                    var val = prop.GetValue(weighingRecord);
                    var propName = prop.Name.ToLower();

                    if ((propName.Equals("kz") || propName.Equals("kl") || propName.Equals("sz") || prop.Name.Contains("By"))
                        && (val != null))
                    {
                        context.Variables[$"_{prop.Name.ToLower()}"] = val.ToString().TryGetDecimal();
                    }
                }

                rst = context.CompileDynamic(byxFormula).Evaluate().ToString();
            }
            catch (Exception ex)
            {
                log.Error($"By公式计算失败:{ex.Message}", ex);
            }

            return rst;
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();
        }

        protected override async void OnClose()
        {
            //IPCHelper.CloseApp("轮轴接收");
            IPCHelper.KillApp("QuickPlate");

            //结束电子支付SDK
            CloudLogic.CloudAPI.Shutdown();

            //以下是第三方平台的结束逻辑
            Common.Platform.PlatformManager.Instance.Dispose();
            //逻辑结束

            //保存首页分割控件的位置

            //_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["SplitterTopPoint"].Value = SplitterTopPoint.Value.ToString();
            //_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["SplitterBottomPoint"].Value = SplitterBottomPoint.Value.ToString();
            //_mobileConfigurationMgr.SaveSetting();

            //关闭App.xaml页面中的 联网检车线程
            App.DisposeSync();

            log.Info("用户 " + Globalspace._currentUser.UserName + "非安全（未备份） 退出系统");
            base.OnClose();
        }

        public void WindowCloseCmd()
        {
            var pwdWindow = new Common.ViewModels.PasswordViewModel(
                Globalspace._currentUser);
            var result = AWSV2.Globalspace.windowManager.ShowDialog(pwdWindow);
            if (!result.GetValueOrDefault())
            {
                return;
            }

            ConfirmClose();
        }

        private async Task BeforeClose()
        {
            this._windowManager.ShowDialog(new BackupViewModel());

            _eventAggregator.Unsubscribe(this);

            await _mqttSvc.UpdateApplicationAsync(Common.Utility.AJ.CloudAPI.MACHINEMD5, false);

            await _mqttSvc.CloseAsync(true);
            _mobileConfigurationMgr.Stop();

            _gateSvc.Stop();
            _lprSvc.Stop();
            _weighSerialPortService.Stop();
            _qrCodeSerialPortService?.Stop();
            _qrCodeSerialPortService2?.Stop();
            WeightLineChart.Stop();

            Common.HardwareSDKS.IDCardReader.IDCardReaderSDK.Stop();

            // 保存首页称重记录列顺序
            var columns = new List<(string column, int index)>();
            foreach (var item in _weightRecordDataGrid.Columns)
            {
                var name = item.Header?.ToString() ?? string.Empty;
                if ("选择".Equals(name))
                {
                    continue;
                }
                columns.Add((name, item.DisplayIndex));
            }
            var orderedColumns = columns.OrderBy(p => p.index).ToArray();

            foreach (var item in orderedColumns)
            {
                var field = DynamicPoundFieldItems.FirstOrDefault(p => p.Label == item.column);
                if (field == null)
                {
                    continue;
                }
                field.SortNo = item.index;
            }
            SettingsHelper.UpdateAWSV2("WeightRecordDataGridOrderedColumns",
                AJUtil.AJSerializeObject(DynamicPoundFieldItems.OrderBy(p => p.SortNo).Select(p => new
                {
                    Field = p.ToConfigFieldKey(),
                    p.Label,
                    p.SortNo,
                    p.IsColumnDisplay,
                    p.IsFormDisplay
                })));


            if (MonitorList?.Count > 0)
            {
                foreach (var monitor in MonitorList)
                {
                    monitor.StopVideo();
                    monitor.Close();
                }

                if (MonitorList.Any(p => p.Type == Common.HardwareSDKS.Models.DeviceType.臻识 && p.Enable))
                {
                    VzClientSDK.VzLPRClient_Cleanup();
                }

                if (MonitorList.Any(p => p.Type == Common.HardwareSDKS.Models.DeviceType.海康 && p.Enable))
                {
                    CHCNetSDK.NET_DVR_Cleanup();
                }

                if (MonitorList.Any(p => p.Type == Common.HardwareSDKS.Models.DeviceType.华夏 && p.Enable))
                {
                    HuaXiaICESDK.ICE_IPCSDK_Fini();
                }
            }
        }

        private async void ConfirmClose(bool needRestart = false)
        {
            await BeforeClose();

            RequestClose();

            Application.Current.Shutdown();
            if (needRestart)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            }
        }

        private void WeightBtnIsEnableChange(object obj, EventArgs e)
        {
            var btn = obj as Button;
            if (btn == null || string.IsNullOrWhiteSpace(btn.Name)) return;
            int index = int.Parse(btn.Name.Replace("btn", ""));
            //btn.Content = btnContents[index];

            //如果是两次称重，禁止单次称重按钮的逻辑，以免混乱
            if (_mainSetting.Settings["WeighingMode"].Value.Equals("Twice") && index == 0)
            {
                return;
            }

            //如果是单次称重，禁止两次称重按钮的逻辑，以免混乱Mixture Once Twice
            if (_mainSetting.Settings["WeighingMode"].Value.Equals("Once") && index != 0)
            {
                return;
            }

            Globalspace.CurrentBtnIndex = index;

            if (btn.IsEnabled)
            {
                btn.SetResourceReference(Button.StyleProperty, "ThisPageYellowButtonStyle");
            }
            else
            {
                btn.SetResourceReference(Button.StyleProperty, "ThisPageButtonStyle");
                //Globalspace.CurrentBtnIndex = 3;
            }

        }

        public void MainWindowStateChanged(object sender, EventArgs e)
        {
            //_eventAggregator.Publish(new MainShellViewEvent(MainShellViewEvent.EventType.仅通知)
            //{
            //    Data = ((Window)sender).WindowState
            //});
        }

        public void StartProcess(string exeName)
        {
            var rootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Tools");
            var exeFile = Path.Combine(rootDir, exeName);
            if (string.IsNullOrWhiteSpace(exeName)
                || !File.Exists(exeFile))
            {
                MessageBox.Show("第三方程序不存在", "提示", MessageBoxButton.OK);
                return;
            }

            Process.Start(exeFile);
        }

        public void TestGateOpen(bool open)
        {
            if (open)
            {
                _gateSvc.Open(null);
            }
            else
            {
                _gateSvc.Close(null);
            }

            var msg = open ? "开闸命令已发送" : "关闸命令已发送";

            MessageBox.Show(msg, "提示", MessageBoxButton.OK);

        }

        public void TestIDCardRead()
        {
            var idCardReadDialog = new AJCommonLoadingDialogViewModel("正在读取身份证...", () =>
            {
                return _lprSvc.IDCardReaderCfg.ReadIDCardAsync();
            });

            var dialogRet = _windowManager.ShowDialog(idCardReadDialog);

            if (!idCardReadDialog.Result.Success)
            {
                MessageBox.Show("未能读取身份证信息");
                return;
            }

            MessageBox.Show(AJUtil.AJSerializeObject(idCardReadDialog.Result.Data as IDCardInfo), "读取成功");

        }

        public void TestHIKCaptureImage()
        {
            var bh = "A001";
            var ch = "京A12345";
            StartCaptureImagesAndUploadRecord(new WeighingRecord
            {
                Bh = bh,
                Ch = ch
            }, false);
            var path = Path.Combine(SettingsHelper.LPRSavePath, $"{ch}\\{bh}\\");
            MessageBox.Show($"测试编号:{bh},测试车号:{ch} 抓图命令已发送, 请稍后至 {path} 查看", "测试抓图");
        }

        public void TestWeightValue()
        {
            var promptDialog = new AJPromptDialogViewModel
            {
                Title = "请输入重量值",
                Required = true,
                ConfirmText = "确定",
                Type = ControlStatusType.Info,
            };

            var promptResult = _windowManager.ShowDialog(promptDialog);

            if (!promptResult.GetValueOrDefault())
            {
                return;
            }
            _gateSvc.RasterCheckResult = new ProcessResult { Success = true };
            WeightStr = WeightStr1 = promptDialog.Text.TryGetDecimal().ToString($"F{WeightValueDisplayFormat}");
            IsStable = IsDelayNZStable = true;
        }

        public void TestTTS()
        {
            //var promptDialog = new AJPromptDialogViewModel
            //{
            //    Title = "请输入文字模板",
            //    Required = true,
            //    ConfirmText = "确定",
            //    Type = ControlStatusType.Info,
            //};

            //var promptResult = _windowManager.ShowDialog(promptDialog);

            //if (!promptResult.GetValueOrDefault())
            //{
            //    return;
            //}

            _ttsSvc.Speak(new Common.Models.WeighingRecord
            {
                Ch = "京A13245",
                Mz = 10,
                Pz = 10,
                Jz = 0
            }, TTSConfig.TTSTextType.第二次称重完成);
        }

    }

    public class WeightLineChartModel : PropertyChangedBase
    {
        private DispatcherTimer _timer;

        private SolidColorPaint _legendTextPaint;
        public SolidColorPaint LegendTextPaint
        {
            get { return _legendTextPaint; }
            set { SetAndNotify(ref _legendTextPaint, value); }
        }

        private ObservableCollection<ISeries> _series;
        public ObservableCollection<ISeries> Series { get => _series; set => SetAndNotify(ref _series, value); }

        private ObservableCollection<Axis> _labels;
        public ObservableCollection<Axis> Labels { get => _labels; set => SetAndNotify(ref _labels, value); }

        private ObservableCollection<Axis> _yAxes;
        public ObservableCollection<Axis> YAxes
        {
            get { return _yAxes; }
            set { SetAndNotify(ref _yAxes, value); }
        }

        private Func<decimal> _getWeightHandle;

        private ObservableCollection<string> _dynamicLabels;
        private ObservableCollection<decimal> _dynamicValues;

        public WeightLineChartModel(Func<decimal> getWeightHandle)
        {
            _getWeightHandle = getWeightHandle;

            _dynamicLabels = new ObservableCollection<string>();
            _dynamicValues = new ObservableCollection<decimal>();

            var _materialDesignBodyBrush = Application.Current.FindResource("MaterialDesignBody") as SolidColorBrush;

            LegendTextPaint = new SolidColorPaint(new SKColor(
                    _materialDesignBodyBrush.Color.R,
                    _materialDesignBodyBrush.Color.G,

                    _materialDesignBodyBrush.Color.B));

            Labels = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Labels = _dynamicLabels,
                    NamePaint = LegendTextPaint,
                    LabelsPaint = LegendTextPaint,
                    TextSize = 10,
                }
            };

            YAxes = new ObservableCollection<Axis>
            {
                new Axis
                {
                    NamePaint = LegendTextPaint,
                    LabelsPaint = LegendTextPaint,
                    TextSize = 10,
                }
            };

            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<decimal>
                {
                    Values = _dynamicValues,
                    Name = string.Empty,
                    Fill = null,
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(
                    _materialDesignBodyBrush.Color.R,
                    _materialDesignBodyBrush.Color.G,

                    _materialDesignBodyBrush.Color.B), 1),
                }
            };

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += (_, __) =>
            {
                _dynamicLabels.Add(DateTime.Now.ToString("mm:ss"));
                _dynamicValues.Add(_getWeightHandle.Invoke());
            };
        }

        /// <summary>
        /// 车牌识别后开始记录重量曲线数据
        /// </summary>
        public void Start()
        {
            _dynamicLabels.Clear();
            _dynamicValues.Clear();
            _timer.Start();
        }

        /// <summary>
        /// 程序退出,称重完成或者取消,停止记录曲线数据
        /// </summary>
        /// <param name="record">称重记录,如果不为空, 需要将曲线记录数据写入称重记录编号的文本文件</param>
        public void Stop(WeighingRecord record = null)
        {
            _timer.Stop();
            if (record != null && !string.IsNullOrWhiteSpace(record.Bh))
            {
                var year = record.EntryTime.Year;
                var month = record.EntryTime.Month;
                var day = record.EntryTime.Day;
                var hour = record.EntryTime.Hour;

                try
                {
                    var dic = new Dictionary<DateTime, decimal>();
                    for (int i = 0; i < _dynamicLabels.Count; i++)
                    {
                        var array = _dynamicLabels[i].Split(':');
                        dic.Add(new DateTime(year, month, day, hour,
                            Convert.ToInt32(array[0]), Convert.ToInt32(array[1])), _dynamicValues.ElementAtOrDefault(i));
                    }
                    var lprSavePath = SettingsHelper.LPRSavePath;
                    System.IO.File.WriteAllText(Path.Combine(lprSavePath, record.Ch, record.Bh, $"lineChart_W{record.WeighingTimes}.data"), AJUtil.AJSerializeObject(dic));
                }
                catch (Exception)
                {
                }

            }
            _dynamicLabels.Clear();
            _dynamicValues.Clear();
        }
    }
}
