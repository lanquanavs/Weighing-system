using AutoUpdaterDotNET;
using Common.Utility;
using Common.Utility.AJ;
using Common.Utility.AJ.MobileConfiguration;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AWSV2.ViewModels
{
    public class AboutAWSViewModel : Screen
    {
        public bool Loading { get; set; }

        private bool _canCheckUpdate;
        public bool CanCheckUpdate
        {
            get
            {
                _canCheckUpdate = !Loading;
                return _canCheckUpdate;
            }
            set
            {
                SetAndNotify(ref _canCheckUpdate, value);
            }
        }

        public CloudAPI.ActiveCode ActiveCode { get; set; }

        public string VersionName
        {
            get
            {
                return Application.ResourceAssembly.GetName().Version.ToString();
            }
        }

        public bool ActiveSuccess { get; set; }

        private ObservableCollection<DropdownOption> _otherParams;
        public ObservableCollection<DropdownOption> OtherParams
        {
            get
            {
                return _otherParams;
            }
            set
            {
                SetAndNotify(ref _otherParams, value);
            }
        }

        public bool HardDiskNoCopySuccess { get; set; } = false;

        private AppSettingsSection _mainSetting;

        public AboutAWSViewModel()
        {
            _mainSetting = SettingsHelper.AWSV2Settings;
            ActiveCode = CloudAPI.GetLocalActiveCode(_mainSetting, out var activeSuccess);
            ActiveSuccess = activeSuccess;

            var parameters = AJUtil.TryGetJSONObject<DropdownOption[]>(ActiveCode.OtherParameters) 
                ?? new DropdownOption[0];

            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i].label = $"{parameters[i].label}：";
            }

            OtherParams = new ObservableCollection<DropdownOption>(parameters);
        }

        public void CopyHardDiskNo()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ActiveCode.HardwareCode))
                {
                    Clipboard.SetText(ActiveCode.HardwareCode);
                    HardDiskNoCopySuccess = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "复制失败");
            }
        }

        public void ToUrl(System.Uri uri)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(uri.AbsoluteUri)
            {
                UseShellExecute = true
            });
        }

        public void CheckUpdate()
        {
            Loading = true;

            AutoUpdater.Start(CloudAPI.UpdateXMLUrl);

            Loading = false;
        }

        public void CloseCmd()
        {
            RequestClose();
        }
    }
}
