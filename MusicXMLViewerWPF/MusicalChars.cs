using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLTestViewerWPF
{
    public class MusicalChars
    {
        protected string name;
        protected string musicalcharacter;
        public MusSymbolType type;
        public MusicalChars()
        {

        }

        protected string Name { get; }
        public MusSymbolType Type { get; }

    }
}
