using System;
using System.Runtime.InteropServices;

namespace AutomationHelper
{
    public class CultureHelper
    {
        [DllImport("kernel32.dll", EntryPoint = "GetSystemDefaultLCID")]
        public static extern int GetSystemDefaultLCID();

        [DllImport("kernel32.dll", EntryPoint = "SetLocaleInfoA")]
        public static extern int SetLocaleInfo(int locale, int lcType, string lpLcData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "SetLocaleInfoW")]
        public static extern int SetLocaleInfoW(int locale, int lcType, string lpLcData);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, UInt32 msg, Int32 wParam, Int32 lParam);

        [DllImport("User32.Dll", EntryPoint = "PostMessageA", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        public const int LocaleSlongdate = 0x20;
        public const int LocaleSshortdate = 0x1F;
        public const int LocaleStime = 0x1003;
        public const int LocaleSshorttime = 0x00000079;
        public const int LocaleS1159 = 0x28;
        public const int LocaleS2359 = 0x29;

        public static IntPtr HwndBroadcast = new IntPtr(0xffff);
        public const UInt32 WmSettingchange = 0x001A;

        static public string GetShortTimeFormat()
        {
            return GetShortTimeFormat(GetSystemDefaultLCID());
        }

        static public string GetLongTimeFormat()
        {
            return GetLongTimeFormat(GetSystemDefaultLCID());
        }

        static public string GetShortDateFormat()
        {
            return GetShortDateFormat(GetSystemDefaultLCID());
        }

        static public string GetLongDateFormat()
        {
            return GetLongDateFormat(GetSystemDefaultLCID());
        }
        
        static public string GetShortTimeFormat(int lcid)
        {
            var ci = new System.Globalization.CultureInfo(lcid);

            return ci.DateTimeFormat.ShortTimePattern;
        }

        static public string GetLongTimeFormat(int lcid)
        {
            var ci = new System.Globalization.CultureInfo(lcid);

            return ci.DateTimeFormat.LongTimePattern;
        }

        static public string GetShortDateFormat(int lcid)
        {
            var ci = new System.Globalization.CultureInfo(lcid);

            return ci.DateTimeFormat.ShortDatePattern;
        }

        static public string GetLongDateFormat(int lcid)
        {
            var ci = new System.Globalization.CultureInfo(lcid);

            return ci.DateTimeFormat.LongDatePattern;
        }

        /// <summary>
        /// set date time format by Language id
        /// </summary>
        /// <param name="lcid"></param>
        static public void SetDateTimeFormat(int lcid)
        {
            try
            {
                var ci = new System.Globalization.CultureInfo(lcid);
                SetLocaleInfo(lcid, LocaleSlongdate, ci.DateTimeFormat.LongDatePattern);
                SetLocaleInfo(lcid, LocaleSshortdate, ci.DateTimeFormat.ShortDatePattern);
                SetLocaleInfo(lcid, LocaleStime, ci.DateTimeFormat.LongTimePattern);
                //SetLocaleInfo(lcid, LOCALE_SSHORTTIME, ci.DateTimeFormat.ShortTimePattern);
                // The below code is a workaround, because LOCALE_SSHORTTIME does not work.
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\International", "sShortTime", ci.DateTimeFormat.LongTimePattern.Replace(":ss", ""));
                //SendMessage(HWND_BROADCAST, WM_SETTINGCHANGE, 0, 0);
                if (!string.IsNullOrEmpty(ci.DateTimeFormat.AMDesignator))
                    SetLocaleInfoW(lcid, LocaleS1159, ci.DateTimeFormat.AMDesignator);
                if (!string.IsNullOrEmpty(ci.DateTimeFormat.PMDesignator))
                    SetLocaleInfoW(lcid, LocaleS2359, ci.DateTimeFormat.PMDesignator);
                PostMessage(HwndBroadcast, WmSettingchange, 0, 0);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// set data time format to be specified format.
        /// </summary>
        /// <param name="shortdateformat"></param>
        /// <param name="shorttimeformat"></param>
        /// <param name="longdateformat"></param>
        /// <param name="longtimeformat"></param>
        /// <param name="am"></param>
        /// <param name="pm"></param>
        static public void SetDateTimeFormat(string shortdateformat, string shorttimeformat, string longdateformat, string longtimeformat, string am, string pm)
        {
            try
            {
                var x = GetSystemDefaultLCID();
                SetLocaleInfo(x, LocaleSshortdate, shortdateformat);
                //SetLocaleInfo(x, LOCALE_SSHORTTIME, shorttimeformat);
                // The below code is a workaround, because LOCALE_SSHORTTIME does not work.
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\International", "sShortTime", longtimeformat.Replace(":ss", ""));
                //SendMessage(HWND_BROADCAST, WM_SETTINGCHANGE, 0, 0);
                SetLocaleInfo(x, LocaleSlongdate, longdateformat);
                SetLocaleInfo(x, LocaleStime, longtimeformat);
                if (!string.IsNullOrEmpty(am))
                    SetLocaleInfoW(x, LocaleS1159, am);
                if (!string.IsNullOrEmpty(pm))
                    SetLocaleInfoW(x, LocaleS2359, pm);
                PostMessage(HwndBroadcast, WmSettingchange, 0, 0);       
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
