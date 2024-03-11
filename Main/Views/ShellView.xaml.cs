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
using System.Windows.Shapes;
using ClassLibrary;

namespace Main.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            this.Title = Utility.prog_name + ". Version: " + Utility.prog_version;
            this.Height = 600;
            this.Width = 800;
            this.MinHeight = 600;
            this.MinWidth = 800;
            InitializeComponent();
        }
    }
}
