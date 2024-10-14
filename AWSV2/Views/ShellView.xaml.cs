using AWSV2.Services;
using AWSV2.ViewModels;
using Common.Utility;
using Common.Utility.AJ;
using Common.Utility.AJ.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AWSV2.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView
    {
        string[] btnContents = new string[3] { "称重", "一次称重", "二次称重" };

        public ShellView()
        {
            Width = SystemParameters.WorkArea.Size.Width;
            Height = SystemParameters.WorkArea.Size.Height;
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        //MainAPP通过此窗口句柄传递过来的消息
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return hwnd;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        public void showercode(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DragMove();

            base.OnMouseLeftButtonDown(e);
        }

        private void DataGrid_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

    }
}
