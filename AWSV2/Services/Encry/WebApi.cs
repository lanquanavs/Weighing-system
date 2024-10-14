//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AWSV2.Models;
//using Newtonsoft.Json;
//using ThoughtWorks.QRCode.Codec;

//namespace AWSV2.Services.Encrt
//{
//    public static class WebApi
//    {
//        private readonly static string webRoot = "https://jiux.tccxtwas.com";

//        /// <summary>
//        /// 获取注册码
//        /// </summary>
//        /// <param name="machineCode"></param>
//        /// <returns></returns>
//        public static WebResult GetRegCode(string machineCode)
//        {
//            dynamic dyObj = new { machine_code = machineCode };
//            var par = Newtonsoft.Json.JsonConvert.SerializeObject(dyObj);
//            var result = new HttpHelper().Post(par, webRoot,"api/rsa/get_registration_code");
//            return Newtonsoft.Json.JsonConvert.DeserializeObject<WebResult>(result);
//        }

//        public static bool Ping()
//        {
//            //return  HttpHelper.Ping(webRoot);
//            return HttpHelper.PingIp(webRoot.Replace("https://", String.Empty));
//        }


//        /// <summary>
//        /// 将DateTime时间格式转换为Unix时间戳格式
//        /// </summary>
//        /// <param name="dateTime">DateTime时间</param>
//        ///  <param name="format">精度：s-秒，m-毫秒</param>
//        /// <returns></returns>
//        public static long ToUnixTimeStamp(this DateTime dateTime, string accuracy)
//        {
//            long intResult = 0;
//            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
//            switch (accuracy)
//            {
//                case "s":
//                    intResult = (long)(dateTime - startTime).TotalSeconds;
//                    break;
//                case "m":
//                    intResult = (long)(dateTime - startTime).TotalMilliseconds;
//                    break;
//                default:
//                    intResult = (long)(dateTime - startTime).TotalSeconds;
//                    break;
//            }

//            return intResult;
//        }


//        /// <summary>
//        /// 生成二维码
//        /// </summary>
//        /// <param name="msg">信息</param>
//        /// <param name="version">版本 1 ~ 40</param>
//        /// <param name="pixel">像素点大小</param>
//        /// <param name="icon_path">图标路径</param>
//        /// <param name="icon_size">图标尺寸</param>
//        /// <param name="icon_border">图标边框厚度</param>
//        /// <param name="white_edge">二维码白边</param>
//        /// <returns>位图</returns>
//        public static void CreateCode(string content, string filename)
//        {
//            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
//            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
//            qrCodeEncoder.QRCodeScale = 4;
//            qrCodeEncoder.QRCodeVersion = 8;
//            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
//            System.Drawing.Image image = qrCodeEncoder.Encode(content, Encoding.UTF8);
//            //string filename = Guid.NewGuid() + ".jpg";
//            string filepath = filename + ".jpg";
//            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
//            image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
//            fs.Stop();
//            image.Dispose();
//        }

//    }
//}
