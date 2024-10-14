using Common.EF.Controllers;
using Common.Utility;
using Common.Utility.AJ;
using log4net;
using Masuit.Tools.Reflection;
using Masuit.Tools.Systems;
using Quartz.Util;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace AWSV2.ViewModels
{
    public class DatabaseChangeViewModel : Screen
    {

        private static ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string ActiveInfo { get; set; }

        public string VersionName
        {
            get
            {
                return Application.ResourceAssembly.GetName().Version.ToString();
            }
        }

        private bool _canSave;
        public bool CanSave
        {
            get
            {
                if (DbCfg == null)
                {
                    return false;
                }
                switch (DbCfg.Type)
                {
                    case AJDatabaseService.DbType.sqlite:
                        _canSave = !Loading  && !string.IsNullOrWhiteSpace(DbCfg.DbFile);
                        break;
                    case AJDatabaseService.DbType.mysql:
                    case AJDatabaseService.DbType.mssql:
                        _canSave = !Loading && !string.IsNullOrWhiteSpace(DbCfg.Server)
                            && !string.IsNullOrWhiteSpace(DbCfg.UserId)
                            && !string.IsNullOrWhiteSpace(DbCfg.Password);
                        break;
                    default:
                        break;
                }
                return _canSave;
            }
            set
            {
                _canSave = value;
            }
        }

        private AJDatabaseService.CommonDbConnectionConfig _dbCfg;
        public AJDatabaseService.CommonDbConnectionConfig DbCfg
        {
            get => _dbCfg;
            set => SetAndNotify(ref _dbCfg, value);
        }

        private Dictionary<string, AJDatabaseService.DbType> _types;
        public Dictionary<string, AJDatabaseService.DbType> TypeOptions
        {
            get => _types;
            set => SetAndNotify(ref _types, value);
        }

        public string ErrorInfo { get; set; }

        public bool Loading { get; set; }

        private IWindowManager _windowManager;
        private IEventAggregator _eventAggregator;
        private AppSettingsSection _mainSetting;


        public DatabaseChangeViewModel(IWindowManager windowManager,
            IEventAggregator eventAggregator)
        {
            _mainSetting = SettingsHelper.AWSV2Settings;
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;

            TypeOptions = AJUtil.EnumToDictionary<AJDatabaseService.DbType>((val) =>
            {
                return val.GetDescription();
            });

            ActiveInfo = CloudAPI.APPLICENSEINFO.IsActive ? "已激活" : "未激活";

            Prepare();
        }

        private void Prepare()
        {
            DbCfg = AJDatabaseService.DbConfig();
            if(DbCfg == null)
            {
                DbCfg = new AJDatabaseService.CommonDbConnectionConfig();
            }
        }

        public void LoadDBPath()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = ".db",
                Filter = "数据库文件（.db）|*.db"
            };
            if (fileDialog.ShowDialog().GetValueOrDefault())
            {
                DbCfg.DbFile = fileDialog.FileName;
            }

        }

        public async void Save()
        {
            Loading = true;
            CanSave = false;

            using var ctrl = new CommonController();

            //try
            //{
            //    var result = await ctrl.TestConnect(DbCfg);

            //    if (!result)
            //    {
            //        ErrorInfo = $"数据库链接失败,请检查配置";
            //        Loading = false;
            //        CanSave = true;
            //        return;
            //    }
            //}
            //catch (Exception e)
            //{
            //    ErrorInfo = $"数据库链接失败:{e.Message}";
            //    Loading = false;
            //    CanSave = true;
            //    return;
            //}

            try
            {
                await ctrl.SaveDbConfig(DbCfg);
            }
            catch (Exception e)
            {
                ErrorInfo = $"数据库设置异常:{e.Message}";
                Loading = false;
                CanSave = true;
                return;
            }

            // 重建数据库builder
            AJDatabaseService.Start();

            await Task.Delay(2000);

            Loading = false;
            CanSave = true;

            Exit();
        }

        public void Exit()
        {
            RequestClose();
            _windowManager.ShowWindow(new LoginViewModel(_windowManager, _eventAggregator));
        }

        public async void TestConnect()
        {
            Loading = true;
            CanSave = false;
            ErrorInfo = string.Empty;

            using var ctrl = new CommonController();

            try
            {
                var result = await ctrl.TestConnect(DbCfg);
                if (!result)
                {
                    ErrorInfo = $"数据库连接失败,请检查配置";
                    Loading = false;
                    CanSave = true;
                    return;
                }
            }
            catch (Exception e)
            {
                ErrorInfo = $"数据库连接失败:{e.Message}";
                Loading = false;
                CanSave = true;
                return;
            }
            

            MessageBox.Show("数据库连接成功", "配置有效", MessageBoxButton.OK);

            Loading = false;
            CanSave = true;
        }

    }

    public class DbTypeToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumStrArray = (parameter?.ToString() ?? "sqlite").Split('|');

            if(value is AJDatabaseService.DbType type)
            {
                return enumStrArray.Contains(type.ToString()) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
