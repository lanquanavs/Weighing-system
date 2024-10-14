using AWSV2.Services.Encrt;
using Common.Utility;
using Common.Utility.AJ.Extension;
using Common.Utility.AJ.MobileConfiguration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace AWSV2.Services
{
    public static class Register
    {  //log
        private static readonly log4net.ILog log = LogHelper.GetLogger();
        public static string GetHdInfo()
        {
            try
            {
                
                return Common.Utility.HardDiskInfo.GetSerialNumber();

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        public static bool Verify(string str, ref MobileConfigurationMgr mobileConfigurationMgr)
        {
            try
            {
                //如果网络不通，则返回失败false
                //if (!WebApi.Ping()) return false;
                log.Debug("远程验证开始");
                //var result = AWSV2.Services.Encrt.WebApi.GetRegCode(str);
                var result = Common.Encrt.WebApi.GetRegCode(str);
               
                log.Debug("远程验证结果：" + JsonConvert.SerializeObject(result));
                if (result == null) return false;
                if (result.data == null) return false;
                var code = CryptoHelper.DesDeCode(result.data.registration_code);
                log.Debug("远程验证code：" + code);
                if (string.IsNullOrWhiteSpace(code)) return false;
                var regDate = CryptoHelper.UnixToDate(code);
                log.Debug("远程验证regDate：" + regDate);
                if (regDate < DateTime.Now) return false;

                //保存到XML里
                AppSettingsSection config;
                if (mobileConfigurationMgr != null)
                {
                    config = mobileConfigurationMgr.SettingList[SettingNameKey.Main];
                }
                else
                {
                    config = SettingsHelper.AWSV2Settings;
                }
                
                config.Settings["RegCode"].Value = result.data.registration_code;
                if (mobileConfigurationMgr != null)
                {
                    mobileConfigurationMgr.SaveSetting();
                }
                else
                {
                    SettingsHelper.UpdateAWSV2("RegCode", result.data.registration_code);
                }
                
                return true;
            }
            catch (Exception e)
            {
                log.Debug("远程验证ERR：" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 本地注册码验证
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool Local_Verify(string regCode)
        {
            try
            {
                log.Info("本地验证开始");
                if (string.IsNullOrWhiteSpace(regCode)) return false;
                var code = CryptoHelper.DesDeCode(regCode);
                log.Info("本地验证结果：" + code);
                var machineCode = GetHdInfo();

                var regDate = CryptoHelper.UnixToDate(code);

                //机器码不一致也验证失败
                if (!code.Contains(machineCode)) return false;

                //时间过期也失败
                if (regDate < DateTime.Now) return false;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }



        public static string Verify_old(string str)
        {
            string ret = "";
            string StrTemp = "";

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.Default.GetBytes(str);
            byte[] result = md5.ComputeHash(data);
            for (int i = 0; i < result.Length; i++)
            {
                StrTemp += result[i].ToString("x").PadLeft(2, '0');
            }
            for (int j = 0; j < StrTemp.Length; j++)
            {
                ret += StrTemp[j];
                if (j > 6 && j < StrTemp.Length - 1 && (j + 1) % 8 == 0)
                    ret += "-";
            }
            return ret;
        }

        ///// <summary>
        ///// 验证注册码有效性"6F748CD1-164B4729-58D8D5D5-640C1A82"
        ///// </summary>
        ///// <param name="par"></param>
        ///// <returns></returns>
        //public static bool VerifyCode(string machineCode, string regCode)
        //{
        //    try
        //    {
        //        List<string> dateRange = new List<string>();
        //        for (int i = 14; i >= 0; i--)
        //        {
        //            dateRange.Add(Verify($"{machineCode}{DateTime.Now.AddDays(i).ToString("yyyyMMdd")}"));
        //        }

        //        var tmp = dateRange.Find(p => p == regCode);
        //        if (tmp != null)
        //        {

        //            return true;
        //        }
        //        else
        //        {

        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return false;
        //    }
        //}


    }
}
