using System;
using System.Reflection;
using Microsoft.Win32.TaskScheduler;
using ClassLibrary;

namespace Service
{
    public class Program
    {
        private static string log_filepath;
        private static string error_filepath;
        public static void Main()
        {
            RunningTask rt = GetCurrentRunningTask();

            if (rt != null)
            {
                Database db = new Database();
                if (db.is_open)
                {
                    db.CheckIfTablesExists(); 
                    db.CheckTablesOverload();

                    log_filepath = Utility.CreateFileIfNotExists("log");
                    error_filepath = Utility.CreateFileIfNotExists("error");

                    Utility.DeleteOldLogsAndErrors();
                }
                
                if (db.is_open)
                {
                    Tasks task = new Tasks();
                    task = task.First("SELECT * FROM `" + task.table + "` WHERE `task_uuid` = '" +
                        task.TrimValue(rt.Name.Replace(task.TaskPrefix, "")) + "' AND `task_stopped` = 0 LIMIT 1");

                    if (task.Exists())
                    {
                        if ((task.HasEndDate() && task.IsTodayEndDate()) || task.IsOneTimeTask())
                        {
                            task.UpdateTaskLastRun(rt.LastRunTime);
                            task.StopTask();

                            if (db.is_open) db.CloseConn();

                            Utility.InsertLog(log_filepath, "Task: " + task.task_name + " started.");

                            task.ProcessTask(error_filepath);

                            Utility.InsertLog(log_filepath, "Task: " + task.task_name + " ended.");
                        }
                        else
                        {
                            task.UpdateTaskLastRun(rt.LastRunTime);
                            task.UpdateTaskNextRun(rt.NextRunTime);

                            if (db.is_open) db.CloseConn();

                            Utility.InsertLog(log_filepath, "Task: " + task.task_name + " started.");

                            task.ProcessTask(error_filepath);

                            Utility.InsertLog(log_filepath, "Task: " + task.task_name + " ended.");
                        }
                    }
                }
                
            }
            Environment.Exit(0);
        }

        private static RunningTask GetCurrentRunningTask()
        {
            DateTime now = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            foreach (RunningTask rt in TaskService.Instance.GetRunningTasks(false))
            {
                if (rt != null && rt.CurrentAction == Assembly.GetEntryAssembly().Location &&
                    DateTime.Compare(now, DateTime.Parse(rt.LastRunTime.ToString("yyyy-MM-dd HH:mm"))) == 0)
                {
                    return rt;
                }
            }

            return null;
        } // end of RunningTask method
    }
}
