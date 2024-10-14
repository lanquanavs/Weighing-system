using Common.Utility.AJ.Extension;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.Models
{
    public class FastWeigthCorrectConfig : PropertyChangedBase
    {
        private bool _enable;
        public bool Enable
        {
            get { return _enable; }
            set { SetAndNotify(ref _enable, value); }
        }

        private string _limit;
        public string Limit
        {
            get { return _limit; }
            set { SetAndNotify(ref _limit, value); }
        }

        private string _f11FunValue;
        public string F11FunValue
        {
            get { return _f11FunValue; }
            set { SetAndNotify(ref _f11FunValue, value); }
        }

        private string _f12FunValue;
        public string F12FunValue
        {
            get { return _f12FunValue; }
            set { SetAndNotify(ref _f12FunValue, value); }
        }

        public int GetLimit()
        {
            return _limit.TryGetInt();
        }

        public decimal GetF11FunValue()
        {
            return _f11FunValue.TryGetDecimal();
        }
        public decimal GetF12FunValue()
        {
            return _f12FunValue.TryGetDecimal();
        }
    }
}
