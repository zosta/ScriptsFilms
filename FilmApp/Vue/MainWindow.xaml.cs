using FilmApp.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfScriptFilms.Vue;

namespace WpfScriptFilms
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ObservableCollection<DisqueDur> ListDisqueDur { get; set; }

        enum OperationConsole { addLine, resetAndAddLine };
        public MainWindow()
        {
            log.Info("Initialisation de la fenetre principale ");
            //InitializeComponent();
            //InitializeListDisqueDur();
            InitializeListDisqueDur();
            InitializeConsoleText();
            log.Info("Initialisation de la fenetre principale terminé ");

        }

        private void InitializeListDisqueDur()
        {
            ListDisqueDur = new ObservableCollection<DisqueDur>();
            foreach (DisqueDur dd in Configuration.Instance.disqueDispo)
            {
                bool isInConf = false;
                if(FilmApp.Properties.Settings.Default.DisqueSelected.Contains(dd.Name))
                {
                    isInConf = true;
                }
                ListDisqueDur.Add(new DisqueDur(isInConf, dd.VolumeLabel, dd.Name));
            }

            this.DataContext = this;
        }

        //public void InitializeListDisqueDur()
        //{
        //    foreach (string disque in Configuration.Instance.disqueChoosen)
        //    {
        //        lstDisque.Items.Add(disque);
        //    }
        //}

        private void InitializeConsoleText()
        {
            //ecrireConsole(MsgConst.accueilConsole, OperationConsole.addLine);
        }

        private void ecrireConsole(string pMessage, OperationConsole pOperation)
        {
            if (pOperation.Equals(OperationConsole.resetAndAddLine))
            {
                //txtBoxConsole.Text = "";
                ecrireConsole(pMessage, OperationConsole.addLine);
            }
            else if (pOperation.Equals(OperationConsole.addLine))
            {
                //txtBoxConsole.Text += pMessage + Environment.NewLine;
            }
        }

        private void btn_ExporterListeFilms_Click(object sender, RoutedEventArgs e)
        {
            new ExportWindow().ShowDialog();
            //Bibliotheque.Instance.exporterListeFilms();
            //ecrireConsole(MsgConst.accueilExportListeFilms, OperationConsole.resetAndAddLine);            
        }

        /// <summary>
        /// Permet de lancer la creationd e dossier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CreerDossierFilms_Click(object sender, RoutedEventArgs e)
        {
            Bibliotheque.Instance.creerDossierFilm();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Bibliotheque.Instance.listerFilmsADL();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            log.Info("ListBox Selection changé");

        }

        private void btn_SaveParams_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Click bouton sauvegarder Disque Dur actif.");

            Configuration.Instance.disqueChoosen.Clear();
            foreach (DisqueDur dd in ListDisqueDur)
            {
                if (dd.IsSelected)
                {
                    Configuration.Instance.disqueChoosen.Add(dd);
                }
            }
            Configuration.Instance.saveConfig();
        }
    }
}
