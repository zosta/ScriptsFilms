using MediaInfoLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using WpfScriptFilms.Model;
using WpfScriptFilms.Model.Util;

public class Film : IComparable
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public string titre { get; set; }
    public string chemin { get; set; }
    public string extension { get; set; }
    public int taille { get; set; }
    public string duree { get; set; }
    public int annee { get; set; }
    public string resolution { get; set; }
    private readonly MediaInfo mediaInfo;
    internal string height;

    public string titreOriginal { get; set; }
    public string noteMoy { get; set; }
    public string noteNb { get; set; }
    public string resume { get; set; }
    public string img { get; set; }
    public string pays { get; set; }
    public List<string> langues { get; set; }
    public List<string> soustitres { get; set; }
    public List<Acteur> acteurs { get; set; }
    public string studio { get; set; }
    public List<string> genres { get; set; }
    public string width { get; internal set; }
    public string codec { get; internal set; }

    public Boolean hasNFO { get; internal set; }
    public string dateAjout { get; internal set; }
    public string idTrailer { get; set; }

    public Film()
    {
        toutANull();
    }

    private void toutANull()
    {
        acteurs = new List<Acteur>();
        langues = new List<string>();
        soustitres = new List<string>() {  };
        genres = new List<string>() {  };
        titre = "";
        chemin = "";
        extension = "";
        taille = 0;
        duree = "";
        annee = 0;
        resolution = "";
        height = "";
        titreOriginal = "";
        noteMoy = "";
        noteNb = "";
        resume = "";
        img = "";
        pays = "";
        studio = "";
        width = "";
        codec = "";
        hasNFO = false;
        idTrailer = "";
    }


    public Film(string pChemin)
    {
        log.Info("Lancement de la classe Film");
        toutANull();
        mediaInfo = new MediaInfo();
        mediaInfo.Open(pChemin);
        mediaInfo.Option("Complete");

        getInfoVideo();


        this.chemin = pChemin;
        this.titre = getNameFromChemin(pChemin);
        this.extension = getExtensionFromChemin(pChemin);
        this.annee = getYearFromChemin(pChemin);
        this.taille = getTailleFromChemin(pChemin);
        this.resolution = getResolution(pChemin);
        //this.duree =getDureeFromChemin(pChemin);

        //log.Info(this.ToString());

    }

    private TimeSpan getDureeFromChemin(string pChemin)
    {
        TimeSpan tps = TimeSpan.Zero;

        tps = TimeUtil.msToTimeSpan(mediaInfo.Get(StreamKind.Video, 0, "Duration"));

        return tps;
    }

    private string getResolution(string pChemin)
    {
        string sizeX = mediaInfo.Get(StreamKind.Video, 0, "Width");
        string sizeY = mediaInfo.Get(StreamKind.Video, 0, "Height");
        string resolution = sizeX + "x" + sizeY;
        return resolution;
    }

    private string getExtensionFromChemin(string pChemin)
    {
        return Path.GetExtension(pChemin);
    }

    private int getTailleFromChemin(string pChemin)
    {
        return (int)new FileInfo(pChemin).Length / 1024 / 1024;
    }

    private void getInfoVideo()
    {

        mediaInfo.Option("Complete");

        string info = mediaInfo.Inform();

        log.Info(info);

        //mediaInfo.Close();

    }

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

    public override string ToString()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("Titre : " + titre);
        strBuilder.AppendLine("Taille : " + taille + " Mo");
        // strBuilder.AppendLine("Duree : " + TimeUtil.formatTimeSpan(duree));
        strBuilder.AppendLine("Annee : " + annee);
        strBuilder.AppendLine("Chemin : " + chemin);
        strBuilder.AppendLine("Resolution : " + resolution);
        strBuilder.AppendLine("Extension : " + extension);

        return strBuilder.ToString();
    }

    public string toStringInLine()
    {
        string separateur = " | ";
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.Append("Titre : " + titre);
        strBuilder.Append(separateur);
        strBuilder.Append("Annee : " + annee);
        strBuilder.Append(separateur);
        // strBuilder.Append("Duree : " + TimeUtil.formatTimeSpan(duree));
        strBuilder.Append(separateur);
        strBuilder.Append("Resolution : " + resolution);
        return strBuilder.ToString();
    }

    internal void move(string pChemin)
    {
        try
        {
            if (!File.Exists(chemin))
            {
                // This statement ensures that the file is created,
                // but the handle is not kept.
                using (FileStream fs = File.Create(chemin)) { }
            }

            string nvxCheminFichier = pChemin + "\\" + titre + " (" + annee + ")" + extension;
            // Ensure that the target does not exist.
            if (Directory.Exists(pChemin))
            {
                log.Info("Le dossier " + pChemin + " existe déjà");
                if (File.Exists(pChemin))
                {
                    log.Info("Le fichier " + nvxCheminFichier + " existe déjà");

                    int fileCount = Directory.GetFiles(pChemin).Length + 1;

                    nvxCheminFichier = pChemin + "\\" + titre + " (" + annee + ")_TMP" + fileCount + extension;

                    File.Move(chemin, nvxCheminFichier);

                    Process.Start("explorer.exe", nvxCheminFichier);

                    log.Info("Le fichier temporaire" + nvxCheminFichier + " a été creer");

                }
                else
                {
                    File.Move(chemin, nvxCheminFichier);
                    log.InfoFormat("{0} a été déplacé, son nouvel emplacement est : {1}.", chemin, pChemin);
                }
            }
            else
            {
                File.Move(chemin, nvxCheminFichier);
                log.InfoFormat("{0} a été déplacé, son nouvel emplacement est : {1}.", chemin, pChemin);
            }

            // See if the original exists now.
            if (File.Exists(chemin))
            {
                log.Warn("Le fichier original existe toujours ce qui n'est pas conforme à ce qui est attendu.");
            }
            else
            {
                log.Info("Le fichier original n'existe plus ce qui est conforme à ce qui est attendu.");
            }

        }
        catch (Exception e)
        {
            log.Error("Le processus de deplacement de fichier a échoué.", e);
        }
    }

    public int CompareTo(object obj)
    {
        Film compare = (Film)obj;
        return titre.CompareTo(compare.titre);
    }

    internal object getListGenres()
    {
        StringBuilder sb = new StringBuilder();
        int i = 1;
        foreach(string genre in genres)
        {
            if(genres.Count>1 && i<genres.Count)
            { 
                sb.Append(genre+", ");
            }
            else
            {
                sb.Append(genre);
            }
            i++;
        }
        return sb.ToString();
    }
}
