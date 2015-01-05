using System;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace AutomationHelper
{
    public class ProcessHelper
    {
        /// <summary>
        /// launch a process
        /// </summary>
        /// <param name="appPath"></param>
        /// <returns></returns>
        public static Process Launch(string appPath)
        {
            Process app = new Process();
            try
            {
                app = Process.Start(appPath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return app;
        }

        public static int Execute(string appPath, string workingdirectory, string arugments, int timeout, out string standardOutput, out string standardError)
        {
            return Execute(appPath, workingdirectory, arugments, timeout, true, out standardOutput, out standardError);
        }
        public static int Execute(string appPath, string workingdirectory, string arugments, int timeout)
        {
            return Execute(appPath, workingdirectory, arugments, timeout, true);
        }

        public static int Execute(string appPath, string workingdirectory, string arugments, int timeout, bool isNoWindow)
        {
            int exitCode = -1;
            timeout = (timeout == 0 ? 60 : timeout) * 1000;

            using (Process process = new Process())
            {
                StringBuilder consoleoutput = new StringBuilder();
                StringBuilder consoleerror = new StringBuilder();

                process.StartInfo.FileName = appPath;
                process.StartInfo.Arguments = arugments;
                if (!string.IsNullOrEmpty(workingdirectory)) process.StartInfo.WorkingDirectory = workingdirectory;
                process.StartInfo.CreateNoWindow = isNoWindow;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                if (process.WaitForExit(timeout))
                {
                    exitCode = process.ExitCode;
                }
                else
                {
                    process.Kill();
                }
            }

            return exitCode;
        }

        //execute a process until it exits.
        public static int Execute(string appPath, string workingdirectory, string arugments, int timeout, bool isNoWindow, out string standardOutput, out string standardError)
        {
            int exitCode = -1;
            timeout = (timeout == 0 ? 60 : timeout )* 1000;

            using (Process process = new Process())
            {
                StringBuilder consoleoutput = new StringBuilder();
                StringBuilder consoleerror = new StringBuilder();
                standardOutput = string.Empty;
                standardError = string.Empty;

                process.StartInfo.FileName = appPath;
                process.StartInfo.Arguments = arugments;
                if (!string.IsNullOrEmpty(workingdirectory)) process.StartInfo.WorkingDirectory = workingdirectory;
                process.StartInfo.CreateNoWindow = isNoWindow;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            if (outputWaitHandle != null)
                                if (!outputWaitHandle.SafeWaitHandle.IsClosed)
                                    outputWaitHandle.Set();
                        }
                        else
                        {
                            consoleoutput.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            if (errorWaitHandle != null)
                                if (!errorWaitHandle.SafeWaitHandle.IsClosed)
                                    errorWaitHandle.Set();
                        }
                        else
                        {
                            consoleerror.AppendLine(e.Data);
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (process.WaitForExit(timeout))
                    {
                        exitCode = process.ExitCode;
                        standardOutput = consoleoutput.ToString();
                        standardError = consoleerror.ToString();
                    }
                    else
                    {
                        process.CancelErrorRead();
                        process.CancelOutputRead();
                        standardOutput = consoleoutput.ToString();
                        standardError = consoleerror.ToString();
                        process.Kill();
                    }
                }
            }

            return exitCode;
        }

        /// <summary>
        /// kill processes by name
        /// </summary>
        /// <param name="processName"></param>
        public static void Kill(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var p in processes)
            {
                try
                {
                    p.Kill();
                    p.WaitForExit();
                }
                catch (Win32Exception e)
                {
                    Console.WriteLine("The process is terminating or could not be terminated. " + e.Message);
                }

                catch (InvalidOperationException e)
                {
                    Console.WriteLine("The process has already exited. " + e.Message);
                }

                catch (Exception e)  // some other exception
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }

            }
        }

        /// <summary>
        /// check if process is running
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static bool IsRunning(string processName)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);
                return processes.Count() != 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
