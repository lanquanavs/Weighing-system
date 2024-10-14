using Aspose.Cells.Charts;
using AWSV2.Services;
using Common.Model.ChargeInfo;
using Common.Utility.AJ.Extension;
using Newtonsoft.Json;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Media.Media3D;

namespace AWSV2.Models
{
    public class WeighingRecordModel
    {
        public int AutoNo { get; set; }//编号
        public string Bh { get; set; }//编号
        public string Kh { get; set; }//客户
        public string Kh2 { get; set; }//客户
        public string Kh3 { get; set; }//客户
        public string Ch { get; set; }//车号
        public string Wz { get; set; }//物资
        public string Mz { get; set; }//毛重
        public string Pz { get; set; }//皮重
        public string Jz { get; set; }//净重
        public string Mzsby { get; set; }//毛重司磅员
        public string Pzsby { get; set; }//皮重司磅员
        public string Mzrq { get; set; }//毛重日期
        public string Pzrq { get; set; }//皮重日期
        public string Jzrq { get; set; }//净重日期，用于称重记录时间段统计
        public string Kz { get; set; }//扣重
        public string Kl { get; set; }//扣率
        public string Sz { get; set; }//实重
        public string Bz { get; set; }//备注
        public string ICCard { get; set; }//IC卡卡号
        public string By1 { get; set; }//备用字段
        public string By2 { get; set; }//备用字段
        public string By3 { get; set; }//备用字段
        public string By4 { get; set; }//备用字段
        public string By5 { get; set; }//备用字段
        public string By6 { get; set; }//备用字段
        public string By7 { get; set; }//备用字段
        public string By8 { get; set; }//备用字段
        public string By9 { get; set; }//备用字段
        public string By10 { get; set; }//备用字段
        public string By11 { get; set; }//备用字段
        public string By12 { get; set; }//备用字段
        public string By13 { get; set; }//备用字段
        public string By14 { get; set; }//备用字段
        public string By15 { get; set; }//备用字段
        public string By16 { get; set; }//备用字段
        public string By17 { get; set; }//备用字段
        public string By18 { get; set; }//备用字段
        public string By19 { get; set; }//备用字段
        public string By20 { get; set; }//备用字段

        public string WeighName { get; set; }//地磅名称
        public string Weigh2Name { get; set; }//地磅2名称
        public string Dyrq { get; set; }//打印日期，数据库中没有，打印时修改
        public int WeighingTimes { get; set; }//称重次数
        public bool IsFinish { get; set; }//是否完成称重
        public bool Valid { get; set; }//有效位
        public string WeighingFormTemplate { get; set; }//称重模版

        public string Je { get; set; }//费用

        public DateTime? EntryTime { get; set; }//进入时间

        public string Gblx { get; set; }//过磅类型

        /// <summary>
        /// 0，未支付，1、电子支付，2、线下支付，3、储值支付，4、免费放行 默认是未支付
        /// </summary>
        public int IsPay { get; set; } = 0;//是否完成支付 
        /// <summary>
        /// 支付流水号，由平提供，如果是线上支付的话
        /// </summary>
        public string Zflsh { get; set; }//
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? Zfsj { get; set; }//
        /// <summary>
        /// 支付金额
        /// </summary>
        public int Zfje { get; set; }//
        /// <summary>
        /// 支付单位：P云，支付宝，微信等
        /// </summary>
        public string Zfdw { get; set; }//
        /// <summary>
        /// 支付到账说明
        /// </summary>
        public string Zfdzsm { get; set; }//

        public string AxleNum { get; set; }//车轴数
        public bool IsLimit { get; set; }//是否超载
        public string LimitType { get; set; }//超载类别：毛重超载，皮重超载，车轴超载
        public string LimitedValue { get; set; }//超载重量
        public bool IsUpload { get; set; }//是否上传
        public bool IsRisk { get; set; }//是否危险品
        public string RiskFactor { get; set; }//危险品说明/种类
        public string GoodsSpec { get; set; }//物资规格
        public string Fhdw { get; set; }//发货单位

        public decimal Amount { get; set; }///充值剩余金额
        public decimal Credit { get; set; }//剩余授信额度
        public decimal Weigh { get; set; }//充值剩余重量
        public string Driver { get; set; }
        public string DriverPhone { get; set; }
        public string SerialNumber { get; set; }

        public bool? IsCover { get; set; }//是否被遮盖

        /// <summary>
        /// 新增物资单价 --阿吉 2023年11月12日13点27分
        /// </summary>
        public decimal GoodsPrice { get; set; }

        /// <summary>
        /// 专门用于主程序磅单表单绑定单价用 --阿吉2023年11月15日14点58分
        /// </summary>
        public string GoodsPriceStr
        {
            get { return GoodsPrice.ToString(); }
            set
            {
                GoodsPrice = value.TryGetDecimal();
            }
        }


        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}"
                , Bh, Kh, Ch, Wz, Mz, Pz, Jz, Mzsby, Pzsby, Mzrq, Pzrq, Jzrq, Kz, Kl, Sz, Bz, WeighingTimes, Je);
        }

        [JsonIgnore]
        public ChargeInfoConfig ChargeInfoConfig { get; set; }


        /// <summary>
        /// 获取收费价格  -- 现逻辑 --阿吉 2023年11月14日15点26分
        /// </summary>
        /// <param name="times">是那一次称重调用的？0：一次称重模式，1、二次称重模式下的第一次称重，2、二次称中模式下的第二次称重</param>
        /// <returns></returns>
        public string GetCost(int times, Action<string> showLogsHandle)
        {
            if (ChargeInfoConfig == null || times != 2)
            {
                Je = "0";
                return Je;
            }

            var fee = GoodsPrice * (Jz.TryGetDecimal());

            if (fee == 0)
            {
                Je = "0";
                return Je;
            }

            Je = fee.ToString("0.00");

            TTSHelper.TTS(this, $"请缴费{Je}元！");
            showLogsHandle?.Invoke($"本次费用：{Je} 元");

            return Je;
        }

        ///// <summary>
        ///// 获取收费价格  -- 原逻辑 --阿吉 2023年11月14日15点26分
        ///// </summary>
        ///// <param name="times">是那一次称重调用的？0：一次称重模式，1、二次称重模式下的第一次称重，2、二次称中模式下的第二次称重</param>
        ///// <returns></returns>
        //public string GetCost(int times, Action<string> showLogsHandle)
        //{
        //    if (ChargeInfoConfig == null)
        //    {
        //        return "0";
        //    }


        //    //----------这里为收费逻辑段  by：汪虎 2023-02-08-------------

        //    if (ChargeInfoConfig.ChargeEnable)//启用收费
        //    {
        //        string fee = "0";
        //        decimal weight = 0;
        //        //如果启用了多次收费（第一次收费+第二次收费），并且不是一次称重模式
        //        if (ChargeInfoConfig.MultipleChargeEnable && times != 0)
        //        {
        //            if (times == 1)//是第一次称重的
        //            {
        //                weight = Convert.ToDecimal(Mz);
        //            }
        //            else //第二次称重的
        //            {
        //                weight = Convert.ToDecimal(Jz);
        //            }
        //        }
        //        else
        //        {//一次称重模式的还用原来的收费逻辑

        //            if (ChargeInfoConfig.ChargeTypes == "按毛重+皮重收费")//收费模式：0 按毛重+皮重收费，1按净重收费，2 按毛重收费
        //            {
        //                weight = Convert.ToDecimal(Mz) + Convert.ToDecimal(Pz);
        //            }
        //            else if (ChargeInfoConfig.ChargeTypes == "按毛重收费")//收费模式：0 按毛重+皮重收费，1按净重收费，2 按毛重收费
        //            {
        //                weight = Convert.ToDecimal(Mz);
        //            }
        //            else
        //            {
        //                weight = Convert.ToDecimal(Jz);
        //            }
        //        }

        //        ////判断是否按物资收费，如果是 那么获取物资收费标准名称用于查询，如果不是，则用默认收费名称，默认名称为：普通收费标准
        //        //var feesTitle = ChargeByMaterial ? !string.IsNullOrWhiteSpace(Wz) ? Wz.Trim() : "Normal_888888" : "Normal_888888";
        //        var feesTitle = "Normal_888888";

        //        var ChargeInfoItem = ChargeInfoConfig.ChargeInfo.FirstOrDefault(p => p.Name == feesTitle);
        //        if (ChargeInfoItem == null)
        //        {
        //            ChargeInfoItem = ChargeInfoConfig.ChargeInfo.First(p => p.Name == "普通收费标准");//如果没有查到对应的物资信息，就采用默认收费标准
        //        }

        //        if (ChargeInfoItem.Name == "普通收费标准")
        //        {
        //            //if (ChargeInfoConfig.ChargeType == "1")//单位收费
        //            //{
        //            //    decimal price = 0;
        //            //    var priceStr = ToFeesList(ChargeInfoItem.Fees).Where(p => p.Type == 1 && (p.ChargeRule.UpperLimit >= weight && p.ChargeRule.LowerLimit <= weight)).Max(o => o.Fee);
        //            //    if (decimal.TryParse(priceStr, out price))
        //            //    {
        //            //        fee = (weight * price).ToString("0.00");
        //            //    }
        //            //}
        //            //else if (ChargeInfoConfig.ChargeType == "2")//范围收费
        //            //{
        //            //    fee = ToFeesList(ChargeInfoItem.Fees).Where(p => p.Type == 0 && (p.ChargeRule.UpperLimit >= weight && p.ChargeRule.LowerLimit <= weight)).Max(o => o.Fee);
        //            //}
        //            //else if (ChargeInfoConfig.ChargeType == "3")//单价收费 --阿吉 2023年11月14日09点15分
        //            //{
        //            //    fee = (GoodsPrice * weight).ToString("0.00");
        //            //}
                    
        //        }
        //        else
        //        {
        //            fee = (weight * ChargeInfoItem.Price).ToString("0.00");
        //        }

        //        if (string.IsNullOrWhiteSpace(fee) || decimal.Parse(fee) == 0)
        //        {
        //            fee = "0";
        //        }

        //        Je = string.IsNullOrWhiteSpace(fee) || fee == "0" ? ChargeInfoConfig.LowestFees : fee;

        //        if (!String.IsNullOrWhiteSpace(Je))//表示有收费金额
        //            TTSHelper.TTS(this, $"请缴费{Je}元！");
        //    }
        //    else
        //    {
        //        Je = "0";
        //    }

        //    if (!String.IsNullOrWhiteSpace(Je) && Je != "0") showLogsHandle?.Invoke($"本次费用：{Je} 元");
        //    return Je;
        //}

        public IList<ChargeRuleModel> ToFeesList(FeesItemModel model)
        {
            ChargeInfoConfig.ChargeRuleList.Clear();
            //------费用范围的相关参数设置
            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Fee = model.Fees1.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage1Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage1Max)
                }
            });

            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Fee = model.Fees2.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage2Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage2Max)
                }
            });

            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Fee = model.Fees3.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage3Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage3Max)
                }
            });

            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Fee = model.Fees4.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage4Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage4Max)
                }
            });

            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Fee = model.Fees5.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage5Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage5Max)
                }
            });

            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Fee = model.Fees6.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage6Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage6Max)
                }
            });

            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Fee = model.Fees7.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage7Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage7Max)
                }
            });

            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Fee = model.Fees8.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage8Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage8Max)
                }
            });

            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Fee = model.Fees9.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage9Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage9Max)
                }
            });
            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Type = 1,
                Fee = model.Fees20.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage20Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage20Max)
                }
            });
            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Type = 1,
                Fee = model.Fees21.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage21Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage21Max)
                }
            });
            ChargeInfoConfig.ChargeRuleList.Add(new ChargeRuleModel
            {
                Type = 1,
                Fee = model.Fees22.ToString(),
                ChargeRule = new Rule
                {
                    LowerLimit = Convert.ToDecimal(model.Tonnage22Min),
                    UpperLimit = Convert.ToDecimal(model.Tonnage22Max)
                }
            });
            return ChargeInfoConfig.ChargeRuleList;
        }
    }
}
