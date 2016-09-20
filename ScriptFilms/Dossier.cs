using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptFilms
{
    class Dossier
    {
        public void creerDossierDesFichiers()
        {
            Console.Clear();
            Console.WriteLine("Creer le dossier associé");
            Console.WriteLine();
            Console.WriteLine("En cours...");

            Boolean isActive = true;
            string[] PathFileNames;
            PathFileNames = Directory.GetFiles(Config.sourceDirectory, "*(????)*", SearchOption.TopDirectoryOnly);
            int nbDossierDeFilm = PathFileNames.Length;
            try
            {

                int nbDossierActuelle = 0;
                
                foreach (string nom in PathFileNames)
                {
                    nbDossierActuelle++;
                    Util.afficherPourcentage(nbDossierActuelle, nbDossierDeFilm);

                    if (isActive)
                    {
                        Directory.CreateDirectory(nom.Split('(')[0]);
                        string sourceFile = nom;
                        string destinationFile = nom.Split('(')[0] + nom.Replace(Config.sourceDirectory, "");
                        // To move a file or folder to a new location:
                        string dossierDestination = nom.Split('(')[0] + @"\";
                        int nbFichierDansDossier = Directory.GetFiles(dossierDestination, "*.*", SearchOption.TopDirectoryOnly).Length;

                        if (File.Exists(destinationFile))
                        {
                            Console.WriteLine("le dossier " + dossierDestination + " existe déjà , il y a déjà " + nbFichierDansDossier + " fichiers dans ce dossier");

                            // on increment le nom 
                            

                            string extension = Path.GetExtension(destinationFile);

                            string nvxNom = destinationFile.Replace(extension, " v" + (nbFichierDansDossier + 1) + extension);

                            File.Move(sourceFile, nvxNom);

                            Process.Start(dossierDestination);
                        }
                        else
                        {
                            File.Move(sourceFile, destinationFile);
                        }
                    }
                }
                Util.OperationTermine();
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Console.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Console.WriteLine(PathEx.Message);
            }
            Util.OperationTermine();
            Console.WriteLine(nbDossierDeFilm + " fichiers traités");
            Console.ReadLine();
            Menu.Instance.afficher();
        }


        public void chercherDoublonsDansDossier()
        {
            Console.Clear();
            Console.WriteLine("Chercher les doublons dans les dossiers et les dossiers vides");
            Console.WriteLine();
            Console.WriteLine("En cours...");
           
            int nbDossierDeFilm = Directory.GetDirectories(Config.sourceDirectory).Length;
            int nbDossierActuelle = 0;
            try
            {
                var repertoires = Directory.EnumerateDirectories(Config.sourceDirectory, "*", SearchOption.TopDirectoryOnly);

                foreach (string repertoire in repertoires)
                {
                    nbDossierActuelle++;
                    int nbFichier = Directory.GetFiles(repertoire, "*", SearchOption.TopDirectoryOnly).Length;

                    if (nbFichier > 1)
                    {
                        foreach(string fichier in Directory.GetFiles(repertoire))
                        {
                            if(!fichier.Contains("CD"))
                            {
                                Process.Start(repertoire);
                            }
                        }
                       
                    }
                    else if(nbFichier <1)
                    {
                        Process.Start(repertoire);
                    }
                    Util.afficherPourcentage(nbDossierActuelle, nbDossierDeFilm);
                }
                Util.OperationTermine();
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Console.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Console.WriteLine(PathEx.Message);
            }
            Console.ReadLine();
            Menu.Instance.afficher();
        }
    }
}
