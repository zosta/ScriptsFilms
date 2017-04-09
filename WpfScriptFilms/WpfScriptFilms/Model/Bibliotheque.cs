using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WpfScriptFilms;
using WpfScriptFilms.Model;

public class Bibliotheque
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private static Bibliotheque instance;
    private List<Film> mesFilms = new List<Film>();

    private Bibliotheque()
    {
        log.Info("Lancement du singleton Bibliotheque");
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
        strBuilder.AppendLine("Nombre de films HD: " + getNbFilmsHD() + " (" + calculerPourcentage(getNbFilmsHD(), mesFilms.Count) + " %)");

        return strBuilder.ToString();
    }
    //private int getTotalSize()
    //{
    //    int totalSize =
    //   (from Film in mesFilms select Film.taille).Sum();
    //    return totalSize;
    //}
    private int getNbFilmsHD()
    {
        int nbFilmsHD = 0;
        return nbFilmsHD;
    }

    internal void exporterListeFilms()
    {
        string nomFichier = Configuration.Instance.nomFichierExportFilms + ".txt";

        foreach (string disque in Configuration.Instance.disqueChoosen)
        {
            string sourceDirectory = disque;
            int nbDossierActuelle = 0;
            //int nbDossierDeFilm = Directory.GetDirectories(sourceDirectory).Length;
            try
            {
                var repertoires = Directory.EnumerateDirectories(sourceDirectory, "*", SearchOption.TopDirectoryOnly);

                foreach (string repertoire in repertoires)
                {
                    if (!repertoire.Contains("$RECYCLE"))
                    {
                        nbDossierActuelle++;
                        Film film = new Film(repertoire);
                        mesFilms.Add(film);
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
        if (mesFilms.Count > 0)
        {
            log.Info("Trie des films par ordre alphabetique en cours.");
            mesFilms.Sort();

            try
            {
                string[] films = new string[mesFilms.Count];
                int i = 0;
                foreach (Film film in mesFilms)
                {
                    films[i] = film.ToString();

                }
                File.WriteAllLines(Configuration.Instance.emplacementFichierExport + nomFichier, films);
                //ouvrir fichier
                Process.Start(Configuration.Instance.emplacementFichierExport + nomFichier);
                //ecrireConsole(MsgConst.exportFilmTermine, OperationConsole.addLine);
            }
            catch (DirectoryNotFoundException DirectoryEx)
            {
                log.Error("Le chemin " + Configuration.Instance.emplacementFichierExport + " n'a pas été trouvé, veuillez modifier la configuration", DirectoryEx);
                //ecrireConsole("Le chemin " + Configuration.Instance.emplacementFichierExport + " n'a pas été trouvé, veuillez modifier la configuration", OperationConsole.addLine);
            }
        }
        else
        {
            log.Error("Aucun film n'a été trouvé.");
        }
        //ici methodes pour afficher les informations


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

                            // on verifie si le dossier n'existe pas deja  
                            if (Directory.Exists(repertoire))
                            {
                                log.Info("Le dossier " + repertoire + " exise déjà.");
                                return;
                            }

                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(sourceDirectory + "\\" + film.titre);

                            log.Info("Le repertoire " + film.titre + " a été crée avec succès");

                            film.move(di.FullName);
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

    private string getNameFromPath(string repertoire)
    {
        string fichier = Path.GetFileName(repertoire);
        string pattern = @"\b[at]\w+"; ;
        Regex defaultRegex = new Regex(pattern);
        string nomFilm = repertoire.Replace(repertoire, pattern);
        return nomFilm;
    }

}