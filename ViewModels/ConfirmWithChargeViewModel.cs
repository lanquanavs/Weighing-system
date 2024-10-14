using AWSV2.Services;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AWSV2.ViewModels
{
    public class ConfirmWithChargeViewModel : Screen
    {
        public System.Windows.WindowState _SDWindowState = System.Windows.WindowState.Maximized;
        public System.Windows.WindowState SDWindowState
        {
            get { return _SDWindowState; }
            set
            {
                _SDWindowState = value;
            }
        }


        public string TextInfo { get; set; }
        Models.WeighingRecordModel _record;

        bool _EnableFree = true;
        public bool EnableFree
        {
            get
            {

                return _EnableFree;
            }
            set
            {

                _EnableFree = value;
                if (value)
                {
                    StateForeground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#000000"));
                }
                else
                {
                    StateForeground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC"));
                }
            }
        }
        public System.Windows.Media.Brush StateForeground { get; set; } = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#000000"));

        public ConfirmWithChargeViewModel(Models.WeighingRecordModel record)
        {

            //检查用户称重操作权限
            string rolePermission = SQLDataAccess.LoadLoginRolePermission(Globalspace._currentUser.LoginId);
            if (rolePermission != null)
            {
                if (!rolePermission.Contains("免费放行")) EnableFree = false;
            }

            _record = record;
            TextInfo = $"{record.Je}元";
            this.Closed += ConfirmWithChargeViewModel_Closed;

            if (record.Je == "0")
            {
                SDWindowState = WindowState.Minimized;
                var delayRunTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
                delayRunTimer.Start();
                delayRunTimer.Tick += (sender, args) =>
                {
                    delayRunTimer.Stop();

                    Confirm();
                };
            }
            else
            {
                SDWindowState = WindowState.Maximized;
            }
        }

        private void ConfirmWithChargeViewModel_Closed(object sender, CloseEventArgs e)
        {

        }

        public void Confirm()
        {
            _record.IsPay = 2;//点击确认按钮就是线下支付
            _record.Zfsj = DateTime.Now;
            this.RequestClose(true);
        }

        public void Cancel()
        {
            _record.IsPay = 4;//点免费按钮就是免费放行
            _record.Zfsj = DateTime.Now;
            this.RequestClose(true);
        }
    }
}
