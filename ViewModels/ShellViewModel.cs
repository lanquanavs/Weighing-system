using Aspose.Cells;
using AWSV2.Models;
using AWSV2.Services;
using AWSV2.Services.Encrt;
using Common.Model;
using Common.Model.ChargeInfo;
using Common.Platform;
using Common.Platform.Citys.HWZHZTelemetry;
using Common.Platform.Citys.Reef;
using Common.Platform.Citys.Shandong_Boxing;
using Common.Platform.Citys.ZJS;
using Common.Utility;
using Common.Utility.AJ;
using Common.Utility.AJ.EventAgregators;
using Common.Utility.AJ.Extension;
using Common.Utility.AJ.MobileConfiguration;
using ControlzEx.Standard;
using Flee.PublicTypes;
using log4net.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using NPOI.XSSF.Streaming.Values;
using Stylet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel.Security;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using static Common.Utility.AJ.MQTTService;
using static NPOI.HSSF.Util.HSSFColor;
using Rule = AWSV2.Models.Rule;

namespace AWSV2.ViewModels
{

    public class ShareMem
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(int hFile, IntPtr lpAttributes, uint flProtect, uint dwMaxSizeHi, uint dwMaxSizeLow, string lpName);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping(int dwDesiredAccess, bool bInheritHandle, string lpName);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMapping, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnmapViewOfFile(IntPtr pvBaseAddress);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32", EntryPoint = "GetLastError")]
        public static extern int GetLastError();
        const int ERROR_ALREADY_EXISTS = 183;
        const int FILE_MAP_COPY = 0x0001;
        const int FILE_MAP_WRITE = 0x0002;
        const int FILE_MAP_READ = 0x0004;
        const int FILE_MAP_ALL_ACCESS = 0x0002 | 0x0004;
        const int PAGE_READONLY = 0x02;
        const int PAGE_READWRITE = 0x04;
        const int PAGE_WRITECOPY = 0x08;
        const int PAGE_EXECUTE = 0x10;
        const int PAGE_EXECUTE_READ = 0x20;
        const int PAGE_EXECUTE_READWRITE = 0x40;
        const int SEC_COMMIT = 0x8000000;
        const int SEC_IMAGE = 0x1000000;
        const int SEC_NOCACHE = 0x10000000;
        const int SEC_RESERVE = 0x4000000;
        const int INVALID_HANDLE_VALUE = -1;
        IntPtr m_hSharedMemoryFile = IntPtr.Zero;
        IntPtr m_pwData = IntPtr.Zero;
        bool m_bAlreadyExist = false;
        bool m_bInit = false;
        long m_MemSize = 0;
        public struct MAIN_LPR_OVER_WEIGHT_DATA
        {
            public string mz;
            public double overWeightCount;
            public string axleNum;
        }

        //[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]

        public byte[] StructToBytes<T>(T obj)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr bufferPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(obj, bufferPtr, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(bufferPtr, bytes, 0, size);
                return bytes;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in StructToBytes ! " + ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(bufferPtr);
            }
        }
        public object BytesToStuct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, structPtr, size);
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }
        public ShareMem()
        {
        }
        ~ShareMem()
        {
            Close();
        }
        public int Init(string strName, long lngSize)
        {
            if (lngSize <= 0 || lngSize > 104857600) lngSize = 104857600;
            //0x00800000
            m_MemSize = lngSize;
            if (strName.Length > 0)
            {
                //创建内存共享体(INVALID_HANDLE_VALUE)
                m_hSharedMemoryFile = CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (uint)PAGE_READWRITE, 0, (uint)lngSize, strName);
                if (m_hSharedMemoryFile == IntPtr.Zero)
                {
                    m_bAlreadyExist = false;
                    m_bInit = false;
                    return 2;
                    //创建共享体失败
                }
                else
                {
                    if (GetLastError() == ERROR_ALREADY_EXISTS)  //已经创建 
                    {
                        m_bAlreadyExist = true;
                    }
                    else                                         //新创建 
                    {
                        m_bAlreadyExist = false;
                    }
                }
                //---------------------------------------
                //创建内存映射
                m_pwData = MapViewOfFile(m_hSharedMemoryFile, FILE_MAP_WRITE, 0, 0, (uint)lngSize);
                if (m_pwData == IntPtr.Zero)
                {
                    m_bInit = false;
                    CloseHandle(m_hSharedMemoryFile);
                    return 3;
                    //创建内存映射失败
                }
                else
                {
                    m_bInit = true;
                    if (m_bAlreadyExist == false)
                    {
                        //初始化
                    }
                }
                //----------------------------------------
            }
            else
            {
                return 1;
                //参数错误
            }
            return 0;
            //创建成功
        }
        /// <summary>
        /// 关闭共享内存
        /// </summary>
        public void Close()
        {
            if (m_bInit)
            {
                UnmapViewOfFile(m_pwData);
                CloseHandle(m_hSharedMemoryFile);
            }
        }
        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="bytData">数据</param>
        /// <param name="lngAddr">起始地址</param>
        /// <param name="lngSize">个数</param>
        /// <returns></returns>
        public int Read(ref byte[] bytData, int lngAddr, int lngSize)
        {
            if (lngAddr + lngSize > m_MemSize) return 2;
            //超出数据区
            if (m_bInit)
            {
                Marshal.Copy(m_pwData, bytData, lngAddr, lngSize);
            }
            else
            {
                return 1;
                //共享内存未初始化
            }
            return 0;
            //读成功
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="bytData">数据</param>
        /// <param name="lngAddr">起始地址</param>
        /// <param name="lngSize">个数</param>
        /// <returns></returns>
        public int Write(byte[] bytData, int lngAddr, int lngSize)
        {
            if (lngAddr + lngSize > m_MemSize) return 2;
            //超出数据区
            if (m_bInit)
            {
                Marshal.Copy(bytData, lngAddr, m_pwData, lngSize);
            }
            else
            {
                return 1;
                //共享内存未初始化
            }
            return 0;
            //写成功
        }
    }

    public class ShellViewModel : Screen, IHandle<MainShellViewEvent>
    {
        #region 基本参数

        /// <summary>
        /// 称重修正功能配置 --阿吉 2023年10月13日17点21分
        /// </summary>
        private FastWeigthCorrectConfig _fastWeigthCorrectConfig;

        /// <summary>
        /// 光栅TTS配置
        /// </summary>
        private GratingTTSConfig _gratingTTSConfig;

        /// <summary>
        /// 过磅抓拍水印配置 --阿吉 2023年10月16日实践
        /// </summary>
        private SnapWatermarkConfig _snapWatermarkConfig;

        //log
        private static readonly log4net.ILog log = LogHelper.GetLogger();

        //加载其他窗口
        private IWindowManager windowManager;

        //支付提醒对话框
        private ConfirmWithChargeViewModel confirmWithChargeView = new ConfirmWithChargeViewModel(new WeighingRecordModel());

        //页面上绑定的属性
        private string _weightStr1 = "";
        public string WeightStr1
        {
            get { return _weightStr1; }
            set
            {
                if (SetAndNotify(ref _weightStr1, value) && StateLab == "信号传输正常")
                {
                    _eventAggregator?.Publish(new MainShellViewEvent(MainShellViewEvent.EventType.重量值更新)
                    {
                        Data = value
                    });
                }
            }
        } //重量数字
        public string WeightStr2 { get; set; } = ""; //重量数字
        public object WeighFormVM { get; set; } //过磅单表单
        public bool TwiceWeighing { get; set; } = true; //显示一个或者二个称重按钮
        public string StatusBar { get; set; } //状态栏
        public bool HaveSecondDevice { get; set; } //启用副设备
        public bool SelectedDevice1 { get; set; }
        public bool SelectedDevice2 { get; set; }

        public string _TitleMessage = string.Empty;
        public string TitleMessage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_TitleMessage))
                {
                    _TitleMessage = Common.Utility.IOHelper.ReadTitleTxt();
                }
                return _TitleMessage;
            }

        }

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


                //if (_EnableWeighing)
                //{
                //    if (TwiceWeighing)
                //    {
                //        if (WeighingTimes == 1)
                //        {
                //            Btn1Style = (System.Windows.Style)Application.Current.Resources["ThisPageYellowButtonStyle"];
                //        }
                //        else
                //        {
                //            Btn2Style = (System.Windows.Style)Application.Current.Resources["ThisPageYellowButtonStyle"];
                //        }
                //    }
                //    else
                //    {
                //        BtnStyle = (System.Windows.Style)Application.Current.Resources["ThisPageYellowButtonStyle"];
                //    }
                //}
                //else
                //    BtnStyle = Btn1Style = Btn2Style = (System.Windows.Style)Application.Current.Resources["ThisPageButtonStyle"];
            }
        }
        public bool EnableCheckSysLog { get; set; } = false;
        public bool EnableSetting { get; set; } = false;
        public bool EnableDataManage { get; set; } = false;
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
        public bool KeepOpen
        {
            get
            {
                return Convert.ToBoolean(_mainSetting.Settings["KeepOpen"]?.Value ?? "False");
            }
            set
            {
                //try
                {
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
                    Common.SyncData.SetKeepOpen(value);
                }
                //catch (Exception e)
                //{

                //}

            }
        }

        public string WeighingUnit
        {
            get
            {
                return _mainSetting.Settings["WeighingUnit"]?.Value ?? string.Empty;
            }
            set
            {
                _mainSetting.Settings["WeighingUnit"].Value = value;

            }
        }
        public string WeighName1
        {
            get
            {
                return _mainSetting.Settings["WeighName"]?.Value ?? string.Empty;
            }
            set
            {
                _mainSetting.Settings["WeighName"].Value = value;
            }
        }
        public string WeighName2
        {
            get
            {
                return _mainSetting.Settings["Weigh2Name"]?.Value ?? string.Empty;
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
        //                            TTSHelper.TTS(weighingRecord, ConfigurationManager.AppSettings["TTS3Text"]);

        //                        //2022-09-01被注释
        //                        if (ConfigurationManager.AppSettings["LED2Enable"] == "True" && ManualWeighing)
        //                        {
        //                            using (StreamWriter sw = new StreamWriter("./Data/LedText/isDelayNZStable.txt", false, Encoding.Default))
        //                            {
        //                                if (weighingRecord.Ch != null) sw.WriteLine(weighingRecord.Ch);
        //                                sw.Write("请停稳车辆并且熄火");
        //                            }
        //                            IPCHelper.SendMsgToApp("ZXLED", IPCHelper.IS_STABLE);

        //                        }

        //                        IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IS_STABLE, StableWeightStr);

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
                    IsDelayStable = false;
                }
                //非零稳定后创建计时器线程开始计时【延时非零稳定】的时间
                if (value
                    && (Convert.ToDecimal(WeightStr) > Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0")))
                {
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
                            if (WeighingTimes != 0)
                            {
                                if (Convert.ToBoolean(_mainSetting.Settings["TTS3Enable"]?.Value ?? "False") && !ManualWeighing)
                                    TTSHelper.TTS(weighingRecord, _mainSetting.Settings["TTS3Text"]?.Value ?? "");
                                if ((_mainSetting.Settings["LED2Enable"]?.Value ?? "False") == "True" && ManualWeighing)
                                {
                                    var file = "./Data/LedText/isDelayNZStable.txt";
                                    if (File.Exists(file))
                                    {
                                        using (StreamWriter sw = new StreamWriter(file, false, Encoding.Default))
                                        {
                                            if (weighingRecord.Ch != null) sw.WriteLine(weighingRecord.Ch);
                                            sw.Write("--------");//删除了请刷卡取票
                                        }
                                        IPCHelper.SendMsgToApp("ZXLED", IPCHelper.IS_STABLE);
                                    }


                                }
                                IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IS_STABLE, StableWeightStr);

                            }
                            log.Debug("延时非零稳定");
                        }
                        else
                        {
                            IsDelayNZStable = false;
                        }
                    };
                }
            }
        }
        public bool IsDelayStable { get; private set; } //重量延时稳定标记
        public bool IsDelayNZStable { get; private set; } //重量延时非零稳定标记
        public List<string> WeightDataList { get; private set; } = new List<string>(); //判断重量稳定的数组
        public SerialPort WeighSerialPort { get; private set; } = new SerialPort();//称重串口1
        public string WeighProtrcolType { get; private set; } //称重协议1
        public SerialPort Weigh2SerialPort { get; private set; } = new SerialPort();//称重串口2
        public string Weigh2ProtrcolType { get; private set; } //称重协议2
        public SerialPort LEDSerialPort { get; private set; } = new SerialPort();//大屏幕串口
        public bool LEDSerialPortEnable { get; private set; } //大屏幕串口已打开的标识，用于判断timer中是否发送串口数据，不要每次都读配置文件
        public SerialPort QRPort { get; private set; } = new SerialPort();//二维码串口
        public SerialPort QRPort2 { get; private set; } = new SerialPort();//二维码串口
        public string QRString { get; private set; } = string.Empty;
        public bool SerialPortClosing { get; private set; } //正在关闭串口的标识
        public bool SerialPortListening { get; private set; } //串口正在接收数据的标识

        private int _weighingTimes;
        // 0:称重完成, ==1:第一次过磅, ==2:第二次过磅 
        public int WeighingTimes
        {
            get { return _weighingTimes; }
            private set
            {
                _weighingTimes = value;
                SetBtnTExt();
            }
        }
        //public ICCard IC { get; private set; } = new ICCard(); //桌面刷卡器
        public bool IsICBusy { get; private set; } = false;
        public bool NoZeroFlag { get; private set; } = true;
        public bool OverWeight { get; private set; } = false;
        public double OverWeightCount { get; private set; } = 0;
        public bool SendCmdOnce { get; private set; } = true;
        public List<string> WeighFromTemplateSheetsName { get; private set; } = new List<string>();
        public string SelectedWeighFromTemplateSheet { get; set; }
        public string LastPlate { get; private set; }
        /// <summary>
        /// 当前磅秤的状态：Waiting 等待称重，ReadyWeigh 准备称重，WeighBegin 开始称重，Weighing 称重中，WeighEnd 称重结束
        /// </summary>
        public Common.Model.Custom.WeighStatus CurrentStatus { get; private set; } = Common.Model.Custom.WeighStatus.Waiting;

        /// <summary>
        /// 首页是否显示日志，默认显示。显示日志则不显示进场图片那些。
        /// </summary>
        public bool ShowLog
        {
            get
            {
                return Convert.ToBoolean(_mainSetting.Settings["ShowLog"].Value);
            }
            set
            {
                if (_mobileConfigurationMgr != null)
                {
                    _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["ShowLog"].Value = value.ToString();
                    _mobileConfigurationMgr.SaveSetting();
                }
                RefreshAWSV2Setting(false);
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
                        var ch = weighingRecord.Ch;//Globalspace._plateNo

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
                                RefreshWeighForm();
                            }
                        }

                        if (cancelWeighingTimer != null)
                        {
                            cancelWeighingTimer.Stop();
                            //cancelWeighingTimer = null;
                        }
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

        private void RefreshWeightRecordModel(string value)
        {
            //1.先查询是否有此车号
            CarModel cm = null;
            try
            {
                cm = SQLDataAccess.LoadCar(getPlateNo);
            }
            catch { }

            weighingRecord.Ch = value;
            if (cm != null) //2.如果有此车号，根据车号查询称重记录表中查未完成的称重记录
            {
                //是否混合模式
                var weiModel = _mainSetting.Settings["WeighingMode"]?.Value ?? string.Empty;
                //TwiceWeighing = weiModel.Equals("Mixture") ? false : weiModel.Equals("Twice") ? true : false;
                TwiceWeighing = weiModel.Equals("Mixture") ? string.IsNullOrWhiteSpace(cm.VehicleWeight) ? true : false : weiModel.Equals("Twice") ? true : false;



                var wrm = SQLDataAccess.LoadWeighingRecord(weighingRecord.Ch, false);


                if (wrm == null) //没有找到未完成的称重记录，是第一次称重
                {
                    //修改车号后，如果是从第二次称重的车号修改为第一次称重的车号，需要清空页面
                    if (weighingRecord.Bh != null)
                    {
                        weighingRecord = new WeighingRecordModel()
                        {
                            Ch = value,
                            ChargeInfoConfig = _chargeInfoCfg
                        };
                    }

                    WeighingTimes = 1;
                    log.Debug("查询到车号，第一次称重:" + cm.PlateNo);
                }
                else //有未完成的称重记录，是第二次称重
                {
                    wrm.ChargeInfoConfig = _chargeInfoCfg;
                    TwiceWeighing = weiModel.Equals("Mixture") ? true : weiModel.Equals("Twice") ? true : false;
                    if (wrm.EntryTime.HasValue)
                    { //如果有记录第一次进入称重场地的时间
                        var validTime = _mainSetting.Settings["WeighingValidTime"]?.Value ?? "0";
                        TimeSpan sp = DateTime.Now.Subtract(wrm.EntryTime.Value);

                        //if (sp.TotalHours >= int.Parse(validTime))
                        if (sp.TotalHours >= double.Parse(validTime))
                        {//有记录，并且已经超时了，那么就将此未完成的记录设置为完成。并且将称重记录重置为1，从第一次开始。
                            wrm.IsFinish = true;
                            SQLDataAccess.UpdateWeighingRecord(wrm);
                            weighingRecord = new WeighingRecordModel()
                            {
                                Ch = value,
                                ChargeInfoConfig = _chargeInfoCfg
                            };
                            WeighingTimes = 1;
                            log.Debug($"查询到车号{cm.PlateNo}，第一次称重，但是超过有效期{validTime}，已经重置为第一次称重。");
                        }
                        else
                        {
                            WeighingTimes = 2;
                            weighingRecord = wrm;
                            log.Debug("查询到车号，第二次称重:" + cm.PlateNo);
                        }

                    }
                    else
                    {

                        WeighingTimes = 2;
                        weighingRecord = wrm;
                        log.Debug("查询到车号，第二次称重:" + cm.PlateNo);
                    }
                }

                if (cm.CarOwner != 0) weighingRecord.Kh3 = SQLDataAccess.LoadCustomer(cm.CarOwner)?.Name;
                if (cm.VehicleWeight != null && cm.VehicleWeight != "") weighingRecord.Pz = cm.VehicleWeight;
                WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
            }
            else //3.如果没此车号，说明是新车，第一次称重    
            {
                //是否混合模式
                var weiModel = _mainSetting.Settings["WeighingMode"]?.Value ?? string.Empty;
                TwiceWeighing = weiModel.Equals("Mixture") ? true : weiModel.Equals("Twice") ? true : false;

                WeighingTimes = 1;

                if ((_mainSetting.Settings["LPR"]?.Value ?? string.Empty) != "0" //车牌识别启用时，要更新界面
                    && Globalspace._getPlateFromLpr)    //只有车牌识别传过来的才更新，排除掉手动输入的情况
                {
                    Globalspace._getPlateFromLpr = false;

                }

                var wrm = SQLDataAccess.LoadWeighingRecord(weighingRecord.Ch, false);

                if (wrm == null) //没有找到未完成的称重记录，是第一次称重
                {
                    //修改车号后，如果是从第二次称重的车号修改为第一次称重的车号，需要清空页面
                    if (weighingRecord.Bh != null)
                    {
                        weighingRecord = new WeighingRecordModel()
                        {
                            Ch = value,
                            ChargeInfoConfig = _chargeInfoCfg
                        };
                    }
                    //WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
                }

                //var form= new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
                //WeighFormVM = form;
                //form.Focus();
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
                    log.Debug("刷新页面，初始化状态，准备好称重");
                    Globalspace._isManual = false;
                    Globalspace._plateNo = string.Empty;
                    Globalspace._lprDevNo = string.Empty;
                    //SetBtnTExt(1);//重置按钮的显示文本，因为光栅遮挡后 这里的文本会改编成 “一次称重(光栅1遮挡中)”，称完后要改成“一次称重”
                    try
                    {
                        weighingRecord = new WeighingRecordModel { ChargeInfoConfig = _chargeInfoCfg };
                        WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
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
                        NoSave = false;

                        //2023-05-11 双向模式下，并且小于最小重量，并且启用了LPR识别的时候，每次刷新会重置LPR的可识别车牌状态为可识别。否则不可识别

                        if ((_mainSetting.Settings["LPR"]?.Value ?? string.Empty) != "0")
                        {
                            if ((_mainSetting.Settings["Barrier"]?.Value ?? string.Empty) == "1")
                            {
                                // IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.READY_TO_WEIGH);
                                LPR_READY_TO_WEIGH();
                            }
                            else if (Math.Abs(Convert.ToDecimal(WeightStr)) <= Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0"))
                            {
                                //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.READY_TO_WEIGH);
                                LPR_READY_TO_WEIGH();
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        log.Debug(e.Message);
                    }
                }
                else
                {
                    log.Debug("正在称重中");
                }
            }
        }

        #region 收费模块参数 by:汪虎 2022-02-21

        private ChargeInfoConfig _chargeInfoCfg;

        #endregion

        private byte[] uart1Buf = new byte[20]; //接收称重串口1数据的buffer
        private byte[] uart2Buf = new byte[20]; //接收称重串口2数据的buffer
        private byte[] hexWeight = new byte[12]; //发送大屏幕串口数据的buffer
        private WeighingRecordModel weighingRecord = new WeighingRecordModel(); //当前称重记录表单
        private DispatcherTimer timer = new DispatcherTimer(); //定义定时器，进行周期的查询车牌号操作
        private DispatcherTimer cancelWeighingTimer; //自动复位计时器，刷卡不上秤倒计时结束后初始化称重操作
        private WorksheetCollection wss;
        private Worksheet worksheet; //过磅单模板

        public Dictionary<string, string> TemplateFieldDic { get; set; } = new Dictionary<string, string>();
        public DataTable RecordDT { get; private set; } //经过转换的称重记录表格，用来显示到列表、导出到excel
        public BindableCollection<string> SelectedItems { get; set; } = new BindableCollection<string>(); //选择的显示字段
        public int SelectedTotalWeighingCount { get; private set; }
        public decimal SelectedTotalJz { get; private set; }
        public decimal SelectedTotalSz { get; private set; }
        public Worksheet Worksheet { get; set; }

        public string FirstImg { get; set; } = "/Resources/Img/ce.png";
        public string SecondImg { get; set; } = "/Resources/Img/cf.png";
        public string ThirdImg { get; set; } = "/Resources/Img/cf.png";
        #endregion



        #region 系统验证模块参数 by：WH 2022-07-01
        /// <summary>
        /// 是否显示二维码面板
        /// </summary>
        public System.Windows.Visibility ShowQRCode { get; set; } = System.Windows.Visibility.Hidden;

        /// <summary>
        /// 二维码对象
        /// </summary>
        public System.Windows.Media.ImageSource QRCode { get; set; }


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
                if (_DataFormViewVisible == System.Windows.Visibility.Visible)
                {
                    //刷新模型
                    if (DataFormVM == null)
                    {
                        DataFormVM = new DataFormViewModel(_eventAggregator, ref _mobileConfigurationMgr);
                    }
                }
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
        public bool EnableQueryList { get; set; } = false;
        public bool EnablePrintList { get; set; } = false;
        public bool EnableExportList { get; set; } = false;
        public bool EnableDeleteList { get; set; } = false;
        public bool EnableUpdateSelected { get; set; } = false;
        public bool EnableDeleteSelected { get; set; } = false;
        public bool EnablePrintSelected { get; set; } = false;
        public bool EnableInsertItem { get; set; } = false;

        #endregion

        DispatcherTimer weightStrChangeTimer = new DispatcherTimer();
        DispatcherTimer minSoftTTSTimer = new DispatcherTimer();
        DispatcherTimer normalTTSTimer = new DispatcherTimer();
        DispatcherTimer coverTTSTimer = new DispatcherTimer();
        public bool IsMinsoftCover { get; set; } = false;

        public string StateLab { get; set; } = "信号传输异常";
        public string CurrentUserName { get; set; } = "用户切换";
        public System.Windows.Media.Brush StateBackground { get; set; } = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F70909"));

        public string ProvicerTitle
        {
            get
            {
                return AWSV2.Globalspace.ProvicerTitle;
            }
        }
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
                if ((_mainSetting.Settings["WeighingMode"]?.Value ?? string.Empty).Equals("Twice"))
                {
                    TwiceWeighing = true;
                }
                else
                {
                    TwiceWeighing = false;
                }
            }
        }
        //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11100);

        // 博兴开关 --阿吉 2023年6月26日08点38分
        private bool? _enable_Shandong_Boxing_Platform;
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
        private AWSV2.Upgrade.ServerInfoSoapClient _serverInfoSoapClient;

        private IEventAggregator _eventAggregator;

        private string _mqttStatusText;
        public string MQTTStatusText
        {
            get { return _mqttStatusText; }
            set
            {
                SetAndNotify(ref _mqttStatusText, value);
            }
        }

        private System.Timers.Timer _queryListTimer;

        //构造函数，初始化
        public ShellViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);

            _mobileConfigurationMgr = new MobileConfigurationMgr();

            ConnectToMQTT();

            _serverInfoSoapClient = new Upgrade.ServerInfoSoapClient();

            _mobileConfigurationMgr.GetLatestVersionHandler = () => _serverInfoSoapClient.GetCurrentVersion();

            _mobileConfigurationMgr.OnConfigUpdated += (s, e) =>
            {
                RefreshAWSV2Setting(false);
                LoadConfig(false);
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

            InitMobileConfigCMDMaps();

            _mobileConfigurationMgr.WeightFormTemplatePath = Globalspace._weightFormTemplatePath;

            RefreshAWSV2Setting(false);

            if (_manualPZOrMZ)
            {
                Btn1Text = "毛重";
                Btn1Text = "皮重";
            }

            this.windowManager = windowManager;

            StartQueryListTimer();

            Globalspace.ShellViewModel = this;
            CurrentUserName = $"用户切换 {AWSV2.Globalspace._currentUser.LoginId}";
            //在界面上显示数据统计模型
            DataFormViewVisible = Visibility.Visible;
            if (File.Exists(Globalspace._weightFormTemplatePath))
            {
                //加载过磅单到内存
                wss = new Workbook(Globalspace._weightFormTemplatePath).Worksheets;
                foreach (var ws in wss)
                {
                    WeighFromTemplateSheetsName.Add(ws.Name); //用来生成模板列表
                }
                SwitchTemplate("1"); //选择称重模版

                worksheet = wss[SelectedWeighFromTemplateSheet];

                //上传磅单数据到平台
                LoadExcelToList(SelectedWeighFromTemplateSheet);

                //在界面上显示过磅单模型
                WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
                LoadConfig(); //加载配置文件

                //新开线程，用来接收称重串口数据
                Thread recvDataThread = new Thread(new ThreadStart(() => WeighSerialPort.DataReceived += new SerialDataReceivedEventHandler(WeighSerialPort_DataReceived)));
                recvDataThread.IsBackground = true;
                recvDataThread.Start();

                Thread recv2DataThread = new Thread(new ThreadStart(() => Weigh2SerialPort.DataReceived += new SerialDataReceivedEventHandler(WeighSerialPort_DataReceived)));
                recv2DataThread.IsBackground = true;
                recv2DataThread.Start();

                //新开线程，用来接收二维码串口数据
                Thread recv3DataThread = new Thread(new ThreadStart(() => QRPort.DataReceived += new SerialDataReceivedEventHandler(QRPort_DataReceived)));
                recv3DataThread.IsBackground = true;
                recv3DataThread.Start();
                Thread recv4DataThread = new Thread(new ThreadStart(() => QRPort2.DataReceived += new SerialDataReceivedEventHandler(QRPort_DataReceived)));
                recv4DataThread.IsBackground = true;
                recv4DataThread.Start();

                //设置定时器，进行轮询操作
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Interval = TimeSpan.FromSeconds(0.5);
                timer.Start();

                //称重记录、数据管理应用模块启动时太耗时，此处延时启动，点击按钮时，直接显示
                var delayRunTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
                delayRunTimer.Start();
                delayRunTimer.Tick += (sender, args) =>
                {
                    delayRunTimer.Stop();

                    IPCHelper.OpenApp("ZXDataManagement", Globalspace._currentUser.LoginId);
                    //IPCHelper.OpenApp("ZXWeighingRecord", Globalspace._currentUser.LoginId);
                };

                //检查用户称重操作权限
                string rolePermission = SQLDataAccess.LoadLoginRolePermission(Globalspace._currentUser.LoginId);
                if (rolePermission != null)
                {
                    if (rolePermission.Contains("称重操作")) EnableWeighing = true;
                    if (rolePermission.Contains("系统设置")) EnableSetting = true;
                    if (rolePermission.Contains("数据管理")) EnableDataManage = true;
                    EnableDataManage = true;
                    if (rolePermission.Contains("系统日志查询")) EnableCheckSysLog = true;

                    //底部查询列表相关权限

                    //if (rolePermission.Contains("导出列表")) EnableExportList = true;
                    if (rolePermission.Contains("查询列表")) EnableQueryList = true;
                    //if (rolePermission.Contains("修改选中")) EnableUpdateSelected = true;
                    if (rolePermission.Contains("修改")) EnableUpdateSelected = true;
                    if (rolePermission.Contains("删除选中")) EnableDeleteSelected = true;
                    if (rolePermission.Contains("补打选中")) EnablePrintSelected = true;
                    if (rolePermission.Contains("手工制单")) EnableInsertItem = true;
                }

                //检测是否注册系统
                if (!Globalspace._isRegister)
                {
                    //StatusBar = "系统未注册，无法使用称重功能！";
                    if (SQLDataAccess.LoadWeighingRecordCount() > 200)
                    {
                        StatusBar = "试用期已结束，请注册后使用！";
                        EnableWeighing = false;
                    }
                }

                try
                {
                    //weightStr的值在一定时间内，如果没有继续被赋值或者改变，那么就修改UI上的 信号传输为不正常
                    weightStrChangeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
                    weightStrChangeTimer.Tick += (sender, args) =>
                    {
                        if (StateLab != "信号传输异常")
                        {
                            StateBackground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F70909"));
                            StateLab = "信号传输异常";
                        }
                    };

                    //这里扫描 道闸常开 的值是否被小程序或者其他程序该变，一旦改变就立即刷新界面上的按钮状态
                    var propertyChange = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(5000) };

                    propertyChange.Start();
                    var serverPath = Common.Utility.SettingsHelper.ZXSyncSettings.Settings["ServerPath"].Value;
                    propertyChange.Tick += (sender, args) =>
                    {
                        var isConnected = Common.Encrt.WebApi.Ping(serverPath);
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

                    Task.Run(() =>
                    {
                        //启动电子支付SDK
                        CloudLogic.CloudAPI.Start(ref _mobileConfigurationMgr);

                        // 以下是第三方平台的启动逻辑
                        //Common.Platform.PlatformManager.Instance.Init("Default");
                        //Common.Platform.PlatformManager.Instance.Init("ZJS_HZS_YHQ");
                        //Common.Platform.PlatformManager.Instance.Init("GXH_A");
                        var platformName = _mainSetting.Settings["PlatformName"]?.Value ?? string.Empty;
                        Common.Platform.PlatformManager.Instance.Init(platformName);
                        _enable_Shandong_Boxing_Platform = "shandong_boxing".Equals(platformName, StringComparison.OrdinalIgnoreCase);
                        //耗时的方法在这里延迟加载
                        CreateColumns();
                        QueryList(string.Empty);
                    });

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

                //weightStr的值在一定时间内，如果没有继续被赋值或者改变，那么就修改UI上的 信号传输为不正常
                minSoftTTSTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(3500) };
                minSoftTTSTimer.Tick += (sender, args) =>
                {
                    //检测光栅是否被遮挡
                    //if ((_mainSetting.Settings["CheckGrating"]?.Value ?? string.Empty).Equals("1") && Globalspace._signoState != "3" && string.IsNullOrWhiteSpace(weighingRecord.Ch))
                    //{
                    //    TTSHelper.TTS(weighingRecord, "红外遮挡");
                    //}
                };

                //weightStr的值大于正常重量，并且光栅被遮挡的时候，执行TTS语音播报的定时器
                normalTTSTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(3500) };
                normalTTSTimer.Tick += (sender, args) =>
                {
                    ////检测光栅是否被遮挡
                    //if ((_mainSetting.Settings["CheckGrating"]?.Value ?? string.Empty).Equals("1") && Globalspace._signoState != "3")
                    //{
                    //    TTSHelper.TTS(weighingRecord, "红外遮挡、请停到秤台中间");
                    //}
                };

                //光栅被遮挡的时候，执行TTS语音播报的定时器
                coverTTSTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(5000) };
                coverTTSTimer.Start();
                coverTTSTimer.Tick += (sender, args) =>
                {
                    ////检测光栅是否被遮挡
                    //if ((_mainSetting.Settings["CheckGrating"]?.Value ?? string.Empty).Equals("1") && Globalspace._signoState != "3")
                    //{
                    //    if (IsMinsoftCover)
                    //    {
                    //        TTSHelper.TTS(weighingRecord, "红外遮挡");
                    //    }
                    //    else
                    //    {
                    //        TTSHelper.TTS(weighingRecord, "红外遮挡、请停到秤台中间");
                    //    }
                    //}
                };

                //称重记录、数据管理应用模块启动时太耗时，此处延时启动，点击按钮时，直接显示
                var delayRunTimer1 = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                delayRunTimer1.Start();
                delayRunTimer1.Tick += (sender, args) =>
                {
                    delayRunTimer1.Stop();

                    IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.REFRESH_DI_STATE, "0");
                };

            }
            else
            {
                ShowLogs("表单模板不存在，请联系软件供应商。");

            }
        }

        private void InitMobileConfigCMDMaps()
        {
            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.平台数据纠正,
                (@params) =>
                {
                    Common.SyncData.Instal.ReUpload();
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.手动备份数据库,
                (@params) =>
                {
                    BackupHelper.ManualBackup();
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.手动数据库恢复,
                (@params) =>
                {
                    BackupHelper.DBRestore();
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.清理全部数据,
                (@params) =>
                {
                    BackupHelper.DbClean(windowManager, @params, false);
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.按月份清理数据,
                (@params) =>
                {
                    var config = AJUtil.TryGetJSONObject<CleanupConfig>(@params);

                    if (!BackupHelper.CleanupByMonthCfg(windowManager, config, false))
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
                    IPCHelper.OpenApp("ZXUpdater", "autoConfirm");
                    object ret = null;
                    return Task.FromResult(ret);
                });

            _mobileConfigurationMgr.CmdMaps.Add(MobileConfigurationMgr.CMDKey.下载数据库,
                 async (@params) =>
                {
                    var file = BackupHelper.AutoBackup(true);
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

                     var bytes = Encoding.Default.GetBytes(@params);

                     var sm = new ShareMem();
                     sm.Init("deviceInfo", bytes.Length);
                     sm.Write(bytes, 0, bytes.Length);

                     IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.UPDATEDEVICE, 
                         new IntPtr(bytes.Length), IntPtr.Zero);
                     object ret = null;
                     return Task.FromResult(ret);
                 });
        }

        private void ConnectToMQTT()
        {
            _mqttSvc = new MQTTService();

            _mobileConfigurationMgr.SetMQTTService(_mqttSvc);

            var cfg = AJUtil.TryGetJSONObject<MQTTConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(MQTTConfig)]?.Value)
                ?? new MQTTConfig();

            Task.Run(async () =>
            {
                _mqttSvc.ConnectionChanged += async (s, e) =>
                {
                    var msg = e ? "已连接" : "未连接或已断开";
                    //ShowLogs($"实时通讯服务:{msg}");
                    MQTTStatusText = $"实时通讯:{msg}";

                    await _mqttSvc.UpdateApplicationAsync(HardDiskInfo.SerialNumber, e);

                    if (e)
                    {
                        await _mqttSvc.SubscribeAsync(MQTTService.MAINAPPLICATION, (mqttMsg) =>
                        {
                            return _mobileConfigurationMgr.ProcessMessageAsync(mqttMsg.ApplicationMessage);

                        });
                    }
                };

                try
                {
                    await _mqttSvc.ConnectTCPAsync(cfg.Server, cfg.Port);
                }
                catch (Exception e)
                {
                    log.Error($"mqtt服务发生异常", e);
                    MQTTStatusText = "实时通讯:异常";
                    //ShowLogs($"实时通讯服务发生异常:{e.Message}");
                }

            });
        }

        private void StartQueryListTimer()
        {
            _queryListTimer = new System.Timers.Timer(9999 * 1000);

            _queryListTimer.Elapsed += (s, e) =>
            {
                _eventAggregator.PublishOnUIThread(new MainShellViewEvent());
                //QueryList(string.Empty);
            };

            _queryListTimer.Start();
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

        public async void Handle(MainShellViewEvent @event)
        {
            switch (@event.Type)
            {
                case MainShellViewEvent.EventType.刷新列表:
                    QueryList(string.Empty);
                    break;
                case MainShellViewEvent.EventType.重启:
                    RequestClose();
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                    break;
                case MainShellViewEvent.EventType.重量值更新:
                    var retMsg = new MQTTMessagePackage(HardDiskInfo.SerialNumber, MQTTMessageType.实时重量, true);
                    retMsg.SetSuccess(JObject.FromObject(new
                    {
                        weight = @event.Data.ToString()
                    }));
                    await _mqttSvc?.PublishAsync(MAINAPPLICATIONRESPONSE, AJUtil.AJSerializeObject(retMsg));
                    break;
                default:
                    break;
            }

        }

        private void RefreshAWSV2Setting(bool force)
        {
            if (force)
            {
                _mainSetting
                    = _mobileConfigurationMgr.SettingList[SettingNameKey.Main]
                    = SettingsHelper.Get("衡七管家.exe", true);
            }
            else
            {
                _mainSetting = _mobileConfigurationMgr.SettingList[SettingNameKey.Main];
            }

            _manualPZOrMZ = (_mainSetting.Settings["ManualPZOrMZ"]?.Value ?? "False").Equals("true", StringComparison.OrdinalIgnoreCase);
            _isCheckGrating = (_mainSetting.Settings["CheckGrating"]?.Value ?? "0").Equals("1");
            _gratingTTSConfig = AJUtil.TryGetJSONObject<GratingTTSConfig>(_mainSetting.Settings[nameof(GratingTTSConfig)]?.Value ?? string.Empty) ?? new GratingTTSConfig();
            _fastWeigthCorrectConfig = AJUtil.TryGetJSONObject<FastWeigthCorrectConfig>(_mainSetting.Settings[nameof(FastWeigthCorrectConfig)]?.Value ?? string.Empty) ?? new FastWeigthCorrectConfig();
            _snapWatermarkConfig = AJUtil.TryGetJSONObject<SnapWatermarkConfig>(_mainSetting.Settings[nameof(SnapWatermarkConfig)]?.Value ?? string.Empty) ?? new SnapWatermarkConfig();
        }

        private void LoadExcelToList(string templateName)
        {
            //读取过磅单中的字段
            try
            {
                //打开过磅单模板
                Worksheet worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[templateName];

                //设置查找区域：有内容的区域
                var range = worksheet.Cells.MaxDisplayRange;
                string cellRange = range.RefersTo.Split('!')[1].Replace("$", "");
                string startCellName = cellRange.Split(':')[0];
                string endCellName = cellRange.Split(':')[1];
                CellArea area = CellArea.CreateCellArea(startCellName, endCellName);
                //查找规则
                FindOptions opts = new FindOptions();
                opts.LookInType = LookInType.Values;
                opts.LookAtType = LookAtType.Contains;
                opts.SetRange(area);

                Cell cell = null;
                List<dynamic> WeighFormList = new List<dynamic>();
                do
                {
                    cell = worksheet.Cells.Find("_", cell, opts);
                    if (cell == null)
                        break;
                    try
                    {
                        WeighFormList.Add(new { label = _mainSetting.Settings[cell.Value.ToString()]?.Value ?? "配置缺失", name = cell.Value.ToString() });
                    }
                    catch { }
                } while (true);

                WeighFormList = WeighFormList.Distinct().ToList();
                if (Common.Encrt.WebApi.Ping())
                    Common.SyncData.Up_WeighForm(JsonConvert.SerializeObject(WeighFormList));

            }
            catch { }
        }

        //加载配置文件
        private void LoadConfig(bool vifCode = true)
        {
            //称重模式：单次称重/二次称重
            if ((_mainSetting.Settings["WeighingMode"]?.Value ?? string.Empty).Equals("Twice"))
            {
                TwiceWeighing = true;
                //WeighModeIcon = "Numeric1Box";
            }
            else
            {
                TwiceWeighing = false;
                //WeighModeIcon = "Numeric2Box";
            }
            WeighingMode = _mainSetting.Settings["WeighingMode"]?.Value ?? string.Empty;
            WeighName1 = _mainSetting.Settings["WeighName"]?.Value ?? string.Empty;
            WeighingUnit = _mainSetting.Settings["WeighingUnit"]?.Value ?? "--";
            var weighingControl = _mainSetting.Settings["WeighingControl"]?.Value ?? string.Empty;
            //称重控制：手动称重/自动称重
            if (weighingControl.Equals("Hand") || weighingControl.Equals("Btn"))
            {
                ManualWeighing = true;
                WeighModeIcon = "BrightnessAuto";
            }
            else
            {
                ManualWeighing = false;
                WeighModeIcon = "HumanHandsdown";
            }

            //打开称重串口和设置协议
            if (WeighSerialPort.IsOpen)
            {
                SerialPortClosing = true;
                while (SerialPortListening) ;
                WeighSerialPort.Close();
                SerialPortClosing = false;
            }

            WeighSerialPort.PortName = AJUtil.TryGetSerialPortName(_mainSetting.Settings["WeighSerialPortName"]?.Value);
            WeighSerialPort.BaudRate = AJUtil.TryGetSerialPortRate(_mainSetting.Settings["WeighSerialPortBaudRate"]?.Value);
            WeighSerialPort.DtrEnable = true;
            WeighSerialPort.RtsEnable = true;
            try
            {
                WeighSerialPort.Open();
                log.Debug(WeighSerialPort.PortName + " is open");
            }
            catch (Exception e)
            {
                StatusBar = "称重串口打开失败！" + e.Message;
                ShowLogs($"仪表串口被占用：{WeighSerialPort.PortName}");
                //隐藏日志面板，显示统计面板
                WeighFormViewVisible = Visibility.Visible;
                DataFormViewVisible = Visibility.Hidden;
            }

            WeighProtrcolType = _mainSetting.Settings["WeighProtocolType"]?.Value ?? string.Empty;

            //启动/关闭副设备串口和协议
            if ((_mainSetting.Settings["EnableSecondDevice"]?.Value ?? string.Empty).Equals("True"))
            {
                HaveSecondDevice = true;

                if (Weigh2SerialPort.IsOpen)
                {
                    SerialPortClosing = true;
                    while (SerialPortListening) ;
                    Weigh2SerialPort.Close();
                    SerialPortClosing = false;
                }
                Weigh2SerialPort.PortName = AJUtil.TryGetSerialPortName(_mainSetting.Settings["Weigh2SerialPortName"]?.Value);
                Weigh2SerialPort.BaudRate = AJUtil.TryGetSerialPortRate(_mainSetting.Settings["Weigh2SerialPortBaudRate"]?.Value);
                Weigh2SerialPort.DtrEnable = true;
                Weigh2SerialPort.RtsEnable = true;
                try
                {
                    Weigh2SerialPort.Open();
                }
                catch (Exception e)
                {
                    StatusBar = "称重串口2打开失败！" + e.Message;
                }

                Weigh2ProtrcolType = _mainSetting.Settings["Weigh2ProtocolType"]?.Value ?? string.Empty;
            }
            else
            {
                HaveSecondDevice = false;
                SelectedDevice1 = false;

                if (Weigh2SerialPort.IsOpen)
                {
                    SerialPortClosing = true;
                    while (SerialPortListening) ;
                    Weigh2SerialPort.Close();
                    SerialPortClosing = false;
                }
            }

            //启动/关闭外设
            //1. 监控摄像头
            if ((_mainSetting.Settings["MonitorEnable"]?.Value ?? string.Empty).Equals("True"))
            {
                IPCHelper.OpenApp("ZXMonitor");
            }
            else
            {
                IPCHelper.CloseApp("ZXMonitor");
            }

            //2. 外接大屏幕
            if ((_mainSetting.Settings["LEDEnable"]?.Value ?? string.Empty).Equals("True"))
            {
                //关闭大屏幕串口
                if (LEDSerialPort.IsOpen)
                {
                    SerialPortClosing = true;
                    while (SerialPortListening) ;
                    LEDSerialPort.Close();
                    SerialPortClosing = false;
                    LEDSerialPortEnable = false;
                }

                //按照设置重新打开大屏幕串口
                LEDSerialPort.PortName = AJUtil.TryGetSerialPortName(_mainSetting.Settings["LEDPortName"]?.Value);
                LEDSerialPort.BaudRate = 2400;
                try
                {
                    LEDSerialPort.Open();
                    LEDSerialPortEnable = true;
                }
                catch (Exception e)
                {
                    StatusBar = "大屏幕串口打开失败！" + e.Message;
                }
            }
            else
            {
                if (LEDSerialPort.IsOpen)
                {
                    SerialPortClosing = true;
                    while (SerialPortListening) ;
                    LEDSerialPort.Close();
                    SerialPortClosing = false;
                    LEDSerialPortEnable = false;
                }
            }

            // 版本之间逻辑检测和处理
            if (Common.Share.VersionControl.CurrentVersion == Common.Share.VersionType.标准版)
            {
                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["Barrier"].Value = "1";
                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["ChargeEnable"].Value = "0";
                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["KeepOpen"].Value = "False";

                _mobileConfigurationMgr.SettingList[SettingNameKey.VirtualWall].Settings["VirtualWall"].Value = "0";

                _mobileConfigurationMgr.SaveSetting();
                _mobileConfigurationMgr.SaveSetting(SettingNameKey.VirtualWall);
            }


            if (_mainSetting.Settings["LED2Enable"].Value.Equals("True"))
            {
                IPCHelper.OpenApp("ZXLED");
            }
            else
            {
                IPCHelper.CloseApp("ZXLED");
            }

            //3.数据同步上传APP
            if (_mainSetting.Settings["SyncDataEnable"].Value.Equals("True"))
            {
                IPCHelper.OpenApp("ZXSyncWeighingData");

            }
            else
            {
                IPCHelper.CloseApp("ZXSyncWeighingData");
            }

            //4. 车牌识别APP
            IPCHelper.KillApp("ZXLPR");//先杀为敬
            if (_mainSetting.Settings["LPR"].Value.Equals("0"))
            {
                IPCHelper.KillApp("ZXLPR");
            }
            else
            {
                Task.Run(() =>
                {
                    if (_mainSetting.Settings["LPR"].Value.Equals("1")) IPCHelper.OpenApp("ZXLPR", "1");
                    if (_mainSetting.Settings["LPR"].Value.Equals("2")) IPCHelper.OpenApp("ZXLPR", "2");
                    Thread.Sleep(500);
                    //2023-05-11 因为加了双向模式下 大于最小重量 不能识别车牌的逻辑，故而在从新打开LPR程序的时候要判断双向模式下是否大于最小重量
                    //从而指定LPR的可识别车牌的状态 需求越做越细，逻辑越来约紧凑。到处都是。后期维修 牵一发动全身了。要注意
                    if (!string.IsNullOrEmpty(WeightStr) &&
                        _mainSetting.Settings["Barrier"].Value != "1" &&
                        Math.Abs(Convert.ToDecimal(WeightStr)) >
                        Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0"))
                    {
                        lprMessage($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", "正在称重", "居中停稳", "熄火下车", IPCHelper.IS_WEIGHING, "r");
                    }
                });
            }

            //5. 明华桌面读卡器
            if (_mainSetting.Settings["TableRFEnable"].Value.Equals("True"))
            {
                short port = Convert.ToInt16((_mainSetting.Settings["TableRFPortName"]?.Value ?? "000").Substring(3));
                if (Globalspace._icdev.ToInt32() == 0)
                {

                    try { Globalspace._icdev = ICCard.rf_init(--port, 9600); } catch { }
                }

                if (Globalspace._icdev.ToInt32() > 0)
                {
                    log.Debug("桌面读卡器连接成功!");
                    ICCard.rf_beep(Globalspace._icdev, 15);
                }
                else
                    log.Debug("桌面读卡器连接失败!");
            }
            else
            {
                if (Globalspace._icdev.ToInt32() > 0)
                {
                    if (0 == ICCard.rf_exit(Globalspace._icdev))
                    {
                        log.Debug("明华桌面读卡器关闭");
                        Globalspace._icdev = IntPtr.Zero;
                    }

                }
            }

            //6. 二维码
            if (_mainSetting.Settings["QREnable"].Value.Equals("True"))
            {
                //关闭二维码串口
                if (QRPort.IsOpen)
                {
                    SerialPortClosing = true;
                    while (SerialPortListening) ;
                    QRPort.Close();
                    SerialPortClosing = false;
                }

                //按照设置重新打开二维码串口
                QRPort.PortName = AJUtil.TryGetSerialPortName(_mainSetting.Settings["QRPortName"]?.Value);
                QRPort.BaudRate = 9600;
                try
                {
                    QRPort.Open();
                }
                catch (Exception e)
                {
                    StatusBar = "二维码串口打开失败！" + e.Message;
                }
            }
            else
            {
                if (QRPort.IsOpen)
                {
                    SerialPortClosing = true;
                    while (SerialPortListening) ;
                    QRPort.Close();
                    SerialPortClosing = false;
                }
            }
            if (_mainSetting.Settings["QR2Enable"].Value.Equals("True"))
            {
                //关闭二维码串口
                if (QRPort2.IsOpen)
                {
                    SerialPortClosing = true;
                    while (SerialPortListening) ;
                    QRPort2.Close();
                    SerialPortClosing = false;
                }

                //按照设置重新打开二维码串口
                QRPort2.PortName = AJUtil.TryGetSerialPortName(_mainSetting.Settings["QRPort2Name"]?.Value);
                QRPort2.BaudRate = 9600;
                try
                {
                    QRPort2.Open();
                }
                catch (Exception e)
                {
                    StatusBar = "二维码串口2打开失败！" + e.Message;
                }
            }
            else
            {
                if (QRPort2.IsOpen)
                {
                    SerialPortClosing = true;
                    while (SerialPortListening) ;
                    QRPort2.Close();
                    SerialPortClosing = false;
                }
            }


            //7. 车轴识别
            if (_mainSetting.Settings["ZXAxleNo"].Value.Equals("True"))
            {
                IPCHelper.OpenApp("ZXAxleNo");
            }
            else
            {
                IPCHelper.CloseApp("ZXAxleNo");
            }

            //8. 收费参数加载
            _chargeInfoCfg = new ChargeInfoConfig(_mainSetting);

            //9.电子围栏开启
            var vwSection = _mobileConfigurationMgr.SettingList[SettingNameKey.VirtualWall];
            var virtualWallState = vwSection.Settings["VirtualWall"].Value;
            if (!vwSection.Settings["VirtualWall"].Value.Equals("0"))//0、不启用电子围栏，1、启用但是用高频读头，2、启用但是用视频
            {
                IPCHelper.OpenApp("ZXVirtualWall");
            }
            else
            {
                IPCHelper.CloseApp("ZXVirtualWall");
            }

            //10.数据备份中图片和视频自定义储存位置初始化，如果在用户没设置的时候这里应该是空路径，但是程序会出错，所以要初始化根目录


            var lprSection = _mobileConfigurationMgr.SettingList[SettingNameKey.LPR];
            var monitorSection = _mobileConfigurationMgr.SettingList[SettingNameKey.Monitor];

            var lprSavePath = lprSection.Settings["LPRSavePath"]?.Value ?? Path.Combine(Environment.CurrentDirectory, "Snap", "pic");

            if (string.IsNullOrEmpty(lprSavePath))
            {
                lprSection.Settings["LPRSavePath"].Value = lprSavePath;
                _mobileConfigurationMgr.SaveSetting(SettingNameKey.LPR);
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
                    _mobileConfigurationMgr.SaveSetting(SettingNameKey.LPR);
                }
            }

            var monitorSavePath = monitorSection.Settings["MonitorSavePath"]?.Value
                ?? Path.Combine(Environment.CurrentDirectory, "Snap", "video");

            if (string.IsNullOrEmpty(monitorSavePath))
            {
                monitorSection.Settings["MonitorSavePath"].Value = Path.Combine(Environment.CurrentDirectory, "Snap", "video");
                _mobileConfigurationMgr.SaveSetting(SettingNameKey.LPR);
            }
            else
            {
                try
                {
                    if (!System.IO.Directory.Exists(monitorSavePath)) System.IO.Directory.CreateDirectory(monitorSavePath);
                }
                catch
                {
                    monitorSection.Settings["MonitorSavePath"].Value = monitorSavePath;
                    _mobileConfigurationMgr.SaveSetting(SettingNameKey.Monitor);
                }
            }
            if (vifCode)
                VerifyLicense();
            ShowWeighingSet();
        }


        public DataRowView SelectedRecrod { get; set; } //选中的称重记录，补充打印用
        public List<WeighingRecordModel> WList { get; set; } = new List<WeighingRecordModel>(); //从数据库中查询出的原始称重记录列表
        public void QueryList(string parameter)
        {
            try
            {
                var maxAbnormalData = SettingsHelper
                    .ZXWeighingRecordSettings.Settings["MaxAbnormalData"]?.Value.TryGetDecimal() ?? 100000m;

                WList = SQLDataAccess.GetLimits(parameter, maxAbnormalData, 20);
                ////处理数据，放入DataTable中
                var dt = new DataTable("WeighingRecord");
                //根据选择的显示字段，构造DataTabel 
                dt.Columns.Add(_mainSetting.Settings["_bh"].Value);
                dt.Columns.Add(_mainSetting.Settings["_weighName"].Value);
                dt.Columns.Add(_mainSetting.Settings["_weigh2Name"].Value);
                //dt.Columns.Add("超载");
                //dt.Columns.Add("车牌");
                //dt.Columns.Add("是否遮挡");
                //dt.Columns.Add("支付方式");
                //dt.Columns.Add(new DataColumn() { ColumnName = "_selected", DataType = typeof(bool) });
                foreach (var item in SelectedItems)
                {
                    if (!dt.Columns.Contains(item))
                    {
                        dt.Columns.Add(item);
                    }
                }
                //修改列类型
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName == _mainSetting.Settings["_mz"].Value ||
                        col.ColumnName == _mainSetting.Settings["_pz"].Value ||
                        col.ColumnName == _mainSetting.Settings["_jz"].Value ||
                        col.ColumnName == _mainSetting.Settings["_kz"].Value ||
                        col.ColumnName == _mainSetting.Settings["_sz"].Value ||
                        col.ColumnName == _mainSetting.Settings["_dj"].Value ||
                        col.ColumnName == _mainSetting.Settings["_je"].Value)
                    {
                        col.DataType = typeof(decimal);
                    }
                }

                //APP打开后，再新增到数据库的数据，列表中没有，所以要再查一次
                var CustomerList = new BindableCollection<CustomerModel>(SQLDataAccess.LoadActiveCustomer(2));
                var GoodsList = new BindableCollection<GoodsModel>(SQLDataAccess.LoadActiveGoods(2));
                var CarList = new BindableCollection<CarModel>(SQLDataAccess.LoadActiveCar(2));

                //根据选择的显示字段，处理条件筛选后获得的数据
                foreach (var wr in WList)
                {
                    DataRow row = dt.NewRow();
                    //row["_selected"] = false;
                    row[_mainSetting.Settings["_bh"].Value] = wr.Bh;
                    //row["车牌"] = wr.Ch;
                    //row["超载"] = wr.IsLimit ? "是" : "否";
                    row[_mainSetting.Settings["_weighName"].Value] = wr.WeighName;
                    row[_mainSetting.Settings["_weigh2Name"].Value] = wr.Weigh2Name;
                    //row["是否遮挡"] = wr.IsCover ? "是" : "否";
                    //row["支付方式"] = wr.IsPay == 1 ? "电子支付" : wr.IsPay == 2 ? "线下支付" : wr.IsPay == 3 ? "储值支付" : wr.IsPay == 4 ? "免费放行" : "未支付";

                    foreach (var item in SelectedItems)
                    {
                        if (item == _mainSetting.Settings["_isLimit"].Value)
                        {
                            row[item] = wr.IsLimit ? "是" : "否";
                        }
                        if (item == _mainSetting.Settings["_isCover"].Value)
                        {
                            row[item] = wr.IsCover.GetValueOrDefault() ? "是" : "否";
                        }
                        if (item == _mainSetting.Settings["_isPay"].Value)
                        {
                            row[item] = wr.IsPay == 1 ? "电子支付" : wr.IsPay == 2 ? "线下支付" : wr.IsPay == 3 ? "储值支付" : wr.IsPay == 4 ? "免费放行" : "未支付";
                        }
                        if (item == _mainSetting.Settings["_iccard"].Value)
                        {
                            row[item] = wr.ICCard;
                        }
                        if (item == _mainSetting.Settings["_fhdw"].Value)
                        {
                            row[item] = wr.Fhdw;
                        }
                        if (item == _mainSetting.Settings["_kh"].Value)
                        {
                            if (CustomerList.Count > 0 && wr.Kh != null && wr.Kh != "")
                                row[item] = CustomerList.FirstOrDefault(c => c.Name == wr.Kh)?.Name;
                        }

                        if (item == _mainSetting.Settings["_ch"].Value)
                        {
                            //if (CarList.Count > 0 && wr.Ch != null)
                            //    row[item] = CarList.SingleOrDefault(c => c.PlateNo == wr.Ch)?.PlateNo;
                            row[item] = wr.Ch;
                        }
                        if (item == _mainSetting.Settings["_wz"].Value)
                        {
                            //if (GoodsList.Count > 0 && wr.Wz != null && wr.Wz != "")
                            //    row[item] = GoodsList.FirstOrDefault(g => g.Name == wr.Wz)?.Name;
                            row[item] = wr.Wz;
                        }
                        if (item == _mainSetting.Settings["_je"].Value)
                        {
                            if (!string.IsNullOrWhiteSpace(wr.Je))
                                row[item] = Convert.ToDecimal(wr.Je);
                        }
                        if (item == _mainSetting.Settings["_dj"].Value)
                        {
                            row[item] = wr.GoodsPrice;
                        }
                        if (item == _mainSetting.Settings["_mz"].Value)
                        {
                            if (!string.IsNullOrWhiteSpace(wr.Mz))
                                row[item] = wr.Mz;
                        }
                        if (item == _mainSetting.Settings["_mzrq"].Value)
                        {
                            row[item] = wr.Mzrq;
                        }
                        if (item == _mainSetting.Settings["_mzsby"].Value)
                        {
                            row[item] = wr.Mzsby;
                        }
                        if (item == _mainSetting.Settings["_pz"].Value)
                        {
                            if (!string.IsNullOrWhiteSpace(wr.Pz))
                                row[item] = wr.Pz;
                        }
                        if (item == _mainSetting.Settings["_pzrq"].Value)
                        {
                            row[item] = wr.Pzrq;
                        }
                        if (item == _mainSetting.Settings["_pzsby"].Value)
                        {
                            row[item] = wr.Pzsby;
                        }
                        if (item == _mainSetting.Settings["_jz"].Value)
                        {
                            var jzVal = wr.Jz.TryGetDecimal();
                            if (!string.IsNullOrWhiteSpace(wr.Jz))
                                row[item] = jzVal;

                            try
                            {
                                SelectedTotalJz += jzVal;
                            }
                            catch { }
                        }
                        if (item == _mainSetting.Settings["_jzrq"].Value)
                        {
                            row[item] = wr.Jzrq;
                        }
                        if (item == _mainSetting.Settings["_kz"].Value)
                        {
                            if (!String.IsNullOrWhiteSpace(wr.Kz))
                                row[item] = Convert.ToDecimal(wr.Kz);
                        }
                        if (item == _mainSetting.Settings["_kl"].Value)
                        {
                            row[item] = wr.Kl;
                        }
                        if (item == _mainSetting.Settings["_sz"].Value)
                        {
                            if (!String.IsNullOrWhiteSpace(wr.Sz))
                                row[item] = Convert.ToDecimal(wr.Sz);

                            try
                            {
                                SelectedTotalSz += Convert.ToDecimal(wr.Sz);
                            }
                            catch { }
                        }
                        if (item == _mainSetting.Settings["_bz"].Value)
                        {
                            row[item] = wr.Bz;
                        }
                        if (item == _mainSetting.Settings["_by1"].Value)
                        {
                            row[item] = wr.By1;
                        }
                        if (item == _mainSetting.Settings["_by2"].Value)
                        {
                            row[item] = wr.By2;
                        }
                        if (item == _mainSetting.Settings["_by3"].Value)
                        {
                            row[item] = wr.By3;
                        }
                        if (item == _mainSetting.Settings["_by4"].Value)
                        {
                            row[item] = wr.By4;
                        }
                        if (item == _mainSetting.Settings["_by5"].Value)
                        {
                            row[item] = wr.By5;
                        }
                        if (item == _mainSetting.Settings["_by6"].Value)
                        {
                            row[item] = wr.By6;
                        }
                        if (item == _mainSetting.Settings["_by7"].Value)
                        {
                            row[item] = wr.By7;
                        }
                        if (item == _mainSetting.Settings["_by8"].Value)
                        {
                            row[item] = wr.By8;
                        }
                        if (item == _mainSetting.Settings["_by9"].Value)
                        {
                            row[item] = wr.By9;
                        }
                        if (item == _mainSetting.Settings["_by10"].Value)
                        {
                            row[item] = wr.By10;
                        }
                        if (item == _mainSetting.Settings["_by11"].Value)
                        {
                            row[item] = wr.By11;
                        }
                        if (item == _mainSetting.Settings["_by12"].Value)
                        {
                            row[item] = wr.By12;
                        }
                        if (item == _mainSetting.Settings["_by13"].Value)
                        {
                            row[item] = wr.By13;
                        }
                        if (item == _mainSetting.Settings["_by14"].Value)
                        {
                            row[item] = wr.By14;
                        }
                        if (item == _mainSetting.Settings["_by15"].Value)
                        {
                            row[item] = wr.By15;
                        }
                        if (item == _mainSetting.Settings["_by16"].Value)
                        {
                            row[item] = wr.By16;
                        }
                        if (item == _mainSetting.Settings["_by17"].Value)
                        {
                            row[item] = wr.By17;
                        }
                        if (item == _mainSetting.Settings["_by18"].Value)
                        {
                            row[item] = wr.By18;
                        }
                        if (item == _mainSetting.Settings["_by19"].Value)
                        {
                            row[item] = wr.By19;
                        }
                        if (item == _mainSetting.Settings["_by20"].Value)
                        {
                            row[item] = wr.By20;
                        }
                        if (item == _mainSetting.Settings["_gblx"].Value)
                        {
                            row[item] = wr.Gblx;
                        }
                        if (item == _mainSetting.Settings["_axleNum"].Value)
                        {
                            row[item] = wr.AxleNum;
                        }
                        if (item == _mainSetting.Settings["_kh2"].Value)
                        {
                            row[item] = wr.Kh2;
                        }
                        if (item == _mainSetting.Settings["_kh3"].Value)
                        {
                            row[item] = wr.Kh3;
                        }
                        if (item == _mainSetting.Settings["_goodsSpec"].Value)
                        {
                            row[item] = wr.GoodsSpec;
                        }
                        //2022-11-03新增字段显示
                        if (item == _mainSetting.Settings["_driver"].Value)
                        {
                            row[item] = wr.Driver;
                        }
                        if (item == _mainSetting.Settings["_driverPhone"].Value)
                        {
                            row[item] = wr.DriverPhone;
                        }
                        if (item == _mainSetting.Settings["_entryTime"].Value)
                        {
                            row[item] = wr.EntryTime;
                        }
                        if (item == _mainSetting.Settings["_zflsh"].Value)
                        {
                            row[item] = wr.Zflsh;
                        }
                        if (item == _mainSetting.Settings["_zfje"].Value)
                        {
                            row[item] = wr.Zfje;
                        }
                        if (item == _mainSetting.Settings["_zfsj"].Value)
                        {
                            row[item] = wr.Zfsj;
                        }
                        if (item == _mainSetting.Settings["_limitType"].Value)
                        {
                            row[item] = wr.LimitType;
                        }
                        if (item == _mainSetting.Settings["_limitedValue"].Value)
                        {
                            row[item] = wr.LimitedValue;
                        }
                        if (item == _mainSetting.Settings["_serialNumber"].Value)
                        {
                            row[item] = wr.SerialNumber;
                        }
                    }
                    dt.Rows.Add(row);
                }
                //把datatable set到属性RecordDT
                RecordDT = dt;
                SelectedTotalWeighingCount = dt.Rows.Count;

            }
            catch (Exception e) { log.Debug($"QueryList Error:{e.Message}"); }
        }

        public void Reprint()
        {
            if (SelectedRecrod == null)
            {
                StatusBar = "请选择一条记录";
            }
            else
            {
                var Bh = SelectedRecrod.Row.ItemArray[0].ToString();
                var Record = Common.Data.SQLDataAccess.GetWeighingRecordByBh(Bh);

                if (Record == null) return;

                Record.Dyrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var model = AJUtil.TryGetJSONObject<AWSV2.Models.WeighingRecordModel>(AJUtil.AJSerializeObject(Record));
                AWSV2.Services.PrintHelper.PrintImg(model, Worksheet, model.IsFinish ? "SecondWeighing" : "FirstWeighing");
                //Common.Utility.PrintHelper.PrintImg(Record, Worksheet);

                log.Info("补充打印磅单:" + Record.Bh);
            }
        }

        public void UpdateSelected()
        {
            if (SelectedRecrod == null)
            {
                StatusBar = "请选择一条记录";
            }
            else
            {
                var Bh = SelectedRecrod.Row.ItemArray[0].ToString();
                var Record = WList.Find(w => w.Bh == Bh);

                var validator = new WeighingRecordViewModelValidator();
                var validatorAdapter = new FluentModelValidator<WeighingRecordViewModel>(validator);

                var viewModel = new WeighingRecordViewModel(validatorAdapter, TemplateFieldDic, this.windowManager, Record);
                var result = this.windowManager.ShowDialog(viewModel);
                if (result.GetValueOrDefault(true))
                {
                    QueryList(string.Empty);
                    StatusBar = "修改记录完成";
                }
            }
        }

        public void DeleteSelected()
        {
            try
            {
                if (SelectedRecrod == null)
                {
                    StatusBar = "请选择一条记录";
                    return;
                }

                {
                    //var vm = new PasswordViewModel(windowManager, 3);
                    bool? result = true;// windowManager.ShowDialog(vm);

                    if (result.GetValueOrDefault(true))
                    {
                        var Bh = SelectedRecrod.Row.ItemArray[0].ToString();
                        var Record = WList.Find(w => w.Bh == Bh);

                        //if (DelBtnText == "作废选中")
                        //{
                        //    SQLDataAccess.DeleteWeighingRecord(Record);
                        //    //StatusBar = "选中记录已删除";
                        //    //DelBtnText = "取消作废";
                        //}
                        //else//取消作废
                        //{
                        //    SQLDataAccess.ResetWeighingRecord(Record);
                        //    //StatusBar = "选中记录已恢复";
                        //    //DelBtnText = "作废选中";
                        //}
                        SQLDataAccess.DeleteWeighingRecord(Record);
                        QueryList(string.Empty);
                        //上传平台
                        var obj = Common.Data.SQLDataAccess.GetWeighingRecordByBh(Bh);
                        if (obj != null)
                        {
                            Common.SyncData.Instal.ModifyWeinightRecord(obj, Common.Api.edit);
                        }
                    }
                }
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

            var viewModel = new WeighingRecordViewModel(validatorAdapter, TemplateFieldDic, this.windowManager, null);
            var result = this.windowManager.ShowDialog(viewModel);
            if (result.GetValueOrDefault(true))
            {
                QueryList(string.Empty);
                StatusBar = "新增记录完成";
            }
        }

        public void OpenPicFolder()
        {
            string path = _mobileConfigurationMgr.SettingList[SettingNameKey.LPR].Settings["LPRSavePath"].Value;
            //string path = Environment.CurrentDirectory + "/Snap/";
            if (SelectedRecrod != null)
            {
                //string path2 = path +"//"+ SelectedRecrod.Row.ItemArray[0].ToString();
                var record = Common.Data.SQLDataAccess.GetWeighingRecordByBh(SelectedRecrod.Row.ItemArray[0].ToString());
                if (record == null)
                {
                    if (Directory.Exists(path)) Process.Start(path);
                    return;
                }

                string path2 = path + "\\" + record.Ch + "\\" + SelectedRecrod.Row.ItemArray[0].ToString();
                if (Directory.Exists(path2))
                {
                    Globalspace.ImagsSource = path2;
                    //Process.Start(path2);
                    windowManager.ShowDialog(new PicListViewModel(windowManager, path2));
                }
                //else if (Directory.Exists(path))
                //{
                //    Process.Start(path);
                //}
                Globalspace.ImagsSource = string.Empty;
            }
            else
            {
                if (Directory.Exists(path)) Process.Start(path);
            }

        }

        /// <summary>
        /// 当出现逃费，程序强制关闭收费对话框的时候。标记该次逃费的称重记录不保存。
        /// </summary>
        bool NoSave = false;
        //称重串口eventhandler
        private void WeighSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (SerialPortClosing) return;
            var wstrl = WeightStr1; ;
            try
            {
                SerialPortListening = true;//设置标记，说明我已经开始处理数据。  
                //获取串口Hex数据，放到全局数组中
                SerialPort sp = (SerialPort)sender;
                //根据所选协议，将串口字符放进buffer中
                if (sp.PortName.Equals(WeighSerialPort.PortName))
                {
                    switch (WeighProtrcolType)
                    {
                        case "西班牙"://01 02 30 34 30 32 30 30 02 30 31 30 30 30 30 31 30 2E 6B 67 20 02 30 32 30 30 30 30 30 30 2E 6B 67 20 02 30 33 30 30 30 30 31 30 2E 6B 67 20 0D 0A 
                            uart1Buf = Encoding.Default.GetBytes(sp.ReadLine());
                            wstrl = ProtocolParseHelper.ProtocolParse(WeighProtrcolType, uart1Buf);
                            break;
                        case "耀华":  //02(2B 30 30 30 30 30 30 32 31 3B 03)
                            if (sp.ReadByte() == 0x02)
                            {
                                while (sp.BytesToRead < 11) ;
                                sp.Read(uart1Buf, 0, 11);
                                wstrl = ProtocolParseHelper.ProtocolParse(WeighProtrcolType, uart1Buf);
                            }
                            break;

                        case "昌信":  //FF(11 00 00 00)
                            if (sp.ReadByte() == 0xFF)
                            {
                                while (sp.BytesToRead < 4) ;
                                sp.Read(uart1Buf, 0, 4);
                                wstrl = ProtocolParseHelper.ProtocolParse(WeighProtrcolType, uart1Buf);
                            }
                            break;

                        case "金钟":  //ST,GS,+00000.0kg\r\n                        
                            uart1Buf = Encoding.Default.GetBytes(sp.ReadLine());
                            wstrl = ProtocolParseHelper.ProtocolParse(WeighProtrcolType, uart1Buf);
                            break;

                        case "衡天":  //(02 20 30 20 30 30 30 30 30 30 0D)0A
                            uart1Buf = Encoding.Default.GetBytes(sp.ReadLine());
                            wstrl = ProtocolParseHelper.ProtocolParse(WeighProtrcolType, uart1Buf);
                            break;

                        case "托利多": //02(3A 30 20 20 20 20 20 20 30)20 20 20 20 20 30 0D C7 
                            if (sp.ReadByte() == 0x02)
                            {
                                while (sp.BytesToRead < 16) ;
                                sp.Read(uart1Buf, 0, 16);
                                wstrl = ProtocolParseHelper.ProtocolParse(WeighProtrcolType, uart1Buf);
                            }
                            break;

                        case "A27": //=84.1200
                            if (sp.ReadByte() == 0x3D)
                            {
                                while (sp.BytesToRead < 7) ;
                                sp.Read(uart1Buf, 0, 7);
                                wstrl = ProtocolParseHelper.ProtocolParse(WeighProtrcolType, uart1Buf);
                            }
                            break;

                        case "A12": //77(6E 30 30 31 39 2E 38 36 6B 67 0D 0A)
                            if (sp.ReadByte() == 0x77)
                            {
                                while (sp.BytesToRead < 12) ;
                                sp.Read(uart1Buf, 0, 12);
                                wstrl = ProtocolParseHelper.ProtocolParse(WeighProtrcolType, uart1Buf);
                            }
                            break;

                        default:
                            break;
                    }
                }
                if (sp.PortName.Equals(Weigh2SerialPort.PortName) && HaveSecondDevice)
                {
                    switch (Weigh2ProtrcolType)
                    {
                        case "耀华":  //02(2B 30 30 30 30 30 30 32 31 3B 03)
                            if (sp.ReadByte() == 0x02)
                            {
                                while (sp.BytesToRead < 11) ;
                                sp.Read(uart2Buf, 0, 11);
                                WeightStr2 = ProtocolParseHelper.ProtocolParse(Weigh2ProtrcolType, uart2Buf);
                            }
                            break;

                        case "昌信":  //FF(11 00 00 00)
                            if (sp.ReadByte() == 0xFF)
                            {
                                while (sp.BytesToRead < 4) ;
                                sp.Read(uart2Buf, 0, 4);
                                WeightStr2 = ProtocolParseHelper.ProtocolParse(Weigh2ProtrcolType, uart2Buf);
                            }
                            break;

                        case "金钟":  //ST,GS,+00000.0kg\r\n                        
                            uart2Buf = Encoding.Default.GetBytes(sp.ReadLine());
                            WeightStr2 = ProtocolParseHelper.ProtocolParse(Weigh2ProtrcolType, uart2Buf);
                            break;

                        case "衡天":  //(02 20 30 20 30 30 30 30 30 30 0D)0A
                            uart2Buf = Encoding.Default.GetBytes(sp.ReadLine());
                            WeightStr2 = ProtocolParseHelper.ProtocolParse(Weigh2ProtrcolType, uart2Buf);
                            break;

                        case "托利多":  //02(3A 30 20 20 20 20 20 20 30)20 20 20 20 20 30 0D C7 
                            if (sp.ReadByte() == 0x02)
                            {
                                while (sp.BytesToRead < 17) ;
                                sp.Read(uart2Buf, 0, 17);
                                WeightStr2 = ProtocolParseHelper.ProtocolParse(Weigh2ProtrcolType, uart2Buf);
                            }
                            break;

                        case "A27": //=84.1200
                            if (sp.ReadByte() == 0x3D)
                            {
                                while (sp.BytesToRead < 7) ;
                                sp.Read(uart2Buf, 0, 7);
                                WeightStr2 = ProtocolParseHelper.ProtocolParse(Weigh2ProtrcolType, uart2Buf);
                            }
                            break;

                        case "A12": //77(6E 30 30 31 39 2E 38 36 6B 67 0D 0A)
                            if (sp.ReadByte() == 0x77)
                            {
                                while (sp.BytesToRead < 12) ;
                                sp.Read(uart2Buf, 0, 12);
                                WeightStr2 = ProtocolParseHelper.ProtocolParse(Weigh2ProtrcolType, uart2Buf);
                            }
                            break;

                        default:
                            break;
                    }
                }

                ////2022-09-05 wanghu
                ////如果IsStable属性内部的定时器设定了阻止 称重串口继续接收数据，那么这里将阻止继续往下的逻辑。保持WeightStr的值不会改变，知道
                ////IsStable属性内部发生了改变，允许继续接收为止。
                //if (!SerialSleep)
                //{
                //    return;
                //}

                if (wstrl != "")
                {
                    //赋值完毕要讲 UI 信号传输 状态的 定时器 停止，后面赋值完毕后再次启动，
                    //并且此时 修改 UI界面 的信号传输为正常状态
                    weightStrChangeTimer.Stop();
                    if (StateLab != "信号传输正常")
                    {
                        if (System.Windows.Application.Current != null)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                StateBackground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#7ED321"));
                                StateLab = "信号传输正常";
                            });
                        }
                    }
                }

                if (wstrl != "")
                {
                    wstrl = (decimal.Parse(wstrl) * decimal.Parse(_mainSetting.Settings["EssentialNum"]?.Value ?? "0")).ToString("0.##");
                    // 新增自动转换单位逻辑  --阿吉 2023年6月12日16点40分
                    WeightStr1 = AutoConvertUnit(wstrl).ToString();
                    //如果小于最小重量，且当前状态为称重结束。那么就要关闭已经打开的收费窗口，因为这个时候车子已经走了
                    //收费窗口还在，会影响下一辆车的称重。
                    if (Math.Abs(Convert.ToDecimal(WeightStr1)) <= Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0") &&
                        CurrentStatus == Common.Model.Custom.WeighStatus.WeighEnd)
                    {
                        //标记为不保存逃费的称重数据
                        NoSave = true;

                        //关闭收费窗口
                        CloseDialog(false);
                    }
                }

                //当启用副设备时，解析副设备协议，并选择一个称重仪表重量进行操作
                if (HaveSecondDevice)
                {
                    if (SelectedDevice1)
                    {
                        WeightStr = WeightStr1;
                    }

                    if (SelectedDevice2)
                    {
                        WeightStr = WeightStr2;
                    }
                }
                else
                {
                    if (WeightStr1 == "")
                    {
                        return;
                    }

                    WeightStr = WeightStr1;
                }
            }
            finally
            {
                SerialPortListening = false;//我用完了，ui可以关闭串口了。
            }

            //赋值完毕要讲 UI 信号传输 状态的 定时器 启动，如果在定时器规定的时间内（2秒）
            //还没 执行weightStrChangeTimer.Stop()方法的话，那么UI 界面 信号传输就被修改为异常状态
            weightStrChangeTimer.Start();
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
            var autoConvertUnitVal = _mainSetting.Settings["AutoConvertUnit"].Value;
            if ("true".Equals(autoConvertUnitVal, StringComparison.OrdinalIgnoreCase))
            {
                var unitVal = _mainSetting.Settings["WeighingUnit"].Value;

                return "kg".Equals(unitVal, StringComparison.OrdinalIgnoreCase)
                    ? (number * 1000) : (number / 1000);
            }
            return number;
        }

        //二维码串口eventhandler
        private void QRPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (SerialPortClosing) return;
            try
            {
                SerialPortListening = true;//设置标记，说明我已经开始处理数据。

                //获取串口Hex数据，放到全局数组中
                SerialPort sp = (SerialPort)sender;

                if (sp.PortName.Equals(QRPort.PortName))
                {
                    var str = AWSV2.Services.Base64.DecryptBase64(sp.ReadLine());
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        QRString = str;
                    }
                    Globalspace._lprDevNo = "A";
                    OpenBarrier(Globalspace._lprDevNo);
                    IPCHelper.SendMsgToApp("ZXLPR", 0x08F0); //绿灯，道闸全落
                }
                if (sp.PortName.Equals(QRPort2.PortName))
                {
                    var str = AWSV2.Services.Base64.DecryptBase64(sp.ReadLine());
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        QRString = str;
                    }
                    Globalspace._lprDevNo = "B";
                    OpenBarrier(Globalspace._lprDevNo);
                    IPCHelper.SendMsgToApp("ZXLPR", 0x08F0); //绿灯，道闸
                }
                log.Debug($"uartrecv-> sp:{sp}, fomart -> QRString:{QRString}");
            }

            catch (Exception ex)
            {
                log.Debug(ex.Message);
            }
            finally
            {
                SerialPortListening = false;//我用完了，ui可以关闭串口了。
            }
        }

        //定时器eventhandler
        public void Timer_Tick(object sender, EventArgs e)
        {
            // 新加逻辑,  实时刷新按钮状态, 之前的相关逻辑一注释 --阿吉 2023年10月10日16点47分

            SetBtnTExt();


            //从全局变量中获取车牌识别APP传递过来的车牌号，并反馈硬件控制命令，打开自动复位计时器
            if (!string.IsNullOrEmpty(Globalspace._plateNo) && ReadyToWeigh)
            {
                if (!SQLDataAccess.CheckConnectionStatus())
                {
                    StatusBar = "数据库访问失败，请联系管理员！";
                    return;
                }

                var currCar = SQLDataAccess.LoadCar(Globalspace._plateNo);
                //2022-11-30 新增判断是否是非称重车辆。如果车辆备注为1的被认为是非称重车辆。否则是称重车辆。
                //如果是非称重车辆，则直接抬起A、B栏杆，放行。
                if (currCar != null && !string.IsNullOrWhiteSpace(currCar.Comment) && currCar.Comment.Trim() == "1")
                {
                    HandToBarrier("upall");
                    ReadyToWeigh = true;
                    Globalspace._plateNo = string.Empty;
                    return;
                }

                CurrentStatus = Common.Model.Custom.WeighStatus.ReadyWeigh;
                //LogContent.Clear();
                log.Debug("获取到车牌号：" + Globalspace._plateNo);

                //2022-11-03 注释，这里没有考虑到手动输入车牌的情况，所以移动至下面区域
                ////调用平台获取ICCARD数据
                //Common.SyncData.GetICCardData(Globalspace._plateNo);

                //2023-01-03再次变动了这块的需求。。。不再保存到ICCARD数据表里，而是把他注释掉。在下面逻辑段里直接获取
                //Common.SyncData.GetICCardData(Globalspace._plateNo);

                //ShowLogs($"检测到车牌：{Globalspace._plateNo} 正在上磅");
                //ShowLogs($"检测到车牌：{Globalspace._plateNo} 正在称重");
                //单项称重模式
                if (_mainSetting.Settings["Barrier"].Value == "1")
                {
                    ShowLogs($"检测到车辆：{Globalspace._plateNo}");
                }
                ReadyToWeigh = false;

                //查询车牌是否被禁用
                if (SQLDataAccess.LoadDisabledCar(Globalspace._plateNo) != null)
                {
                    log.Debug("识别到已禁用的车牌:" + Globalspace._plateNo);
                    ShowLogs($"识别到已禁用的车牌：{Globalspace._plateNo}，无法称重");

                    TTSHelper.TTS(weighingRecord, $"{Globalspace._plateNo}未经授权，无法称重");

                    if (_mainSetting.Settings["LED2Enable"].Value == "True")
                    {
                        ShowLedContent($"{weighingRecord.Ch}，未经授权，无法称重", "./Data/LedText/getplate.txt", weighingRecord);
                        IPCHelper.SendMsgToApp("ZXLED", IPCHelper.GET_PLATE);
                    }
                    lprMessage(Globalspace._plateNo, "未授权", "禁止上磅", DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"), IPCHelper.VIOLATE_WHITE_LIST);

                    ReadyToWeigh = true;
                    return;
                }

                //2022-11-23 新增。增加了 预约数据的判断。并且修改了之前的白名单逻辑。一旦开启预约判断，就不会再处理白名单逻辑。
                //判断是否开启称重完成就删除IC卡的开关状态。
                if ("true".Equals((_mainSetting.Settings["SyncYycz"]?.Value ?? string.Empty), StringComparison.OrdinalIgnoreCase))
                {
                    //2023-01-03新增此段，直接从平台获取ICCard数据。
                    //Common.Models.ICCardModel card = Common.SyncData.GetICCardData(Globalspace._plateNo);
                    Common.Models.WeighingRecordModel card = Common.SyncData.GetWXData(Globalspace._plateNo);
                    //2023-01-03 注释，此处需求变动为。直接获取平台的ICCard数据，不再从数据表里获取。
                    //Common.Models.ICCardModel card = Common.Data.SQLDataAccess.LoadICCard(Globalspace._plateNo, 1);
                    //没有预约的车辆
                    if (card == null)
                    {
                        log.Debug("识别到未预约车牌:" + Globalspace._plateNo);
                        weighingRecord.Ch = Globalspace._plateNo;
                        TTSHelper.TTS(weighingRecord, "%ch%未经授权，无法称重");

                        ShowLogs($"未预约车牌：{Globalspace._plateNo}，未经授权，无法称重");

                        if (_mainSetting.Settings["LED2Enable"].Value == "True")
                        {
                            ShowLedContent($"{weighingRecord.Ch}，未经授权，无法称重", "./Data/LedText/getplate.txt", weighingRecord);
                            IPCHelper.SendMsgToApp("ZXLED", IPCHelper.GET_PLATE);
                        }
                        ShowLogs($"未预约车牌：{Globalspace._plateNo}，禁止上磅");
                        //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.VIOLATE_WHITE_LIST);
                        lprMessage(Globalspace._plateNo, "未预约", "禁止上磅", DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"), IPCHelper.VIOLATE_WHITE_LIST);
                        ReadyToWeigh = true;
                        return;
                    }
                }
                // 如果没有开启预约，则查询车牌白名单
                else if (_mainSetting.Settings["LPRWhiteList"].Value == "1")
                {
                    if (currCar == null)
                    {
                        log.Debug("识别到非白名单中的车牌:" + Globalspace._plateNo);
                        weighingRecord.Ch = Globalspace._plateNo;
                        TTSHelper.TTS(weighingRecord, "%ch%未经授权，无法称重");

                        ShowLogs($"非白名单车牌：{Globalspace._plateNo}，未经授权，无法称重");

                        if (_mainSetting.Settings["LED2Enable"].Value == "True")
                        {
                            ShowLedContent($"{weighingRecord.Ch}，未经授权，无法称重", "./Data/LedText/getplate.txt", weighingRecord);
                            IPCHelper.SendMsgToApp("ZXLED", IPCHelper.GET_PLATE);
                        }
                        //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.VIOLATE_WHITE_LIST);
                        lprMessage(Globalspace._plateNo, "未授权", "禁止上磅", DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"), IPCHelper.VIOLATE_WHITE_LIST);
                        ReadyToWeigh = true;
                        return;
                    }
                }

                //排除扫到车尾拍照的情况
                if (Globalspace._plateNo == LastPlate)
                {
                    log.Debug("识别刚称重完成的车牌:" + Globalspace._plateNo);
                    ReadyToWeigh = true;
                    return;
                }

                //获取到车牌号信息后，通知APP把状态改为称重中
                if (_mainSetting.Settings["LPR"].Value != "0")
                {
                    //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IN_WEIGHING);
                    if (_mainSetting.Settings["Barrier"].Value == "1")//单向模式
                    {
                        lprMessage(Globalspace._plateNo, "正在称重", "居中停稳", "熄火下车", IPCHelper.GET_PLATE);
                    }
                    else
                    {
                        IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.GET_PLATE);
                    }
                }

                //设置车辆的lpr识别车牌
                weighingRecord.Ch = Globalspace._plateNo;

                //设置车辆的lpr抓拍图片
                SetImg("lpr", "", "");

                //2022-11-04 ICCard数据读取逻辑被移至下方。涉及到手动输入车牌也能读取的问题

                //控制道闸
                if (_mainSetting.Settings["Barrier"].Value == "2")
                {
                    //var hasJieGuanZha = (_mainSetting.Settings["JieGuanZha"]?.Value ?? "False") == "True";

                    if (Globalspace._lprDevNo == "A")
                    {
                        //IPCHelper.SendMsgToApp("ZXLPR", 0x08F1); //0000 -> 0001
                        //log.Debug("发送命令：A入道闸起");
                        //if (!hasJieGuanZha)
                        //{
                        //    IPCHelper.SendMsgToApp("ZXLPR", 0x08F1); //0000 -> 0001
                        //    log.Debug("发送命令：A入道闸起");
                        //    Thread.Sleep(500);
                        //    IPCHelper.SendMsgToApp("ZXLPR", 0X08FC);
                        //}
                        //else
                        //{
                        IPCHelper.SendMsgToApp("ZXLPR", 0x08F1); //0000 -> 0001
                        log.Debug("发送命令：A入道闸起");
                        //}
                    }
                    if (Globalspace._lprDevNo == "B")
                    {
                        ShowLogs($"道闸B起杆");
                        //if (!hasJieGuanZha)
                        //{
                        //    IPCHelper.SendMsgToApp("ZXLPR", 0x08F2); //0000 -> 0010
                        //    log.Debug("发送命令：B入道闸起");
                        //    Thread.Sleep(500);
                        //    IPCHelper.SendMsgToApp("ZXLPR", 0X08FC);
                        //}
                        //else
                        //{
                        IPCHelper.SendMsgToApp("ZXLPR", 0x08F2); //0000 -> 0010
                        log.Debug("发送命令：B入道闸起");
                        //}
                    }
                }

                //大屏幕显示请上秤
                if (_mainSetting.Settings["LED2Enable"].Value == "True")
                {
                    //因为 不能正常显示 请上称，原先的(WeightStr == "0") 逻辑无法包含 最小重量的逻辑，所以要改成根据最小重量判断
                    if (!string.IsNullOrWhiteSpace(WeightStr) &&
                        Math.Abs(Convert.ToDecimal(WeightStr)) <= Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0"))
                    {
                        var text = _mainSetting.Settings["Ledsbcps"].Value;
                        ShowLedContent(text, "./Data/LedText/getplate.txt", weighingRecord);
                    }
                    else
                    {
                        var text = _mainSetting.Settings["Ledzlwds"].Value;
                        ShowLedContent(text, "./Data/LedText/getplate.txt", weighingRecord);
                    }

                    IPCHelper.SendMsgToApp("ZXLED", IPCHelper.GET_PLATE);
                }

                //开启60秒取消称重倒计时
                var cancelTimeStr = _mainSetting.Settings["CancelWeighingTime"].Value;
                var cancelTime = int.Parse(string.IsNullOrWhiteSpace(cancelTimeStr) ? "60" : cancelTimeStr);

                cancelWeighingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(cancelTime) };
                cancelWeighingTimer.Start();
                cancelWeighingTimer.Tick += (s, args) =>
                {
                    //2022-12-02新增，在称重完成状态下，不执行60秒取消称重的逻辑
                    if (CurrentStatus != Common.Model.Custom.WeighStatus.WeighEnd)
                    {
                        if (cancelWeighingTimer != null) cancelWeighingTimer.Stop();
                        log.Debug("60秒未上秤，称重中状态取消");

                        //2022-12-02 注释，因为Globalspace._plateNo 在这个地方有可能被清空了。获取不到，所以换成下面的weighingRecord.Ch
                        //if (!string.IsNullOrWhiteSpace(Globalspace._plateNo))
                        //{
                        //    ShowLogs($"车牌：{Globalspace._plateNo} ，60秒未上称，取消称重");
                        //}

                        if (!string.IsNullOrWhiteSpace(weighingRecord.Ch))
                        {
                            ShowLogs($"车牌：{weighingRecord.Ch} ，60秒未上称，取消称重");
                        }

                        ReadyToWeigh = true;

                        if (_mainSetting.Settings["LPR"].Value != "0")
                        {
                            //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.READY_TO_WEIGH);
                            LPR_READY_TO_WEIGH();
                        }

                        if (_mainSetting.Settings["LED2Enable"].Value == "True")
                        {
                            var text = _mainSetting.Settings["Ledxsnr"].Value;
                            ShowLedContent(text, "./Data/LedText/readytoweigh.txt", weighingRecord);
                            IPCHelper.SendMsgToApp("ZXLED", IPCHelper.READY_TO_WEIGH);
                        }
                    }
                };

                //语音提示
                if (Convert.ToBoolean(_mainSetting.Settings["TTS0Enable"]?.Value ?? "False"))
                {
                    TTSHelper.TTS(weighingRecord, _mainSetting.Settings["TTS0Text"].Value);
                    log.Debug("语音提示上秤");
                }

                if (Globalspace._getPlateFrom == "QuickPlate")
                {
                    Globalspace._getPlateFrom = string.Empty;
                }

                Globalspace._plateNo = string.Empty;

                WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
            }

            if (Globalspace._getPlateFrom == "QuickPlate" && Globalspace._plateNo != weighingRecord.Ch)
            {
                weighingRecord.Ch = Globalspace._plateNo;
                WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
                Globalspace._getPlateFrom = string.Empty;
            }

            ////根据DI状态获取绑定的物资，并赋值给界面
            //if (!string.IsNullOrEmpty(Globalspace._diState) && Globalspace._diState != "0")
            //{
            //    var wzStr = ConfigurationManager.AppSettings[$"DI{Globalspace._diState}State"];
            //    if (!string.IsNullOrEmpty(wzStr))
            //    {
            //        weighingRecord.Wz = wzStr;
            //        WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
            //    }
            //}

            if (!string.IsNullOrEmpty(weighingRecord.Ch))
            {
                //2023-05-10 将此段从下面IF 段落迁移出来
                ICCardModel cardInfo = SQLDataAccess.LoadICCard(weighingRecord.Ch, 0);

                if (cardInfo != null) //数据库中查到此卡
                {
                    weighingRecord.ICCard = cardInfo.Id;
                    log.Debug("刷卡：" + cardInfo.Id);

                    if (cardInfo.CarAutoNo > 0)
                    {
                        var car = SQLDataAccess.LoadCar(cardInfo.CarAutoNo);
                        weighingRecord.Ch = car.PlateNo;
                    }
                    if (cardInfo.CustomerId > 0)
                    {
                        var customer = SQLDataAccess.LoadCustomer(cardInfo.CustomerId);
                        weighingRecord.Kh = customer.Name;
                    }
                    if (cardInfo.GoodsId > 0)
                    {
                        var goods = SQLDataAccess.LoadGoods(cardInfo.GoodsId);
                        weighingRecord.Wz = goods.Name;
                    }
                    if (cardInfo.SpecId > 0)
                    {
                        var goods = Common.Data.SQLDataAccess.GetGoodsSpec(cardInfo.SpecId);
                        weighingRecord.GoodsSpec = goods.Name;
                    }
                    if (cardInfo.By1 != null)
                    {
                        weighingRecord.By1 = cardInfo.By1;
                    }
                    if (cardInfo.By2 != null)
                    {
                        weighingRecord.By2 = cardInfo.By2;
                    }
                    if (cardInfo.By3 != null)
                    {
                        weighingRecord.By3 = cardInfo.By3;
                    }
                    if (cardInfo.By4 != null)
                    {
                        weighingRecord.By4 = cardInfo.By4;
                    }
                    if (cardInfo.By5 != null)
                    {
                        weighingRecord.By5 = cardInfo.By5;
                    }
                    if (cardInfo.Fhdw != null)
                    {
                        weighingRecord.Fhdw = cardInfo.Fhdw;
                    }

                }//数据库中查到此卡
            }

            //如果车牌不为空的情况下，就去找IC卡数据
            //读取IC卡数据
            if (!string.IsNullOrEmpty(weighingRecord.Ch) &&
                weighingRecord.Ch.Length >= 7 &&
                GetPlateNo != weighingRecord.Ch &&
                _mainSetting.Settings["SyncYycz"].Value == "True"
                )
            {
                //检测页面上输入的车号是否有变化，变化了则在 GetPlateNo 属性里进行一系列操作            
                GetPlateNo = weighingRecord.Ch;

                //2022-11-22 将这段移至上面开头处，不然白名单判断的时候，获取不到本地数据库里的车辆信息
                ////调用平台获取ICCARD数据
                //Common.SyncData.GetICCardData(weighingRecord.Ch);

                #region 虚拟IC卡绑定车牌、客户、物资信息
                //2023-01-03新增此段，直接从平台获取ICCard数据。
                //Common.Models.ICCardModel card = Common.SyncData.GetICCardData(weighingRecord.Ch);

                //2023-05-10 将下面这段IF 注释，因为跟预约数据混淆，公用一个Yycz开关，导致无法跳过开关执行本地IC数据，故而
                //将此段注释，移出开关控制范围。
                //ICCardModel cardInfo = SQLDataAccess.LoadICCard(weighingRecord.Ch, 0);

                //if (cardInfo != null) //数据库中查到此卡
                //{
                //    weighingRecord.ICCard = cardInfo.Id;
                //    log.Debug("刷卡：" + cardInfo.Id);

                //    if (cardInfo.CarAutoNo > 0)
                //    {
                //        var car = SQLDataAccess.LoadCar(cardInfo.CarAutoNo);
                //        weighingRecord.Ch = car.PlateNo;
                //    }
                //    if (cardInfo.CustomerId > 0)
                //    {
                //        var customer = SQLDataAccess.LoadCustomer(cardInfo.CustomerId);
                //        weighingRecord.Kh = customer.Name;
                //    }
                //    if (cardInfo.GoodsId > 0)
                //    {
                //        var goods = SQLDataAccess.LoadGoods(cardInfo.GoodsId);
                //        weighingRecord.Wz = goods.Name;
                //    }
                //    if (cardInfo.SpecId > 0)
                //    {
                //        var goods = Common.Data.SQLDataAccess.GetGoodsSpec(cardInfo.SpecId);
                //        weighingRecord.GoodsSpec = goods.Name;
                //    }
                //    if (cardInfo.By1 != null)
                //    {
                //        weighingRecord.By1 = cardInfo.By1;
                //    }
                //    if (cardInfo.By2 != null)
                //    {
                //        weighingRecord.By2 = cardInfo.By2;
                //    }
                //    if (cardInfo.By3 != null)
                //    {
                //        weighingRecord.By3 = cardInfo.By3;
                //    }
                //    if (cardInfo.By4 != null)
                //    {
                //        weighingRecord.By4 = cardInfo.By4;
                //    }
                //    if (cardInfo.By5 != null)
                //    {
                //        weighingRecord.By5 = cardInfo.By5;
                //    }
                //    if (cardInfo.Fhdw != null)
                //    {
                //        weighingRecord.Fhdw = cardInfo.Fhdw;
                //    }

                //}//数据库中查到此卡
                //else
                {
                    Common.Models.WeighingRecordModel card = Common.SyncData.GetWXData(weighingRecord.Ch);
                    //Common.Models.ICCardModel card = Common.SyncData.GetICCardData(Globalspace._plateNo);
                    //2023-01-03注释此段
                    //Common.Models.ICCardModel card = Common.Data.SQLDataAccess.LoadICCard(weighingRecord.Ch, 1);
                    if (card != null) //数据库中查到此卡
                    {
                        if (!string.IsNullOrEmpty(card.By1))
                            weighingRecord.By1 = card.By1;
                        if (!string.IsNullOrEmpty(card.By2))
                            weighingRecord.By2 = card.By2;
                        if (!string.IsNullOrEmpty(card.By3))
                            weighingRecord.By3 = card.By3;
                        if (!string.IsNullOrEmpty(card.By4))
                            weighingRecord.By4 = card.By4;
                        if (!string.IsNullOrEmpty(card.By5))
                            weighingRecord.By5 = card.By5;
                        if (!string.IsNullOrEmpty(card.By6))
                            weighingRecord.By6 = card.By6;
                        if (!string.IsNullOrEmpty(card.By7))
                            weighingRecord.By7 = card.By7;
                        if (!string.IsNullOrEmpty(card.By8))
                            weighingRecord.By8 = card.By8;
                        if (!string.IsNullOrEmpty(card.Fhdw))
                            weighingRecord.Fhdw = card.Fhdw;
                        if (!string.IsNullOrEmpty(card.Driver))
                            weighingRecord.Driver = card.Driver;
                        if (!string.IsNullOrEmpty(card.DriverPhone))
                            weighingRecord.DriverPhone = card.DriverPhone;
                        if (!string.IsNullOrEmpty(card.AxleNum))
                            weighingRecord.AxleNum = card.AxleNum;
                        if (!string.IsNullOrEmpty(card.Gblx))
                            weighingRecord.Gblx = card.Gblx;
                        if (!string.IsNullOrEmpty(card.Kh))
                            weighingRecord.Kh = card.Kh;
                        if (!string.IsNullOrEmpty(card.Kh2))
                            weighingRecord.Kh2 = card.Kh2;
                        if (!string.IsNullOrEmpty(card.Kh3))
                            weighingRecord.Kh3 = card.Kh3;
                        if (!string.IsNullOrEmpty(card.Wz))
                            weighingRecord.Wz = card.Wz;
                        if (!string.IsNullOrEmpty(card.Bz))
                            weighingRecord.Bz = card.Bz;

                    }//数据库中未查到此卡
                    //else  //2023-04-25将这段注释，因为混在预约逻辑一起了。将这段逻辑上移到预约逻辑前面
                    //{
                    //    //如果启用了白名单模式
                    //    if (ConfigurationManager.AppSettings["LPRWhiteList"] == "1")
                    //    {
                    //        log.Debug("识别到非预约的车牌:" + Globalspace._plateNo);
                    //        weighingRecord.Ch = Globalspace._plateNo;
                    //        TTSHelper.TTS(weighingRecord, "%ch%未经授权，无法称重");

                    //        ShowLogs($"非预约车牌：{Globalspace._plateNo}，未经授权，无法称重");

                    //        if (ConfigurationManager.AppSettings["LED2Enable"] == "True")
                    //        {
                    //            ShowLedContent($"{weighingRecord.Ch}，未经授权，无法称重", "./Data/LedText/getplate.txt", weighingRecord);
                    //            IPCHelper.SendMsgToApp("ZXLED", IPCHelper.GET_PLATE);
                    //        }
                    //        IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.VIOLATE_WHITE_LIST);
                    //        ReadyToWeigh = true;
                    //        return;
                    //    }
                    //}
                }
                #endregion
            }
            else
            {
                //检测页面上输入的车号是否有变化，变化了则在 GetPlateNo 属性里进行一系列操作            
                GetPlateNo = weighingRecord.Ch;
            }

            //检测光栅是否被遮挡，并且根据第几次称重 去设定相关按钮的文本
            //if (ConfigurationManager.AppSettings["CheckGrating"].Equals("1") && Globalspace._signoState != "3")
            //System.Diagnostics.Trace.WriteLine($"Globalspace._ajGPIOState :{Globalspace._ajGPIOState}");
            //if (_mainSetting.Settings["CheckGrating"].Value.Equals("1"))
            //{
            //    //此处修改为 动态显示 光栅提示在称重按钮文本内容里
            //    SetBtnTExt(Globalspace._ajGPIOState);
            //}
            //ShowLogs(Globalspace._signoState,true);

            //检测仪表重量，[当前选择的称重仪表]重量小于最小称重重量后可以进行下一次称重
            try
            {
                if (Math.Abs(WeightStr.TryGetDecimal()) <= _mainSetting.Settings["MinSlotWeight"]?.Value.TryGetDecimal())
                {
                    ////如果小于最小重量，且当前状态为称重结束。那么就要关闭已经打开的收费窗口，因为这个时候车子已经走了
                    ////收费窗口还在，会影响下一辆车的称重。
                    //if (CurrentStatus == Common.Model.Custom.WeighStatus.WeighEnd)
                    //{
                    //    ShowLogs($"下磅完成，关闭收费对话框。");
                    //    //关闭收费窗口
                    //    CloseDialog();
                    //}

                    //防作弊光栅TTS语音提示
                    //检测光栅是否被遮挡
                    //if (_mainSetting.Settings["CheckGrating"].Value.Equals("1") && string.IsNullOrWhiteSpace(weighingRecord.Ch))
                    //{
                    //    if (Globalspace._signoState != "3")
                    //    {
                    //        IsMinsoftCover = false;
                    //    }

                    //    SetBtnTExt(Globalspace._ajGPIOState);

                    //    //2023-01-09再次注释
                    //    //if (!minSoftTTSTimer.IsEnabled)
                    //    //{
                    //    //    normalTTSTimer.Stop();//需要先停止正常称重的定时器的TTS语音的播放动作
                    //    //    // TTSHelper.TTS(weighingRecord, "红外遮挡");
                    //    //    minSoftTTSTimer.Start();
                    //    //}
                    //}

                    //由非0变为0，绿灯，道闸全落
                    if (NoZeroFlag)
                    {
                        if ((WeighingTimes == 0) || (WeighingTimes != 0 && IsDelayStable))
                        {
                            //在切换为准备好称重状态之前拿到最后一次的车牌号
                            if (weighingRecord.Ch != null)
                            {
                                LastPlate = weighingRecord.Ch;
                                log.Debug("最后一次称重车牌是:" + LastPlate);

                                var lastTimeStr = _mainSetting.Settings["LastPlateTimer"].Value;
                                var lastTime = int.Parse(string.IsNullOrWhiteSpace(lastTimeStr) ? "60" : lastTimeStr);

                                var lastPlateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(lastTime) };
                                lastPlateTimer.Start();
                                lastPlateTimer.Tick += (s, args) =>
                                {
                                    lastPlateTimer.Stop();
                                    log.Debug("清空最后一次称重车牌:" + LastPlate);
                                    LastPlate = string.Empty;
                                };
                            }

                            NoZeroFlag = false;
                            ReadyToWeigh = false;
                            //ReadyToWeigh = true;

                            //if (ConfigurationManager.AppSettings["LPR"] != "0")
                            //if (ConfigurationManager.AppSettings["LightSwitch"] == "0")//2022-09-01因为不能正常 亮灯和 道闸执行，所以注释


                            //if (ConfigurationManager.AppSettings["LightType"] == "1")
                            //{
                            //    IPCHelper.SendMsgToApp("ZXLPR", 0x08F0); //绿灯，道闸全落
                            //    log.Debug("发送命令：绿灯，道闸全落");
                            //}
                            //else
                            //{
                            //    IPCHelper.SendMsgToApp("ZXLPR", 0x08FC); //红灯，道闸全落
                            //    log.Debug("发送命令：红灯，道闸全落");
                            //}

                            LightByType("1");

                            if (_mainSetting.Settings["LPR"].Value != "0")
                            {
                                // IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.READY_TO_WEIGH);
                                lprMessage("", "", "", "", IPCHelper.READY_TO_WEIGH);
                            }
                            if (_mainSetting.Settings["LED2Enable"].Value == "True")
                            {
                                var text = _mainSetting.Settings["Ledxsnr"].Value;
                                ShowLedContent(text, "./Data/LedText/readytoweigh.txt", weighingRecord);
                                IPCHelper.SendMsgToApp("ZXLED", IPCHelper.READY_TO_WEIGH);
                            }

                            if (CurrentStatus == Common.Model.Custom.WeighStatus.WeighEnd)
                            {
                                ShowLogs($"下磅完成，刷新磅单，称重准备就绪");
                                //延迟刷新表单，不然看不见 下磅的提示那些
                                var purifier = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(2000) };
                                purifier.Start();
                                purifier.Tick += (s, args) =>
                                {
                                    purifier.Stop();
                                    //在这里刷新，不然weighingRecord 为null
                                    RefreshWeighForm();
                                };
                            }
                            else
                            {
                                RefreshWeighForm();
                            }
                            CurrentStatus = Common.Model.Custom.WeighStatus.Waiting;
                        }

                        if ((LogContent.Any(p => p.Contains("正在上磅")) && !LogContent.Any(p => p.Contains("正在下磅")) &&
                             CurrentStatus != Common.Model.Custom.WeighStatus.WeighEnd))
                        {

                            if (!string.IsNullOrEmpty(CurrentPlate))
                            {
                                //清空当前车牌
                                CurrentPlate = string.Empty;
                                //停止录像
                                IPCHelper.SendMsgToApp("ZXMonitor", "EndWeighing:" + weighingRecord.Ch + ":" + weighingRecord.Bh);
                            }

                            //////在这里刷新，不然weighingRecord 为null
                            //RefreshWeighForm();

                            ////debug
                            // log.Info($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}发生一次刷新磅单 CurrentStatus:{CurrentStatus}");
                        }

                        //// 2023-02-28 新增，将上面录像部分代码注释，逻辑下移至此处
                        //if (CurrentStatus == Common.Model.Custom.WeighStatus.WeighEnd)
                        //{
                        //    //增加上车录像功能
                        //    if (Convert.ToBoolean(ConfigurationManager.AppSettings["MonitorEnable"]) && !string.IsNullOrEmpty(weighingRecord.Ch))
                        //    {
                        //        IPCHelper.SendMsgToApp("ZXMonitor", "EndWeighing:" + weighingRecord.Ch + ":" + weighingRecord.Bh);
                        //        log.Debug("发送命令：停止录像");
                        //    }
                        //}

                    }
                }
                else
                {

                    //检测光栅是否被遮挡
                    //if (_mainSetting.Settings["CheckGrating"].Value.Equals("1"))
                    //{
                    //    if (Globalspace._signoState != "3")
                    //    {
                    //        IsMinsoftCover = false;
                    //    }
                    //    SetBtnTExt(Globalspace._ajGPIOState);

                    //    //2023-01-09再次注释
                    //    //minSoftTTSTimer.Stop();//需要先停止小于最小重量的定时器的TTS语音的播放动作
                    //    //if (!normalTTSTimer.IsEnabled)
                    //    //{
                    //    //    normalTTSTimer.Start();
                    //    //}

                    //    //2023-01-03 注释，因为此处播放过于频繁。所以加定时器延迟播放
                    //    //TTSHelper.TTS(weighingRecord, "红外遮挡、请停到秤台中间");
                    //}


                    // 2023-02-28 新增,修改上车录像功能
                    if (Convert.ToBoolean(_mainSetting.Settings["MonitorEnable"]?.Value ?? "False") &&
                        Convert.ToBoolean(Common.Utility.SettingsHelper.ZXMonitorSettings.Settings["MonitorCaptureEnable"].Value) &&
                        !string.IsNullOrEmpty(weighingRecord.Ch) &&
                        CurrentPlate != weighingRecord.Ch)
                    {
                        CurrentPlate = weighingRecord.Ch;
                        IPCHelper.SendMsgToApp("ZXMonitor", "StartWeighing:" + weighingRecord.Ch + ":" + weighingRecord.Bh);
                        log.Debug("发送命令：开始录像");
                    }



                    //单项称重模式
                    if (_mainSetting.Settings["Barrier"].Value == "1")
                    {
                        ShowLogs($"检测到车辆，正在上磅");
                        if (!NoZeroFlag)
                        {

                            //当检测到有重量，但是没有车牌的时候。需要通过监控相机的LED做出提示
                            if (string.IsNullOrWhiteSpace(weighingRecord.Ch))
                            {
                                //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IS_WEIGHING);
                                lprMessage("缓慢通行", "正在过磅", "正在过磅", "正在过磅", IPCHelper.IS_WEIGHING, "r");
                            }
                            else
                            {
                                lprMessage($"{weighingRecord.Ch}", "正在称重", "居中停稳", "熄火下车", IPCHelper.GET_PLATE);
                            }

                            IPCHelper.SendMsgToApp("ZXLPR", 0x08FC); //红灯，道闸全落


                            if (_mainSetting.Settings["LED2Enable"].Value == "True")
                            {
                                using (StreamWriter sw = new StreamWriter("./Data/LedText/isweighing.txt", false, Encoding.Default))
                                {
                                    if (weighingRecord.Ch != null) sw.WriteLine(weighingRecord.Ch);
                                    sw.Write("正在称重中");
                                }
                                IPCHelper.SendMsgToApp("ZXLED", IPCHelper.IS_WEIGHING);
                            }
                            NoZeroFlag = true;
                            CurrentStatus = Common.Model.Custom.WeighStatus.Weighing;
                        }
                    }
                    else//双向称重或其他
                    {
                        if (string.IsNullOrWhiteSpace(weighingRecord.Ch))
                        {
                            ShowLogs($"没有识别到车牌请手动输入车牌");
                        }
                        else
                        {
                            ShowLogs($"检测到车牌：{weighingRecord.Ch} 正在上磅");
                        }

                        if (!NoZeroFlag)
                        {
                            if (!string.IsNullOrWhiteSpace(weighingRecord.Ch))
                            {
                                ShowLogs($"检测到车牌：{weighingRecord.Ch} 正在称重");
                                lprMessage($"{weighingRecord.Ch}", "正在称重", "居中停稳", "熄火下车", IPCHelper.IS_WEIGHING, "r");
                            }
                            else
                            {
                                lprMessage($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", "正在称重", "居中停稳", "熄火下车", IPCHelper.IS_WEIGHING, "r");
                            }

                            //if (ConfigurationManager.AppSettings["LPR"] != "0" && !NoZeroFlag)
                            //if (ConfigurationManager.AppSettings["LightSwitch"] != "1" && !NoZeroFlag)//2022-09-01因为不能正常 亮灯和 道闸执行，所以注释
                            if (!NoZeroFlag)
                            {
                                IPCHelper.SendMsgToApp("ZXLPR", 0x08FC); //红灯，道闸全落
                                log.Debug("发送命令：红灯，道闸全落");
                                //var signoState = ConfigurationManager.AppSettings["CheckGrating"].Equals("1")? Globalspace._signoState != "3" ? "，光栅未遮挡" : $"，光栅 {Globalspace._signoState} 遮挡":string.Empty;
                                //ShowLogs($"道闸全落{signoState}");
                                if (!string.IsNullOrWhiteSpace(weighingRecord.Ch))
                                {
                                    ShowLogs($"道闸全落");
                                }
                            }

                            //双道闸时，大屏幕由【车号请上秤】改为【车号正在称重中】
                            if (_mainSetting.Settings["LED2Enable"].Value == "True" &&
                                _mainSetting.Settings["Barrier"].Value == "2" && !NoZeroFlag)
                            {
                                using (StreamWriter sw = new StreamWriter("./Data/LedText/isweighing.txt", false, Encoding.Default))
                                {
                                    if (weighingRecord.Ch != null) sw.WriteLine(weighingRecord.Ch);
                                    sw.Write("正在称重中");
                                }
                                IPCHelper.SendMsgToApp("ZXLED", IPCHelper.IS_WEIGHING);
                            }
                            if (_mainSetting.Settings["Barrier"].Value == "2" && !NoZeroFlag)
                            {
                                // IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IS_WEIGHING);
                                lprMessage("", "", "", "", IPCHelper.IS_WEIGHING, "o");
                            }

                            ////2023 - 02 - 28 注释，此功能新增了单独设置录像的开关，以及增加了1、2号摄像头的同时摄像功能。
                            ////增加上车录像功能
                            //// if (Convert.ToBoolean(ConfigurationManager.AppSettings["MonitorEnable"]) && !NoZeroFlag)
                            //if (Convert.ToBoolean(ConfigurationManager.AppSettings["MonitorEnable"]) && !string.IsNullOrEmpty(weighingRecord.Ch))
                            ////if (Convert.ToBoolean(ConfigurationManager.AppSettings["MonitorEnable"]))
                            //{

                            //    IPCHelper.SendMsgToApp("ZXMonitor", "StartWeighing:" + weighingRecord.Ch + ":" + weighingRecord.Bh);
                            //    log.Debug("发送命令：开始录像");
                            //}

                            NoZeroFlag = true;
                            CurrentStatus = Common.Model.Custom.WeighStatus.Weighing;
                        }
                        //else
                        //{
                        //    if (!string.IsNullOrWhiteSpace(weighingRecord.Ch))
                        //    {
                        //        ShowLogs($"检测到车牌：{weighingRecord.Ch} 正在称重");
                        //        lprMessage($"{weighingRecord.Ch}", "正在称重", "居中停稳", "熄火下车", IPCHelper.IS_WEIGHING, "r");
                        //    }
                        //    else
                        //    {
                        //        lprMessage($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", "正在称重", "居中停稳", "熄火下车", IPCHelper.IS_WEIGHING, "r");
                        //    }
                        //}
                    }


                }

            }
            catch (Exception exception) { }

            //大屏幕串口发送数据
            if (LEDSerialPortEnable)
            {
                DataConvertHelper.ConvertWeightToLedHex(WeightStr, ref hexWeight);

                LEDSerialPort.Write(hexWeight, 0, hexWeight.Length);
            }

            //轮询桌面读卡器
            if (Globalspace._icdev.ToInt32() > 0 && !IsICBusy)
            {
                //ulong icCardNo = 0;
                byte[] snr = new byte[5];

                //读取到射频卡
                try
                {
                    var ret = ICCard.rf_card(Globalspace._icdev, 1, snr);
                    if (0 == ret && Convert.ToDecimal(WeightStr) > Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0"))
                    {
                        uint uisnr = BitConverter.ToUInt32(snr, 0);
                        ICCardModel card = SQLDataAccess.LoadICCard(uisnr.ToString());
                        ICCard.rf_beep(Globalspace._icdev, 10);

                        if (card != null) //数据库中查到此卡
                        {
                            weighingRecord.ICCard = card.Id;
                            log.Debug("刷卡：" + card.Id);

                            if (card.CarAutoNo > 0)
                            {
                                var car = SQLDataAccess.LoadCar(card.CarAutoNo);
                                weighingRecord.Ch = car.PlateNo;
                            }
                            if (card.CustomerId > 0)
                            {
                                var customer = SQLDataAccess.LoadCustomer(card.CustomerId);
                                weighingRecord.Kh = customer.Name;
                            }
                            if (card.GoodsId > 0)
                            {
                                var goods = SQLDataAccess.LoadGoods(card.GoodsId);
                                weighingRecord.Wz = goods.Name;
                            }
                            if (card.By1 != null)
                            {
                                weighingRecord.By1 = card.By1;
                            }
                            if (card.By2 != null)
                            {
                                weighingRecord.By2 = card.By2;
                            }
                            if (card.By3 != null)
                            {
                                weighingRecord.By3 = card.By3;
                            }
                            if (card.By4 != null)
                            {
                                weighingRecord.By4 = card.By4;
                            }
                            if (card.By5 != null)
                            {
                                weighingRecord.By5 = card.By5;
                            }


                            if (card.By6 != null)
                            {
                                weighingRecord.By6 = card.By6;
                            }
                            if (card.By7 != null)
                            {
                                weighingRecord.By7 = card.By7;
                            }
                            if (card.By8 != null)
                            {
                                weighingRecord.By8 = card.By8;
                            }
                            if (card.By9 != null)
                            {
                                weighingRecord.By9 = card.By9;
                            }
                            if (card.By10 != null)
                            {
                                weighingRecord.By10 = card.By10;
                            }
                            if (card.By11 != null)
                            {
                                weighingRecord.By11 = card.By11;
                            }
                            if (card.By12 != null)
                            {
                                weighingRecord.By12 = card.By12;
                            }
                            if (card.By13 != null)
                            {
                                weighingRecord.By13 = card.By13;
                            }
                            if (card.By14 != null)
                            {
                                weighingRecord.By14 = card.By14;
                            }
                            if (card.By15 != null)
                            {
                                weighingRecord.By15 = card.By15;
                            }
                            if (card.By16 != null)
                            {
                                weighingRecord.By16 = card.By16;
                            }
                            if (card.By17 != null)
                            {
                                weighingRecord.By17 = card.By17;
                            }
                            if (card.By18 != null)
                            {
                                weighingRecord.By18 = card.By18;
                            }
                            if (card.By19 != null)
                            {
                                weighingRecord.By19 = card.By19;
                            }
                            if (card.By20 != null)
                            {
                                weighingRecord.By20 = card.By20;
                            }


                            //显示到页面
                            WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);

                            IsICBusy = true;


                            //检测光栅是否被遮挡
                            //if (_mainSetting.Settings["CheckGrating"].Value.Equals("1"))
                            //{
                            //    if (Globalspace._signoState != "3")
                            //    {
                            //        return;
                            //    }
                            //    SetBtnTExt(Globalspace._ajGPIOState);


                            //}


                            if (TwiceWeighing)
                            {
                                if (WeighingTimes == 1)
                                {
                                    FirstWeighing();
                                }
                                if (WeighingTimes == 2)
                                {
                                    SecondWeighing();
                                }
                            }
                            else
                            {
                                if (WeighingTimes == 1)
                                {
                                    OnceWeighing();
                                }
                            }
                        }//数据库中查到此卡
                    }
                }
                catch (Exception ex)
                {
                    log.Debug(ex.Message);
                }
            }

            //轮询二维码扫码器
            if (!string.IsNullOrWhiteSpace(QRString))
            {
                try
                {
                    // 新逻辑 --阿吉 2023年10月8日17点41分
                    //直接赋值显示
                    var resObj = JsonConvert.DeserializeObject<JObject>(QRString);

                    resObj.TryGetValue("car_plate", out var ch);
                    weighingRecord.Ch = ch.ToString();

                    resObj.TryGetValue("formv2_user_input", out var inputObj);
                    if (inputObj != null)
                    {
                        weighingRecord.Wz = inputObj["_wz"]?.ToString();

                        //weighingRecord.dh = inputObj["_dh"]?.ToString();

                        weighingRecord.Kh = inputObj["_kh"]?.ToString();
                    }


                    // 以下旧逻辑 --阿吉 2023年10月8日17点41分
                    //if (!String.IsNullOrWhiteSpace(weighingRecord.Ch))
                    //{
                    //    if ((weighingRecord.Ch.Length >= 5 &&
                    //        qrdata.Ch.Length >= 5) &&
                    //        weighingRecord.Ch.Substring(0, 5) == qrdata.Ch.Substring(0, 5))
                    //    {
                    //        CopyValues(weighingRecord, qrdata);
                    //    }
                    //    else
                    //    {
                    //        var text = $"%ch%:扫码结果和识别结果不一致";
                    //        if (_mainSetting.Settings["LED2Enable"].Value == "True")
                    //        {
                    //            ShowLedContent(text, "./Data/LedText/getplate.txt", weighingRecord);
                    //            IPCHelper.SendMsgToApp("ZXLED", IPCHelper.GET_PLATE);
                    //        }
                    //        if (Convert.ToBoolean(_mainSetting.Settings["TTS3Enable"]?.Value ?? "False"))
                    //        {
                    //            TTSHelper.TTS(weighingRecord, text);
                    //        }
                    //        return;//一旦不一致，就不允许称重。
                    //    }
                    //}
                }
                catch { }

                Globalspace._plateNo = weighingRecord.Ch; //为了触发ReadyToWeigh = false
                WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);

                QRString = string.Empty;

                //检测光栅是否被遮挡
                //if (_mainSetting.Settings["CheckGrating"].Value.Equals("1"))
                //{
                //    if (Globalspace._signoState != "3")
                //    {
                //        ShowLogs("检测到光栅被遮挡，无法称重", true);
                //        CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;
                //        return;
                //    }
                //    SetBtnTExt(Globalspace._ajGPIOState);

                //}

                if (TwiceWeighing)
                {
                    if (WeighingTimes == 1)
                    {
                        FirstWeighing();
                    }
                    if (WeighingTimes == 2)
                    {
                        SecondWeighing();
                    }
                }
                else
                {
                    if (WeighingTimes == 1)
                    {
                        OnceWeighing();
                    }
                }
            }

            //查询轮轴识别
            if (Globalspace._axleNo != "0")
            {
                //weighingRecord.By2 = Globalspace._axleNo;
                weighingRecord.AxleNum = Globalspace._axleNo;
                Globalspace._axleNo = "0";
                WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
            }

            //检测光栅是否被遮挡
            //if (_mainSetting.Settings["CheckGrating"].Value.Equals("1"))
            //{
            //    if (Globalspace._signoState != "3")
            //    {
            //        ShowLogs("检测到光栅被遮挡，无法称重", true);
            //        CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;
            //        return;
            //    }
            //    SetBtnTExt(Globalspace._ajGPIOState);

            //}

            //自动称重 延时非零稳定后执行
            if (!ManualWeighing && EnableWeighing && IsDelayNZStable
                && !(_isCheckGrating && _cameraGPIOCovered))
            {
                ////第三方接口，请求是否可以称重（下磅接口） --阿吉 2023年6月15日16点28分
                //var platform = PlatformManager.Instance.Current as Shandong_Boxing_Platform;
                //if (platform != null)
                //{
                //    var platNo = Globalspace._plateNo;
                //    platform
                //        .Finish(platNo, Globalspace._lprDevNo,
                //        weighingRecord.Weigh,
                //        (msg) =>
                //        {
                //            ShowLogs($"车牌：{platNo} 禁止下磅：{msg}", false);
                //        });
                //}
                if (TwiceWeighing)
                {
                    if (WeighingTimes == 1)
                    {
                        FirstWeighing();
                    }
                    if (WeighingTimes == 2)
                    {
                        SecondWeighing();
                    }
                }
                else
                {
                    if (WeighingTimes == 1)
                    {
                        OnceWeighing();
                    }
                }

            }

            ////按钮称重
            //if (Globalspace._gpioState == "0" && !OverWeight)
            //{
            //    if (ManualWeighing && EnableWeighing)
            //    {
            //        //检测光栅是否被遮挡
            //        if (ConfigurationManager.AppSettings["CheckGrating"].Equals("1") && Globalspace._signoState != "3")
            //        {
            //            ShowLogs("检测到光栅被遮挡，无法称重", true);
            //            CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;
            //            return;
            //        }

            //        if (TwiceWeighing)
            //        {
            //            if (WeighingTimes == 1 && CanFirstWeighing)
            //            {
            //                FirstWeighing();
            //            }
            //            if (WeighingTimes == 2 && CanSecondWeighing)
            //            {
            //                SecondWeighing();
            //            }
            //        }
            //        else
            //        {
            //            if (WeighingTimes == 1 && CanOnceWeighing)
            //            {
            //                OnceWeighing();
            //            }
            //        }
            //        Globalspace._gpioState = "1";
            //    }
            //}


            //按钮称重
            if (_mainSetting.Settings["WeighingControl"].Value.Equals("Btn") &&
                !string.IsNullOrEmpty(Globalspace._diState) && Globalspace._diState != "0" &&
                !string.IsNullOrEmpty(weighingRecord.Ch) && !OverWeight)
            {
                //if (ManualWeighing && EnableWeighing&& CurrentStatus!= Common.Model.Custom.WeighStatus.WeighEnd)
                if (ManualWeighing && EnableWeighing)
                {
                    //检测光栅是否被遮挡
                    //if (_mainSetting.Settings["CheckGrating"].Value.Equals("1"))
                    //{
                    //    if (Globalspace._signoState != "3")
                    //    {
                    //        ShowLogs("检测到光栅被遮挡，无法称重", true);
                    //        CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;
                    //        return;
                    //    }
                    //    SetBtnTExt(Globalspace._ajGPIOState);

                    //}

                    var wzStr = _mainSetting.Settings[$"DI{Globalspace._diState}State"]?.Value ?? string.Empty;
                    if (!string.IsNullOrEmpty(wzStr))
                    {
                        weighingRecord.Wz = wzStr;
                        //WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
                    }

                    if (TwiceWeighing)
                    {
                        if (WeighingTimes == 1 && CanFirstWeighing)
                        {
                            FirstWeighing();
                        }
                        if (WeighingTimes == 2 && CanSecondWeighing)
                        {
                            SecondWeighing();
                        }
                    }
                    else
                    {
                        if (WeighingTimes == 1 && CanOnceWeighing)
                        {
                            OnceWeighing();
                        }
                    }
                    Globalspace._gpioState = "1";
                }
            }
        }

        /// <summary>
        /// 根据LightType 0：称重完成亮绿灯 ，1：车下磅后绿灯 发送红绿灯指令给LPR
        /// </summary>
        private void LPR_READY_TO_WEIGH()
        {
            //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.READY_TO_WEIGH);
            lprMessage("", "", "", "", IPCHelper.READY_TO_WEIGH);
            LightByType();
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
                log.Debug($"JA Debug Before LightType,WeightStr1:{WeightStr1}，type:{type}");
                if (type == "1" && WeightStr1.TryGetDecimal() > limit)
                {
                    IPCHelper.SendMsgToApp("ZXLPR", 0x08FC); //红灯，道闸全落
                    log.Debug("发送命令：红灯，道闸全落");
                    log.Debug($"JA Debug LightType 红灯亮,WeightStr1:{WeightStr1}，type:{type}");
                    return;
                }

                IPCHelper.SendMsgToApp("ZXLPR", 0x08F0); //绿灯，道闸全落
                log.Debug("JA Debug LightType 绿灯亮");

                log.Debug("发送命令：绿灯，道闸全落");
            }
            //else
            //{
            //    IPCHelper.SendMsgToApp("ZXLPR", 0x08FC); //红灯，道闸全落
            //    log.Debug("发送命令：红灯，道闸全落");
            //}
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

                if (Globalspace._isRegister)
                {
                    if (!LogContent.Any(p => p == content) || allowRepeat)
                    {
                        LogContent.Add(content);
                    }
                }
                else
                {
                    LogContent.Add($"试用期已过请联系服务商 {Globalspace.Provicer.mobile}");
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

        private void ShowLedContent(string text, string filePah, WeighingRecordModel record)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(record.Ch))
                {
                    text = TTSHelper.Replace(record, text);
                }

                var list = text.Split('，');
                using (StreamWriter sw = new StreamWriter(filePah, false, Encoding.Default))
                {
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(list[i])) continue;

                        if ((i + 1) >= list.Length)
                        {
                            sw.Write(list[i]);
                        }
                        else
                        {
                            sw.WriteLine(list[i]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }

        }

        //ThisPageButtonStyle ThisPageYellowButtonStyle
        System.Windows.Style _Btn1Style = (System.Windows.Style)Application.Current.Resources["ThisPageButtonStyle"];
        public System.Windows.Style Btn1Style
        {
            get
            {
                return _Btn1Style;
            }
            set
            {
                _Btn1Style = value;
            }
        }

        //ThisPageButtonStyle ThisPageYellowButtonStyle
        System.Windows.Style _Btn2Style = (System.Windows.Style)Application.Current.Resources["ThisPageButtonStyle"];
        public System.Windows.Style Btn2Style
        {
            get
            {
                return _Btn2Style;
            }
            set
            {
                _Btn2Style = value;
            }
        }

        //ThisPageButtonStyle ThisPageYellowButtonStyle
        System.Windows.Style _BtnStyle = (System.Windows.Style)Application.Current.Resources["ThisPageButtonStyle"];
        public System.Windows.Style BtnStyle
        {
            get
            {
                return _BtnStyle;
            }
            set
            {
                _BtnStyle = value;
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


        public bool CanFirstWeighing
        {
            get
            {
                if (WeighingTimes == 1
                    && !WeightStr.Equals("")
                    && ManualWeighing
                    && EnableWeighing
                    && (Convert.ToDecimal(WeightStr) > Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0")))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool CanSecondWeighing
        {
            get
            {
                if (WeighingTimes == 2
                    && !WeightStr.Equals("")
                    && ManualWeighing
                    && EnableWeighing
                    && (Convert.ToDecimal(WeightStr) > Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0")))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CanOnceWeighing
        {
            get
            {
                if (WeighingTimes == 1
                    && !WeightStr.Equals("")
                    && ManualWeighing
                    && EnableWeighing
                    && (Convert.ToDecimal(WeightStr) > Convert.ToDecimal(_mainSetting.Settings["MinSlotWeight"]?.Value ?? "0")))
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
        /// <summary>
        /// 逻辑调整, state参数已经没什么用了  --阿吉 2023年10月10日16点48分 
        /// </summary>
        /// <param name="state"></param>
        public void SetBtnTExt(int state = 0)
        {
            var autoWeigthing = (_mainSetting.Settings["WeighingControl"]?.Value ?? string.Empty)
                .Equals("auto", StringComparison.OrdinalIgnoreCase);
            if (!autoWeigthing && _manualPZOrMZ && WeighingTimes == 1)
            {
                ManualPZOrMZBtnsVisible = true;
                ManualMZText = _mainSetting.Settings["_mz"]?.Value ?? "毛重";
                ManualPZText = _mainSetting.Settings["_pz"]?.Value ?? "皮重";
            }
            else
            {
                ManualPZOrMZBtnsVisible = false;
            }

            var text = string.Empty;
            var camera1CoverIndex = Globalspace._ajCamera1GPIOState.FindIndex(p => p == 0);

            var camera2CoverIndex = Globalspace._ajCamera2GPIOState.FindIndex(p => p == 0);

            var tempWeighingTimes = TwiceWeighing ? WeighingTimes : 0;

            _cameraGPIOCovered = camera1CoverIndex != -1 || camera2CoverIndex != -1;

            if (_isCheckGrating && _cameraGPIOCovered)
            {
                var cameraTxt = string.Empty;
                var info = string.Empty;
                if (camera1CoverIndex != -1)
                {
                    cameraTxt = "相机1";
                    info = $"光栅{camera1CoverIndex + 1}";
                }
                else if (camera2CoverIndex != -1)
                {
                    cameraTxt = "相机2";
                    info = $"光栅{camera2CoverIndex + 1}";
                }

                text = $"({cameraTxt}-{info})被遮挡";

                if (camera1CoverIndex != -1 && camera2CoverIndex != -1)
                {
                    text = "(所有相机光栅全遮挡)";
                }
                _firstWeighingBtn.IsEnabled = _secondWeighingBtn.IsEnabled = _onceWeighingBtn.IsEnabled = false;

                if (_gratingTTSConfig.Enable && !string.IsNullOrWhiteSpace(_gratingTTSConfig.Text))
                {
                    WeighingRecordModel tempRcd;
                    if (weighingRecord == null)
                    {
                        tempRcd = new WeighingRecordModel { ChargeInfoConfig = _chargeInfoCfg };
                    }
                    else
                    {
                        tempRcd = weighingRecord;
                    }

                    if (_currentPrompt == null || _currentPrompt.IsCompleted)
                    {
                        _currentPrompt = TTSHelper.TTS(tempRcd, _gratingTTSConfig.Text);
                    }
                }
            }
            else
            {
                if (_firstWeighingBtn != null && tempWeighingTimes == 1)
                {
                    _firstWeighingBtn.IsEnabled = EnableWeighing;
                    _secondWeighingBtn.IsEnabled = false;
                }

                if (_secondWeighingBtn != null && tempWeighingTimes == 2)
                {
                    _firstWeighingBtn.IsEnabled = false;
                    _secondWeighingBtn.IsEnabled = EnableWeighing;
                }

                if (_onceWeighingBtn != null && tempWeighingTimes == 0)
                {
                    _firstWeighingBtn.IsEnabled = false;
                    _secondWeighingBtn.IsEnabled = false;
                    _onceWeighingBtn.IsEnabled = EnableWeighing;
                }
            }

            //text = Globalspace._signoState != "3" ? text : string.Empty;
            switch (tempWeighingTimes)
            {
                case 1:
                    Btn1Text = $"{_defaultBtn1Txt}{text}";
                    Btn2Text = $"{_defaultBtn2Txt}";
                    Btn3Text = $"称重{text}";
                    Globalspace.CurrentBtnContent = Btn1Text;
                    break;
                case 2:
                    Btn1Text = $"{_defaultBtn1Txt}";
                    Btn2Text = $"{_defaultBtn2Txt}{text}";
                    Btn3Text = $"称重{text}";
                    Globalspace.CurrentBtnContent = Btn2Text;
                    break;
                case 0:
                    Btn1Text = $"{_defaultBtn1Txt}";
                    Btn2Text = $"{_defaultBtn2Txt}";
                    Btn3Text = $"称重{text}";
                    Globalspace.CurrentBtnContent = Btn3Text;
                    break;
                default:
                    Btn1Text = $"{_defaultBtn1Txt}{text}";
                    Globalspace.CurrentBtnContent = Btn1Text;
                    Btn2Text = $"{_defaultBtn2Txt}{text}";
                    Btn3Text = $"称重{text}";
                    Globalspace.CurrentBtnContent = Btn1Text;
                    //Globalspace.CurrentBtnContent = string.Empty;
                    break;
            }
        }

        /// <summary>
        /// 博兴接口，接口返回成功才开闸，走后续逻辑 --阿吉 2023年6月25日15点39分
        /// </summary>
        /// <returns></returns>
        private bool CheckBoxingPlatform()
        {
            if (!_enable_Shandong_Boxing_Platform.GetValueOrDefault())
            {
                return true;
            }
            using (var bxPlatform = new Shandong_Boxing_Platform())
            {
                var ret = bxPlatform.Finish(weighingRecord.Bh, weighingRecord.Ch,
                    Globalspace._lprDevNo,
                    WeightStr.TryGetDecimal(), (err) => { });
                if (ret.code == 10001)
                {
                    var errMsg = ret.message ?? "称重异常，禁止下磅";
                    ShowLogs($"{weighingRecord.Ch}:{errMsg}");
                    TTSHelper.TTS(weighingRecord, errMsg);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 手动称重的话, reef接口判断 --阿吉 2023年12月9日18点42分
        /// </summary>
        /// <returns></returns>
        private bool CheckReefPlatform()
        {
            if (!"auto".Equals(_mainSetting.Settings["WeighingControl"].Value, StringComparison.CurrentCultureIgnoreCase))
            {
                if (Common.Platform.PlatformManager.Instance.Current is ReefPlatform reefPlatform)
                {
                    var reefApiRet = reefPlatform.API.CheckCarNo(weighingRecord.Ch);
                    if (!reefApiRet.Success)
                    {
                        log.Info($"reef 接口返回失败:{reefApiRet.Message}");
                        TTSHelper.TTS(weighingRecord, "预约车辆禁止通行");
                        return false;
                    }
                }
            }

            return true;
        }

        private System.Windows.Controls.Button _firstWeighingBtn;
        public void FirstWeighingLoadedCmd(object sender, RoutedEventArgs e)
        {
            _firstWeighingBtn = (System.Windows.Controls.Button)sender;
        }

        public void FirstWeighing()
        {
            
            ManualPZOrMZBtnsVisible = false;
            ShowLogs($"检测到车牌：{weighingRecord.Ch} 正在保存数据");
            if (!SQLDataAccess.CheckConnectionStatus())
            {
                ShowLogs("数据库访问失败，请联系管理员！");
                return;
            }

            // 博兴接口判断 --阿吉 2023年6月25日19点02分
            if (!CheckBoxingPlatform())
            {
                return;
            }

            #region 表单数据预处理
            //没输入车号，提示并返回，车牌号被禁用，提示并返回，新车牌，先保存到车辆表
            weighingRecord.Ch = weighingRecord.Ch?.Trim();
            if (weighingRecord.Ch != null && weighingRecord.Ch != "")
            {
                if (SQLDataAccess.LoadDisabledCar(weighingRecord.Ch) != null)
                {
                    ShowLogs("此车牌号已被禁用，请在【数据管理】中启用此车牌号后继续使用！");
                    return;
                }

                if (SQLDataAccess.LoadCar(weighingRecord.Ch) == null)
                {
                    var car = new CarModel
                    {
                        PlateNo = weighingRecord.Ch

                    };
                    SQLDataAccess.SaveCar(car);
                }
            }
            else
            {
                System.Diagnostics.Trace.WriteLine($"hit {nameof(FirstWeighing)}");
                ShowLogs("请输入车号！");
                return;
            }

            //如果客户被禁用，提示并返回，如果是新客户，先存新的客户到客户表
            weighingRecord.Kh = weighingRecord.Kh?.Trim();
            weighingRecord.Kh2 = weighingRecord.Kh2?.Trim();
            weighingRecord.Kh3 = weighingRecord.Kh3?.Trim();

            CustomerModel mainCustomer = null;
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kh))
            {
                if (SQLDataAccess.LoadDisabledCustomer(weighingRecord.Kh) != null)
                {
                    ShowLogs("此客户已被禁用，请在【数据管理】中启用此客户后继续使用！");
                    return;
                }
                mainCustomer = SQLDataAccess.LoadCustomer(weighingRecord.Kh);
                if (mainCustomer == null)
                {
                    mainCustomer = new CustomerModel
                    {
                        Num = DateTime.Now.Ticks.ToString(),
                        Name = weighingRecord.Kh
                    };
                    SQLDataAccess.SaveCustomer(mainCustomer);
                }
            }
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kh2))
            {
                if (SQLDataAccess.LoadDisabledCustomer(weighingRecord.Kh2) != null)
                {
                    ShowLogs("此客户2已被禁用，请在【数据管理】中启用此客户后继续使用！");
                    return;
                }

                if (SQLDataAccess.LoadCustomer(weighingRecord.Kh2) == null)
                {
                    CustomerModel customer = new CustomerModel
                    {
                        Num = DateTime.Now.Ticks.ToString(),
                        Name = weighingRecord.Kh2
                    };
                    SQLDataAccess.SaveCustomer(customer);
                }
            }
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kh3))
            {
                if (SQLDataAccess.LoadDisabledCustomer(weighingRecord.Kh3) != null)
                {
                    ShowLogs("此客户3已被禁用，请在【数据管理】中启用此客户后继续使用！");
                    return;
                }

                if (SQLDataAccess.LoadCustomer(weighingRecord.Kh3) == null)
                {
                    CustomerModel customer = new CustomerModel
                    {
                        Num = DateTime.Now.Ticks.ToString(),
                        Name = weighingRecord.Kh3
                    };
                    SQLDataAccess.SaveCustomer(customer);
                }
            }

            //如果物资被禁用，提示并返回
            weighingRecord.Wz = weighingRecord.Wz?.Trim();
            GoodsModel goods = null;
            if (weighingRecord.Wz != null && weighingRecord.Wz != "")
            {
                if (SQLDataAccess.LoadDisabledGoods(weighingRecord.Wz) != null)
                {
                    ShowLogs("此物资已被禁用，请在【数据管理】中启用此物资后继续使用！");
                    return;
                }

                //如果是新的物资，先存新的物资到物资表
                goods = SQLDataAccess.LoadGoods(weighingRecord.Wz);
                if (goods == null)
                {
                    goods = new GoodsModel
                    {
                        Num = DateTime.Now.Ticks.ToString(),
                        Name = weighingRecord.Wz
                    };
                    SQLDataAccess.SaveGoods(goods);
                }
                //else //不是新物资
                //{
                //    weighingRecord.By1 = g.Comment;
                //}
            }

            // 如果不等于0， 则表示是手动输入的 --阿吉 2023年11月15日11点21分
            if (weighingRecord.GoodsPrice == 0)
            {
                weighingRecord.GoodsPrice = GetGoodsPrice(mainCustomer, goods);
            }

            #endregion


            // Reef接口判断 --阿吉 2023年12月9日18点42分
            if (!CheckReefPlatform())
            {
                return;
            }

            #region 保存数据到数据库
            weighingRecord.Bh = CreateBh();// Common.Data.SQLDataAccess.CreateBh(DateTime.Now);
                                           //weighingRecord.Bh = DateTime.Today.ToString("yyyyMMdd");
                                           //WeighingRecordModel queryBh = SQLDataAccess.LoadWeighingRecord(weighingRecord.Bh);
                                           //if (queryBh != null)
                                           //{
                                           //    int iBh = Convert.ToInt32(queryBh.Bh.Substring(queryBh.Bh.Length - 4, 4)) + 1;
                                           //    weighingRecord.Bh += iBh.ToString().PadLeft(4, '0');
                                           //}
                                           //else
                                           //{
                                           //    weighingRecord.Bh += "0001";
                                           //}

            // 如果没有启用手动皮重/毛重,或者皮重未赋值(来自SetPZCmd),则毛重赋值,
            var normalWeigh = !_manualPZOrMZ || string.IsNullOrEmpty(weighingRecord.Pz);
            if (normalWeigh)
            {
                weighingRecord.Mz = WeightStr;
                weighingRecord.Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                weighingRecord.Mzsby = Globalspace._currentUser.UserName;
            }

            string discount = _mainSetting.Settings["Discount"].Value;
            if (discount.Equals("1"))
            {
                weighingRecord.Kz = _mainSetting.Settings["DiscountWeight"].Value;
            }
            else if (discount.Equals("2"))
            {
                weighingRecord.Kl = _mainSetting.Settings["DiscountRate"].Value;
            }

            if (WeightStr == WeightStr1)
            {
                weighingRecord.WeighName = _mainSetting.Settings["WeighName"].Value;
            }
            else if (WeightStr == WeightStr2)
            {
                weighingRecord.WeighName = _mainSetting.Settings["Weigh2Name"].Value;
            }
            weighingRecord.WeighingTimes = 1;
            weighingRecord.IsFinish = false;
            weighingRecord.WeighingFormTemplate = _mainSetting.Settings["PrintTemplate"].Value;

            #endregion

            IPCHelper.SendMsgToApp("ZXWeighingRecord", IPCHelper.UPDATE_RECORD);
            //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IS_STABLE, weighingRecord.Mz);
            log.Debug("1");
            //输出到文本文档，台帐用
            using (StreamWriter sw = new StreamWriter("./Data/LastWeighInfo.txt", false, Encoding.Default))
            {
                sw.WriteLine(weighingRecord.ToString() + "," + Register.GetHdInfo());
            }
            log.Debug("2");

            var overloadLog = _mainSetting.Settings["OverloadLog"].Value;
            if (_mainSetting.Settings["OverloadWarning"].Value == "1") //毛重超载报警
            {
                decimal maxWeight = Convert.ToDecimal(_mainSetting.Settings["OverloadWarningWeight"]?.Value ?? "0");
                if (maxWeight > 0)
                {
                    if (Convert.ToDecimal(WeightStr) > maxWeight)
                    {
                        TTSHelper.TTS(weighingRecord, _mainSetting.Settings["OverloadWarningText"].Value);
                        OverWeight = true;
                        weighingRecord.IsLimit = true;
                        weighingRecord.LimitedValue = maxWeight.ToString();
                        weighingRecord.LimitType = "毛重超载";

                        SQLDataAccess.SavOverloadLog(new OverloadLog()
                        {
                            PlateNo = weighingRecord.Ch,
                            AxleCount = weighingRecord.AxleNum,
                            Constraints = "毛重超载",
                            OverloadWeight = WeightStr,
                            StandardWeight = maxWeight.ToString(),
                            Times = "First",
                            CreateDate = DateTime.Now
                        }, overloadLog);
                    }
                }
                else // =0 时使用轮轴识别
                {
                    switch (weighingRecord.By2)
                    {
                        case "2":
                            if (Convert.ToDecimal(WeightStr) > 18000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 18000;
                            }
                            break;
                        case "3":
                            if (Convert.ToDecimal(WeightStr) > 27000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 18000;
                            }
                            break;
                        case "4":
                            if (Convert.ToDecimal(WeightStr) > 36000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 18000;
                            }
                            break;
                        case "5":
                            if (Convert.ToDecimal(WeightStr) > 43000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 18000;
                            }
                            break;
                        case "6":
                            if (Convert.ToDecimal(WeightStr) > 49000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 18000;
                            }
                            break;
                        case "7":
                            log.Debug("weighingRecord.By2 Value:" + weighingRecord.By2);
                            break;
                        default:
                            log.Debug("weighingRecord.By2 Value:" + weighingRecord.By2);
                            break;
                    }
                    if (OverWeight)
                    {
                        TTSHelper.TTS(weighingRecord, _mainSetting.Settings["OverloadWarningText"].Value);
                        log.Debug($"轴数：{weighingRecord.By2},已超载");

                        if (_mainSetting.Settings["LED2Enable"].Value == "True")
                        {
                            using (StreamWriter sw = new StreamWriter("./Data/LedText/weighingcomplete.txt", false, Encoding.Default))
                            {
                                if (weighingRecord.Ch != null) sw.WriteLine("车号: " + weighingRecord.Ch);
                                if (weighingRecord.Mz != null) sw.WriteLine("毛重: " + weighingRecord.Mz + _mainSetting.Settings["WeighingUnit"].Value);
                                sw.Write("已超载");
                            }

                            IPCHelper.SendMsgToApp("ZXLED", IPCHelper.WEIGHING_COMPLETE);
                        }

                        setOverLpr(WeightStr, OverWeightCount.ToString(), weighingRecord.By2);
                        return;
                    }
                }

            }
            else if (_mainSetting.Settings["OverloadWarning"].Value == "3") //车轴计算超载报警
            {
                var standard = _mainSetting.Settings[$"OverloadAxle{weighingRecord.AxleNum}"]?.Value ?? "0";

                if (Convert.ToDecimal(WeightStr) > Convert.ToDecimal(standard))
                {
                    OverWeight = true;
                    weighingRecord.IsLimit = true;
                    weighingRecord.LimitedValue = standard;
                    weighingRecord.LimitType = "车轴计算超载";

                    SQLDataAccess.SavOverloadLog(new OverloadLog()
                    {
                        PlateNo = weighingRecord.Ch,
                        AxleCount = weighingRecord.AxleNum,
                        Constraints = "车轴计算超载",
                        OverloadWeight = WeightStr,
                        StandardWeight = standard,
                        Times = "First",
                        CreateDate = DateTime.Now
                    }, overloadLog);
                }

            }

            weighingRecord.EntryTime = DateTime.Now;//第一次称重的，记录出称重时间
                                                    //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IS_STABLE, weighingRecord.Mz);
            weighingRecord.SerialNumber = Common.Data.SQLDataAccess.CreateRecordSerialNumber();

            //2022-09-06 如果是光栅遮挡了，还要保存的，就要将车牌号码后面追加“?”来区分
            if (Globalspace.CurrentBtnContent.Contains("遮挡"))
            {
                weighingRecord.IsCover = true;
            }
            

            //-------------------2023-03-03 汪虎 这里加入第一次的收费逻辑-------------------------
            //如果启用了多次收费（第一次收费+第二次收费）
            if (_chargeInfoCfg.ChargeEnable && _chargeInfoCfg.MultipleChargeEnable)
            {
                weighingRecord.GetCost(1, (msg) => ShowLogs(msg));

                //2022-12-01 此处添加是为了 电子支付那边获取费用而定
                AWSV2.Globalspace.CurrentCost = weighingRecord.Je;

                //ShowLogs($"本次费用：{weighingRecord.Je} 元");
                //------------------------------------------------------------
                //2022-11-07增加 LPR 单向模式 下的 显示称重费用
                if (_chargeInfoCfg.ChargeEnable && _mainSetting.Settings["Barrier"].Value == "1")//单向模式
                {
                    lprMessage(weighingRecord.Ch, "过磅总费", $"{weighingRecord.Je}元", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), IPCHelper.WEIGHING_COMPLETE);
                }

                Globalspace.CurrentOutPlate = weighingRecord.Ch;
                //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IS_STABLE, weighingRecord.Mz);


                //在计算费用这里标记为称重完成状态，因为下面的收费框一旦弹出来就是模态的，会阻塞其他用到
                //称重完成状态的子线程的逻辑
                CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;
                var testVal = string.Empty;
                if (_chargeInfoCfg.ChargeEnable && _chargeInfoCfg.ChargeWay == "1" && (!_chargeInfoCfg.ChargeStorage || string.IsNullOrWhiteSpace(weighingRecord.Kh)))//0：缴费后出场，1：缴费后下磅
                                                                                                                                                                      //if (ChargeEnable && ChargeWay == "1" && !ChargeStorage)//0：缴费后出场，1：缴费后下磅
                {
                    //默认设置收费方式为线上支付,如果是线下支付，则在弹窗里点击按钮的时候修改
                    weighingRecord.IsPay = 1;
                    weighingRecord.Zfsj = DateTime.Now;

                    if (_chargeInfoCfg.ChargeType != "3")
                    {

                        confirmWithChargeView = new ConfirmWithChargeViewModel(weighingRecord);
                        confirmWithChargeView.Parent = this;
                        bool? result = windowManager.ShowDialog(confirmWithChargeView);

                        if (result.GetValueOrDefault(true))
                        {
                            OpenBarrier(false);
                        }
                    }

                }
                else if (_chargeInfoCfg.ChargeEnable && _chargeInfoCfg.ChargeStorage)//启用客户储值功能结算
                {
                    var mess = GetAmount(weighingRecord, weighingRecord.Kh);
                    if (!string.IsNullOrEmpty(mess))//如果是余额不足，那么将通过线下支付或者电子支付的方式继续流程 modify by：2022-10-24
                    {
                        //语音播报余额不足
                        TTSHelper.TTS(weighingRecord, mess);

                        //默认设置收费方式为线上支付,如果是线下支付，则在弹窗里点击按钮的时候修改
                        weighingRecord.IsPay = 1;
                        weighingRecord.Zfsj = DateTime.Now;

                        //弹出收费框，强制首费。
                        if (_chargeInfoCfg.ChargeType != "3")
                        {

                            confirmWithChargeView = new ConfirmWithChargeViewModel(weighingRecord);
                            confirmWithChargeView.Parent = this;
                            bool? result = windowManager.ShowDialog(confirmWithChargeView);

                            if (result.GetValueOrDefault(true))
                            {
                                OpenBarrier(false);
                            }
                        }

                    }
                    else
                    {
                        //设置为储值支付方式
                        weighingRecord.IsPay = 3;
                        weighingRecord.Zfsj = DateTime.Now;
                    }
                }
                else
                {
                    OpenBarrier(false);
                }

                //不保存本次称重的数据
                if (NoSave)
                {
                    WeighingTimes = 0;
                    return;
                }
            }
            else
            {
                OpenBarrier(false);
            }
            //------------------------收费逻辑结束-------------------------

            SQLDataAccess.SaveWeighingRecord(weighingRecord);
            ShowLogs($"检测到车牌：{weighingRecord.Ch} 数据保存完成");

            if (Convert.ToBoolean(_mainSetting.Settings["TTS1Enable"]?.Value ?? "False") && !OverWeight)
            {
                var formatStr = _mainSetting.Settings["TTS1Text"].Value;
                if (!normalWeigh)
                {
                    formatStr = formatStr.Replace("%mz%", "%pz%");
                }

                TTSHelper.TTS(weighingRecord, formatStr);
            }

            if (Convert.ToBoolean(_mainSetting.Settings["MonitorEnable"]?.Value ?? "False"))
            {
                try
                {
                    if (!Directory.Exists("./Snap/" + weighingRecord.Bh)) Directory.CreateDirectory("./Snap/" + weighingRecord.Bh);
                    IPCHelper.SendMsgToApp("ZXMonitor", "FirstWeighing:" + weighingRecord.Ch + ":" + weighingRecord.Bh);
                    CopyVideo(weighingRecord);
                }
                catch (Exception e)
                {
                    log.Debug(e.Message);
                }
            }

            if (_mainSetting.Settings["LPR"].Value == "2")
            {
                CopyImg(weighingRecord);
            }


            if (_mainSetting.Settings["LED2Enable"].Value == "True")
            {
                var text = _mainSetting.Settings["Leddyc"].Value;
                ShowLedContent(text, "./Data/LedText/weighingcomplete.txt", weighingRecord);
                IPCHelper.SendMsgToApp("ZXLED", IPCHelper.WEIGHING_COMPLETE);
            }

            setOverLpr(WeightStr, "0", weighingRecord.By2);

            //OpenBarrier(false);

            //2022-11-07增加 LPR 单向模式 下的 称重 LED显示
            if (_mainSetting.Settings["Barrier"].Value == "1")//单向模式
            {
                string unit = _mainSetting.Settings["WeighingUnit"].Value;
                unit = string.IsNullOrEmpty(unit) ? "千克" : unit.ToLower() == "kg" ? "千克" : "吨";
                lprMessage(weighingRecord.Ch, "称重完成", $"重量{weighingRecord.Mz}{unit}", "缓慢下磅", IPCHelper.WEIGHING_COMPLETE);
            }


            // 原逻辑 Convert.ToBoolean(ConfigurationManager.AppSettings["SyncDataEnable"]) --阿吉 2023年7月3日12点10分
            // 现在不管这个开关， 直接写数据库 --阿吉 2023年7月3日12点11分
            if (!string.IsNullOrWhiteSpace(weighingRecord.Bh))
            {
                var SyncData = new SyncDataModel
                {
                    WeighingRecordBh = weighingRecord.Bh,
                    SyncMode = "insert"
                };
                SQLDataAccess.SaveSyncData(SyncData);
                IPCHelper.SendMsgToApp("ZXSyncWeighingData", IPCHelper.SYNC_DATA);
            }

            if (Convert.ToBoolean(_mainSetting.Settings["WithPrinting"]?.Value ?? "False"))
            {
                var copy = JsonConvert.DeserializeObject<WeighingRecordModel>(JsonConvert.SerializeObject(weighingRecord));
                Task.Run(() =>
                {
                    weighingRecord.Dyrq = copy.Dyrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    AWSV2.Services.PrintHelper.PrintImg(copy, worksheet, "FirstWeighing");
                });
            }
            //Thread.Sleep(1500);
            //IPCHelper.SendMsgToApp("ZXLPR", 0x0F0A); //绿灯，道闸不动

            WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);

            StatusBar = "第一次称重完成";
            WeighingTimes = 0;
            CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;

            SetImg("first", weighingRecord.Ch, weighingRecord.Bh);

            //RemoveICCard(weighingRecord); 2022-11-01 第一次不删除，因为这个时候称重还没完成。
            QueryList(string.Empty);
            if (_mainSetting.Settings["TimelyRefresh"].Value == "1")
            {
                RefreshWeighForm();
            }
        }

        public void SetPZCmd()
        {
            weighingRecord.Pz = WeightStr;
            weighingRecord.Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            weighingRecord.Pzsby = Globalspace._currentUser.UserName;
            FirstWeighing();
        }


        private System.Windows.Controls.Button _secondWeighingBtn;
        public void SecondWeighingLoadedCmd(object sender, RoutedEventArgs e)
        {
            _secondWeighingBtn = (System.Windows.Controls.Button)sender;
        }

        private void ComputeByFieldsValue()
        {
            var total = 20;
            var formula = string.Empty;
            var field = string.Empty;
            var @props = weighingRecord.GetType().GetRuntimeProperties();

            var context = new ExpressionContext();
            context.Imports.AddType(typeof(Math));

            for (int i = 1; i <= total; i++)
            {
                field = $"By{i}";
                formula = _mainSetting.Settings[$"{field}Formula"]?.Value ?? string.Empty;
                if (string.IsNullOrWhiteSpace(formula))
                {
                    continue;
                }
                var prop = @props.FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));
                if (prop == null)
                {
                    continue;
                }

                prop.SetValue(weighingRecord, FormulaCalc(formula, context, @props));
            }
        }

        public void SecondWeighing()
        {
            ManualPZOrMZBtnsVisible = false;

            ShowLogs($"检测到车牌：{weighingRecord.Ch} 正在保存数据");
            if (!SQLDataAccess.CheckConnectionStatus())
            {
                ShowLogs("数据库访问失败，请联系管理员！");
                return;
            }

            // 博兴接口判断 --阿吉 2023年6月25日19点02分
            if (!CheckBoxingPlatform())
            {
                return;
            }

            #region 表单数据预处理
            //没输入车号，提示并返回，车牌号被禁用，提示并返回，新车牌，先保存到车辆表
            weighingRecord.Ch = weighingRecord.Ch?.Trim();
            if (weighingRecord.Ch != null && weighingRecord.Ch != "")
            {
                if (SQLDataAccess.LoadDisabledCar(weighingRecord.Ch) != null)
                {
                    ShowLogs("此车牌号已被禁用，请在【数据管理】中启用此车牌号后继续使用！");
                    return;
                }

                if (SQLDataAccess.LoadCar(weighingRecord.Ch) == null)
                {
                    var car = new CarModel
                    {
                        PlateNo = weighingRecord.Ch

                    };
                    SQLDataAccess.SaveCar(car);
                }
            }
            else
            {
                System.Diagnostics.Trace.WriteLine($"hit {nameof(SecondWeighing)}");
                ShowLogs("请输入车号！");
                return;
            }

            //如果客户被禁用，提示并返回，如果是新客户，先存新的客户到客户表
            weighingRecord.Kh = weighingRecord.Kh?.Trim();
            weighingRecord.Kh2 = weighingRecord.Kh2?.Trim();
            weighingRecord.Kh3 = weighingRecord.Kh3?.Trim();

            CustomerModel mainCustomer = null;
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kh))
            {
                if (SQLDataAccess.LoadDisabledCustomer(weighingRecord.Kh) != null)
                {
                    ShowLogs("此客户已被禁用，请在【数据管理】中启用此客户后继续使用！");
                    return;
                }
                mainCustomer = SQLDataAccess.LoadCustomer(weighingRecord.Kh);
                if (mainCustomer == null)
                {
                    mainCustomer = new CustomerModel
                    {
                        Num = DateTime.Now.Ticks.ToString(),
                        Name = weighingRecord.Kh
                    };
                    SQLDataAccess.SaveCustomer(mainCustomer);
                }
            }
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kh2))
            {
                if (SQLDataAccess.LoadDisabledCustomer(weighingRecord.Kh2) != null)
                {
                    ShowLogs("此客户2已被禁用，请在【数据管理】中启用此客户后继续使用！");
                    return;
                }

                if (SQLDataAccess.LoadCustomer(weighingRecord.Kh2) == null)
                {
                    CustomerModel customer = new CustomerModel
                    {
                        Num = DateTime.Now.Ticks.ToString(),
                        Name = weighingRecord.Kh2
                    };
                    SQLDataAccess.SaveCustomer(customer);
                }
            }
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kh3))
            {
                if (SQLDataAccess.LoadDisabledCustomer(weighingRecord.Kh3) != null)
                {
                    ShowLogs("此客户3已被禁用，请在【数据管理】中启用此客户后继续使用！");

                    return;
                }

                if (SQLDataAccess.LoadCustomer(weighingRecord.Kh3) == null)
                {
                    CustomerModel customer = new CustomerModel
                    {
                        Num = DateTime.Now.Ticks.ToString(),
                        Name = weighingRecord.Kh3
                    };
                    SQLDataAccess.SaveCustomer(customer);
                }
            }

            //如果物资被禁用，提示并返回
            weighingRecord.Wz = weighingRecord.Wz?.Trim();
            GoodsModel goods = null;
            if (weighingRecord.Wz != null && weighingRecord.Wz != "")
            {
                if (SQLDataAccess.LoadDisabledGoods(weighingRecord.Wz) != null)
                {
                    ShowLogs("此物资已被禁用，请在【数据管理】中启用此物资后继续使用！");
                    return;
                }

                //如果是新的物资，先存新的物资到物资表
                goods = SQLDataAccess.LoadGoods(weighingRecord.Wz);
                if (goods == null)
                {
                    goods = new GoodsModel
                    {
                        Num = DateTime.Now.Ticks.ToString(),
                        Name = weighingRecord.Wz
                    };
                    SQLDataAccess.SaveGoods(goods);
                }
                //else //不是新物资
                //{
                //    weighingRecord.By1 = g.Comment;
                //}
            }
            // 如果不等于0， 则表示是手动输入的 --阿吉 2023年11月15日11点21分
            if (weighingRecord.GoodsPrice == 0)
            {
                weighingRecord.GoodsPrice = GetGoodsPrice(mainCustomer, goods);
            }


            #endregion

            // Reef接口判断 --阿吉 2023年12月9日18点42分
            if (!CheckReefPlatform())
            {
                return;
            }

            #region 保存数据到数据库

            #region  保存数据到数据库 老的逻辑段
            // //根据重量大小，设置毛重、皮重的数据
            // try
            // {
            //     ////这个判断是为了能保存用户手动输入的值
            //     //if (string.IsNullOrWhiteSpace(weighingRecord.Mz))
            //     //    weighingRecord.Mz = WeightStr;


            //     ////这个判断是为了能保存用户手动输入的值
            //     //if (string.IsNullOrWhiteSpace(weighingRecord.Pz))
            //     //    weighingRecord.Pz = WeightStr;

            //     //这个判断是为了能保存用户手动输入的值
            //     if (string.IsNullOrWhiteSpace(weighingRecord.Mz))
            //     {
            //         weighingRecord.Mz = WeightStr;
            //         weighingRecord.Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //     }

            //     //这个判断是为了能保存用户手动输入的值
            //     if (string.IsNullOrWhiteSpace(weighingRecord.Pz))
            //     {
            //         weighingRecord.Pz = WeightStr;
            //         weighingRecord.Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //     }


            //     var zdgblx = ConfigurationManager.AppSettings["AutoWeighingType"];
            //     if (Convert.ToDecimal(weighingRecord.Mz) > Convert.ToDecimal(WeightStr))
            //     {
            //         weighingRecord.Pz = WeightStr;
            //         weighingRecord.Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //         weighingRecord.Pzsby = Globalspace._currentUser.UserName;
            //         if (zdgblx == "1")
            //         {
            //             weighingRecord.Gblx = "采购";
            //         }

            //     }
            //     else
            //     {
            //         if (zdgblx == "1")
            //         {
            //             if (Convert.ToDecimal(weighingRecord.Mz) == Convert.ToDecimal(WeightStr))
            //                 weighingRecord.Gblx = "其他";
            //             else
            //                 weighingRecord.Gblx = "销售";
            //         }

            //         weighingRecord.Pz = weighingRecord.Mz;
            //         weighingRecord.Pzrq = weighingRecord.Mzrq;
            //         weighingRecord.Pzsby = weighingRecord.Mzsby;
            //         weighingRecord.Mz = WeightStr;
            //         weighingRecord.Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //         weighingRecord.Mzsby = Globalspace._currentUser.UserName;
            //     }
            // }
            // catch (Exception)
            // {
            // }

            // //2022-09-23 要求 毛重、皮重、净重都允许为空，所以加此if判断、
            // //weighingRecord.Jz = Math.Abs(Convert.ToDecimal(weighingRecord.Mz) - Convert.ToDecimal(weighingRecord.Pz)).ToString();
            //if(!string.IsNullOrEmpty(weighingRecord.Mz)&& !string.IsNullOrEmpty(weighingRecord.Pz))
            // {
            //     weighingRecord.Jz = Math.Abs(Convert.ToDecimal(weighingRecord.Mz) - Convert.ToDecimal(weighingRecord.Pz)).ToString();
            // }
            // else
            // {
            //     weighingRecord.Jz = "0";
            // }

            // weighingRecord.Jzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            #endregion

            //根据重量大小，设置毛重、皮重的数据
            try
            {
                decimal t_mz = 0;
                decimal t_pz = 0;

                if (!string.IsNullOrEmpty(weighingRecord.Mz) && !decimal.TryParse(weighingRecord.Mz, out t_mz))
                {
                    windowManager.ShowMessageBox("毛重请输入正确的数字格式");
                    return;
                }
                if (!string.IsNullOrEmpty(weighingRecord.Pz) && !decimal.TryParse(weighingRecord.Pz, out t_pz))
                {
                    windowManager.ShowMessageBox("皮重请输入正确的数字格式");
                    return;
                }

                //这个判断是为了能保存用户手动输入的值
                if (string.IsNullOrWhiteSpace(weighingRecord.Mz))
                {
                    weighingRecord.Mz = WeightStr;
                    weighingRecord.Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                //这个判断是为了能保存用户手动输入的值
                if (string.IsNullOrWhiteSpace(weighingRecord.Pz))
                {
                    weighingRecord.Pz = WeightStr;
                    weighingRecord.Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                //这个判断是为了验证，用户或者机器自动生成的 毛重是否小于皮重，小于则互换。不能出现毛重小于皮重的情况
                if (Convert.ToDecimal(weighingRecord.Mz) < Convert.ToDecimal(weighingRecord.Pz))
                {
                    string temp = weighingRecord.Mz;
                    string temp_rq = weighingRecord.Mzrq;
                    weighingRecord.Mz = weighingRecord.Pz;
                    //weighingRecord.Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    weighingRecord.Mzrq = weighingRecord.Pzrq;
                    weighingRecord.Mzsby = Globalspace._currentUser.UserName;

                    weighingRecord.Pz = temp;
                    //weighingRecord.Pzrq = weighingRecord.Mzrq;
                    weighingRecord.Pzrq = temp_rq;
                    weighingRecord.Pzsby = weighingRecord.Mzsby;

                    if (Convert.ToDecimal(weighingRecord.Mz) == Convert.ToDecimal(WeightStr))
                        weighingRecord.Gblx = "其他";
                    else
                        weighingRecord.Gblx = "销售";
                }
                else
                {
                    //weighingRecord.Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    weighingRecord.Pzsby = Globalspace._currentUser.UserName;
                    weighingRecord.Gblx = "采购";
                }
            }
            catch (Exception)
            {
            }

            weighingRecord.Jz = Math.Abs(Convert.ToDecimal(weighingRecord.Mz) - Convert.ToDecimal(weighingRecord.Pz)).ToString();
            weighingRecord.Jzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string discount = _mainSetting.Settings["Discount"].Value;
            if (discount.Equals("1"))
            {
                weighingRecord.Sz = (Convert.ToDecimal(weighingRecord.Jz) - Convert.ToDecimal(weighingRecord.Kz)).ToString();
            }
            else if (discount.Equals("2"))
            {
                weighingRecord.Sz = (Convert.ToDecimal(weighingRecord.Jz) * (100 - Convert.ToDecimal(weighingRecord.Kl)) / 100).ToString();
            }
            else
            {
                weighingRecord.Sz = weighingRecord.Jz;
            }

            //weighingRecord.By3 = NumFormatHelper.NumToChn(weighingRecord.Jz) + "吨";

            //备用字段公式运算
            ComputeByFieldsValue();

            //更新IC卡的备用字段信息
            if (weighingRecord.ICCard != null)
            {
                var ic = SQLDataAccess.LoadICCard(weighingRecord.ICCard);
                if (_mainSetting.Settings["By1Formula"].Value != "") ic.By1 = weighingRecord.By1;
                if (_mainSetting.Settings["By2Formula"].Value != "") ic.By2 = weighingRecord.By2;
                if (_mainSetting.Settings["By3Formula"].Value != "") ic.By3 = weighingRecord.By3;
                if (_mainSetting.Settings["By4Formula"].Value != "") ic.By4 = weighingRecord.By4;
                if (_mainSetting.Settings["By5Formula"].Value != "") ic.By5 = weighingRecord.By5;

                if (_mainSetting.Settings["By6Formula"].Value != "") ic.By6 = weighingRecord.By6;
                if (_mainSetting.Settings["By7Formula"].Value != "") ic.By7 = weighingRecord.By7;
                if (_mainSetting.Settings["By8Formula"].Value != "") ic.By8 = weighingRecord.By8;
                if (_mainSetting.Settings["By9Formula"].Value != "") ic.By9 = weighingRecord.By9;
                if (_mainSetting.Settings["By10Formula"].Value != "") ic.By10 = weighingRecord.By10;
                if (_mainSetting.Settings["By11Formula"].Value != "") ic.By11 = weighingRecord.By11;
                if (_mainSetting.Settings["By12Formula"].Value != "") ic.By12 = weighingRecord.By12;
                if (_mainSetting.Settings["By13Formula"].Value != "") ic.By13 = weighingRecord.By13;
                if (_mainSetting.Settings["By14Formula"].Value != "") ic.By14 = weighingRecord.By14;
                if (_mainSetting.Settings["By15Formula"].Value != "") ic.By15 = weighingRecord.By15;
                if (_mainSetting.Settings["By16Formula"].Value != "") ic.By6 = weighingRecord.By16;
                if (_mainSetting.Settings["By17Formula"].Value != "") ic.By17 = weighingRecord.By17;
                if (_mainSetting.Settings["By18Formula"].Value != "") ic.By18 = weighingRecord.By18;
                if (_mainSetting.Settings["By19Formula"].Value != "") ic.By19 = weighingRecord.By19;
                if (_mainSetting.Settings["By20Formula"].Value != "") ic.By20 = weighingRecord.By20;

                SQLDataAccess.UpdateICCard(ic);
            }

            if (WeightStr == WeightStr1)
            {
                weighingRecord.Weigh2Name = _mainSetting.Settings["WeighName"].Value;
            }
            else if (WeightStr == WeightStr2)
            {
                weighingRecord.Weigh2Name = _mainSetting.Settings["Weigh2Name"].Value;
            }
            weighingRecord.WeighingTimes = 2;
            weighingRecord.IsFinish = true;

            #endregion

            IPCHelper.SendMsgToApp("ZXWeighingRecord", IPCHelper.UPDATE_RECORD);
            //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IS_STABLE, weighingRecord.Mz);
            //输出到文本文档，台帐用
            using (StreamWriter sw = new StreamWriter("./Data/LastWeighInfo.txt", false, Encoding.Default))
            {
                sw.WriteLine(weighingRecord.ToString() + "," + Register.GetHdInfo());
            }

            var overloadLog = _mainSetting.Settings["OverloadLog"].Value;
            if (_mainSetting.Settings["OverloadWarning"].Value == "1") //毛重超载报警
            {
                decimal maxWeight = Convert.ToDecimal(_mainSetting.Settings["OverloadWarningWeight"]?.Value ?? "0");
                if (maxWeight > 0)
                {
                    if (Convert.ToDecimal(WeightStr) > maxWeight)
                    {
                        TTSHelper.TTS(weighingRecord, _mainSetting.Settings["OverloadWarningText"].Value);
                        OverWeight = true;
                        weighingRecord.IsLimit = true;
                        weighingRecord.LimitedValue = maxWeight.ToString();
                        weighingRecord.LimitType = "毛重超载";
                        log.Debug($"当前毛重重量：{WeightStr},已超载");

                        SQLDataAccess.SavOverloadLog(new OverloadLog()
                        {
                            PlateNo = weighingRecord.Ch,
                            AxleCount = weighingRecord.AxleNum,
                            Constraints = "毛重超载",
                            OverloadWeight = WeightStr,
                            StandardWeight = maxWeight.ToString(),
                            Times = "Second",
                            CreateDate = DateTime.Now
                        }, overloadLog);
                    }
                }
                else // =0 时使用轮轴识别
                {
                    switch (weighingRecord.By2)
                    {
                        case "2":
                            if (Convert.ToDecimal(WeightStr) > 18000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 18000;
                            }
                            break;
                        case "3":
                            if (Convert.ToDecimal(WeightStr) > 27000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 27000;
                            }
                            break;
                        case "4":
                            if (Convert.ToDecimal(WeightStr) > 36000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 36000;
                            }
                            break;
                        case "5":
                            if (Convert.ToDecimal(WeightStr) > 43000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 43000;
                            }
                            break;
                        case "6":
                            if (Convert.ToDecimal(WeightStr) > 49000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 49000;
                            }
                            break;
                        case "7":
                            log.Debug("weighingRecord.By2 Value:" + weighingRecord.By2);
                            break;
                        default:
                            log.Debug("weighingRecord.By2 Value:" + weighingRecord.By2);
                            break;
                    }
                    if (OverWeight)
                    {
                        TTSHelper.TTS(weighingRecord, _mainSetting.Settings["OverloadWarningText"].Value);
                        log.Debug($"轴数：{weighingRecord.By2},已超载");

                        if (_mainSetting.Settings["LED2Enable"].Value == "True")
                        {
                            using (StreamWriter sw = new StreamWriter("./Data/LedText/weighingcomplete.txt", false, Encoding.Default))
                            {
                                if (weighingRecord.Ch != null) sw.WriteLine("车号: " + weighingRecord.Ch);
                                if (weighingRecord.Mz != null) sw.WriteLine("毛重: " + weighingRecord.Mz + _mainSetting.Settings["WeighingUnit"].Value);
                                sw.Write("已超载");
                            }

                            IPCHelper.SendMsgToApp("ZXLED", IPCHelper.WEIGHING_COMPLETE);
                        }

                        setOverLpr(weighingRecord.Mz, OverWeightCount.ToString(), weighingRecord.By2);
                        return;
                    }
                }

            }
            else if (_mainSetting.Settings["OverloadWarning"].Value == "2") //净重超载报警
            {
                var _OverloadWarningWeight = _mainSetting.Settings["OverloadWarningWeight"].Value;
                if (Convert.ToDecimal(weighingRecord.Jz) > Convert.ToDecimal(_OverloadWarningWeight))
                {
                    TTSHelper.TTS(weighingRecord, _mainSetting.Settings["OverloadWarningText"].Value);
                    OverWeight = true;
                    weighingRecord.IsLimit = true;
                    weighingRecord.LimitedValue = _OverloadWarningWeight;
                    weighingRecord.LimitType = "净重超载";
                    log.Debug($"当前净重重量：{weighingRecord.Jz},已超载");

                    SQLDataAccess.SavOverloadLog(new OverloadLog()
                    {
                        PlateNo = weighingRecord.Ch,
                        AxleCount = weighingRecord.AxleNum,
                        Constraints = "净重超载",
                        OverloadWeight = weighingRecord.Jz,
                        StandardWeight = _OverloadWarningWeight,
                        Times = "Second",
                        CreateDate = DateTime.Now
                    }, overloadLog);
                }
            }
            else if (_mainSetting.Settings["OverloadWarning"].Value == "3") //车轴计算超载报警
            {
                var standard = _mainSetting.Settings[$"OverloadAxle{weighingRecord.AxleNum}"]?.Value ?? "0";

                if (Convert.ToDecimal(WeightStr) > Convert.ToDecimal(standard))
                {
                    OverWeight = true;
                    weighingRecord.IsLimit = true;
                    weighingRecord.LimitedValue = standard;
                    weighingRecord.LimitType = "车轴计算超载";

                    SQLDataAccess.SavOverloadLog(new OverloadLog()
                    {
                        PlateNo = weighingRecord.Ch,
                        AxleCount = weighingRecord.AxleNum,
                        Constraints = "车轴计算超载",
                        OverloadWeight = WeightStr,
                        StandardWeight = standard,
                        Times = "Second",
                        CreateDate = DateTime.Now
                    }, overloadLog);
                }

            }

            //----------这里为收费逻辑段  by：汪虎 2022-02-21-------------

            //if (ChargeEnable)//启用收费
            //{
            //    decimal weight = 0;
            //    if (ChargeTypes == "按毛重+皮重收费")//收费模式：0 按毛重+皮重收费，1按净重收费，2 按毛重收费
            //    {
            //        weight = Convert.ToDecimal(weighingRecord.Mz) + Convert.ToDecimal(weighingRecord.Pz);
            //    }
            //    else if (ChargeTypes == "按毛重收费")//收费模式：0 按毛重+皮重收费，1按净重收费，2 按毛重收费
            //    {
            //        weight = Convert.ToDecimal(weighingRecord.Mz);
            //    }
            //    else
            //    {
            //        weight = Convert.ToDecimal(weighingRecord.Jz);
            //    }
            //    //判断是否按物资收费，如果是 那么获取物资收费标准名称用于查询，如果不是，则用默认收费名称，默认名称为：普通收费标准
            //    var feesTitle = ChargeByMaterial ? !string.IsNullOrWhiteSpace(weighingRecord.Wz) ? weighingRecord.Wz.Trim() : "Normal_888888" : "Normal_888888";

            //    var ChargeInfoItem = ChargeInfo.FirstOrDefault(p => p.Name == feesTitle);
            //    if (ChargeInfoItem == null)
            //    {
            //        ChargeInfoItem = ChargeInfo.First(p => p.Name == "普通收费标准");//如果没有查到对应的物资信息，就采用默认收费标准
            //    }

            //    string fee = "0";
            //    if (ChargeInfoItem.Name == "普通收费标准")
            //    {
            //        fee = ToFeesList(ChargeInfoItem.Fees).Where(p => p.ChargeRule.UpperLimit >= weight && p.ChargeRule.LowerLimit <= weight).Max(o => o.Fee);
            //    }
            //    else
            //    {
            //        fee = (weight * ChargeInfoItem.Price).ToString("#.00");
            //    }

            //    weighingRecord.Je = string.IsNullOrWhiteSpace(fee) || fee == "0" ? LowestFees : fee;

            //    //TTSHelper.TTS(weighingRecord, ConfigurationManager.AppSettings["OverloadWarningText"]);
            //    if (!String.IsNullOrWhiteSpace(weighingRecord.Je))//表示有收费金额
            //        TTSHelper.TTS(weighingRecord, $"请缴费{weighingRecord.Je}元！");
            //}
            //else
            //{
            //    weighingRecord.Je = "0";
            //}

            var firstFee = string.IsNullOrEmpty(weighingRecord.Je) ? "0" : weighingRecord.Je;  //第一次收费的金额
            weighingRecord.GetCost(2, (msg) => ShowLogs(msg));//第二次收费的金额

            //2022-12-01 此处添加是为了 电子支付那边获取费用而定
            AWSV2.Globalspace.CurrentCost = weighingRecord.Je;
            //------------------------------------------------------------
            //2022-11-07增加 LPR 单向模式 下的 显示称重费用
            if (_chargeInfoCfg.ChargeEnable && _mainSetting.Settings["Barrier"].Value == "1")//单向模式
            {
                lprMessage(weighingRecord.Ch, "过磅总费", $"{weighingRecord.Je}元", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), IPCHelper.WEIGHING_COMPLETE);
            }

            Globalspace.CurrentOutPlate = weighingRecord.Ch;
            //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IS_STABLE, weighingRecord.Mz);


            //在计算费用这里标记为称重完成状态，因为下面的收费框一旦弹出来就是模态的，会阻塞其他用到
            //称重完成状态的子线程的逻辑
            CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;
            var testVal = string.Empty;
            if (_chargeInfoCfg.ChargeEnable && _chargeInfoCfg.ChargeWay == "1" && (!_chargeInfoCfg.ChargeStorage || string.IsNullOrWhiteSpace(weighingRecord.Kh)))//0：缴费后出场，1：缴费后下磅
            //if (ChargeEnable && ChargeWay == "1" && !ChargeStorage)//0：缴费后出场，1：缴费后下磅
            {
                //默认设置收费方式为线上支付,如果是线下支付，则在弹窗里点击按钮的时候修改
                weighingRecord.IsPay = 1;
                weighingRecord.Zfsj = DateTime.Now;

                if (_chargeInfoCfg.ChargeType != "3")
                {

                    confirmWithChargeView = new ConfirmWithChargeViewModel(weighingRecord);
                    confirmWithChargeView.Parent = this;
                    bool? result = windowManager.ShowDialog(confirmWithChargeView);

                    if (result.GetValueOrDefault(true))
                    {
                        OpenBarrier(false);
                    }
                }
            }
            else if (_chargeInfoCfg.ChargeEnable && _chargeInfoCfg.ChargeStorage)//启用客户储值功能结算
            {
                var mess = GetAmount(weighingRecord, weighingRecord.Kh);
                if (!string.IsNullOrEmpty(mess))//如果是余额不足，那么将通过线下支付或者电子支付的方式继续流程 modify by：2022-10-24
                {
                    //语音播报余额不足
                    TTSHelper.TTS(weighingRecord, mess);

                    //默认设置收费方式为线上支付,如果是线下支付，则在弹窗里点击按钮的时候修改
                    weighingRecord.IsPay = 1;
                    weighingRecord.Zfsj = DateTime.Now;

                    //弹出收费框，强制首费。
                    if (_chargeInfoCfg.ChargeType != "3")
                    {
                        confirmWithChargeView = new ConfirmWithChargeViewModel(weighingRecord);
                        confirmWithChargeView.Parent = this;
                        bool? result = windowManager.ShowDialog(confirmWithChargeView);

                        if (result.GetValueOrDefault(true))
                        {
                            OpenBarrier(false);
                        }
                    }

                }
                else
                {
                    //设置为储值支付方式
                    weighingRecord.IsPay = 3;
                    weighingRecord.Zfsj = DateTime.Now;
                }
            }
            else
            {
                OpenBarrier();
            }


            //不保存本次称重的数据
            if (NoSave)
            {
                WeighingTimes = 0;
                return;
            }


            //SQLDataAccess.UpdateWeighingRecord(weighingRecord);
            if (Convert.ToBoolean(_mainSetting.Settings["TTS2Enable"]?.Value ?? "False") && !OverWeight)
                TTSHelper.TTS(weighingRecord, _mainSetting.Settings["TTS2Text"].Value);

            if (Convert.ToBoolean(_mainSetting.Settings["MonitorEnable"]?.Value ?? "False"))
            {
                try
                {
                    IPCHelper.SendMsgToApp("ZXMonitor", "SecondWeighing:" + weighingRecord.Ch + ":" + weighingRecord.Bh);
                    CopyVideo(weighingRecord);
                }
                catch (Exception e)
                {
                    log.Debug(e.Message);
                }
            }


            if (_mainSetting.Settings["LPR"].Value == "2")
            {
                CopyImg(weighingRecord);
            }

            //判断扣重、扣率是否都填写了。如果都填写了。需要提示。
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kz) &&
                !string.IsNullOrWhiteSpace(weighingRecord.Kl) &&
                weighingRecord.Kz.Trim() != "0" &&
                 weighingRecord.Kl.Trim() != "0")
            {
                windowManager.ShowMessageBox("扣重、扣率只允许有一个，请处理！");
                return;
            }
            else
            {
                //计算实重多少
                if (!string.IsNullOrWhiteSpace(weighingRecord.Kz) && weighingRecord.Kz.Trim() != "0")
                {
                    double val = 0;
                    double.TryParse(weighingRecord.Kz, out val);
                    // jz-kz=sz
                    weighingRecord.Sz = (double.Parse(weighingRecord.Jz) - val).ToString();
                }
                else if (!string.IsNullOrWhiteSpace(weighingRecord.Kl) && weighingRecord.Kl.Trim() != "0")
                {
                    double val = 0;
                    double.TryParse(weighingRecord.Kl, out val);
                    //  jz*kl=sz
                    var t_jz = double.Parse(weighingRecord.Jz);
                    weighingRecord.Sz = (t_jz - (t_jz) * (val / 100)).ToString();
                }
            }
            //2022-09-06 如果是光栅遮挡了，还要保存的，就要将车牌号码后面追加“?”来区分 Globalspace.CurrentBtnContent
            if (Globalspace.CurrentBtnContent.Contains("遮挡") && !weighingRecord.Ch.Contains("?"))
            {
                weighingRecord.IsCover = true;
            }

            //最后判断是否是多次收费模式，如果是，那么在第二次收费的时候要把Je+第一次的Je返回给 weighingRecord.Je
            if (_chargeInfoCfg.MultipleChargeEnable)
            {
                weighingRecord.Je = string.IsNullOrEmpty(weighingRecord.Je) ? "0" : weighingRecord.Je;
                //第一次金额+第二次金额
                weighingRecord.Je = (decimal.Parse(firstFee) + decimal.Parse(weighingRecord.Je)).ToString("0.00");
            }

            SQLDataAccess.UpdateWeighingRecord(weighingRecord);



            //写图片视频文件地址到数据库
            ShowLogs($"检测到车牌：{weighingRecord.Ch} 数据保存完成");

            try
            {
                var picPath = Path.Combine(".\\Snap\\" + weighingRecord.Bh);
                if (Directory.Exists(picPath))
                {
                    var images = Directory.GetFiles(picPath).Where(s => s.EndsWith(".jpg") || s.EndsWith(".bmp")).ToList();
                    var videos = Directory.GetFiles(picPath).Where(s => s.EndsWith(".mp4")).ToList();

                    var wim = new WeighingImgModel();
                    wim.WRBh = weighingRecord.Bh;

                    Type t = wim.GetType();

                    for (int i = 0; i < images.Count(); i++)
                    {
                        string dbField = "Pic" + (i + 1);
                        if (i < 6) t.GetProperty(dbField).SetValue(wim, images[i]);
                    }
                    for (int i = 0; i < videos.Count(); i++)
                    {
                        string dbField = "Video" + (i + 1);
                        if (i < 2) t.GetProperty(dbField).SetValue(wim, videos[i]);
                    }

                    SQLDataAccess.SaveWeighingImg(wim);
                }
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }
            // 原逻辑 Convert.ToBoolean(ConfigurationManager.AppSettings["SyncDataEnable"]) --阿吉 2023年7月3日12点10分
            // 现在不管这个开关， 直接写数据库 --阿吉 2023年7月3日12点11分
            if (!string.IsNullOrWhiteSpace(weighingRecord.Bh))
            {
                var SyncData = new SyncDataModel
                {
                    WeighingRecordBh = weighingRecord.Bh,
                    SyncMode = "update"
                };
                SQLDataAccess.SaveSyncData(SyncData);
                IPCHelper.SendMsgToApp("ZXSyncWeighingData", IPCHelper.SYNC_DATA);
            }

            if (_mainSetting.Settings["LED2Enable"].Value == "True")
            {
                var text = _mainSetting.Settings["Leddec"].Value;
                ShowLedContent(text, "./Data/LedText/weighingcomplete.txt", weighingRecord);
                IPCHelper.SendMsgToApp("ZXLED", IPCHelper.WEIGHING_COMPLETE);
            }

            setOverLpr(weighingRecord.Mz, "0", weighingRecord.By2);

            //------------------------------------------------------------
            //2022-11-07增加 LPR 单向模式 下的 显示LED信息
            if (_mainSetting.Settings["Barrier"].Value == "1")//单向模式
            {
                string unit = _mainSetting.Settings["WeighingUnit"].Value;
                unit = string.IsNullOrEmpty(unit) ? "千克" : unit.ToLower() == "kg" ? "千克" : "吨";
                lprMessage("称重完成", $"毛重:{weighingRecord.Mz}{unit}", $"皮重:{weighingRecord.Pz}{unit}", $"净重:{weighingRecord.Jz}{unit}", IPCHelper.WEIGHING_COMPLETE);
            }

            WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
            var copy = JsonConvert.DeserializeObject<WeighingRecordModel>(JsonConvert.SerializeObject(weighingRecord));
            if (Convert.ToBoolean(_mainSetting.Settings["WithPrinting"]?.Value ?? "False"))
            {
                //Task.Run(() =>
                //{
                weighingRecord.Dyrq = copy.Dyrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                AWSV2.Services.PrintHelper.PrintImg(copy, worksheet, "SecondWeighing");
                //});
            }
            copy.ChargeInfoConfig = weighingRecord.ChargeInfoConfig;
            WeighFormVM = new WeighFormViewModel(copy, SelectedWeighFromTemplateSheet);

            StatusBar = "第二次称重完成";
            WeighingTimes = 0;
            //CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;

            SetImg("second", copy.Ch, copy.Bh);

            RemoveICCard(weighingRecord);
            QueryList(string.Empty);
            AddWaterMask();
            //UploadData(weighingRecord);

            Task.Run(() =>
            {
                UploadData(copy);
            });

            if (_mainSetting.Settings["TimelyRefresh"].Value == "1")
            {
                RefreshWeighForm();
            }

        }

        private System.Windows.Controls.Button _onceWeighingBtn;
        public void OnceWeighingLoadedCmd(object sender, RoutedEventArgs e)
        {
            _onceWeighingBtn = (System.Windows.Controls.Button)sender;
        }

        public void OnceWeighing()
        {
            ShowLogs($"检测到车牌：{weighingRecord.Ch} 正在保存数据");
            if (!SQLDataAccess.CheckConnectionStatus())
            {
                ShowLogs("数据库访问失败，请联系管理员！");
                return;
            }

            #region 表单数据预处理
            //处理车号、客户、物资中的前后空格，若车号为空则返回
            if (weighingRecord.Ch?.Trim() == "")
            {
                System.Diagnostics.Trace.WriteLine($"hit {nameof(OnceWeighing)}");
                ShowLogs("请输入车号！");
                return;
            }
            weighingRecord.Kh = weighingRecord.Kh?.Trim();
            weighingRecord.Kh2 = weighingRecord.Kh2?.Trim();
            weighingRecord.Kh3 = weighingRecord.Kh3?.Trim();
            weighingRecord.Wz = weighingRecord.Wz?.Trim();

            //如果车牌号被禁用，提示并返回
            if (SQLDataAccess.LoadDisabledCar(weighingRecord.Ch) != null)
            {
                ShowLogs("此车牌号已被禁用，请在【数据管理】中启用此车牌号后继续使用！");
                return;
            }

            //如果是新的车牌，先存新的车牌号到车辆表
            var car = SQLDataAccess.LoadCar(weighingRecord.Ch);
            if (car == null)
            {
                car = new CarModel
                {
                    PlateNo = weighingRecord.Ch
                };
                SQLDataAccess.SaveCar(car);
            }

            //如果客户被禁用，提示并返回
            if (SQLDataAccess.LoadDisabledCustomer(weighingRecord.Kh) != null)
            {
                ShowLogs("此客户已被禁用，请在【数据管理】中启用此客户后继续使用！");
                return;
            }
            if (SQLDataAccess.LoadDisabledCustomer(weighingRecord.Kh2) != null)
            {
                ShowLogs("此客户2已被禁用，请在【数据管理】中启用此客户后继续使用！");
                return;
            }
            if (SQLDataAccess.LoadDisabledCustomer(weighingRecord.Kh3) != null)
            {
                ShowLogs("此客户3已被禁用，请在【数据管理】中启用此客户后继续使用！");
                return;
            }
            //如果是新客户，先存新的客户到客户表
            CustomerModel mainCustomer = null;
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kh))
            {
                mainCustomer = SQLDataAccess.LoadCustomer(weighingRecord.Kh);
                if (mainCustomer == null)
                {
                    mainCustomer = new CustomerModel
                    {
                        Num = DateTime.Now.Ticks.ToString(),
                        Name = weighingRecord.Kh
                    };
                    SQLDataAccess.SaveCustomer(mainCustomer);
                }

            }
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kh2) && SQLDataAccess.LoadCustomer(weighingRecord.Kh2) == null)
            {
                CustomerModel customer = new CustomerModel
                {
                    Num = DateTime.Now.Ticks.ToString(),
                    Name = weighingRecord.Kh2
                };
                SQLDataAccess.SaveCustomer(customer);
            }
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kh3) && SQLDataAccess.LoadCustomer(weighingRecord.Kh3) == null)
            {
                CustomerModel customer = new CustomerModel
                {
                    Num = DateTime.Now.Ticks.ToString(),
                    Name = weighingRecord.Kh3
                };
                SQLDataAccess.SaveCustomer(customer);
            }

            //如果物资被禁用，提示并返回
            if (SQLDataAccess.LoadDisabledGoods(weighingRecord.Wz) != null)
            {
                ShowLogs("此物资已被禁用，请在【数据管理】中启用此物资后继续使用！");
                return;
            }

            //如果是新的物资，先存新的物资到物资表
            GoodsModel goods = null;
            if (!string.IsNullOrWhiteSpace(weighingRecord.Wz))
            {
                goods = SQLDataAccess.LoadGoods(weighingRecord.Wz);
                if (goods == null)
                {
                    goods = new GoodsModel
                    {
                        Num = DateTime.Now.Ticks.ToString(),
                        Name = weighingRecord.Wz
                    };
                    SQLDataAccess.SaveGoods(goods);
                }
            }

            // 如果不等于0， 则表示是手动输入的 --阿吉 2023年11月15日11点21分
            if (weighingRecord.GoodsPrice == 0)
            {
                weighingRecord.GoodsPrice = GetGoodsPrice(mainCustomer, goods);
            }
            #endregion

            // Reef接口判断 --阿吉 2023年12月9日18点42分
            if (!CheckReefPlatform())
            {
                return;
            }

            #region 保存数据到数据库

            #region 保存数据到数据库 老的逻辑段
            //weighingRecord.Bh = Common.Data.SQLDataAccess.CreateBh(DateTime.Now);
            ////weighingRecord.Bh = DateTime.Today.ToString("yyyyMMdd");
            ////WeighingRecordModel queryBh = SQLDataAccess.LoadWeighingRecord(weighingRecord.Bh);
            ////if (queryBh != null)
            ////{
            ////    int iBh = Convert.ToInt32(queryBh.Bh.Substring(queryBh.Bh.Length - 4, 4)) + 1;
            ////    weighingRecord.Bh += iBh.ToString().PadLeft(4, '0');
            ////}
            ////else
            ////{
            ////    weighingRecord.Bh += "0001";
            ////}


            ////这个判断是为了能保存用户手动输入的值
            //if (string.IsNullOrWhiteSpace(weighingRecord.Mz))
            //    weighingRecord.Mz = WeightStr;


            //////这个判断是为了能保存用户手动输入的值
            ////if (string.IsNullOrWhiteSpace(weighingRecord.Pz))
            ////    weighingRecord.Pz = WeightStr;

            //weighingRecord.Mz = WeightStr;
            //weighingRecord.Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //weighingRecord.Mzsby = Globalspace._currentUser.UserName;

            ////皮重首先由输入车号时带出来，然后如果有手动修改，则取手动修改后的值
            //weighingRecord.Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //weighingRecord.Pzsby = Globalspace._currentUser.UserName;

            ////weighingRecord.Jz = Math.Abs(Convert.ToDecimal(weighingRecord.Mz) - Convert.ToDecimal(weighingRecord.Pz)).ToString();

            ////2022-09-23 要求 毛重、皮重、净重都允许为空，所以加此if判断、
            ////weighingRecord.Jz = Math.Abs(Convert.ToDecimal(weighingRecord.Mz) - Convert.ToDecimal(weighingRecord.Pz)).ToString();
            //if (!string.IsNullOrEmpty(weighingRecord.Mz) && !string.IsNullOrEmpty(weighingRecord.Pz))
            //{
            //    weighingRecord.Jz = Math.Abs(Convert.ToDecimal(weighingRecord.Mz) - Convert.ToDecimal(weighingRecord.Pz)).ToString();
            //}
            //else
            //{
            //    weighingRecord.Jz = "0";
            //}
            //weighingRecord.Jzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            #endregion

            weighingRecord.Bh = CreateBh();// Common.Data.SQLDataAccess.CreateBh(DateTime.Now);

            decimal t_mz = 0;
            decimal t_pz = 0;

            if (!string.IsNullOrEmpty(weighingRecord.Mz) && !decimal.TryParse(weighingRecord.Mz, out t_mz))
            {
                windowManager.ShowMessageBox("毛重请输入正确的数字格式");
                return;
            }
            if (!string.IsNullOrEmpty(weighingRecord.Pz) && !decimal.TryParse(weighingRecord.Pz, out t_pz))
            {
                windowManager.ShowMessageBox("皮重请输入正确的数字格式");
                return;
            }

            if (string.IsNullOrWhiteSpace(weighingRecord.Mz))
                weighingRecord.Mz = WeightStr;

            weighingRecord.Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            weighingRecord.Mzsby = Globalspace._currentUser.UserName;

            //皮重首先由输入车号时带出来，然后如果有手动修改，则取手动修改后的值
            weighingRecord.Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            weighingRecord.Pzsby = Globalspace._currentUser.UserName;

            //这个判断是为了验证，用户或者机器自动生成的 毛重是否小于皮重，小于则互换。不能出现毛重小于皮重的情况
            if (Convert.ToDecimal(weighingRecord.Mz) < Convert.ToDecimal(weighingRecord.Pz))
            {
                string temp = weighingRecord.Mz;
                weighingRecord.Mz = weighingRecord.Pz;
                weighingRecord.Pz = temp;
                if (Convert.ToDecimal(weighingRecord.Mz) == Convert.ToDecimal(WeightStr))
                    weighingRecord.Gblx = "其他";
                else
                    weighingRecord.Gblx = "销售";
            }
            else
            {
                weighingRecord.Gblx = "采购";
            }

            weighingRecord.Jz = Math.Abs(Convert.ToDecimal(weighingRecord.Mz) - Convert.ToDecimal(weighingRecord.Pz)).ToString();
            weighingRecord.Jzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            string discount = _mainSetting.Settings["Discount"].Value;
            if (discount.Equals("1"))
            {
                weighingRecord.Sz = (Convert.ToDecimal(weighingRecord.Jz) - Convert.ToDecimal(weighingRecord.Kz)).ToString();
            }
            else if (discount.Equals("2"))
            {
                weighingRecord.Sz = (Convert.ToDecimal(weighingRecord.Jz) * (100 - Convert.ToDecimal(weighingRecord.Kl)) / 100).ToString();
            }
            else
            {
                weighingRecord.Sz = weighingRecord.Jz;
            }

            //weighingRecord.By3 = NumFormatHelper.NumToChn(weighingRecord.Jz) + "吨";

            //备用字段公式运算
            ComputeByFieldsValue();

            //更新IC卡的备用字段信息
            if (weighingRecord.ICCard != null)
            {
                var ic = SQLDataAccess.LoadICCard(weighingRecord.ICCard);
                if (_mainSetting.Settings["By1Formula"].Value != "") ic.By1 = weighingRecord.By1;
                if (_mainSetting.Settings["By2Formula"].Value != "") ic.By2 = weighingRecord.By2;
                if (_mainSetting.Settings["By3Formula"].Value != "") ic.By3 = weighingRecord.By3;
                if (_mainSetting.Settings["By4Formula"].Value != "") ic.By4 = weighingRecord.By4;
                if (_mainSetting.Settings["By5Formula"].Value != "") ic.By5 = weighingRecord.By5;
                if (_mainSetting.Settings["By6Formula"].Value != "") ic.By6 = weighingRecord.By6;
                if (_mainSetting.Settings["By7Formula"].Value != "") ic.By7 = weighingRecord.By7;
                if (_mainSetting.Settings["By8Formula"].Value != "") ic.By8 = weighingRecord.By8;
                if (_mainSetting.Settings["By9Formula"].Value != "") ic.By9 = weighingRecord.By9;
                if (_mainSetting.Settings["By10Formula"].Value != "") ic.By10 = weighingRecord.By10;
                if (_mainSetting.Settings["By11Formula"].Value != "") ic.By11 = weighingRecord.By11;
                if (_mainSetting.Settings["By12Formula"].Value != "") ic.By12 = weighingRecord.By12;
                if (_mainSetting.Settings["By13Formula"].Value != "") ic.By13 = weighingRecord.By13;
                if (_mainSetting.Settings["By14Formula"].Value != "") ic.By14 = weighingRecord.By14;
                if (_mainSetting.Settings["By15Formula"].Value != "") ic.By15 = weighingRecord.By15;
                if (_mainSetting.Settings["By16Formula"].Value != "") ic.By6 = weighingRecord.By16;
                if (_mainSetting.Settings["By17Formula"].Value != "") ic.By17 = weighingRecord.By17;
                if (_mainSetting.Settings["By18Formula"].Value != "") ic.By18 = weighingRecord.By18;
                if (_mainSetting.Settings["By19Formula"].Value != "") ic.By19 = weighingRecord.By19;
                if (_mainSetting.Settings["By20Formula"].Value != "") ic.By20 = weighingRecord.By20;

                SQLDataAccess.UpdateICCard(ic);
            }

            if (WeightStr == WeightStr1)
            {
                weighingRecord.WeighName = _mainSetting.Settings["WeighName"].Value;
            }
            else if (WeightStr == WeightStr2)
            {
                weighingRecord.WeighName = _mainSetting.Settings["Weigh2Name"].Value;
            }
            weighingRecord.WeighingTimes = 1;
            weighingRecord.IsFinish = true;
            weighingRecord.WeighingFormTemplate = _mainSetting.Settings["PrintTemplate"].Value;

            #endregion

            IPCHelper.SendMsgToApp("ZXWeighingRecord", IPCHelper.UPDATE_RECORD);
            //IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.IS_STABLE, weighingRecord.Mz);

            //输出到文本文档，台帐用
            using (StreamWriter sw = new StreamWriter("./Data/LastWeighInfo.txt", false, Encoding.Default))
            {
                sw.WriteLine(weighingRecord.ToString() + "," + Register.GetHdInfo());
            }

            var overloadLog = _mainSetting.Settings["OverloadLog"].Value;
            if (_mainSetting.Settings["OverloadWarning"].Value == "1") //毛重超载报警
            {
                decimal maxWeight = Convert.ToDecimal(_mainSetting.Settings["OverloadWarningWeight"].Value);
                if (maxWeight > 0)
                {
                    if (Convert.ToDecimal(WeightStr) > maxWeight)
                    {
                        TTSHelper.TTS(weighingRecord, _mainSetting.Settings["OverloadWarningText"].Value);
                        weighingRecord.IsLimit = true;
                        weighingRecord.LimitedValue = maxWeight.ToString();
                        weighingRecord.LimitType = "毛重超载";
                        OverWeight = true;

                        SQLDataAccess.SavOverloadLog(new OverloadLog()
                        {
                            PlateNo = weighingRecord.Ch,
                            AxleCount = weighingRecord.AxleNum,
                            Constraints = "毛重超载",
                            OverloadWeight = WeightStr,
                            StandardWeight = maxWeight.ToString(),
                            Times = "Once",
                            CreateDate = DateTime.Now
                        }, overloadLog);
                    }
                }
                else // =0 时使用轮轴识别
                {
                    switch (weighingRecord.By2)
                    {
                        case "2":
                            if (Convert.ToDecimal(WeightStr) > 18000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 18000;
                            }
                            break;
                        case "3":
                            if (Convert.ToDecimal(WeightStr) > 27000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 27000;
                            }
                            break;
                        case "4":
                            if (Convert.ToDecimal(WeightStr) > 36000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 36000;
                            }
                            break;
                        case "5":
                            if (Convert.ToDecimal(WeightStr) > 43000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 43000;
                            }
                            break;
                        case "6":
                            if (Convert.ToDecimal(WeightStr) > 49000)
                            {
                                OverWeight = true;
                                OverWeightCount = Convert.ToDouble(WeightStr) - 49000;
                            }
                            break;
                        case "7":
                            log.Debug("weighingRecord.By2 Value:" + weighingRecord.By2);
                            break;
                        default:
                            log.Debug("weighingRecord.By2 Value:" + weighingRecord.By2);
                            break;
                    }
                    if (OverWeight)
                    {
                        TTSHelper.TTS(weighingRecord, _mainSetting.Settings["OverloadWarningText"].Value);
                        log.Debug($"轴数：{weighingRecord.By2},已超载");

                        if (_mainSetting.Settings["LED2Enable"].Value == "True")
                        {
                            using (StreamWriter sw = new StreamWriter("./Data/LedText/weighingcomplete.txt", false, Encoding.Default))
                            {
                                if (weighingRecord.Ch != null) sw.WriteLine("车号: " + weighingRecord.Ch);
                                if (weighingRecord.Mz != null) sw.WriteLine("毛重: " + weighingRecord.Mz + _mainSetting.Settings["WeighingUnit"].Value);
                                sw.Write("已超载");
                            }

                            IPCHelper.SendMsgToApp("ZXLED", IPCHelper.WEIGHING_COMPLETE);
                        }

                        setOverLpr(weighingRecord.Mz, OverWeightCount.ToString(), weighingRecord.By2);
                        return;
                    }
                }

            }
            else if (_mainSetting.Settings["OverloadWarning"].Value == "2") //净重超载报警
            {
                var _OverloadWarningWeight = _mainSetting.Settings["OverloadWarningWeight"].Value;
                if (Convert.ToDecimal(weighingRecord.Jz) > Convert.ToDecimal(_OverloadWarningWeight))
                {
                    TTSHelper.TTS(weighingRecord, _mainSetting.Settings["OverloadWarningText"].Value);
                    OverWeight = true;
                    weighingRecord.IsLimit = true;
                    weighingRecord.LimitedValue = _OverloadWarningWeight;
                    weighingRecord.LimitType = "净重超载";

                    SQLDataAccess.SavOverloadLog(new OverloadLog()
                    {
                        PlateNo = weighingRecord.Ch,
                        AxleCount = weighingRecord.AxleNum,
                        Constraints = "净重超载",
                        OverloadWeight = weighingRecord.Jz,
                        StandardWeight = _OverloadWarningWeight,
                        Times = "Once",
                        CreateDate = DateTime.Now
                    }, overloadLog);
                }
            }
            else if (_mainSetting.Settings["OverloadWarning"].Value == "3") //车轴计算超载报警
            {
                var standard = _mainSetting.Settings[$"OverloadAxle{weighingRecord.AxleNum}"]?.Value ?? "0";

                if (Convert.ToDecimal(WeightStr) > Convert.ToDecimal(standard))
                {
                    OverWeight = true;
                    weighingRecord.IsLimit = true;
                    weighingRecord.LimitedValue = standard;
                    weighingRecord.LimitType = "车轴计算超载";

                    SQLDataAccess.SavOverloadLog(new OverloadLog()
                    {
                        PlateNo = weighingRecord.Ch,
                        AxleCount = weighingRecord.AxleNum,
                        Constraints = "车轴计算超载",
                        OverloadWeight = WeightStr,
                        StandardWeight = standard,
                        Times = "Once",
                        CreateDate = DateTime.Now
                    }, overloadLog);
                }

            }

            //----------这里为收费逻辑段  by：汪虎 2022-02-21-------------

            //if (ChargeEnable)//启用收费
            //{

            //    decimal weight = 0;
            //    if (ChargeTypes == "按毛重+皮重收费")//收费模式：0 按毛重+皮重收费，1按净重收费，2 按毛重收费
            //    {
            //        weight = Convert.ToDecimal(weighingRecord.Mz) + Convert.ToDecimal(string.IsNullOrWhiteSpace(weighingRecord.Pz) ? "0" : weighingRecord.Pz);
            //    }
            //    else if (ChargeTypes == "按毛重收费")//收费模式：0 按毛重+皮重收费，1按净重收费，2 按毛重收费
            //    {
            //        weight = Convert.ToDecimal(weighingRecord.Mz);
            //    }
            //    else
            //    {
            //        weight = Convert.ToDecimal(weighingRecord.Jz);
            //    }

            //    //判断是否按物资收费，如果是 那么获取物资收费标准名称用于查询，如果不是，则用默认收费名称，默认名称为：Normal_888888
            //    var feesTitle = ChargeByMaterial ? !string.IsNullOrWhiteSpace(weighingRecord.Wz) ? weighingRecord.Wz.Trim() : "Normal_888888" : "Normal_888888";
            //    var ChargeInfoItem = ChargeInfo.FirstOrDefault(p => p.Name == feesTitle);
            //    if (ChargeInfoItem == null)
            //    {
            //        ChargeInfoItem = ChargeInfo.First(p => p.Name == "普通收费标准");//如果没有查到对应的物资信息，就采用默认收费标准
            //    }
            //    string fee = "0";
            //    if (ChargeInfoItem.Name == "普通收费标准")
            //    {
            //        fee = ToFeesList(ChargeInfoItem.Fees).Where(p => p.ChargeRule.UpperLimit >= weight && p.ChargeRule.LowerLimit <= weight).Max(o => o.Fee);
            //    }
            //    else
            //    {
            //        fee = (weight * ChargeInfoItem.Price).ToString("#.00");
            //    }
            //    //var fee = ToFeesList(ChargeInfoItem.Fees).Where(p => p.ChargeRule.UpperLimit >= weight && p.ChargeRule.LowerLimit <= weight).Max(o => o.Fee);
            //    weighingRecord.Je = string.IsNullOrWhiteSpace(fee) || fee == "0" ? LowestFees : fee;

            //    //TTSHelper.TTS(weighingRecord, ConfigurationManager.AppSettings["OverloadWarningText"]);
            //    if (!String.IsNullOrWhiteSpace(weighingRecord.Je))//表示有收费金额
            //        TTSHelper.TTS(weighingRecord, $"请缴费{weighingRecord.Je}元！");
            //}
            //else
            //{
            //    weighingRecord.Je = "0";
            //}

            weighingRecord.GetCost(0, (msg) => ShowLogs(msg));

            //2022-12-01 此处添加是为了 电子支付那边获取费用而定
            AWSV2.Globalspace.CurrentCost = weighingRecord.Je;

            ShowLogs($"本次费用：{weighingRecord.Je} 元");
            //------------------------------------------------------------
            //2022-11-07增加 LPR 单向模式 下的 显示称重费用
            if (_chargeInfoCfg.ChargeEnable && _mainSetting.Settings["Barrier"].Value == "1")//单向模式
            {
                lprMessage(weighingRecord.Ch, "过磅总费", $"{weighingRecord.Je}元", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), IPCHelper.WEIGHING_COMPLETE);
            }

            weighingRecord.EntryTime = DateTime.Now;//记录称重时间
            Globalspace.CurrentOutPlate = weighingRecord.Ch;

            //在计算费用这里标记为称重完成状态，因为下面的收费框一旦弹出来就是模态的，会阻塞其他用到
            //称重完成状态的子线程的逻辑
            CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;

            if (_chargeInfoCfg.ChargeEnable && _chargeInfoCfg.ChargeWay == "1" && (!_chargeInfoCfg.ChargeStorage || string.IsNullOrWhiteSpace(weighingRecord.Kh)))//0：缴费后出场，1：缴费后下磅
            //if (ChargeEnable && ChargeWay == "1" && !ChargeStorage)
            {
                //默认设置收费方式为线上支付,如果是线下支付，则在弹窗里点击按钮的时候修改
                weighingRecord.IsPay = 1;
                weighingRecord.Zfsj = DateTime.Now;

                if (_chargeInfoCfg.ChargeType != "3")
                {

                    confirmWithChargeView = new ConfirmWithChargeViewModel(weighingRecord);
                    confirmWithChargeView.Parent = this;
                    bool? result = windowManager.ShowDialog(confirmWithChargeView);

                    if (result.GetValueOrDefault(true))
                    {
                        OpenBarrier(false);
                    }
                }
            }
            else if (_chargeInfoCfg.ChargeEnable && _chargeInfoCfg.ChargeStorage)//启用客户储值功能结算
            {
                var mess = GetAmount(weighingRecord, weighingRecord.Kh);
                if (!string.IsNullOrEmpty(mess))//如果是余额不足，那么将通过线下支付或者电子支付的方式继续流程 modify by：2022-10-24
                {
                    //默认设置收费方式为线上支付,如果是线下支付，则在弹窗里点击按钮的时候修改
                    weighingRecord.IsPay = 1;
                    weighingRecord.Zfsj = DateTime.Now;

                    //语音播报余额不足
                    TTSHelper.TTS(weighingRecord, mess);

                    //弹出收费框，强制首费。
                    confirmWithChargeView = new ConfirmWithChargeViewModel(weighingRecord);
                    confirmWithChargeView.Parent = this;
                    bool? result = windowManager.ShowDialog(confirmWithChargeView);
                    if (result.GetValueOrDefault(true))
                    {
                        OpenBarrier();
                    }
                }
                else
                {
                    //设置为储值支付方式
                    weighingRecord.IsPay = 3;
                    weighingRecord.Zfsj = DateTime.Now;
                }
            }
            else
            {
                OpenBarrier();
            }

            //不保存本次称重的数据
            if (NoSave)
            {
                WeighingTimes = 0;
                return;
            }

            //SQLDataAccess.SaveWeighingRecord(weighingRecord);

            if (Convert.ToBoolean(_mainSetting.Settings["TTS1Enable"]?.Value ?? "False") && !OverWeight)
            {
                TTSHelper.TTS(weighingRecord, _mainSetting.Settings["TTS1Text"].Value);
            }

            if (Convert.ToBoolean(_mainSetting.Settings["MonitorEnable"]?.Value ?? "False"))
            {
                IPCHelper.SendMsgToApp("ZXMonitor", "OnceWeighing:" + weighingRecord.Ch + ":" + weighingRecord.Bh);
                CopyVideo(weighingRecord);
            }

            if (_mainSetting.Settings["LPR"].Value == "2")
            {
                CopyImg(weighingRecord);
            }


            //判断扣重、扣率是否都填写了。如果都填写了。需要提示。
            if (!string.IsNullOrWhiteSpace(weighingRecord.Kz) &&
                !string.IsNullOrWhiteSpace(weighingRecord.Kl) &&
                weighingRecord.Kz.Trim() != "0" &&
                 weighingRecord.Kl.Trim() != "0")
            {
                windowManager.ShowMessageBox("扣重、扣率只允许有一个，请处理！");
                return;
            }
            else
            {
                //计算实重多少
                if (!string.IsNullOrWhiteSpace(weighingRecord.Kz) && weighingRecord.Kz.Trim() != "0")
                {
                    double val = 0;
                    double.TryParse(weighingRecord.Kz, out val);
                    // jz-kz=sz
                    weighingRecord.Sz = (double.Parse(weighingRecord.Jz) - val).ToString();
                }
                else if (!string.IsNullOrWhiteSpace(weighingRecord.Kl) && weighingRecord.Kl.Trim() != "0")
                {
                    double val = 0;
                    double.TryParse(weighingRecord.Kl, out val);
                    //  jz*kl=sz
                    var t_jz = double.Parse(weighingRecord.Jz);
                    weighingRecord.Sz = (t_jz - (t_jz) * (val / 100)).ToString();
                }
            }

            weighingRecord.SerialNumber = Common.Data.SQLDataAccess.CreateRecordSerialNumber();

            //2022-09-06 如果是光栅遮挡了，还要保存的，就要将车牌号码后面追加“?”来区分
            if (Globalspace.CurrentBtnContent.Contains("遮挡"))
            {
                weighingRecord.IsCover = true;
            }


            SQLDataAccess.SaveWeighingRecord(weighingRecord);

            ShowLogs($"检测到车牌：{weighingRecord.Ch} 数据保存完成");

            if (_mainSetting.Settings["LED2Enable"].Value == "True")
            {
                var text = _mainSetting.Settings["Leddec"].Value;
                ShowLedContent(text, "./Data/LedText/weighingcomplete.txt", weighingRecord);
                IPCHelper.SendMsgToApp("ZXLED", IPCHelper.WEIGHING_COMPLETE);
            }

            setOverLpr(weighingRecord.Mz, "0", weighingRecord.By2);
            //setWeightComplete(weighingRecord.Mz);
            //2022-11-07增加 LPR 单向模式 下的 显示LED信息
            if (_mainSetting.Settings["Barrier"].Value == "1")//单向模式
            {
                string unit = _mainSetting.Settings["WeighingUnit"].Value;
                unit = string.IsNullOrEmpty(unit) ? "千克" : unit.ToLower() == "kg" ? "千克" : "吨";
                if (string.IsNullOrWhiteSpace(weighingRecord.Pz))
                {
                    lprMessage(weighingRecord.Ch, "称重完成", $"重量:{weighingRecord.Mz}{unit}", "缓慢下磅", IPCHelper.WEIGHING_COMPLETE);
                }
                else
                {
                    lprMessage("称重完成", $"毛重:{weighingRecord.Mz}{unit}", $"皮重:{weighingRecord.Pz}{unit}", $"净重:{weighingRecord.Jz}{unit}", IPCHelper.WEIGHING_COMPLETE);
                }
            }

            WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);

            // 原逻辑 Convert.ToBoolean(ConfigurationManager.AppSettings["SyncDataEnable"]) --阿吉 2023年7月3日12点10分
            // 现在不管这个开关， 直接写数据库 --阿吉 2023年7月3日12点11分
            if (true)
            {
                var SyncData = new SyncDataModel
                {
                    WeighingRecordBh = weighingRecord.Bh,
                    SyncMode = "insert"
                };
                SQLDataAccess.SaveSyncData(SyncData);
                IPCHelper.SendMsgToApp("ZXSyncWeighingData", IPCHelper.SYNC_DATA);
            }
            var copy = JsonConvert.DeserializeObject<WeighingRecordModel>(JsonConvert.SerializeObject(weighingRecord));
            if (Convert.ToBoolean(_mainSetting.Settings["WithPrinting"]?.Value ?? "False"))
            {
                Task.Run(() =>
                {
                    weighingRecord.Dyrq = copy.Dyrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    AWSV2.Services.PrintHelper.PrintImg(copy, worksheet, "OnceWeighing");
                });

            }

            //IPCHelper.SendMsgToApp("ZXLPR", 0x0F0A); //绿灯，道闸不动
            copy.ChargeInfoConfig = weighingRecord.ChargeInfoConfig;
            WeighFormVM = new WeighFormViewModel(copy, SelectedWeighFromTemplateSheet);

            StatusBar = "称重完成";
            WeighingTimes = 0;
            //CurrentStatus = Common.Model.Custom.WeighStatus.WeighEnd;

            SetImg("once", copy.Ch, copy.Bh);

            RemoveICCard(copy);
            QueryList(string.Empty);
            AddWaterMask();
            //UploadData(weighingRecord);
            Task.Run(() =>
            {
                UploadData(copy);
            });

            if (_mainSetting.Settings["TimelyRefresh"].Value == "1")
            {
                RefreshWeighForm();
            }
        }

        /// <summary>
        /// 生成称重编号
        /// </summary>
        /// <returns></returns>
        private string CreateBh()
        {
            var prefix = _mainSetting.Settings["Prefix"].Value;
            var type = _mainSetting.Settings["GenerationType"].Value;
            return Common.Data.SQLDataAccess.CreateBh(DateTime.Now, prefix, type);
        }

        public decimal GetGoodsPrice(CustomerModel customerInfo, GoodsModel goodsInfo)
        {
            var pricesInfo = Common.Data.SQLDataAccess.GetGoodsVsCustomerPriceList(customerInfo?.Id ?? -1, goodsInfo?.Id ?? -1);

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
        private void RemoveICCard(WeighingRecordModel record)
        {
            //判断是否开启称重完成就删除IC卡的开关状态。
            if ("true".Equals((_mainSetting.Settings["SyncYycz"]?.Value ?? string.Empty), StringComparison.OrdinalIgnoreCase))
            {
                Common.Models.ICCardModel card = Common.Data.SQLDataAccess.LoadICCard(weighingRecord.Ch, 1);
                if (card != null)
                {
                    Common.Data.SQLDataAccess.DelICCardById(card);
                }
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

                if (saveralTimes == "lpr")
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

        public void setOverLpr(string oriMz, string oriOverWeight, string oriAxleNum)
        {
            try
            {
                ShareMem MemDB = new ShareMem();
                ShareMem MemDB2 = new ShareMem();
                ShareMem MemDB3 = new ShareMem();
                MemDB.Init("shared_over_mz", 10 * 8);
                MemDB2.Init("shared_over_count", 10 * 8);
                MemDB3.Init("shared_over_axle_num", 10 * 8);
                byte[] mz = System.Text.Encoding.Default.GetBytes(oriMz.PadRight(10));
                byte[] overWeight = System.Text.Encoding.Default.GetBytes(oriOverWeight.PadRight(10));
                byte[] axleNum = System.Text.Encoding.Default.GetBytes(oriAxleNum == null ? "0".PadRight(10) : oriAxleNum.PadRight(10));
                //byte[] MzArr = System.Text.Encoding.Default.GetBytes(weighingRecord.Mz.PadRight(20));
                MemDB.Write(mz, 0, 10);
                MemDB2.Write(overWeight, 0, 10);
                MemDB3.Write(axleNum, 0, 10);
                IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.OVER_WEIGHT);
            }
            catch { }
        }

        public void lprMessage(string mess1, string mess2, string mess3, string mess4, int type, string mess5 = "", string mess6 = "")
        {
            try
            {
                ShareMem MemDB1 = new ShareMem();
                ShareMem MemDB2 = new ShareMem();
                ShareMem MemDB3 = new ShareMem();
                ShareMem MemDB4 = new ShareMem();
                ShareMem MemDB5 = new ShareMem();
                ShareMem MemDB6 = new ShareMem();

                MemDB1.Init("mess1", 20 * 8);
                MemDB2.Init("mess2", 20 * 8);
                MemDB3.Init("mess3", 20 * 8);
                MemDB4.Init("mess4", 20 * 8);
                MemDB5.Init("mess5", 20 * 8);
                MemDB6.Init("mess6", 20 * 8);

                byte[] m1 = System.Text.Encoding.Default.GetBytes(mess1.PadRight(20));
                byte[] m2 = System.Text.Encoding.Default.GetBytes(mess2.PadRight(20));
                byte[] m3 = System.Text.Encoding.Default.GetBytes(mess3.PadRight(20));
                byte[] m4 = System.Text.Encoding.Default.GetBytes(mess4.PadRight(20));
                byte[] m5 = System.Text.Encoding.Default.GetBytes(mess5.PadRight(20));
                byte[] m6 = System.Text.Encoding.Default.GetBytes(mess6.PadRight(20));

                MemDB1.Write(m1, 0, 20);
                MemDB2.Write(m2, 0, 20);
                MemDB3.Write(m3, 0, 20);
                MemDB4.Write(m4, 0, 20);
                MemDB5.Write(m5, 0, 20);
                MemDB6.Write(m6, 0, 20);
                IPCHelper.SendMsgToApp("ZXLPR", type);
            }
            catch { }
        }

        public void setWeightComplete(string oriMz)
        {
            ShareMem MemDB = new ShareMem();
            MemDB.Init("shared_weighing_complete", 10 * 8);
            byte[] mz = System.Text.Encoding.Default.GetBytes(oriMz.PadRight(10));
            MemDB.Write(mz, 0, 10);
            IPCHelper.SendMsgToApp("ZXLPR", IPCHelper.WEIGHING_COMPLETE);
        }

        /// <summary>
        /// 计算剩余充值信息
        /// </summary>
        /// <param name="record">需要修改的称重记录</param>
        /// <param name="custom">客户名称</param>
        private string GetAmount(WeighingRecordModel record, string custom)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(custom)) return string.Empty;

                var obj = Common.Data.SQLDataAccess.GetBalancePay(custom);
                if (!string.IsNullOrWhiteSpace(record.Je))
                {
                    var temp = obj.Amount - (decimal.Parse(record.Je));

                    if (temp <= 0)
                    {
                        record.Credit = (obj.Credit + obj.Amount) - (decimal.Parse(record.Je));
                        record.Amount = 0;
                    }
                    else
                    {
                        record.Credit = obj.Credit;
                        record.Amount = temp;
                    }

                }
                else
                {
                    record.Amount = obj.Amount;
                }

                if (!string.IsNullOrWhiteSpace(record.Jz))
                {
                    record.Weigh = obj.Weigh - (decimal.Parse(record.Jz));
                }
                else
                {
                    record.Weigh = obj.Weigh;
                }

                //if (record.Credit < 0) return "授信额度超限！";
                //if (record.Weigh < 0) return "购买重量超限！";

                if (record.Credit < 0)
                {
                    record.Amount = 0;
                    record.Credit = 0;
                    record.Weigh = 0;
                    return "授信额度超限！";
                }

                if (record.Weigh < 0)
                {
                    record.Amount = 0;
                    record.Credit = 0;
                    record.Weigh = 0;
                    return "购买重量超限！";
                }

                //SQLDataAccess.UpdateWeighingRecord(record);
                var customer = Common.Data.SQLDataAccess.GetCustomer(custom);
                customer.Amount = decimal.Parse(record.Amount.ToString("0.00"));
                customer.Credit = decimal.Parse(record.Credit.ToString("0.00"));
                customer.Weigh = decimal.Parse(record.Weigh.ToString("0.00"));
                Common.Data.SQLDataAccess.UpdateCustomer(customer);
            }
            catch (Exception e) { }

            return string.Empty;
        }

        private void UploadData(WeighingRecordModel record)
        {
            try
            {
                if (Common.Platform.PlatformManager.Instance.IsDefault) return;

                string channelCode = _mainSetting.Settings["platformChannel"]?.Value ?? string.Empty;
                string imgDirectory = string.Empty;
                string videoDirectory = string.Empty;
                Common.Models.WeighingRecordModel obj
                    = AJUtil.TryGetJSONObject<Common.Models.WeighingRecordModel>(AJUtil.AJSerializeObject(record));

                Common.Platform.PlatformManager.Instance.Current.UploadWeighingData(obj, channelCode, imgDirectory, videoDirectory);
            }
            catch (Exception e)
            {
                log.Debug($"上传超载平台出错：{e.Message}");
            }
        }

        public void CloseDialog(bool flag = true)
        {
            try
            {
                //SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));

                //SynchronizationContext.Current.Post(pl =>
                //{
                //    if (confirmWithChargeView.IsActive)
                //    {
                //        confirmWithChargeView.RequestClose(true);
                //    }

                //}, null);

                if (System.Windows.Application.Current != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (confirmWithChargeView.IsActive)
                        {
                            confirmWithChargeView.RequestClose(flag);
                        }
                    });
                }

            }
            catch { }
        }

        /// <summary>
        /// 开关闸门
        /// </summary>
        /// <param name="ovFlag">是否参与超载的逻辑里.第一次称重是不需要参与的。所有第一次称重，这个参数是false</param>
        public void OpenBarrier(bool ovFlag = true)
        {
            //var hasJieGuanZha = (_mainSetting.Settings["JieGuanZha"]?.Value ?? "False") == "True";
            //判断是否启用过载报警
            var olw = _mainSetting.Settings["OverloadWarning"].Value;
            if (!string.IsNullOrWhiteSpace(olw) && olw != "0" && OverWeight && ovFlag)
            {
                var OverloadChannel = _mainSetting.Settings["OverloadChannel"].Value;
                var OverloadAction = _mainSetting.Settings["OverloadAction"].Value;
                if (OverloadAction == "1")//如果设置了，超载后不开启任何闸道
                {
                    return;
                }
                else//如果设置的是超载后开启后面的闸道
                {
                    OverloadChannel = OverloadChannel.ToUpper();
                    if (OverloadChannel == "A")
                    {
                        IPCHelper.SendMsgToApp("ZXLPR", 0x08FE); //1100 -> 1110
                        log.Debug("发送命令：超载后A道闸起");
                        ShowLogs($"道闸A起杆");
                    }
                    if (OverloadChannel == "B")
                    {
                        IPCHelper.SendMsgToApp("ZXLPR", 0x08FD); //1100 -> 1101
                        log.Debug("发送命令：超载后B道闸起");
                        ShowLogs($"道闸B起杆");
                    }
                }

            }
            else
            {
                if (_mainSetting.Settings["Barrier"].Value != "0")
                {

                    //如果是手动的，全部的闸道都抬起来。
                    if (Globalspace._isManual)
                    {
                        IPCHelper.SendMsgToApp("ZXLPR", 0x08F3);
                        Globalspace._isManual = false;
                        //LightByType();
                        //if (!hasJieGuanZha)
                        //{
                        //    Thread.Sleep(500);
                        //    IPCHelper.SendMsgToApp("ZXLPR", 0X08FC);
                        //}
                        return;
                    }

                    //如果设定的开启闸道B，那么就始终执行B的开启动作，其他的不会执行
                    if (_mainSetting.Settings["OpenBarrierB"].Value != "0")
                    {
                        IPCHelper.SendMsgToApp("ZXLPR", 0x08FD); //1100 -> 1101 13
                        log.Debug("发送命令：B出道闸起");
                        ShowLogs($"道闸B起杆");
                        //if (!hasJieGuanZha)
                        //{
                        //    Thread.Sleep(500);
                        //    IPCHelper.SendMsgToApp("ZXLPR", 0X08FC);
                        //}
                    }
                    else
                    {
                        if (Globalspace._lprDevNo == "A")
                        {
                            IPCHelper.SendMsgToApp("ZXLPR", 0x08FE); //1100 -> 1110 14
                            log.Debug("发送命令：A出道闸起");
                            ShowLogs($"道闸A起杆");
                            //if (!hasJieGuanZha)
                            //{
                            //    Thread.Sleep(500);
                            //    IPCHelper.SendMsgToApp("ZXLPR", 0X08FC);
                            //}
                        }
                        if (Globalspace._lprDevNo == "B")
                        {
                            IPCHelper.SendMsgToApp("ZXLPR", 0x08FD); //1100 -> 1101 13
                            log.Debug("发送命令：B出道闸起");
                            ShowLogs($"道闸B起杆");
                            //if (!hasJieGuanZha)
                            //{
                            //   Thread.Sleep(500);
                            //   IPCHelper.SendMsgToApp("ZXLPR", 0X08FC);
                            //}
                        }
                    }
                }

            }
            //LightByType();
        }

        public void OpenBarrier(string channel)
        {
            if (channel == "A")
            {
                IPCHelper.SendMsgToApp("ZXLPR", 0x08FE); //1100 -> 1110
                log.Debug("发送命令：A出道闸起");
            }
            if (channel == "B")
            {
                IPCHelper.SendMsgToApp("ZXLPR", 0x08FD); //1100 -> 1101
                log.Debug("发送命令：B出道闸起");
            }
            //LightByType();
        }

        private void CopyImg(WeighingRecordModel weighingRecord)
        {
            //if (ConfigurationManager.AppSettings["LPR"] == "2")
            {
                try
                {

                    var lprConfig = SettingsHelper.ZXLPRSettings;
                    var lprPath = lprConfig.Settings["LPRSavePath"].Value;
                    string basePath = $"{lprPath}\\{weighingRecord.Ch}";
                    string root = $"{basePath}\\{weighingRecord.Bh}";

                    if (!Directory.Exists(root)) Directory.CreateDirectory(root);


                    if (!string.IsNullOrWhiteSpace(Globalspace._lprImgPath))
                    {

                        if (File.Exists(Globalspace._lprImgPath))
                        {
                            string fileName = System.IO.Path.GetFileName(Globalspace._lprImgPath);

                            File.Copy(Globalspace._lprImgPath, $"{root}\\{fileName}");

                            File.Delete(Globalspace._lprImgPath);

                            Globalspace._lprImgPath = string.Empty;
                        }

                        //把图片根目录车号文件夹下的所有图片都拷贝到编号文件夹里
                        var imgs = Directory.GetFiles(basePath).Where(p => System.IO.Path.GetExtension(p).ToLower() == ".jpg");//获取所有图片
                        foreach (var img in imgs)
                        {
                            string fileName = $"EWD_{System.IO.Path.GetFileName(img)}";
                            var target = $"{root}\\{fileName}";
                            File.Copy(img, target);
                            File.Delete(img);

                        }

                    }

                    var files = Directory.GetFiles(root);

                    if (files != null && files.Length > 0)
                    {
                        foreach (var file in files)
                        {
                            if (!Path.GetExtension(file).ToLower().Contains("jpg")
                                || Path.GetFileName(file).Contains("marked"))
                            {
                                continue;
                            }
                            _snapWatermarkConfig.ProcessSnapWatermark(file,
                                weighingRecord.Ch,
                                weighingRecord.Mz,
                                weighingRecord.Pz,
                                weighingRecord.Jz,
                                weighingRecord.WeighName);
                        }
                    }

                }
                catch (Exception e)
                {
                    log.Debug(e.Message);
                }
            }
        }

        private void CopyVideo(WeighingRecordModel weighingRecord)
        {

            try
            {
                var lprConfig = SettingsHelper.ZXMonitorSettings;
                var lprPath = lprConfig.Settings["MonitorSavePath"].Value;
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



        private void AddWaterMask()
        {
            try
            {
                var picFloder = Path.Combine(Environment.CurrentDirectory, "Snap", weighingRecord.Bh);
                if (Directory.Exists(picFloder))
                {
                    var images = Directory.GetFiles(picFloder).Where(s => s.EndsWith(".jpg") || s.EndsWith(".bmp"));
                    foreach (var i in images)
                    {
                        Bitmap bitmap = new Bitmap(i);
                        Graphics g = Graphics.FromImage(bitmap);
                        System.Drawing.Font font = new System.Drawing.Font("微软雅黑", 24.0f);
                        SolidBrush sbrush = new SolidBrush(System.Drawing.Color.Red);
                        int x = 50;
                        int y = bitmap.Height - 200;
                        string label = "毛重： " + weighingRecord.Mz;
                        g.DrawString(label, font, sbrush, x, y);
                        label = "皮重： " + weighingRecord.Pz;
                        g.DrawString(label, font, sbrush, x, y + 40);
                        label = "净重： " + weighingRecord.Jz;
                        g.DrawString(label, font, sbrush, x, y + 80);
                        label = "车号： " + weighingRecord.Ch;
                        g.DrawString(label, font, sbrush, x, y - 40);

                        bitmap.Save(i.Substring(0, i.Length - 4) + "_mask.jpg");
                        bitmap.Dispose();
                        g.Dispose();
                        File.Delete(i);
                    }
                }
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }
        }

        //当有2个仪表接入时，选择其中一个进行数据处理
        public void SelectDevice(string deviceNo)
        {
            if (deviceNo.Equals("1"))
            {
                SelectedDevice2 = false;
            }
            if (deviceNo.Equals("2"))
            {
                SelectedDevice1 = false;
            }
        }

        public void ShowWeighingRecordWindow()
        {
            IPCHelper.ActiveApp("ZXWeighingRecord");
        }

        public void ShowDataManagementWindow()
        {
            IPCHelper.ActiveApp("ZXDataManagement");
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
                var viewModel = new SettingViewModel(windowManager, validatorAdapter, ref _mobileConfigurationMgr);
                windowManager.ShowDialog(viewModel);

                //修改配置后重载配置
                RefreshAWSV2Setting(true);
                LoadConfig(false);

                //如果重新设计了磅单，需要重新绘制到页面和加载到内存
                WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
                worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[0];

                //更新前端的 称重单位、地磅名称
                //WeighingUnit= ConfigurationManager.AppSettings["WeighingUnit"];
                //WeighName1 = ConfigurationManager.AppSettings["WeighName"];
                var WeighingUnit_1 = WeighingUnit;
                var WeighName1_1 = WeighName1;
                WeighingUnit = "";
                WeighName1 = "";
                WeighingUnit = WeighingUnit_1;
                WeighName1 = WeighName1_1;
            }
            catch (Exception e)
            {
                windowManager.ShowMessageBox(e.Message);
            }
        }

        //public void ShowSimpleSettingWindow()
        //{
        //    try
        //    {
        //        //加载字段验证功能后打开设置窗口
        //        var validator = new SimpleSettingViewModelValidator();
        //        var validatorAdapter = new FluentModelValidator<SimpleSettingViewModel>(validator);
        //        var viewModel = new SimpleSettingViewModel(windowManager, validatorAdapter);
        //        windowManager.ShowDialog(viewModel);

        //        //修改配置后重载配置
        //        RefreshAWSV2Setting(true);
        //        LoadConfig();

        //        //如果重新设计了磅单，需要重新绘制到页面和加载到内存
        //        WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
        //        worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[0];

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
        //        windowManager.ShowMessageBox(e.Message);
        //    }
        //}

        public void ShowRegInfo()
        {
            bool flag = Globalspace._isRegister;
            var viewModel = new QrCodeViewModel();
            bool? result = windowManager.ShowDialog(viewModel);
            //等二维码关闭后再验证一次，防止用户通过二维码注册后，不能即时感知
            //还必须是在联网的状态下才能即时感知，否则只能认为注册失败
            if (Common.Encrt.WebApi.Ping())
            {
                Globalspace._isRegister = Register.Verify(Register.GetHdInfo(), ref _mobileConfigurationMgr);
            }
            else
            {
                Globalspace._isRegister = false;
            }

            if (flag != Globalspace._isRegister)
            {
                StatusBar = "注册成功，请重新启动程序！";
            }
        }

        public void ShowHelpChm()
        {
            //打开chm文档
            try
            {
                System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\无人值守汽车衡称重系统V2.2 说明书.chm");
            }
            catch { }
        }

        public void ShowSysLog()
        {
            var viewModel = new SysLogViewModel(windowManager);

            windowManager.ShowDialog(viewModel);
        }

        public void ShowAboutAWS()
        {
            var viewModel = new AboutAWSViewModel();

            windowManager.ShowDialog(viewModel);
        }

        public void RefreshWeighForm()
        {
            //清空主界面上的3张图片
            //FirstImg = SecondImg = ThirdImg = string.Empty;

            FirstImg = "/Resources/Img/ce.png";
            SecondImg = "/Resources/Img/cf.png";
            ThirdImg = "/Resources/Img/cf.png";

            //隐藏图片窗口，显示数据统计窗口
            WeighFormViewVisible = Visibility.Hidden;
            DataFormViewVisible = Visibility.Visible;


            //重新加载
            //LoadConfig(false); //2022-11-09 下午，痛苦的发现。不能加这个，加这个会卡死。。。
            weighingRecord = new WeighingRecordModel { ChargeInfoConfig = _chargeInfoCfg };

            WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
            worksheet = new Workbook(AWSV2.Globalspace._weightFormTemplatePath).Worksheets[0];

            AWSV2.Globalspace.CurrentCost = "0";
            AWSV2.Globalspace._lprDevNo = string.Empty;
            AWSV2.Globalspace.CurrentBtnIndex = 3;
            ReadyToWeigh = true;
            // 刷新重新赋值 IsStable, 修复刷新后,再称重问题 --阿吉 2023年10月6日15点09分
            IsStable = false;

            // EnableWeighing = false;

            ////清空窗体数据
            //weighingRecord = new WeighingRecordModel();
            //WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
            //Globalspace._lprDevNo = string.Empty;
            //Globalspace.CurrentBtnIndex = 3;
            //ReadyToWeigh = true;
            //// EnableWeighing = false;


        }

        public void QuickPlate()
        {
            IPCHelper.OpenApp("ZXQuickPlate");
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
        //    WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
        //    worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[0];
        //}

        public string WeighingText { get; set; }
        public string WeighingImg { get; set; }

        public System.Windows.Visibility CzxzVisibility { get; set; }
        public System.Windows.Visibility VersionVisibility { get; set; }

        public void OpenVersion()
        {
            ShowQRCode = System.Windows.Visibility.Hidden;
        }

        public void ShowVersion()
        {
            QRCode = Common.Utility.ImageHelper.CreateCode();
            ShowQRCode = System.Windows.Visibility.Visible;
        }

        public void OpenWeighingSet()
        {
            CzxzVisibility = CzxzVisibility == System.Windows.Visibility.Hidden ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void ShowWeighingSet()
        {
            string mode = _mainSetting.Settings["WeighingControl"]?.Value ?? "Hand";
            WeighingText = mode == "Auto" ? "自动称重" : mode == "Hand" ? "手动称重" : "按钮称重";
            WeighingImg = mode == "Auto" ? "/Resources/Img/xza.png" : mode == "Hand" ? "/Resources/Img/xz.png" : "/Resources/Img/btn.png";
            CzxzVisibility = System.Windows.Visibility.Hidden;
            VersionVisibility = System.Windows.Visibility.Hidden;
        }

        public void ShowSystemWindow()
        {
            try
            {
                var viewModel = new SystemViewModel(windowManager);
                windowManager.ShowWindow(viewModel);
            }
            catch (Exception e)
            {
                windowManager.ShowMessageBox(e.Message);
            }
        }


        public void SwitchWeighingControl(string name)
        {
            _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["WeighingControl"].Value = name;
            _mobileConfigurationMgr.SaveSetting();
            LoadConfig(false);

            //if (name == "Hand")
            //{
            //    config.AppSettings.Settings["WeighingControl"].Value = "Hand";

            //}
            //else if (name == "Auto")
            //{
            //    config.AppSettings.Settings["WeighingControl"].Value = "Auto";

            //}
            CzxzVisibility = System.Windows.Visibility.Hidden;

            WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
            worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[0];
        }

        public void SwitchTemplate(string tempNo)
        {
            if (tempNo == "1")
            {
                SelectedWeighFromTemplateSheet = WeighFromTemplateSheetsName[0];
            }
            if (tempNo == "2")
            {
                SelectedWeighFromTemplateSheet = WeighFromTemplateSheetsName[1];
            }
            if (tempNo == "3")
            {
                SelectedWeighFromTemplateSheet = WeighFromTemplateSheetsName[2];
            }

            _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["PrintTemplate"].Value = tempNo;
            _mobileConfigurationMgr.SaveSetting();

            worksheet = wss[SelectedWeighFromTemplateSheet];
            WeighFormVM = new WeighFormViewModel(weighingRecord, SelectedWeighFromTemplateSheet);
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
                IPCHelper.SendMsgToApp("ZXLPR", 0x08F1);
                //if (!hasJieGuanZha)
                //{
                //    Thread.Sleep(500);
                //    IPCHelper.SendMsgToApp("ZXLPR", 0X08F0);
                //}
            }
            if (s == "up2")
            {
                IPCHelper.SendMsgToApp("ZXLPR", 0x08F2);
                //if (!hasJieGuanZha)
                //{
                //    Thread.Sleep(500);
                //    IPCHelper.SendMsgToApp("ZXLPR", 0X08F0);
                //}
            }
            if (s == "upall")
            {
                IPCHelper.SendMsgToApp("ZXLPR", 0x08F3);
                //if (!hasJieGuanZha)
                //{
                //    Thread.Sleep(500);
                //    IPCHelper.SendMsgToApp("ZXLPR", 0X08F0);
                //}
            }
            if (s == "downall")
            {
                IPCHelper.SendMsgToApp("ZXLPR", 0x08F0);
                LogHelper.GetLogger().Debug("JA Debug 绿灯亮");
                //if (!hasJieGuanZha)
                //{
                //    Thread.Sleep(500);
                //    IPCHelper.SendMsgToApp("ZXLPR", 0X08F0);
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

            WeightStr1 = (WeightStr1.TryGetDecimal() + value).ToString();

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

        public async void Logout()
        {
            //2022-10-27 新增 切换用户之前 要先密码验证。通过才能切换。
            var pwdWindow = new PasswordViewModel(windowManager, 2);
            bool? result = windowManager.ShowDialog(pwdWindow);
            if (!result.HasValue || !result.Value)
            {
                return;
            }

            _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["AutoLogin"].Value = "False";
            _mobileConfigurationMgr.SaveSetting();

            if (WeighSerialPort.IsOpen)
            {
                SerialPortClosing = true;
                while (SerialPortListening) ;
                WeighSerialPort.Close();
                SerialPortClosing = false;
            }
            if (Weigh2SerialPort.IsOpen)
            {
                SerialPortClosing = true;
                while (SerialPortListening) ;
                Weigh2SerialPort.Close();
                SerialPortClosing = false;
            }
            if (LEDSerialPort.IsOpen)
            {
                SerialPortClosing = true;
                while (SerialPortListening) ;
                LEDSerialPort.Close();
                SerialPortClosing = false;
                LEDSerialPortEnable = false;
            }
            //关闭二维码串口
            if (QRPort.IsOpen)
            {
                SerialPortClosing = true;
                while (SerialPortListening) ;
                QRPort.Close();
                SerialPortClosing = false;
            }

            _eventAggregator.Unsubscribe(this);
            _queryListTimer.Stop();
            await _mqttSvc.CloseAsync();
            _mobileConfigurationMgr.Stop();

            var viewModel = new LoginViewModel(windowManager, _eventAggregator);
            this.windowManager.ShowWindow(viewModel);
            this.RequestClose();
        }

        public void CreateColumns()
        {

            var printTemplate = _mainSetting.Settings["PrintTemplate"].Value;

            //根据过磅单初始化显示字段列表
            try
            {
                //打开过磅单模板
                if (printTemplate == "1") Worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[0];
                if (printTemplate == "2") Worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[1];
                if (printTemplate == "3") Worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[2];

                //设置查找区域：有内容的区域
                var range = Worksheet.Cells.MaxDisplayRange;
                string cellRange = range.RefersTo.Split('!')[1].Replace("$", "");
                string startCellName = cellRange.Split(':')[0];
                string endCellName = cellRange.Split(':')[1];
                CellArea area = CellArea.CreateCellArea(startCellName, endCellName);
                //查找规则
                FindOptions opts = new FindOptions();
                opts.LookInType = LookInType.Values;
                opts.LookAtType = LookAtType.Contains;
                opts.SetRange(area);

                Cell cell = null;
                do
                {
                    // Search the cell contain value search within range
                    cell = Worksheet.Cells.Find("_", cell, opts);

                    // If no such cell found, then break the loop
                    if (cell == null)
                        break;

                    string key = cell.Value.ToString();
                    if (_mainSetting.Settings[cell.Value.ToString()] != null && !TemplateFieldDic.ContainsKey(key))
                    {
                        TemplateFieldDic.Add(cell.Value.ToString(), _mainSetting.Settings[cell.Value.ToString()].Value);

                    }
                    //此处删掉DIC中的编号字段，在后面Datatable中再添加，放到第一列
                    if (TemplateFieldDic.ContainsKey("_bh"))
                    {
                        TemplateFieldDic.Remove("_bh");
                    }
                    if (TemplateFieldDic.ContainsKey("_dyrq"))
                    {
                        TemplateFieldDic.Remove("_dyrq");
                    }
                } while (true);
            }
            catch (Exception e)
            {
                StatusBar = e.Message;
            }

            foreach (var item in TemplateFieldDic)
            {
                SelectedItems.Add(item.Value);
            }
        }



        private string FormulaCalc(string byxFormula, ExpressionContext context, IEnumerable<PropertyInfo> @props)
        {
            //备用字段公式计算
            var rst = string.Empty;

            try
            {
                context.Variables["_mz"] = weighingRecord.Mz.TryGetDecimal();
                context.Variables["_pz"] = weighingRecord.Pz.TryGetDecimal();
                context.Variables["_jz"] = weighingRecord.Jz.TryGetDecimal();

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

        /// <summary>
        /// 验证注册码
        /// </summary>
        private void VerifyLicense()
        {
            if (Common.Encrt.WebApi.Ping())
            {
                log.Debug("远程验证注册码");
                //联网验证
                if (!Register.Verify(Common.Utility.HardDiskInfo.SerialNumber, ref _mobileConfigurationMgr))//WEB验证注册码失败
                {
                    //var viewModel = new QrCodeViewModel();
                    //bool? result = windowManager.ShowDialog(viewModel);
                    //VisibilityType.VeryHidden
                    ShowQRCode = System.Windows.Visibility.Visible;
                    QRCode = Common.Utility.ImageHelper.CreateCode();
                    //等二维码关闭后再验证一次，防止用户通过二维码注册后，不能即时感知
                    Globalspace._isRegister = Register.Verify(Common.Utility.HardDiskInfo.SerialNumber, ref _mobileConfigurationMgr);
                }
                else
                {
                    Globalspace._isRegister = true;
                    //DelLogs($"试用期已过请联系服务商 {Globalspace.Provicer.mobile}");
                }
            }
            else
            {
                log.Debug("本地验证注册码");
                //本地验证
                if (!Register.Local_Verify(_mainSetting.Settings["RegCode"].Value))
                {
                    //var viewModel = new QrCodeViewModel();
                    //bool? result = windowManager.ShowDialog(viewModel);
                    ShowQRCode = System.Windows.Visibility.Visible;
                    QRCode = Common.Utility.ImageHelper.CreateCode();
                    //等二维码关闭后再验证一次，防止用户通过二维码注册后，不能即时感知
                    //还必须是在联网的状态下才能即时感知，否则只能认为注册失败
                    if (Common.Encrt.WebApi.Ping())
                    {
                        Globalspace._isRegister = Register.Verify(Common.Utility.HardDiskInfo.SerialNumber, ref _mobileConfigurationMgr);
                    }
                    else
                    {
                        Globalspace._isRegister = false;
                    }
                }
                else
                {
                    Globalspace._isRegister = true;
                    //DelLogs($"试用期已过请联系服务商 {Globalspace.Provicer.mobile}");

                }
            }

            if (Globalspace._isRegister)
            {
                RefreshWeighForm();
            }
        }


        protected override void OnClose()
        {
            BackupHelper.AutoBackup();
            IPCHelper.CloseApp("ZXMonitor");
            IPCHelper.CloseApp("ZXWeighingRecord");
            IPCHelper.CloseApp("ZXDataManagement");
            IPCHelper.CloseApp("ZXSyncWeighingData");
            //IPCHelper.CloseApp("ZXLPR");
            IPCHelper.KillApp("ZXLPR");
            IPCHelper.CloseApp("ZXLED");
            IPCHelper.CloseApp("ZXAxleNo");
            IPCHelper.KillApp("ZXQuickPlate");
            //IPCHelper.KillApp("ZXVirtualWall");
            IPCHelper.CloseApp("ZXVirtualWall");

            //结束电子支付SDK
            CloudLogic.CloudAPI.Shutdown();

            //以下是第三方平台的结束逻辑
            Common.Platform.PlatformManager.Instance.Dispose();
            //逻辑结束

            //保存首页分割控件的位置

            _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["SplitterTopPoint"].Value = SplitterTopPoint.Value.ToString();
            _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings["SplitterBottomPoint"].Value = SplitterBottomPoint.Value.ToString();
            _mobileConfigurationMgr.SaveSetting();

            //关闭App.xaml页面中的 联网检车线程
            App.DisposeSync();

            log.Info("用户 " + Globalspace._currentUser.UserName + "非安全（未备份） 退出系统");
            base.OnClose();
        }

        public void FilterList(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi)
            {
                QueryList(mi.CommandParameter?.ToString());
            }
        }

        public async void WindowCloseCmd()
        {
            //if (AWSV2.Globalspace.windowManager == null) return;
            //2022-10-27 新增 切换用户之前 要先密码验证。通过才能切换。
            var pwdWindow = new PasswordViewModel(AWSV2.Globalspace.windowManager, 1);
            bool? result = AWSV2.Globalspace.windowManager.ShowDialog(pwdWindow);
            if (!result.HasValue || !result.Value)
            {
                return;
            }

            //await _mqttSvc.UpdateApplicationAsync(HardDiskInfo.SerialNumber, false);

            await _mqttSvc.CloseAsync();
            _mobileConfigurationMgr.Stop();

            Application.Current.Shutdown();
            //Close();
        }
    }
}
