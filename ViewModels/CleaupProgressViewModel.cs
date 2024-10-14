using AWSV2.Services;
using Common.Model;
using NPOI.SS.Formula.Functions;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace AWSV2.ViewModels
{
    public class CleaupProgressViewModel : Screen
    {

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetAndNotify(ref _text, value);
            }
        }

        private double _total;
        public double Total
        {
            get
            {
                return _total;
            }
            set
            {
                SetAndNotify(ref _total, value);
            }
        }

        private double _current;
        public double Current
        {
            get
            {
                return _current;
            }
            set
            {
                SetAndNotify(ref _current, value);
            }
        }

        private CleanupConfig _cleanupCfg;

        private IWindowManager _windowManager;
        /// <summary>
        /// 创建窗体
        /// </summary>
        public CleaupProgressViewModel(IWindowManager windowManager, CleanupConfig cleanupCfg)
        {
            _windowManager = windowManager;
            _cleanupCfg = cleanupCfg;
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += (s, e) =>
            {
                _cleanupCfg.Process((progress) =>
                {
                    worker.ReportProgress(0, progress);
                    e.Result = progress;
                });

            };

            worker.ProgressChanged += (s, e) =>
            {
                var m = e.UserState as CleanupConfig.ProgressReportModel;

                Text = m.Text;
                Current = m.Current;
                Total = m.Total;
            };

            worker.RunWorkerCompleted += async (s, e) =>
            {
                var ret = e.Result as CleanupConfig.ProgressReportModel;
                Text = e.Error?.Message ?? ret.Text;

                await Task.Delay(2500);

                this.RequestClose();

            };

            worker.RunWorkerAsync();
        }


    }
}
