using System;
using System.Text;



public class Film
{
    private String titre;
    public int taille { get; }
    private int duree;
    private int annee;

    public Film()
    {

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
