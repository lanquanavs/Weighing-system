using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AWSV2.Models;
using AWSV2.Services;

namespace AWSV2.ViewModels
{
    public class ConfirmWithPwdViewModel : Screen
    {
        public string TextInfo { get; set; }
        public bool InfoBarVisible { get; set; } = false;

        public UserModel CurrentUser { get; private set; } = Globalspace._currentUser;

        public void Confirm(object pwd)
        {
            var Password = ((PasswordBox)pwd).Password;

            if (CurrentUser != null) //根据打开程序时的LoginId参数查询到UserModel
            {
                if (CurrentUser.LoginPwd == null)
                {
                    CurrentUser.LoginPwd = "";
                }
                if (CurrentUser.LoginPwd.Equals(Password)) //用户名密码相同，验证通过，保存到全局，打开主窗口
                {
                    this.RequestClose(true);
                }
            }

            InfoBarVisible = true;
            TextInfo = "密码错误，请重试！";
        }
    }
}
