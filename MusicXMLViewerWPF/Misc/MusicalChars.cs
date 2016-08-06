using MusicXMLViewerWPF.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF
{
    public class MusicalChars : IAutoPosition
    {
        protected string name;
        protected string musicalcharacter;
        protected float width;
        public float Width { get { return width; } }
        public MusSymbolType type;

        public MusicalChars()
        {

        }

        protected string Name { get; }
        public MusSymbolType Type { get; }

    }
}
