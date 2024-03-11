using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace ClassLibrary
{
    public static class Uninstaller
    {
        private static Process process;
        private static ProcessStartInfo startInfo;

        public static bool isAppRunning(string app_process)
        {
            Process[] runningProcesses = Process.GetProcesses();

            foreach (Process myProcess in runningProcesses)
            {
                if (myProcess.ProcessName.Contains(app_process))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool killRunningApp(string app_process)
        {
            Process[] runningProcesses = Process.GetProcesses();

            foreach (Process myProcess in runningProcesses)
            {
                if (myProcess.ProcessName.Contains(app_process))
                {
                    myProcess.Kill();
                }
            }

            return true;
        }

        public static void uninstallApp(string product_code, string app_process)
        {
            string uninstallString = "msiexec.exe /x {" + product_code + "}";

            if (!string.IsNullOrEmpty(product_code))
            {
                batFileCreateStartProcess(uninstallString, app_process);
            }
        }

        private static void batFileCreateStartProcess(string uninstallString, string app_process)
        {
            string tempPathfile = Path.GetTempPath() + app_process + ".bat";

            if (!File.Exists(@tempPathfile))
            {
                using (FileStream createfile = File.Create(@tempPathfile))
                {
                    createfile.Close();
                }
            }

            using (StreamWriter writefile = new StreamWriter(@tempPathfile))
            {
                writefile.WriteLine(@"start " + uninstallString);
            }

            process = new Process();
            startInfo = new ProcessStartInfo();

            startInfo.FileName = tempPathfile;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            File.Delete(tempPathfile);
            isAppRunning(app_process);
            killRunningApp(app_process);
        }
    }
}
