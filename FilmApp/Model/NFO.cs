
using log4net.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FilmApp.Model
{
    /// <summary>
    ///  La classe NFO , utilisé lorsque le film possède un fichier NFO
    /// </summary>
    class NFO
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string urlNfo;
        public int[] resolution { get; }

        private XDocument doc;
        /// <summary>
        /// Le constructeur
        /// </summary>
        /// <param name="pUrlNfo">Le chemin du NFO</param>
        public NFO(string pUrlNfo)
        {
            urlNfo = pUrlNfo;
        }


        internal Film getInfosNfo()
        {
            doc = XDocument.Load(urlNfo);

            Film film = new Film("");

            var varfilm = (from b in doc.Descendants("Block")
                        select new Film("")
                        {
                            codec = (string)b.Attribute("Name"),
                            acteurs = (from a in b.Elements("Attributes")
                                       select new Acteur
                                       {
                                           ordre = (int)a.Element("order"),
                                           urlImgActeur = (string)a.Element("image")
                                       }).ToList()
                        });

            //on essaye de caster le var en film
            try
            {
                film = (Film)varfilm;
            }
            catch(InvalidCastException e)
            {
                log.Error("Conversion impossible entre le var Film et le type Film", e);
            }
            return film;
        }
    }
}