using AWSV2.Services;
using Common.Utility;
using Stylet;
using System.Configuration;

namespace AWSV2.ViewModels
{
    public class RegViewModel : Screen
    {
        public string HDCode { get; set; } = Register.GetHdInfo();
        public string RegCode { get; set; }
        public bool ReadOnlyRegCode { get; set; } = false;
        public bool EnableRegBtn { get; set; } = true;
        public string RegBtnContent { get; set; }

        public RegViewModel(string regCode)
        {
            RegCode = regCode;
            ReadOnlyRegCode = true;
            EnableRegBtn = false;
            RegBtnContent = "已注册";
        }

        public RegViewModel()
        {
            RegBtnContent = "注　册";
        }

        public void Reg()
        {
           // string lowerReg = RegCode?.ToLower();
            //if (Register.Verify(HDCode).Equals(lowerReg))
            if (Register.Local_Verify(RegCode))
            {
                Configuration config = SettingsHelper.AWSV2Settings.CurrentConfiguration;

                /*config.AppSettings.Settings["RegCode"].Value = lowerReg;*/
                config.AppSettings.Settings["RegCode"].Value = RegCode;

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                RequestClose(true);
            }
            else
            {
                RegCode = "注册码无效！";
            }
        }
    }
}
