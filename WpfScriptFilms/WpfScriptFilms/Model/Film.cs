using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class Film
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public string titre { get; }
    public string chemin { get; }
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
        this.annee = getYearFromChemin(pChemin);
        log.Info(this.ToString());
    }

    private int getYearFromChemin(string pChemin)
    {
        string annee = "";
        int anneeInt = 0;
        Regex regex = new Regex(@"\(([^(19|20)\d{2}$]*)\)");
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
        Regex regex = new Regex(@"\(([^(19|20)\d{2}$]*)\)");
        try
        {
            titre = regex.Replace(pChemin, "");
        }
        catch (ArgumentNullException e)
        {
            log.Error("Le chemin dont on veut trouver le titre est null", e);
        }
        return titre;
    }

    public override string ToString()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine ("Titre : " + titre);
        strBuilder.AppendLine("Taille : " + taille);
        strBuilder.AppendLine("Duree : " + duree);
        strBuilder.AppendLine("Annee : " + annee);

        return strBuilder.ToString();
    }
}
