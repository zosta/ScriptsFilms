using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptFilms
{
    class InfoMediatheque
    {
        internal void exporterListeFilms()
        {

            const string nomFichier = @"MesFilms.txt";

            Console.Clear();
            Console.WriteLine("Exporter la liste des films");
            Console.WriteLine();
            Console.WriteLine("En cours...");


            int nbDossierDeFilm = Directory.GetDirectories(Config.sourceDirectory).Length;
            int nbDossierActuelle = 0;
            try
            {
                var repertoires = Directory.EnumerateDirectories(Config.sourceDirectory, "*", SearchOption.TopDirectoryOnly);

                List<string> liFilms = new List<string>();
                foreach (string repertoire in repertoires)
                {
                    nbDossierActuelle++;
                    liFilms.Add(repertoire.Replace(Config.sourceDirectory+@"\", ""));
                    Util.afficherPourcentage(nbDossierActuelle, nbDossierDeFilm);
                }

                liFilms.Sort();

                liFilms.Add("==========================");
                liFilms.Add("Nombre total de films : " + nbDossierDeFilm);
                File.WriteAllLines(Config.sourceDirectory + nomFichier, liFilms);
                //ouvrir fichier
                Process.Start(Config.sourceDirectory + nomFichier);
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
