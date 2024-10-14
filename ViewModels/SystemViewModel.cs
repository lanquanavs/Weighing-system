using Aspose.Cells;
using AWSV2.Models;
using AWSV2.Services;
using AWSV2.Services.Encrt;
using Common.Model.Custom;
using Flee.PublicTypes;
using IWshRuntimeLibrary;
using Newtonsoft.Json;
//using Quartz.Impl;
//using Quartz;
using Stylet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows;
using System.Reflection;
using Common.Utility;

namespace AWSV2.ViewModels
{

    public class SystemViewModel : Screen
    {
        private IWindowManager windowManager;

        public string WdfwsImg { get; set; }
        public string ShwhImg { get; set; }
        public string XtpzImg { get; set; }

        public string WdfwsForeground { get; set; }
        public string ShwhForeground { get; set; }
        public string XtpzForeground { get; set; }

      
        public Visibility ShowUpgrade
        {
            get
            {
                var bol = Common.Share.VersionControl.CurrentVersion == Common.Share.VersionType.智能版;
                return bol ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Common.Model.Custom.ServiceProvider Provicer
        {
            get
            {
                return AWSV2.Globalspace.Provicer;
            }
        }

        public string ProvicerTitle
        {
            get
            {
                return AWSV2.Globalspace.ProvicerTitle;
            }
        }

        public MaintenanceRecordView RecordView { get; set; }
        private DispatcherTimer tips_Timer = new DispatcherTimer();

        public System.Windows.Visibility WdfwsVisibility { get; set; }
        public System.Windows.Visibility ShwhVisibility { get; set; }
        public System.Windows.Visibility XtpzVisibility { get; set; }

        public string UploadTips { get; set; }
        public string DownloadTips { get; set; }
        public bool DownloadBtnEnable { get; set; } = true;

        public string versionInfo;
        public string VersionInfo
        {
            get
            {
                return versionInfo;
            }
            set
            {
                versionInfo = value;
                OnPropertyChanged("VersionInfo");
            }
        }

        public string _updateRemark;
        public string UpdateRemark
        {
            get
            {
                return _updateRemark;
            }
            set
            {
                _updateRemark = value;
                OnPropertyChanged("UpdateRemark");
            }
        }

        //构造函数，初始化
        public SystemViewModel(IWindowManager windowManager)
        {
            ResetIcon(0);
            this.windowManager = windowManager;
            RecordView = Common.SyncData.GetMaintenanceRecord();

            tips_Timer.Interval = TimeSpan.FromMilliseconds(3000);
            tips_Timer.Tick += Tips_Timer_Tick;
            Task.Run(() => {

                AWSV2.Upgrade.ServerInfoSoapClient server = new AWSV2.Upgrade.ServerInfoSoapClient();
                var newVersion = server.GetCurrentVersion();

                var currentVersion = Common.Utility.SettingsHelper.AWSV2Settings.Settings["Version"].Value;
                UpdateRemark = server.GetDiscrepantDetails(currentVersion)?.LastOrDefault()?.UpgradeContent ?? "暂无版本更新";

                VersionInfo = $"当前版本：{currentVersion}    最新版本：{newVersion}";
            });
           
        }

        private void Tips_Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                DownloadBtnEnable = true;
                System.Windows.Application.Current.Shutdown();
            }
            catch { }
        }

        public void ResetIcon(int SelectIndex = 0)
        {
            WdfwsImg = string.Format("/Resources/Img/07/xt-aa{0}.png", SelectIndex == 0 ? "" : "a");
            ShwhImg = string.Format("/Resources/Img/07/xt-bb{0}.png", SelectIndex == 1 ? "" : "b");
            XtpzImg = string.Format("/Resources/Img/07/xt-cc{0}.png", SelectIndex == 2 ? "" : "c");

            WdfwsVisibility = SelectIndex == 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            ShwhVisibility = SelectIndex == 1 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            XtpzVisibility = SelectIndex == 2 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            WdfwsForeground = SelectIndex == 0 ? "#000000" : "#666666";
            ShwhForeground = SelectIndex == 1 ? "#000000" : "#666666";
            XtpzForeground = SelectIndex == 2 ? "#000000" : "#666666";
        }

        public void SwitchVM(string name)
        {
            switch (name)
            {
                case "Wdfws":
                    ResetIcon(0);
                    break;
                case "Shwh":
                    ResetIcon(1);
                    break;
                case "Xtpz":
                    ResetIcon(2);
                    break;

                default:
                    break;
            }
        }

        public void UploadBackup()
        {
            try
            {
                UploadTips = "开始上传中...";
                string directoryPath = $"{AppContext.BaseDirectory}Data\\Backup\\";
                //排除掉待.名称的文件夹，因为这些文件夹是 自动升级程序生成的。不能上传。
                var directory = Directory.GetDirectories(directoryPath).Where(p=>!p.Contains(".")).OrderByDescending(o => o).FirstOrDefault();
                if (!string.IsNullOrEmpty(directory))
                {

                    var cleanResult = Common.SyncData.CleanBackup();

                    var files = Directory.GetFiles(directory);

                    foreach (var filePath in files)
                    {
                        //var fileName= System.IO.Path.GetFileName(filePath);
                        //var fileNameEscape = Uri.EscapeDataString(fileName);
                        //filePath = filePath.Replace(fileName, fileNameEscape);
                        var result = Common.SyncData.UpLoadBackup(filePath);
                    }
                }
                UploadTips = "上传完毕！";
            }
            catch (Exception e)
            {
                UploadTips = $"上传失败：{e.Message}";
            }
        }

        /// <summary>
        /// 新增重新注册逻辑 --阿吉 2023年6月5日17点17分
        /// </summary>
        public void ResetReg()
        {


            Configuration config = SettingsHelper.AWSV2Settings.CurrentConfiguration;

            /*config.AppSettings.Settings["RegCode"].Value = lowerReg;*/
            config.AppSettings.Settings["RegCode"].Value = "";

            config.Save(ConfigurationSaveMode.Modified);

            IPCHelper.KillApp("ZXMonitor");
            IPCHelper.KillApp("ZXWeighingRecord");
            IPCHelper.KillApp("ZXDataManagement");
            IPCHelper.KillApp("ZXSyncWeighingData");
            //IPCHelper.CloseApp("ZXLPR");
            IPCHelper.KillApp("ZXLPR");
            IPCHelper.KillApp("ZXLED");
            IPCHelper.KillApp("ZXAxleNo");
            IPCHelper.KillApp("ZXQuickPlate");
            //IPCHelper.KillApp("ZXVirtualWall");
            IPCHelper.KillApp("ZXVirtualWall");

            Process.Start(Assembly.GetExecutingAssembly().Location);
            Application.Current.Shutdown();

        }

        public void DownloadBackup()
        {
            DownloadBtnEnable = false;
            try
            {
                string exePath = Environment.CurrentDirectory + "\\ZXUpdater.exe";
                Process process = new Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = String.Format(Common.Utility.HardDiskInfo.SerialNumber);
                process.Start();
            }
            catch (Exception e)
            {
                DownloadBtnEnable = true;
            }
        }

        public void DownloadBackup1()
        {
            DownloadBtnEnable = false;
            try
            {
                DownloadTips = "开始下载中...";
                bool isError = false;
                string directoryPath = $"{AppContext.BaseDirectory}";
                var result = Common.SyncData.DowmloadBackup();
                if (!string.IsNullOrEmpty(result))
                {
                    var files = JsonConvert.DeserializeObject<List<string>>(result);//result.Split(',');
                    foreach (var file in files)
                    {
                        var filePath = file.Trim().Replace("\"", "");

                        if (filePath.ToLower().Contains("error") || filePath.ToLower().Contains("aws.db")) continue;
                        var falg = false;

                        var fileName=filePath.Trim().Substring(filePath.Trim().LastIndexOf("/")).Replace("/", "").Replace("\\", "").Replace("\"", "");

                        fileName = Uri.UnescapeDataString(fileName);

                        if (filePath.ToLower().Contains("weighformtemplate.xlsx"))
                        {
                            falg = Common.SyncData.FileDownload(filePath, $"{directoryPath}Data\\{fileName}");
                        }
                        else
                        {
                            falg = Common.SyncData.FileDownload(filePath, $"{directoryPath}{fileName}");
                        }

                        if (!falg)
                        {
                            isError=true;
                        }
                      
                    }
                    if (!isError)
                    {
                        DownloadTips = "下载完成，即将关闭程序。";
                       
                    }
                    else
                    {
                        DownloadTips = "部分文件下载失败！即将关闭程序。";
                    }
                   
                    tips_Timer.Start();
                }
            }
            catch (Exception e)
            {
                DownloadBtnEnable = true;
            }
        }

        public void ShowHelpChm()
        {
            //打开chm文档
            try
            {
                //System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\无人值守汽车衡称重系统V2.2 说明书.chm");
                System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\衡七管家说明书.pdf");
            }
            catch { }
        }

        public void ShowSysLog()
        {
            var viewModel = new SysLogViewModel(windowManager);

            windowManager.ShowDialog(viewModel);
        }

        public void Upgrade()
        {
            try
            {
                IPCHelper.OpenApp("ZXUpdater");
            }
            catch (Exception e)
            {
                UploadTips = $"启动升级程序失败：{e.Message}";
            }
        }

    }
}
