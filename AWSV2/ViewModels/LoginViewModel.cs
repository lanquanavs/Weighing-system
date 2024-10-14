using AWSV2.Services;
using AWSV2.Views;
using Common.Utility;
using Common.Utility.AJ;
using FluentValidation;
using Stylet;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using System.Windows;
using Masuit.Tools.Security;
using File = System.IO.File;
using static Common.Utility.AJ.CloudAPI;
using Common.Platform;
using Common.LPR;
using Common.Utility.AJ.Extension;
using Common.EF.Controllers;
using Common.ViewModels;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Linq;
using Common.Models;
using Common.EF;
using System.IO;
using Common.Platform.Citys.LuoYang;

namespace AWSV2.ViewModels
{
    public class LoginViewModel : Screen
    {

        private bool showShellView = true;

        //加载其他窗口

        public IWindowManager WindowManager { get; set; }

        //界面上绑定的属性


        private bool _logining;
        public bool Logining
        {
            get
            {
                return _logining;
            }
            set
            {
                _logining = value;
            }
        }

        private bool _canLogin;
        public bool CanLogin
        {
            get
            {
                _canLogin = !Logining && !LicenseRunning && !PrepareRunning
                    && !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Password);
                return _canLogin;
            }
            set { _canLogin = value; }
        }

        public bool ShowProgressBar { get; set; }
        private string _TextInfo;
        public string TextInfo { get { return _TextInfo; } set { _TextInfo = $"{value}"; } }

        public string UserId { get; set; }
        public string Password { get; set; }

        public string ActiveInfo { get; set; }

        public string VersionName
        {
            get
            {
                return Application.ResourceAssembly.GetName().Version.ToString();
            }
        }

        public bool InfoBarVisible { get; set; }

        

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

                    AJUtil.SetMeAutoStart(value);
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

        private CloudAPI.ActiveCode _activeCode;
        public CloudAPI.ActiveCode ActiveCode
        {
            get
            {
                return _activeCode;
            }
            set
            {
                SetAndNotify(ref _activeCode, value);
            }
        }

        public bool LicenseRunning { get; set; } = true;
        public bool PrepareRunning { get; set; } = true;

        public IEventAggregator EventAggregator { get; set; }

        public LoginViewModel(IWindowManager windowManager, IEventAggregator eventAggregator) : base(new FluentModelValidator<LoginViewModel>(new LoginViewModellValidator()))
        {
            WindowManager = windowManager;
            EventAggregator = eventAggregator;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.5)
            };
            timer.Tick += (_, __) =>
            {
                timer.Stop();
                var appName = Application.ResourceAssembly.GetName().Name;
                var backupConfigFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "avs", $"{appName}.dll.config");

                var dataBaseFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "avs", $"{DbService.DatabaseName}.db");

                var configFileExists = File.Exists(backupConfigFile);
                var dbFileExists = File.Exists(dataBaseFileName);

                if (configFileExists || dbFileExists)
                {
                    var restoreConfigDialog = new AJCommonLoadingDialogViewModel("正在还原文件...", () =>
                    {
                        ProcessResult result = null;

                        if (configFileExists)
                        {
                            result = RestoreConfigFile(backupConfigFile, appName);
                        }

                        if (dbFileExists)
                        {
                            result = RestoreDbFile(dataBaseFileName);
                        }


                        return Task.FromResult(result);
                    });

                    windowManager.ShowDialog(restoreConfigDialog);

                    if (!restoreConfigDialog.Result.Success)
                    {
                        AJLog4NetLogger.Instance().Error($"还原配置文件失败:{restoreConfigDialog.Result.Message}", restoreConfigDialog.Result.Data as Exception);
                        MessageBox.Show($"还原配置文件失败:{restoreConfigDialog.Result.Message};将使用默认配置文件", "提示", MessageBoxButton.OK);
                    }
                }

                _mainSetting = SettingsHelper.AWSV2Settings;

                ActiveCode = new CloudAPI.ActiveCode();

                ActiveCode.RunImageWorker();

                ActiveCode.RunDeviceWorker();

                VerifyLicense();
                Prepare();
            };
            timer.Start();
        }

        public void OpenChangePassword()
        {
            WindowManager.ShowWindow(new ChangePasswordViewModel(WindowManager, EventAggregator));
            RequestClose();

        }

        public void OpenDataSet()
        {
            WindowManager.ShowWindow(new DatabaseChangeViewModel(WindowManager, EventAggregator));
            RequestClose();
        }

        private void Prepare()
        {

            var worker = new BackgroundWorker();


            worker.DoWork += (s, e) =>
            {
                var result = new ProcessResult();

                //log.Info("测试登录日志。。。。。。。。。。");
                var error = 0;
                var connected = false;
                while (true)
                {
                    if (error > 5)
                    {
                        break;
                    }
                    var connectedTask = AJDatabaseService.CheckConnectionAsync();
                    connectedTask.Wait();
                    connected = connectedTask.Result;
                    if (connected)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                    error++;
                }
                if (connected)
                {
                    result.SetSuccess();
                }
                else
                {
                    result.SetError("数据库访问失败，请联系管理员！");
                }
                e.Result = result;
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                PrepareRunning = false;
                if (e.Error != null)
                {
                    AJLog4NetLogger.Instance().Error($"登录程序异常:{e.Error.Message}");
                    TextInfo = "登录发生错误,请查看日志";
                    return;
                }
                var result = e.Result as ProcessResult;

                if (!result.Success)
                {
                    TextInfo = result.Message;
                    return;
                }


                AutoStart = Convert.ToBoolean(_mainSetting.Settings["AutoStart"]?.Value ?? "False");

                if (Convert.ToBoolean(_mainSetting.Settings["StorePwd"]?.Value ?? "False"))
                {
                    StorePwd = true;

                    UserId = Properties.Settings.Default.LastLoginId;
                    Password = Properties.Settings.Default.LastUserPwd;
                }
                AutoLogin = _mainSetting.Settings["AutoLogin"].TryGetBoolean();
                if (AutoLogin)
                {
                    Password = Properties.Settings.Default.LastUserPwd;
                    Login();
                }
            };

            worker.RunWorkerAsync();

        }

        public async void Login()
        {
            //PlatformManager.Instance.Init("dongfanghong");

            //var api = PlatformManager.Instance.Current as LuoYangDongFangHongPlatform;

            //LPRService.LPRSavePath = _mainSetting.Settings["LPRSavePath"].TryGetString();

            //var result = await api.UploadWeighRecordAsync(new PlatformBase.UploadWeighRecordParams
            //{
            //    WeighRecord = new Common.Models.WeighingRecord
            //    {
            //        Bh = "A202407211525abc",
            //        Ch = "京A45781",
            //        Mz = 100,
            //        Pz = 10,
            //        Jz = 90,
            //        Sz = 90,
            //        Fhdw = "",
            //        WeighName = "203286259755",
            //        Kz = 0,
            //        Je = 0,
            //        Wz = "测试物资",
            //        By11 = "测试物资规格",
            //        By10 = "admin",
            //        Driver = "阿吉",
            //        By1 = "As1234560",
            //        GoodsPrice = 0,
            //        Mzrq = DateTime.Now,
            //        Pzrq = DateTime.Now,
            //        IsFinish = true,
            //        Mzsby = "admin",
            //        Pzsby = "admin",
            //        Bz = "abc",
            //        GoodsSpec = "abc"
            //    }
            //});

            //MessageBox.Show(AJUtil.AJSerializeObject(result));

            //return;

            TextInfo = string.Empty;
            Logining = true;
            CanLogin = false;
            using var ctrl = new UserController();
            var loginRet = await ctrl.Login(UserId, Password);

            Logining = false;
            CanLogin = true;

            if (!loginRet)
            {
                TextInfo = "用户名或密码错误";
                return;
            }

            if (!File.Exists(Globalspace._weightFormTemplatePath))
            {
                TextInfo = "表单模板文件不存在，请联系供应商！";
                return;
            }

            if (StorePwd)
            {
                Properties.Settings.Default.LastLoginId = UserId;
                Properties.Settings.Default.LastUserPwd = Password;
                Properties.Settings.Default.Save();
            }
            var dbUser = await ctrl.Detail(UserId, string.Empty);
            Globalspace._currentUser = AJAutoMapperService.Instance().Mapper.Map<Common.EF.Tables.User, Common.Models.User>(dbUser);

            Common.Globalspace._currentUser = Globalspace._currentUser;
            Common.Globalspace._currentUserId = UserId;

            if (showShellView)//当窗口是正式登陆窗口的时候该要的逻辑都要运行
            {
                Properties.Settings.Default.LastLoginId = Globalspace._currentUser.LoginId;
                Properties.Settings.Default.LastUserPwd = Globalspace._currentUser.LoginPwd;
                Properties.Settings.Default.Save();
                AWSV2.Globalspace.windowManager = WindowManager;

                if (File.Exists(Globalspace._weightFormTemplatePath))
                {

                    this.WindowManager.ShowWindow(new ShellViewModel(WindowManager, EventAggregator));
                    this.RequestClose();

                    //Task.Run(() => BackupHelper.AutoBackup());
                }

            }
            else//当窗口被数据备份功能中的数据清理借用的时候,是不需要再次打开主页面
                //,再次全局赋值，再次备份数据库那些操作的。只要返回模态框的 true返回值
            {
                this.RequestClose(Globalspace._currentUser.LoginId.ToLower() == "admin");
            }
        }

        //验证注册码
        private async void VerifyLicense()
        {
            var code = _mainSetting.Settings[CloudAPI.AppActiveConfigKey].TryGetString();
            var result = await CloudAPI.TryActiveAsync(code.Base64Decrypt(), autoMode: true);

            // 这里不再返回错误, 而是清空注册码重新在尝试自动注册
            if (result.Message == "激活码数据错误:400")
            {
                result = await CloudAPI.TryActiveAsync(string.Empty, false, true);
            }

            ActiveInfo = CloudAPI.APPLICENSEINFO.IsActive ? "已激活" : "未激活";
            if (result.Success)
            {
                ActiveCode = result.Data as CloudAPI.ActiveCode;
                ActiveCode.RunImageWorker();
                ActiveCode.RunDeviceWorker();
            }
            LicenseRunning = false;
        }

        //public void ChangePwd(object parameter)
        //{
        //    var values = (object[])parameter;

        //    var inputUser = UserList.Find(u => u == User);
        //    Password = ((PasswordBox)values[0]).Password;

        //    if (Password == null || Password == "")
        //    {
        //        ConfirmBtnText = "请输入原密码";
        //        return;
        //    }
        //    var uname = ((TextBox)values[3]).Text;
        //    if (uname == null || uname == "")
        //    {
        //        ConfirmBtnText = "请输入用户名";
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
        //                ConfirmBtnText = "请输入新密码";
        //                return;
        //            }
        //            if (!newPwd.Equals(confirmPwd))
        //            {
        //                ConfirmBtnText = "两次密码不一致，请重新输入。";
        //                return;
        //            }
        //            else
        //            {
        //                inputUser.LoginPwd = newPwd;
        //                SQLDataAccess.UpdateUser(inputUser);
        //                ConfirmBtnText = "密码修改成功！";

        //                log.Info("用户 " + inputUser.UserName + " 修改登录密码");
        //            }
        //        }
        //        else
        //        {
        //            ConfirmBtnText = "原密码错误";
        //        }
        //    }
        //    else
        //    {
        //        ConfirmBtnText = "用户不存在";
        //    }

        //    InfoBarVisible = true;
        //}

        private LoginView _loginWindow;

        public void Exit()
        {
            _loginWindow.Close();
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _loginWindow = (LoginView)sender;
        }

        public string UserName { get; set; }
        public string OldPwd { get; set; }
        public string NewPwd { get; set; }
        public string ReNewPwd { get; set; }
        public bool ShowEditPwd { get; set; }
        public string EditMessage { get; set; }


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

        private ProcessResult RestoreConfigFile(string backupConfigFile, string exeName)
        {
            var result = new ProcessResult();
            try
            {
                var localFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,$"{exeName}.dll.config");
                var currentSection = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                {
                    ExeConfigFilename  = localFile,
                }, ConfigurationUserLevel.None);

                var backupFileSection = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                {
                    ExeConfigFilename = backupConfigFile,
                }, ConfigurationUserLevel.None);

                var keys = currentSection.AppSettings.Settings.AllKeys;

                var backupSettings = backupFileSection.AppSettings.Settings;
                var backupAllKeys = backupSettings.AllKeys;

                foreach (var key in keys)
                {
                    if (backupAllKeys.Any(p => p.Equals(key, StringComparison.OrdinalIgnoreCase)))
                    {
                        currentSection.AppSettings.Settings[key].Value = backupSettings[key].Value;
                    }
                }

                currentSection.Save();
                ConfigurationManager.RefreshSection("appSettings");

                result.SetSuccess();

                try
                {
                    File.Delete(backupConfigFile);
                }
                catch (Exception)
                {
                }

            }
            catch (Exception e)
            {
                result.SetError(e.Message, e);
            }

            return result;
        }

        private ProcessResult RestoreDbFile(string dataBaseFileName)
        {
            var result = new ProcessResult();
            try
            {
                new FileInfo(dataBaseFileName).CopyTo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{DbService.DatabaseName}.db"), true);

                try
                {
                    File.Delete(dataBaseFileName);
                }
                catch (Exception)
                {
                }
                result.SetSuccess();
            }
            catch (Exception e)
            {
                result.SetError(e.Message, e);
            }
            return result;
        }
    }

    //public class UserAutocompleteSource : AutocompleteSource<Common.Models.User>
    //{
    //    private List<Common.Models.User> _source;

    //    public List<Common.Models.User> Source
    //    {
    //        get { return _source; }
    //    }

    //    public UserAutocompleteSource()
    //    {
    //        _source = Common.Data.SQLDataAccess.LoadActiveUser();
    //    }

    //    public override IEnumerable<Common.Models.User> Search(string searchTerm)
    //    {
    //        searchTerm = searchTerm ?? string.Empty;
    //        searchTerm = searchTerm.ToLower();

    //        return _source.Where(p => p.LoginId.ToLower().Contains(searchTerm));
    //    }
    //}

}
