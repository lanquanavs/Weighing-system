using AWSV2.Services;
using AWSV2.Services.Encrt;
using Common.Model.Custom;
using Common.Models;
using Common.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MaterialDesignExtensions.Controls;

namespace AWSV2.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : MaterialWindow
    {

        public LoginView()
        {
            #region 检测

            string MName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
            string PName = System.IO.Path.GetFileNameWithoutExtension(MName);
            System.Diagnostics.Process[] myProcess = System.Diagnostics.Process.GetProcessesByName(PName);

            if (myProcess.Length > 1)
            {
                // 某客户定制,需要多开 --阿吉 2024年10月8日13点51分
                //MessageBox.Show("程序已运行，不能再次打开！", "提示");
                //Application.Current.Shutdown();
                //return;
            }

            #endregion

            ////以下是服务商接口访问
            //var platformTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            //platformTimer.Start();
            //platformTimer.Tick += (sender, args) =>
            //{
            //    platformTimer.Stop();
            //    var config = SettingsHelper.AWSV2Settings;
            //    if (Common.Encrt.WebApi.Ping())
            //    {
            //        var providerDetails = Common.SyncData.GetServiceProvider();
            //        AWSV2.Globalspace.Provicer = providerDetails;
            //        AWSV2.Globalspace.ProvicerTitle = $"{providerDetails.service_products}  |  {providerDetails.company_name}  服务热线：{providerDetails.mobile}";
            //        log.Debug("服务商接口访问成功！");
            //        //将获取的数据保存到配置文件里
            //        config.Settings["Provicer"].Value = JsonConvert.SerializeObject(providerDetails);
            //        SettingsHelper.UpdateAWSV2("Provicer", JsonConvert.SerializeObject(providerDetails));
            //    }
            //    else
            //    {
            //        var provicer = config.Settings["Provicer"].Value;
            //        if (!string.IsNullOrWhiteSpace(provicer))
            //        {
            //            var providerDetails = JsonConvert.DeserializeObject<ServiceProvider>(provicer);
            //            AWSV2.Globalspace.Provicer = providerDetails;
            //            AWSV2.Globalspace.ProvicerTitle = $"{providerDetails.service_products}  |  {providerDetails.company_name}  服务热线：{providerDetails.mobile}";
            //            log.Debug("服务商数据本地获取！");
            //        }
            //        else
            //        {
            //            log.Debug("服务商本地为空！");
            //        }
            //    }
            //};

            InitializeComponent();
            //if (Convert.ToBoolean(ConfigurationManager.AppSettings["StorePwd"]))
            //{
            //    pwd.Password = Properties.Settings.Default.LastUserPwd;
            //}
            //Resources["OldPasswordHintText"] = "请输入原密码";
            //Resources["NewPasswordHintText"] = "请输入新密码";
            //Resources["ReNewPasswordHintText"] = "请输入新密码确认";

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





        //private void pwd_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    if (pwd.Password.Length <= 0)
        //    {
        //        Resources["HintText"] = "默认密码：123";
        //    }
        //    else
        //    {
        //        Resources["HintText"] = "";
        //    }
        //}
        //private void oldpwd_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    Resources["OldPasswordHintText"] = (oldpwd.Password.Length <= 0) ? "请输入原密码" : "";
        //}

        //private void newpwd_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    Resources["NewPasswordHintText"] = (newpwd.Password.Length <= 0) ? "请输入原密码" : "";
        //}
        //private void renewpwd_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    Resources["ReNewPasswordHintText"] = (renewpwd.Password.Length <= 0) ? "请输入原密码" : "";
        //}

        //private void OpenChangePassword(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    OpacityMask.Visibility = Visibility.Visible;
        //    xgmm.Visibility = Visibility.Visible;
        //}

        //private void CloseChangePassword(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    OpacityMask.Visibility = Visibility.Hidden;
        //    xgmm.Visibility = Visibility.Hidden;
        //}
        //private void OpenDataSet(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    OpacityMask.Visibility = Visibility.Visible;
        //    sjklj.Visibility = Visibility.Visible;
        //}

        //public void CloseDataSet(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    OpacityMask.Visibility = Visibility.Hidden;
        //    sjklj.Visibility = Visibility.Hidden;
        //}

        //private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        //    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        //    {
        //        txtFilePath.Text = dialog.FileName;
        //    }
        //}
    }
}
