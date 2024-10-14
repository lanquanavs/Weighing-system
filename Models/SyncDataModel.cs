using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.Models
{
    public class SyncDataModel
    {
        public int? AutoNo { get; set; }
        public string WeighingRecordBh { get; set; }
        public string SyncMode { get; set; }
        public bool SyncStatus { get; set; }
    }
}
