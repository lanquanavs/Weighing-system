using AWSV2.Services;
using AWSV2.Views;
using NPOI.SS.Formula.Functions;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;
using static NPOI.SS.Formula.Functions.LinearRegressionFunction;

namespace AWSV2.ViewModels
{
    public class BackupViewModel : Screen
    {
        public string TextInfo { get; set; }
        public string ErrorInfo { get; set; }
        public System.Windows.Media.Brush StateForeground { get; set; } = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#000000"));
        private static readonly log4net.ILog log = LogHelper.GetLogger();
        /// <summary>
        ///窗体类型 暂无定义，好像这个还有点用，先留着吧。
        /// </summary>
        private int WindowType { get; set; }

        /// <summary>
        /// 滚动条的当前运行值
        /// </summary>
        public int ProgressValue { get; set; } = 0;

        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="type">1、数据库文件备份。2、未知。3、未知。4、未知。先默认为1吧</param>
        public BackupViewModel(int type = 1)
        {
            //定义此次任务的类型，见上参数说明
            WindowType = type;
            ManualBackup();
        }


        public void Cancel()
        {
            this.RequestClose(false);
        }

        public void ManualBackup()
        {
            try
            {
                Task.Run(() =>
                {

                    RunAction(() => { ProgressValue = 8; });
                    Thread.Sleep(300);
                    RunAction(() => { ProgressValue = 19; });
                    Thread.Sleep(300);
                    RunAction(() => { ProgressValue = 36; });
                    RunAction(() => { 
                        
                        BackupHelper.AssistBackup();
                        log.Info("用户 " + Globalspace._currentUser.UserName + "安全（已完成备份） 退出系统");
                    });
                    RunAction(() => { ProgressValue = 53; });
                    Thread.Sleep(300);
                    RunAction(() => { ProgressValue = 67; });
                    Thread.Sleep(300);
                    RunAction(() => { ProgressValue = 100; });
                    Thread.Sleep(300);
                    //这些滚动效果，全都是假的。。。
                    RunAction(() => { this.RequestClose(true); });
                    //this.RequestClose(false);
                });
            }
            catch (Exception e)
            {
                ErrorInfo = $"备份出错：{e.Message}";
                this.RequestClose(false);
            }
        }

        private void RunAction(Action act)
        {
            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    if (act != null)
                    {
                        act();
                    }
                });
            }
        }

    }
}
