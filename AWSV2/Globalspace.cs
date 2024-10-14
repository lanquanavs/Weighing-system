using AWSV2.ViewModels;
using Common.LPR;
using Common.Model;
using Stylet;
using System;
using System.Collections.Generic;

namespace AWSV2
{
    public class Globalspace
    {
        //称重表单模版
        public static string _weightFormTemplatePath = string.Format("{0}\\Data\\WeighFormTemplate.xlsx", AppDomain.CurrentDomain.BaseDirectory);

        public static Common.Models.User _currentUser;

        public static bool _isManual = false;

        public static int _dcRead = -1;
        public static IntPtr _icdev = IntPtr.Zero;

        //车牌识别系统传递来的车牌号
        public static string _lprDevNo = string.Empty;
        public static string _plateNo { get; set; } = string.Empty;
        public static string _lprImgPath = string.Empty;
        public static bool _getPlateFromLpr = false;
        public static string _getPlateFrom = "";
        public static string _signoState = "3"; //落杆状态为1 01（落右杆） 10（落左杆） 11（默认状态，落双杆）

        public static string _diState = "";//控制器DI端口状态，1、按钮按下，0、按钮未按下
        
        public static string _gpioState = "1";
        /// <summary>
        /// 新增的臻识相机1光栅状态数组,第一位表示光栅1,第二位表示光栅2, 值 0 表示遮挡, 1 未遮挡 --阿吉 2023年10月10日08点57分
        /// </summary>
        public static List<int> _ajCamera1GPIOState = new List<int> { 1,1};
        /// <summary>
        /// 新增的臻识相机2光栅状态数组,第一位表示光栅1,第二位表示光栅2, 值 0 表示遮挡, 1 未遮挡 --阿吉 2023年10月10日08点57分
        /// </summary>
        public static List<int> _ajCamera2GPIOState = new List<int> { 1, 1 };

        public static string _axleNo = "0";
        /// <summary>
        /// 当前称重出口的车牌号码。称重结束后这个变量会被清空。
        /// </summary>
        public static string CurrentOutPlate=String.Empty;

        /// <summary>
        /// 当前称重产生的费用
        /// </summary>
        public static string CurrentCost ="0";

        public static ShellViewModel ShellViewModel;

        public static Common.Model.Custom.ServiceProvider Provicer { get; set; }

        public static string ProvicerTitle { get; set; }

        /// <summary>
        /// 当前称重按钮的状态文本
        /// </summary>
        public static string CurrentBtnContent { get; set; } = String.Empty;

        /// <summary>
        /// 当前活动状态的按钮索引。0、称重按钮，1、一次称重按钮，2、二次称重按钮，3、无活动按钮
        /// </summary>
        public static int CurrentBtnIndex = 3;

        public static IWindowManager windowManager;
        public static string ImagsSource { get; set; }

        /// <summary>
        /// 存储车牌颜色，值来自 车牌识别
        /// </summary>
        public static CarPlateColor CarPlateColorKey { get; set; }

    }
}
