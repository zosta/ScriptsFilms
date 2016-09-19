using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptFilms
{
    class Program
    {
        private const string sourceDirectory = @"H:\Videos\Films";
        private static Util util;

        static void Main(string[] args)
        {
             menuAccueil();
        }

        private static void menuAccueil()
        {
            Console.Clear();
            util = new Util();
            int userInput = 0;

            userInput = choixMenu();

            if (userInput == 1)
            {
                creerDossierDesFichiers();
            }
            else if (userInput == 2)
            {
                chercherDoublonsDansDossier();
            }
            else if (userInput == 3)
            {
                exporterListeFilms();
            }
            else if (userInput == 4)
            {
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Ce choix n'est pas disponible, appuyer sur une touche pour revenir au menu d'accueil");
                Console.ReadLine();
                menuAccueil();
            }
        }

        private static void exporterListeFilms()
        {

            const string nomFichier = @"MesFilms.txt"; 

            Console.Clear();
            Console.WriteLine("Exporter la liste des films");
            Console.WriteLine();
            Console.WriteLine("En cours...");


            int nbDossierDeFilm = Directory.GetDirectories(sourceDirectory).Length;
            int nbDossierActuelle = 0;
            try
            {
                var repertoires = Directory.EnumerateDirectories(sourceDirectory, "*", SearchOption.TopDirectoryOnly);

                List<string> liFilms = new List<string>();
                foreach (string repertoire in repertoires)
                {
                    nbDossierActuelle++;
                    liFilms.Add(repertoire.Replace(sourceDirectory, ""));
                    Util.afficherPourcentage(nbDossierActuelle, nbDossierDeFilm);
                }

                liFilms.Sort();

                liFilms.Add("==========================");
                liFilms.Add("Nombre total de films : " + nbDossierDeFilm);
                File.WriteAllLines(sourceDirectory + nomFichier, liFilms);
                //ouvrir fichier
                Process.Start(sourceDirectory + nomFichier);
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
            menuAccueil();
        }

        private static void chercherDoublonsDansDossier()
        {
            // On parcours le dossier choisis

            // On prend les fichiers qui ont comme nom une date entre parenthèse

            // On cherche le dossier qui a comme nom le film sans la parenthèse, si il existe on colle le fichier dedans, si dans le dossier il y a déjà un fichier qui porte le nom+extension alors on renomme le fichiers

            

            Console.Clear();
            Console.WriteLine("Chercher les doublons dans les dossiers");
            Console.WriteLine();
            Console.WriteLine("En cours...");

           

            int nbDossierDeFilm =Directory.GetDirectories(sourceDirectory).Length;
            int nbDossierActuelle = 0;
            try
            {
                var repertoires = Directory.EnumerateDirectories(sourceDirectory, "*", SearchOption.TopDirectoryOnly);

                foreach (string repertoire in repertoires)
                {
                    nbDossierActuelle++;
                    int nbFichier = Directory.GetFiles(repertoire, "*", SearchOption.TopDirectoryOnly).Length;

                    if(nbFichier > 1)
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
            menuAccueil();
        }

        private static void creerDossierDesFichiers()
        {
            Console.Clear();
            Console.WriteLine("Creer le dossier associé");
            Console.WriteLine();
            Console.WriteLine("En cours...");

            Boolean isActive = true;

            try
            {
                string[] PathFileNames;
                PathFileNames = Directory.GetFiles(sourceDirectory, "*(????)*", SearchOption.TopDirectoryOnly);
                int nbDossierActuelle = 0;
                int nbDossierDeFilm = Directory.GetDirectories(sourceDirectory).Length;
                foreach (string nom in PathFileNames)
                {
                    nbDossierActuelle++;
                    Util.afficherPourcentage(nbDossierActuelle, nbDossierDeFilm);

                    if (isActive)
                    {
                        Directory.CreateDirectory(nom.Split('(')[0]);
                        string sourceFile = nom;
                        string destinationFile = nom.Split('(')[0] + nom.Replace(@"H:\Videos\Films", "");
                        // To move a file or folder to a new location:
                        if (File.Exists(destinationFile))
                        {
                            Console.WriteLine("le dossier " + destinationFile + " existe déjà");
                        }
                        else
                        {
                            File.Move(sourceFile, destinationFile);
                        }
                        Console.WriteLine("OK");
                    }

                }
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
            menuAccueil();
        }

        static public int choixMenu()
        {
            int choix = 4;
            Console.WriteLine("Ranger Films");
            Console.WriteLine();
            Console.WriteLine("1. Creer le dossier associé");
            Console.WriteLine("2. Chercher les doublons");
            Console.WriteLine("3. Exporter liste de mes films");
            Console.WriteLine("4. Fermer");
            Console.Write("Choix :");
            var result = Console.ReadLine();
            try
            {
                choix =  Convert.ToInt32(result);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Appuyez sur une touche pour revenir au menu d'accueil");
                Console.ReadLine();
                menuAccueil();
            }
            return choix;
        }
    }
}
