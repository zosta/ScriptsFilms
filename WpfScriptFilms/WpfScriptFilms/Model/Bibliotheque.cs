using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Bibliotheque
{
    private static Bibliotheque instance;

    private Bibliotheque() { }

    private List<Film> mesFilms;

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
        strBuilder.AppendLine("Taille total de la mediatheque: " + getTotalSize());
        strBuilder.AppendLine("Nombre de films HD: " + getNbFilmsHD() + " (" + calculerPourcentage(getNbFilmsHD(), mesFilms.count()) + " %)");

        return strBuilder.ToString();
    }
    private int getTotalSize()
    {
        int totalSize =
       (from Film in mesFilms select Film.taille).Sum();
        return totalSize;
    }
    private int getNbFilmsHD()
    {
        int nbFilmsHD = 0;
        return nbFilmsHD;
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

        }
        return prCent;
    }
}