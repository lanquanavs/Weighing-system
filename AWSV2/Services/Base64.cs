using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.Services
{
    public static class Base64
    {
        /// <summary>
        /// base64加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncryptBase64(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            return Convert.ToBase64String(inputBytes);
        }

        /// <summary>
        /// base64解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DecryptBase64(string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);

            return Encoding.UTF8.GetString(inputBytes);
        }
    }
}
