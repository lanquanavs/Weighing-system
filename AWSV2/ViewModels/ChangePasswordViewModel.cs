using Common.EF.Controllers;
using Common.Utility;
using Common.Utility.AJ;
using log4net;
using NPOI.SS.Formula.Functions;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace AWSV2.ViewModels
{
    public class ChangePasswordViewModel : Screen
    {

        public string OldPwd { get; set; }
        public string NewPwd { get; set; }
        public string ReNewPwd { get; set; }

        public string ActiveInfo { get; set; }

        public string VersionName
        {
            get
            {
                return Application.ResourceAssembly.GetName().Version.ToString();
            }
        }

        private IWindowManager _windowManager;
        private IEventAggregator _eventAggregator;
        private AppSettingsSection _mainSetting;

        private bool _fromLogin;
        public bool FromLogin
        {
            get => _fromLogin;
            set => SetAndNotify(ref _fromLogin, value);
        }

        public ChangePasswordViewModel(IWindowManager windowManager,
            IEventAggregator eventAggregator, bool fromLogin = true)
        {
            _fromLogin = fromLogin;
            if (!fromLogin)
            {
                UserId = Globalspace._currentUser.UserId.ToString();
            }
            _mainSetting = SettingsHelper.AWSV2Settings;
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;

            ActiveInfo = CloudAPI.APPLICENSEINFO.IsActive ? "已激活" : "未激活";

            Prepare();

        }

        private void Prepare()
        {

        }

        public void Exit()
        {
            RequestClose();
            if (_fromLogin)
            {
                _windowManager.ShowWindow(new LoginViewModel(_windowManager, _eventAggregator));
            }
        }

        public bool Loading { get; set; }

        private bool _canConfirm;
        public bool CanConfirm
        {
            get
            {
                _canConfirm = !Loading
                    && !string.IsNullOrWhiteSpace(OldPwd)
                    && !string.IsNullOrWhiteSpace(NewPwd)
                    && !string.IsNullOrWhiteSpace(ReNewPwd);
                return _canConfirm;
            }
            set
            {
                _canConfirm = value;
            }
        }

        public string ErrorInfo { get; set; }

        public string UserId { get; set; }

        public async void Confirm()
        {
            Loading = true;
            CanConfirm = false;

            using var ctrl = new UserController();

            var user = await ctrl.Detail(UserId, string.Empty);

            Loading = false;
            CanConfirm = true;
            ErrorInfo = string.Empty;

            if (user == null || user.LoginPwd != OldPwd)
            {
                ErrorInfo = "指定账户名或旧密码不正确";
                return;
            }

            user.LoginPwd = NewPwd;
            try
            {
                var result = await ctrl.Save(user);
                Exit();
            }
            catch (Exception e)
            {
                ErrorInfo = e.Message;
                return;
            }
            
        }
    }
}
