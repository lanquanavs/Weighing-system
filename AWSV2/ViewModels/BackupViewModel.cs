using AWSV2.Services;
using AWSV2.Views;
using Common.Utility;
using Common.Utility.AJ;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;


namespace AWSV2.ViewModels
{
    public class BackupViewModel : Screen
    {
        public string TextInfo { get; set; } = "正在安全退出...";
        public string ErrorInfo { get; set; }

        /// <summary>
        /// 滚动条的当前运行值
        /// </summary>
        public double ProgressValue { get; set; }

        private int[] _keyPoints;

        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="type">1、数据库文件备份。2、未知。3、未知。4、未知。先默认为1吧</param>
        public BackupViewModel(int type = 1)
        {
            // 第一个关键点显示开始备份, 第二个关键点显示备份完成, 其他任何时候显示安全退出
            _keyPoints = new int[2] { AJUtil.RandomRange(30, 60), AJUtil.RandomRange(61, 90) };

            ManualBackup();
        }

        public void ManualBackup()
        {
            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
            };

            worker.DoWork += (s, e) =>
            {
                for (int i = 1; i <= 100; i++)
                {
                    worker.ReportProgress(i);
                    if (_keyPoints[1] == i)
                    {
                        BackupHelper.AssistBackupAsync();
                        AJLog4NetLogger.Instance().Info("用户 " + Globalspace._currentUser.UserName + "安全（已完成备份） 退出系统");
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
                }
            };

            worker.ProgressChanged += (s, e) =>
            {
                ProgressValue = e.ProgressPercentage;

                if (ProgressValue == _keyPoints[0])
                {
                    TextInfo = "正在备份...";
                }
                else if (ProgressValue == _keyPoints[1])
                {
                    TextInfo = "备份成功!";
                }
                else
                {
                    TextInfo = "正在安全退出...";
                }
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                var hasError = e.Error != null;
                if (hasError)
                {
                    AJLog4NetLogger.Instance().Error($"备份数据库失败:{e.Error.Message}", e.Error);
                    TextInfo = $"备份数据库失败,请查看日志";
                }
                Thread.Sleep(hasError ? 3000 : 1000);
                RequestClose(true);
            };

            worker.RunWorkerAsync();

        }

    }
}
