using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ClassLibrary;

namespace Main.ViewModels
{
    public class CreateTaskViewModel : Conductor<object>
    {
        private Tasks task;

        private string _taskName;
        private bool _directoryClearCheckbox;
        private bool _directoryCopyCheckbox;
        private bool _oneWaySyncCheckbox;
        private bool _bothWaysSyncCheckbox;
        private bool _withSubDir;
        private string _sourceDirectoryPath;
        private string _destinationDirectoryPath;
        private bool _oneTimeTask;
        private bool _dailyTask;
        private bool _weeklyTask;
        private bool _monthlyTask;
        private short _taskInterval = 1;
        private DateTime _taskStartDate = DateTime.Now;
        private DateTime _taskEndDate = DateTime.Now.AddYears(10);
        private string _taskTime = DateTime.Now.ToString("HH:mm");

        public string TaskName
        {
            get { return _taskName; }
            set
            {
                _taskName = value;
                NotifyOfPropertyChange(() => TaskName);
            }
        }

        public bool DirectoryClearCheckbox
        {
            get { return _directoryClearCheckbox; }
            set
            {
                _directoryClearCheckbox = value;
                NotifyOfPropertyChange(() => DirectoryClearCheckbox);
            }
        }

        public bool DirectoryCopyCheckbox
        {
            get { return _directoryCopyCheckbox; }
            set
            {
                _directoryCopyCheckbox = value;
                NotifyOfPropertyChange(() => DirectoryCopyCheckbox);
            }
        }

        public bool OneWaySyncCheckbox
        {
            get { return _oneWaySyncCheckbox; }
            set
            {
                _oneWaySyncCheckbox = value;
                NotifyOfPropertyChange(() => OneWaySyncCheckbox);
            }
        }

        public bool BothWaysSyncCheckbox
        {
            get { return _bothWaysSyncCheckbox; }
            set
            {
                _bothWaysSyncCheckbox = value;
                NotifyOfPropertyChange(() => BothWaysSyncCheckbox);
            }
        }

        public bool WithSubDir
        {
            get { return _withSubDir; }
            set
            {
                _withSubDir = value;
                NotifyOfPropertyChange(() => WithSubDir);
            }
        }

        public string SourceDirectoryPath
        {
            get { return _sourceDirectoryPath; }
            set
            {
                _sourceDirectoryPath = value;
                NotifyOfPropertyChange(() => SourceDirectoryPath);
            }
        }

        public string DestinationDirectoryPath
        {
            get { return _destinationDirectoryPath; }
            set
            {
                _destinationDirectoryPath = value;
                NotifyOfPropertyChange(() => DestinationDirectoryPath);
            }
        }

        public bool OneTimeTask
        {
            get { return _oneTimeTask; }
            set
            {
                _oneTimeTask = value;
                NotifyOfPropertyChange(() => OneTimeTask);
            }
        }

        public bool DailyTask
        {
            get { return _dailyTask; }
            set
            {
                _dailyTask = value;
                NotifyOfPropertyChange(() => DailyTask);
            }
        }

        public bool WeeklyTask
        {
            get { return _weeklyTask; }
            set
            {
                _weeklyTask = value;
                NotifyOfPropertyChange(() => WeeklyTask);
            }
        }

        public bool MonthlyTask
        {
            get { return _monthlyTask; }
            set
            {
                _monthlyTask = value;
                NotifyOfPropertyChange(() => MonthlyTask);
            }
        }

        public short TaskInterval
        {
            get { return _taskInterval; }
            set
            {
                _taskInterval = value;
                NotifyOfPropertyChange(() => TaskInterval);
            }
        }

        public DateTime TaskStartDate
        {
            get { return _taskStartDate; }
            set
            {
                _taskStartDate = value;
                NotifyOfPropertyChange(() => TaskStartDate);
            }
        }

        public DateTime TaskEndDate
        {
            get { return _taskEndDate; }
            set
            {
                _taskEndDate = value;
                NotifyOfPropertyChange(() => TaskEndDate);
            }
        }

        public string TaskTime
        {
            get { return _taskTime; }
            set
            {
                _taskTime = value;
                NotifyOfPropertyChange(() => TaskTime);
            }
        }

        public void SelectSourceDirectory()
        {
            FolderBrowserDialog source_dir = new FolderBrowserDialog();
            source_dir.ShowDialog();
            SourceDirectoryPath = source_dir.SelectedPath;
        }

        public void SelectDestinationDirectory()
        {
            FolderBrowserDialog dest_dir = new FolderBrowserDialog();
            dest_dir.ShowDialog();
            DestinationDirectoryPath = dest_dir.SelectedPath;
        }

        public void SaveTaskButton()
        {
            task = new Tasks();
            task.task_uuid = task.GenerateUuid(32, true);

            if (string.IsNullOrEmpty(TaskName))
            {
                MessageBox.Show("Vă rugăm introduceți un nume descriptiv pentru acest task, " +
                    "înainte de a apăsa butonul [ Salvează Task ]", "Avertisment",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                task.task_name = TaskName;
            }

            if (!DirectoryClearCheckbox && !DirectoryCopyCheckbox && !OneWaySyncCheckbox && !BothWaysSyncCheckbox)
            {
                MessageBox.Show("Este necesară selectarea uneia dintre opțiunile listate în Tip Task: curățare, " +
                    "copiere, sincronizare într-o singură direcție, sincronizare în ambele direcții, " +
                    "înainte de a apăsa butonul [ Salvează Task ]", "Avertisment",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                if (DirectoryClearCheckbox) task.task_action = 0;
                if (DirectoryCopyCheckbox) task.task_action = 1;
                if (OneWaySyncCheckbox) task.task_action = 2;
                if (BothWaysSyncCheckbox) task.task_action = 3;
            }

            if (string.IsNullOrEmpty(SourceDirectoryPath))
            {
                MessageBox.Show("Este necesară selectarea dosarului sursă, înainte de a apăsa butonul [ Salvează Task ]",
                    "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DirectoryCopyCheckbox || OneWaySyncCheckbox || BothWaysSyncCheckbox)
            {
                if (string.IsNullOrEmpty(DestinationDirectoryPath))
                {
                    MessageBox.Show("Este necesară selectarea dosarului de destinație, înainte de a apăsa butonul " +
                        "[ Salvează Task ]", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            task.source_path = SourceDirectoryPath;
            task.dest_path = DestinationDirectoryPath;
            task.sub_dirs = WithSubDir ? 1 : 0;

            if (!OneTimeTask && !DailyTask && !WeeklyTask && !MonthlyTask)
            {
                MessageBox.Show("Este necesară selectarea uneia dintre opțiunile listate în Repetare: o dată, zilnic, " +
                    "săptămânal, lunar, înainte de a apăsa butonul [ Salvează Task ]", "Avertisment",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                if (OneTimeTask) task.task_type = Tasks.Type.ONE_TIME.ToString();
                if (DailyTask) task.task_type = Tasks.Type.DAILY.ToString();
                if (WeeklyTask) task.task_type = Tasks.Type.WEEKLY.ToString();
                if (MonthlyTask) task.task_type = Tasks.Type.MONTHLY.ToString();
            }

            if (DailyTask || WeeklyTask || MonthlyTask)
            {
                task.task_repetable = 1;
                task.task_interval = 1;

                if (DailyTask || WeeklyTask)
                {
                    Regex regex = new Regex(@"[0-9]*");
                    if (regex.IsMatch(TaskInterval.ToString()) && TaskInterval >= 1)
                    {
                        task.task_interval = TaskInterval;
                    }
                    else
                    {
                        MessageBox.Show("Intervalul trebuie să fie numeric și mai mare sau egal cu 1!", "Avertisment", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            else
            {
                task.task_repetable = 0;
                task.task_interval = 0;
            }

            if (string.IsNullOrEmpty(TaskStartDate.ToString()))
            {
                MessageBox.Show("Este necesară setarea datei de început, înainte de a apăsa butonul " +
                    "[ Salvează Task ]", "Avertisment",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                task.task_start_date = TaskStartDate.ToString("yyyy-MM-dd");
            }

            if (!string.IsNullOrEmpty(TaskEndDate.ToString()))
            {
                task.task_end_date = TaskEndDate.ToString("yyyy-MM-dd");
            }

            if (string.IsNullOrEmpty(TaskTime))
            {
                MessageBox.Show("Este necesară setarea timpului, înainte de a apăsa butonul " +
                    "[ Salvează Task ]", "Avertisment",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                Regex regex = new Regex(@"^(\d{2}:\d{2})$");
                if (regex.IsMatch(TaskTime))
                {
                    string[] arr = TaskTime.Split(':');
                    int hours, minutes;
                    Int32.TryParse(arr[0], out hours);
                    Int32.TryParse(arr[1], out minutes);

                    if (Convert.ToBoolean(hours) && hours > 23)
                    {
                        MessageBox.Show("Ora nu poate fi mai mare de 23!", "Avertisment",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (Convert.ToBoolean(minutes) && minutes > 59)
                    {
                        MessageBox.Show("Minutele nu pot fi mai mari de 59!", "Avertisment",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (task.CheckTaskTimeExists(TaskTime))
                    {
                        MessageBox.Show("Acest timp este deja setat unui task existent. " +
                            "Vă rugăm alegeți altul.", "Avertisment",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        task.task_time = TaskTime;
                    }
                }
                else
                {
                    MessageBox.Show("Timpul introdus nu are formatul corespunzător " +
                        "Introduceţi timpul după urmatorul format: HH:mm", "Avertisment",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            task.task_next_run = task.task_start_date + " " + task.task_time;

            if (task.Save())
            {
                task.CreateTask();

                MessageBox.Show("Task-ul a fost salvat cu succes", "Succes",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.InitTaskProperties();
            }
            else
            {
                MessageBox.Show("Ceva a mers prost, vă rugăm contactați dezvoltatorul!", "Eroare",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitTaskProperties()
        {
            TaskName = null;
            DirectoryClearCheckbox = false;
            DirectoryCopyCheckbox = false;
            OneWaySyncCheckbox = false;
            BothWaysSyncCheckbox = false;
            WithSubDir = false;
            SourceDirectoryPath = null;
            DestinationDirectoryPath = null;
            OneTimeTask = false;
            DailyTask = false;
            WeeklyTask = false;
            MonthlyTask = false;
            TaskInterval = 1;
            TaskStartDate = DateTime.Now;
            TaskEndDate = DateTime.Now.AddYears(10);
            TaskTime = DateTime.Now.ToString("HH:mm");
        }
    }
}
