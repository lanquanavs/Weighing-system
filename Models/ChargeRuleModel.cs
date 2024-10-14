using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.Models
{
    public class ChargeRuleModel
    {

        public Rule ChargeRule { get; set; }

        public string Fee { get; set; }

        /// <summary>
        /// 0、范围收费规则。1、单位收费规则
        /// </summary>
        public int Type { get; set; } = 0;

    }

    public class Rule
    {
        public decimal UpperLimit { get; set; }

        public decimal LowerLimit { get; set; }
    }

}
