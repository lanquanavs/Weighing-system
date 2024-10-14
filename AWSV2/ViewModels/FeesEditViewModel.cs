using Common.Model.ChargeInfo;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.ViewModels
{
    public class FeesEditViewModel : Screen
    {
        FeesItemModel feesItem;

        public decimal Fees1
        {
            get { return feesItem.Fees1; }
            set
            {
                feesItem.Fees1 = value;
            }
        }

        public decimal Fees2
        {
            get { return feesItem.Fees2; }
            set
            {
                feesItem.Fees2 = value;
            }
        }

        public decimal Fees3
        {
            get { return feesItem.Fees3; }
            set
            {
                feesItem.Fees3 = value;
            }
        }

        public decimal Fees4
        {
            get { return feesItem.Fees4; }
            set
            {
                feesItem.Fees4 = value;
            }
        }

        public decimal Fees5
        {
            get { return feesItem.Fees5; }
            set
            {
                feesItem.Fees5 = value;
            }
        }

        public decimal Fees6
        {
            get { return feesItem.Fees6; }
            set
            {
                feesItem.Fees6 = value;
            }
        }

        public decimal Fees7
        {
            get { return feesItem.Fees7; }
            set
            {
                feesItem.Fees7 = value;
            }
        }

        public decimal Fees8
        {
            get { return feesItem.Fees8; }
            set
            {
                feesItem.Fees8 = value;
            }
        }

        public decimal Fees9
        {
            get { return feesItem.Fees9; }
            set
            {
                feesItem.Fees9 = value;
            }
        }

        public int Tonnage1Min
        {
            get { return feesItem.Tonnage1Min; }
            set
            {
                feesItem.Tonnage1Min = value;
            }
        }

        public int Tonnage1Max
        {
            get { return feesItem.Tonnage1Max; }
            set
            {
                feesItem.Tonnage1Max = value;
            }
        }

        public int Tonnage2Min
        {
            get { return feesItem.Tonnage2Min; }
            set
            {
                feesItem.Tonnage2Min = value;
            }
        }

        public int Tonnage2Max
        {
            get { return feesItem.Tonnage2Max; }
            set
            {
                feesItem.Tonnage2Max = value;
            }
        }

        public int Tonnage3Min
        {
            get { return feesItem.Tonnage3Min; }
            set
            {
                feesItem.Tonnage3Min = value;
            }
        }

        public int Tonnage3Max
        {
            get { return feesItem.Tonnage3Max; }
            set
            {
                feesItem.Tonnage3Max = value;
            }
        }


        public int Tonnage4Min
        {
            get { return feesItem.Tonnage4Min; }
            set
            {
                feesItem.Tonnage4Min = value;
            }
        }

        public int Tonnage4Max
        {
            get { return feesItem.Tonnage4Max; }
            set
            {
                feesItem.Tonnage4Max = value;
            }
        }

        public int Tonnage5Min
        {
            get { return feesItem.Tonnage5Min; }
            set
            {
                feesItem.Tonnage5Min = value;
            }
        }

        public int Tonnage5Max
        {
            get { return feesItem.Tonnage5Max; }
            set
            {
                feesItem.Tonnage5Max = value;
            }
        }

        public int Tonnage6Min
        {
            get { return feesItem.Tonnage6Min; }
            set
            {
                feesItem.Tonnage6Min = value;
            }
        }

        public int Tonnage6Max
        {
            get { return feesItem.Tonnage6Max; }
            set
            {
                feesItem.Tonnage6Max = value;
            }
        }

        public int Tonnage7Min
        {
            get { return feesItem.Tonnage7Min; }
            set
            {
                feesItem.Tonnage7Min = value;
            }
        }

        public int Tonnage7Max
        {
            get { return feesItem.Tonnage7Max; }
            set
            {
                feesItem.Tonnage7Max = value;
            }
        }

        public int Tonnage8Min
        {
            get { return feesItem.Tonnage8Min; }
            set
            {
                feesItem.Tonnage8Min = value;
            }
        }

        public int Tonnage8Max
        {
            get { return feesItem.Tonnage8Max; }
            set
            {
                feesItem.Tonnage8Max = value;
            }
        }

        public int Tonnage9Min
        {
            get { return feesItem.Tonnage9Min; }
            set
            {
                feesItem.Tonnage9Min = value;
            }
        }

        public int Tonnage9Max
        {
            get { return feesItem.Tonnage9Max; }
            set
            {
                feesItem.Tonnage9Max = value;
            }
        }

        public FeesEditViewModel(FeesItemModel _feesItem)
        {
            feesItem = _feesItem;
        }

        public void Confirm()
        {
            this.RequestClose(true);
        }
    }
}
