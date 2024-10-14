using AWSV2.Services;
using Common.HardwareSDKS.FFmpeg;
using Common.HardwareSDKS.HIKVision;
using Common.HardwareSDKS.Models;
using Common.Models;
using Common.Utility;
using Common.Utility.AJ;
using FFmpeg.AutoGen;
using IWshRuntimeLibrary;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using log4net;
using Masuit.Tools.Security;
using MaterialDesignThemes.Wpf;
using NPOI.SS.Formula.Functions;
using Quartz.Impl;
using Quartz;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Yitter.IdGenerator;
using static Common.HardwareSDKS.HIKVision.CHCNetSDK;

namespace AWSV2
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                // 程序启动, 赋值机器码
                CloudAPI.MACHINEMD5 = AJUtil.MachineMD5();

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                YidService.Init();

                //var a = $"{DateTime.Now.Date.AddDays(7)}_{CloudAPI.MACHINEMD5}_{YitIdHelper.NextId()}".AESEncrypt(Common.Properties.Resources.AESKey).Base64Encrypt();

                // 启动数据库服务
                AJDatabaseService.Start();

                Hearbeat();
                //var providerDetails = Common.SyncData.GetServiceProvider();
                //AWSV2.Globalspace.Provicer = providerDetails;
                //AWSV2.Globalspace.ProvicerTitle = $"{providerDetails.service_products}  |  {providerDetails.company_name}  服务热线：{providerDetails.mobile}";

                //2023-05-18 新增应用程序快捷方式创建

                string QuickName = Application.ResourceAssembly.GetName().Name;
                //string QuickName1 = "篮圈AVS1修复工具";
                var appAllPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                //var systemStartPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                var systemStartPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (!System.IO.File.Exists(Path.Combine(systemStartPath, string.Format("{0}.lnk", QuickName))))
                {
                    CreateShortcut(systemStartPath, QuickName, appAllPath, QuickName);
                }
                //if (!System.IO.File.Exists(Path.Combine(systemStartPath, string.Format("{0}.lnk", QuickName1))))
                //{
                //    appAllPath = appAllPath.Replace(QuickName, QuickName1);
                //    CreateShortcut(systemStartPath, QuickName1, appAllPath, QuickName1);
                //}

                AWSV2.Globalspace.Provicer = new Common.Model.Custom.ServiceProvider();
                AWSV2.Globalspace.ProvicerTitle = $"正在加载中...";

                #region 注册 FFMPEG --阿吉 2024年4月12日09点18分

                FFmpegBinariesHelper.RegisterFFmpegBinaries();

                //FFmpegUtil.ConfigureHWDecoder();

                #endregion


                #region 版本之间逻辑检测和处理
                //暂时搁置 2023-03-10 此处涉及到不同程序修改版本号的问题。
                //var version = Application.ResourceAssembly.GetName().Version.ToString();
                //Common.Utility.SettingsHelper.UpdateAWSV2("Version", version);

                ////始终是开启同步模式
                //Common.Utility.SettingsHelper.UpdateAWSV2("SyncDataEnable", true.ToString());

                //// 版本之间逻辑检测和处理
                //if (Common.Share.VersionControl.CurrentVersion == Common.Share.VersionType.标准版)
                //{
                //    Common.Utility.SettingsHelper.UpdateAWSV2("Barrier", "0");

                //    Common.Utility.SettingsHelper.UpdateVirtualWall("VirtualWall", "0");
                //}
                #endregion

                #region 定时任务处理逻辑

                await TimedTask.Run();

                #endregion


                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"初始启动存在异常:{ex.Message}");
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }

            Common.Utility.ExcelHelper.Instance.Init();

            base.OnStartup(e);

            LiveCharts.Configure(cfg =>
            {
                cfg.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter("Microsoft YaHei", '汉'));
            });

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        }

        protected override void OnExit(ExitEventArgs e)
        {
            AJDatabaseService.Dispose();
            base.OnExit(e);
        }

        static Thread thread = null;
        private void Hearbeat()
        {
            //Common.Encrt.WebApi.IsNetConnection = false;
            //thread = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Common.Encrt.WebApi.PingTest();
            //        Thread.Sleep(5000);
            //    }
            //});
            //thread.IsBackground = true;
            //thread.Start();
        }

        public static void DisposeSync()
        {
            try
            {
                if (thread != null)
                {
                    thread.Abort();
                }
            }
            catch (Exception e)
            {
                // log.Debug(ex.Message);
            }
        }
        /// <summary>
        ///  向目标路径创建指定文件的快捷方式
        /// </summary>
        /// <param name="directory">目标目录</param>
        /// <param name="shortcutName">快捷方式名字</param>
        /// <param name="targetPath">文件完全路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标地址</param>
        /// <returns>成功或失败</returns>
        private bool CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
        {
            try
            {
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);                         //目录不存在则创建
                                                                                                                //添加引用 Com 中搜索 Windows Script Host Object Model
                string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));          //合成路径
                WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);    //创建快捷方式对象
                shortcut.TargetPath = targetPath;                                                               //指定目标路径
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);                                  //设置起始位置
                shortcut.WindowStyle = 1;                                                                       //设置运行方式，默认为常规窗口
                shortcut.Description = description;                                                             //设置备注
                shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;    //设置图标路径
                shortcut.Save();                                                                                //保存快捷方式
                return true;
            }
            catch (Exception ex)
            {
                string temp = ex.Message;
                temp = "";
            }
            return false;
        }


        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            if (e.Exception is FriendlyException fex)
            {
                if (!fex.Silent )
                {
                    MessageBox.Show(fex.Message, fex.Title, MessageBoxButton.OK);
                }
                return;
            }
            AJLog4NetLogger.Instance().Error($"{nameof(App_DispatcherUnhandledException)} 全局异常已捕获", e.Exception);

        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            if (e.Exception.GetBaseException() is FriendlyException fex)
            {
                if (!fex.Silent)
                {
                    MessageBox.Show(fex.Message, fex.Title, MessageBoxButton.OK);
                }
                return;
            }
            AJLog4NetLogger.Instance().Error($"{nameof(TaskScheduler_UnobservedTaskException)} 全局异常已捕获", e.Exception);

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                if ((e.ExceptionObject as Exception) is FriendlyException fex)
                {
                    if (!fex.Silent)
                    {
                        MessageBox.Show(fex.Message, fex.Title, MessageBoxButton.OK);
                    }
                    return;
                }

                AJLog4NetLogger.Instance().Error($"{nameof(CurrentDomain_UnhandledException)} 全局异常已捕获", e.ExceptionObject as Exception);
            }

        }
    }
}
