using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptFilms
{
    class Menu
    {
        private static readonly Menu instance = new Menu();

        private Menu() { }

        public static Menu Instance
        {
            get
            {
                return instance;
            }
        }

        internal void afficher()
        {
            Console.Clear();
            Util util = new Util();
            int userInput = 0;

            userInput = choixMenu();

            if (userInput == 1)
            {
                Dossier doss = new Dossier();
                doss.creerDossierDesFichiers();

            }
            else if (userInput == 2)
            {
                Console.Clear();
                int userInputDoublon = 0;
                userInputDoublon = choixMenuDoublon();

                if (userInputDoublon == 1)
                {
                    Dossier doss = new Dossier();
                    doss.chercherDoublonsDansDossier();
                }
                else if(userInputDoublon == 2)
                 {
                     Dossier doss = new Dossier();
                     doss.chercherDoublonsDeDossier();
                 }
                else
                {
                    afficher();
                }

            }
            else if (userInput == 3)
            {
                InfoMediatheque info = new InfoMediatheque();
                info.exporterListeFilms();
            }
            else if (userInput == 4)
            {
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Ce choix n'est pas disponible, appuyer sur une touche pour revenir au menu d'accueil");
                Console.ReadLine();
                afficher();
            }
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
                choix = Convert.ToInt32(result);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Appuyez sur une touche pour revenir au menu d'accueil");
                Console.ReadLine();
                instance.afficher();
            }
            return choix;
        }

        static public int choixMenuDoublon()
        {
            int choix = 4;
            Console.WriteLine("Chercher les doublons");
            Console.WriteLine();
            Console.WriteLine("1. Chercher les dossiers contenant plus de un fichier");
            Console.WriteLine("2. Chercher les dossier qui existe en double dans plusieurs emplacement");
            Console.WriteLine("3. Fermer");
            Console.Write("Choix :");
            var result = Console.ReadLine();
            try
            {
                choix = Convert.ToInt32(result);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Appuyez sur une touche pour revenir au menu d'accueil");
                Console.ReadLine();
                instance.afficher();
            }
            return choix;
        }
    }

}
