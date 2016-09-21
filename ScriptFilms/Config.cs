using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptFilms
{
    class Config
    {
        // Emplacement de tout les films exemple D:\Films
        public const string  sourceDirectory = @"D:\test\";


        public const Boolean multiEmplacement = true;

        public List<string> liEmplacements { get; }

        public Config()
        {
            liEmplacements = new List<string>();
            liEmplacements.Add(@"D:\test2");
            liEmplacements.Add(@"D:\test");
        }
    }
}
