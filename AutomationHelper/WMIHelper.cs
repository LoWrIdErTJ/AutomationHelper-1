using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Management;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Diagnostics;


namespace AutomationHelper
{
    public class WMIHelper
    {       
        private ConnectionOptions _connectionOptions;
        private ManagementScope mgtScope;
        private int defaultTimeOutSeconds = 120;
        private List<ManagementObject> computerSystemObjects;
        private List<ManagementObject> operationSystemObjects;
        private List<ManagementObject> processorObjects;
        private List<ManagementObject> timezoneObjects;
        private List<ManagementObject> logicalDiskObjects;
        private List<ManagementObject> ipObjects;
        private List<ManagementObject> envVarObjects;

        private string query_computersystem = "Select * from Win32_ComputerSystem";
        private string query_operationsystem = "SELECT * FROM  Win32_OperatingSystem";
        private string query_processor = "Select * from Win32_Processor";
        private string query_timezone = "Select * from Win32_TimeZone";
        private string query_logicdisk = "Select * from win32_logicalDisk where driveType=3";
        private string query_network = "Select * from Win32_NetworkAdapterConfiguration";
        private string query_environmentvariable = "Select * from Win32_Environment";


        private string _serverName;        

        public WMIHelper(string server, string username, string password)
        {
            string pingerror = null;
            
            // Ping server first, but might be fail.
            PingServer(server, out pingerror);

            _serverName = server;
            _connectionOptions = new ConnectionOptions();
            _connectionOptions.Username = username;
            _connectionOptions.Password = password;
            _connectionOptions.EnablePrivileges = true;
            _connectionOptions.Authentication = AuthenticationLevel.Default;
            _connectionOptions.Timeout = new TimeSpan(0, 15, 0);

            try
            {
                mgtScope = new ManagementScope(string.Format(@"\\{0}\root\CIMV2", server), _connectionOptions);
                if (!mgtScope.IsConnected)
                    mgtScope.Connect();
            }
            catch (Exception ex)
            {
                string exception = null;
                if (!string.IsNullOrEmpty(pingerror))
                    exception = string.Format("{0} | {1}", pingerror, ex.Message);
                else
                    exception = ex.Message;

                throw new Exception(exception);
            }
        }

        /// <summary>
        /// Identify if searching product is installed on System.
        /// </summary>
        /// <param name="productnames"></param>
        /// <returns></returns>
        public string GetProducts()
        {
            string products = string.Empty;
            string query = string.Format("Select * from Win32_Product");

            try
            {
                List<ManagementObject> molist = WMISearch(query, mgtScope, defaultTimeOutSeconds);
                foreach (ManagementObject mo in molist)
                {
                    products += string.Format("{0} {1};",
                        mo.Properties["Caption"].Value == null ? string.Empty : mo.Properties["Caption"].Value.ToString(),
                        mo.Properties["Version"].Value == null ? string.Empty : mo.Properties["Version"].Value.ToString());
                }
            }
            catch(Exception ex){
                Console.WriteLine("Error On {0}: {1}", mgtScope.Path, ex.Message);
            }
            
            return products.Length > 0 ? products.Substring(0, products.Length-1) : string.Empty;
        }

        public string GetEnvironmentVariable(string name)
        {
            string value = null;
            try
            {
                envVarObjects = WMISearch(query_environmentvariable, mgtScope, defaultTimeOutSeconds);

                foreach (ManagementObject envVarObj in envVarObjects)
                {
                    if ((envVarObj["Name"] as string).Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        value = envVarObj["VariableValue"] as string;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetIEVersion(string iepath)
        {
            return GetFileVersion(iepath);
        }

        public string GetPhysicalMemory()
        {
            string tpm = GetInfo(ref computerSystemObjects, query_computersystem, "TotalPhysicalMemory");
            return Math.Round(Convert.ToDouble(tpm) / 1024 / 1024, 0).ToString();
        }

        public string GetName()
        {
            return GetInfo(ref computerSystemObjects, query_computersystem, "Name");
        }

        public string GetAvailablePhysicalMemory()
        {
            string fpm = GetInfo(ref operationSystemObjects, query_operationsystem, "FreePhysicalMemory");
            return Math.Round(Convert.ToDouble(fpm) / 1024, 0).ToString();
        }

        public string GetCPUInfo()
        {
            return GetInfo(ref processorObjects, query_processor, "Name");
        }

        public string GetCPUUsage()
        {
            string result = null;

            if (processorObjects == null)
            {
                try
                {
                    processorObjects = WMISearch(query_processor, mgtScope, defaultTimeOutSeconds);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            try
            {
                double loadPercentage = 0;
                int i = 0;

                foreach (ManagementObject obj in processorObjects)
                {
                    i = i + 1;
                    loadPercentage += Convert.ToDouble(obj.Properties["LoadPercentage"].Value.ToString());
                }
                result += string.Format("{0}%", loadPercentage / i);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return result;            
        }

        public string GetDriverInfo()
        {
            string result = null;

            if (logicalDiskObjects == null)
            {
                try
                {
                    logicalDiskObjects = WMISearch(query_logicdisk, mgtScope, defaultTimeOutSeconds);
                }
                catch (Exception ex)
                {
                    return ex.InnerException.Message;
                }
            }

            try
            {
                foreach (ManagementObject obj in logicalDiskObjects)
                {
                    result += string.Format("{0} ({1},{3}/{2}MB) ",
                        obj.Properties["Name"].Value.ToString(),
                        obj.Properties["FileSystem"].Value.ToString(),
                        Math.Round(Convert.ToDouble(obj.Properties["Size"].Value.ToString()) / 1024 / 1024, 0),
                        Math.Round(Convert.ToDouble(obj.Properties["FreeSpace"].Value.ToString()) / 1024 / 1024, 0));
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return result;
        }

        public string GetOS()
        {
            return GetInfo(ref operationSystemObjects, query_operationsystem, "Caption"); 
        }

        public string GetOSBit()
        {
            string bit = null;

            bit = GetInfo(ref processorObjects, query_processor, "AddressWidth");

            return bit.Substring(0, 2);
        }

        public string GetCodeSet()
        {
            return GetInfo(ref operationSystemObjects, query_operationsystem, "CodeSet");
        }

        public string GetServicePackNumber()
        {
            return GetInfo(ref operationSystemObjects, query_operationsystem, "CSDVersion");
        }

        public string GetCurrentTimeZone()
        {
            return GetInfo(ref timezoneObjects, query_timezone, "Caption");
        }

        public string GetLocale()
        {
            string lang = GetInfo(ref operationSystemObjects, query_operationsystem, "Locale");
            CultureInfo ci = new CultureInfo(Convert.ToInt32(lang, 16));
            return ci.DisplayName;
        }

        public string GetOSLanguage()
        {
            string lang = GetInfo(ref operationSystemObjects, query_operationsystem, "OSLanguage");
            CultureInfo ci = new CultureInfo(Convert.ToInt32(lang, 10));
            return ci.DisplayName;
        }

        public string GetSerialNumber()
        {
            return GetInfo(ref operationSystemObjects, query_operationsystem, "SerialNumber");
        }

        public string GetIPs()
        {
            string ips = string.Empty;

            try
            {
                ipObjects = WMISearch(query_network, mgtScope, defaultTimeOutSeconds);

                foreach (ManagementObject ip in ipObjects)
                {
                    if ((bool)ip["IPEnabled"])
                    {
                        ips += string.Format("{0};", ((string[])ip["IPAddress"])[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return ips.Length > 0 ?ips.Substring(0, ips.Length-1) : string.Empty;
        }

        public string GetInfo(ref List<ManagementObject> manobj, string query, string property)
        {
            string result = null;

            if (manobj == null)
            {
                try
                {
                    manobj = WMISearch(query, mgtScope, defaultTimeOutSeconds);
                }
                catch (Exception ex)
                {
                    return ex.InnerException.Message;
                }
            }

            try
            {
                result = manobj[0].Properties[property].Value == null ? string.Empty : manobj[0].Properties[property].Value.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return result;
        }

        public string GetFileVersion(string file)
        {
            string result = null;

            string drive = string.IsNullOrEmpty(Path.GetPathRoot(file)) ? string.Empty : Path.GetPathRoot(file).Remove(2, 1);
            string path = string.IsNullOrEmpty(Path.GetDirectoryName(file)) ? string.Empty : string.Format(@"{0}\", Path.GetDirectoryName(file)).Remove(0, 2).Replace(@"\", @"\\");
            string filename = Path.GetFileNameWithoutExtension(file);
            string fileext = string.IsNullOrEmpty(Path.GetExtension(file)) ? string.Empty : Path.GetExtension(file).Remove(0, 1);

            string query = "SELECT * FROM CIM_DataFile WHERE";
            if (!string.IsNullOrEmpty(drive))
                query += string.Format(" Drive='{0}' AND", drive);

            if (!string.IsNullOrEmpty(path))
                query += string.Format(" Path like '%{0}%' AND", path);

            if (!string.IsNullOrEmpty(filename))
                query += string.Format(" FileName='{0}' AND", filename);

            if (!string.IsNullOrEmpty(fileext))
                query += string.Format(" Extension='{0}'", fileext);

            try
            {
                List<ManagementObject> molist = WMISearch(query, mgtScope, defaultTimeOutSeconds);
                foreach (ManagementObject mo in molist)
                {
                    result = mo.Properties["Version"].Value == null ? string.Empty : mo.Properties["Version"].Value.ToString();
                }
            }
            catch { }

            return result;
        }

        private List<ManagementObject> WMISearch(string query, ManagementScope scope, int timeoutseconds)
        {
            List<ManagementObject> objs = new List<ManagementObject>();

            try
            {
                ManagementObjectSearcher search = new ManagementObjectSearcher(scope, new ObjectQuery(query));
                search.Options.Timeout = new TimeSpan(0, 0, 0, timeoutseconds, 0);
                ManagementObjectCollection collection = search.Get();

                foreach (ManagementObject mObject in collection)
                {
                    objs.Add(mObject);
                }

                if (objs.Count == 0)
                    throw new Exception(string.Format("No object return.", query));
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Query: {0}, failed with exception: {1}.", query, ex.Message));
            }

            return objs;
        }

        private bool PingServer(string server, out string error)
        {
            bool result = false;
            error = string.Empty;

            Ping ping = new Ping();


            int i = 0;
            while (i < 4 && result == false)
            {
                try
                {
                    PingReply pingreply = ping.Send(server);
                    switch (pingreply.Status)
                    {
                        case IPStatus.Success:
                            result = true;
                            break;
                        case IPStatus.TimedOut:
                            error = string.Format("Ping server {0} time out.", server);
                            result = false;
                            break;
                        case IPStatus.DestinationUnreachable:
                            error = string.Format("Ping server {0} unreachable.", server);
                            result = false;
                            break;
                        default:
                            error = string.Format("Ping server {0} fail with unknown reason.", server);
                            result = false;
                            break;

                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                
                i++;
            }

            return result;
        }
    }
}