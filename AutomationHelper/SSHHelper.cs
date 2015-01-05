using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using Renci.SshNet;

namespace AutomationHelper
{
    public class SSHHelper
    {
        public class MyAsyncInfo
        {
            public Byte[] ByteArray { get; set; }
            public ShellStream Stream { get; set; }

            public MyAsyncInfo(Byte[] array, ShellStream stream)
            {
                ByteArray = array;
                Stream = stream;
            }
        }

        public SshClient SSHClient { get; set; }
        public ShellStream ShellStream { get; set; }

        public SSHHelper(string server, string username, string password)
        {
            try
            {
                SSHClient = new SshClient(server, username, password);
                SSHClient.Connect();

                ShellStream = SSHClient.CreateShellStream("xterm", 80, 24, 800, 600, 1024);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        ~SSHHelper()
        {
            if (SSHClient != null)
            {
                SSHClient.Disconnect();
            }
        }

        public int RunCommand(string command, out string stdout, out string error)
        {
            stdout = string.Empty;
            error = string.Empty;
            int result = -1;
            try
            {
                if (!SSHClient.IsConnected)
                    SSHClient.Connect();

                SshCommand sshcmd = SSHClient.RunCommand(command);
                stdout += sshcmd.Result;
                error += sshcmd.Error;
                result = sshcmd.ExitStatus;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public int SendCommand(string command, string expected, int timeout, out string stdout)
        {
            int result = -1;
            stdout = string.Empty;
            try
            {
                if (!SSHClient.IsConnected)
                    SSHClient.Connect();
                
                var reader = new StreamReader(ShellStream);
                var writer = new StreamWriter(ShellStream);

                writer.AutoFlush = true;
                writer.WriteLine(command);

                while(ShellStream.Length == 0)
                {
                    System.Threading.Thread.Sleep(500);
                }

                stdout = ShellStream.Expect(new Regex(expected), TimeSpan.FromSeconds(timeout));
                if (string.IsNullOrEmpty(stdout)) stdout = reader.ReadToEnd();

                result = 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

    }
}
