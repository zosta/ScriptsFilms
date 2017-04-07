using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScriptFilms
{
    class DisqueDur
    {
    //    private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    //(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DisqueDur(string pName)
        {
            //log.Info("Lancement de la classe Disque Dur");
            Name = pName;
            if (Configuration.Instance.disqueChoosen.Contains(pName))
            {
                IsSelected = true;
            }
            else
                IsSelected = false;
        }

        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
