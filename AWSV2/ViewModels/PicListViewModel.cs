using AWSV2.Services;
using NPOI.SS.Formula.Functions;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace AWSV2.ViewModels
{
    public class PicListViewModel : Screen
    {
        public string TextInfo { get; set; }
        public string ErrorInfo { get; set; }
        public System.Windows.Media.Brush StateForeground { get; set; } = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#000000"));


        public string dirctoryPath;
        public string DirctoryPath 
        { get {
                return dirctoryPath;
            }
            set
            {
                SetAndNotify(ref dirctoryPath, value);
            }
        }

        //加载其他窗口
        private IWindowManager _windowManager;
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="dirctoryPath">图片的文件夹路径</param>
        public PicListViewModel(IWindowManager windowManager, string dirctoryPath)
        {
            _windowManager = windowManager;
            DirctoryPath = dirctoryPath;
            TextInfo = dirctoryPath;
            this.Closed += ConfirmWithChargeViewModel_Closed;
        }

        private void ConfirmWithChargeViewModel_Closed(object sender, CloseEventArgs e)
        {
            //为了不让这段代码独守空房，索性加个注释吧。。。
        }


        private void BackupWindow_Closed(object sender, CloseEventArgs e)
        {
            this.RequestClose(true);
        }

        public void Cancel()
        {
            this.RequestClose(false);
        }
    }
}
