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
using Common.Model.ChargeInfo;

namespace AWSV2.ViewModels
{
    public class SimpleSettingViewModel : Screen
    {
        //加载其他窗口
        private IWindowManager windowManager;
        private static readonly log4net.ILog log = LogHelper.GetLogger();
        #region 页面上的属性
        public IEnumerable<string> SerialPorts { get; set; } = SerialPort.GetPortNames();
        public IEnumerable<string> BaudRates { get; set; } = new string[] { "1200", "2400", "4800", "9600" };
        public IEnumerable<string> ProtocolTypes { get; set; } = new string[] { "耀华", "托利多", "金钟", "A27", "昌信", "衡天", "A12" };
        public IEnumerable<string> AutoBackupFrequency { get; set; } = new string[] { "无", "每天", "每周", "每月" };
        public ObservableCollection<string> PrinterList { get; set; } = new ObservableCollection<string>();
        public IEnumerable<string> Flags { get; set; } = new string[] { "启用", "不启用" };//闸道启用相机控制选择项
        public IEnumerable<string> ChargeModles { get; set; } = new string[] { "按毛重+皮重收费", "按毛重收费", "按净重收费" };//闸道启用相机控制选择项
        public bool EnableWeighFormTemplate { get; set; } = false; //权限管理：允许修改称重表单
        public bool EnableBackup { get; set; } = false; //权限管理：允许进行备份还原

        public IEnumerable<string> WZList { get; set; }

        //各收费标准信息（包含：普通收费 id=Normal_888888，物资收费6种方式）
        public List<ChargeInfoModel> ChargeInfo { get; set; } = new List<ChargeInfoModel>();
        public ChargeInfoModel NormalInfo { get; set; } = new ChargeInfoModel();

        public IEnumerable<string> GoodsList { get; set; }

        #region 各种属性

        public string DownloadBarCodeTips { get; set; }

        private string gblxXS;
        public string GblxXS
        {
            get { return gblxXS; }
            set
            {
                gblxXS = value;
                config.AppSettings.Settings["GblxXS"].Value = value;
            }
        }

        private string gblxCG;
        public string GblxCG
        {
            get { return gblxCG; }
            set
            {
                gblxCG = value;
                config.AppSettings.Settings["GblxCG"].Value = value;
            }
        }

        private string gblxQT;
        public string GblxQT
        {
            get { return gblxQT; }
            set
            {
                gblxQT = value;
                config.AppSettings.Settings["GblxQT"].Value = value;
            }
        }

        private string fhdw_mrz;
        public string Fhdw_mrz
        {
            get { return fhdw_mrz; }
            set
            {
                fhdw_mrz = value;
                config.AppSettings.Settings["Fhdw_mrz"].Value = value;
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
                config.AppSettings.Settings["DI3State"].Value = value.Trim();
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
                config.AppSettings.Settings["DI4State"].Value = value.Trim();
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
                config.AppSettings.Settings["DI5State"].Value = value.Trim();
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
                config.AppSettings.Settings["DI6State"].Value = value.Trim();
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
                config.AppSettings.Settings["DI7State"].Value = value.Trim();
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
                config.AppSettings.Settings["DI8State"].Value = value.Trim();
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
                config.AppSettings.Settings["Byzd_Key1"].Value = value;
                CheckByzd();
            }
        }

        private string byzd_Value1;
        public string Byzd_Value1
        {
            get { return byzd_Value1; }
            set
            {
                byzd_Value1 = value;
                config.AppSettings.Settings["Byzd_Value1"].Value = value;
            }
        }

        private string byzd_Key2;
        public string Byzd_Key2
        {
            get { return byzd_Key2; }
            set
            {
                byzd_Key2 = value;
                config.AppSettings.Settings["Byzd_Key2"].Value = value;
                CheckByzd();
            }
        }

        private string byzd_Value2;
        public string Byzd_Value2
        {
            get { return byzd_Value2; }
            set
            {
                byzd_Value2 = value;
                config.AppSettings.Settings["Byzd_Value2"].Value = value;
            }
        }

        private string byzd_Key3;
        public string Byzd_Key3
        {
            get { return byzd_Key3; }
            set
            {
                byzd_Key3 = value;
                config.AppSettings.Settings["Byzd_Key3"].Value = value;
                CheckByzd();
            }
        }

        private string byzd_Value3;
        public string Byzd_Value3
        {
            get { return byzd_Value3; }
            set
            {
                byzd_Value3 = value;
                config.AppSettings.Settings["Byzd_Value3"].Value = value;
            }
        }

        private string byzd_Key4;
        public string Byzd_Key4
        {
            get { return byzd_Key4; }
            set
            {
                byzd_Key4 = value;
                config.AppSettings.Settings["Byzd_Key4"].Value = value;
                CheckByzd();
            }
        }

        private string byzd_Value4;
        public string Byzd_Value4
        {
            get { return byzd_Value4; }
            set
            {
                byzd_Value4 = value;
                config.AppSettings.Settings["Byzd_Value4"].Value = value;
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
                config.AppSettings.Settings["WeighName"].Value = value;
            }
        }
        private string weigh2Name;
        public string Weigh2Name
        {
            get { return weigh2Name; }
            set
            {
                weigh2Name = value;
                config.AppSettings.Settings["Weigh2Name"].Value = value;
            }
        }

        private string weighProtocolType;      //称重仪表协议
        public string WeighProtocolType
        {
            get { return weighProtocolType; }
            set
            {
                weighProtocolType = value;

                config.AppSettings.Settings["WeighProtocolType"].Value = value;
            }
        }

        private string weighSerialPortName;      //称重仪表串口号
        public string WeighSerialPortName
        {
            get { return weighSerialPortName; }
            set
            {
                weighSerialPortName = value;

                config.AppSettings.Settings["WeighSerialPortName"].Value = value;
            }
        }

        private string weighSerialPortBaudRate;      //称重仪表串口波特率
        public string WeighSerialPortBaudRate
        {
            get { return weighSerialPortBaudRate; }
            set
            {
                weighSerialPortBaudRate = value;

                config.AppSettings.Settings["WeighSerialPortBaudRate"].Value = value;
            }
        }

        private bool enableSecondDevice;                  //是否开启副设备
        public bool EnableSecondDevice
        {
            get { return enableSecondDevice; }
            set
            {
                enableSecondDevice = value;

                config.AppSettings.Settings["EnableSecondDevice"].Value = value.ToString();
            }
        }

        private string weigh2ProtocolType;      //称重仪表2协议
        public string Weigh2ProtocolType
        {
            get { return weigh2ProtocolType; }
            set
            {
                weigh2ProtocolType = value;

                config.AppSettings.Settings["Weigh2ProtocolType"].Value = value;
            }
        }

        private string weigh2SerialPortName;      //称重仪表2串口号
        public string Weigh2SerialPortName
        {
            get { return weigh2SerialPortName; }
            set
            {
                weigh2SerialPortName = value;

                config.AppSettings.Settings["Weigh2SerialPortName"].Value = value;
            }
        }

        private string weigh2SerialPortBaudRate;      //称重仪表2串口波特率
        public string Weigh2SerialPortBaudRate
        {
            get { return weigh2SerialPortBaudRate; }
            set
            {
                weigh2SerialPortBaudRate = value;

                config.AppSettings.Settings["Weigh2SerialPortBaudRate"].Value = value;
            }
        }

        private string weighFormDisplayMode;        //称重表单显示模式：List，Preview
        public string WeighFormDisplayMode
        {
            get { return weighFormDisplayMode; }
            set
            {
                weighFormDisplayMode = value;

                config.AppSettings.Settings["WeighFormDisplayMode"].Value = value;
            }
        }


        private bool withPrinting;                  //称重后是否打印
        public bool WithPrinting
        {
            get { return withPrinting; }
            set
            {
                withPrinting = value;

                config.AppSettings.Settings["WithPrinting"].Value = value.ToString();
            }
        }

        private string printingMode;                  //打印模式，立即打印还是先预览再打印
        public string PrintingMode
        {
            get { return printingMode; }
            set
            {
                printingMode = value;
                config.AppSettings.Settings["PrintingMode"].Value = value;
            }
        }

        private string printingType;                  //打印类别，一次过磅打印，二次过磅打印，一次二次都打印。。。。
        public string PrintingType
        {
            get { return printingType; }
            set
            {
                printingType = value;
                config.AppSettings.Settings["PrintingType"].Value = value;
            }
        }


        private string printer;

        public string Printer
        {
            get { return printer; }
            set
            {
                printer = value;
                config.AppSettings.Settings["Printer"].Value = value;
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
                    config.AppSettings.Settings["PageSizeHeight"].Value = value;
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
                    config.AppSettings.Settings["PageSizeWidth"].Value = value;
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
                    config.AppSettings.Settings["Dyrqgs"].Value = value;
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
                    config.AppSettings.Settings["Mzrqgs"].Value = value;
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
                    config.AppSettings.Settings["Pzrqgs"].Value = value;
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
                    config.AppSettings.Settings["Jzrqgs"].Value = value;
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

                config.AppSettings.Settings["WeighingUnit"].Value = value;
            }
        }

        private string weighingMode;                //称重模式 Once / Twice
        public string WeighingMode
        {
            get { return weighingMode; }
            set
            {
                weighingMode = value;

                config.AppSettings.Settings["WeighingMode"].Value = value;
            }
        }

        private string weighingControl;             //称重控制方式 Auto / Hand 
        public string WeighingControl
        {
            get { return weighingControl; }
            set
            {
                weighingControl = value;

                config.AppSettings.Settings["WeighingControl"].Value = value;

                if (value == "Auto") EnableSecondDevice = false;
                //if (value == "Hand") LPR = "0";
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
                    config.AppSettings.Settings["StableDelay"].Value = value;
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
                    config.AppSettings.Settings["MinSlotWeight"].Value = value;
                }
                Common.SyncData.SetMinSlotWeight(value);

            }
        }

        private bool tableRFEnable;             //启用明华桌面读卡器
        public bool TableRFEnable
        {
            get { return tableRFEnable; }
            set
            {
                tableRFEnable = value;
                config.AppSettings.Settings["TableRFEnable"].Value = value.ToString();
            }
        }

        private string tableRFPortName;              //明华桌面读卡器串口号
        public string TableRFPortName
        {
            get { return tableRFPortName; }
            set
            {
                tableRFPortName = value;
                config.AppSettings.Settings["TableRFPortName"].Value = value;
            }
        }

        private bool qrEnable;             //启用二维码扫码器
        public bool QREnable
        {
            get { return qrEnable; }
            set
            {
                qrEnable = value;
                config.AppSettings.Settings["QREnable"].Value = value.ToString();
            }
        }

        private string qrPortName;              //二维码扫码器串口号
        public string QRPortName
        {
            get { return qrPortName; }
            set
            {
                qrPortName = value;
                config.AppSettings.Settings["QRPortName"].Value = value;
            }
        }

        private bool qr2Enable;             //启用二维码扫码器2
        public bool QR2Enable
        {
            get { return qr2Enable; }
            set
            {
                qr2Enable = value;
                config.AppSettings.Settings["QR2Enable"].Value = value.ToString();
            }
        }

        private string qrPort2Name;              //二维码扫码器2串口号
        public string QRPort2Name
        {
            get { return qrPort2Name; }
            set
            {
                qrPort2Name = value;
                config.AppSettings.Settings["QRPort2Name"].Value = value;
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

                config.AppSettings.Settings["Discount"].Value = value;
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
                    config.AppSettings.Settings["DiscountWeight"].Value = value;
                }
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
                    config.AppSettings.Settings["DiscountRate"].Value = value;
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

                config.AppSettings.Settings["LPR"].Value = value;
            }
        }

        private string barrier;

        public string Barrier
        {
            get { return barrier; }
            set
            {
                barrier = value;

                config.AppSettings.Settings["Barrier"].Value = value;
            }
        }

        private string openBarrierB;

        public string OpenBarrierB
        {
            get { return openBarrierB; }
            set
            {
                openBarrierB = value;

                config.AppSettings.Settings["OpenBarrierB"].Value = value;
            }
        }

        private string lightSwitch;

        public string LightSwitch
        {
            get { return lightSwitch; }
            set
            {
                lightSwitch = value;

                config.AppSettings.Settings["LightSwitch"].Value = value;
            }
        }

        private string ledxsnr;

        public string Ledxsnr
        {
            get { return ledxsnr; }
            set
            {
                ledxsnr = value;

                config.AppSettings.Settings["Ledxsnr"].Value = value;
            }
        }

        private string ledsbcps;

        public string Ledsbcps
        {
            get { return ledsbcps; }
            set
            {
                ledsbcps = value;

                config.AppSettings.Settings["Ledsbcps"].Value = value;
            }
        }

        private string ledzlwds;

        public string Ledzlwds
        {
            get { return ledzlwds; }
            set
            {
                ledzlwds = value;

                config.AppSettings.Settings["Ledzlwds"].Value = value;
            }
        }

        private string leddyc;

        public string Leddyc
        {
            get { return leddyc; }
            set
            {
                leddyc = value;

                config.AppSettings.Settings["Leddyc"].Value = value;
            }
        }

        private string leddec;

        public string Leddec
        {
            get { return leddec; }
            set
            {
                leddec = value;

                config.AppSettings.Settings["Leddec"].Value = value;
            }
        }

        private bool monitorEnable;                         //启用监控摄像头
        public bool MonitorEnable
        {
            get { return monitorEnable; }
            set
            {
                monitorEnable = value;

                config.AppSettings.Settings["MonitorEnable"].Value = value.ToString();
            }
        }

        private bool monitorCaptureEnable;                         //启用监控摄像头抓拍
        public bool MonitorCaptureEnable
        {
            get { return monitorCaptureEnable; }
            set
            {
                monitorCaptureEnable = value;
                MONITORSection.Settings["MonitorCaptureEnable"].Value = value.ToString();
                //config.AppSettings.Settings["MonitorCaptureEnable"].Value = value.ToString();
            }
        }

        private bool syncDataEnable;                         //启用数据同步
        public bool SyncDataEnable
        {
            get { return syncDataEnable; }
            set
            {
                syncDataEnable = value;

                config.AppSettings.Settings["SyncDataEnable"].Value = value.ToString();
            }
        }

        private bool syncYycz;                         //数据同步的方式 预约称重
        public bool SyncYycz
        {
            get { return syncYycz; }
            set
            {
                syncYycz = value;

                config.AppSettings.Settings["SyncYycz"].Value = value.ToString();
            }

        }

        private bool syncYyzdsh;                         //数据同步的方式 预约自动审核
        public bool SyncYyzdsh
        {
            get { return syncYyzdsh; }
            set
            {
                syncYyzdsh = value;

                config.AppSettings.Settings["SyncYyzdsh"].Value = value.ToString();
            }
        }

        private bool enableVideo;                         //启用称重过磅
        public bool EnableVideo
        {
            get { return enableVideo; }
            set
            {
                enableVideo = value;

                config.AppSettings.Settings["EnableVideo"].Value = value.ToString();
            }
        }

        private bool ledEnable;                         //启用大屏幕
        public bool LEDEnable
        {
            get { return ledEnable; }
            set
            {
                ledEnable = value;

                config.AppSettings.Settings["LEDEnable"].Value = value.ToString();
            }
        }

        private string ledPortName;               //大屏幕串口
        public string LEDPortName
        {
            get { return ledPortName; }
            set
            {
                ledPortName = value;

                config.AppSettings.Settings["LEDPortName"].Value = value;

            }
        }

        private bool led2Enable;                         //启用大屏幕
        public bool LED2Enable
        {
            get { return led2Enable; }
            set
            {
                led2Enable = value;

                config.AppSettings.Settings["LED2Enable"].Value = value.ToString();
            }
        }

        private bool led3Enable;                         //启用大屏幕
        public bool LED3Enable
        {
            get { return led3Enable; }
            set
            {
                led3Enable = value;

                config.AppSettings.Settings["LED3Enable"].Value = value.ToString();
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

                config.AppSettings.Settings["SpeechSpeed"].Value = value.ToString();
            }
        }
        public bool TTS0Enable
        {
            get { return tts0Enable; }
            set
            {
                tts0Enable = value;

                config.AppSettings.Settings["TTS0Enable"].Value = value.ToString();
            }
        }
        public bool TTS1Enable
        {
            get { return tts1Enable; }
            set
            {
                tts1Enable = value;

                config.AppSettings.Settings["TTS1Enable"].Value = value.ToString();
            }
        }
        public bool TTS2Enable
        {
            get { return tts2Enable; }
            set
            {
                tts2Enable = value;

                config.AppSettings.Settings["TTS2Enable"].Value = value.ToString();
            }
        }
        public bool TTS3Enable
        {
            get { return tts3Enable; }
            set
            {
                tts3Enable = value;

                config.AppSettings.Settings["TTS3Enable"].Value = value.ToString();
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
                    config.AppSettings.Settings["TTS0Text"].Value = value;
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
                    config.AppSettings.Settings["TTS1Text"].Value = value;
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
                    config.AppSettings.Settings["TTS2Text"].Value = value;
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
                    config.AppSettings.Settings["TTS3Text"].Value = value;
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

                config.AppSettings.Settings["OverloadWarning"].Value = value;
            }
        }

        private string overloadAction;             //超载动作 0:开后闸 1:不开闸
        public string OverloadAction
        {
            get { return overloadAction; }
            set
            {
                overloadAction = value;

                config.AppSettings.Settings["OverloadAction"].Value = value;
            }
        }

        private string overloadLog;             //超载动作 0:开后闸 1:不开闸
        public string OverloadLog
        {
            get { return overloadLog; }
            set
            {
                overloadLog = value;

                config.AppSettings.Settings["OverloadLog"].Value = value;
            }
        }

        private string overloadAxle2;
        public string OverloadAxle2
        {
            get { return overloadAxle2; }
            set
            {
                overloadAxle2 = value;
                config.AppSettings.Settings["OverloadAxle2"].Value = value;
            }
        }

        private string overloadAxle3;
        public string OverloadAxle3
        {
            get { return overloadAxle3; }
            set
            {
                overloadAxle3 = value;
                config.AppSettings.Settings["OverloadAxle3"].Value = value;
            }
        }

        private string overloadAxle4;
        public string OverloadAxle4
        {
            get { return overloadAxle4; }
            set
            {
                overloadAxle4 = value;
                config.AppSettings.Settings["OverloadAxle4"].Value = value;
            }
        }

        private string overloadAxle5;
        public string OverloadAxle5
        {
            get { return overloadAxle5; }
            set
            {
                overloadAxle5 = value;
                config.AppSettings.Settings["OverloadAxle5"].Value = value;
            }
        }

        private string overloadAxle6;
        public string OverloadAxle6
        {
            get { return overloadAxle6; }
            set
            {
                overloadAxle6 = value;
                config.AppSettings.Settings["OverloadAxle6"].Value = value;
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
                    config.AppSettings.Settings["OverloadWarningWeight"].Value = value;
                }
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
                    config.AppSettings.Settings["OverloadWarningText"].Value = value;
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
                    config.AppSettings.Settings["BackupPath"].Value = value;
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
                    MONITORSection.Settings["MonitorSavePath"].Value = value;
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
                    LPRSection.Settings["LPRSavePath"].Value = value;
                }
            }
        }

        /// <summary>
        /// 2022-11-25 新增。相机的控制器类型集合
        /// </summary>
        private List<string> controllers = new List<string>() { "道通物联", "方控" };
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
                LPRSection.Settings["SelectedController"].Value = value;
            }
        }

        private string currentAutoBackupFrequency;  //备份频率
        public string CurrentAutoBackupFrequency
        {
            get { return currentAutoBackupFrequency; }
            set
            {
                currentAutoBackupFrequency = value;

                config.AppSettings.Settings["CurrentAutoBackupFrequency"].Value = value;
            }
        }


        private string camera1IP;
        public string Camera1IP
        {
            get { return camera1IP; }
            set
            {
                camera1IP = value;
                LPRSection.Settings["Camera1IP"].Value = value;
            }
        }

        private string camera2IP;
        public string Camera2IP
        {
            get { return camera2IP; }
            set
            {
                camera2IP = value;
                LPRSection.Settings["Camera2IP"].Value = value;
            }
        }



        private string camera1Username;
        public string Camera1Username
        {
            get { return camera1Username; }
            set
            {
                camera1Username = value;
                LPRSection.Settings["Camera1Username"].Value = value;
            }
        }


        private string camera2Username;
        public string Camera2Username
        {
            get { return camera2Username; }
            set
            {
                camera2Username = value;
                LPRSection.Settings["Camera2Username"].Value = value;
            }
        }


        private string camera1Password;
        public string Camera1Password
        {
            get { return camera1Password; }
            set
            {
                camera1Password = value;
                LPRSection.Settings["Camera1Password"].Value = value;
            }
        }

        private string camera2Password;
        public string Camera2Password
        {
            get { return camera2Password; }
            set
            {
                camera2Password = value;
                LPRSection.Settings["Camera2Password"].Value = value;
            }
        }

        private bool camera1Enable;
        public bool Camera1Enable
        {
            get { return Convert.ToBoolean(camera1Enable); }
            set
            {
                camera1Enable = Convert.ToBoolean(value);
                LPRSection.Settings["Camera1Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool camera2Enable;
        public bool Camera2Enable
        {
            get { return Convert.ToBoolean(camera2Enable); }
            set
            {
                camera2Enable = Convert.ToBoolean(value);
                LPRSection.Settings["Camera2Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool camera1LEDEnable;
        public bool Camera1LEDEnable
        {
            get { return Convert.ToBoolean(camera1LEDEnable); }
            set
            {
                camera1LEDEnable = Convert.ToBoolean(value);
                LPRSection.Settings["Camera1LEDEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool camera2LEDEnable;
        public bool Camera2LEDEnable
        {
            get { return Convert.ToBoolean(camera2LEDEnable); }
            set
            {
                camera2LEDEnable = Convert.ToBoolean(value);
                LPRSection.Settings["Camera2LEDEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool camera1GPIO;
        public bool Camera1GPIO
        {
            get { return Convert.ToBoolean(camera1GPIO); }
            set
            {
                camera1GPIO = Convert.ToBoolean(value);
                LPRSection.Settings["Camera1GPIO"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool rF1Enable;
        public bool RF1Enable
        {
            get { return Convert.ToBoolean(rF1Enable); }
            set
            {
                rF1Enable = Convert.ToBoolean(value);
                LPRSection.Settings["RF1Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool rF2Enable;
        public bool RF2Enable
        {
            get { return Convert.ToBoolean(rF2Enable); }
            set
            {
                rF2Enable = Convert.ToBoolean(value);
                LPRSection.Settings["RF2Enable"].Value = Convert.ToInt32(value).ToString();

            }
        }

        private string rF1Uart;
        public string RF1Uart
        {
            get { return rF1Uart; }
            set
            {
                rF1Uart = value;
                LPRSection.Settings["RF1Uart"].Value = value;
            }
        }

        private string rF2Uart;
        public string RF2Uart
        {
            get { return rF2Uart; }
            set
            {
                rF2Uart = value;
                LPRSection.Settings["RF2Uart"].Value = value;
            }
        }


        private string rF3Ip;
        public string RF3Ip
        {
            get { return rF3Ip; }
            set
            {
                rF3Ip = value;
                LPRSection.Settings["RF3IP"].Value = value;
            }
        }

        private string rF4Ip;
        public string RF4Ip
        {
            get { return rF4Ip; }
            set
            {
                rF4Ip = value;
                LPRSection.Settings["RF4IP"].Value = value;
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
                LPRSection.Settings["RelayEnable"].Value = Convert.ToInt32(value).ToString();

            }
        }

        private string relayUart;
        public string RelayUart
        {
            get { return relayUart; }
            set
            {
                relayUart = value;
                LPRSection.Settings["RelayUart"].Value = value;
            }
        }

        private string plate2Enable;
        public string Plate2Enable
        {
            get { return plate2Enable; }
            set
            {
                plate2Enable = value;
                LPRSection.Settings["Plate2Enable"].Value = value == "启用" ? "1" : "0";
            }
        }

        private bool monitor1Enable;
        public bool Monitor1Enable
        {
            get { return Convert.ToBoolean(monitor1Enable); }
            set
            {
                monitor1Enable = Convert.ToBoolean(value);
                MONITORSection.Settings["Monitor1Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool monitor2Enable;
        public bool Monitor2Enable
        {
            get { return Convert.ToBoolean(monitor2Enable); }
            set
            {
                monitor2Enable = Convert.ToBoolean(value);
                MONITORSection.Settings["Monitor2Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool monitor3Enable;
        public bool Monitor3Enable
        {
            get { return Convert.ToBoolean(monitor3Enable); }
            set
            {
                monitor3Enable = Convert.ToBoolean(value);
                MONITORSection.Settings["Monitor3Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }


        private bool monitor4Enable;
        public bool Monitor4Enable
        {
            get { return Convert.ToBoolean(monitor4Enable); }
            set
            {
                monitor4Enable = Convert.ToBoolean(value);
                MONITORSection.Settings["Monitor4Enable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private string monitor1IP;
        public string Monitor1IP
        {
            get { return monitor1IP; }
            set
            {
                monitor1IP = value;
                MONITORSection.Settings["Monitor1IP"].Value = value;
            }
        }

        private string monitor2IP;
        public string Monitor2IP
        {
            get { return monitor2IP; }
            set
            {
                monitor2IP = value;
                MONITORSection.Settings["Monitor2IP"].Value = value;
            }
        }

        private string monitor3IP;
        public string Monitor3IP
        {
            get { return monitor3IP; }
            set
            {
                monitor3IP = value;
                MONITORSection.Settings["Monitor3IP"].Value = value;
            }
        }


        private string monitor4IP;
        public string Monitor4IP
        {
            get { return monitor4IP; }
            set
            {
                monitor4IP = value;
                MONITORSection.Settings["Monitor4IP"].Value = value;
            }
        }


        private string monitor1Username;
        public string Monitor1Username
        {
            get { return monitor1Username; }
            set
            {
                monitor1Username = value;
                MONITORSection.Settings["Monitor1Username"].Value = value;
            }
        }


        private string monitor2Username;
        public string Monitor2Username
        {
            get { return monitor2Username; }
            set
            {
                monitor2Username = value;
                MONITORSection.Settings["Monitor2Username"].Value = value;
            }
        }


        private string monitor3Username;
        public string Monitor3Username
        {
            get { return monitor3Username; }
            set
            {
                monitor3Username = value;
                MONITORSection.Settings["Monitor3Username"].Value = value;
            }
        }

        private string monitor4Username;
        public string Monitor4Username
        {
            get { return monitor4Username; }
            set
            {
                monitor4Username = value;
                MONITORSection.Settings["Monitor4Username"].Value = value;
            }
        }

        private string monitor1Password;
        public string Monitor1Password
        {
            get { return monitor1Password; }
            set
            {
                monitor1Password = value;
                MONITORSection.Settings["Monitor1Password"].Value = value;
            }
        }


        private string monitor2Password;
        public string Monitor2Password
        {
            get { return monitor2Password; }
            set
            {
                monitor2Password = value;
                MONITORSection.Settings["Monitor2Password"].Value = value;
            }
        }


        private string monitor3Password;
        public string Monitor3Password
        {
            get { return monitor3Password; }
            set
            {
                monitor3Password = value;
                MONITORSection.Settings["Monitor3Password"].Value = value;
            }
        }

        private string monitor4Password;
        public string Monitor4Password
        {
            get { return monitor4Password; }
            set
            {
                monitor4Password = value;
                MONITORSection.Settings["Monitor4Password"].Value = value;
            }
        }


        private string lEDIP;
        public string LEDIP
        {
            get { return lEDIP; }
            set
            {
                lEDIP = value;
                LEDSection.Settings["LEDIP"].Value = value;
            }
        }

        private string lED1IP;
        public string LED1IP
        {
            get { return lED1IP; }
            set
            {
                lED1IP = value;
                LEDSection.Settings["LED1IP"].Value = value;
            }
        }

        private string lEDWIDTH;
        public string LEDWIDTH
        {
            get { return lEDWIDTH; }
            set
            {
                lEDWIDTH = value;
                LEDSection.Settings["LEDWIDTH"].Value = value;
            }
        }

        private string lED1WIDTH;
        public string LED1WIDTH
        {
            get { return lED1WIDTH; }
            set
            {
                lED1WIDTH = value;
                LEDSection.Settings["LED1WIDTH"].Value = value;
            }
        }

        private string lEDHEIGHT;
        public string LEDHEIGHT
        {
            get { return lEDHEIGHT; }
            set
            {
                lEDHEIGHT = value;
                LEDSection.Settings["LEDHEIGHT"].Value = value;
            }
        }

        private string lED1HEIGHT;
        public string LED1HEIGHT
        {
            get { return lED1HEIGHT; }
            set
            {
                lED1HEIGHT = value;
                LEDSection.Settings["LED1HEIGHT"].Value = value;
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
                ChargeSection.Settings["ChargeEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool eChargeEnable;
        public bool EChargeEnable
        {
            get { return Convert.ToBoolean(eChargeEnable); }
            set
            {
                eChargeEnable = Convert.ToBoolean(value);
                ChargeSection.Settings["EChargeEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private string parkingID;
        public string ParkingID
        {
            get { return parkingID; }
            set
            {
                parkingID = value;
                ChargeSection.Settings["ParkingID"].Value = value;
            }
        }

        private string pServerPath;
        public string PServerPath
        {
            get { return pServerPath; }
            set
            {
                pServerPath = value;
                ChargeSection.Settings["PServerPath"].Value = value;
            }
        }

        private string uuid;
        public string UUID
        {
            get { return uuid; }
            set
            {
                uuid = value;
                ChargeSection.Settings["UUID"].Value = value;
            }
        }

        private string mac;
        public string MAC
        {
            get { return mac; }
            set
            {
                mac = value;
                ChargeSection.Settings["MAC"].Value = value;
            }
        }

        private bool chargeByMaterial;
        public bool ChargeByMaterial
        {
            get { return Convert.ToBoolean(chargeByMaterial); }
            set
            {
                chargeByMaterial = Convert.ToBoolean(value);
                ChargeSection.Settings["ChargeByMaterial"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private string chargeWay;
        public string ChargeWay
        {
            get { return chargeWay; }
            set
            {
                chargeWay = value;
                ChargeSection.Settings["ChargeWay"].Value = value;
            }
        }

        private string lowestFees;
        public string LowestFees
        {
            get { return lowestFees; }
            set
            {
                lowestFees = value;
                ChargeSection.Settings["LowestFees"].Value = value;
            }
        }

        private string chargeTypes;
        public string ChargeTypes
        {
            get { return chargeTypes; }
            set
            {
                chargeTypes = value;
                ChargeSection.Settings["ChargeTypes"].Value = value;
            }
        }

        private bool chargeStorage;
        public bool ChargeStorage
        {
            get { return chargeStorage; }
            set
            {
                chargeStorage = value;
                ChargeSection.Settings["ChargeStorage"].Value = Convert.ToInt32(value).ToString();
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
            }
        }

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
                VirtualWallSection.Settings["VirtualWall"].Value = value;
            }
        }

        private string vWCamera1IP;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera1IP
        {
            get { return vWCamera1IP; }
            set
            {
                vWCamera1IP = value;
                VirtualWallSection.Settings["VWCamera1IP"].Value = value;
            }
        }

        private string vWCamera1Username;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera1Username
        {
            get { return vWCamera1Username; }
            set
            {
                vWCamera1Username = value;
                VirtualWallSection.Settings["VWCamera1Username"].Value = value;
            }
        }

        private string vWCamera1Password;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera1Password
        {
            get { return vWCamera1Password; }
            set
            {
                vWCamera1Password = value;
                VirtualWallSection.Settings["VWCamera1Password"].Value = value;
            }
        }

        private string vWCamera2IP;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera2IP
        {
            get { return vWCamera2IP; }
            set
            {
                vWCamera2IP = value;
                VirtualWallSection.Settings["VWCamera2IP"].Value = value;
            }
        }

        private string vWCamera2Username;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera2Username
        {
            get { return vWCamera2Username; }
            set
            {
                vWCamera2Username = value;
                VirtualWallSection.Settings["VWCamera2Username"].Value = value;
            }
        }

        private string vWCamera2Password;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera2Password
        {
            get { return vWCamera2Password; }
            set
            {
                vWCamera2Password = value;
                VirtualWallSection.Settings["VWCamera2Password"].Value = value;
            }
        }

        private string vWCamera3IP;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera3IP
        {
            get { return vWCamera3IP; }
            set
            {
                vWCamera3IP = value;
                VirtualWallSection.Settings["VWCamera3IP"].Value = value;
            }
        }

        private string vWCamera3Username;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera3Username
        {
            get { return vWCamera3Username; }
            set
            {
                vWCamera3Username = value;
                VirtualWallSection.Settings["VWCamera3Username"].Value = value;
            }
        }

        private string vWCamera3Password;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera3Password
        {
            get { return vWCamera3Password; }
            set
            {
                vWCamera3Password = value;
                VirtualWallSection.Settings["VWCamera3Password"].Value = value;
            }
        }

        private string vWCamera4IP;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera4IP
        {
            get { return vWCamera4IP; }
            set
            {
                vWCamera4IP = value;
                VirtualWallSection.Settings["VWCamera4IP"].Value = value;
            }
        }

        private string vWCamera4Username;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera4Username
        {
            get { return vWCamera4Username; }
            set
            {
                vWCamera4Username = value;
                VirtualWallSection.Settings["VWCamera4Username"].Value = value;
            }
        }

        private string vWCamera4Password;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWCamera4Password
        {
            get { return vWCamera4Password; }
            set
            {
                vWCamera4Password = value;
                VirtualWallSection.Settings["VWCamera4Password"].Value = value;
            }
        }

        private string vWRF3Ip;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWRF3Ip
        {
            get { return vWRF3Ip; }
            set
            {
                vWRF3Ip = value;
                VirtualWallSection.Settings["VWRF3Ip"].Value = value;
            }
        }

        private string vWRF4Ip;                         //车辆识别方式 0:无 1:汽车标签 2:车牌识别
        public string VWRF4Ip
        {
            get { return vWRF4Ip; }
            set
            {
                vWRF4Ip = value;
                VirtualWallSection.Settings["VWRF4Ip"].Value = value;
            }
        }

        private bool enableWeighFinish;
        public bool EnableWeighFinish
        {
            get { return Convert.ToBoolean(enableWeighFinish); }
            set
            {
                enableWeighFinish = Convert.ToBoolean(value);
                VirtualWallSection.Settings["EnableWeighFinish"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool vWWhiteListeEnable;
        public bool VWWhiteListeEnable
        {
            get { return Convert.ToBoolean(vWWhiteListeEnable); }
            set
            {
                vWWhiteListeEnable = Convert.ToBoolean(value);
                VirtualWallSection.Settings["VWWhiteListeEnable"].Value = Convert.ToInt32(value).ToString();
            }
        }

        private bool vWStorageOverflow;
        public bool VWStorageOverflow
        {
            get { return vWStorageOverflow; }
            set
            {
                vWStorageOverflow = value;
                VirtualWallSection.Settings["VWStorageOverflow"].Value = Convert.ToInt32(value).ToString();
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
                config.AppSettings.Settings["CheckGrating"].Value = value == "启用" ? "1" : "0";
            }
        }

        private string lPRWhiteList;
        public string LPRWhiteList
        {
            get { return lPRWhiteList; }
            set
            {
                lPRWhiteList = value;
                config.AppSettings.Settings["LPRWhiteList"].Value = value == "启用" ? "1" : "0";
            }
        }

        private string autoWeighingType;
        public string AutoWeighingType
        {
            get { return autoWeighingType; }
            set
            {
                autoWeighingType = value;
                config.AppSettings.Settings["AutoWeighingType"].Value = value == "启用" ? "1" : "0";
            }
        }

        private string timelyRefresh;
        public string TimelyRefresh
        {
            get { return timelyRefresh; }
            set
            {
                timelyRefresh = value;
                config.AppSettings.Settings["TimelyRefresh"].Value = value == "启用" ? "1" : "0";
            }
        }

        private string _autoConvertUnit;
        public string AutoConvertUnit
        {
            get { return _autoConvertUnit; }
            set
            {
                _autoConvertUnit = value;
                config.AppSettings.Settings["AutoConvertUnit"].Value = value == "启用" ? "True" : "False";
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
                config.AppSettings.Settings["CancelWeighingTime"].Value = value;
            }
        }

        private string lastPlateTimer;
        public string LastPlateTimer
        {
            get { return lastPlateTimer; }
            set
            {
                lastPlateTimer = value;
                config.AppSettings.Settings["LastPlateTimer"].Value = value;
            }
        }
        private string weighingValidTime;
        public string WeighingValidTime
        {
            get { return weighingValidTime; }
            set
            {
                weighingValidTime = value;
                config.AppSettings.Settings["WeighingValidTime"].Value = value;
            }
        }

        private string pageCount;
        public string PageCount
        {
            get { return pageCount; }
            set
            {
                pageCount = value;
                config.AppSettings.Settings["PageCount"].Value = value;
            }
        }


        private bool pzbjzt;
        public bool Pzbjzt
        {
            get { return Convert.ToBoolean(pzbjzt); }
            set
            {
                pzbjzt = Convert.ToBoolean(value);
                SyncSection.Settings["Pzbjzt"].Value = value.ToString();
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
                SyncSection.Settings["ServerPath"].Value = value;
            }
        }
        private string pzbjfz;
        public string Pzbjfz
        {
            get { return pzbjfz; }
            set
            {
                pzbjfz = value;
                SyncSection.Settings["Pzbjfz"].Value = value;
                Common.SyncData.set_pz_weight_alarm_threshold(value);
            }
        }

        public object WeighFormVM { get; set; } //过磅单表单
        public List<string> WeighFromTemplateSheetsName { get; private set; } = new List<string>();
        public string SelectedWeighFromTemplateSheet { get; set; }
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
        #endregion
        public string ProvicerTitle
        {
            get
            {
                return AWSV2.Globalspace.ProvicerTitle;
            }
        }

        protected Configuration config;
        protected Configuration ZXLPRConfig;
        protected AppSettingsSection LPRSection;
        protected Configuration LEDConfig;
        protected AppSettingsSection LEDSection;
        protected Configuration MONITORConfig;
        protected AppSettingsSection MONITORSection;
        protected Configuration ChargeConfig;
        protected AppSettingsSection ChargeSection;
        protected Configuration VirtualWallConfig;
        protected AppSettingsSection VirtualWallSection;
        protected Configuration SyncConfig;
        protected AppSettingsSection SyncSection;

        Common.Socket.NiRen_UDP nirenUDP = new Common.Socket.NiRen_UDP();

        public SimpleSettingViewModel(IWindowManager windowManager, IModelValidator<SimpleSettingViewModel> validator) : base(validator)
        {
            //var wzList
            WZList = SQLDataAccess.LoadActiveGoods().Select(s => s.Name);
            WZList = WZList.Prepend(string.Empty);


            nirenUDP = new Common.Socket.NiRen_UDP();
            nirenUDP.OnResult += NirenUDP_OnResult;
            this.windowManager = windowManager;

            var pList = PrinterSettings.InstalledPrinters;
            foreach (string p in pList)
            {
                PrinterList.Add(p);
            }

            //加载物资数据
            GoodsList = SQLDataAccess.LoadActiveGoods().Select(p => p.Name);

            //Configuration config = ConfigurationManager.OpenExeConfiguration("ZXLPR.exe");
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ZXLPRConfig = ConfigurationManager.OpenExeConfiguration("ZXLPR.exe");
            LPRSection = (AppSettingsSection)ZXLPRConfig.GetSection("appSettings");

            LEDConfig = ConfigurationManager.OpenExeConfiguration("ZXLED.exe");
            LEDSection = (AppSettingsSection)LEDConfig.GetSection("appSettings");

            MONITORConfig = ConfigurationManager.OpenExeConfiguration("ZXMonitor.exe");
            MONITORSection = (AppSettingsSection)MONITORConfig.GetSection("appSettings");

            //ChargeConfig = ConfigurationManager.OpenExeConfiguration("ZXCharge.exe");
            //ChargeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ChargeSection = config.AppSettings;// (AppSettingsSection)ChargeConfig.GetSection("appSettings");

            VirtualWallConfig = ConfigurationManager.OpenExeConfiguration("ZXVirtualWall.exe");
            VirtualWallSection = (AppSettingsSection)VirtualWallConfig.GetSection("appSettings");

            SyncConfig = ConfigurationManager.OpenExeConfiguration("ZXSyncWeighingData.exe");
            SyncSection = (AppSettingsSection)SyncConfig.GetSection("appSettings");

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

        }

        /// <summary>
        /// 磅单图片源
        /// </summary>
        public System.Windows.Media.ImageSource WeighingFormImg { get; set; }

        private void LoadWeighFormView()
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
        }

        private void LoadZXLPRConfig()
        {
            Camera1IP = LPRSection.Settings["Camera1IP"].Value;
            Camera2IP = LPRSection.Settings["Camera2IP"].Value;
            Camera1Username = LPRSection.Settings["Camera1Username"].Value;
            Camera2Username = LPRSection.Settings["Camera2Username"].Value;
            Camera1Password = LPRSection.Settings["Camera1Password"].Value;
            Camera2Password = LPRSection.Settings["Camera2Password"].Value;
            Camera1Enable = Convert.ToBoolean(Convert.ToInt32(LPRSection.Settings["Camera1Enable"].Value));
            Camera2Enable = Convert.ToBoolean(Convert.ToInt32(LPRSection.Settings["Camera2Enable"].Value));
            Camera1LEDEnable = Convert.ToBoolean(Convert.ToInt32(LPRSection.Settings["Camera1LEDEnable"].Value));
            Camera2LEDEnable = Convert.ToBoolean(Convert.ToInt32(LPRSection.Settings["Camera2LEDEnable"].Value));
            Camera1GPIO = Convert.ToBoolean(Convert.ToInt32(LPRSection.Settings["Camera1GPIO"].Value));
            RF1Enable = Convert.ToBoolean(Convert.ToInt32(LPRSection.Settings["RF1Enable"].Value));
            RF2Enable = Convert.ToBoolean(Convert.ToInt32(LPRSection.Settings["RF2Enable"].Value));
            RF1Uart = LPRSection.Settings["RF1Uart"].Value;
            RF2Uart = LPRSection.Settings["RF2Uart"].Value;
            RF3Ip = LPRSection.Settings["RF3IP"].Value;
            RF4Ip = LPRSection.Settings["RF4IP"].Value;
            //RF1Power = LPRSection.Settings["RF1Power"].Value;
            //RF2Power = LPRSection.Settings["RF2Power"].Value;
            RelayEnable = Convert.ToBoolean(Convert.ToInt32(LPRSection.Settings["RelayEnable"].Value));
            RelayUart = LPRSection.Settings["RelayUart"].Value;
            Plate2Enable = LPRSection.Settings["Plate2Enable"].Value == "1" ? "启用" : "不启用";
            LPRSavePath = LPRSection.Settings["LPRSavePath"].Value;
            selectedController = LPRSection.Settings["SelectedController"].Value;

            Relay2IP = LPRSection.Settings["Relay2IP"].Value;
            Relay2Mac = LPRSection.Settings["Relay2Mac"].Value;

        }

        private void LoadLEDConfig()
        {
            LEDIP = LEDSection.Settings["LEDIP"].Value;
            LEDWIDTH = LEDSection.Settings["LEDWIDTH"].Value;
            LEDHEIGHT = LEDSection.Settings["LEDHEIGHT"].Value;
            LED1IP = LEDSection.Settings["LED1IP"].Value;
            LED1WIDTH = LEDSection.Settings["LED1WIDTH"].Value;
            LED1HEIGHT = LEDSection.Settings["LED1HEIGHT"].Value;
        }

        private void LoadMONITORConfig()
        {
            Monitor1Enable = Convert.ToBoolean(Convert.ToInt32(MONITORSection.Settings["Monitor1Enable"].Value));
            Monitor2Enable = Convert.ToBoolean(Convert.ToInt32(MONITORSection.Settings["Monitor2Enable"].Value));
            Monitor3Enable = Convert.ToBoolean(Convert.ToInt32(MONITORSection.Settings["Monitor3Enable"].Value));
            Monitor4Enable = Convert.ToBoolean(Convert.ToInt32(MONITORSection.Settings["Monitor4Enable"].Value));
            Monitor1IP = MONITORSection.Settings["Monitor1IP"].Value;
            Monitor2IP = MONITORSection.Settings["Monitor2IP"].Value;
            Monitor3IP = MONITORSection.Settings["Monitor3IP"].Value;
            Monitor4IP = MONITORSection.Settings["Monitor4IP"].Value;
            Monitor1Username = MONITORSection.Settings["Monitor1Username"].Value;
            Monitor2Username = MONITORSection.Settings["Monitor2Username"].Value;
            Monitor3Username = MONITORSection.Settings["Monitor3Username"].Value;
            Monitor4Username = MONITORSection.Settings["Monitor4Username"].Value;
            Monitor1Password = MONITORSection.Settings["Monitor1Password"].Value;
            Monitor2Password = MONITORSection.Settings["Monitor2Password"].Value;
            Monitor3Password = MONITORSection.Settings["Monitor3Password"].Value;
            Monitor4Password = MONITORSection.Settings["Monitor4Password"].Value;
            MonitorSavePath = MONITORSection.Settings["MonitorSavePath"].Value;

            //2022-12-15 add
            MonitorCaptureEnable = Convert.ToBoolean(MONITORSection.Settings["MonitorCaptureEnable"].Value);
        }

        private void LoadChargeConfig()
        {
            //收费基数配置数据加载
            ChargeEnable = Convert.ToBoolean(Convert.ToInt32(ChargeSection.Settings["ChargeEnable"].Value));
            //是否启用称重处的电子支付功能
            EChargeEnable = Convert.ToBoolean(Convert.ToInt32(ChargeSection.Settings["EChargeEnable"].Value));
            //是否按照物资收费
            ChargeByMaterial = Convert.ToBoolean(Convert.ToInt32(ChargeSection.Settings["ChargeByMaterial"].Value));
            ChargeWay = ChargeSection.Settings["ChargeWay"].Value;

            ParkingID = ChargeSection.Settings["ParkingID"].Value;
            PServerPath = ChargeSection.Settings["PServerPath"].Value;
            UUID = ChargeSection.Settings["UUID"].Value;
            MAC = ChargeSection.Settings["MAC"].Value;

            //最低消费
            LowestFees = ChargeSection.Settings["LowestFees"].Value;
            //称重结束收费?
            ChargeTypes = ChargeSection.Settings["ChargeTypes"].Value;

            //是否支持用户存储充值
            ChargeStorage = Convert.ToBoolean(Convert.ToInt32(ChargeSection.Settings["ChargeStorage"].Value));

            //收费信息解析组装
            var info = ChargeSection.Settings["ChargeInfo"].Value;
            if (string.IsNullOrWhiteSpace(info))
            {
                //如果没有配置，那么初始化一个默认的
                ChargeInfo.Add(new ChargeInfoModel() { Id = "1", Name = "1", Fees = new FeesItemModel() });
                ChargeInfo.Add(new ChargeInfoModel() { Id = "2", Name = "2", Fees = new FeesItemModel() });
                ChargeInfo.Add(new ChargeInfoModel() { Id = "3", Name = "3", Fees = new FeesItemModel() });
                ChargeInfo.Add(new ChargeInfoModel() { Id = "4", Name = "4", Fees = new FeesItemModel() });
                ChargeInfo.Add(new ChargeInfoModel() { Id = "5", Name = "5", Fees = new FeesItemModel() });
                ChargeInfo.Add(new ChargeInfoModel() { Id = "6", Name = "6", Fees = new FeesItemModel() });
                //这个是普通收费的标准，是系统自己维护的，不受设置
                ChargeInfo.Add(new ChargeInfoModel() { Id = "Normal_888888", Name = "普通收费标准", Fees = new FeesItemModel() { Fees1 = 1234566 } });
            }
            else
            {
                //否则开始解析
                ChargeInfo = JsonConvert.DeserializeObject<List<ChargeInfoModel>>(info);
            }
            //获取普通收费的标准
            NormalInfo = ChargeInfo.Find(p => p.Id == "Normal_888888");
        }

        private void LoadSyncConfig()
        {
            Pzbjzt = Convert.ToBoolean(SyncSection.Settings["Pzbjzt"].Value);
            ServerPath = SyncSection.Settings["ServerPath"].Value;
            Pzbjfz = SyncSection.Settings["Pzbjfz"].Value;
        }

        private void LoadVirtualWallConfig()
        {
            VirtualWall = VirtualWallSection.Settings["VirtualWall"].Value;
            VWCamera1IP = VirtualWallSection.Settings["VWCamera1IP"].Value;
            VWCamera1Username = VirtualWallSection.Settings["VWCamera1Username"].Value;
            VWCamera1Password = VirtualWallSection.Settings["VWCamera1Password"].Value;
            VWCamera2IP = VirtualWallSection.Settings["VWCamera2IP"].Value;
            VWCamera2Username = VirtualWallSection.Settings["VWCamera2Username"].Value;
            VWCamera2Password = VirtualWallSection.Settings["VWCamera2Password"].Value;

            VWCamera3IP = VirtualWallSection.Settings["VWCamera3IP"].Value;
            VWCamera3Username = VirtualWallSection.Settings["VWCamera3Username"].Value;
            VWCamera3Password = VirtualWallSection.Settings["VWCamera3Password"].Value;

            VWCamera4IP = VirtualWallSection.Settings["VWCamera4IP"].Value;
            VWCamera4Username = VirtualWallSection.Settings["VWCamera4Username"].Value;
            VWCamera4Password = VirtualWallSection.Settings["VWCamera4Password"].Value;

            VWRF3Ip = VirtualWallSection.Settings["VWRF3Ip"].Value;
            VWRF4Ip = VirtualWallSection.Settings["VWRF4Ip"].Value;
            VWWhiteListeEnable = Convert.ToBoolean(Convert.ToInt32(VirtualWallSection.Settings["VWWhiteListeEnable"].Value));
            EnableWeighFinish = Convert.ToBoolean(Convert.ToInt32(VirtualWallSection.Settings["EnableWeighFinish"].Value));
            VWStorageOverflow = Convert.ToBoolean(Convert.ToInt32(VirtualWallSection.Settings["VWStorageOverflow"].Value));
        }

        private void LoadConfig()
        {
            GblxCG = ConfigurationManager.AppSettings["GblxCG"];
            GblxXS = ConfigurationManager.AppSettings["GblxXS"];
            GblxQT = ConfigurationManager.AppSettings["GblxQT"];
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
            LoadWeighFormView();
        }

        public void EditWeighFormList()
        {
            //var viewModel = new WeighFormListViewModel(windowManager, config);
            var viewModel = WeighFormListViewModel.GetInstance(windowManager, ref config);
            this.windowManager.ShowWindow(viewModel);
        }

        public void EditByFieldFormula()
        {
            var viewModel = new ByFieldFormulaViewModel(windowManager, ref config);
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
            //bool? result = windowManager.ShowDialog(new LoginViewModel(windowManager, false));
            var pwdWindow = new PasswordViewModel(windowManager, 3, true);
            bool? result = windowManager.ShowDialog(pwdWindow);

            if (result.GetValueOrDefault(true))
            {
                SQLDataAccess.DataClean();
                windowManager.ShowMessageBox("数据清理完毕！");
            }
            //else
            //{
            //    windowManager.ShowMessageBox("您没有权限或账号密码错误，已取消操作！");
            //}
        }

        public void showEChargeEdit(string type)
        {
           // bool? result = windowManager.ShowDialog(new ChargeEditViewModel(this));
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
                OnPropertyChanged("SelectedItem");
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
                    LPRSection.Settings["Relay2IP"].Value = value;
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
                    LPRSection.Settings["Relay2Mac"].Value = value;
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
                    LPRSection.Settings["Relay2Mask"].Value = value;
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
                    LPRSection.Settings["Relay2Gateway"].Value = value;
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

        protected override void OnClose()
        {
            try
            {
                //始终是开启同步模式
                config.AppSettings.Settings["SyncDataEnable"].Value = true.ToString();
                //版本之间逻辑检测和处理
                if (Common.Share.VersionControl.CurrentVersion == Common.Share.VersionType.标准版)
                {
                    //Common.Utility.SettingsHelper.UpdateAWSV2("Barrier", "0");
                    Barrier = "0";
                }

                ChargeSection.Settings["ChargeInfo"].Value = JsonConvert.SerializeObject(ChargeInfo);
                config.Save(ConfigurationSaveMode.Modified);
                ZXLPRConfig.Save();
                LEDConfig.Save();
                MONITORConfig.Save();
                //ChargeConfig.Save();
                VirtualWallConfig.Save();
                SyncConfig.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception e)
            {

            }
            base.OnClose();
        }
    }

    public class SimpleSettingViewModelValidator : AbstractValidator<SimpleSettingViewModel>
    {
        public SimpleSettingViewModelValidator()
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
                if(!int.TryParse(text.ToLower().Replace("_by",string.Empty),out index))
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
