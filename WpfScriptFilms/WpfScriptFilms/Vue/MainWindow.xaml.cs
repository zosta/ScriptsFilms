﻿using System;
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

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        enum OperationConsole { addLine, resetAndAddLine };
        public MainWindow()
        {
            InitializeComponent();
            InitializeListDisqueDur();
            InitializeConsoleText();
        }

        public void InitializeListDisqueDur()
        {
            foreach (string disque in Configuration.Instance.disqueChoosen)
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
            Bibliotheque.Instance.exporterListeFilms();
            ecrireConsole(MsgConst.accueilExportListeFilms, OperationConsole.resetAndAddLine);            
        }

        private void btn_conf_Click(object sender, RoutedEventArgs e)
        {
            new ConfigurationWindow().ShowDialog();
        }

        private void btn_CreerDossierFilms_Click(object sender, RoutedEventArgs e)
        {
            Bibliotheque.Instance.creerDossierFilmsSeul();
        }
    }
}