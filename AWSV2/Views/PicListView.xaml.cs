using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// PasswordView.xaml 的交互逻辑
    /// </summary>
    public partial class PicListView
    {
        public PicListView()
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(Globalspace.ImagsSource))
            {
                //pl.ItemSourcePath = Globalspace.ImagsSource;
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {

        }

        private void MetroWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void pl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                //var path = (e.AddedItems[0] as AWSControls.ListBindData).ItemPath;
                //Process.Start(path);
            }
        }
    }
}
