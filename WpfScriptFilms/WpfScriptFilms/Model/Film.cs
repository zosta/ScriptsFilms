using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using WpfScriptFilms.Model;

public class Film
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public string titre { get; }
    public string chemin { get; }
    public string extension { get; }
    public int taille { get; }
    private int duree;
    private int annee;

    public Film()
    {
        log.Info("Lancement de la classe Film");
    }

    public Film(string pChemin)
    {
 
        this.chemin = pChemin;
        this.titre = getNameFromChemin(pChemin);
        this.extension = getExtensionFromChemin(pChemin);
        this.annee = getYearFromChemin(pChemin);
        this.taille = getTailleFromChemin(pChemin);
        log.Info(this.ToString());

    }

    private string getExtensionFromChemin(string pChemin)
    {
        return Path.GetExtension(pChemin);
    }

    private int getTailleFromChemin(string pChemin)
    {
        return (int)new FileInfo(pChemin).Length/1024/1024;
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
        catch(FormatException e)
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
        if(!isFilm)
        {
            log.Info(cheminSansExtensions + " n'a pas le format attendu, le fichier sera ignoré");
            throw new Exception();
        }
        return titre;
    }

    public override string ToString()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine ("Titre : " + titre);
        strBuilder.AppendLine("Taille : " + taille + " Mo");
        strBuilder.AppendLine("Duree : " + duree);
        strBuilder.AppendLine("Annee : " + annee);

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
                if(File.Exists(pChemin))
                {
                    log.Info("Le fichier " + nvxCheminFichier + " existe déjà");
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
}
