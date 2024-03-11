using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32.TaskScheduler;

namespace ClassLibrary
{
    public class Task : Database
    {
        protected string table = "tasks";
        private readonly Random _random = new Random();
        private string task_prefix = "MdsDevIt_SD_";
        public enum taskType {one_time, daily, weekly, monthly};
        public int id { get; set; }
        public string task_uuid { get; set; }
        public string task_name { get; set; }
        public string source_path { get; set; }
        public string dest_path { get; set; }
        public int clear_dest { get; set; }
        public int sub_dirs { get; set; }
        public int task_action { get; set; }
        public string task_start_date { get; set; }
        public string task_time { get; set; }
        public string task_next_run { get; set; }
        public string task_end_date { get; set; }
        public string task_type { get; set; }
        public int task_recursive { get; set; }
        public short task_running { get; set; }
        public int task_stopped { get; set; }
        public long task_size { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set;  }

        public string Table
        {
            get { return this.table; }
        }

        public string TaskPrefix
        {
            get { return this.task_prefix; }
        }

        public bool CreateTask()
        {
            using (TaskService ts = new TaskService())
            {
                TaskDefinition td = ts.NewTask();
                string TaskName = this.task_prefix + this.task_uuid;
                td.RegistrationInfo.Author = Utility.author;
                td.RegistrationInfo.Description = this.task_name;
                td.Settings.Priority = System.Diagnostics.ProcessPriorityClass.BelowNormal;
                td.Settings.StartWhenAvailable = false;
                td.Settings.Enabled = true;

                if (this.task_type == "one_time")
                {
                    td.Triggers.Add(
                    new TimeTrigger
                    {
                        StartBoundary = DateTime.Parse(this.task_start_date + " " + this.task_time)
                    });
                }

                if (this.task_type == "daily")
                {
                    td.Triggers.Add(
                    new DailyTrigger
                    {
                        StartBoundary = DateTime.Parse(this.task_start_date + " " + this.task_time),
                        DaysInterval = 1
                    });
                }

                if (this.task_type == "weekly")
                {
                    td.Triggers.Add(
                    new WeeklyTrigger
                    {
                        StartBoundary = DateTime.Today,
                        DaysOfWeek = this.GetDayOfTheWeek(),
                        WeeksInterval = 1
                    });
                }

                if (this.task_type == "monthly")
                {
                    int[] days = { DateTime.Parse(this.task_start_date).Day };
                    td.Triggers.Add(
                    new MonthlyTrigger()
                    {
                        DaysOfMonth = days,
                        MonthsOfYear = MonthsOfTheYear.AllMonths
                    });
                }
                
                td.Actions.Add(new ExecAction(Path.Combine(Utility.app_dir, "CopyDirectories.exe"), null, null));
                ts.RootFolder.RegisterTaskDefinition(@TaskName, td);
                return true;
            }
        } // end of CreateTask method

        public DaysOfTheWeek GetDayOfTheWeek()
        {
            switch (DateTime.Parse(this.task_start_date).DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return DaysOfTheWeek.Sunday;
                case DayOfWeek.Monday:
                    return DaysOfTheWeek.Monday;
                case DayOfWeek.Tuesday:
                    return DaysOfTheWeek.Tuesday;
                case DayOfWeek.Wednesday:
                    return DaysOfTheWeek.Wednesday;
                case DayOfWeek.Thursday:
                    return DaysOfTheWeek.Thursday;
                case DayOfWeek.Friday:
                    return DaysOfTheWeek.Friday;
                case DayOfWeek.Saturday:
                    return DaysOfTheWeek.Saturday;
                default:
                    return DaysOfTheWeek.Monday;
            }
        }

        public bool DeleteTask()
        {
            using (TaskService ts = new TaskService())
            {
                string TaskName = this.task_prefix + this.task_uuid;
                TaskDefinition td = ts.NewTask();
                ts.RootFolder.DeleteTask(@TaskName);
                return true;
            }
        } // end of DeleteTask method

        public Task First(string order = "asc")
        {
            var result = this.GetTasks("SELECT * FROM `" + this.table + "` ORDER BY `id` " + order.ToUpper() + " LIMIT 1");
            return this.Instantiate(result);
        } // end of First method

        public Task FindByUuid(string uuid)
        {
            var result = this.GetTasks("SELECT * FROM `" + this.table + "` WHERE `task_uuid` = '" + uuid + "'");
            return this.Instantiate(result);
        } // end of FindById method

        public Task FindById(int id)
        {
            var result = this.GetTasks("SELECT * FROM `" + this.table + "` WHERE `id` = " + id);
            return this.Instantiate(result);
        } // end of FindById method

        public List<Task> Get()
        {
            return this.GetTasks("SELECT * FROM `" + this.table + "`");
        } // end of Get method

        public Task WhereFirst(string sql)
        {
            var result = this.GetTasks(sql);
            return this.Instantiate(result);
        } // end of WhereFirst method

        public List<Task> Where(string sql)
        {
            return this.GetTasks(sql);
        } // end of Where method

        public bool Exists()
        {
            return Convert.ToBoolean(this.id);
        } // end of Exists method

        public bool IsRecursive()
        {
            return Convert.ToBoolean(this.task_recursive);
        } // end of IsRecursive method

        public bool IsRunning()
        {
            return Convert.ToBoolean(this.task_running);
        } // end of IsRunning method

        public bool UpdateTaskRunning(int value)
        {
            this.task_stopped = value;

            var sql = "UPDATE `" + this.table + "` SET `task_running` = " + this.task_running + ", `updated_at` = '" +
                DateTime.Now.ToString() + "' WHERE `id` = " + this.id;

            try
            {
                this.ExecuteQuery(sql);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of UpdateTaskRunning method

        public bool IsStopped()
        {
            return Convert.ToBoolean(this.task_stopped);
        } // end of IsStopped method

        public bool UpdateTaskStopped(int value)
        {
            this.task_stopped = value;

            var sql = "UPDATE `" + this.table + "` SET `task_stopped` = " + this.task_stopped + ", `updated_at` = '" + 
                DateTime.Now.ToString() + "' WHERE `id` = " + this.id;

            try
            {
                this.ExecuteQuery(sql);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of UpdateTaskStopped method

        public bool HasEndDate()
        {
            return !string.IsNullOrEmpty(this.task_end_date);
        } // end of HasEndDate method

        public bool IsTodayEndDate()
        {
            return DateTime.Compare(
                DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm")), 
                DateTime.Parse(this.task_end_date + " " + this.task_time)) == 0 ? true : false;
        } // end of IsTodayEndDate method

        public void StopTask()
        {
            this.UpdateTaskStopped(1);
            this.DeleteTask();
        } // end of StopTask method

        public bool UpdateTaskNextRun(string value)
        {
            this.task_next_run = value;

            var sql = "UPDATE `" + this.table + "` SET `task_next_run` = '" + this.task_next_run + "', `updated_at` = '" +
                DateTime.Now.ToString() + "' WHERE `id` = " + this.id;

            try
            {
                this.ExecuteQuery(sql);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of UpdateTaskNextRun method

        private bool WithSubDirs()
        {
            return Convert.ToBoolean(this.sub_dirs);
        } // end of WithSubDirs method

        private bool ClearDest()
        {
            return Convert.ToBoolean(this.clear_dest);
        } // end of ClearDest method

        public bool Save()
        {
            return Convert.ToBoolean(this.id) ? this.Update() : this.Create();
        } // end of Save method

        private bool Create()
        {
            this.created_at = DateTime.Now.ToString();
            this.TrimValues();
            var sql = "INSERT INTO `" + this.table + "` (" +
                "`task_uuid`, " +
                "`task_name`, " +
                "`source_path`, " +
                "`dest_path`, " +
                "`clear_dest`, " +
                "`sub_dirs`, " +
                "`task_action`, " +
                "`task_start_date`, " +
                "`task_time`, " +
                "`task_next_run`, " +
                "`task_end_date`, " +
                "`task_type`, " +
                "`task_recursive`, " +
                "`task_running`, " +
                "`created_at`) " +
                "VALUES (" +
                "'" + this.task_uuid + "', " +
                "'" + this.task_name + "', " +
                "'" + this.source_path + "', " +
                "'" + this.dest_path + "', " +
                this.clear_dest + ", " +
                this.sub_dirs + ", " +
                this.task_action + ", " +
                "'" + this.task_start_date + "', " +
                "'" + this.task_time + "', " +
                "'" + this.task_next_run + "', " +
                "'" + this.task_end_date + "', " +
                "'" + this.task_type + "', " +
                this.task_recursive + ", " +
                this.task_running + ", " +
                "'" + this.created_at + "')";
            
            try
            {
                this.ExecuteQuery(sql);
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        } // end of Create method

        private bool Update()
        {
            this.updated_at = this.TrimValue(DateTime.Now.ToString());
            this.TrimValues();
            var sql = "UPDATE `" + this.table + "` SET " +
                "`task_name` = '" + this.task_name + "', " +
                "`source_path` = '" + this.source_path + "', " +
                "`dest_path` = '" + this.dest_path + "', " +
                "`clear_dest` = " + this.clear_dest + ", " +
                "`sub_dirs` = " + this.sub_dirs + ", " +
                "`task_action` = " + this.task_action + ", " +
                "`task_start_date` = '" + this.task_start_date + "', " +
                "`task_time` = '" + this.task_time + "', " +
                "`task_type` = '" + this.task_type + "', " +
                "`task_next_run` = '" + this.task_next_run + "', " +
                "`task_end_date` = '" + this.task_end_date + "', " +
                "`task_recursive` = " + this.task_recursive + ", " +
                "`task_running` = " + this.task_running + ", " +
                "`created_at` = '" + this.created_at + "', " +
                "`updated_at` = '" + this.updated_at + "' " +
                "WHERE `id` = " + this.id;

            try
            {
                this.ExecuteQuery(sql);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of Update method

        public bool Delete()
        {
            try
            {
                this.ExecuteQuery("DELETE FROM `" + this.table + "` WHERE `id` = " + this.id);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of Delete method

        protected Task Instantiate(List<Task> result)
        {
            if (result.Count > 0)
            {
                this.id = result[0].id;
                this.task_uuid = result[0].task_uuid;
                this.task_name = result[0].task_name;
                this.source_path = result[0].source_path;
                this.dest_path = result[0].dest_path;
                this.sub_dirs = result[0].sub_dirs;
                this.task_action = result[0].task_action;
                this.task_start_date = result[0].task_start_date;
                this.task_time = result[0].task_time;
                this.task_next_run = result[0].task_next_run;
                this.task_end_date = result[0].task_end_date;
                this.task_type = result[0].task_type;
                this.task_recursive = result[0].task_recursive;
                this.task_running = result[0].task_running;
                this.task_stopped = result[0].task_stopped;
                this.created_at = result[0].created_at;
                this.updated_at = result[0].updated_at;
            }

            return this;
        } // end of Instantiate method

        private void TrimValues()
        {
            this.task_uuid = this.TrimValue(this.task_uuid);
            this.task_name = this.TrimValue(this.task_name);
            this.source_path = this.TrimValue(this.source_path);
            this.dest_path = this.TrimValue(this.dest_path);
            this.task_start_date = this.TrimValue(this.task_start_date);
            this.task_time = this.TrimValue(this.task_time);
            this.task_next_run = this.TrimValue(this.task_next_run);
            this.task_end_date = this.TrimValue(this.task_end_date);
            this.task_type = this.TrimValue(this.task_type);
            this.created_at = this.TrimValue(this.created_at);
        } // end of TrimValues method

        /*
         * ------------------------------
         *  All processing task methods
         * ------------------------------
         */
        public void ProcessTask()
        {
            if (this.task_action == 0)
            {
                if (this.ClearDest())
                {
                    this.DirectoryClear(this.dest_path, this.WithSubDirs());
                }
                else
                {
                    this.DirectoryClear(this.source_path, this.WithSubDirs());
                }
            }

            if (this.task_action == 1)
            {
                this.DirectoryCopy(this.source_path, this.dest_path, this.WithSubDirs());
            }

            if (this.task_action == 2)
            {
                this.DirectoryCopy(this.source_path, this.dest_path, this.WithSubDirs());
                this.DirectoryOneWaySync(this.dest_path, this.source_path, this.WithSubDirs());
            }

            if (this.task_action == 3)
            {
                this.DirectoryCopy(this.source_path, this.dest_path, this.WithSubDirs());
                this.DirectoryBothWaysSync(this.dest_path, this.source_path, this.WithSubDirs());
            }
        } // end of ProcessTask method

        public void DirectorySize(string sourceDirName, bool subDirsSize)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                this.task_size = 0;
            }

            // Get the subdirectories for the specified directory.
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Get the files in the directory and sum their size.
            FileInfo[] files = dir.GetFiles();
            if (files.Length > 0)
            {
                foreach (FileInfo file in files)
                {
                    this.task_size += file.Length;
                }
            }

            // If with subdirectories, calculate their size also.
            if (subDirsSize)
            {
                if (dirs.Length > 0)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        this.DirectorySize(subdir.FullName, subDirsSize);
                    }
                }
            }
        } // end of DirectorySize method

        private void DirectoryClear(string sourceDirName, bool clearSubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName) { Attributes = FileAttributes.Normal };

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Dosarul sursă nu există sau nu a fost găsit: "
                    + sourceDirName);
            }

            // Get the files in the directory and then delete them.
            FileInfo[] files = dir.GetFiles();
            if (files.Length > 0)
            {
                foreach (FileInfo file in files)
                {
                    try
                    {
                        this.ChangeFileSecurity(file);
                        File.Delete(file.FullName);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }

            // Get the subdirectories for the specified directory.
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Delete all the subdirectories from the specified directory.
            if (clearSubDirs)
            {
                if (dirs.Length > 0)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        this.ChangeDirectorySecurity(subdir);
                        try
                        {
                            Directory.Delete(subdir.FullName, true);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            this.ChangeAllDirectoryFileAttributes(subdir);
                            Directory.Delete(subdir.FullName, true);
                        }
                    }
                }
            }
        } // end of DirectoryClear method

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Dosarul sursă nu există sau nu a fost găsit: "
                    + sourceDirName);
            }

            // Get the subdirectories for the specified directory.
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);
            this.ChangeDirectorySecurity(new DirectoryInfo(destDirName));

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            if (files.Length > 0)
            {
                foreach (FileInfo file in files)
                {
                    this.FileCopyTo(Path.Combine(destDirName, file.Name), file);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs && dirs.Length > 0)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    this.DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        } // end of DirectoryCopy method

        private void DirectoryOneWaySync(string sourceDirName, string destDirName, bool syncSubDirs)
        {
            // If the destination directory doesn't exist, delete it.       
            if (!Directory.Exists(destDirName))
            {
                this.ChangeDirectorySecurity(new DirectoryInfo(sourceDirName));
                try
                {
                    Directory.Delete(sourceDirName, true);
                }
                catch (UnauthorizedAccessException)
                {
                    this.ChangeAllDirectoryFileAttributes(new DirectoryInfo(sourceDirName));
                    Directory.Delete(sourceDirName, true);
                }
            }

            // Get the files in the directory and delete them from sourceDirName if they don't exists in destDirName.
            if (Directory.Exists(destDirName))
            {
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                this.ChangeDirectorySecurity(dir);

                FileInfo[] files = dir.GetFiles();
                if (files.Length > 0)
                {
                    foreach (FileInfo file in files)
                    {
                        var tempPath = Path.Combine(destDirName, file.Name);
                        
                        if (!File.Exists(tempPath))
                        {
                            try
                            {
                                this.ChangeFileSecurity(new FileInfo(Path.Combine(sourceDirName, file.Name)));
                                File.Delete(Path.Combine(sourceDirName, file.Name));
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }
                }

                DirectoryInfo[] dirs = dir.GetDirectories();

                // If sync subdirectories, sync them and their contents.
                if (syncSubDirs && dirs.Length > 0)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string tempPath = Path.Combine(destDirName, subdir.Name);
                        this.DirectoryOneWaySync(subdir.FullName, tempPath, syncSubDirs);
                    }
                }
            }
        } // end of DirectoryOneWaySync method

        private void DirectoryBothWaysSync(string sourceDirName, string destDirName, bool syncSubDirs)
        {
            // If the destination directory doesn't exist, create it.       
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                this.ChangeDirectorySecurity(new DirectoryInfo(destDirName));
            }

            // Get the files in the directory and copy them to destination directory if they don't exists.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            FileInfo[] files = dir.GetFiles();
            if (files.Length > 0)
            {
                foreach (FileInfo file in files)
                {
                    var tempPath = Path.Combine(destDirName, file.Name);

                    this.FileCopyTo(tempPath, file);
                }
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If sync subdirectories, sync them and their contents.
            if (syncSubDirs && dirs.Length > 0)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    this.DirectoryBothWaysSync(subdir.FullName, tempPath, syncSubDirs);
                }
            }
        } // end of DirectoryBothWaysSync method

        private void FileCopyTo(string path, FileInfo file)
        {
            if (File.Exists(path))
            {
                FileInfo dest_file = new FileInfo(path);
                if (file.Length != dest_file.Length)
                {
                    try
                    {
                        file.CopyTo(path, true);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            else
            {
                try
                {
                    file.CopyTo(path, false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            this.ChangeFileSecurity(new FileInfo(path));
        } // end of FileCopyTo method

        private void ChangeFileSecurity(FileInfo file)
        {
            try
            {
                FileSecurity fSec = file.GetAccessControl();
                fSec.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    FileSystemRights.FullControl, AccessControlType.Allow));
                file.Attributes = FileAttributes.Normal;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of ChangeFileSecurity method

        private void ChangeDirectorySecurity(DirectoryInfo dir)
        {
            try
            {
                DirectorySecurity dSec = dir.GetAccessControl();
                dSec.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    FileSystemRights.FullControl, AccessControlType.Allow));
                dir.Attributes = FileAttributes.Normal;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of ChangeDirectorySecurity method

        private void ChangeAllDirectoryFileAttributes(DirectoryInfo dir)
        {
            foreach (var info in dir.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }
        } // end of ChangeAllDirectoryFileAttributes method

        private string CheckUuidExists(string uuid, int length, bool lowerCase)
        {
            var sql = "SELECT `task_uuid` FROM `" + this.table + "` WHERE `task_uuid` = '" + this.TrimValue(uuid) + "' LIMIT 1";
            var results = this.GetTasks(sql).ToArray();
            if (results.Length > 0)
            {
                for (int i = 0; i < results.Length; i++)
                {
                    if (string.Compare(uuid, results[i].task_uuid, false) == 0)
                    {
                        return this.GenerateUuid(length, lowerCase);
                    }
                }
                return uuid;
            }
            return uuid;
        } // end of CheckUuidExists method

        public string GenerateUuid(int length, bool lowerCase)
        {
            return this.CheckUuidExists(this.RandomString(length, lowerCase), length, lowerCase);
        }

        private string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;
            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        } // end of RandomString method
    }
}
