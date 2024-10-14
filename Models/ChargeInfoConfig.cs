using Common.Model.ChargeInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.Models
{
    public class ChargeInfoConfig
    {
        #region 收费模块参数 by:汪虎 2022-02-21

        /// <summary>
        /// 是否启用收费。0：不启用，1：启用
        /// </summary>
        public bool ChargeEnable { get; set; }

        /// <summary>
        /// 是否启用多次(第一次收费+第二次收费)收费。0：不启用，1：启用
        /// </summary>
        public bool MultipleChargeEnable { get; set; }

        /// <summary>
        ///收费方式。1、单位收费，2、范围收费 ,3 按单价收费
        /// </summary>
        public string ChargeType { get; set; }

        /// <summary>
        /// 是否启用客户储值功能。0：不启用，1：启用
        /// </summary>
        public bool ChargeStorage { get; set; }

        /// <summary>
        /// 是否启用按物资收费。0：不启用，1：启用
        /// </summary>
        public bool ChargeByMaterial { get; set; }

        /// <summary>
        /// 0：缴费后出场，1：缴费后下磅
        /// </summary>
        public string ChargeWay { get; set; }

        /// <summary>
        /// 保底收费设置
        /// </summary>
        public string LowestFees { get; set; }

        /// <summary>
        /// 收费模式：0 按毛重+皮重收费，1按净重收费
        /// </summary>
        public string ChargeTypes { get; set; }

        /// <summary>
        /// 收费规则
        /// </summary>
        public IList<ChargeRuleModel> ChargeRuleList { get; set; } = new List<ChargeRuleModel>();

        //各收费标准信息（包含：普通收费 id=Normal_888888，物资收费6种方式）
        public List<ChargeInfoModel> ChargeInfo { get; set; } = new List<ChargeInfoModel>();

        #endregion

        public ChargeInfoConfig(AppSettingsSection _mainSetting)
        {
            if (_mainSetting != null)
            {
                ChargeEnable = Convert.ToBoolean(Convert.ToInt32(_mainSetting.Settings["ChargeEnable"]?.Value ?? "0"));
                MultipleChargeEnable = Convert.ToBoolean(Convert.ToInt32(_mainSetting.Settings["MultipleChargeEnable"]?.Value ?? "0"));

                ChargeType = _mainSetting.Settings["ChargeType"].Value.ToString();
                ChargeStorage = Convert.ToBoolean(Convert.ToInt32(_mainSetting.Settings["ChargeStorage"]?.Value ?? "0"));
                ChargeByMaterial = Convert.ToBoolean(Convert.ToInt32(_mainSetting.Settings["ChargeByMaterial"]?.Value ?? "0"));
                ChargeWay = _mainSetting.Settings["ChargeWay"].Value.ToString();
                LowestFees = _mainSetting.Settings["LowestFees"].Value.ToString();
                ChargeTypes = _mainSetting.Settings["ChargeTypes"].Value.ToString();
                ChargeInfo = JsonConvert.DeserializeObject<List<ChargeInfoModel>>(_mainSetting.Settings["ChargeInfo"].Value);
            }
            
        }
    }
}
