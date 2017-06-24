using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilmApp.Model
{
    /// <summary>
    /// La classe utilisateur
    /// </summary>
    class Utilisateur
    {
        private Bibliotheque bibliotheque;

        /// <summary>
        /// Le constructeur : il initialise une bibliothèque
        /// </summary>
        public Utilisateur()
        {
            bibliotheque = Bibliotheque.Instance;
        }
    }
}