using Common.ViewModels;
using Common.Views;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.ViewModels
{
    public class WeighPlanDialogViewModel : Screen
    {
        private WeighPlanListViewModel _weighPlan;
        public WeighPlanListViewModel WeighPlan 
        { get => _weighPlan; set => SetAndNotify(ref _weighPlan, value); }

        public WeighPlanDialogViewModel(IWindowManager windowManager)
        {
            WeighPlan  = new WeighPlanListViewModel(windowManager);
        }
    }
}
