using System.Collections.Generic;
using System.IO;

namespace FilmApp.Model
{
    internal class Dossier
    {
        private string urlDossier;
        public string urlFilms { get; }
        public string urlPoster { get; }
        public string urlFanart { get; }
        public string urlNfo { get; }
        public string urlActeurs { get; }

        public bool hasFilm { get; }
        public bool hasFanart { get; }
        public bool hasPoster { get; }
        public bool hasNfo { get; }
        public bool hasActeurs { get; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Dossier(string pUrlDossier)
        {
            urlDossier = pUrlDossier;

            hasFilm = false;
            hasFanart = false;
            hasNfo = false;
            hasActeurs = false;
            hasPoster = false;

            foreach (string sFileName in Directory.GetFiles(pUrlDossier, "*", SearchOption.TopDirectoryOnly))
            {
                List<string> extensionsFilms = new List<string>() { ".avi", ".mkv", ".mp4", ".wma", ".webm", ".vob", ".ogg", ".mpg", ".flv" };
                string extension = Path.GetExtension(sFileName).ToLower();
                if (extensionsFilms.Contains(extension))
                {
                    urlFilms = sFileName;
                    hasFilm = true;
                }
                if (sFileName.Contains("-poster"))
                {
                    urlPoster = sFileName;
                    hasPoster = true;
                }
                if (sFileName.Contains("-fanart"))
                {
                    urlFanart = sFileName;
                    hasFanart = true;
                }
                if (extension == ".nfo")
                {
                    urlNfo = sFileName;
                    hasNfo = true;
                }
                if (Directory.Exists(pUrlDossier + "\\.actors"))
                {
                    if (Directory.GetFiles(pUrlDossier + "\\.actors", "*.jpg").Length > 0)
                    {
                        hasActeurs = true;
                    }
                }
            }
            if (hasFilm && hasNfo && hasPoster && hasFanart && hasActeurs)
            {
                log.Info(string.Format("Le repertoire {0} contient un film, un NFO, une affiche de film , un fanart et les photos des acteurs.", urlDossier));
            }
            else if (hasFilm && hasNfo && hasPoster && hasActeurs)
            {
                log.Info(string.Format("Le repertoire {0} contient un film, un NFO, une affiche de film , et les photos des acteurs.", urlDossier));
            }
            else if (hasFilm && hasNfo && hasPoster)
            {
                log.Info(string.Format("Le repertoire {0} contient un film, un NFO, une affiche de film.", urlDossier));
            }
            else if (hasFilm && hasNfo)
            {
                log.Info(string.Format("Le repertoire {0} contient un film et un NFO", urlDossier));
            }
            else if (hasFilm)
            {
                log.Info(string.Format("Le repertoire {0} contient un film", urlDossier));
            }
            else
            {
                log.Info("Aucun film n'a été trouvé dans le repertoire :" + urlDossier + ", il sera ignoré.");
            }
        }

    }
}