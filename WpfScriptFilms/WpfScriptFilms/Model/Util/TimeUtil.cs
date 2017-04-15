using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScriptFilms.Model.Util
{
    public static class TimeUtil
    {
        public static string formatTimeSpan(TimeSpan pTimeSpan)
        {
            return pTimeSpan.ToString("%hh ' h ' %m' min.'");
        }
    }
}
