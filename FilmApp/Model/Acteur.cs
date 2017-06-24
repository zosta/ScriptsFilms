using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilmApp.Model
{
    class Acteur
    {
        public string nom { get; internal set; }
        public int ordre { get; internal set; }
        public string urlImgActeur { get; internal set; }
    }
}