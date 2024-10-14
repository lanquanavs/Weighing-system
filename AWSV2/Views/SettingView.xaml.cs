﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AWSV2.Views
{
    /// <summary>
    /// SettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingView
    {
        //public List<Button> ButtonList = new List<Button>();
        //public List<StackPanel> AreaList = new List<StackPanel>();

        //public string bdczszImg;
        //public string znhxtszImg;


        public SettingView()
        {
            //WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            //ButtonList.Add(this.FindName("cpsb") as Button);
            //ButtonList.Add(this.FindName("dzkz") as Button);
            //ButtonList.Add(this.FindName("yxzp") as Button);
            //ButtonList.Add(this.FindName("yykz") as Button);
            //ButtonList.Add(this.FindName("hldkz") as Button);
            //ButtonList.Add(this.FindName("swdp") as Button);
            //ButtonList.Add(this.FindName("dzwl") as Button);
            //ButtonList.Add(this.FindName("gbsf") as Button);

            //ButtonList.Add(this.FindName("yblj") as Button);
            //ButtonList.Add(this.FindName("czms") as Button);
            //ButtonList.Add(this.FindName("bdsz") as Button);
            //ButtonList.Add(this.FindName("dysz") as Button);
            //ButtonList.Add(this.FindName("kzbl") as Button);
            //ButtonList.Add(this.FindName("sjtb") as Button);
            //ButtonList.Add(this.FindName("sjbf") as Button);
            //ButtonList.Add(this.FindName("czbj") as Button);


            //AreaList.Add(this.FindName("cpsbArea") as StackPanel);
            //AreaList.Add(this.FindName("dzkzArea") as StackPanel);
            //AreaList.Add(this.FindName("yxzpArea") as StackPanel);
            //AreaList.Add(this.FindName("yykzArea") as StackPanel);
            //AreaList.Add(this.FindName("hldkzArea") as StackPanel);
            //AreaList.Add(this.FindName("swdpArea") as StackPanel);
            //AreaList.Add(this.FindName("dzwlArea") as StackPanel);
            //AreaList.Add(this.FindName("gbsfArea") as StackPanel);


            //AreaList.Add(this.FindName("ybljArea") as StackPanel);
            //AreaList.Add(this.FindName("czmsArea") as StackPanel);
            //AreaList.Add(this.FindName("bdszArea") as StackPanel);
            //AreaList.Add(this.FindName("dyszArea") as StackPanel);
            //AreaList.Add(this.FindName("kzblArea") as StackPanel);
            //AreaList.Add(this.FindName("sjtbArea") as StackPanel);
            //AreaList.Add(this.FindName("sjbfArea") as StackPanel);
            //AreaList.Add(this.FindName("czbjArea") as StackPanel);




            //var btn = this.FindName("znhxtsz") as Button;
            //LabelClick(btn, null);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri)
            {
                UseShellExecute = true,
            });
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

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PopPanel1.IsOpen = true;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            PopPanel2.IsOpen = true;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            PopPanel3.IsOpen = true;
        }
    }
}