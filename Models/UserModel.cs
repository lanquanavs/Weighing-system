using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.Models
{
    public class UserModel
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string LoginId { get; set; }
        public string LoginPwd { get; set; }
        public bool Valid { get; set; }
    }
}
