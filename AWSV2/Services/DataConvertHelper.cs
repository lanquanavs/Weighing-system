using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.Services
{
    public static class DataConvertHelper
    {
        public static void ConvertWeightToLedHex(string strWeight, ref byte[] hexWeight)
        {
            //byte[] hexWeight = new byte[12];

            //起始标记
            hexWeight[0] = 0x02;
            hexWeight[11] = 0x03;

            try
            {
                //正负号
                decimal dWeight = Convert.ToDecimal(strWeight);
                if (dWeight >= 0)
                    hexWeight[1] = 0x2B;
                else
                    hexWeight[1] = 0x2D;

                //小数点
                hexWeight[8] = 0x30;
                string[] splitWeight = strWeight.Split('.');
                if(splitWeight.Length > 1)
                {
                    hexWeight[8] += Convert.ToByte(splitWeight[1].Length);
                }

                //剩余的纯数字
                string strWeightNum = strWeight.Replace(".", string.Empty);
                int iWeight = Convert.ToInt32(strWeightNum);
                for (int i = 7; i > 1; i--)
                {
                    hexWeight[i] = Convert.ToByte(iWeight % 10);
                    iWeight /= 10;
                }

                //校验位
                byte[] bccCheck = new byte[8];
                Buffer.BlockCopy(hexWeight, 1, bccCheck, 0, 8);
                byte bccRst = Get_CheckXor(bccCheck);
                hexWeight[9] = Convert.ToByte(bccRst / 16 + 0x30);
                hexWeight[10] = Convert.ToByte(bccRst % 16 + 0x30);
            }
            catch {}

        }

        private static byte Get_CheckXor(byte[] data)
        {
            byte CheckCode = 0;
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                CheckCode ^= data[i];
            }
            return CheckCode;
        }
    }
}
