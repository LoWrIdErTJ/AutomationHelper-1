using System;
using System.Runtime.InteropServices;

namespace AutomationHelper
{
    public class TimeHelper
    {
        /// <summary>
        /// SYSTEMTIME structure with some useful methods
        /// </summary>
        public struct Systemtime
        {
            public ushort WYear;
            public ushort WMonth;
            public ushort WDayOfWeek;
            public ushort WDay;
            public ushort WHour;
            public ushort WMinute;
            public ushort WSecond;
            public ushort WMilliseconds;

            /// <summary>
            /// Convert form System.DateTime
            /// </summary>
            /// <param name="time"></param>
            public void FromDateTime(DateTime time)
            {
                WYear = (ushort)time.Year;
                WMonth = (ushort)time.Month;
                WDayOfWeek = (ushort)time.DayOfWeek;
                WDay = (ushort)time.Day;
                WHour = (ushort)time.Hour;
                WMinute = (ushort)time.Minute;
                WSecond = (ushort)time.Second;
                WMilliseconds = (ushort)time.Millisecond;
            }

            /// <summary>
            /// Convert to System.DateTime
            /// </summary>
            /// <returns></returns>
            public DateTime ToDateTime()
            {
                return new DateTime(WYear, WMonth, WDay, WHour, WMinute, WSecond, WMilliseconds);
            }
            /// <summary>
            /// STATIC: Convert to System.DateTime
            /// </summary>
            /// <param name="time"></param>
            /// <returns></returns>
            public static DateTime ToDateTime(Systemtime time)
            {
                return time.ToDateTime();
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SetLocalTime(ref Systemtime systemtime); 

        /// <summary>
        /// Change local system time to specified.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        static public string ChangeLocalSystemTime(ushort year, ushort month, ushort day, ushort hour, ushort minute, ushort second)
        {
            var st = new Systemtime
                         {
                             WDay = day,
                             WMonth = month,
                             WYear = year,
                             WHour = hour,
                             WMinute = minute,
                             WSecond = second
                         };

            SetLocalTime(ref st);

            return st.ToDateTime().ToString();
        }

        /// <summary>
        /// change local system time from datatime string
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        static public string ChangeLocalSystemTime(string datetime)
        {
            var t = Convert.ToDateTime(datetime);
            var st = new Systemtime();
            st.FromDateTime(t);

            SetLocalTime(ref st);
            return st.ToDateTime().ToString();
        }

        /// <summary>
        /// change local system time to be past/future by days
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        static public string ChangeLocalSystemTime(double days)
        {
            var t = DateTime.Now.AddDays(days);
            var st = new Systemtime();
            st.FromDateTime(t);

            SetLocalTime(ref st);
            return st.ToDateTime().ToString();
        }
    }
}
