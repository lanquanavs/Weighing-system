using AWSV2.Models;
using AWSV2.Services;
using AWSV2.Services.Encrt;
using Common.Utility;
using Common.Utility.AJ;
using Common.Utility.AJ.MobileConfiguration;
using FluentValidation;
using IWshRuntimeLibrary;
using log4net;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Stylet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using File = System.IO.File;

//记录log必须的语句，整个项目只用加这一次
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace AWSV2.ViewModels
{
    public class LoginViewModel : Screen
    {
        private bool showShellView = true;
        //log
        private static ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //加载其他窗口
        private IWindowManager windowManager;

        //界面上绑定的属性
        public List<UserModel> UserList { get; set; }
        public UserModel User { get; set; }
        public bool EnableLogin { get; set; } = true;
        public bool ShowProgressBar { get; set; }
        private string _TextInfo;
        public string TextInfo { get { return _TextInfo; } set { _TextInfo = $"{value}"; } }


        public string VersionName
        {
            get
            {
                var v = _mainSetting.Settings["Version"]?.Value ?? string.Empty;
                return $"V{v} {Common.Share.VersionControl.CurrentVersion.ToString()}";
            }
        }

        public bool InfoBarVisible { get; set; }

        private string dbPath;
        public string DBPath
        {
            get { return dbPath; }
            set
            {
                dbPath = value;
                if (showShellView)
                {


                    //           < add name = "AWSDB" connectionString = "Data Source=.\Data\AWS.db;initial catalog=main;Version=3;" providerName = "System.Data.SqlClient" />

                    //< add name = "LogDB" connectionString = "Data Source=.\Data\Log.db;Version=3;" providerName = "System.Data.SqlClient" />

                    //     < add name = "AWSMYSQL" connectionString = "Data Source=127.0.0.1;port=3306;initial catalog=aws;userid=root;password=root" />


                    Configuration config = SettingsHelper.AWSV2Settings.CurrentConfiguration;

                    //config.AppSettings.Settings["CurrentDBPath"].Value = value;
                    ////同时修改连接字符串
                    //config.ConnectionStrings.ConnectionStrings["AWSDB"].ConnectionString = "Data Source=" + value + ";Version=3;Password=zxhq029;";

                    //config.Save(ConfigurationSaveMode.Modified);
                    ////ConfigurationManager.RefreshSection("appSettings");
                    ////ConfigurationManager.RefreshSection("connectionStrings");

                    //config = ConfigurationManager.OpenExeConfiguration("ZXDataManagement.exe");
                    //config.ConnectionStrings.ConnectionStrings["AWSDB"].ConnectionString = "Data Source=" + value + ";Version=3;Password=zxhq029;";
                    //config.Save(ConfigurationSaveMode.Modified);

                    //config = ConfigurationManager.OpenExeConfiguration("ZXLPR.exe");
                    //config.ConnectionStrings.ConnectionStrings["AWSDB"].ConnectionString = "Data Source=" + value + ";Version=3;Password=zxhq029;";
                    //config.Save(ConfigurationSaveMode.Modified);

                    //config = ConfigurationManager.OpenExeConfiguration("ZXSyncWeighingData.exe");
                    //config.ConnectionStrings.ConnectionStrings["AWSDB"].ConnectionString = "Data Source=" + value + ";Version=3;Password=zxhq029;";
                    //config.Save(ConfigurationSaveMode.Modified);

                    //config = ConfigurationManager.OpenExeConfiguration("ZXWeighingRecord.exe");
                    //config.ConnectionStrings.ConnectionStrings["AWSDB"].ConnectionString = "Data Source=" + value + ";Version=3;Password=zxhq029;";
                    //config.Save(ConfigurationSaveMode.Modified);



                    //同时修改连接字符串
                    config.ConnectionStrings.ConnectionStrings["AWSDB"].ConnectionString = "Data Source=" + value + ";Version=3;";

                    config.Save(ConfigurationSaveMode.Modified);
                    //ConfigurationManager.RefreshSection("appSettings");
                    //ConfigurationManager.RefreshSection("connectionStrings");

                    config = ConfigurationManager.OpenExeConfiguration("ZXDataManagement.exe");
                    config.ConnectionStrings.ConnectionStrings["AWSDB"].ConnectionString = "Data Source=" + value + ";Version=3;";
                    config.Save(ConfigurationSaveMode.Modified);

                    config = SettingsHelper.ZXLPRSettings.CurrentConfiguration;
                    config.ConnectionStrings.ConnectionStrings["AWSDB"].ConnectionString = "Data Source=" + value + ";Version=3;";
                    config.Save(ConfigurationSaveMode.Modified);

                    config = ConfigurationManager.OpenExeConfiguration("ZXSyncWeighingData.exe");
                    config.ConnectionStrings.ConnectionStrings["AWSDB"].ConnectionString = "Data Source=" + value + ";Version=3;";
                    config.Save(ConfigurationSaveMode.Modified);

                    config = ConfigurationManager.OpenExeConfiguration("ZXWeighingRecord.exe");
                    config.ConnectionStrings.ConnectionStrings["AWSDB"].ConnectionString = "Data Source=" + value + ";Version=3;";
                    config.Save(ConfigurationSaveMode.Modified);


                    SettingsHelper.UpdateAWSV2("CurrentDBPath", value);
                    ConfigurationManager.RefreshSection("connectionStrings");

                    log.Info("数据库路径修改为：" + value);
                }
            }
        }
        private string dbSelected;
        public string DBSelected
        {
            get { return dbSelected; }
            set
            {
                dbSelected = value;
                if (showShellView)
                {
                    SettingsHelper.UpdateAWSV2("DBSelect", value);
                }
            }
        }

        public string MySQLIP { get; set; }

        public string Password { get; private set; }

        private bool storePwd;
        public bool StorePwd
        {
            get { return storePwd; }
            set
            {
                storePwd = value;
                if (showShellView)
                {
                    SettingsHelper.UpdateAWSV2("StorePwd", value.ToString());
                }
                if (!value)
                {
                    Properties.Settings.Default.LastLoginId = string.Empty;
                    Properties.Settings.Default.LastUserPwd = string.Empty;
                    Properties.Settings.Default.Save();
                }

            }
        }

        private bool autoStart;
        public bool AutoStart
        {
            get { return autoStart; }
            set
            {
                autoStart = value;
                if (showShellView)
                {

                    SettingsHelper.UpdateAWSV2("AutoStart", value.ToString());

                    if (value)
                    {
                        AJUtil.SetMeAutoStart(true);
                    }
                    else
                    {
                        AJUtil.SetMeAutoStart(false);
                    }
                }
            }
        }

        private bool autoLogin;
        public bool AutoLogin
        {
            get { return autoLogin; }
            set
            {
                autoLogin = value;
                if (showShellView)
                {
                    SettingsHelper.UpdateAWSV2("AutoLogin", value.ToString());
                }
            }
        }

        Common.Model.Custom.ServiceProvider _Provicer;
        public Common.Model.Custom.ServiceProvider Provicer
        {
            get
            {
                //return AWSV2.Globalspace.Provicer;
                return _Provicer;
            }
            set
            {
                _Provicer = value;
            }
        }

        string _ProvicerName;
        public string ProvicerName
        {
            get
            {
                //return $"{AWSV2.Globalspace.Provicer.company_name}@服务商";
                return $"{_ProvicerName}@服务商";
            }
            set
            {
                _ProvicerName = value;
            }
        }

        private AppSettingsSection _mainSetting;
        private IEventAggregator _eventAggregator;

        public LoginViewModel(IWindowManager windowManager, IEventAggregator eventAggregator) : base(new FluentModelValidator<LoginViewModel>(new LoginViewModellValidator()))
        {
            _eventAggregator = eventAggregator;
            _mainSetting = SettingsHelper.AWSV2Settings;
            dbSelected = _mainSetting.Settings["DBSelect"]?.Value ?? string.Empty;
            MySQLIP = SQLDataAccess.DecryptMySqlConnStr().host;
            AWSV2.Globalspace.windowManager = windowManager;
            this.windowManager = windowManager;
            //log.Info("测试登录日志。。。。。。。。。。");
            if (DBSelected == "mysql")
            {
                if (SQLDataAccess.CheckConnectionStatus("AWSMYSQL"))
                {
                    UserList = SQLDataAccess.LoadActiveUser(); //查询出的用户列表，显示到页面上
                }
                else
                {
                    TextInfo = "数据库访问失败，请联系管理员！";
                    InfoBarVisible = true;
                    EnableLogin = false;
                }
            }
            else if (DBSelected == "sqlite")
            {
                if (SQLDataAccess.CheckConnectionStatus())
                {
                    UserList = SQLDataAccess.LoadActiveUser(); //查询出的用户列表，显示到页面上
                }
                else
                {
                    TextInfo = "数据库访问失败，请联系管理员！";
                    InfoBarVisible = true;
                    EnableLogin = false;
                }
            }

            AutoStart = Convert.ToBoolean(_mainSetting.Settings["AutoStart"]?.Value ?? "False");

            if (Convert.ToBoolean(_mainSetting.Settings["StorePwd"]?.Value ?? "False"))
            {
                StorePwd = true;

                var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
                timer.Start();
                timer.Tick += (sender, args) =>
                {
                    timer.Stop();
                    User = UserList?.Find(u => u.LoginId == Properties.Settings.Default.LastLoginId);
                };
            }

            if (Convert.ToBoolean(_mainSetting.Settings["AutoLogin"]?.Value ?? "False"))
            {
                AutoLogin = true;

                var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                timer.Start();
                timer.Tick += (sender, args) =>
                {
                    timer.Stop();
                    PasswordBox pwd = new PasswordBox();
                    pwd.Password = Properties.Settings.Default.LastUserPwd;
                    if (EnableLogin)
                    {
                        Login(pwd);
                    }
                };
            }

            Task.Run(() =>
            {
                try
                {
                    //以下是服务商接口访问
                    var providerDetails = Common.SyncData.GetServiceProvider();
                    AWSV2.Globalspace.Provicer = providerDetails;
                    AWSV2.Globalspace.ProvicerTitle = $"{providerDetails.service_products}  |  {providerDetails.company_name}  服务热线：{providerDetails.mobile}";

                    Provicer = AWSV2.Globalspace.Provicer;
                    ProvicerName = AWSV2.Globalspace.ProvicerTitle;
                    log.Debug("服务商接口访问成功！");
                }
                catch (Exception e)
                {
                    log.Debug($"服务商接口访问Error:{e.Message}！");
                }
            });

        }

        public LoginViewModel(IWindowManager windowManager, bool showShellView, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            AWSV2.Globalspace.windowManager = windowManager;
            this.windowManager = windowManager;
            this.showShellView = showShellView;
            if (DBSelected == "mysql")
            {
                if (SQLDataAccess.CheckConnectionStatus("AWSMYSQL"))
                {
                    UserList = SQLDataAccess.LoadActiveUser(); //查询出的用户列表，显示到页面上
                }
                else
                {
                    TextInfo = "数据库访问失败，请联系管理员！";
                    InfoBarVisible = true;
                    EnableLogin = false;
                }
            }
            else if (DBSelected == "sqlite")
            {
                if (SQLDataAccess.CheckConnectionStatus())
                {
                    UserList = SQLDataAccess.LoadActiveUser(); //查询出的用户列表，显示到页面上
                }
                else
                {
                    TextInfo = "数据库访问失败，请联系管理员！";
                    InfoBarVisible = true;
                    EnableLogin = false;
                }
            }
            Properties.Settings.Default.LastUserPwd = "";
            Properties.Settings.Default.LastLoginId = "";

            ////以下是服务商接口访问
            //var platformTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            //platformTimer.Start();
            //platformTimer.Tick += (sender, args) =>
            //{
            //    platformTimer.Stop();
            //    var providerDetails = Common.SyncData.GetServiceProvider();
            //    AWSV2.Globalspace.Provicer = providerDetails;
            //    AWSV2.Globalspace.ProvicerTitle = $"{providerDetails.service_products}  |  {providerDetails.company_name}  服务热线：{providerDetails.mobile}";

            //    Provicer = AWSV2.Globalspace.Provicer;
            //    ProvicerName = AWSV2.Globalspace.ProvicerTitle;
            //    log.Debug("服务商接口访问成功！");
            //};

            Task.Run(() =>
            {
                try
                {
                    if (Common.Encrt.WebApi.Ping())
                    {
                        //以下是服务商接口访问
                        var providerDetails = Common.SyncData.GetServiceProvider();
                        AWSV2.Globalspace.Provicer = providerDetails;
                        AWSV2.Globalspace.ProvicerTitle = $"{providerDetails.service_products}  |  {providerDetails.company_name}  服务热线：{providerDetails.mobile}";

                        Provicer = AWSV2.Globalspace.Provicer;
                        ProvicerName = AWSV2.Globalspace.ProvicerTitle;
                        log.Debug("服务商接口访问成功！");
                    }
                }
                catch (Exception e)
                {
                    log.Debug($"服务商接口访问Error:{e.Message}！");
                }


            });

        }


        public void Login(object pwd)
        {

            if (!EnableLogin) return;

            //加载过程缓慢，显示进度条
            EnableLogin = false;
            ShowProgressBar = true;

            //此处验证已经迁移至称重程序初始化方法内。该处注释 2022-07-01 by：wangh
            //if (showShellView)//正式登陆窗口才验证，否则在数据备份哪里清理数据库的时候是不需要的。
            //{ //登录时先验证注册码
            //    VerifyLicense();
            //}

            Password = ((PasswordBox)pwd).Password;

            var inputUser = UserList.Find(u => u == User);

            if (inputUser != null) //从用户列表中查询到页面上获取到的用户
            {
                if (inputUser.LoginPwd == null)
                {
                    inputUser.LoginPwd = "";
                }
                if (inputUser.LoginPwd.Equals(Password)) //用户名密码相同，验证通过，保存到全局，打开主窗口
                {
                    EnableLogin = false;

                    if (showShellView)//当窗口是正式登陆窗口的时候该要的逻辑都要运行
                    {
                        Globalspace._currentUser = inputUser;

                        log.Info("用户 " + Globalspace._currentUser.UserName + " 登录系统");

                        Properties.Settings.Default.LastLoginId = User.LoginId;
                        Properties.Settings.Default.LastUserPwd = User.LoginPwd;
                        Properties.Settings.Default.Save();

                        BackupHelper.AutoBackup();
                        AWSV2.Globalspace.windowManager = windowManager;

                        if (File.Exists(Globalspace._weightFormTemplatePath))
                        {
                            var viewModel = new ShellViewModel(windowManager, _eventAggregator);
                            this.windowManager.ShowWindow(viewModel);
                            this.RequestClose();
                        }
                    }
                    else//当窗口被数据备份功能中的数据清理借用的时候,是不需要再次打开主页面
                        //,再次全局赋值，再次备份数据库那些操作的。只要返回模态框的 true返回值
                    {
                        if (inputUser.LoginId.ToLower() != "admin")//只有管理员才有权限
                        {
                            this.RequestClose(false);
                        }
                        else
                        {
                            this.RequestClose(true);
                        }

                    }
                }
            }

            //未查询到用户 或 密码不正确
            ShowProgressBar = false;

            if (File.Exists(Globalspace._weightFormTemplatePath))
            {
                TextInfo = "用户名或密码错误";
            }
            else
            {
                TextInfo = "表单模板文件不存在，请联系供应商！";
            }

            InfoBarVisible = true;
            EnableLogin = true;
        }

        //验证注册码
        private void VerifyLicense()
        {
            MobileConfigurationMgr mobileConfigurationMgr = null;
            if (Common.Encrt.WebApi.Ping())
            {
                log.Debug("远程验证注册码");

                //联网验证
                //if (!ConfigurationManager.AppSettings["RegCode"].Equals(Register.Verify(Register.GetHdInfo())))
                if (!Register.Verify(Register.GetHdInfo(), ref mobileConfigurationMgr))//WEB验证注册码失败
                {
                    var viewModel = new QrCodeViewModel();
                    bool? result = windowManager.ShowDialog(viewModel);
                    //等二维码关闭后再验证一次，防止用户通过二维码注册后，不能即时感知
                    Globalspace._isRegister = Register.Verify(Register.GetHdInfo(), ref mobileConfigurationMgr);

                    //if (!Register.Local_Verify(ConfigurationManager.AppSettings["RegCode"]))
                    //{
                    //    //注册码无效，弹出输入注册码的窗口
                    //    var viewModel = new RegViewModel();
                    //    bool? result = windowManager.ShowDialog(viewModel);
                    //    if (result.GetValueOrDefault(true)) //没有注册成功则设置全局变量
                    //    {
                    //        Globalspace._isRegister = true;
                    //    }
                    //    else
                    //    {
                    //        Globalspace._isRegister = false;
                    //    }
                    //}
                    //else
                    //{
                    //    Globalspace._isRegister = true;
                    //}
                }
                else
                {
                    Globalspace._isRegister = true;
                }
            }
            else
            {
                log.Debug("本地验证注册码");
                //本地验证
                if (!Register.Local_Verify(_mainSetting.Settings["RegCode"]?.Value ?? string.Empty))
                {
                    var viewModel = new QrCodeViewModel();
                    bool? result = windowManager.ShowDialog(viewModel);
                    //等二维码关闭后再验证一次，防止用户通过二维码注册后，不能即时感知
                    //还必须是在联网的状态下才能即时感知，否则只能认为注册失败
                    if (Common.Encrt.WebApi.Ping())
                    {
                        Globalspace._isRegister = Register.Verify(Register.GetHdInfo(), ref mobileConfigurationMgr);
                    }
                    else
                    {
                        Globalspace._isRegister = false;
                    }
                }
                else
                {
                    Globalspace._isRegister = true;
                }
            }
        }

        //public void ChangePwd(object parameter)
        //{
        //    var values = (object[])parameter;

        //    var inputUser = UserList.Find(u => u == User);
        //    Password = ((PasswordBox)values[0]).Password;

        //    if (Password == null || Password == "")
        //    {
        //        TextInfo = "请输入原密码";
        //        return;
        //    }
        //    var uname = ((TextBox)values[3]).Text;
        //    if (uname == null || uname == "")
        //    {
        //        TextInfo = "请输入用户名";
        //        return;
        //    }
        //    string newPwd = ((PasswordBox)values[1]).Password;
        //    string confirmPwd = ((PasswordBox)values[2]).Password;

        //    if (inputUser != null) //从用户列表中查询到页面上获取到的用户
        //    {
        //        if (inputUser.LoginPwd == null)
        //        {
        //            inputUser.LoginPwd = "";
        //        }
        //        if (inputUser.LoginPwd.Equals(Password)) //用户名密码相同，验证通过，可以修改密码
        //        {
        //            if (newPwd == null || newPwd == "")
        //            {
        //                TextInfo = "请输入新密码";
        //                return;
        //            }
        //            if (!newPwd.Equals(confirmPwd))
        //            {
        //                TextInfo = "两次密码不一致，请重新输入。";
        //                return;
        //            }
        //            else
        //            {
        //                inputUser.LoginPwd = newPwd;
        //                SQLDataAccess.UpdateUser(inputUser);
        //                TextInfo = "密码修改成功！";

        //                log.Info("用户 " + inputUser.UserName + " 修改登录密码");
        //            }
        //        }
        //        else
        //        {
        //            TextInfo = "原密码错误";
        //        }
        //    }
        //    else
        //    {
        //        TextInfo = "用户不存在";
        //    }

        //    InfoBarVisible = true;
        //}

        public string UserName { get; set; }
        public string OldPwd { get; set; }
        public string NewPwd { get; set; }
        public string ReNewPwd { get; set; }
        public bool ShowEditPwd { get; set; }
        public string EditMessage { get; set; }
        public void ChangePwd()
        {

            if (!Validate())
            {
                return;
            }

            var inputUser = UserList.Find(u => u == User);

            if (inputUser != null) //从用户列表中查询到页面上获取到的用户
            {
                if (inputUser.LoginPwd == null)
                {
                    inputUser.LoginPwd = "";
                }
                if (OldPwd == null)
                {
                    OldPwd = "";
                }

                //2022-11-16注释掉此段，修改成用loginID来验证
                //if (inputUser.UserName.Equals(UserName))//用户名相同，验证通过，验证密码是否相同

                if (inputUser.LoginId.Equals(UserName))//用户名相同，验证通过，验证密码是否相同
                {
                    if (inputUser.LoginPwd.Equals(OldPwd)) //密码相同，验证通过，可以修改密码
                    {
                        if (string.IsNullOrEmpty(NewPwd))
                        {
                            EditMessage = "请输入新密码";
                            return;
                        }
                        if (!NewPwd.Equals(ReNewPwd))
                        {
                            EditMessage = "两次密码不一致，请重新输入。";
                            return;
                        }
                        else
                        {
                            inputUser.LoginPwd = NewPwd;
                            SQLDataAccess.UpdateUser(inputUser);
                            EditMessage = "密码修改成功！";

                            log.Info("用户 " + inputUser.UserName + " 修改登录密码");
                        }
                    }
                    else
                    {
                        EditMessage = "原密码错误";
                    }
                }
                else { EditMessage = "用户名错误"; }
            }
            else
            {
                EditMessage = "用户不存在";
            }

            InfoBarVisible = true;
        }

        public void LoadDBPath()
        {
            //DBPath = ConfigurationManager.AppSettings["CurrentDBPath"];

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DBPath = dialog.FileName;
            }

            UserList = SQLDataAccess.LoadActiveUser(); //修改数据库路径后重新加载数据库
        }

        public void ShowHelpChm()
        {
            //打开chm文档
            System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\无人值守汽车衡称重系统V2.2 说明书.chm");
        }

        private static string GetIp(string ipString)
        {
            string regEx = "((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)";
            return Regex.Match(ipString, regEx).Value;
        }

        public void SetIP()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AWSMYSQL"].ConnectionString;
            string regEx = "((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)";

            connectionString = Regex.Replace(connectionString, regEx, MySQLIP);

            Configuration config = SettingsHelper.AWSV2Settings.CurrentConfiguration;
            config.ConnectionStrings.ConnectionStrings["AWSMYSQL"].ConnectionString = connectionString;
            config.Save(ConfigurationSaveMode.Modified);

            config = ConfigurationManager.OpenExeConfiguration("ZXDataManagement.exe");
            config.ConnectionStrings.ConnectionStrings["AWSMYSQL"].ConnectionString = connectionString;
            config.Save(ConfigurationSaveMode.Modified);

            config = SettingsHelper.ZXLPRSettings.CurrentConfiguration;
            config.ConnectionStrings.ConnectionStrings["AWSMYSQL"].ConnectionString = connectionString;
            config.Save(ConfigurationSaveMode.Modified);

            config = ConfigurationManager.OpenExeConfiguration("ZXSyncWeighingData.exe");
            config.ConnectionStrings.ConnectionStrings["AWSMYSQL"].ConnectionString = connectionString;
            config.Save(ConfigurationSaveMode.Modified);

            config = ConfigurationManager.OpenExeConfiguration("ZXWeighingRecord.exe");
            config.ConnectionStrings.ConnectionStrings["AWSMYSQL"].ConnectionString = connectionString;
            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("connectionStrings");
        }

        #region 开机自启动





        ///// <summary>
        /////  向目标路径创建指定文件的快捷方式
        ///// </summary>
        ///// <param name="directory">目标目录</param>
        ///// <param name="shortcutName">快捷方式名字</param>
        ///// <param name="targetPath">文件完全路径</param>
        ///// <param name="description">描述</param>
        ///// <param name="iconLocation">图标地址</param>
        ///// <returns>成功或失败</returns>
        //private bool CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
        //{
        //    try
        //    {
        //        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);                         //目录不存在则创建
        //                                                                                                        //添加引用 Com 中搜索 Windows Script Host Object Model
        //        string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));          //合成路径
        //        WshShell shell = new IWshRuntimeLibrary.WshShell();
        //        IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);    //创建快捷方式对象
        //        shortcut.TargetPath = targetPath;                                                               //指定目标路径
        //        shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);                                  //设置起始位置
        //        shortcut.WindowStyle = 1;                                                                       //设置运行方式，默认为常规窗口
        //        shortcut.Description = description;                                                             //设置备注
        //        shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;    //设置图标路径
        //        shortcut.Save();                                                                                //保存快捷方式
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        string temp = ex.Message;
        //        temp = "";
        //    }
        //    return false;
        //}






        #endregion

        public class LoginViewModellValidator : AbstractValidator<LoginViewModel>
        {
            public LoginViewModellValidator()
            {
                RuleFor(x => x.UserName).NotNull().WithMessage("必填");
                // RuleFor(x => x.OldPwd).NotNull().WithMessage("必填");
                RuleFor(x => x.NewPwd).NotNull().WithMessage("必填");
                RuleFor(x => x.ReNewPwd).NotNull().WithMessage("必填");

                RuleFor(x => new { x.NewPwd, x.ReNewPwd }).Must(x => check(x.NewPwd, x.ReNewPwd)).WithMessage("两次密码输入不一致！");

            }

            public bool check(string newpwd, string reNewpwd)
            {
                return newpwd == reNewpwd;
            }
        }


    }
}
