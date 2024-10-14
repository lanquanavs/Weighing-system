using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.Models
{
    public class SubData
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int user_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string machine_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string registration_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string machine_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int create_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int update_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string create_time_text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string update_time_text { get; set; }
    }

    public class WebResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 返回成功
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SubData data { get; set; }
    }

}
