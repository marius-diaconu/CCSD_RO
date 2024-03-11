using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary;

namespace Main.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private Database db;
        private List<Tasks> tasks;
        private List<Logs> logs;
        private List<Errors> errors;

        public ShellViewModel()
        {
            this.InitDb();
            this.LoadAboutView();
        }

        public void InitDb()
        {
            db = new Database();
            if (db.is_open)
            {
                db.CheckIfTablesExists();
            }
            else
            {
                MessageBox.Show("Conexiunea la baza de date a eșuat!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadCreateTaskView()
        {
            ActivateItem(new CreateTaskViewModel());
        }

        public void LoadTasksView()
        {
            ActivateItem(new TasksListViewModel());
        }

        public void CloseAllTasks()
        {
            var response = MessageBox.Show("Această acțiune va duce la închiderea tuturor task-urilor care rulează în prezent. " +
                "Sunteți sigur că doriți acest lucru? Apasă [ OK ] pentru a continua sau [ Cancel ] " +
                "pentru anulare.", "Informativ",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (response == DialogResult.OK)
            {
                if (Uninstaller.isAppRunning(Utility.sd_process))
                {
                    Uninstaller.killRunningApp(Utility.sd_process);

                    MessageBox.Show("Toate task-urile au fost închise cu succes", "Succes",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Nu aveți task-uri care rulează în prezent", "Informativ",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void DeleteAll()
        {
            var response = MessageBox.Show("Această acțiune va duce la ștergerea tuturor înregistrărilor. " +
                "Sunteți sigur că doriți acest lucru? Apasă [ OK ] pentru a continua sau [ Cancel ] " +
                "pentru anulare.", "Avertisment",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (response == DialogResult.OK)
            {
                Tasks getTask = new Tasks();
                tasks = getTask.Get();

                Logs getLogs = new Logs();
                logs = getLogs.Get();

                Errors getErrors = new Errors();
                errors = getErrors.Get();

                if (tasks.Count > 0 || logs.Count > 0 || errors.Count > 0)
                {
                    if (tasks.Count > 0)
                    {
                        foreach (Tasks task in tasks)
                        {
                            task.DeleteTask();
                            task.Delete();
                        }
                    }

                    if (logs.Count > 0)
                    {
                        foreach (Logs log in logs)
                        {
                            if (File.Exists(log.filepath)) File.Delete(log.filepath);
                            log.Delete();
                        }
                    }

                    if (errors.Count > 0)
                    {
                        foreach (Errors error in errors)
                        {
                            if (File.Exists(error.filepath)) File.Delete(error.filepath);
                            error.Delete();
                        }
                    }

                    MessageBox.Show("Toate înregistrările au fost șterse cu succes!", "Succes",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ActivateItem(new AboutViewModel());
                }
                else
                {
                    MessageBox.Show("Nu aveți înregistrări disponibile pentru ștergere", "Informativ",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void LoadLogView()
        {
            ActivateItem(new LogViewModel());
        }

        public void LoadErrorView()
        {
            ActivateItem(new ErrorViewModel());
        }

        public void UninstallAction()
        {
            var response = MessageBox.Show("Această acțiune va duce la ștergerea tuturor înregistrărilor și dezinstalarea programului. " +
                "Sunteți sigur că doriți acest lucru? Apasă [ OK ] pentru a continua sau [ Cancel ] " +
                "pentru anulare.", "Avertisment",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (response == DialogResult.OK)
            {
                if (Uninstaller.isAppRunning(Utility.sd_process))
                {
                    Uninstaller.killRunningApp(Utility.sd_process);
                }

                Tasks getTask = new Tasks();
                tasks = getTask.Get();

                Logs getLog = new Logs();
                logs = getLog.Get();

                Errors getError = new Errors();
                errors = getError.Get();

                if (tasks.Count > 0 || logs.Count > 0 || errors.Count > 0)
                {
                    if (tasks.Count > 0)
                    {
                        foreach (Tasks task in tasks)
                        {
                            task.DeleteTask();
                            task.Delete();
                        }
                    }

                    if (logs.Count > 0)
                    {
                        foreach (Logs log in logs)
                        {
                            if (File.Exists(log.filepath)) File.Delete(log.filepath);
                            log.Delete();
                        }
                    }

                    if (errors.Count > 0)
                    {
                        foreach (Errors error in errors)
                        {
                            if (File.Exists(error.filepath)) File.Delete(error.filepath);
                            error.Delete();
                        }
                    }

                    if (db.is_open) db.CloseConn();
                }
                else
                {
                    if (db.is_open) db.CloseConn();
                }

                Uninstaller.uninstallApp(Utility.product_code, Utility.ma_process);
            }
        }

        public void LoadAboutView()
        {
            ActivateItem(new AboutViewModel());
        }
    }
}
