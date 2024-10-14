using AWSV2.Models;
using AWSV2.Services;
using FluentValidation;
using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Web.UI.WebControls.Expressions;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.Remoting.Contexts;
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

namespace AWSV2.ViewModels
{
    public class SettingViewModel : Screen
    {
        #region 新的配置调整 --阿吉 2023年11月7日09点24分

        private InstrumentCfg _instrument = new InstrumentCfg();
        private WeightingCfg _weightingCfg = new WeightingCfg();
        private PoundCfg _poundCfg = new PoundCfg();
        private PrintCfg _printCfg = new PrintCfg();

        #endregion

        //加载其他窗口
        private IWindowManager windowManager;
        private static readonly log4net.ILog log = LogHelper.GetLogger();
        #region 页面上的属性
        public IEnumerable<string> SerialPorts { get; set; }
        public IEnumerable<string> WZList { get; set; }

        public IEnumerable<string> BaudRates { get; set; }
        public IEnumerable<string> ProtocolTypes { get; set; }
        public IEnumerable<string> AutoBackupFrequency { get; set; }
        public ObservableCollection<string> PrinterList { get; set; } = new ObservableCollection<string>();
        public IEnumerable<string> Flags { get; set; } = new string[] { "启用", "不启用" };//闸道启用相机控制选择项
        public IEnumerable<string> ChargeModles { get; set; }//闸道启用相机控制选择项
        public bool EnableWeighFormTemplate { get; set; } = false; //权限管理：允许修改称重表单
        public bool EnableBackup { get; set; } = false; //权限管理：允许进行备份还原

        //各收费标准信息（包含：普通收费 id=Normal_888888，物资收费6种方式）
        public List<ChargeInfoModel> ChargeInfo { get; set; } = new List<ChargeInfoModel>();
        public ChargeInfoModel NormalInfo { get; set; } = new ChargeInfoModel();

        public IEnumerable<string> GoodsList { get; set; }

        #region 各种属性

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

        private GratingTTSConfig _gratingTTSConfig;
        /// <summary>
        /// 光栅TTS配置 --阿吉 -- 2023年10月13日17点07分
        /// </summary>
        public GratingTTSConfig GratingTTSConfig
        {
            get
            {
                return _gratingTTSConfig;
            }
            set
            {
                SetAndNotify(ref _gratingTTSConfig, value);
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

                GenerationTypeTips = $"预览效果：{Common.Data.SQLDataAccess.CreateBh(DateTime.Now, value, SelectedGenerationType)}";
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
                GenerationTypeTips = $"预览效果：{Common.Data.SQLDataAccess.CreateBh(DateTime.Now, SelectedPrefix, value)}";
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
                _mainConfig.Settings["Printer"].Value = value;
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

        private ObservableCollection<string> selectedPrinter;
        public ObservableCollection<string> SelectedPrinter
        {
            get
            {
                if (selectedPrinter == null)
                {
                    if (Printer.Length > 0)
                    {
                        selectedPrinter = new ObservableCollection<string>(new List<string>(Printer.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)));
                    }
                    else
                    {
                        PrinterSettings s = new PrinterSettings();

                        selectedPrinter = new ObservableCollection<string>() { s.PrinterName };
                    }

                    Printer = WriteSelectedPrinterString(selectedPrinter);
                    selectedPrinter.CollectionChanged +=
                        (s, e) =>
                        {
                            Printer = WriteSelectedPrinterString(selectedPrinter);
                        };
                }
                return selectedPrinter;
            }
            set
            {
                selectedPrinter = value;
            }
        }

        private static string WriteSelectedPrinterString(IList<string> list)
        {
            if (list.Count == 0)
                return String.Empty;

            StringBuilder builder = new StringBuilder(list[0]);

            for (int i = 1; i < list.Count; i++)
            {
                builder.Append(", ");
                builder.Append(list[i]);
            }

            return builder.ToString();
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
                //if (value == "Hand") LPR = "0";
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
                Common.SyncData.SetMinSlotWeight(value);
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

                _mainConfig.Settings["LPR"].Value = value;
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
                else if (barrier == "2")//双向道闸模式
                {
                    CameraLEDMode = "1";//LPR自带的LED的为双向模式
                }
                //   新增逻辑结束

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
                _monitorSection.Settings["MonitorCaptureEnable"].Value = value.ToString();
                CaptureIcon = value ? Visibility.Visible : Visibility.Hidden;
                //_mainConfig.Settings["MonitorCaptureEnable"].Value = value.ToString();
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
        private bool tts0Enable;
        private bool tts1Enable;
        private bool tts2Enable;
        private bool tts3Enable;
        private string tts0Text;
        private string tts1Text;
        private string tts2Text;
        private string tts3Text;
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
        public bool TTS0Enable
        {
            get { return tts0Enable; }
            set
            {
                tts0Enable = value;

                _mainConfig.Settings["TTS0Enable"].Value = value.ToString();
                Log("语音-识别到车牌时", value);
            }
        }
        public bool TTS1Enable
        {
            get { return tts1Enable; }
            set
            {
                tts1Enable = value;

                _mainConfig.Settings["TTS1Enable"].Value = value.ToString();
                Log("语音-第一次称重完成时", value);
            }
        }
        public bool TTS2Enable
        {
            get { return tts2Enable; }
            set
            {
                tts2Enable = value;

                _mainConfig.Settings["TTS2Enable"].Value = value.ToString();
                Log("语音-第二次称重完成时", value);
            }
        }
        public bool TTS3Enable
        {
            get { return tts3Enable; }
            set
            {
                tts3Enable = value;

                _mainConfig.Settings["TTS3Enable"].Value = value.ToString();
                Log("语音-重量稳定时", value);
            }
        }
        public string TTS0Text
        {
            get { return tts0Text; }
            set
            {
                tts0Text = value;

                if (ValidateProperty("TTS0Text"))
                {
                    _mainConfig.Settings["TTS0Text"].Value = value;
                }
            }
        }
        public string TTS1Text
        {
            get { return tts1Text; }
            set
            {
                tts1Text = value;

                if (ValidateProperty("TTS1Text"))
                {
                    _mainConfig.Settings["TTS1Text"].Value = value;
                }
            }
        }
        public string TTS2Text
        {
            get { return tts2Text; }
            set
            {
                tts2Text = value;

                if (ValidateProperty("TTS2Text"))
                {
                    _mainConfig.Settings["TTS2Text"].Value = value;
                }
            }
        }
        public string TTS3Text
        {
            get { return tts3Text; }
            set
            {
                tts3Text = value;

                if (ValidateProperty("TTS3Text"))
                {
                    _mainConfig.Settings["TTS3Text"].Value = value;
                }
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
                    _monitorSection.Settings["MonitorSavePath"].Value = value;
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
                    _lprSection.Settings["LPRSavePath"].Value = value;
                    Log("图片抓拍路径", value);
                }
            }
        }

        /// <summary>
        /// 2022-11-25 新增。相机的控制器类型集合
        /// </summary>
        private List<string> controllers = CarIdentificationCfg.ControllerOptions;
        public List<string> Controllers
        {
            get { return controllers; }
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
                _lprSection.Settings["SelectedController"].Value = value;
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


        private string camera1IP;
        public string Camera1IP
        {
            get { return camera1IP; }
            set
            {
                camera1IP = value;
                _lprSection.Settings["Camera1IP"].Value = value;
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
                _lprSection.Settings["Camera2IP"].Value = value;
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
                _lprSection.Settings["Camera1Username"].Value = value;
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
                _lprSection.Settings["Camera2Username"].Value = value;
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
                _lprSection.Settings["Camera1Password"].Value = value;
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
                _lprSection.Settings["Camera2Password"].Value = value;
                Log("相机2密码", value);
            }
        }

        private bool camera1Enable;
        public bool Camera1Enable
        {
            get { return Convert.ToBoolean(camera1Enable); }
            set
            {
                camera1Enable = Convert.ToBoolean(value);
                _lprSection.Settings["Camera1Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool camera2Enable;
        public bool Camera2Enable
        {
            get { return Convert.ToBoolean(camera2Enable); }
            set
            {
                camera2Enable = Convert.ToBoolean(value);
                _lprSection.Settings["Camera2Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool camera1LEDEnable;
        public bool Camera1LEDEnable
        {
            get { return Convert.ToBoolean(camera1LEDEnable); }
            set
            {
                camera1LEDEnable = Convert.ToBoolean(value);
                _lprSection.Settings["Camera1LEDEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool camera2LEDEnable;
        public bool Camera2LEDEnable
        {
            get { return Convert.ToBoolean(camera2LEDEnable); }
            set
            {
                camera2LEDEnable = Convert.ToBoolean(value);
                _lprSection.Settings["Camera2LEDEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool camera1GPIO;
        public bool Camera1GPIO
        {
            get { return Convert.ToBoolean(camera1GPIO); }
            set
            {
                camera1GPIO = Convert.ToBoolean(value);
                _lprSection.Settings["Camera1GPIO"].Value = Convert.ToInt32(value).ToString();
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
                _lprSection.Settings["RF1Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool rF2Enable;
        public bool RF2Enable
        {
            get { return Convert.ToBoolean(rF2Enable); }
            set
            {
                rF2Enable = Convert.ToBoolean(value);
                _lprSection.Settings["RF2Enable"].Value = Convert.ToInt32(value).ToString();

            }
        }

        private string rF1Uart;
        public string RF1Uart
        {
            get { return rF1Uart; }
            set
            {
                rF1Uart = value;
                _lprSection.Settings["RF1Uart"].Value = value;
            }
        }

        private string rF2Uart;
        public string RF2Uart
        {
            get { return rF2Uart; }
            set
            {
                rF2Uart = value;
                _lprSection.Settings["RF2Uart"].Value = value;
            }
        }


        private string rF3Ip;
        public string RF3Ip
        {
            get { return rF3Ip; }
            set
            {
                rF3Ip = value;
                _lprSection.Settings["RF3IP"].Value = value;
            }
        }

        private string rF4Ip;
        public string RF4Ip
        {
            get { return rF4Ip; }
            set
            {
                rF4Ip = value;
                _lprSection.Settings["RF4IP"].Value = value;
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
                _lprSection.Settings["RelayEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool relay2Enable;
        public bool Relay2Enable
        {
            get { return Convert.ToBoolean(relay2Enable); }
            set
            {
                relay2Enable = Convert.ToBoolean(value);
                _lprSection.Settings["Relay2Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private string relayType = "0";
        public string RelayType
        {
            get
            {
                return relayType;
            }
            set
            {
                relayType = value;
                if (value == "0")
                {
                    //LPRSection.Settings["RelayEnable"].Value = Convert.ToInt32(true).ToString();
                    //LPRSection.Settings["Relay2Enable"].Value = Convert.ToInt32(false).ToString();
                    RelayEnable = true;
                    Relay2Enable = false;
                }
                else
                {
                    //LPRSection.Settings["RelayEnable"].Value = Convert.ToInt32(false).ToString();
                    //LPRSection.Settings["Relay2Enable"].Value = Convert.ToInt32(true).ToString();
                    RelayEnable = false;
                    Relay2Enable = true;
                }
            }
        }

        private string relayUart;
        public string RelayUart
        {
            get { return relayUart; }
            set
            {
                relayUart = value;
                _lprSection.Settings["RelayUart"].Value = value;
            }
        }

        private string cameraLEDMode;
        public string CameraLEDMode
        {
            get { return cameraLEDMode; }
            set
            {
                cameraLEDMode = value;
                _lprSection.Settings["CameraLEDMode"].Value = value;
            }
        }

        private string plate2Enable;
        public string Plate2Enable
        {
            get { return plate2Enable; }
            set
            {
                plate2Enable = value;
                _lprSection.Settings["Plate2Enable"].Value = value == "启用" ? "1" : "0";
            }
        }

        private bool monitor1Enable;
        public bool Monitor1Enable
        {
            get { return Convert.ToBoolean(monitor1Enable); }
            set
            {
                monitor1Enable = Convert.ToBoolean(value);
                _monitorSection.Settings["Monitor1Enable"].Value = Convert.ToInt32(value).ToString();
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
                _monitorSection.Settings["Monitor2Enable"].Value = Convert.ToInt32(value).ToString();
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
                _monitorSection.Settings["Monitor3Enable"].Value = Convert.ToInt32(value).ToString();
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
                _monitorSection.Settings["Monitor4Enable"].Value = Convert.ToInt32(value).ToString();
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
                _monitorSection.Settings["Monitor1IP"].Value = value;
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
                _monitorSection.Settings["Monitor2IP"].Value = value;
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
                _monitorSection.Settings["Monitor3IP"].Value = value;
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
                _monitorSection.Settings["Monitor4IP"].Value = value;
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
                _monitorSection.Settings["Monitor1Username"].Value = value;
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
                _monitorSection.Settings["Monitor2Username"].Value = value;
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
                _monitorSection.Settings["Monitor3Username"].Value = value;
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
                _monitorSection.Settings["Monitor4Username"].Value = value;
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
                _monitorSection.Settings["Monitor1Password"].Value = value;
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
                _monitorSection.Settings["Monitor2Password"].Value = value;
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
                _monitorSection.Settings["Monitor3Password"].Value = value;
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
                _monitorSection.Settings["Monitor4Password"].Value = value;
                Log("影像抓拍-相机4的密码", value);
            }
        }


        private string lEDIP;
        public string LEDIP
        {
            get { return lEDIP; }
            set
            {
                lEDIP = value;
                _ledSection.Settings["LEDIP"].Value = value;
                Log("室外大屏-设备1IP", value);
            }
        }

        private string lED1IP;
        public string LED1IP
        {
            get { return lED1IP; }
            set
            {
                lED1IP = value;
                _ledSection.Settings["LED1IP"].Value = value;
                Log("室外大屏-设备2IP", value);
            }
        }

        private string lEDWIDTH;
        public string LEDWIDTH
        {
            get { return lEDWIDTH; }
            set
            {
                lEDWIDTH = value;
                _ledSection.Settings["LEDWIDTH"].Value = value;
                Log("室外大屏-设备1宽度", value);
            }
        }

        private string lED1WIDTH;
        public string LED1WIDTH
        {
            get { return lED1WIDTH; }
            set
            {
                lED1WIDTH = value;
                _ledSection.Settings["LED1WIDTH"].Value = value;
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
                _ledSection.Settings["LEDHEIGHT"].Value = value;
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
                _ledSection.Settings["LED1HEIGHT"].Value = value;
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
                Log("电子围栏-超出授信车辆禁止出场", value);
            }
        }


        public string Fees1
        {
            get { return NormalInfo.Fees.Fees1.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees1 = paseValue;
                Log("过磅收费-按范围收费-费用1价格", value);
            }
        }

        public string Fees2
        {
            get { return NormalInfo.Fees.Fees2.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees2 = paseValue;
                Log("过磅收费-按范围收费-费用2价格", value);
            }
        }

        public string Fees3
        {
            get { return NormalInfo.Fees.Fees3.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees3 = paseValue;
                Log("过磅收费-按范围收费-费用3价格", value);
            }
        }

        public string Fees4
        {
            get { return NormalInfo.Fees.Fees4.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees4 = paseValue;
                Log("过磅收费-按范围收费-费用4价格", value);
            }
        }

        public string Fees5
        {
            get { return NormalInfo.Fees.Fees5.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees5 = paseValue;
                Log("过磅收费-按范围收费-费用5价格", value);
            }
        }

        public string Fees6
        {
            get { return NormalInfo.Fees.Fees6.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees6 = paseValue;
                Log("过磅收费-按范围收费-费用6价格", value);
            }
        }

        public string Fees7
        {
            get { return NormalInfo.Fees.Fees7.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees7 = paseValue;
                Log("过磅收费-按范围收费-费用7价格", value);
            }
        }

        public string Fees8
        {
            get { return NormalInfo.Fees.Fees8.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees8 = paseValue;
                Log("过磅收费-按范围收费-费用8价格", value);
            }
        }

        public string Fees9
        {
            get { return NormalInfo.Fees.Fees9.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees9 = paseValue;
                Log("过磅收费-按范围收费-费用9价格", value);
            }
        }
        public string Fees20
        {
            get { return NormalInfo.Fees.Fees20.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees20 = paseValue;
                Log("过磅收费-按单位收费-费用1价格", value);
            }
        }
        public string Fees21
        {
            get { return NormalInfo.Fees.Fees21.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees21 = paseValue;
                Log("过磅收费-按单位收费-费用2价格", value);
            }
        }
        public string Fees22
        {
            get { return NormalInfo.Fees.Fees22.ToString(); }
            set
            {
                decimal paseValue = 0;
                decimal.TryParse(value, out paseValue);
                NormalInfo.Fees.Fees22 = paseValue;
                Log("过磅收费-按单位收费-费用3价格", value);
            }
        }

        public string Tonnage1Min
        {
            get { return NormalInfo.Fees.Tonnage1Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage1Min = paseValue;
                Log("过磅收费-按范围收费-第1档起始重量", value);
            }
        }

        public string Tonnage1Max
        {
            get { return NormalInfo.Fees.Tonnage1Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage1Max = paseValue;
                Log("过磅收费-按范围收费-第1档截至重量", value);
            }
        }

        public string Tonnage2Min
        {
            get { return NormalInfo.Fees.Tonnage2Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage2Min = paseValue;
                Log("过磅收费-按范围收费-第2档起始重量", value);
            }
        }

        public string Tonnage2Max
        {
            get { return NormalInfo.Fees.Tonnage2Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage2Max = paseValue;
                Log("过磅收费-按范围收费-第2档截至重量", value);
            }
        }

        public string Tonnage3Min
        {
            get { return NormalInfo.Fees.Tonnage3Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage3Min = paseValue;
                Log("过磅收费-按范围收费-第3档起始重量", value);
            }
        }

        public string Tonnage3Max
        {
            get { return NormalInfo.Fees.Tonnage3Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage3Max = paseValue;
                Log("过磅收费-按范围收费-第3档截至重量", value);
            }
        }

        public string Tonnage4Min
        {
            get { return NormalInfo.Fees.Tonnage4Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage4Min = paseValue;
                Log("过磅收费-按范围收费-第4档起始重量", value);
            }
        }

        public string Tonnage4Max
        {
            get { return NormalInfo.Fees.Tonnage4Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage4Max = paseValue;
                Log("过磅收费-按范围收费-第4档截至重量", value);
            }
        }

        public string Tonnage5Min
        {
            get { return NormalInfo.Fees.Tonnage5Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage5Min = paseValue;
                Log("过磅收费-按范围收费-第5档起始重量", value);
            }
        }

        public string Tonnage5Max
        {
            get { return NormalInfo.Fees.Tonnage5Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage5Max = paseValue;
                Log("过磅收费-按范围收费-第5档截至重量", value);
            }
        }

        public string Tonnage6Min
        {
            get { return NormalInfo.Fees.Tonnage6Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage6Min = paseValue;
                Log("过磅收费-按范围收费-第6档起始重量", value);
            }
        }

        public string Tonnage6Max
        {
            get { return NormalInfo.Fees.Tonnage6Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage6Max = paseValue;
                Log("过磅收费-按范围收费-第6档截至重量", value);
            }
        }

        public string Tonnage7Min
        {
            get { return NormalInfo.Fees.Tonnage7Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage7Min = paseValue;
                Log("过磅收费-按范围收费-第7档起始重量", value);
            }
        }

        public string Tonnage7Max
        {
            get { return NormalInfo.Fees.Tonnage7Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage7Max = paseValue;
                Log("过磅收费-按范围收费-第7档截至重量", value);
            }
        }


        public string Tonnage8Min
        {
            get { return NormalInfo.Fees.Tonnage8Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage8Min = paseValue;
                Log("过磅收费-按范围收费-第8档起始重量", value);
            }
        }

        public string Tonnage8Max
        {
            get { return NormalInfo.Fees.Tonnage8Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage8Max = paseValue;
                Log("过磅收费-按范围收费-第8档截至重量", value);
            }
        }


        public string Tonnage9Min
        {
            get { return NormalInfo.Fees.Tonnage9Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage9Min = paseValue;
                Log("过磅收费-按范围收费-第9档起始重量", value);
            }
        }

        public string Tonnage9Max
        {
            get { return NormalInfo.Fees.Tonnage9Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage9Max = paseValue;
                Log("过磅收费-按范围收费-第9档截至重量", value);
            }
        }

        //以下属性为单位收费配置项
        public string Tonnage20Min
        {
            get { return NormalInfo.Fees.Tonnage20Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage20Min = paseValue;
                Log("过磅收费-按单位收费-第1档起始重量", value);
            }
        }

        public string Tonnage20Max
        {
            get { return NormalInfo.Fees.Tonnage20Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage20Max = paseValue;
                Log("过磅收费-按单位收费-第1档截至重量", value);
            }
        }
        public string Tonnage21Min
        {
            get { return NormalInfo.Fees.Tonnage21Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage21Min = paseValue;
                Log("过磅收费-按单位收费-第2档起始重量", value);
            }
        }

        public string Tonnage21Max
        {
            get { return NormalInfo.Fees.Tonnage21Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage21Max = paseValue;
                Log("过磅收费-按单位收费-第2档截至重量", value);
            }
        }
        public string Tonnage22Min
        {
            get { return NormalInfo.Fees.Tonnage22Min.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage22Min = paseValue;
                Log("过磅收费-按单位收费-第3档起始重量", value);
            }
        }

        public string Tonnage22Max
        {
            get { return NormalInfo.Fees.Tonnage22Max.ToString(); }
            set
            {
                int paseValue = 0;
                int.TryParse(value, out paseValue);
                NormalInfo.Fees.Tonnage22Max = paseValue;
                Log("过磅收费-按单位收费-第3档截至重量", value);
            }
        }
        //结束

        public string goods1
        {
            get { return ChargeInfo.First(p => p.Id == "1").Name; }
            set
            {
                ChargeInfo.First(p => p.Id == "1").Name = value;
            }
        }

        public string goods2
        {
            get { return ChargeInfo.First(p => p.Id == "2").Name; }
            set
            {
                ChargeInfo.First(p => p.Id == "2").Name = value;
            }
        }
        public string goods3
        {
            get { return ChargeInfo.First(p => p.Id == "3").Name; }
            set
            {
                ChargeInfo.First(p => p.Id == "3").Name = value;
            }
        }
        public string goods4
        {
            get { return ChargeInfo.First(p => p.Id == "4").Name; }
            set
            {
                ChargeInfo.First(p => p.Id == "4").Name = value;
            }
        }

        public string goods5
        {
            get { return ChargeInfo.First(p => p.Id == "5").Name; }
            set
            {
                ChargeInfo.First(p => p.Id == "5").Name = value;
            }
        }
        public string goods6
        {
            get { return ChargeInfo.First(p => p.Id == "6").Name; }
            set
            {
                ChargeInfo.First(p => p.Id == "6").Name = value;
            }
        }

        public decimal goodPrice1
        {
            get { return ChargeInfo.First(p => p.Id == "1").Price; }
            set
            {
                ChargeInfo.First(p => p.Id == "1").Price = value;
            }
        }

        public decimal goodPrice2
        {
            get { return ChargeInfo.First(p => p.Id == "2").Price; }
            set
            {
                ChargeInfo.First(p => p.Id == "2").Price = value;
            }
        }
        public decimal goodPrice3
        {
            get { return ChargeInfo.First(p => p.Id == "3").Price; }
            set
            {
                ChargeInfo.First(p => p.Id == "3").Price = value;
            }
        }
        public decimal goodPrice4
        {
            get { return ChargeInfo.First(p => p.Id == "4").Price; }
            set
            {
                ChargeInfo.First(p => p.Id == "4").Price = value;
            }
        }

        public decimal goodPrice5
        {
            get { return ChargeInfo.First(p => p.Id == "5").Price; }
            set
            {
                ChargeInfo.First(p => p.Id == "5").Price = value;
            }
        }
        public decimal goodPrice6
        {
            get { return ChargeInfo.First(p => p.Id == "6").Price; }
            set
            {
                ChargeInfo.First(p => p.Id == "6").Price = value;
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
                Log("电子围栏-是否启用电子围栏", value);
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
                Log("电子围栏-进口1相机", value);
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
                Log("电子围栏-进口1相机用户名", value);
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
                Log("电子围栏-进口1相机密码", value);
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
                Log("电子围栏-进口2相机", value);
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
                Log("电子围栏-进口2相机用户名", value);
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
                Log("电子围栏-进口2相机密码", value);
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
                Log("电子围栏-进口3相机", value);
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
                Log("电子围栏-进口3相机用户名", value);
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

        private string checkGrating;
        public string CheckGrating
        {
            get { return checkGrating; }
            set
            {
                checkGrating = value;
                _mainConfig.Settings["CheckGrating"].Value = value == "启用" ? "1" : "0";
                Log("称重设置-功能设置-光栅防作弊", value);
            }
        }

        private string lPRWhiteList;
        public string LPRWhiteList
        {
            get { return lPRWhiteList; }
            set
            {
                lPRWhiteList = value;
                _mainConfig.Settings["LPRWhiteList"].Value = value == "启用" ? "1" : "0";
                Log("称重设置-功能设置-白名单功能", value);
            }
        }

        private string autoWeighingType;
        public string AutoWeighingType
        {
            get { return autoWeighingType; }
            set
            {
                autoWeighingType = value;
                _mainConfig.Settings["AutoWeighingType"].Value = value == "启用" ? "1" : "0";
                Log("称重设置-功能设置-自动过磅类型", value);
            }
        }

        private string timelyRefresh;
        public string TimelyRefresh
        {
            get { return timelyRefresh; }
            set
            {
                timelyRefresh = value;
                _mainConfig.Settings["TimelyRefresh"].Value = value == "启用" ? "1" : "0";
                Log("称重设置-功能设置-立即刷新磅单", value);
            }
        }

        private string _manualPZOrMZ;
        /// <summary>
        ///  一次过磅人工判断皮毛 --阿吉 2023年10月6日15点50分
        /// </summary>
        public string ManualPZOrMZ
        {
            get { return _manualPZOrMZ; }
            set
            {
                _manualPZOrMZ = value;
                _mainConfig.Settings["ManualPZOrMZ"].Value = value == "启用" ? "True" : "False";
            }
        }

        private string _autoConvertUnit;
        public string AutoConvertUnit
        {
            get { return _autoConvertUnit; }
            set
            {
                _autoConvertUnit = value;
                _mainConfig.Settings["AutoConvertUnit"].Value = value == "启用" ? "True" : "False";
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
                Common.SyncData.set_alarm_status(value);
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
                Common.SyncData.set_pz_weight_alarm_threshold(value);
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

        protected AppSettingsSection _lprSection;

        protected AppSettingsSection _ledSection;

        protected AppSettingsSection _monitorSection;


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
            _lprSection = mobileConfigurationMgr.SettingList[SettingNameKey.LPR];
            _ledSection = mobileConfigurationMgr.SettingList[SettingNameKey.LED];
            _monitorSection = mobileConfigurationMgr.SettingList[SettingNameKey.Monitor];
            _virtualWallSection = mobileConfigurationMgr.SettingList[SettingNameKey.VirtualWall];
            _syncSection = mobileConfigurationMgr.SettingList[SettingNameKey.Sync];

            _instrument.InitPickerSource();
            ProtocolTypes = _instrument.pickerSource[nameof(_instrument.model)];
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

            _printCfg.InitPickerSource();
            PrinterList = new ObservableCollection<string>(_printCfg.pickerSource[nameof(_printCfg.printer)]);

            //加载物资数据
            GoodsList = SQLDataAccess.LoadActiveGoods().Select(p => p.Name);

            GratingTTSConfig = AJUtil.TryGetJSONObject<GratingTTSConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(GratingTTSConfig)]?.Value ?? string.Empty) ?? new GratingTTSConfig();

            FastWeigthCorrectConfig = AJUtil.TryGetJSONObject<FastWeigthCorrectConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(FastWeigthCorrectConfig)]?.Value ?? string.Empty) ?? new FastWeigthCorrectConfig();

            SnapWatermarkConfig = AJUtil.TryGetJSONObject<SnapWatermarkConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(SnapWatermarkConfig)]?.Value ?? string.Empty) ?? new SnapWatermarkConfig();

            AJPrintConfig = AJUtil.TryGetJSONObject<AJPrintConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(AJPrintConfig)]?.Value ?? string.Empty) ?? new AJPrintConfig();

            CleanupConfig = AJUtil.TryGetJSONObject<CleanupConfig>(_mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(CleanupConfig)]?.Value ?? string.Empty) ?? new CleanupConfig();

            JieGuanZha = (_mainConfig.Settings["JieGuanZha"]?.Value ?? "False").Equals("True");

            LoadConfig();
            LoadZXLPRConfig();
            LoadLEDConfig();
            LoadMONITORConfig();
            LoadChargeConfig();
            LoadVirtualWallConfig();
            LoadWeighFormView();
            LoadSyncConfig();

            //检查用户 修改磅单样式、数据备份恢复 权限
            string rolePermission = SQLDataAccess.LoadLoginRolePermission(Globalspace._currentUser.LoginId);
            if (rolePermission != null)
            {
                if (rolePermission.Contains("修改磅单样式")) EnableWeighFormTemplate = true;
                if (rolePermission.Contains("数据备份恢复")) EnableBackup = true;
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

        /// <summary>
        /// 磅单图片源
        /// </summary>
        public System.Windows.Media.ImageSource WeighingFormImg { get; set; }

        private void LoadWeighFormView()
        {
            Task.Run(() =>
            {
                var wss = new Workbook(Globalspace._weightFormTemplatePath).Worksheets;

                //foreach (var ws in wss)
                //{
                //    WeighFromTemplateSheetsName.Add(ws.Name); //用来生成模板列表
                //}
                //SelectedWeighFromTemplateSheet = WeighFromTemplateSheetsName[0];

                SelectedWeighFromTemplateSheet = wss[0].Name;

                var recoed = new WeighingRecordModel();

                recoed.Bh = DateTime.Now.ToString("yyyyMMddHHmmss");
                recoed.Ch = "粤B12345";
                recoed.Mz = "35778";
                recoed.Pz = "20000";
                recoed.Jz = "15778";
                recoed.Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                recoed.Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                recoed.Jzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //WeighFormVM = new WeighFormViewModel1(recoed, SelectedWeighFromTemplateSheet);

                System.Drawing.Bitmap bmp = PrintHelper.PrintView(recoed, wss[0]);// qrCodeEncoder.Encode(content, Encoding.UTF8);
                MemoryStream stream = new MemoryStream();

                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                System.Windows.Media.ImageBrush imgBrush = new System.Windows.Media.ImageBrush();
                System.Windows.Media.ImageSourceConverter isConverter = new System.Windows.Media.ImageSourceConverter();
                WeighingFormImg = (System.Windows.Media.ImageSource)isConverter.ConvertFrom(stream);
            });
        }

        private void LoadZXLPRConfig()
        {
            Camera1IP = _lprSection.Settings["Camera1IP"].Value;
            Camera2IP = _lprSection.Settings["Camera2IP"].Value;
            Camera1Username = _lprSection.Settings["Camera1Username"].Value;
            Camera2Username = _lprSection.Settings["Camera2Username"].Value;
            Camera1Password = _lprSection.Settings["Camera1Password"].Value;
            Camera2Password = _lprSection.Settings["Camera2Password"].Value;
            Camera1Enable = Convert.ToBoolean(Convert.ToInt32(_lprSection.Settings["Camera1Enable"].Value));
            Camera2Enable = Convert.ToBoolean(Convert.ToInt32(_lprSection.Settings["Camera2Enable"].Value));
            Camera1LEDEnable = Convert.ToBoolean(Convert.ToInt32(_lprSection.Settings["Camera1LEDEnable"].Value));
            Camera2LEDEnable = Convert.ToBoolean(Convert.ToInt32(_lprSection.Settings["Camera2LEDEnable"].Value));
            Camera1GPIO = Convert.ToBoolean(Convert.ToInt32(_lprSection.Settings["Camera1GPIO"].Value));
            RF1Enable = Convert.ToBoolean(Convert.ToInt32(_lprSection.Settings["RF1Enable"].Value));
            RF2Enable = Convert.ToBoolean(Convert.ToInt32(_lprSection.Settings["RF2Enable"].Value));
            RF1Uart = _lprSection.Settings["RF1Uart"].Value;
            RF2Uart = _lprSection.Settings["RF2Uart"].Value;
            RF3Ip = _lprSection.Settings["RF3IP"].Value;
            RF4Ip = _lprSection.Settings["RF4IP"].Value;
            //RF1Power = LPRSection.Settings["RF1Power"].Value;
            //RF2Power = LPRSection.Settings["RF2Power"].Value;
            RelayEnable = Convert.ToBoolean(Convert.ToInt32(_lprSection.Settings["RelayEnable"].Value));
            Relay2Enable = Convert.ToBoolean(Convert.ToInt32(_lprSection.Settings["Relay2Enable"].Value));

            RelayType = RelayEnable ? "0" : Relay2Enable ? "1" : "0";

            RelayUart = _lprSection.Settings["RelayUart"].Value;
            Plate2Enable = _lprSection.Settings["Plate2Enable"].Value == "1" ? "启用" : "不启用";
            LPRSavePath = _lprSection.Settings["LPRSavePath"].Value;
            selectedController = _lprSection.Settings["SelectedController"].Value;

            Relay2IP = _lprSection.Settings["Relay2IP"].Value;
            Relay2Mac = _lprSection.Settings["Relay2Mac"].Value;

        }

        private void LoadLEDConfig()
        {
            LEDIP = _ledSection.Settings["LEDIP"].Value;
            LEDWIDTH = _ledSection.Settings["LEDWIDTH"].Value;
            LEDHEIGHT = _ledSection.Settings["LEDHEIGHT"].Value;
            LED1IP = _ledSection.Settings["LED1IP"].Value;
            LED1WIDTH = _ledSection.Settings["LED1WIDTH"].Value;
            LED1HEIGHT = _ledSection.Settings["LED1HEIGHT"].Value;
        }

        private void LoadMONITORConfig()
        {
            Monitor1Enable = Convert.ToBoolean(Convert.ToInt32(_monitorSection.Settings["Monitor1Enable"].Value));
            Monitor2Enable = Convert.ToBoolean(Convert.ToInt32(_monitorSection.Settings["Monitor2Enable"].Value));
            Monitor3Enable = Convert.ToBoolean(Convert.ToInt32(_monitorSection.Settings["Monitor3Enable"].Value));
            Monitor4Enable = Convert.ToBoolean(Convert.ToInt32(_monitorSection.Settings["Monitor4Enable"].Value));
            Monitor1IP = _monitorSection.Settings["Monitor1IP"].Value;
            Monitor2IP = _monitorSection.Settings["Monitor2IP"].Value;
            Monitor3IP = _monitorSection.Settings["Monitor3IP"].Value;
            Monitor4IP = _monitorSection.Settings["Monitor4IP"].Value;
            Monitor1Username = _monitorSection.Settings["Monitor1Username"].Value;
            Monitor2Username = _monitorSection.Settings["Monitor2Username"].Value;
            Monitor3Username = _monitorSection.Settings["Monitor3Username"].Value;
            Monitor4Username = _monitorSection.Settings["Monitor4Username"].Value;
            Monitor1Password = _monitorSection.Settings["Monitor1Password"].Value;
            Monitor2Password = _monitorSection.Settings["Monitor2Password"].Value;
            Monitor3Password = _monitorSection.Settings["Monitor3Password"].Value;
            Monitor4Password = _monitorSection.Settings["Monitor4Password"].Value;
            MonitorSavePath = _monitorSection.Settings["MonitorSavePath"].Value;

            //2022-12-15 add
            MonitorCaptureEnable = Convert.ToBoolean(_monitorSection.Settings["MonitorCaptureEnable"].Value);
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

            //收费信息解析组装
            ChargeInfo = AJUtil.TryGetJSONObject<List<ChargeInfoModel>>(_mainConfig.Settings["ChargeInfo"]?.Value) ?? ChargeInfoModel.Init();

            //获取普通收费的标准
            NormalInfo = ChargeInfo.Find(p => p.Id == "Normal_888888");
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
            SelectedPrefix = ConfigurationManager.AppSettings["Prefix"];
            SelectedGenerationType = ConfigurationManager.AppSettings["GenerationType"];

            GblxCG = ConfigurationManager.AppSettings["GblxCG"];
            GblxXS = ConfigurationManager.AppSettings["GblxXS"];
            GblxQT = ConfigurationManager.AppSettings["GblxQT"];
            CompanyName = ConfigurationManager.AppSettings["CompanyName"];
            Fhdw_mrz = ConfigurationManager.AppSettings["Fhdw_mrz"];

            DI3State = ConfigurationManager.AppSettings["DI3State"];
            DI4State = ConfigurationManager.AppSettings["DI4State"];
            DI5State = ConfigurationManager.AppSettings["DI5State"];
            DI6State = ConfigurationManager.AppSettings["DI6State"];
            DI7State = ConfigurationManager.AppSettings["DI7State"];
            DI8State = ConfigurationManager.AppSettings["DI8State"];

            Byzd_Key1 = ConfigurationManager.AppSettings["Byzd_Key1"];
            Byzd_Key2 = ConfigurationManager.AppSettings["Byzd_Key2"];
            Byzd_Key3 = ConfigurationManager.AppSettings["Byzd_Key3"];
            Byzd_Key4 = ConfigurationManager.AppSettings["Byzd_Key4"];

            Byzd_Value1 = ConfigurationManager.AppSettings["Byzd_Value1"];
            Byzd_Value2 = ConfigurationManager.AppSettings["Byzd_Value2"];
            Byzd_Value3 = ConfigurationManager.AppSettings["Byzd_Value3"];
            Byzd_Value4 = ConfigurationManager.AppSettings["Byzd_Value4"];

            WeighName = ConfigurationManager.AppSettings["WeighName"];
            Weigh2Name = ConfigurationManager.AppSettings["Weigh2Name"];
            WeighProtocolType = ConfigurationManager.AppSettings["WeighProtocolType"];
            WeighSerialPortName = ConfigurationManager.AppSettings["WeighSerialPortName"];
            WeighSerialPortBaudRate = ConfigurationManager.AppSettings["WeighSerialPortBaudRate"];
            EnableSecondDevice = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSecondDevice"]);
            Weigh2ProtocolType = ConfigurationManager.AppSettings["Weigh2ProtocolType"];
            Weigh2SerialPortName = ConfigurationManager.AppSettings["Weigh2SerialPortName"];
            Weigh2SerialPortBaudRate = ConfigurationManager.AppSettings["Weigh2SerialPortBaudRate"];
            WeighFormDisplayMode = ConfigurationManager.AppSettings["WeighFormDisplayMode"];
            WithPrinting = Convert.ToBoolean(ConfigurationManager.AppSettings["WithPrinting"]);
            PrintingMode = ConfigurationManager.AppSettings["PrintingMode"];
            PrintingType = ConfigurationManager.AppSettings["PrintingType"];
            Printer = ConfigurationManager.AppSettings["Printer"];
            PageSizeHeight = ConfigurationManager.AppSettings["PageSizeHeight"];

            Dyrqgs = ConfigurationManager.AppSettings["Dyrqgs"];
            Pzrqgs = ConfigurationManager.AppSettings["Pzrqgs"];
            Mzrqgs = ConfigurationManager.AppSettings["Mzrqgs"];
            Jzrqgs = ConfigurationManager.AppSettings["Jzrqgs"];

            PageSizeWidth = ConfigurationManager.AppSettings["PageSizeWidth"];
            WeighingUnit = ConfigurationManager.AppSettings["WeighingUnit"];
            WeighingMode = ConfigurationManager.AppSettings["WeighingMode"];
            TableRFEnable = Convert.ToBoolean(ConfigurationManager.AppSettings["TableRFEnable"]);
            TableRFPortName = ConfigurationManager.AppSettings["TableRFPortName"];
            QREnable = Convert.ToBoolean(ConfigurationManager.AppSettings["QREnable"]);
            QRPortName = ConfigurationManager.AppSettings["QRPortName"];
            QR2Enable = Convert.ToBoolean(ConfigurationManager.AppSettings["QR2Enable"]);
            QRPort2Name = ConfigurationManager.AppSettings["QRPort2Name"];
            WeighingControl = ConfigurationManager.AppSettings["WeighingControl"];
            StableDelay = ConfigurationManager.AppSettings["StableDelay"];
            MinSlotWeight = ConfigurationManager.AppSettings["MinSlotWeight"];
            Discount = ConfigurationManager.AppSettings["Discount"];
            DiscountWeight = ConfigurationManager.AppSettings["DiscountWeight"];
            DiscountRate = ConfigurationManager.AppSettings["DiscountRate"];
            LPR = ConfigurationManager.AppSettings["LPR"];
            Barrier = ConfigurationManager.AppSettings["Barrier"];
            LightType = ConfigurationManager.AppSettings["LightType"];
            OpenBarrierB = ConfigurationManager.AppSettings["OpenBarrierB"];

            MonitorEnable = Convert.ToBoolean(ConfigurationManager.AppSettings["MonitorEnable"]);

            SyncDataEnable = Convert.ToBoolean(ConfigurationManager.AppSettings["SyncDataEnable"]);
            LEDEnable = Convert.ToBoolean(ConfigurationManager.AppSettings["LEDEnable"]);
            LEDPortName = ConfigurationManager.AppSettings["LEDPortName"];
            LED2Enable = Convert.ToBoolean(ConfigurationManager.AppSettings["LED2Enable"]);
            LED3Enable = Convert.ToBoolean(ConfigurationManager.AppSettings["LED3Enable"]);
            SpeechSpeed = Convert.ToDouble(ConfigurationManager.AppSettings["SpeechSpeed"]);
            TTS0Enable = Convert.ToBoolean(ConfigurationManager.AppSettings["TTS0Enable"]);
            TTS1Enable = Convert.ToBoolean(ConfigurationManager.AppSettings["TTS1Enable"]);
            TTS2Enable = Convert.ToBoolean(ConfigurationManager.AppSettings["TTS2Enable"]);
            TTS3Enable = Convert.ToBoolean(ConfigurationManager.AppSettings["TTS3Enable"]);
            TTS0Text = ConfigurationManager.AppSettings["TTS0Text"];
            TTS1Text = ConfigurationManager.AppSettings["TTS1Text"];
            TTS2Text = ConfigurationManager.AppSettings["TTS2Text"];
            TTS3Text = ConfigurationManager.AppSettings["TTS3Text"];
            OverloadWarning = ConfigurationManager.AppSettings["OverloadWarning"];
            OverloadAction = ConfigurationManager.AppSettings["OverloadAction"];
            OverloadLog = ConfigurationManager.AppSettings["OverloadLog"];

            OverloadAxle2 = ConfigurationManager.AppSettings["OverloadAxle2"];
            OverloadAxle3 = ConfigurationManager.AppSettings["OverloadAxle3"];
            OverloadAxle4 = ConfigurationManager.AppSettings["OverloadAxle4"];
            OverloadAxle5 = ConfigurationManager.AppSettings["OverloadAxle5"];
            OverloadAxle6 = ConfigurationManager.AppSettings["OverloadAxle6"];

            OverloadWarningWeight = ConfigurationManager.AppSettings["OverloadWarningWeight"];
            OverloadWarningText = ConfigurationManager.AppSettings["OverloadWarningText"];
            BackupPath = ConfigurationManager.AppSettings["BackupPath"];
            CurrentAutoBackupFrequency = ConfigurationManager.AppSettings["CurrentAutoBackupFrequency"];

            CheckGrating = ConfigurationManager.AppSettings["CheckGrating"] == "1" ? "启用" : "不启用";
            lPRWhiteList = ConfigurationManager.AppSettings["lPRWhiteList"] == "1" ? "启用" : "不启用";
            AutoWeighingType = ConfigurationManager.AppSettings["AutoWeighingType"] == "1" ? "启用" : "不启用";
            TimelyRefresh = ConfigurationManager.AppSettings["TimelyRefresh"] == "1" ? "启用" : "不启用";

            ManualPZOrMZ = (ConfigurationManager.AppSettings["ManualPZOrMZ"]?.Equals("true", StringComparison.OrdinalIgnoreCase)).GetValueOrDefault()
                ? "启用" : "不启用";

            AutoConvertUnit = ConfigurationManager.AppSettings["AutoConvertUnit"] == "True" ? "启用" : "不启用";
            CancelWeighingTime = ConfigurationManager.AppSettings["CancelWeighingTime"];
            LastPlateTimer = ConfigurationManager.AppSettings["LastPlateTimer"];
            WeighingValidTime = ConfigurationManager.AppSettings["WeighingValidTime"];

            PageCount = ConfigurationManager.AppSettings["PageCount"];

            SyncDataEnable = Convert.ToBoolean(ConfigurationManager.AppSettings["SyncDataEnable"]);
            SyncYycz = Convert.ToBoolean(ConfigurationManager.AppSettings["SyncYycz"]);
            SyncYyzdsh = Convert.ToBoolean(ConfigurationManager.AppSettings["SyncYyzdsh"]);

            LightSwitch = ConfigurationManager.AppSettings["LightSwitch"];

            Ledxsnr = ConfigurationManager.AppSettings["Ledxsnr"];
            Ledsbcps = ConfigurationManager.AppSettings["Ledsbcps"];
            Ledzlwds = ConfigurationManager.AppSettings["Ledzlwds"];
            Leddyc = ConfigurationManager.AppSettings["Leddyc"];
            Leddec = ConfigurationManager.AppSettings["Leddec"];

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
                log.Info($"用户{Globalspace._currentUser.UserName}修改了配置项：{key} 的结果为：{value.ToString()}");
        }

        public void OpenTemplateExeclFile()
        {
            Process.Start(Globalspace._weightFormTemplatePath).WaitForExit();

            //标记此时，不允许记录日志
            canWriteLog = false;
            LoadWeighFormView();
            //标记此后，允许记录日志
            canWriteLog = true;
        }

        public void EditWeighFormList()
        {
            //var viewModel = new WeighFormListViewModel(windowManager, config);
            var viewModel = WeighFormListViewModel.GetInstance(windowManager, ref _mainConfig);
            this.windowManager.ShowWindow(viewModel);
        }

        public void EditByFieldFormula()
        {
            var viewModel = new ByFieldFormulaViewModel(windowManager, ref _mainConfig);
            this.windowManager.ShowWindow(viewModel);
        }

        public void SetBackupPath()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = BackupPath,
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                BackupPath = dialog.FileName;
            }
        }

        public void SetLPRSavePath()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = LPRSavePath,
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LPRSavePath = dialog.FileName;
            }
        }

        public void SetMonitorSavePath()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = MonitorSavePath,
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                MonitorSavePath = dialog.FileName;
            }
        }

        public void ManualBackup()
        {
            try
            {
                BackupHelper.ManualBackup();
                windowManager.ShowMessageBox("数据备份完成！");
            }
            catch (Exception e)
            {
                windowManager.ShowMessageBox(e.Message);
            }

        }

        public void OpenBackupFolder()
        {
            Process.Start("explorer.exe", BackupPath);
        }

        public void DBRestore()
        {
            try
            {
                BackupHelper.DBRestore();
                windowManager.ShowMessageBox("数据恢复完毕！");
            }
            catch (Exception e)
            {
                windowManager.ShowMessageBox(e.Message);
            }
        }

        public void DBClean()
        {
            try
            {
                BackupHelper.DbClean(this.windowManager);
            }
            catch (Exception e)
            {
                windowManager.ShowMessageBox(e.Message);
            }
        }

        public void showEChargeEdit(string type)
        {
            bool? result = windowManager.ShowDialog(new ChargeEditViewModel(this));
        }

        public void showFeeEdit(string type)
        {
            var feeItem = ChargeInfo.First(p => p.Id == type);

            bool? result = windowManager.ShowDialog(new FeesEditViewModel(feeItem.Fees));

            if (result.GetValueOrDefault(true))
            {
                switch (type)
                {
                    case "1":
                        //feeItem.Name = goods1;
                        break;
                    case "2":

                        break;
                    case "3":

                        break;
                    case "4":

                        break;
                    case "5":

                        break;
                    case "6":

                        break;
                }
            }
        }

        public void LPRFuzzyMatching()
        {
            var viewModel = new LPRFuzzyMatchingViewModel();
            this.windowManager.ShowWindow(viewModel);
        }

        public void BackupSettings()
        {
            BackupHelper.BackupSettings();
        }

        public void RestoreSettings()
        {
            BackupHelper.RestoreSettings();
        }

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
                _lprSection.Settings["Relay2IP"].Value = value.Key.ToString();
                _lprSection.Settings["Relay2Mac"].Value = value.Value.ToString();
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
                    _lprSection.Settings["Relay2IP"].Value = value;
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
                    _lprSection.Settings["Relay2Mac"].Value = value;
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
                    _lprSection.Settings["Relay2Mask"].Value = value;
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
                    _lprSection.Settings["Relay2Gateway"].Value = value;
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

        public System.Windows.Visibility ShowEditPanel { get; set; } = System.Windows.Visibility.Hidden;
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
            ShowEditPanel = System.Windows.Visibility.Visible;
        }

        public void HideEdit()
        {
            ShowEditPanel = System.Windows.Visibility.Hidden;
        }

        public void ResetPrinters()
        {
            Printer = string.Empty;
            //var temp = SelectedPrinter;
            selectedPrinter.Clear();
            selectedPrinter = null;
            if (Printer.Length > 0)
            {
                selectedPrinter = new ObservableCollection<string>(new List<string>(Printer.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)));
            }
            else
            {
                PrinterSettings s = new PrinterSettings();

                selectedPrinter = new ObservableCollection<string>() { s.PrinterName };
            }

            Printer = WriteSelectedPrinterString(selectedPrinter);
            selectedPrinter.CollectionChanged +=
                (s, e) =>
                {
                    Printer = WriteSelectedPrinterString(selectedPrinter);
                };
        }

        public void DownloadBarCode()
        {
            try
            {
                string result = Common.SyncData.GetBarCode();
                BarCodeUrl = result;

                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var fileName = $"{desktopPath}\\自助过磅码.jpg";
                Common.SyncData.FileDownload(result, fileName);
                if (string.IsNullOrEmpty(BarCodeUrl))
                {

                    DownloadBarCodeTips = "下载失败请激活软件，或者联系服务商";
                }
                else
                {
                    DownloadBarCodeTips = string.Empty;
                }
            }
            catch (Exception e)
            {
                BarCodeUrl = string.Empty;
            }

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
                    System.Diagnostics.Process.Start(path);
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

        protected override async void OnClose()
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

                _mobileConfigurationMgr.SettingList[SettingNameKey.Main].Settings[nameof(GratingTTSConfig)].Value = AJUtil.AJSerializeObject(new
                {
                    GratingTTSConfig.Enable,
                    GratingTTSConfig.Text
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

                _mainConfig.Settings["ChargeInfo"].Value = AJUtil.AJSerializeObject(ChargeInfo);
                return _mobileConfigurationMgr.SaveSetting();
            };

            var sections = new Dictionary<string, Func<ProcessResult>>
            {
                {"主程序", mainConfigSaveHandler},
                {"LPR程序", () => _mobileConfigurationMgr.SaveSetting(SettingNameKey.LPR) },
                {"LED程序", () => _mobileConfigurationMgr.SaveSetting(SettingNameKey.LED)},
                {"Monitor程序", () => _mobileConfigurationMgr.SaveSetting(SettingNameKey.Monitor)},
                {"VirtualWall程序", () => _mobileConfigurationMgr.SaveSetting(SettingNameKey.VirtualWall)},
                {"Sync程序", () => _mobileConfigurationMgr.SaveSetting(SettingNameKey.Sync)},
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
            else
            {
                await _mobileConfigurationMgr.NotifyConfigUpdatedAsync();
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

            RuleFor(x => x.TTS0Text).NotNull();

            RuleFor(x => x.TTS1Text).NotNull();

            RuleFor(x => x.TTS2Text).NotNull();

            RuleFor(x => x.TTS3Text).NotNull();

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


            RuleFor(x => x.Fees1).Custom((text, context) =>
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

            RuleFor(x => x.Fees2).Custom((text, context) =>
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

            RuleFor(x => x.Fees3).Custom((text, context) =>
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

            RuleFor(x => x.Fees4).Custom((text, context) =>
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

            RuleFor(x => x.Fees5).Custom((text, context) =>
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

            RuleFor(x => x.Fees6).Custom((text, context) =>
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

            RuleFor(x => x.Fees7).Custom((text, context) =>
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

            RuleFor(x => x.Fees8).Custom((text, context) =>
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

            RuleFor(x => x.Fees9).Custom((text, context) =>
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
            RuleFor(x => x.Fees20).Custom((text, context) =>
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
            RuleFor(x => x.Fees21).Custom((text, context) =>
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
            RuleFor(x => x.Fees22).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage1Min).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage2Min).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage3Min).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage4Min).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage5Min).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage6Min).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage7Min).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage8Min).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage9Min).Custom((text, context) =>
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


            RuleFor(x => x.Tonnage20Min).Custom((text, context) =>
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
            RuleFor(x => x.Tonnage21Min).Custom((text, context) =>
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
            RuleFor(x => x.Tonnage22Min).Custom((text, context) =>
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
            RuleFor(x => x.Tonnage1Max).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage2Max).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage3Max).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage4Max).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage5Max).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage6Max).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage7Max).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage8Max).Custom((text, context) =>
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

            RuleFor(x => x.Tonnage9Max).Custom((text, context) =>
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
            RuleFor(x => x.Tonnage20Max).Custom((text, context) =>
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
            RuleFor(x => x.Tonnage21Max).Custom((text, context) =>
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
            RuleFor(x => x.Tonnage22Max).Custom((text, context) =>
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
