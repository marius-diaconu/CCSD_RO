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
using System.Diagnostics;
using ClassLibrary;

namespace Main.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        private string build_year = "2021";

        private string default_tip = "Acest program este protejat prin drepturi de autor și este deținut de SC MDS DEV IT SRL. " +
            "Programul este certificat și autentificat cu certificat de securitate și autenticitate agreeat de Microsoft. " +
            "Orice încercare de copiere și/sau piratare a acestuia se pedepsește conform legilor în vigoare.";
        public AboutView()
        {
            var now = DateTime.Now.ToString("yyyy");

            InitializeComponent();
            AboutTips.Text = default_tip;
            SoftName.Text = "Nume: " + Utility.assembly_name;
            SoftDescription.Text = "Descriere: " + Utility.description;

            if (now != build_year)
            {
                CopyrightLabel.Text = "Copyright © " + build_year + " - " + now + " SC MDS DEV IT SRL, All rights reserved";
            }
            else
            {
                CopyrightLabel.Text = "Copyright © " + build_year + " SC MDS DEV IT SRL, All rights reserved";
            }
        }

        private void Hyperlink_OpenClientMail(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void Hyperlink_GoToLinkedin(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        public void PhoneNo_OnMouseEnter(object sender, MouseEventArgs e)
        {
            AboutTips.Text = "Dacă aveți întrebări, nelămuriri sau întâmpinați dificultăți în utilizarea acestui program, " +
                "sau pentru orice alte sugestii, vă rugăm, nu ezitați să ne contactați telefonic.";
        }

        public void PhoneNo_OnMouseLeave(object sender, MouseEventArgs e)
        {
            AboutTips.Text = default_tip;
        }

        public void MailMe_OnMouseEnter(object sender, MouseEventArgs e)
        {
            AboutTips.Text = "La click se deschide clientul dvs. de mail cu ajutorul căruia ne puteți " +
                "trimite email la adresa specificată.";
        }

        public void MailMe_OnMouseLeave(object sender, MouseEventArgs e)
        {
            AboutTips.Text = default_tip;
        }

        public void FollowUs_OnMouseEnter(object sender, MouseEventArgs e)
        {
            AboutTips.Text = "La click se deschide în browser-ul dvs. pagina noastră de Facebook, " +
                "unde ne puteți urmării activitatea.";
        }

        public void FollowUs_OnMouseLeave(object sender, MouseEventArgs e)
        {
            AboutTips.Text = default_tip;
        }
    }
}
