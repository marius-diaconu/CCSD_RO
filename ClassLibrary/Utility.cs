using System;
using System.Reflection;
using System.IO;
using System.Security.AccessControl;

namespace ClassLibrary
{
    public static class Utility
    {
        public static string system_username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        public static string product_code = "5733AC37-62D1-4A48-BDFA-7D2CD0EF35B1";
        public static string author = "MDS DEV IT";
        public static string description = "Program pentru curățarea, copierea și sincronizarea directoarelor";
        public static string assembly_name = "CCSD";
        public static string prog_name = "CCSD RO";
        public static string prog_version = "1.0.1";

        public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string root_dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString();
        public static string app_dir = Path.Combine(Path.Combine(appData, author), prog_name);
        public static string ma_process = assembly_name.ToLower() + "-main";
        public static string sd_process = assembly_name.ToLower() + "-service";
        public static string sd_exe = sd_process + ".exe";
        public static int files = 30;

        public static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        } // end of GetTimestamp method

        public static string CreateFileIfNotExists(string type)
        {
            if (type == "log")
            {
                Logs log = new Logs();
                log = log.FindByDate(DateTime.Now);

                if (log.Exists())
                {
                    if (File.Exists(@log.filepath))
                    {
                        return log.filepath;
                    }
                    else
                    {
                        FileStream fs = File.Create(@log.filepath);
                        fs.Close();

                        return log.filepath;
                    }
                }
                else
                {
                    log.filename = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                    log.filepath = Path.Combine(log.log_dir, log.filename);
                    log.Save();

                    FileStream fs = File.Create(@log.filepath);
                    fs.Close();

                    return log.filepath;
                }
            }

            if(type == "error")
            {
                Errors error = new Errors();
                error = error.FindByDate(DateTime.Now);

                if (error.Exists())
                {
                    if (File.Exists(@error.filepath))
                    {
                        return error.filepath;
                    }
                    else
                    {
                        FileStream fs = File.Create(@error.filepath);
                        fs.Close();

                        return error.filepath;
                    }
                }
                else
                {
                    error.filename = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                    error.filepath = Path.Combine(error.error_dir, error.filename);
                    error.Save();

                    FileStream fs = File.Create(@error.filepath);
                    fs.Close();

                    return error.filepath;
                }
            }

            return null;
        } // end of CreateFileIfNotExists method

        public static void DeleteOldLogsAndErrors()
        {
            Logs log = new Logs();
            var logs = log.Get("desc");
            if (logs.Count > files)
            {
                for (int i = 0; i < logs.Count; i++)
                {
                    if(i > files)
                    {
                        if (logs[i].Delete()) File.Delete(@logs[i].filepath);
                    }
                }
            }

            Errors error = new Errors();
            var errors = error.Get("desc");
            if (errors.Count > files)
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    if (i > files)
                    {
                        if (errors[i].Delete()) File.Delete(@errors[i].filepath);
                    }
                }
            }
        } // end of DeleteOldLogsAndErrors method

        public static void InsertLog(string filepath, string msg)
        {
            WriteLine(filepath, msg);
        } // end of InsertLog method

        public static void InsertLog(string msg)
        {
            Logs log = new Logs();
            log = log.FindByDate(DateTime.Now);

            if (log.Exists())
            {
                if (File.Exists(@log.filepath))
                {
                    WriteLine(log.filepath, msg);
                }
                else
                {
                    FileStream fs = File.Create(@log.filepath);
                    fs.Close();

                    WriteLine(log.filepath, msg);
                }
            }
            else
            {
                var filepath = CreateFileIfNotExists("log");

                WriteLine(filepath, msg);
            }
        } // end of InsertLog method

        public static void InsertError(string filepath, string msg)
        {
            WriteLine(filepath, msg);
        } // end of InsertError method

        public static void InsertError(string msg)
        {
            Errors error = new Errors();
            error = error.FindByDate(DateTime.Now);

            if (error.Exists())
            {
                if (File.Exists(@error.filepath))
                {
                    WriteLine(error.filepath, msg);
                }
                else
                {
                    FileStream fs = File.Create(@error.filepath);
                    fs.Close();

                    WriteLine(error.filepath, msg);
                }
            }
            else
            {
                var filepath = CreateFileIfNotExists("error");

                WriteLine(filepath, msg);
            }
        } // end of InsertError method

        private static void WriteLine(string filepath, string msg)
        {
            using (StreamWriter file = new StreamWriter(@filepath, true))
            {
                file.WriteLine(msg + " @ " + GetTimestamp());
                file.Close();
            }
        } // end of WriteLine method

        public static void ChangeDirectorySecurity(string dir_path)
        {
            DirectoryInfo dir = new DirectoryInfo(dir_path);
            try
            {
                DirectorySecurity dSec = dir.GetAccessControl();
                dSec.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
                dir.SetAccessControl(dSec);
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of ChangeDirectorySecurity method
    }
}
