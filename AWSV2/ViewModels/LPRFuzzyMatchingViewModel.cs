using Common.Utility;
using Stylet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.ViewModels
{
    public class LPRFuzzyMatchingViewModel : Screen
    {
        Configuration config = SettingsHelper.AWSV2Settings.CurrentConfiguration;

        public string PlateNoStr { get; set; } = "";
        public bool EnableFuzzyMatching { get; set; }
        public string StatusBar { get; set; }

        public LPRFuzzyMatchingViewModel()
        {
            if (config.AppSettings.Settings["FuzzyMatching"].Value == "1")
                EnableFuzzyMatching = true;
            else
                EnableFuzzyMatching = false;

            string[] plateArray = config.AppSettings.Settings["FuzzyPlateNo"].Value.Split(',');
            PlateNoStr = string.Join("\r\n", plateArray);
        }

        public void SavePlate()
        {
            if (EnableFuzzyMatching)
                config.AppSettings.Settings["FuzzyMatching"].Value = "1";
            else
                config.AppSettings.Settings["FuzzyMatching"].Value = "0";

            config.AppSettings.Settings["FuzzyPlateNo"].Value = PlateNoStr.Replace("\n", "").Replace("\r", ",");
            config.Save();

            StatusBar = "保存成功";
        }
    }
}
