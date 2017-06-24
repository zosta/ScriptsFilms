using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace FilmApp.Model
{
    class Html
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        StringBuilder sbTableau;

        StringBuilder sbPopup;

        StringBuilder sbActeurs;


        StringBuilder sbStats;

        /// <summary>
        /// Le constructeur, il va appeller les autres méthodes pour generer le html
        /// </summary>
        /// <param name="pFilms">La liste des films</param>
        public Html(List<Film> pFilms)
        {
            sbStats = new StringBuilder();
            sbPopup = new StringBuilder();
            sbTableau = new StringBuilder();


            //une ligne du tableau 
            foreach (Film film in pFilms)
            {
                remplirTableau(film);
                remplirPopup(film);

            }
            remplirStats(pFilms);
            genererFichier();
        }

        private void genererFichier()
        {
            string fileContent = Properties.Resources.tableaufilm;
            var reader = new StringReader(fileContent);
            string template = reader.ReadToEnd();
            reader.Close();

            //on a creer le html 
            string nvxHtml = template.Replace("{MES_FILMS}", sbTableau.ToString()).Replace("{INFOS_FILMS}", sbPopup.ToString()).Replace("{STATISTIQUES}", sbStats.ToString());
            string cheminDossierExport = Configuration.Instance.emplacementFichierExportFilms;

            //on y place le fichier html 
            string cheminDossierExportIndex = cheminDossierExport + "\\index.html";
            StreamWriter fichierHtml = File.CreateText(cheminDossierExportIndex);
            fichierHtml.Write(nvxHtml);
            fichierHtml.Close();

            //on creer le dossier scripts
            string cheminDossierExportScripts = cheminDossierExport + "\\Scripts";
            createDirectory(cheminDossierExportScripts, true);

            //on creer le dossier scripts
            string cheminDossierExportStyle = cheminDossierExport + "\\Style";
            createDirectory(cheminDossierExportStyle, true);

            //on y copie les fichiers en rapport avec le style
            WriteResourceToFile(Properties.Resources.style, cheminDossierExportStyle + "\\style.css");
            WriteResourceImgToFile(Properties.Resources.asc, cheminDossierExportStyle + "\\asc.gif");
            WriteResourceImgToFile(Properties.Resources.bg, cheminDossierExportStyle + "\\bg.gif");
            WriteResourceImgToFile(Properties.Resources.desc, cheminDossierExportStyle + "\\desc.gif");

            //on y copie jquery et tablesorter
            WriteResourceToFile(Properties.Resources.jquery_3_1_1_min, cheminDossierExportScripts + "\\jquery-3.1.1_min.js");
            WriteResourceToFile(Properties.Resources.jquery_tablesorter_min, cheminDossierExportScripts + "\\jquery_tablesorter_min.js");

            Process.Start(cheminDossierExportIndex);
        }

        /// <summary>
        /// Copie une image depuis ressources à dossier local
        /// </summary>
        /// <param name="pImage">L'image a copier</param>
        /// <param name="pChemin">Le chemin ou on va la copier</param>
        private void WriteResourceImgToFile(Bitmap pImage, string pChemin)
        {
            pImage.Save(pChemin);
        }

        /// <summary>
        /// Copie une ressouce vers un dossier local
        /// </summary>
        /// <param name="pRessourceName">Le fichier a copier</param>
        /// <param name="pFileName">Le chemin ou on va la copier</param>
        private void WriteResourceToFile(string pRessourceName, string pFileName)
        {
            try
            {
                log.Info("Copy de fichier :" + pFileName);
                File.WriteAllText(pFileName, pRessourceName);
            }
            catch (ArgumentException e)
            {

                log.Error("path est une chaîne de longueur nulle, contient seulement un espace, ou contient" +
                "un ou plusieurs caractères non valides définis par System.IO.Path.InvalidPathChars.", e);

            }
            catch (PathTooLongException e)
            {
                log.Error("Le chemin et / ou le nom de fichier spécifiés dépassent la longueur maximale définie" +
                     "par le système.Par exemple, sur les plateformes Windows, les chemins ne doivent" +
                     "pas dépasser 248 caractères et les noms de fichiers ne doivent pas dépasser 260" +
                     "caractères.", e);

            }
            catch (DirectoryNotFoundException e)
            {
                log.Error("Le chemin spécifié n’est pas valide(par exemple, il est sur un lecteur non mappé).", e);

            }
            catch (IOException e)
            {
                log.Error("Une erreur d’E / S s’est produite à l’ouverture du fichier.", e);

            }
            catch (UnauthorizedAccessException e)
            {
                log.Error("path a spécifié un fichier en lecture seule.ou Cette opération n'est pas prise" +
                     "en charge sur la plateforme actuelle.ou path a spécifié un répertoire.ou L'appelant" +
                     "n'a pas l'autorisation requise.", e);
            }
            catch (NotSupportedException e)
            {
                log.Error("path a un format non valide.", e);

            }
            catch (SecurityException e)
            {
                log.Error("L'appelant n'a pas l'autorisation requise.", e);
            }
        }

        /// <summary>
        /// Méthode qui créer un repertoire
        /// </summary>
        /// <param name="pChemin">le repertoire à créer</param>
        /// <param name="pEcraserSiExiste">Si le fichier existe faut il l'écraser ?</param>
        private void createDirectory(string pChemin, bool pEcraserSiExiste)
        {
            // on verifie si le dossier n'existe pas deja  
            if (Directory.Exists(pChemin) && !pEcraserSiExiste)
            {
                log.Info("Le dossier " + pChemin + " exise déjà.");
                return;
            }

            // Try to create the directory.
            DirectoryInfo di = Directory.CreateDirectory(pChemin);

            log.Info("Le repertoire " + pChemin + " a été crée avec succès");
        }
        /// <summary>
        /// Remplis la div statistiques
        /// </summary>
        /// <param name="pFilms">La listes des films</param>
        private void remplirStats(List<Film> pFilms)
        {
            // nombre de films
            sbStats.AppendLine(string.Format("<div class=\"stats-films\">{0} films au total</div>", pFilms.Count()));
            // TODO sbStats.AppendLine(string.Format("<div class=\"stats-films\">{0} films au total n'ont pas de NFO</div>", this.getFilmsSelonNFO(false).Count()));

            //nombre de films en HD

        }

        private void remplirPopup(Film film)
        {
            
            //on remplie le popup plus d'infos
            if(!string.IsNullOrEmpty(film.titre))
            { 
                sbPopup.AppendLine(string.Format("<div class=\"details-film2\" title=\"Détails du film\" id=\"{0}\">", film.titre.Replace(" ", "_")));
            }
            // si on aaucun NFO alors on a peut être ces infos
            if (!string.IsNullOrEmpty(film.urlPoster))
            {
                sbPopup.AppendLine(string.Format("<div id = \"definition-img\" class=\"colonne-attribut col1\"><img width=\"200\" height=\"300\"src=\"{0}\"/></div>", film.urlPoster));
            }

            sbPopup.AppendLine("<div class=\"col2\">");

            if (!string.IsNullOrEmpty(film.titre))
            {
                sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label>Titre : </label><div id = \"titre\" class=\"colonne-attribut\">{0}</div></div>", film.titre));
            }
            if (film.annee>0)
            {
                sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label>Année : </label><div id = \"annee\" class=\"colonne-attribut\">{0}</div></div>", film.annee));
            }
            if (film.duree != null)
            {
                sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label>Durée : </label><div id = \"duree\" class=\"colonne-attribut\">{0}</div></div>", film.duree.ToString(@"hh:ss")));
            }
            if (!string.IsNullOrEmpty(film.codec))
            {
                sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label>Format : </label><div id = \"extension\" class=\"colonne-attribut\">{0}</div></div>", film.codec));
            }
            if (film.resolution != null && film.resolution.Count()==1)
            {
                sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label>Résolution : </label><div id = \"resolution\" class=\"colonne-attribut\">{0}</div></div>", film.resolution[0] + "x"+ film.resolution[1]));
            }

            if(film.nfo != null)
            {
                remplirInfosIssusNfo(film);
            }


            // TODO decouper en methodes

            sbPopup.AppendLine("</div>");

            //sbPlusInfos.AppendLine("<div class=\"col3\">");
            //    sbPlusInfos.AppendLine(string.Format("<img src = \"{0}\" id=\"definition-img\">",film.img));
            //sbPlusInfos.AppendLine("</div>");
            if (film.nfo != null)
            {
                if (!string.IsNullOrEmpty(film.resume))
                {
                    sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label class=\"description\">Résumé : </label> <div id = \"description\" class=\"colonne-attribut description\">{0}</div></div>", film.resume));
                }

                
                //TODO img des acteurs dans le dossier images
                //les acteurs
                if (film.acteurs.Count() > 0)
                {
                    remplirActeurs(film.acteurs);                    
                }
            }
            //TODO : methode pour ajuster ordre des lignes 
            sbPopup.AppendLine("</div>");
        }
        /// <summary>
        /// Rempli les informations issu de NFO
        /// </summary>
        /// <param name="film">Le film dont on veut les informations</param>
        private void remplirInfosIssusNfo(Film film)
        {
            if (!string.IsNullOrEmpty(film.titreOriginal))
            {
                sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label>Titre original : </label><div id = \"titre_original\" class=\"colonne-attribut\">{0}</div></div>", film.titreOriginal));
            }

            if (!string.IsNullOrEmpty(film.noteMoy))
            {
                sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label>Note Moyenne : </label><div id = \"note_moy\" class=\"colonne-attribut\">{0}</div></div>", film.noteMoy));
            }
            if (!string.IsNullOrEmpty(film.noteNb))
            {
                sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label>Nombre de votes : </label><div id = \"note_nb\" class=\"colonne-attribut\">{0}</div></div>", film.noteNb));
            }
            if (!string.IsNullOrEmpty(film.langueOrigine))
            {
                sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label>Langue d'origine : </label><div id=\"original_language\" class=\"colonne-attribut\">{0}</div></div>", ""));
            }
            if (film.genres != null && film.genres.Count() > 0)
            {
              //  sbPopup.AppendLine(string.Format("<div class=\"attribut\"><label>Genre : </label><div id = \"genre\" class=\"colonne-attribut\">{0}</div></div>", film.getListGenres()));
            }
        }

        /// <summary>
        /// Methode pour remplir la div concernant les acteurs 
        /// </summary>
        /// <param name="acteurs"></param>
        private void remplirActeurs(List<Acteur> pActeurs)
        {
            sbActeurs.AppendLine("<div class=\"acteurs\"><label>Acteurs : </label></div>");
            foreach (Acteur act in pActeurs.OrderBy(f => f.ordre))
            {
                sbActeurs.AppendLine(string.Format("<div class=\"imgacteur\" ><img src=\"{0}\"></div>", act.urlImgActeur));
                sbActeurs.AppendLine(string.Format("<div class=\"nomacteur\" >{0}</div>", act.nom));
            }
            sbActeurs.AppendLine(string.Format("</div>"));
        }

        private void remplirTableau(Film film)
        {
            //debut de la ligne
            sbTableau.AppendLine("<tr>");

            //Le titre
            sbTableau.AppendLine(string.Format("<td class=\"titre\"><div href=\"{0}\">{1}</div></td>",
                (film.titre.Replace(" ", "_")), film.titre));

            //L'année
            sbTableau.AppendLine(string.Format("<td>{0}</td>", film.annee));

            //La durée
            //sb.AppendLine(string.Format("<td>{0}</td>", film.duree));

            //La resolution
            //sb.AppendLine(string.Format("<td class=\"resolution-film\">{0}</td>", film.resolution));

            if (film.nfo != null)
            {
                sbTableau.AppendLine(string.Format("<td class=\"hasNFO\">Oui</td>"));
            }
            else
            {
                sbTableau.AppendLine(string.Format("<td class=\"hasNFO\">Non</td>"));
            }
            //fin de la ligne
            sbTableau.AppendLine("</tr>");
        }


    }

}