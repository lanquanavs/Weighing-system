using AWSV2.ViewModels;
using Common.EF;
using Common.EF.Controllers;
using Common.Model;
using Common.Utility;
using Common.Utility.AJ;
using Common.Utility.AJ.Extension;
using Common.ViewModels;
using Masuit.Tools.Files;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using Quartz;
using Quartz.Impl;
using Stylet;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Xml;
using Path = System.IO.Path;

namespace AWSV2.Services
{
    public static class BackupHelper
    {
        private static IScheduler _scheduler;

        public static async Task InitBackupJobAsync(IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                _scheduler = scheduler;
            }
            else
            {
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                _scheduler = await schedulerFactory.GetScheduler();
            }

            var mainSettings = SettingsHelper.AWSV2Settings;
            var jobKey = new JobKey(nameof(AutoDatabaseBackupJob));
            if (_scheduler.GetJobDetail(jobKey) != null)
            {
                await _scheduler.ResumeJob(jobKey);
            }

            var type = mainSettings.Settings["CurrentAutoBackupFrequency"].TryGetString();

            if (string.IsNullOrWhiteSpace(type))
            {
                return;
            }

            var job = JobBuilder.Create<AutoDatabaseBackupJob>().WithIdentity(jobKey).Build();

            var cronExp = string.Empty;
            var startDate = DateTime.Today.AddHours(12);
            if (type == "每天")
            {
                cronExp = "0 0 0 1/1 * ? *";
            }
            else if (type == "每周")
            {
                cronExp = "0 0 0 ? * MON,TUE,WED,THU,FRI,SAT,SUN *";
            }
            else
            {
                cronExp = "0 0 0 1 1/1 ? *";
            }

            var jobTrigger = TriggerBuilder.Create().StartAt(startDate)
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronExp)).Build();

            await _scheduler.ScheduleJob(job, jobTrigger);
        }

        public static bool CleanupByMonthCfg(IWindowManager windowManager, CleanupConfig cleanupConfig,
            bool manual = true)
        {
            if (manual)
            {
                var pwdWindow = new Common.ViewModels.PasswordViewModel(
                    Globalspace._currentUser,
                    Common.ViewModels.PasswordConfirmType.用户验证,
                    true);
                bool? result = windowManager.ShowDialog(pwdWindow);
                if (!result.GetValueOrDefault())
                {
                    return false;
                }
            }
            else
            {
                var curUser = AWSV2.Globalspace._currentUser;
                if (string.IsNullOrWhiteSpace(cleanupConfig.Password)
                    || !curUser.LoginId
                    .Equals("admin", StringComparison.CurrentCultureIgnoreCase)
                    || !curUser.LoginPwd.Equals(cleanupConfig.Password))
                {
                    return false;
                }
            }


            var progressView = new CleaupProgressViewModel(windowManager, cleanupConfig);
            if (manual)
            {
                windowManager.ShowDialog(progressView);
            }
            else
            {
                windowManager.ShowWindow(progressView);
            }
            return true;
        }

        public static async Task<bool> DbCleanAsync(IWindowManager windowManager, string psw = "", bool manual = true)
        {
            if (manual)
            {
                var pwdWindow = new Common.ViewModels.PasswordViewModel(
                    Globalspace._currentUser,
                    Common.ViewModels.PasswordConfirmType.用户验证, true);
                bool? result = windowManager.ShowDialog(pwdWindow);

                if (!result.GetValueOrDefault())
                {
                    return false;
                }
            }
            else
            {
                var curUser = AWSV2.Globalspace._currentUser;
                if (string.IsNullOrWhiteSpace(psw)
                    || !curUser.LoginId.Equals("admin", StringComparison.CurrentCultureIgnoreCase)
                    || !curUser.LoginPwd.Equals(psw))
                {
                    return false;
                }
            }
            using var ctrl = new CommonController();
            await ctrl.ClearDatabase();
            if (manual)
            {
                windowManager.ShowMessageBox("数据清理完毕！");
            }
            return true;

        }

        public static async Task<string> AutoBackupAsync()
        {
            using var ctrl = new CommonController();
            var curDb = ctrl.GetCurrentDbType();

            var isMysql = curDb.Equals("mysql", StringComparison.OrdinalIgnoreCase);

            var _mainSetting = SettingsHelper.AWSV2Settings;

            var fileExt = isMysql ? ".sql" : ".db";

            var now = DateTime.Now;

            string backupPath = Path.Combine(_mainSetting.Settings["BackupPath"]?.Value ?? string.Empty, now.ToString("yyyyMMddHHmmss"), $"{DbService.DatabaseName}{fileExt}");

            try
            {
                var result = await BackupDatabaseCoreAsync(backupPath);
                if (!result.Success)
                {
                    throw new NotSupportedException(result.Message);
                }

                SettingsHelper.UpdateAWSV2("LastBackupDate", now.Ticks.ToString());
            }
            catch (Exception e)
            {
                AJLog4NetLogger.Instance().Error($"自动备份数据库失败", e);
                backupPath = string.Empty;
            }

            return backupPath;
        }

        public static async void ManualBackupAsync(IWindowManager windowManager)
        {
            using var ctrl = new CommonController();
            var loadingDialog = new AJCommonLoadingDialogViewModel("正在准备...", () =>
            {
                var result = new ProcessResult();

                var curDbVal = ctrl.GetCurrentDbType();
                result.SetSuccess(curDbVal);
                return Task.FromResult(result);
            });

            windowManager.ShowDialog(loadingDialog);

            var curDb = loadingDialog.Result.Data as string;

            var isMysql = curDb.Equals("mysql", StringComparison.OrdinalIgnoreCase);

            var fileExt = isMysql ? ".sql" : ".db";

            var _mainSetting = SettingsHelper.AWSV2Settings;

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                InitialDirectory = _mainSetting.Settings["BackupPath"]?.Value ?? string.Empty,
                FileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{DbService.DatabaseName}",
                AddExtension = true,
                CreatePrompt = true,
                OverwritePrompt = true,
                DefaultExt = fileExt,
                Filter = $"数据库备份（.{fileExt}）|*{fileExt}",
                CheckPathExists = true,
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                try
                {

                    var result = await BackupDatabaseCoreAsync(dialog.FileName);
                    if (!result.Success)
                    {
                        throw new NotSupportedException(result.Message);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"备份失败:{e.Message}", "提示", MessageBoxButton.OK);
                    AJLog4NetLogger.Instance().Error($"备份失败", e);
                }

            }

        }

        private static async Task<ProcessResult> BackupDatabaseCoreAsync(string backupFileName)
        {
            using var ctrl = new CommonController();
            var result = new ProcessResult();
            try
            {
                var success = await ctrl.BackupDatabase(new
                    AJDatabaseService.DatabaseBackupParam
                {
                    BackupFileName = backupFileName
                });
                if (!success)
                {
                    result.SetError("备份失败");
                }
                else
                {
                    result.SetSuccess();
                }
            }
            catch (Exception e)
            {
                result.SetError(e.Message, e);
            }
            return result;

        }

        private static async Task<ProcessResult> RestoreDataBaseCoreAsync(string backupFileName)
        {
            using var ctrl = new CommonController();
            var result = new ProcessResult();
            try
            {
                var success = await ctrl.RestoreDatabase(new
                    AJDatabaseService.DatabaseBackupParam
                {
                    BackupFileName = backupFileName
                });
                if (!success)
                {
                    result.SetError("还原失败");
                }
            }
            catch (Exception e)
            {
                result.SetError(e.Message, e);
            }
            return result;

        }

        /// <summary>
        /// 辅助备份，每次关闭程序时自动备份
        /// </summary>
        public static async void AssistBackupAsync()
        {
            try
            {
                var backupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Backup", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));
                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }

                using var ctrl = new CommonController();
                var curDb = ctrl.GetCurrentDbType();

                var isMysql = curDb.Equals("mysql", StringComparison.OrdinalIgnoreCase);

                var fileExt = isMysql ? ".sql" : ".db";

                var fileName = Path.Combine(backupPath, $"{DbService.DatabaseName}{fileExt}");

                var result = await BackupDatabaseCoreAsync(fileName);
                var backupMsg = result.Success ? "成功" : result.Message;


                //2、磅单模板备份
                var file1Name = Path.Combine(backupPath, "WeighFormTemplate.xlsx");
                File.Copy($".\\Data\\WeighFormTemplate.xlsx", file1Name);


                //3、配置文件备份
                var path = string.Empty;
                var configRoot = $".\\";
                //-------------------各种文件的处理开始---------------------//
                CopyConfigFile(backupPath, $"{Application.ResourceAssembly.GetName().Name}.dll.config");
                CopyConfigFile(backupPath, "WeighingRecord.dll.config");
                //CopyConfigFile(backupPath, "电子围栏.exe.config");
                //CopyConfigFile(backupPath, "ZXSyncWeighingData.exe.config");
                //CopyConfigFile(backupPath, "QuickPlate.exe.config");
                //CopyConfigFile(backupPath, "监控.exe.config");
                //CopyConfigFile(backupPath, "车牌识别.exe.config");
                //CopyConfigFile(backupPath, "大屏幕.exe.config");
                CopyConfigFile(backupPath, "DataManagement.dll.config");
                //CopyConfigFile(backupPath, "ZXCharge.exe.config");
                //CopyConfigFile(backupPath, "轮轴接收.dll.config");

                //-------------------各种文件的处理结束---------------------//

            }
            catch (Exception e)
            {
                AJLog4NetLogger.Instance().Error($"辅助备份出错：{e.Message}.db", e);
            }
        }

        public static void CopyConfigFile(string backupPath, string fileName)
        {
            var configRoot = $".\\";
            var path = $"{configRoot}{fileName}";
            if (File.Exists(path))
            {
                var fName = Path.Combine(backupPath, Path.GetFileName(path));
                CopyFile(path, fName);
            }
        }

        public static void CopyFile(string initial, string destination)
        {
            if (File.Exists(initial))
            {
                File.Copy(initial, destination);
            }
        }

        public static async void DBRestoreAsync(IWindowManager windowManager)
        {
            using var ctrl = new CommonController();
            var loadingDialog = new AJCommonLoadingDialogViewModel("正在准备...", () =>
            {
                var result = new ProcessResult();

                var curDbVal = ctrl.GetCurrentDbType();
                result.SetSuccess(curDbVal);
                return Task.FromResult(result);
            });

            windowManager.ShowDialog(loadingDialog);

            var curDb = loadingDialog.Result.Data as string;

            var isMySql = curDb.Equals("mysql", StringComparison.OrdinalIgnoreCase);

            var _mainSetting = SettingsHelper.AWSV2Settings;

            var fileExt = isMySql ? ".sql" : ".db";
            var currentDBPath = _mainSetting.Settings["BackupPath"].TryGetString(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Backup"));
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = currentDBPath,
                CheckFileExists = true,
                AddExtension = true,
                DefaultExt = fileExt,
                Filter = $"数据库备份（.{fileExt}）|*{fileExt}"
            };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var result = await RestoreDataBaseCoreAsync(dialog.FileName);
                if (!result.Success)
                {
                    AJLog4NetLogger.Instance().Error($"还原数据库失败", result.Data as Exception);
                }

                MessageBox.Show(result.Success
                    ? "还原数据库成功"
                    : $"还原数据库失败:{result.Message}", "提示", MessageBoxButton.OK);

            }
        }

        //public static void BackupSettings()
        //{
        //    CommonSaveFileDialog dialog = new CommonSaveFileDialog
        //    {
        //        InitialDirectory = _mainSetting.Settings["BackupPath"]?.Value ?? string.Empty,
        //        DefaultFileName = "SettingsBackup",
        //        DefaultExtension = "xml",
        //        AlwaysAppendDefaultExtension = true
        //    };

        //    var dialog = new Microsoft.Win32.SaveFileDialog
        //    {
        //        InitialDirectory = _mainSetting.Settings["BackupPath"]?.Value ?? string.Empty,
        //        FileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{DbService.DbName}",
        //        AddExtension = true,
        //        CreatePrompt = true,
        //        OverwritePrompt = true,
        //        DefaultExt = fileExt,
        //        Filter = $"数据库备份（.{fileExt}）|*{fileExt}",
        //    };

        //    dialog.Filters.Add(new CommonFileDialogFilter("xml文件", ".xml"));

        //    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        //    {
        //        try
        //        {
        //            XmlTextWriter xmlWriter = new XmlTextWriter(dialog.FileName, Encoding.UTF8);//创建一个xml文档
        //            xmlWriter.Formatting = Formatting.Indented;
        //            xmlWriter.WriteStartDocument();
        //            xmlWriter.WriteStartElement("Settings");

        //            xmlWriter.WriteStartElement("篮圈AVS1");
        //            XmlDocument xml = new XmlDocument();
        //            xml.Load("篮圈AVS1.exe.Config");
        //            XmlNode root = xml.SelectSingleNode("/configuration/appSettings");
        //            XmlNodeList childlist = root.ChildNodes;
        //            foreach (XmlNode node in childlist)
        //            {
        //                xmlWriter.WriteStartElement("add");
        //                xmlWriter.WriteAttributeString("key", node.Attributes["key"].Value);
        //                xmlWriter.WriteAttributeString("value", node.Attributes["value"].Value);
        //                xmlWriter.WriteEndElement();
        //            }
        //            xmlWriter.WriteEndElement();

        //            xmlWriter.WriteStartElement("大屏幕");
        //            xml.Load("大屏幕.exe.config");
        //            root = xml.SelectSingleNode("/configuration/appSettings");
        //            childlist = root.ChildNodes;
        //            foreach (XmlNode node in childlist)
        //            {
        //                xmlWriter.WriteStartElement("add");
        //                xmlWriter.WriteAttributeString("key", node.Attributes["key"].Value);
        //                xmlWriter.WriteAttributeString("value", node.Attributes["value"].Value);
        //                xmlWriter.WriteEndElement();
        //            }
        //            xmlWriter.WriteEndElement();

        //            xmlWriter.WriteStartElement("车牌识别");
        //            xml.Load("车牌识别.exe.config");
        //            root = xml.SelectSingleNode("/configuration/appSettings");
        //            childlist = root.ChildNodes;
        //            foreach (XmlNode node in childlist)
        //            {
        //                xmlWriter.WriteStartElement("add");
        //                xmlWriter.WriteAttributeString("key", node.Attributes["key"].Value);
        //                xmlWriter.WriteAttributeString("value", node.Attributes["value"].Value);
        //                xmlWriter.WriteEndElement();
        //            }
        //            xmlWriter.WriteEndElement();

        //            //xmlWriter.WriteStartElement("监控");
        //            //xml.Load("监控.exe.config");
        //            //root = xml.SelectSingleNode("/configuration/appSettings");
        //            //childlist = root.ChildNodes;
        //            //foreach (XmlNode node in childlist)
        //            //{
        //            //    xmlWriter.WriteStartElement("add");
        //            //    xmlWriter.WriteAttributeString("key", node.Attributes["key"].Value);
        //            //    xmlWriter.WriteAttributeString("value", node.Attributes["value"].Value);
        //            //    xmlWriter.WriteEndElement();
        //            //}
        //            //xmlWriter.WriteEndElement();

        //            xmlWriter.WriteStartElement("QuickPlate");
        //            xml.Load("车牌识别.exe.config");
        //            root = xml.SelectSingleNode("/configuration/appSettings");
        //            childlist = root.ChildNodes;
        //            foreach (XmlNode node in childlist)
        //            {
        //                xmlWriter.WriteStartElement("add");
        //                xmlWriter.WriteAttributeString("key", node.Attributes["key"].Value);
        //                xmlWriter.WriteAttributeString("value", node.Attributes["value"].Value);
        //                xmlWriter.WriteEndElement();
        //            }
        //            xmlWriter.WriteEndElement();

        //            //xmlWriter.WriteStartElement("ZXSyncWeighingData");
        //            //xml.Load("ZXSyncWeighingData.exe.config");
        //            //root = xml.SelectSingleNode("/configuration/appSettings");
        //            //childlist = root.ChildNodes;
        //            //foreach (XmlNode node in childlist)
        //            //{
        //            //    xmlWriter.WriteStartElement("add");
        //            //    xmlWriter.WriteAttributeString("key", node.Attributes["key"].Value);
        //            //    xmlWriter.WriteAttributeString("value", node.Attributes["value"].Value);
        //            //    xmlWriter.WriteEndElement();
        //            //}
        //            //xmlWriter.WriteEndElement();

        //            xmlWriter.WriteStartElement("磅单记录");
        //            xml.Load("磅单记录.exe.config");
        //            root = xml.SelectSingleNode("/configuration/appSettings");
        //            childlist = root.ChildNodes;
        //            foreach (XmlNode node in childlist)
        //            {
        //                xmlWriter.WriteStartElement("add");
        //                xmlWriter.WriteAttributeString("key", node.Attributes["key"].Value);
        //                xmlWriter.WriteAttributeString("value", node.Attributes["value"].Value);
        //                xmlWriter.WriteEndElement();
        //            }
        //            xmlWriter.WriteEndElement();

        //            xmlWriter.WriteEndElement();
        //            xmlWriter.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            log.Debug(e.Message);
        //        }
        //    }
        //}

        //public static void RestoreSettings()
        //{
        //    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        //    dialog.Filters.Add(new CommonFileDialogFilter("xml文件", ".xml"));
        //    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        //    {
        //        XmlDocument xml = new XmlDocument();
        //        xml.Load(dialog.FileName);

        //        XmlNodeList childlist = xml.SelectSingleNode("Settings").ChildNodes;

        //        foreach (XmlNode node in childlist)
        //        {
        //            XmlDocument configxml = new XmlDocument();
        //            string path = string.Format("{0}\\{1}.exe.Config", Environment.CurrentDirectory, node.Name);
        //            configxml.Load(path);
        //            XmlNodeList configAddList = configxml.SelectSingleNode("/configuration/appSettings").ChildNodes;

        //            XmlElement xe = (XmlElement)node;
        //            XmlNodeList nls = xe.ChildNodes; //备份文件中的add列表
        //            foreach (XmlNode xn in nls)
        //            {
        //                foreach (XmlNode configxn in configAddList)
        //                {
        //                    if (configxn.Attributes["key"].Value == xn.Attributes["key"].Value)
        //                    {
        //                        configxn.Attributes["value"].Value = xn.Attributes["value"].Value;
        //                        break;
        //                    }
        //                }
        //            }
        //            configxml.Save(path);
        //        }
        //    }
        //}
    }
}
