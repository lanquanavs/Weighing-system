using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.ViewModels
{
    public class ChargeEditViewModel : Screen
    {
        SettingViewModel _vm;

        public string ParkingID
        {
            get { return _vm.ParkingID; }
            set
            {
                _vm.ParkingID = value;
            }
        }

        public string UUID
        {
            get { return _vm.UUID; }
            set
            {
                _vm.UUID = value;
            }
        }

        public string MAC
        {
            get { return _vm.MAC; }
            set
            {
                _vm.MAC = value;
            }
        }

        public string PServerPath
        {
            get { return _vm.PServerPath; }
            set
            {
                _vm.PServerPath = value;
            }
        }

        public ChargeEditViewModel(SettingViewModel VM)
        {
            _vm = VM;
        }

        public void Confirm()
        {
            this.RequestClose(true);
        }

        public void CloseWindow()
        {
            this.RequestClose(true);
        }
    }
}
