using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace AWSV2.Services
{
    public static class IPCHelper
    {
        public static readonly int WM_COPYDATA = 0x004A; // 固定数值，不可更改
        public static readonly int REFRESH_DI_STATE = 0x0800;//刷新DI 端口（通道）记录值为 0，意味着LPR需要从新发送新的值给各个关联程序
        public static readonly int CLOSE_APP = 0x0801;
        public static readonly int ACTIVE_APP = 0x0802;
        public static readonly int SYNC_DATA = 0x0803;
        public static readonly int READY_TO_WEIGH = 0x0804;
        public static readonly int IN_WEIGHING = 0x0805;//IN_WEIGHING状态包括GET_PLATE和IS_WEIGHING
        public static readonly int GET_PLATE = 0x0806;
        public static readonly int WEIGHING_COMPLETE = 0x0807;
        public static readonly int IS_WEIGHING = 0x0808;
        public static readonly int UPDATE_RECORD = 0x0809;
        public static readonly int IS_STABLE = 0x080A;
        public static readonly int VIOLATE_WHITE_LIST = 0x080B;
        public static readonly int OVER_WEIGHT = 0x080C;
        private static readonly int CHARGE_FALG = 0x08AA;//发送收费通知的消息
        public static readonly int UPDATEDEVICE = 100;

        [DllImport("user32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostThreadMessage(int threadId, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        public struct IS_STABLE_OBJ
        {
            public string weight;
        }

        public static void ActiveApp(string appName)
        {
            OpenApp(appName, Globalspace._currentUser.LoginId);
            SendMsgToApp(appName, ACTIVE_APP);
        }

        public static void OpenApp(string appName, string argv = null)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(appName);

                if (proc.Length == 0)
                {
                    Process.Start(appName, argv);
                }
            }
            catch { }
        }

        public static void CloseApp(string appName)
        {
            SendMsgToApp(appName, CLOSE_APP);
        }

        public static void KillApp(string appName)
        {
            Process[] proc = Process.GetProcessesByName(appName);
            foreach (Process p in Process.GetProcessesByName(appName))
            {
                try
                {
                    p.Kill();
                    p.WaitForExit();
                }
                catch { }
            }

        }

        public static void SendMsgToApp(string appName, int msg)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(appName);

                if (proc.Length > 0)
                {
                    int threadID = Process.GetProcessById(proc[0].Id).Threads[0].Id;
                    PostThreadMessage(threadID, msg, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch { }
        }

        public static void SendMsgToApp(string appName, int msg, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(appName);

                if (proc.Length > 0)
                {
                    int threadID = Process.GetProcessById(proc[0].Id).Threads[0].Id;
                    PostThreadMessage(threadID, msg, wParam, lParam);
                }
            }
            catch { }
        }

        public static void SendMsgToApp(string appName, string msg)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(appName);

                if (proc.Length > 0)
                {
                    // 获取目标进程句柄
                    IntPtr hWnd = proc[0].MainWindowHandle;

                    // 封装消息
                    byte[] sarr = Encoding.Default.GetBytes(msg);
                    int len = sarr.Length;
                    COPYDATASTRUCT cds;
                    cds.dwData = (IntPtr)0;
                    cds.cbData = len + 1;
                    cds.lpData = msg;

                    // 发送消息
                    SendMessage(hWnd, WM_COPYDATA, IntPtr.Zero, ref cds);
                }
            }
            catch { }
        }
        public static void SendMsgToApp(string appName, int msg, string notify)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(appName);

                if (proc.Length > 0)
                {
                    int threadID = Process.GetProcessById(proc[0].Id).Threads[0].Id;
                    PostThreadMessage(threadID, msg, Marshal.StringToHGlobalAuto(notify), IntPtr.Zero);
                }
            }
            catch { }
        }
    }
}
