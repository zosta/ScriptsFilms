using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using WpfScriptFilms;
using WpfScriptFilms.Model;
using WpfScriptFilms.Model.Util;

using WpfScriptFilms.Vue;

public class Bibliotheque
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private static Bibliotheque instance;
    private List<Film> mesFilms = new List<Film>();
    private string cheminDossierExport;
    private string cheminDossierExportImage;

    private Bibliotheque()
    {
        //on creer le dossier
        cheminDossierExport = Configuration.Instance.emplacementFichierExport + "\\Mes Films";
        createDirectory(cheminDossierExport, true);

        log.Info("Lancement du singleton Bibliotheque");
        //on creer le dossier images
        cheminDossierExportImage = cheminDossierExport + "\\Image";
        createDirectory(cheminDossierExportImage, true);
    }
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
    /**
     * Methode qui retourne la liste des " films avec NFO" ou "films sans NFO"
     * 
     * */
    public List<Film> getFilmsSelonNFO(Boolean pPossedeNFO)
    {
        return mesFilms.Where(f => (f.hasNFO.Equals(pPossedeNFO))).ToList();
    }
    public override string ToString()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("Ma bibliothèque : ");
        foreach (Film film in mesFilms)
        {
            strBuilder.AppendLine(film.ToString());
        }
        return strBuilder.ToString();
    }
    private String getInfos()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("Nombre de films : " + mesFilms.Count);
        //strBuilder.AppendLine("Taille total de la mediatheque: " + getTotalSize());
        //int nbFilmHD = getNbFilmsHD(720, 1080);
        //strBuilder.AppendLine("Nombre de films HD: " + nbFilmHD + " (" + calculerPourcentage(nbFilmHD, mesFilms.Count) + " %)");

        return strBuilder.ToString();
    }
    private int getTotalSize()
    {
        int totalSize =
       (from Film in mesFilms select Film.taille).Sum();
        return totalSize;
    }
    //private int getNbFilmsHD(int pResolutionMin, int pResolutionMax)
    //{
    //    int nbFilmsHD = mesFilms.Count(a => a.resolution >= pResolutionMin && a.resolutionX <= pResolutionMax);
    //    return nbFilmsHD;
    //}
    internal void exporterListeFilms(string pFormat)
    {
        //debut du temps d'execution
        DateTime startTime = DateTime.Now;

        log.Info("Debut de l'export de la liste des films");

        string nomFichier = Configuration.Instance.nomFichierExportFilms + ".txt";

        Configuration.Instance.saveConf();
        foreach (string disque in Configuration.Instance.disqueChoosen)
        {
            string sourceDirectory = disque;
            try
            {
                var repertoires = Directory.EnumerateDirectories(sourceDirectory, "*", SearchOption.TopDirectoryOnly);
                foreach (string repertoire in repertoires)
                {
                    try
                    {
                        if (!repertoire.Contains("$RECYCLE"))
                        {

                            //v1 basé sur le nom du film et les propriétés fichiers

                            //on est dans le dossier du film
                            //foreach (string filmDansRepertoire in FileUtil.extractFilmsOfDirectory(repertoire))
                            //{
                            //    Film film = new Film(filmDansRepertoire);
                            //    mesFilms.Add(film);
                            //}

                            // ici pour la v2, basé sur le nfo
                            foreach (string NfoDansRepertoire in FileUtil.extractNfoOfDirectory(repertoire))
                            {
                                Film film = new Film();
                                // si "{NONFO}"
                                if (NfoDansRepertoire == "{NONFO}")
                                {
                                    // si 0 fichier nfo trouvé on recupere les infos d'une autre façon
                                    //foreach (string filmDansRepertoire in FileUtil.extractFilmsOfDirectory(repertoire))
                                    //{
                                    //    film = new Film(filmDansRepertoire);
                                    //    film.hasNFO = false;
                                    //    log.Info("Les informations du film \"" + film.titre + "\" on été récupéré via ses propriétés fichiers. Les informations sont donc incomplètes.");
                                    //    mesFilms.Add(film);
                                    //}
                                }
                                else
                                {
                                    film = extractFilmFromNfo(NfoDansRepertoire);
                                    if (film != null)
                                    {
                                        film.hasNFO = true;
                                        mesFilms.Add(film);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Une erreur est survenue lors de la creation du film : " + repertoire + " le film a été ignorée", e);
                    }
                }
            }
            catch (UnauthorizedAccessException UAEx)
            {
                log.Error("Le repertoire " + disque + " n'a pas les droits pour acceder à ce repertoire.", UAEx);
            }
            catch (PathTooLongException PathEx)
            {
                log.Error("Le repertoire " + disque + " a un chemin trop long, operation impossible.", PathEx);
            }
            catch (DirectoryNotFoundException DirectoryEx)
            {
                log.Error("Le repertoire " + disque + " n'a pas été trouvé, veuillez le brancher ou modifier la configuration.", DirectoryEx);
            }
        }
        if (mesFilms.Count > 0)
        {
            log.Info("Trie des films par ordre alphabetique en cours.");
            mesFilms.Sort();

            if (pFormat == "txt")
            {
                try
                {
                    string[] films = new string[mesFilms.Count];
                    int i = 0;
                    foreach (Film film in mesFilms)
                    {
                        films[i] = film.toStringInLine();
                    }
                    File.WriteAllLines(Configuration.Instance.emplacementFichierExport + nomFichier, films);
                    //ouvrir fichier
                    Process.Start(Configuration.Instance.emplacementFichierExport + nomFichier);
                }
                catch (DirectoryNotFoundException DirectoryEx)
                {
                    log.Error("Le chemin " + Configuration.Instance.emplacementFichierExport + " n'a pas été trouvé, veuillez modifier la configuration", DirectoryEx);
                }
            }
            else if (pFormat == "html")
            {
                generateHtmlFilms();
            }
            else if (pFormat == "htmlv2")
            {
                generateHtmlFilmsV2();
            }
        }
        else
        {
            log.Error("Aucun film n'a été trouvé.");
        }

        TimeSpan dureeExecution = DateTime.Now - startTime;
        log.Info("Fin de l'export de la liste des films, temps d'execution : " + dureeExecution);

    }
    /**
     * Fonction qui genere un html basé sur le fichier .nfo du dossier 
     * 
     */
    private void generateHtmlFilmsV2()
    {
        StringBuilder sb = new StringBuilder();

        StringBuilder sbPlusInfos = new StringBuilder();

        StringBuilder sbStats = new StringBuilder();

        //une ligne du tableau 
        foreach (Film film in mesFilms)
        {
            //debut de la ligne
            sb.AppendLine("<tr>");

            //Le titre
            sb.AppendLine(string.Format("<td class=\"titre\"><div href=\"{0}\">{1}</div></td>",
                (film.titre.Replace(" ", "_")),
                HttpUtility.HtmlEncode(film.titre)));

            //L'année
            sb.AppendLine(string.Format("<td>{0}</td>",
                HttpUtility.HtmlEncode(film.annee)));

            //La durée
            sb.AppendLine(string.Format("<td>{0}</td>",
                HttpUtility.HtmlEncode(film.duree)));

            //La resolution
            sb.AppendLine(string.Format("<td class=\"resolution-film\">{0}</td>",
                HttpUtility.HtmlEncode(film.resolution)));

            if (film.hasNFO)
            {
                sb.AppendLine(string.Format("<td class=\"hasNFO\">Oui</td>"));
            }
            else
            {
                sb.AppendLine(string.Format("<td class=\"hasNFO\">Non</td>"));
            }



            //fin de la ligne
            sb.AppendLine("</tr>");

            //on remplie le popup plus d'infos
            sbPlusInfos.AppendLine(string.Format("<div class=\"details-film2\" title=\"Détails du film\" id=\"{0}\">", film.titre.Replace(" ", "_")));
            sbPlusInfos.AppendLine(string.Format("<div id = \"definition-img\" class=\"colonne-attribut col1\"><img width=\"200\" height=\"300\"src=\"{0}\"/></div>", film.img));
            sbPlusInfos.AppendLine("<div class=\"col2\">");
            sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label>Titre : </label><div id = \"titre\" class=\"colonne-attribut\">{0}</div></div>", film.titre));
            sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label>Titre original : </label><div id = \"titre_original\" class=\"colonne-attribut\">{0}</div></div>", film.titreOriginal));
            sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label>Année : </label><div id = \"annee\" class=\"colonne-attribut\">{0}</div></div>", film.annee));
            sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label>Durée : </label><div id = \"duree\" class=\"colonne-attribut\">{0}</div></div>", film.duree));
            sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label>Note Moyenne : </label><div id = \"note_moy\" class=\"colonne-attribut\">{0}</div></div>", film.noteMoy));
            sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label>Nombre de votes : </label><div id = \"note_nb\" class=\"colonne-attribut\">{0}</div></div>", film.noteNb));
            sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label>Format : </label><div id = \"extension\" class=\"colonne-attribut\">{0}</div></div>", film.codec));
            sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label>Résolution : </label><div id = \"resolution\" class=\"colonne-attribut\">{0}</div></div>", film.resolution));
            sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label>Langue d'origine : </label><div id=\"original_language\" class=\"colonne-attribut\">{0}</div></div>", ""));
            if (film.genres.Count > 0)
            {
                sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label>Genre : </label><div id = \"genre\" class=\"colonne-attribut\">{0}</div></div>", film.getListGenres()));
            }
            if (film.idTrailer.Count() > 1)
            {
                sbPlusInfos.AppendLine(string.Format("<div class=\"trailer\"><label>Trailer : </label><div><a href = \"http://www.youtube.com/watch?v={0}\"> Voir le trailer </a></div></div>", film.idTrailer));
            }

            // TODO decouper en methodes

            sbPlusInfos.AppendLine("</div>");

            //sbPlusInfos.AppendLine("<div class=\"col3\">");
            //    sbPlusInfos.AppendLine(string.Format("<img src = \"{0}\" id=\"definition-img\">",film.img));
            //sbPlusInfos.AppendLine("</div>");

            sbPlusInfos.AppendLine(string.Format("<div class=\"attribut\"><label class=\"description\">Résumé : </label> <div id = \"description\" class=\"colonne-attribut description\">{0}</div></div>", film.resume));

            //TODO img des acteurs dans le dossier images
            //les acteurs
            //if(film.acteurs.Count()>0)
            //{
            //    sbPlusInfos.AppendLine("<div class=\"acteurs\"><label>Acteurs : </label></div>");

            //    foreach (Acteur act in film.acteurs.OrderBy(f => f.ordre))
            //    {
            //        sbPlusInfos.AppendLine(string.Format("<div class=\"imgacteur\" ><img src=\"{0}\"></div>",act.photo));
            //        sbPlusInfos.AppendLine(string.Format("<div class=\"nomacteur\" >{0}</div>", act.nom));
            //    }
            //    sbPlusInfos.AppendLine(string.Format("</div>"));
            //}

            sbPlusInfos.AppendLine("</div>");

        }


        sbStats.AppendLine(string.Format("<div class=\"stats-films\">{0} films au total</div>", mesFilms.Count()));
        sbStats.AppendLine(string.Format("<div class=\"stats-films\">{0} films au total n'ont pas de NFO</div>", this.getFilmsSelonNFO(false).Count()));

        string fileContent = WpfScriptFilms.Properties.Resources.tableaufilm;
        var reader = new StringReader(fileContent);
        string template = reader.ReadToEnd();
        reader.Close();
        //on a creer le html 
        string nvxHtml = template.Replace("{MES_FILMS}", sb.ToString()).Replace("{INFOS_FILMS}", sbPlusInfos.ToString()).Replace("{STATISTIQUES}", sbStats.ToString());



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
        WriteResourceToFile(WpfScriptFilms.Properties.Resources.style, cheminDossierExportStyle + "\\style.css");
        WriteResourceImgToFile(WpfScriptFilms.Properties.Resources.asc, cheminDossierExportStyle + "\\asc.gif");
        WriteResourceImgToFile(WpfScriptFilms.Properties.Resources.bg, cheminDossierExportStyle + "\\bg.gif");
        WriteResourceImgToFile(WpfScriptFilms.Properties.Resources.desc, cheminDossierExportStyle + "\\desc.gif");


        //on y copie jquery et tablesorter
        WriteResourceToFile(WpfScriptFilms.Properties.Resources.jquery_3_1_1_min, cheminDossierExportScripts + "\\jquery-3.1.1_min.js");

        // File.WriteAllBytes(cheminDossierExportScripts + "\\jquery-3.1.1_min.js", WpfScriptFilms.Properties.Resources.jquery_3_1_1_min);

        WriteResourceToFile(WpfScriptFilms.Properties.Resources.jquery_tablesorter_min, cheminDossierExportScripts + "\\jquery_tablesorter_min.js");

        Process.Start(cheminDossierExportIndex);
    }

    internal void listerFilmsADL()
    {
        log.Info("Lancement de la methode listerFilmsADL");

        log.Info("Lancement de le l'API TheMovieDB");

        //WsFilms wsFilms = new WsFilms();

        for (int annee = DateTime.Now.Year; annee > 2000; annee--)
        {
            log.Info("Recherche des 20 meilleures films de " + annee);
            // List<Film> topFilm =  wsFilms.chercherTopFilmsAsync(annee);

        }

    }

    private int calculerPourcentage(int pNombreACalc, int pNombreTotal)
    {
        int prCent = 0;
        if (pNombreTotal != 0)
        {
            prCent = 100 * pNombreACalc / pNombreTotal;

        }
        else
        {
            // division par 0
            log.Fatal("Division par 0 pour le calcul de pourcentage");
        }
        return prCent;
    }



    internal void creerDossierFilmsSeul()
    {

        afficherProgressBar();

        log.Info("Script de creation des dossiers de films lancé");
        //dans les dossiers configuré, on cherche les films qui ne sont pas dans un dossier 
        foreach (string disque in Configuration.Instance.disqueChoosen)
        {
            log.Info("Analyse du disque " + disque);
            string sourceDirectory = disque;
            int nbDossierActuelle = 0;
            //int nbDossierDeFilm = Directory.GetDirectories(sourceDirectory).Length;
            try
            {
                var repertoires = Directory.GetFiles(sourceDirectory, "*", SearchOption.TopDirectoryOnly);

                foreach (string repertoire in repertoires)
                {
                    if (!repertoire.Contains("$RECYCLE"))
                    {
                        nbDossierActuelle++;
                        try
                        {
                            Film film = new Film(repertoire);
                            string cheminComplet = sourceDirectory + "\\" + film.titre;
                            createDirectory(cheminComplet, false);
                            film.move(cheminComplet);

                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                }

            }
            catch (UnauthorizedAccessException UAEx)
            {
                //ecrireConsole(UAEx.Message, OperationConsole.resetAndAddLine);
            }
            catch (PathTooLongException PathEx)
            {
                //ecrireConsole(PathEx.Message, OperationConsole.resetAndAddLine);
            }
            catch (DirectoryNotFoundException DirectoryEx)
            {
                //ecrireConsole(DirectoryEx.Message, OperationConsole.resetAndAddLine);
                log.Error("Le repertoire " + disque + " n'a pas été trouvé, veuillez le brancher ou modifier la configuration", DirectoryEx);
            }

        }
        log.Info("Script de creation des dossiers de films terminé");
    }

    private void afficherProgressBar()
    {
        ProgressBar progressBar = new ProgressBar();

        progressBar.worker_DoWork(new object(), new DoWorkEventArgs(new object()));

    }

    private string getNameFromPath(string repertoire)
    {
        string fichier = Path.GetFileName(repertoire);
        string pattern = @"\b[at]\w+"; ;
        Regex defaultRegex = new Regex(pattern);
        string nomFilm = repertoire.Replace(repertoire, pattern);
        return nomFilm;
    }

    private void generateHtmlFilms()
    {
        StringBuilder sb = new StringBuilder();
        foreach (Film film in mesFilms)
        {
            //debut de la ligne
            sb.AppendLine("<tr>");

            //Le titre
            sb.AppendLine(string.Format("<td class=\"titre\"><div href=\"{0}\">{1}</div></td>",
                HttpUtility.HtmlEncode(HttpUtility.UrlEncode(film.titre)),
                HttpUtility.HtmlEncode(film.titre)));

            //L'année
            sb.AppendLine(string.Format("<td>{0}</td>",
                HttpUtility.HtmlEncode(film.annee)));

            //La durée
            sb.AppendLine(string.Format("<td>{0}</td>",
                HttpUtility.HtmlEncode(film.duree)));

            //La resolution
            sb.AppendLine(string.Format("<td class=\"resolution-film\">{0}</td>",
                HttpUtility.HtmlEncode(film.resolution)));

            sb.AppendLine(string.Format("<td><a href=\"{0}\"> + d'infos</a></td>",
            HttpUtility.HtmlEncode(HttpUtility.UrlEncode(film.titre))));

            //fin de la ligne
            sb.AppendLine("</tr>");
        }

        string fileContent = WpfScriptFilms.Properties.Resources.tableaufilm;
        var reader = new StringReader(fileContent);
        string template = reader.ReadToEnd();
        reader.Close();
        //on a creer le html 
        string nvxHtml = template.Replace("{MES_FILMS}", sb.ToString());

        //on creer le dossier
        string cheminDossierExport = Configuration.Instance.emplacementFichierExport + "\\Mes Films";
        createDirectory(cheminDossierExport, true);

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
        WriteResourceToFile(WpfScriptFilms.Properties.Resources.style, cheminDossierExportStyle + "\\style.css");
        WriteResourceImgToFile(WpfScriptFilms.Properties.Resources.asc, cheminDossierExportStyle + "\\asc.gif");
        WriteResourceImgToFile(WpfScriptFilms.Properties.Resources.bg, cheminDossierExportStyle + "\\bg.gif");
        WriteResourceImgToFile(WpfScriptFilms.Properties.Resources.desc, cheminDossierExportStyle + "\\desc.gif");


        //on y copie jquery et tablesorter
        WriteResourceToFile(WpfScriptFilms.Properties.Resources.jquery_3_1_1_min, cheminDossierExportScripts + "\\jquery-3.1.1_min.js");

        // File.WriteAllBytes(cheminDossierExportScripts + "\\jquery-3.1.1_min.js", WpfScriptFilms.Properties.Resources.jquery_3_1_1_min);

        WriteResourceToFile(WpfScriptFilms.Properties.Resources.jquery_tablesorter_min, cheminDossierExportScripts + "\\jquery_tablesorter_min.js");

        Process.Start(cheminDossierExportIndex);
    }

    private void WriteResourceImgToFile(Bitmap asc, string pChemin)
    {
        asc.Save(pChemin);
    }

    private void copyFile(string pRessource, string pDestination)
    {
        if (!File.Exists(pDestination))
        {
            string msg = "Erreur lors de la copie du fichier " + pRessource + " vers " + pDestination;
            try
            {
                File.Copy(pRessource, pDestination);
            }
            catch (UnauthorizedAccessException e)
            {
                log.Error(msg, e);
            }
            catch (ArgumentException e)
            {
                log.Error(msg, e);
            }
            catch (PathTooLongException e)
            {
                log.Error(msg, e);
            }
            catch (DirectoryNotFoundException e)
            {
                log.Error(msg, e);
            }
            catch (FileNotFoundException e)
            {
                log.Error(msg, e);
            }
            catch (IOException e)
            {
                log.Error(msg, e);
            }
            catch (NotSupportedException e)
            {
                log.Error(msg, e);
            }
        }
    }

    public void WriteResourceToFile(string resourceName, string fileName)
    {
        try
        {
            log.Info("Copy de fichier :" + fileName);
            File.WriteAllText(fileName, resourceName);
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

    internal string extraireBalise(string pBalise, XmlTextReader reader)
    {
        string content = "";
        try
        {
            reader.ReadToFollowing(pBalise);
            content = reader.ReadElementContentAsString();
            Console.WriteLine("La balise " + pBalise + "  contient: " + content);
        }
        catch (Exception e)
        {
            log.Error("Erreur lors de la lecture de la balise " + pBalise, e);

        }

        return content;
    }


    internal Film extractFilmFromNfo(string nfoDansRepertoire)
    {
        Film film = new Film();



        string imgUrl = getImg(nfoDansRepertoire);
        copyFile(imgUrl, cheminDossierExportImage + "\\" + Path.GetFileName(imgUrl));
        film.img = cheminDossierExportImage + "\\" + Path.GetFileName(imgUrl);
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(nfoDansRepertoire);
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/movie");


            //on cherche les infos du premier fils de movie
            foreach (XmlNode node in nodes)
            {

                film.titre = getInnerText(node.SelectSingleNode("title"));
                film.titreOriginal = getInnerText(node.SelectSingleNode("originaltitle"));



                var duree = TimeSpan.FromMinutes(getInnerInt(node.SelectSingleNode("runtime")));
                film.duree = duree.ToString(@"hh\:mm");

                film.annee = getInnerInt(node.SelectSingleNode("year"));
                film.resume = getInnerText(node.SelectSingleNode("plot"));



                film.pays = getInnerText(node.SelectSingleNode("country"));
                film.studio = getInnerText(node.SelectSingleNode("studio"));
                film.img = getInnerText(node.SelectSingleNode("thumb"));
                film.dateAjout = getInnerText(node.SelectSingleNode("dateadded"));

                try
                {
                    string strTrailer = getInnerText(node.SelectSingleNode("trailer"));
                    // si pas de ttrailer
                    if (!string.IsNullOrEmpty(strTrailer))
                    {
                        Char charRange = '=';
                        int endIndex = strTrailer.LastIndexOf(charRange);
                        film.idTrailer = strTrailer.Substring(endIndex);
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    log.Info("Pas de trailer pour ce film", e);
                }


                foreach (XmlNode nodeGenre in node.SelectNodes("genre"))
                {
                    film.genres.Add(nodeGenre.InnerText);
                }


            }
            //on cherche les  infos fils de rating 
            XmlNodeList nodeRatings = doc.DocumentElement.SelectNodes("/movie/ratings/rating");
            foreach (XmlNode nodeRating in nodeRatings)
            {
                try
                {
                    string noteMoy = nodeRating.SelectSingleNode("value").InnerText;
                    film.noteNb = getInnerText(nodeRating.SelectSingleNode("votes"));
                    if (noteMoy.Count() > 4)
                    {
                        film.noteMoy = noteMoy.Substring(0, 3);
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    log.Info("Pas de notes pour ce film", e);
                }
            }

            //on cherche les infos fils de fanart 

            //on cherche les infos fils de fileinfo
            XmlNodeList nodeFilsInfos = doc.DocumentElement.SelectNodes("/movie/fileinfo/streamdetails/video");
            foreach (XmlNode nodeFileInfo in nodeFilsInfos)
            {
                film.codec = nodeFileInfo.SelectSingleNode("codec").InnerText;
                film.width = nodeFileInfo.SelectSingleNode("width").InnerText;
                film.height = nodeFileInfo.SelectSingleNode("height").InnerText;
                film.resolution = film.width + "x" + film.height;
            }
            //on cherche les infos actors
            XmlNodeList nodeActorInfos = doc.DocumentElement.SelectNodes("/movie/actor");
            foreach (XmlNode nodeActorInfo in nodeActorInfos)
            {
                Acteur act = new Acteur(getInnerText(nodeActorInfo.SelectSingleNode("name")), getInnerText(nodeActorInfo.SelectSingleNode("thumb")), getInnerInt(nodeActorInfo.SelectSingleNode("order")));
                film.acteurs.Add(act);
            }

            return film;
        }
        catch (NullReferenceException e)
        {
            log.Error("Problème lors de la lecture du NFO.", e);
            return film;
        }
    }

    private string getImg(string nfoDansRepertoire)
    {
        string img = nfoDansRepertoire.Replace(".nfo", "-poster.jpg");
        return img;
    }

    private int getInnerInt(XmlNode xmlNode)
    {
        int i = 0;
        if (xmlNode != null)
        {
            i = Int16.Parse(xmlNode.InnerText);
        }
        else
        {
            log.Info("La balise " + xmlNode + " n'a pas été trouvé dans ce NFO.");
        }
        return i;
    }

    private string getInnerText(XmlNode xmlNode)
    {
        string str = "";
        if (xmlNode != null)
        {
            str = xmlNode.InnerText;
        }
        else
        {
            log.Info("Une balise n'a pas été trouvé dans ce NFO.");
        }
        return str;
    }
}