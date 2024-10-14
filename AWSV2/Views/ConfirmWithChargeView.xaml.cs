using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static NPOI.HSSF.Util.HSSFColor;

namespace AWSV2.Views
{
    /// <summary>
    /// ConfirmWithChargeView.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmWithChargeView 
    {
        public ConfirmWithChargeView()
        {
            InitializeComponent();
            //// 设置全屏
            //this.WindowState = System.Windows.WindowState.Maximized;//不显示边框，只显示工作区
            //this.WindowStyle = System.Windows.WindowStyle.None;//无边框
            //this.ResizeMode = System.Windows.ResizeMode.NoResize;//禁止大小调整


            ////窗口全屏大小设置，通过传入参数获得主界面窗口大小，进行软件界面大小蒙版，不是全屏覆盖
            //this.Left = 0.0;
            //this.Top = 0.0;
            //this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;//获取屏幕宽度大小
            //this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;


            //this.Left = 0; //设置位置
            //this.Top = 0;
            //this.Width = SystemParameters.WorkArea.Width;
            //this.Height = SystemParameters.WorkArea.Height;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
          
        }
    }
}
