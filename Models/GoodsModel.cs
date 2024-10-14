namespace AWSV2.Models
{
    public class GoodsModel
    {
        public int? Id { get; set; }
        public string Num { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Spec { get; set; }
        public bool Valid { get; set; }
        /// <summary>
        /// 新增默认价格 --阿吉 2023年11月12日11点33分
        /// </summary>
        public decimal Price { get; set; }
    }
}
