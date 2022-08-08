using System;
using System.IO;
using System.Runtime.InteropServices;

namespace RyzenTuner
{
    public class CommonUtils
    {
        // https://www.pinvoke.net/default.aspx/user32.GetLastInputInfo
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-lastinputinfo
        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)] public int cbSize;
            [MarshalAs(UnmanagedType.U4)] public UInt32 dwTime;
        }

        /**
         * 检查提供的日期是否出于晚上
         */
        public static bool IsNight(DateTime now)
        {
            TimeSpan nightShiftStart = new TimeSpan(23, 59, 0); // 23:59pm 
            TimeSpan nightShiftEnd = new TimeSpan(7, 0, 0); // 7:00am

            if (now.TimeOfDay > nightShiftStart || now.TimeOfDay < nightShiftEnd)
            {
                return true;
            }

            return false;
        }

        /**
         * 返回系统空闲时间，单位：秒
         *
         * 参考：
         * https://stackoverflow.com/questions/203384/how-to-tell-when-windows-is-inactive
         */
        public static int GetIdleSecond()
        {
            int idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int envTicks = Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                int lastInputTick = (int)lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return ((idleTime > 0) ? (idleTime / 1000) : idleTime);
        }

        public static void LogInfo(string content)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "\\runtime\\ryzen-tuner.log.txt";
            File.AppendAllText(filePath,
                string.Format(@"[INFO]{0:yyyy-MM-dd hh:mm:ss} {1}{2}", DateTime.Now, content,
                    Environment.NewLine));
        }
    }
}