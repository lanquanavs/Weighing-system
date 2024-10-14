
using Aspose.Cells;
using AWSV2.Services;
using Common.Models;
using Common.Utility;
using Common.Utility.AJ.EventAgregators;
using Common.Utility.AJ.Extension;
using Common.Utility.AJ.MobileConfiguration;
using Stylet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace AWSV2.ViewModels
{
    public class DataFormViewModel : IHandle<MainShellViewEvent>
    {
        
        public string mz { get; set; }
        public string mz_1 { get; set; }
        public string jz { get; set; }
        public string jz_1 { get; set; }
        public string cc { get; set; }
        public string cc_1 { get; set; }
        public string cjjl { get; set; }
        public string cjjl_1 { get; set; }
        public string gblx1 { get; set; }
        public string gblx1_1 { get; set; }
        public string gblx2 { get; set; }
        public string gblx2_1 { get; set; }

        public string GblxXS {

            get
            {
                return  $"{Common.Data.SQLDataAccess.GetGblxItems()[0]}过磅";
            }
        }

        public string GblxCG
        {

            get
            {
                return $"{Common.Data.SQLDataAccess.GetGblxItems()[1]}过磅";
            }
        }

        private IEventAggregator _eventAggregator;

        public DataFormViewModel(IEventAggregator eventAggregator, ref MobileConfigurationMgr mobileConfigurationMgr)
        {
            AppSection = mobileConfigurationMgr.SettingList[SettingNameKey.Main];
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            RecordDT.Columns.Add(new DataColumn("统计时间"));
            RecordDT.Columns.Add(new DataColumn("总毛重"));
            RecordDT.Columns.Add(new DataColumn("总净重"));
            RecordDT.Columns.Add(new DataColumn("总车次"));
            RecordDT.Columns.Add(new DataColumn("称重记录"));
            RecordDT.Columns.Add(new DataColumn("销售过磅"));
            RecordDT.Columns.Add(new DataColumn("采购过磅"));

            WeighingUnit = AppSection.Settings["WeighingUnit"].Value;
            GetData(string.Empty, string.Empty, "当天");
        }

        public void Handle(MainShellViewEvent e)
        {
            GetData(string.Empty, string.Empty, "当天");
        }

        public string StatusBar { get; set; }
        public Dictionary<string, string> TemplateFieldDic { get; set; } = new Dictionary<string, string>();
        public AppSettingsSection AppSection { get; }
        public Worksheet Worksheet { get; }
        public string WeighingUnit { get; }
        public List<WeighingRecordModel> WList { get; private set; } //从数据库中查询出的原始称重记录列表
        public DataTable RecordDT { get; private set; } = new DataTable(); //经过转换的称重记录表格，用来显示到列表、导出到excel


      
        public void TotalToday_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            setTotalStyle(0);
            var before1 = DateTime.Now.Date.ToString("yyyy-MM-dd");
            var before2 = DateTime.Now.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            GetData(before1, before2, "当天");
        }
        public void TotalWeek_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            setTotalStyle(1);

            var start = Common.Utility.DateHelper.GetStartDate(Common.Utility.DateTimeType.Week, DateTime.Now).Date.ToString("yyy-MM-dd HH:mm:ss");
            var end = Common.Utility.DateHelper.GetEndDate(Common.Utility.DateTimeType.Week, DateTime.Now).Date.AddDays(1).AddSeconds(-1).ToString("yyy-MM-dd HH:mm:ss");
            GetData(start, end, "当周");
        }
        public void TotalMonth_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            setTotalStyle(2);
            var start = Common.Utility.DateHelper.GetStartDate(Common.Utility.DateTimeType.Month, DateTime.Now).Date.ToString("yyy-MM-dd HH:mm:ss");
            var end = Common.Utility.DateHelper.GetEndDate(Common.Utility.DateTimeType.Month, DateTime.Now).Date.AddDays(1).AddSeconds(-1).ToString("yyy-MM-dd HH:mm:ss");
            GetData(start, end, "当月");
        }
        public void TotalYear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            setTotalStyle(3);
            var start = Common.Utility.DateHelper.GetStartDate(Common.Utility.DateTimeType.Year, DateTime.Now).Date.ToString("yyy-MM-dd HH:mm:ss");
            var end = Common.Utility.DateHelper.GetEndDate(Common.Utility.DateTimeType.Year, DateTime.Now).Date.AddDays(1).AddSeconds(-1).ToString("yyy-MM-dd HH:mm:ss");
            GetData(start, end, "当年");
        }

        public void setTotalStyle(int index)
        {
            //TotalToday.SetResourceReference(StyleProperty, "whiteStyle");
            //TotalWeek.SetResourceReference(StyleProperty, "whiteStyle");
            //TotalMonth.SetResourceReference(StyleProperty, "whiteStyle");
            //TotalYear.SetResourceReference(StyleProperty, "whiteStyle");
            //if (index == 0) TotalToday.SetResourceReference(StyleProperty, "yellowStyle");
            //else if (index == 1) TotalWeek.SetResourceReference(StyleProperty, "yellowStyle");
            //else if (index == 2) TotalMonth.SetResourceReference(StyleProperty, "yellowStyle");
            //else if (index == 3) TotalYear.SetResourceReference(StyleProperty, "yellowStyle");
        }

        public void GetData(string beginDate, string endDate, string dateRange)
        {
            if (string.IsNullOrEmpty(beginDate))
            {
                beginDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            }

            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd");
            }

            RecordDT.Rows.Clear();
            var before1 = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd");
            var before2 = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss");

            var maxAbnormalData = SettingsHelper
                    .ZXWeighingRecordSettings.Settings["MaxAbnormalData"]?.Value.TryGetDecimal() ?? 100000m;

            //获取当前选择日期的统计值
            var zh = Common.Data.SQLDataAccess.GetTotal(beginDate, endDate, maxAbnormalData);
            //获取前一天的统计值
            var zh1 = Common.Data.SQLDataAccess.GetTotal(before1, before2, maxAbnormalData);
            //获取当前选择日期的统计值
            var xs = Common.Data.SQLDataAccess.GetCountByGblx(beginDate, endDate, "销售", maxAbnormalData);
            //获取前一天的统计值
            var xs1 = Common.Data.SQLDataAccess.GetCountByGblx(before1, before2, "销售", maxAbnormalData);
            //获取当前选择日期的统计值
            var cg = Common.Data.SQLDataAccess.GetCountByGblx(beginDate, endDate, "采购", maxAbnormalData);
            //获取前一天的统计值
            var cg1 = Common.Data.SQLDataAccess.GetCountByGblx(before1, before2, "采购", maxAbnormalData);

            zh = zh == null ? new Common.Models.WeighingRecordModel() : zh;
            zh1 = zh1 == null ? new Common.Models.WeighingRecordModel() : zh1;
            xs = xs == null ? new Common.Models.WeighingRecordModel() : xs;
            xs1 = xs1 == null ? new Common.Models.WeighingRecordModel() : xs1;
            cg = cg == null ? new Common.Models.WeighingRecordModel() : cg;
            cg1 = cg1 == null ? new Common.Models.WeighingRecordModel() : cg1;

            //this.mz = string.IsNullOrEmpty(zh.Mz) ? "0" : zh.Mz;
            //this.mz_1 = string.IsNullOrEmpty(zh1.Mz) ? "0" : zh1.Mz;
            //this.jz = string.IsNullOrEmpty(zh.Jz) ? "0" : zh.Jz;
            //this.jz_1 = string.IsNullOrEmpty(zh1.Jz) ? "0" : zh1.Jz;

            decimal outNum = 0;
            decimal.TryParse(zh.Mz, out outNum);
            this.mz = outNum.ToString("0.##");

            outNum = 0;
            decimal.TryParse(zh1.Mz, out outNum);
            this.mz_1 = outNum.ToString("0.##");

            outNum = 0;
            decimal.TryParse(zh.Jz, out outNum);
            this.jz = outNum.ToString("0.##");

            outNum = 0;
            decimal.TryParse(zh1.Jz, out outNum);
            this.jz_1 = outNum.ToString("0.##");

            this.cc = string.IsNullOrEmpty(zh.By1) ? "0" : zh.By1;
            this.cc_1 = string.IsNullOrEmpty(zh1.By1) ? "0" : zh1.By1;
            this.cjjl = string.IsNullOrEmpty(zh.By2) ? "0" : zh.By2;
            this.cjjl_1 = string.IsNullOrEmpty(zh1.By2) ? "0" : zh1.By2;
            this.gblx1 = string.IsNullOrEmpty(xs.Gblx) ? "0" : xs.Gblx;
            this.gblx1_1 = string.IsNullOrEmpty(xs1.Gblx) ? "0" : xs1.Gblx;
            this.gblx2 = string.IsNullOrEmpty(cg.Gblx) ? "0" : cg.Gblx;
            this.gblx2_1 = string.IsNullOrEmpty(cg1.Gblx) ? "0" : cg1.Gblx;



            var unit = Common.Utility.SettingsHelper.AWSV2Settings.Settings["WeighingUnit"].Value;
            this.mz += unit;
            this.mz_1 += unit;
            this.jz += unit;
            this.jz_1 += unit;
            this.cc += "次";
            this.cc_1 += "次";
            this.cjjl += "条";
            this.cjjl_1 += "条";
            this.gblx1 += "条";
            this.gblx1_1 += "条";
            this.gblx2 += "条";
            this.gblx2_1 += "条";

            DataRow dr = RecordDT.NewRow();
            dr["统计时间"] = dateRange;
            dr["总毛重"] = this.mz;
            dr["总净重"] = this.jz;
            dr["总车次"] = this.cc;
            dr["称重记录"] = this.cjjl;
            dr["销售过磅"] = this.gblx1;
            dr["采购过磅"] = this.gblx2;
            RecordDT.Rows.Add(dr);

            DataRow dr1 = RecordDT.NewRow();
            dr1["统计时间"] = "前一天";
            dr1["总毛重"] = this.mz_1;
            dr1["总净重"] = this.jz_1;
            dr1["总车次"] = this.cc_1;
            dr1["称重记录"] = this.cjjl_1;
            dr1["销售过磅"] = this.gblx1_1;
            dr1["采购过磅"] = this.gblx2_1;
            RecordDT.Rows.Add(dr1);
        }


    }
}
