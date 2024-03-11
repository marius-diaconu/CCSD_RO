using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

namespace Main.Views
{
    /// <summary>
    /// Interaction logic for CreateTaskView.xaml
    /// </summary>
    public partial class CreateTaskView : UserControl
    {
        private string default_tip = "Crează task-uri ce pot: curăța, copia și sincroniza dosare, la intervale de " +
            "timp alese de tine. Poți alege ca task-urile să ruleze doar o singură dată, zilnic, săptamânal sau lunar.";
        public CreateTaskView()
        {
            InitializeComponent();
            TaskTips.Text = default_tip;
        }

        public void DirectoryClearCheckbox_Clicked(object sender, RoutedEventArgs args)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                DestDirPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                DestDirPanel.Visibility = Visibility.Visible;
            }
        }

        public void DirectoryClearCheckbox_Checked(object sender, RoutedEventArgs args)
        {
            DirectoryCopyCheckbox.IsChecked = false;
            OneWaySyncCheckbox.IsChecked = false;
            BothWaysSyncCheckbox.IsChecked = false;
            DestDirPanel.Visibility = Visibility.Collapsed;
        }

        public void DirectoryCopyCheckbox_Checked(object sender, RoutedEventArgs args)
        {
            DirectoryClearCheckbox.IsChecked = false;
            OneWaySyncCheckbox.IsChecked = false;
            BothWaysSyncCheckbox.IsChecked = false;
            DestDirPanel.Visibility = Visibility.Visible;
        }

        public void OneWaySyncCheckbox_Checked(object sender, RoutedEventArgs args)
        {
            DirectoryClearCheckbox.IsChecked = false;
            DirectoryCopyCheckbox.IsChecked = false;
            BothWaysSyncCheckbox.IsChecked = false;
            DestDirPanel.Visibility = Visibility.Visible;
        }

        public void BothWaysSyncCheckbox_Checked(object sender, RoutedEventArgs args)
        {
            DirectoryClearCheckbox.IsChecked = false;
            DirectoryCopyCheckbox.IsChecked = false;
            OneWaySyncCheckbox.IsChecked = false;
            DestDirPanel.Visibility = Visibility.Visible;
        }

        public void OneTimeTask_Checked(object sender, RoutedEventArgs args)
        {
            DailyTask.IsChecked = false;
            WeeklyTask.IsChecked = false;
            MonthlyTask.IsChecked = false;
            FrequencyPanel.Visibility = Visibility.Collapsed;
        }

        public void DailyTask_Clicked(object sender, RoutedEventArgs args)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                FrequencyPanel.Visibility = Visibility.Visible;
            }
            else
            {
                FrequencyPanel.Visibility = Visibility.Collapsed;
            }
        }

        public void DailyTask_Checked(object sender, RoutedEventArgs args)
        {
            OneTimeTask.IsChecked = false;
            WeeklyTask.IsChecked = false;
            MonthlyTask.IsChecked = false;
            FrequencyPanel.Visibility = Visibility.Visible;
        }

        public void WeeklyTask_Clicked(object sender, RoutedEventArgs args)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                FrequencyPanel.Visibility = Visibility.Visible;
            }
            else
            {
                FrequencyPanel.Visibility = Visibility.Collapsed;
            }
        }

        public void WeeklyTask_Checked(object sender, RoutedEventArgs args)
        {
            OneTimeTask.IsChecked = false;
            DailyTask.IsChecked = false;
            MonthlyTask.IsChecked = false;
            FrequencyPanel.Visibility = Visibility.Visible;
        }

        public void MonthlyTask_Checked(object sender, RoutedEventArgs args)
        {
            OneTimeTask.IsChecked = false;
            DailyTask.IsChecked = false;
            WeeklyTask.IsChecked = false;
            FrequencyPanel.Visibility = Visibility.Collapsed;
        }

        public void TaskName_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Introdu un nume descriptiv pentru task-ul tau.";
        }

        public void TaskTypeClear_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Curăță dosarul sursă";
        }

        public void TaskTypeCopy_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Copiază în dosarul destinație conținutul dosarului sursă.";
        }

        public void TaskTypeOneWaySync_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Sincronizare într-o singură direcție. Întâi copiază conținutul dosarului sursă în dosarul destinație, " +
                "după care sincronizează dosarul de destinație cu dosarul sursă, astfel încât, dacă în dosarul de destinație sunt " +
                "fișiere și/sau subdosare ce nu se regăsesc în dosarul sursă va trece la ștergerea acestora din dosarul destinație. " +
                "Prin urmare, dacă doriți ștergerea unui fișier sau subdosar este suficient să-l ștergeți din dosarul sursă, iar " +
                "acesta va fi șters și din dosarul de destinație.";
        }

        public void TaskTypeBothWaysSync_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Sincronizare în ambele direcții. Întâi copiază dosarul sursă în dosarul destinație, după care " +
                "sincronizează cele două dosare, astfel încât, dacă la una dintre locații se regăsesc fișiere și/sau subdosare " +
                "ce nu se regăsesc și în cealaltă locție, va trece la copierea acestora în locația respectivă. Prin urmare, dacă " +
                "doriți ștergerea unui fișier sau subdosar, va trebuii să le ștergeți din ambele locații simultan.";
        }

        public void WithSubDir_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Bifează dacă dorești să ștergi, copiezi sau să sincronizezi și toate subdosarele din " +
                "dosarul sursă.";
        }

        public void SourceDirPath_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Apasă butonul [ Selectează ] și navighează la locația dosarului sursă, selectează-l, iar " +
                "apoi apasa butonul [ OK ]";
        }

        public void DestinationDirPath_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Apasă butonul [ Selectează ] și navighează la locația dosarului de destinație, selectează-l, " +
                "iar apoi apasa butonul [ OK ]";
        }

        public void OneTimeTask_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Task-ul rulează doar o singură dată la data și timpul specificate de dvs.";
        }

        public void DailyTask_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Task-ul rulează zilnic la ora specificată de dvs.";
        }

        public void WeeklyTask_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Task-ul rulează săptămânal în ziua și timpul specificate de dvs.";
        }

        public void MonthlyTask_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Task-ul rulează lunar la aceeaşi dată şi timp din lună specificate de dvs.";
        }

        public void TaskInterval_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Setează o valoare numerică mai mare sau egală cu 1. Ex: 3, iar task-ul va rula " +
                "o dată la 3 zile sau 3 săptămâni.";
        }

        public void TaskTime_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Setați timpul la care doriți ca task-ul să ruleze. Vă rugăm " +
                "introduceți timpul după următorul format: HH:mm. Ex: 09:01";
        }

        public void TaskStartDate_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Selectează data de la care dorești ca task-ul să ruleze.";
        }

        public void TaskEndDate_OnMouseEnter(object sender, MouseEventArgs args)
        {
            TaskTips.Text = "Selectează data de la care dorești ca task-ul să fie dezactivat automat.";
        }
    }
}
