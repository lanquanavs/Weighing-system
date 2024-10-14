using System;

namespace AWSV2.Models
{
    public class OverloadLog
    {
        public int? AutoNo { get; set; }
        public string PlateNo { get; set; }
        public string Constraints { get; set; }
        public string AverageWeight{ get; set; } 
        public string OverloadWeight { get; set; }
        public string StandardWeight { get; set; } 
        public string AxleCount { get; set; } 
        public string Times { get; set; } 
        public DateTime? CreateDate { get; set; } 
    }
}
