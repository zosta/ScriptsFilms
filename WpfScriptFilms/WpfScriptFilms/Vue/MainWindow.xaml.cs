using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WpfScriptFilms
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum OperationConsole { addLine, resetAndAddLine };
        Configuration conf;
        public MainWindow()
        {
            conf = new Configuration();

            InitializeComponent();
            InitializeListDisqueDur();
            InitializeConsoleText();
        }

        public void InitializeListDisqueDur()
        {
            foreach (string disque in conf.disqueChoosen)
            {
                lstDisque.Items.Add(disque);
            }
        }

        private void InitializeConsoleText()
        {
            ecrireConsole(MsgConst.accueilConsole, OperationConsole.addLine);
        }

        private void ecrireConsole(string pMessage, OperationConsole pOperation)
        {
            if (pOperation.Equals(OperationConsole.resetAndAddLine))
            {
                txtBoxConsole.Text = "";
                ecrireConsole(pMessage, OperationConsole.addLine);
            }
            else if (pOperation.Equals(OperationConsole.addLine))
            {
                txtBoxConsole.Text += pMessage + Environment.NewLine;
            }
        }

        private void btn_ExporterListeFilms_Click(object sender, RoutedEventArgs e)
        {
            ecrireConsole(MsgConst.accueilExportListeFilms, OperationConsole.resetAndAddLine);

            string nomFichier = conf.nomFichierExportFilms + ".txt";
            List<string> liFilms = new List<string>();

            foreach (string disque in conf.disqueChoosen)
            {
                string sourceDirectory = disque;
                int nbDossierActuelle = 0;
                //int nbDossierDeFilm = Directory.GetDirectories(sourceDirectory).Length;
                try
                {
                    var repertoires = Directory.EnumerateDirectories(sourceDirectory, "*", SearchOption.TopDirectoryOnly);

                    foreach (string repertoire in repertoires)
                    {
                        if (!repertoire.Contains("$RECYCLE"))
                        {
                            nbDossierActuelle++;
                            liFilms.Add(repertoire.Replace(sourceDirectory, ""));
                        }
                    }

                }
                catch (UnauthorizedAccessException UAEx)
                {
                    ecrireConsole(UAEx.Message, OperationConsole.resetAndAddLine);
                }
                catch (PathTooLongException PathEx)
                {
                    ecrireConsole(PathEx.Message, OperationConsole.resetAndAddLine);
                }
            }
            liFilms.Sort();

            liFilms.Add("==========================");
            liFilms.Add("Nombre total de films : " + liFilms.Count);
            File.WriteAllLines(conf.emplacementFichierExport + nomFichier, liFilms);
            //ouvrir fichier
            Process.Start(conf.emplacementFichierExport + nomFichier);
            ecrireConsole(MsgConst.exportFilmTermine, OperationConsole.addLine);
        }

        private void btn_conf_Click(object sender, RoutedEventArgs e)
        {
            new ConfigurationWindow().ShowDialog();
        }
    }
}
