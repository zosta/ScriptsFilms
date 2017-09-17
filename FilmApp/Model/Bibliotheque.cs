using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FilmApp.Model
{
    /// <summary>
    /// La classe Bibliotheque, celle ci est unique
    /// </summary>
    class Bibliotheque
    {

        private static Bibliotheque instance;
        private List<Film> mesFilms = new List<Film>();


        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Le constructeur du singleton Bibliothèque.
        /// </summary>
        private Bibliotheque()
        {
            log.Info("Lancement du singleton Bibliotheque");
        }

        /// <summary>
        /// Le singleton de la classe Bibliothèque
        /// </summary>
        public static Bibliotheque Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Bibliotheque();
                }
                return instance;
            }
        }

        public void exporterListeFilms()
        {
            //debut du temps d'execution
            DateTime startTime = DateTime.Now;

            log.Info("Debut de l'export de la liste des films");

            string nomFichier = Configuration.Instance.nomFichierExportFilms + ".txt";

            foreach (DisqueDur dd in Configuration.Instance.disqueChoosen)
            {
                string sourceDirectory = dd.Name;
                try
                {
                    var repertoires = Directory.EnumerateDirectories(sourceDirectory, "*", SearchOption.TopDirectoryOnly);
                    //on cherche dans chaque repertoire
                    foreach (string repertoire in repertoires)
                    {
                        //gérer le cas ou on a plusieurs films dans un seul repertoire

                        //on cherche ce que contient le repertoire
                        Dossier dossierInfos = new Dossier(repertoire);
                        
                        if (dossierInfos.hasFilm && dossierInfos.hasFanart && dossierInfos.hasPoster && dossierInfos.hasActeurs && dossierInfos.hasNfo)
                        {
                            Film film = new Film(dossierInfos.urlFilms, dossierInfos.urlNfo, dossierInfos.urlPoster, dossierInfos.urlFanart, dossierInfos.urlActeurs);
                            mesFilms.Add(film);
                            
                        }
                        if (dossierInfos.hasFilm && dossierInfos.hasFanart && dossierInfos.hasPoster && dossierInfos.hasNfo)
                        {
                           
                            Film film = new Film(dossierInfos.urlFilms, dossierInfos.urlNfo, dossierInfos.urlPoster, dossierInfos.urlFanart);
                            mesFilms.Add(film);
                        }
                        if (dossierInfos.hasFilm && dossierInfos.hasNfo)
                        {
                 
                            Film film = new Film(dossierInfos.urlFilms, dossierInfos.urlNfo);
                            mesFilms.Add(film);
                        }
                        else if (dossierInfos.hasFilm)
                        {
                            Film film = new Film(dossierInfos.urlFilms);
                            mesFilms.Add(film);
                        }
                    }
                }
                catch (UnauthorizedAccessException UAEx)
                {
                    log.Error("Le repertoire " + sourceDirectory + " n'a pas les droits pour acceder à ce repertoire.", UAEx);
                }
                catch (PathTooLongException PathEx)
                {
                    log.Error("Le repertoire " + sourceDirectory + " a un chemin trop long, operation impossible.", PathEx);
                }
                catch (DirectoryNotFoundException DirectoryEx)
                {
                    log.Error("Le repertoire " + sourceDirectory + " n'a pas été trouvé, veuillez le brancher ou modifier la configuration.", DirectoryEx);
                }
            }
            if (mesFilms.Count > 0)
            {
                mesFilms.Sort();
            }
            else
            {
                string listeDisques = string.Join(",", Configuration.Instance.disqueChoosen);
                log.Error("Aucun film n'a été trouvé dans les repertoires : " + listeDisques);
            }
            TimeSpan dureeExecution = DateTime.Now - startTime;
            log.Info("Fin de l'export de la liste des films, temps d'execution : " + dureeExecution);
        }
        /// <summary>
        /// Méthode qui recherche les fichiers présents dans le repertoire
        /// </summary>
        /// <param name="pRepertoire">Le repertoire dans lequel on veut rechercher</param>
        /// <returns>Retourne "Film+NFO+Img+Acteurs" ou "Film+NFO+Img" ou "Film+NFO" ou "Film" ou null</returns>
        

        private void generateHtml(List<Film> pFilms)
        {
            Html html = new Html(pFilms);
        }
    }
}