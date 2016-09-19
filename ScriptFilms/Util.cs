using System;
using System.IO;
using System.Threading;

namespace ScriptFilms
{
    class Util
    {
        
        public Util()
        {

        }

        public static void afficherPourcentage()
        {
            for (int i = 0; i <= 100; i++)
            {
                Console.Write("\r{0}%", i);
                Thread.Sleep(50);
            }
            Console.Write("\rOpération terminé avec succès !");
        }

        internal static void afficherPourcentage(int nbDossierActuelle, int nbDossierDeFilm)
        {
            Console.Write("\rAvancement{0}%", pourcent(nbDossierActuelle, nbDossierDeFilm));
            Thread.Sleep(50);
        }

        private static int pourcent(int nbDossierActuelle, int nbDossierDeFilm)
        {
            return nbDossierActuelle * 100 / nbDossierDeFilm;
        }

        internal static void OperationTermine()
        {
            Console.Write("\rOpération terminé avec succès !");
        }

        public void renommer(string ancienNom, string nouveauNom)
        {
            File.Move(ancienNom, nouveauNom);
        }
    }
}
