using AWSV2.Models;
using Stylet;
using AWSV2.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.ViewModels
{
    public class SysLogViewModel : Screen
    {
        private IWindowManager windowManager;

        private List<SysLogModel> SysLogList { get; set; } = SQLDataAccess.LoadSysLog("%");
        public BindableCollection<SysLogModel> SysLogs { get; set; }
        public SysLogModel SelectedLog { get; set; }
        public string StatusBar { get; set; }
        public string KeyWords { get; set; }

        public SysLogViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            SysLogs = new BindableCollection<SysLogModel>(SysLogList);
        }

        public void SearchLog()
        {
            SysLogList = SQLDataAccess.LoadSysLog(KeyWords);
            SysLogs = new BindableCollection<SysLogModel>(SysLogList);
        }

        public void DeleteSelected()
        {
            if(SelectedLog == null)
            {
                StatusBar = "请选择一条记录";
                return;
            }

            var vm = new ConfirmWithPwdViewModel();
            bool? result = windowManager.ShowDialog(vm);

            if (result.GetValueOrDefault(true))
            {
                SQLDataAccess.DeleteSysLog(SelectedLog);
                SearchLog();
                StatusBar = "选中记录已删除";
            }
        }

        public void DeleteList()
        {
            var vm = new ConfirmWithPwdViewModel();
            bool? result = windowManager.ShowDialog(vm);

            if (result.GetValueOrDefault(true))
            {
                foreach (var log in SysLogList)
                {
                    SQLDataAccess.DeleteSysLog(log);
                }
                SearchLog();
                StatusBar = "记录列表已删除";
            }
        }
    }
}
