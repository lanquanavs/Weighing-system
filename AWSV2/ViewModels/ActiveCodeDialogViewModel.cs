using Common.Utility;
using MaterialDesignThemes.Wpf;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Common.Utility.AJ;

namespace AWSV2.ViewModels
{
    public class ActiveCodeDialogViewModel : Screen
    {
        #region 激活弹窗绑定参数 --阿吉 2024年3月5日16点51分

        public string HardDiskNo { get; set; }


        public bool HardDiskNoCopySuccess { get; set; } = false;

        /// <summary>
        /// 二维码对象
        /// </summary>
        public System.Windows.Media.ImageSource QRCode { get; set; }

        public string ActiveCode { get; set; }

        private bool? _activeCodeCorrect;
        public bool? ActiveCodeCorrect
        {
            get
            {
                return _activeCodeCorrect;
            }
            set
            {
                _activeCodeCorrect = value;
            }
        }

        public void CancelActive()
        {
            RequestClose();
        }

        public ActiveCodeDialogViewModel()
        {
            HardDiskNo = CloudAPI.MACHINEMD5;
            QRCode = Common.Utility.ImageHelper.CreateCode(HardDiskNo);
        }

        public bool ActiveLoading { get; set; }

        private bool _canConfirmActive;
        public bool CanConfirmActive
        {
            get
            {
                _canConfirmActive = !ActiveLoading && !string.IsNullOrWhiteSpace(ActiveCode);
                return _canConfirmActive;
            }
            set
            {
                _canConfirmActive = value;
            }
        }

        public CloudAPI.ActiveCode ActiveCodeData { get; set; }

        public void CopyHardDiskNo()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(HardDiskNo))
                {
                    Clipboard.SetText(HardDiskNo);
                    HardDiskNoCopySuccess = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "复制失败");
            }

        }

        public async void ConfirmActive()
        {
            ActiveLoading = true;

            var result = await CloudAPI.TryActiveAsync(ActiveCode, false);
            ActiveCodeCorrect = result.Success;

            if (!result.Success)
            {
                ActiveCode = string.Empty;
                ActiveLoading = false;
                MessageBox.Show(result.Message, "激活失败");
                return;
            }

            ActiveCodeData = result.Data as CloudAPI.ActiveCode;
            ActiveLoading = false;
            RequestClose(true);
        }

        #endregion
    }
}
