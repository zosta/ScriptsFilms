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
using System.Windows.Shapes;

namespace WpfScriptFilms
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        List<DisqueDur> lstDisque;

        public ConfigurationWindow()
        {
            lstDisque = new List<DisqueDur>();
            InitializeComponent();
            recupAllDisque();
        }

        private void recupAllDisque()
        {
            

            //listAllDisque.ItemsSource = DriveInfo.GetDrives();

            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    DisqueDur disque = new DisqueDur(drive.Name);
                    //listAllDisque.Items.Add(drive.Name);
                    lstDisque.Add(disque);
                    //Console.WriteLine("Drive Type: {0}", drive.VolumeLabel);
                }
                catch (IOException e)
                {
                    
                }
            }
            listAllDisque.DataContext = lstDisque;
        }

        private void btnAnnulerConfig_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnValiderConfig_Click(object sender, RoutedEventArgs e)
        {
            Boolean doitSauvegarder = false;
            foreach(DisqueDur disque in lstDisque)
            {
                if(disque.IsSelected)
                {
                    saveDisqueInConf(disque.Name,"add");
                    doitSauvegarder = true;
                }
                else if(!disque.IsSelected)
                {
                    saveDisqueInConf(disque.Name,"remove");
                    doitSauvegarder = true;
                }
            }
            if(doitSauvegarder)
            {
                Configuration.Instance.saveConf();
            }

            this.Close();
        }

        private void saveDisqueInConf(string name,string pOperation)
        {
            //si il n'y est pas déjà
            if(pOperation == "add" && !Configuration.Instance.disqueChoosen.Contains(name))
            {
                Configuration.Instance.disqueChoosen.Add(name);
            }
            else if (pOperation == "remove" && Configuration.Instance.disqueChoosen.Contains(name))
            {
                Configuration.Instance.disqueChoosen.Remove(name);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                //FileNameTextBox.Text = filename;
            }
        }
    }
}
