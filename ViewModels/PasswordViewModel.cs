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
    public class PasswordViewModel : Screen
    {
        public string TextInfo { get; set; }
        public string ErrorInfo { get; set; }
        public System.Windows.Media.Brush StateForeground { get; set; } = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#000000"));

        /// <summary>
        ///窗体类型 1、关闭软件。2、切换用户
        /// </summary>
        private int WindowType { get; set; }

        /// <summary>
        /// 是否指定只能是admin这个管理员账号
        /// </summary>
        private bool IsAdmin { get; set; } = false;

        //加载其他窗口
        private IWindowManager _windowManager;
        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="type">1、关闭软件。2、切换用户。3、用户验证</param>
        public PasswordViewModel(IWindowManager windowManager, int type,bool isAdmin=false)
        {
            _windowManager = windowManager;
            IsAdmin = isAdmin;
            WindowType = type;
            TextInfo = WindowType == 1 ? $"安全退出" : WindowType == 2 ? "确认交班":"用户验证";
            this.Closed += ConfirmWithChargeViewModel_Closed;
        }

        private void ConfirmWithChargeViewModel_Closed(object sender, CloseEventArgs e)
        {
            //为了不让这段代码独守空房，索性加个注释吧。。。
        }

        public void Confirm(object pwd)
        {
            var Password = ((PasswordBox)pwd).Password;
            var inputUser = SQLDataAccess.LoadActiveUser().Find(u => u.UserName == AWSV2.Globalspace._currentUser.UserName && u.UserId == AWSV2.Globalspace._currentUser.UserId);

            if (inputUser != null) //从用户列表中查询到页面上获取到的用户
            {
                if (inputUser.LoginPwd.Equals(Password)) //密码为空或相同，验证通过
                {
                    if (WindowType == 1)
                    {
                        if (_windowManager != null)
                        {
                            var backupWindow = new BackupViewModel(1);
                            // backupWindow.Closed += BackupWindow_Closed;
                            var result = _windowManager.ShowDialog(backupWindow);
                            this.RequestClose(result);
                        }
                        else
                        {
                            this.RequestClose(true);
                        }
                    }
                    else if (WindowType == 3)//用户验证
                    {
                       var CurrentUser = Globalspace._currentUser;

                        if (IsAdmin&& CurrentUser.LoginId.ToLower()!="admin")
                        {
                            ErrorInfo = "你无权执行该操作！";
                            return;
                        }
                        
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
                        ErrorInfo = "提示：请输入正确密码";
                    }
                    else
                    {
                        this.RequestClose(true);
                    }
                }
                else
                {
                    ErrorInfo = "提示：请输入正确密码";
                }
            }
            else
            {
                ErrorInfo = "提示：当前登录用户失效。";
            }
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
