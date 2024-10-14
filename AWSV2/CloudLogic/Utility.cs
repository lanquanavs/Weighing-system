using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2
{
    public static class Utility
    {
        /// <summary>
        /// 获取2给日期之间相差的秒数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int TimeSPanBySeconds(DateTime dateTime)
        {
            return DateTime.Now.Subtract(dateTime).Duration().Seconds;
        }

        /// <summary>
        /// 获取2给日期之间相差的分数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int TimeSPanByMinutes(DateTime dateTime)
        {
            return DateTime.Now.Subtract(dateTime).Duration().Minutes;
        }

        /// <summary>
        /// 获取2给日期之间相差的时数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int TimeSPanByHours(DateTime dateTime)
        {
            return DateTime.Now.Subtract(dateTime).Duration().Hours;
        }

    }
}
