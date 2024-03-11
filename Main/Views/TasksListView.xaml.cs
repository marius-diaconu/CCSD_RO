using Caliburn.Micro;
using System;
using System.Collections.Generic;
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
using Main.ViewModels;

namespace Main.Views
{
    /// <summary>
    /// Interaction logic for TasksListView.xaml
    /// </summary>
    public partial class TasksListView : UserControl
    {
        private Tasks _task;
        private List<Tasks> _tasks;
        public int countTasks;
        public int per_page;
        public int index;
        public int maxIndex;
        public int offset;
        public string search;
        public TasksListView()
        {
            per_page = 10;
            index = 1;
            offset = (index * per_page) - per_page;

            InitializeComponent();

            GetTasks();
            BuildTasksTable();
        } // end of construct method

        public void GetTasks()
        {
            _task = new Tasks();
            countTasks = _task.Count();

            if (string.IsNullOrEmpty(search))
            {
                _tasks = _task.Where("SELECT * FROM `" + _task.table + "` LIMIT " + 
                    per_page + " OFFSET " + offset);
            }
            else
            {
                var tasksList = _task.Where("SELECT * FROM `" + _task.table + "` " +
                    "WHERE `task_name` LIKE '%" + search + "%'");

                _tasks = _task.Where("SELECT * FROM `" + _task.table + "` " +
                    "WHERE `task_name` LIKE '%" + search + "%' " +
                    "LIMIT " + per_page + " OFFSET " + offset);

                countTasks = tasksList.Count;
            }

            if (countTasks > per_page)
            {
                maxIndex = Convert.ToInt32(Math.Ceiling((decimal)countTasks / per_page));

                if (maxIndex > 1)
                {
                    NavigationPanel.Visibility = Visibility.Visible;

                    if (index == maxIndex)
                    {
                        PreviousPage.Visibility = Visibility.Visible;
                        NextPage.Visibility = Visibility.Collapsed;
                        PageIndexMsg.Text = "Pagina: " + index + " din " + maxIndex;
                    }
                    else if (index == 1)
                    {
                        PreviousPage.Visibility = Visibility.Collapsed;
                        NextPage.Visibility = Visibility.Visible;
                        PageIndexMsg.Text = "Pagina: " + index + " din " + maxIndex;
                    }
                    else
                    {
                        PreviousPage.Visibility = Visibility.Visible;
                        NextPage.Visibility = Visibility.Visible;
                        PageIndexMsg.Text = "Pagina: " + index + " din " + maxIndex;
                    }
                }
                else
                {
                    NavigationPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                NavigationPanel.Visibility = Visibility.Collapsed;
            }
        } // end of GetTasks method

        public void PerPageSelect_Changed(object sender, RoutedEventArgs args)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)PerPageSelect.SelectedItem;
            int tempLimit = Convert.ToInt32(selectedItem.Content);
            if (per_page != tempLimit)
            {
                per_page = tempLimit;
                offset = (index * per_page) - per_page;

                GetTasks();
                BuildTasksTable();
            }
        } // end of PerPageSelect_Changed method

        public void SearchBtn_Clicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                MessageBox.Show("Trebuie să introduci un termen de căutare!", "Avertisment", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                search = SearchBox.Text;
                SearchBox.Text = null;

                GetTasks();
                BuildTasksTable();
            }
        } // end of SearchBtn_Clicked method

        public void RefreshBtn_Clicked(object sender, RoutedEventArgs args)
        {
            per_page = 10;
            index = 1;
            offset = (index * per_page) - per_page;
            PerPageSelect.Text = "10";
            SearchBox.Text = null;
            search = null;

            GetTasks();
            BuildTasksTable();
        } // end of RefreshBtn_Clicked method

        public void PreviousPageBtn_Clicked(object sender, RoutedEventArgs args)
        {
            if (index > 1)
            {
                index--;
                offset = (index * per_page) - per_page;

                GetTasks();
                BuildTasksTable();
            }
        } // end of PreviousPageBtn_Clicked method

        public void NextPageBtn_Clicked(object sender, RoutedEventArgs args) 
        {
            if (index < maxIndex)
            {
                index++;
                offset = (index * per_page) - per_page;

                GetTasks();
                BuildTasksTable();
            }
        } // end of NextPageBtn_Clicked method

        public void BuildTasksTable()
        {
            TasksListGrid.Children.Clear();

            if (_tasks.Count > 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    TasksListGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
                }

                for (int i = 0; i <= _tasks.Count; i++)
                {
                    TasksListGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
                }

                for (int i = 0; i <= _tasks.Count; i++)
                {
                    if (i == 0)
                    {
                        // Bordered box for header id
                        Border hid_border = new Border();
                        hid_border.BorderBrush = new SolidColorBrush(Colors.Black);
                        hid_border.BorderThickness = new Thickness(1);
                        Grid.SetRow(hid_border, i);
                        Grid.SetColumn(hid_border, 0);

                        TextBlock hid_column = new TextBlock();
                        hid_column.Text = "ID";
                        hid_column.FontSize = 16;
                        hid_column.FontWeight = FontWeights.Bold;
                        hid_column.VerticalAlignment = VerticalAlignment.Center;
                        hid_column.HorizontalAlignment = HorizontalAlignment.Center;
                        hid_column.Margin = new Thickness(5);
                        hid_column.Padding = new Thickness(10, 2, 10, 2);
                        hid_border.Child = hid_column;
                        TasksListGrid.Children.Add(hid_border);

                        // Bordered box for header task name
                        Border hname_border = new Border();
                        hname_border.BorderBrush = new SolidColorBrush(Colors.Black);
                        hname_border.BorderThickness = new Thickness(1);
                        Grid.SetRow(hname_border, i);
                        Grid.SetColumn(hname_border, 1);

                        TextBlock hname_column = new TextBlock();
                        hname_column.Text = "Nume Task";
                        hname_column.FontSize = 16;
                        hname_column.FontWeight = FontWeights.Bold;
                        hname_column.VerticalAlignment = VerticalAlignment.Center;
                        hname_column.HorizontalAlignment = HorizontalAlignment.Center;
                        hname_column.Margin = new Thickness(5);
                        hname_column.Padding = new Thickness(10, 2, 10, 2);
                        hname_border.Child = hname_column;
                        TasksListGrid.Children.Add(hname_border);

                        // Bordered box for header buttons
                        Border option_border = new Border();
                        option_border.BorderBrush = new SolidColorBrush(Colors.Black);
                        option_border.BorderThickness = new Thickness(1);
                        Grid.SetRow(option_border, i);
                        Grid.SetColumn(option_border, 2);
                        Grid.SetColumnSpan(option_border, 3);

                        TextBlock option_column = new TextBlock();
                        option_column.Text = "Opțiuni";
                        option_column.FontSize = 16;
                        option_column.FontWeight = FontWeights.Bold;
                        option_column.VerticalAlignment = VerticalAlignment.Center;
                        option_column.HorizontalAlignment = HorizontalAlignment.Center;
                        option_column.Margin = new Thickness(5);
                        option_column.Padding = new Thickness(10, 2, 10, 2);
                        option_border.Child = option_column;
                        TasksListGrid.Children.Add(option_border);
                    }
                    else
                    {
                        // Bordered box for task id
                        Border id_border = new Border();
                        id_border.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                        id_border.BorderThickness = new Thickness(1);
                        Grid.SetRow(id_border, i);
                        Grid.SetColumn(id_border, 0);

                        TextBlock id_column = new TextBlock();
                        id_column.Text = _tasks[i - 1].id.ToString();
                        id_column.Margin = new Thickness(5);
                        id_column.Padding = new Thickness(10, 2, 10, 2);
                        id_border.Child = id_column;
                        TasksListGrid.Children.Add(id_border);

                        // Bordered box for task name
                        Border name_border = new Border();
                        name_border.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                        name_border.BorderThickness = new Thickness(1);
                        Grid.SetRow(name_border, i);
                        Grid.SetColumn(name_border, 1);

                        TextBlock name_column = new TextBlock();
                        name_column.Text = _tasks[i - 1].task_name;
                        name_column.Margin = new Thickness(5);
                        name_column.Padding = new Thickness(10, 2, 10, 2);
                        name_border.Child = name_column;
                        TasksListGrid.Children.Add(name_border);

                        // Bordered box for activate/deactivate button
                        Border actv_border = new Border();
                        actv_border.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                        actv_border.BorderThickness = new Thickness(1);
                        Grid.SetRow(actv_border, i);
                        Grid.SetColumn(actv_border, 2);

                        Button actv_button = new Button();
                        actv_button.Content = _tasks[i-1].IsStopped() ? "Activează" : "Dezactivează";
                        actv_button.Tag = _tasks[i - 1].task_uuid;
                        actv_button.Margin = new Thickness(5);
                        actv_button.Padding = new Thickness(10, 2, 10, 2);
                        actv_button.Click += new RoutedEventHandler(StopTask_OnClick);
                        actv_border.Child = actv_button;
                        TasksListGrid.Children.Add(actv_border);

                        // Bordered box for edit button
                        Border edit_border = new Border();
                        edit_border.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                        edit_border.BorderThickness = new Thickness(1);
                        Grid.SetRow(edit_border, i);
                        Grid.SetColumn(edit_border, 3);

                        Button edit_button = new Button();
                        edit_button.Content = "Editează";
                        edit_button.Tag = _tasks[i - 1].task_uuid;
                        edit_button.Margin = new Thickness(5);
                        edit_button.Padding = new Thickness(10, 2, 10, 2);
                        edit_button.Click += new RoutedEventHandler(EditTask_OnClick);
                        edit_border.Child = edit_button;
                        TasksListGrid.Children.Add(edit_border);

                        // Bordered box for delete button
                        Border delete_border = new Border();
                        delete_border.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                        delete_border.BorderThickness = new Thickness(1);
                        Grid.SetRow(delete_border, i);
                        Grid.SetColumn(delete_border, 4);

                        Button delete_button = new Button();
                        delete_button.Content = "Șterge";
                        delete_button.Tag = _tasks[i - 1].task_uuid;
                        delete_button.Margin = new Thickness(5);
                        delete_button.Padding = new Thickness(10, 2, 10, 2);
                        delete_button.Click += new RoutedEventHandler(DeleteTask_OnClick);
                        delete_border.Child = delete_button;
                        TasksListGrid.Children.Add(delete_border);
                    }
                }
            }
            else
            {
                TasksListGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
                TasksListGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });

                TextBlock textBlock = new TextBlock();
                textBlock.Text = "Nu aveți task-uri!";
                textBlock.FontSize = 24;
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.Margin = new Thickness(5);
                textBlock.Padding = new Thickness(10, 2, 10, 2);
                Grid.SetRow(textBlock, 0);
                Grid.SetColumn(textBlock, 0);

                TasksListGrid.Children.Add(textBlock);
            }
        } // end of BuildTasksTable method

        public void StopTask_OnClick(object sender, RoutedEventArgs args)
        {
            Tasks task = new Tasks();
            task = task.FindByUuid(((Button)sender).Tag.ToString());

            if (task.Exists())
            {
                if (task.IsStopped())
                {
                    task.task_stopped = 0;
                    if (task.Save()) task.CreateTask();
                }
                else
                {
                    task.task_stopped = 1;
                    if (task.Save()) task.DeleteTask();
                }

                GetTasks();
                BuildTasksTable();
            }
        } // end of StopTask_OnClick method

        public void EditTask_OnClick(object sender, RoutedEventArgs args)
        {
            string task_uuid = ((Button)sender).Tag.ToString();
            IWindowManager manager = new WindowManager();
            manager.ShowDialog(new EditTaskViewModel(task_uuid));

            GetTasks();
            BuildTasksTable();
        } // end of EditTask_OnClick method

        public void DeleteTask_OnClick(object sender, RoutedEventArgs args)
        {
            Tasks task = new Tasks();
            task = task.FindByUuid(((Button)sender).Tag.ToString());
            task.DeleteTask();

            if (task.Delete())
            {
                GetTasks();
                BuildTasksTable();

                MessageBox.Show("Task-ul a fost șters cu succes!", "Succes",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ceva a mers prost, vă rugăm contactați dezvoltatorul!", "Eroare",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } // end of DeleteTask_OnClick method
    }
}
