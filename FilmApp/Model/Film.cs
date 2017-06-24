using FilmApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FilmApp.Model
{
    /// <summary>
    ///  La classe film , qui décrit un film
    /// </summary>
    class Film
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string titre { get; }
        public NFO nfo { get; internal set; }
        public string urlPoster { get; }
        public string urlFanart { get; }
        public string urlDossierActeurs { get; }
        private string urlFilm { get; }
        public string noteMoy { get; internal set; }
        public string noteNb { get; internal set; }
        public string codec { get; set; }
        public string langueOrigine { get; internal set; }
        public int annee { get; }
        public string extension { get; }
        public DateTime dateAjout { get; }
        public int[] resolution { get; internal set; }
        public string resume { get; internal set; }
        public string titreOriginal { get; internal set; }
        public int taille { get; internal set; }

        public TimeSpan duree { get; set; }
        public List<string> genres { get; internal set; }
        public List<Acteur> acteurs { get; internal set; }



        /// <summary>
        /// Constructeur, le constructeur utilisé lorsqu'on a un film.
        /// </summary>
        /// <param name="pUrlFilm"></param>
        public Film(string pUrlFilm)
        {
            urlFilm = pUrlFilm;
            titre = getNameFromChemin(pUrlFilm);
            annee = getYearFromChemin(pUrlFilm);
            extension = Path.GetExtension(pUrlFilm);
            dateAjout = File.GetCreationTime(pUrlFilm);
        }
        /// <summary>
        /// Constructeur, le constructeur utilisé lorsqu'on a un film et un NFO. 
        /// </summary>
        /// <param name="pUrlFilm"></param>
        /// <param name="pUrlNfo"></param>
        public Film(string pUrlFilm, string pUrlNfo)
        {
            urlFilm = pUrlFilm;
            titre = getNameFromChemin(pUrlFilm);
            annee = getYearFromChemin(pUrlFilm);
            extension = Path.GetExtension(pUrlFilm);
            dateAjout = File.GetCreationTime(pUrlFilm);
            addInfosFromNfo(pUrlNfo);
            
        }



        /// <summary>
        /// Constructeur, le constructeur utilisé lorsqu'on a un film, un NFO , un poster et un fanart. 
        /// </summary>
        /// <param name="pUrlFilm"></param>
        /// <param name="pUrlNfo"></param>
        /// <param name="pUrlPoster"></param>
        /// <param name="pUrlFanart"></param>
        public Film(string pUrlFilm, string pUrlNfo, string pUrlPoster, string pUrlFanart)
        {
            
            urlFilm = pUrlFilm;
            titre = getNameFromChemin(pUrlFilm);
            annee = getYearFromChemin(pUrlFilm);
            extension = Path.GetExtension(pUrlFilm);
            dateAjout = File.GetCreationTime(pUrlFilm);
            addInfosFromNfo(pUrlNfo);
        }
        /// <summary>
        /// Constructeur, le constructeur utilisé lorsqu'on a un film, un poster , un fanart et des acteurs. 
        /// </summary>
        /// <param name="pUrlFilm"></param>
        /// <param name="pUrlNfo"></param>
        /// <param name="pUrlPoster"></param>
        /// <param name="pUrlFanart"></param>
        /// <param name="pUrlDossierActeurs"></param>
        public Film(string pUrlFilm, string pUrlNfo, string pUrlPoster, string pUrlFanart, string pUrlDossierActeurs) 
        {
            urlFilm = pUrlFilm;
            titre = getNameFromChemin(pUrlFilm);
            annee = getYearFromChemin(pUrlFilm);
            extension = Path.GetExtension(pUrlFilm);
            dateAjout = File.GetCreationTime(pUrlFilm);
            addInfosFromNfo(pUrlNfo);
        }
        /// <summary>
        /// Méthode qui retourne le titre du film depuis son chemin
        /// </summary>
        /// <param name="pChemin">Le chemin du film</param>
        /// <returns>Le titre du film</returns>
        private string getNameFromChemin(string pChemin)
        {
            string titre = "";
            Regex regex = new Regex(@" \(([(19|20)\d{2}$]*)\)");
            string cheminSansExtensions = Path.GetFileNameWithoutExtension(pChemin);
            Boolean isFilm = false;

            if (regex.Match(cheminSansExtensions).Success)
            {
                try
                {
                    titre = regex.Replace(cheminSansExtensions, "");
                    isFilm = true;
                }
                catch (ArgumentNullException e)
                {
                    log.Error("Le chemin dont on veut trouver le titre est null", e);
                }
            }
            if (!isFilm)
            {
                log.Info(cheminSansExtensions + " n'a pas le format attendu, le fichier sera ignoré");
                throw new Exception();
            }
            return titre;
        }
        /// <summary>
        /// Méthode qui retourne l'année du film depuis son chemin
        /// </summary>
        /// <param name="pChemin">Le chemin du film</param>
        /// <returns>Le titre du film</returns>
        private int getYearFromChemin(string pChemin)
        {
            string annee = "";
            int anneeInt = 0;
            Regex regex = new Regex(@"\(([(19|20)\d{2}$]*)\)");
            annee = regex.Match(pChemin).Groups[1].Value;
            try
            {
                anneeInt = Int32.Parse(annee);
            }
            catch (FormatException e)
            {
                log.Error("La chaine entre parenthèse n'a pu etre convertit en année", e);
            }
            return anneeInt;
        }
        /// <summary>
        /// Ajoute des infos aux films depuis le NFO
        /// </summary>
        /// <param name="pUrlNfo"></param>
        private void addInfosFromNfo(string pUrlNfo)
        {
            nfo = new NFO(pUrlNfo);

            Film filmExtrait = nfo.getInfosNfo();
            resume = filmExtrait.resume;
            codec = filmExtrait.codec;
            langueOrigine = filmExtrait.langueOrigine;
            acteurs = filmExtrait.acteurs;
            noteMoy = filmExtrait.noteMoy;
            noteNb = filmExtrait.noteNb;
            resolution = nfo.resolution;
        }

        public override string ToString()
        {
            StringBuilder strbld = new StringBuilder();
            strbld.AppendFormat("Titre : {0} , ", titre);
            strbld.AppendFormat("Année : {0} , ", annee);
            strbld.AppendFormat("Extension : {0} , ", extension);
            strbld.AppendFormat("Durée : {0} , ", duree);
            strbld.AppendFormat("Résolution : {0} , ", resolution);
            strbld.AppendFormat("Taille : {0} , ", taille);

            return strbld.ToString();
        }
    }
}