using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptFilms
{
    class Program
    {
        private const string sourceDirectory = @"H:\Videos\Films";
        private static Util util;

        static void Main(string[] args)
        {
            Menu.Instance.afficher();
        }
    }
}
