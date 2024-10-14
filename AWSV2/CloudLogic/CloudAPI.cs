using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AWSV2.Services;
using Common.Utility.AJ.Extension;
using Common.Utility.AJ.MobileConfiguration;
using log4net;
using NPOI.SS.Formula.Functions;
using PYun;
using PYun.API;
using PYun.API.Reply;
using PYun.API.Request;
using PYun.Events;
using PYun.Utils;

namespace AWSV2.CloudLogic
{
    public class CloudAPI
    {
        private static PYunAPI instance = null;

        private static PYunAPI CreatePYunAPI(ref MobileConfigurationMgr mobileConfigurationMgr)
        {
            if (instance == null)
            {
                //var host = "huizhou.gate.4pyun.com";
                //int port = 8661;
                //instance = new PYunAPI(host, port);
                //instance.Type = "public:parking:agent";
                ////instance.IdleInterval = 1000;
                //instance.Device = "001,002,003";//设置通道号码，多个通道号用,分割。如：1,2,3,4,5
                //instance.ChannelEventHandler += new EventHandler<ChannelEventArgs>(ChannelEventHandler);
                //instance.ServiceHandler = new PYunService();
                
                var config = mobileConfigurationMgr.SettingList[SettingNameKey.Main];
                var PServerPath = config.Settings["PServerPath"].Value;

                //var host = "shenzhen.gate.4pyun.com";
                //host = "qiandongnan.gate.4pyun.com";
               
                int port = 8661;
                instance = new PYunAPI(PServerPath, port);
                instance.Type = "public:parking:agent";
                //instance.IdleInterval = 1000;
                instance.Device = "001,002,003";//设置通道号码，多个通道号用,分割。如：1,2,3,4,5
                instance.ChannelEventHandler += new EventHandler<ChannelEventArgs>(ChannelEventHandler);
                instance.ServiceHandler = new PYunService(ref mobileConfigurationMgr);
            }
            return instance;
        }

        public static void Start(ref MobileConfigurationMgr mobileConfigurationMgr)
        {
            var maiSetting = mobileConfigurationMgr.SettingList[SettingNameKey.Main];
            var uuid = maiSetting.Settings["UUID"].Value;
            var mac = maiSetting.Settings["MAC"].Value;
            //var uuid = "a3b7080a-3684-49b7-9948-2eb67676198e";
            //var mac = "EPXUA7UUWKBJLV1U";
            CreatePYunAPI(ref mobileConfigurationMgr).Startup(uuid, mac);
        }

        public static void Shutdown()
        {
            if (instance != null)
            {
                instance.Shutdown();
                instance = null;
            }
        }

        #region Event
        static void ChannelEventHandler(object sender, ChannelEventArgs args)
        {
            switch (args.Type)
            {
                case ChannelEventType.AccessGranted:
                    //Logging(args.Type.ToString(), Color.LightGreen);
                    break;
                case ChannelEventType.AccessDenied:
                    //Logging(args.Type.ToString(), Color.Red);
                    break;
                case ChannelEventType.ChannelError:
                    //Logging(args.Type.ToString(), Color.OrangeRed);
                    break;
                case ChannelEventType.ChannelClosed:
                    //Logging(args.Type.ToString(), Color.Yellow);
                    break;
            }
        }

        #endregion
    }
}
