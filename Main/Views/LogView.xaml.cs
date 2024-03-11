using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassLibrary;

namespace Main.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        private List<Logs> logs;
        private Logs log;
        public DateTime minDate;
        public DateTime maxDate;
        public LogView()
        {
            InitializeComponent();

            log = new Logs();
            logs = log.Get("desc");

            if (logs.Count > 0)
            {
                maxDate = DateTime.Parse(DateTime.Parse(logs[0].created_at).ToString("yyyy-MM-dd"));
                minDate = DateTime.Parse(DateTime.Parse(logs[logs.Count - 1].created_at).ToString("yyyy-MM-dd"));

                LogCalendar.SelectedDate = maxDate;
                LogCalendar.DisplayDateEnd = maxDate;
                LogCalendar.DisplayDateStart = minDate;
            }
        }

        public void OnDateChanged_Event(object sender, SelectionChangedEventArgs args)
        {
            foreach (Logs log in logs)
            {
                if (DateTime.Parse(DateTime.Parse(sender.ToString()).ToString("yyyy-MM-dd"))
                    == DateTime.Parse(DateTime.Parse(log.created_at).ToString("yyyy-MM-dd")))
                {
                    LogFileHeader.Text = "Activitate: " + DateTime.Parse(log.created_at).ToString("d MMM yyyy");
                    LoadLogsFile.Text = File.Exists(log.filepath) ? File.ReadAllText(log.filepath) : null;
                    break;
                }
            }
        }
    }
}
