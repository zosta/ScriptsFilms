using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WpfScriptFilms.Model.Util
{
    public static class FileUtil
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal static List<string> extractFilmsOfDirectory(string repertoire)
        {

            //on va chercher le film à l'interieur de ce dossier
            var files = Directory.EnumerateFiles(repertoire)
                .Where(file => file.ToLower().EndsWith("flv") ||
                file.ToLower().EndsWith("mkv") ||
                file.ToLower().EndsWith("wmv") ||
                file.ToLower().EndsWith("avi") ||
                file.ToLower().EndsWith("mp4")).ToList();

            log.Info(files.Count +" film a été trouvé dans le repertoire "+repertoire);
           return files;
        }

        

        internal static List<string> extractNfoOfDirectory(string repertoire)
        {
            //on va chercher le film à l'interieur de ce dossier
            var files = Directory.EnumerateFiles(repertoire)
                .Where(file => file.ToLower().EndsWith("nfo")).ToList();

            log.Info(files.Count + " NFO a été trouvé dans le repertoire " + repertoire);
            if(files.Count == 0)
            {
                files = new List<string> { "{NONFO}" };
            }
            return files;
        }



    }
}
