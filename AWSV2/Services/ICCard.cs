using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AWSV2.Services
{
    public class ICCard
    {
        #region 对USB接口的使用(PHILIPH卡)
        [DllImport("dcrf32.dll")]
        public static extern int dc_init(Int16 port, int baud);  //初试化
        [DllImport("dcrf32.dll")]
        public static extern short dc_exit(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_reset(int icdev, uint sec);
        [DllImport("dcrf32.dll")]
        public static extern short dc_config_card(int icdev, byte cardtype);
        [DllImport("dcrf32.dll")]
        public static extern short dc_request(int icdev, char _Mode, ref uint TagType);
        [DllImport("dcrf32.dll")]
        public static extern short dc_card(int icdev, char _Mode, ref ulong Snr);
        [DllImport("dcrf32.dll")]
        public static extern short dc_halt(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_anticoll(int icdev, char _Bcnt, ref ulong IcCardNo);
        [DllImport("dcrf32.dll")]
        public static extern short dc_beep(int icdev, uint _Msec);
        [DllImport("dcrf32.dll")]
        public static extern short dc_authentication(int icdev, int _Mode, int _SecNr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_load_key(int icdev, int mode, int secnr, [In] byte[] nkey);  //密码装载到读写模块中
        [DllImport("dcrf32.dll")]
        public static extern short dc_load_key_hex(int icdev, int mode, int secnr, string nkey);  //密码装载到读写模块中

        [DllImport("dcrf32.dll")]
        public static extern short dc_write(int icdev, int adr, [In] byte[] sdata);  //向卡中写入数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_write(int icdev, int adr, [In] string sdata);  //向卡中写入数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_hex(int icdev, int adr, [In] string sdata);  //向卡中写入数据(转换为16进制)

        [DllImport("dcrf32.dll")]
        public static extern short dc_read(int icdev, int adr, [Out] byte[] sdata);

        [DllImport("dcrf32.dll")]
        public static extern short dc_read(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_read_hex(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据(转换为16进制)
        [DllImport("dcrf32.dll")]
        public static extern int a_hex(string oldValue, ref string newValue, Int16 len);  //普通字符转换成十六进制字符
        [DllImport("dcrf32.dll")]
        public static extern void hex_a(ref string oldValue, ref string newValue, int len);  //十六进制字符转换成普通字符

        #endregion

        #region 明华串口读卡器
        [DllImport("mwrf32.dll", EntryPoint = "rf_init", SetLastError = true,
               CharSet = CharSet.Auto, ExactSpelling = false,
               CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr rf_init(Int16 port, int baud);

        [DllImport("mwrf32.dll", EntryPoint = "rf_exit", SetLastError = true,
               CharSet = CharSet.Auto, ExactSpelling = false,
               CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_exit(IntPtr icdev);

        [DllImport("mwrf32.dll", EntryPoint = "rf_beep", SetLastError = true,
               CharSet = CharSet.Auto, ExactSpelling = false,
               CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_beep(IntPtr icdev, int msec);

        [DllImport("mwrf32.dll", EntryPoint = "rf_card", SetLastError = true,
            CharSet = CharSet.Auto, ExactSpelling = false,
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int16 rf_card(IntPtr icdev, byte mode, byte[] snr);
        #endregion

        public int IcDev { get; set; } = -1;

        public int InitIC(short devNo)
        {
            return dc_init(devNo, 0);
        }

        //关闭串口
        public void ExitIC(short devNo)
        {
            dc_exit(devNo);
        }


    }
}
