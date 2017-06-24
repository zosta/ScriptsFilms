public class Acteur
{
    private int v;

    public string nom { get;  }
    public string photo { get; }
    public int ordre { get; }

    public Acteur(string pNom,string pPhoto, int pOrdre)
    {
        this.nom = pNom;
        this.photo = pPhoto;
        this.ordre = pOrdre;
    }

    public override string ToString()
    {
        return ordre+") Nom : " + nom + ", URLPhoto : " + photo ;
    }

}