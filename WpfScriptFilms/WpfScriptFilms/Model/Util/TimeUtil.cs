using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScriptFilms.Model.Util
{
    public static class TimeUtil
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string formatTimeSpan(TimeSpan pTimeSpan)
        {
            string time = "00:00:00";
            try
            {
                time = pTimeSpan.ToString(@"hh\:mm\:ss");
            }
            catch (FormatException e)
            {
                log.Error("Le temps n'as pas pu etre casté au format attendu", e);
            }
            return time;
        }

        internal static TimeSpan msToTimeSpan(string pMiliseconds)
        {
            int ms = 0;
            int seconds = 0;
            try
            {
                ms = Int32.Parse(pMiliseconds);
                seconds = ms / 1000;
            }
            catch(FormatException e)
            {
                log.Error("Impossible de convertir "+pMiliseconds +" en entier");
            }
            return new TimeSpan(0,0,seconds);
        }
    }
}
