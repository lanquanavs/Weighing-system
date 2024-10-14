using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.ViewModels
{
    public class AboutAWSViewModel : Screen
    {
        public string TextInfo { get; set; }

        public AboutAWSViewModel()
        {
            TextInfo =  "汽车衡称重系统 - 企业版" + "\r\n";
            TextInfo += "版本： 2.2.0";
        }
    }
}
