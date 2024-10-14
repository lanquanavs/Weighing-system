using System;

namespace AWSV2.Models
{
    public class CarModel
    {
        public int? AutoNo { get; set; }
        /// <summary>
        /// 车号
        /// </summary>
        public string PlateNo { get; set; }
        public string VehicleWeight { get; set; }
        public int? CarOwner { get; set; } = null;
        public string CarOwnerName { get; set; }
        public string CarLabel { get; set; }
        public string Comment { get; set; }
        public bool Valid { get; set; }
        public DateTime? begindate { get; set; }
        public DateTime? enddate { get; set; }
        public string Driver { get; set; }
        public string DriverPhone { get; set; }
    }
}
