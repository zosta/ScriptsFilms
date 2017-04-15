using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScriptFilms.Model.WS
{
    class WsFilms
    {
        private const string adresseOmdb = "http://www.omdbapi.com/?";
        //voir tmdb

        public void chercherFilms(Film pFilm)
        {
            string titre = "t="+pFilm.titre.Replace(" ","+");
            string annee = "&y=" + pFilm.annee;
        } 
    }
}
