using Aspose.Cells;
using AWSV2.Models;
using AWSV2.Services;
using Stylet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AWSV2.ViewModels
{
    public class WeighFormListViewModel : Screen
    {
        //log
        private static readonly log4net.ILog log = LogHelper.GetLogger();

        //加载其他窗口
        private IWindowManager windowManager;

        public WeighFormModel SelectedWeighForm { get; set; }

        //用来存放excel过磅单模板的List
        private List<WeighFormModel> WeighFormList = new List<WeighFormModel>();
        public BindableCollection<WeighFormModel> WeighFormSortList { get; set; } = new BindableCollection<WeighFormModel>();

        public WeighFormModel[] WeighFormSortList1 { get; set; } = new WeighFormModel[0];

        public string StatusBar { get; set; }
        private AppSettingsSection config;

        private static WeighFormListViewModel winform = null;

        public static WeighFormListViewModel GetInstance(IWindowManager windowManager, ref AppSettingsSection cfg)
        {
            if (winform == null)
            {
                winform=new WeighFormListViewModel(windowManager,ref cfg);
            }
            return winform;
        }

        private WeighFormListViewModel(IWindowManager windowManager, ref AppSettingsSection cfg)
        {
            this.windowManager = windowManager;
            config = cfg;

            LoadExcelToList(); //读取excel，存放数据到LIST里
            WeighFormSortList1 = new WeighFormModel[WeighFormSortList.Count];
            //WeighFormSortList.CopyTo(WeighFormSortList1,0);

            //WeighFormSortList1 = (WeighFormModel[])WeighFormSortList.ToArray().Clone();


            //for (int i = 0; i < WeighFormSortList.Count; i++)
            //{

            //    WeighFormSortList1[i] = new WeighFormModel() { Key = WeighFormSortList[i].Key, Value = WeighFormSortList[i] .Value};
            //}

        }


        private void LoadExcelToList()
        {
            //读取过磅单中的字段
            try
            {
                //打开过磅单模板
                Worksheet worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[0];

                //设置查找区域：有内容的区域
                var range = worksheet.Cells.MaxDisplayRange;
                string cellRange = range.RefersTo.Split('!')[1].Replace("$", "");
                string startCellName = cellRange.Split(':')[0];
                string endCellName = cellRange.Split(':')[1];
                CellArea area = CellArea.CreateCellArea(startCellName, endCellName);
                //查找规则
                FindOptions opts = new FindOptions();
                opts.LookInType = LookInType.Values;
                opts.LookAtType = LookAtType.Contains;
                opts.SetRange(area);

                Cell cell = null;
                do
                {
                    // Search the cell contain value search within range
                    cell = worksheet.Cells.Find("_", cell, opts);

                    // If no such cell found, then break the loop
                    if (cell == null)
                        break;

                    // 通过读取配置文件把key-value放到WeighFormList中
                    WeighFormModel wf = new WeighFormModel
                    {
                        Key = cell.Value.ToString(),
                        Value = ConfigurationManager.AppSettings[cell.Value.ToString()]
                    };
                    if (wf.Value != null && !WeighFormList.Any(a => a.Key == wf.Key))
                    {
                        WeighFormList.Add(wf);
                    }


                } while (true);

                string[] sortList = ConfigurationManager.AppSettings["ListSort"].Split(',');
                List<string> sortKeyList = new List<string>(sortList);

                foreach (var sortItem in sortKeyList)
                {
                    for (int i = 0; i < WeighFormList.Count; i++)
                    {
                        if (sortItem == WeighFormList[i].Key)
                        {
                            WeighFormSortList.Add(WeighFormList[i]);
                            WeighFormList.Remove(WeighFormList[i]);
                        }
                    }
                }
                for (int i = 0; i < WeighFormList.Count; i++)
                {
                    WeighFormSortList.Add(WeighFormList[i]);
                }
            }
            catch { }
        }

        public void SaveListName()
        {
            if (SelectedWeighForm == null)
            {
                StatusBar = "请选择一个条目";
                return;
            }
            //if (SelectedWeighForm.Key.ToLower() == ("_by1") ||
            //    SelectedWeighForm.Key.ToLower() == ("_by2") ||
            //    SelectedWeighForm.Key.ToLower() == ("_by3") ||
            //    SelectedWeighForm.Key.ToLower() == ("_by4") ||
            //    SelectedWeighForm.Key.ToLower() == ("_by5"))
            //{

            //    //var oldData = WeighFormSortList1.FirstOrDefault(p => p.Key == SelectedWeighForm.Key);
            //    //if (oldData != null)
            //    //{
            //    //    SelectedWeighForm.Value = oldData.Value;
            //    //}
            //    //WeighFormList.Clear();
            //    //LoadExcelToList(); //读取excel，存放数据到LIST里

            //    StatusBar = "By1 - By5不允许修改！";
            //    return;
            //}

            var keys = config.Settings.AllKeys.Where(x => x.StartsWith("_")).ToList();
            var repeats = keys.Where(x => config.Settings[x].Value == SelectedWeighForm.Value && x != SelectedWeighForm.Key).ToList();

            if (repeats.Count() >0)
            {
                var firstItem = repeats.FirstOrDefault();
                StatusBar = $"{firstItem}已经设置为：{SelectedWeighForm.Value}，不可以重复设置！";
                return;
            }

            //foreach (var key in keys)
            //{
            //    var val = config.AppSettings.Settings[SelectedWeighForm.Key].Value;
            //    if (!string.IsNullOrEmpty(val) && val == SelectedWeighForm.Value && key != SelectedWeighForm.Key)
            //    {
            //        StatusBar = $"{key}已经设置为：{val}，不可以重复设置！";
            //        return;
            //    }
            //}

            config.Settings[SelectedWeighForm.Key].Value = SelectedWeighForm.Value;

            StatusBar = "更新名称完成";

            log.Info("修改磅单列表中的名称");
        }

        public void SaveListSort()
        {
            string str = string.Empty;
            foreach (var item in WeighFormSortList)
            {
                str = str + item.Key + ",";
            }
            str = str.Remove(str.Length - 1);

            config.Settings["ListSort"].Value = str;

            StatusBar = "更新排序完成";

            log.Info("修改磅单列表排序");
        }
    }
}
