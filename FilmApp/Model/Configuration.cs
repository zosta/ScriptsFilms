using System;
using System.Collections.Generic;
using System.IO;

namespace FilmApp.Model
{
    /// <summary>
    /// Classe configuration
    /// </summary>
    public class Configuration
    {
        private static Configuration instance;
        internal readonly string nomFichierExportFilms;
        internal readonly List<DisqueDur> disqueChoosen;
        internal readonly string emplacementFichierExportFilms;
        internal List<DisqueDur> disqueDispo;

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
            disqueChoosen = new List<DisqueDur>();

            disqueDispo = new List<DisqueDur>();
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady == true)
                {
                    disqueDispo.Add(new DisqueDur(false, d.Name, d.VolumeLabel));                   
                }
            }
        }


        internal void saveConfig()
        {
            Properties.Settings.Default.DisqueSelected = string.Empty;
            foreach ( DisqueDur dd in disqueChoosen )
            {
                Properties.Settings.Default.DisqueSelected += dd.VolumeLabel+ " ; ";

            }
            Properties.Settings.Default.Save();

        }
    }
}