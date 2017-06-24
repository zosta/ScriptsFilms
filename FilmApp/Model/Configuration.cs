using System.Collections.Generic;

namespace FilmApp.Model
{
    /// <summary>
    /// Classe configuration
    /// </summary>
    public class Configuration
    {
        private static Configuration instance;
        internal readonly string nomFichierExportFilms;
        internal readonly List<string> disqueChoosen;
        internal readonly string emplacementFichierExportFilms;

        /// <summary>
        /// L'instance du singleton Configuration
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configuration();
                }
                return instance;
            }
        }

        /// <summary>
        /// Constructeur : permet de charger le fichier Settings.Settings
        /// </summary>
        public Configuration()
        {
            nomFichierExportFilms = Properties.Settings.Default.NomFichierExportFilms;
            emplacementFichierExportFilms = Properties.Settings.Default.NomFichierExportFilms;
            disqueChoosen = new List<string>();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Disque1))
            {
                disqueChoosen.Add(Properties.Settings.Default.Disque1);
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Disque2))
            {
                disqueChoosen.Add(Properties.Settings.Default.Disque2);
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Disque3))
            {
                disqueChoosen.Add(Properties.Settings.Default.Disque3);
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Disque4))
            {
                disqueChoosen.Add(Properties.Settings.Default.Disque4);
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Disque5))
            {
                disqueChoosen.Add(Properties.Settings.Default.Disque5);
            }
        }
    }
}