using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AWSV2.Views
{
    /// <summary>
    /// SettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SystemView
    {


        public SystemView()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
        private void MinButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        private void MaxButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //var path = "";
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                //path = @"pack://application:,,,/Resources/Img/fd_c.png";
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
                //path = @"pack://application:,,,/Resources/Img/fd_b.png";
            }
            //ImageBrush brush = new ImageBrush();
            //var img = new BitmapImage(new Uri(path, UriKind.Absolute));
            //brush.ImageSource = img;
            //var MaxButton = sender as Button;
            //MaxButton.Background = brush;
        }

        
    }
}
