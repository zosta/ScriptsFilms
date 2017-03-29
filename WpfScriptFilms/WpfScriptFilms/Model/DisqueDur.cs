using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScriptFilms
{
    class DisqueDur
    {
        public DisqueDur(string pName)
        {
            Configuration conf = new Configuration();
            Name = pName;
            if (conf.disqueChoosen.Contains(pName))
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
