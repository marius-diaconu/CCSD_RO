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
    /// Interaction logic for ErrorView.xaml
    /// </summary>
    public partial class ErrorView : UserControl
    {
        private List<Errors> errors;
        private Errors error;
        public DateTime minDate;
        public DateTime maxDate;
        public ErrorView()
        {
            InitializeComponent();

            error = new Errors();
            errors = error.Get("desc");

            if (errors.Count > 0)
            {
                maxDate = DateTime.Parse(DateTime.Parse(errors[0].created_at).ToString("yyyy-MM-dd"));
                minDate = DateTime.Parse(DateTime.Parse(errors[errors.Count - 1].created_at).ToString("yyyy-MM-dd"));

                ErrorCalendar.SelectedDate = maxDate;
                ErrorCalendar.DisplayDateEnd = maxDate;
                ErrorCalendar.DisplayDateStart = minDate;
            }
        }

        public void OnDateChanged_Event(object sender, SelectionChangedEventArgs args)
        {
            foreach (Errors error in errors)
            {
                if (DateTime.Parse(DateTime.Parse(sender.ToString()).ToString("yyyy-MM-dd"))
                    == DateTime.Parse(DateTime.Parse(error.created_at).ToString("yyyy-MM-dd")))
                {
                    ErrorFileHeader.Text = "Erori: " + DateTime.Parse(error.created_at).ToString("d MMM yyyy");
                    LoadErrorsFile.Text = File.Exists(error.filepath) ? File.ReadAllText(error.filepath) : null;
                    break;
                }
            }
        }
    }
}
