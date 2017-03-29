using System;
using System.Collections.Generic;
using System.Linq;
using WpfScriptFilms.Properties;

namespace WpfScriptFilms
{
    class Configuration
    {
        public List<string> disqueChoosen { get; }
        public string nomFichierExportFilms { get; internal set;}
        public string emplacementFichierExport { get; internal set; }

        public Configuration()
        {
            disqueChoosen = new List<string>();
            nomFichierExportFilms = "";
            loadConfigDisque();
            loadConfigExportFils();
        }

        private void loadConfigExportFils()
        {
            nomFichierExportFilms = Settings.Default.NomFichierExport;
            emplacementFichierExport = Settings.Default.EmplacementFichierExport;
        }

        private void loadConfigDisque()
        {
            string disque1 = Settings.Default.Disque1;
            string disque2 = Settings.Default.Disque2;
            string disque3 = Settings.Default.Disque3;
            string disque4 = Settings.Default.Disque4;
            string disque5 = Settings.Default.Disque5;

            addDisque(disque1);
            addDisque(disque2);
            addDisque(disque3);
            addDisque(disque4);
            addDisque(disque5);


        }


        private void addDisque(string pDisque)
        {
            if(!string.IsNullOrEmpty(pDisque))
                disqueChoosen.Add(pDisque);
        }

        internal void saveConf()
        {
            Settings.Default.Disque1 = null;
            Settings.Default.Disque2 = null;
            Settings.Default.Disque3 = null;
            Settings.Default.Disque4 = null;
            Settings.Default.Disque5 = null;
            int i = 1;
            foreach(String disque in disqueChoosen)
            {

                if(i == 1)
                { 
                    Settings.Default.Disque1 = disque;
                }
                else if (i == 2)
                {
                    Settings.Default.Disque2 = disque;
                }
                else if (i == 3)
                {
                    Settings.Default.Disque3 = disque;
                }
                else if (i == 4)
                {
                    Settings.Default.Disque4 = disque;
                }
                else if (i == 5)
                {
                    Settings.Default.Disque5 = disque;
                }
                i++;
            }
            Properties.Settings.Default.Save();
        }
    }
}
