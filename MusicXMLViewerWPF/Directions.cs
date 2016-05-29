using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLTestViewerWPF
{
    class Directions : MusicalChars
    {
        public Dynamics dynamics;
        public DirectionType typ;
        public string placement;
        public int posY;
        public int relX;

        public Directions(Dynamics dynamic, string placement)
        {
            typ = DirectionType.dynamics;
            dynamics = dynamic;
            this.placement = placement;
        }
        public Directions(string placement, string direct_type, int posY, string type)
        {
            this.placement= placement;
            this.posY = posY;
            switch (direct_type)
            {
                case "wedge": typ = DirectionType.wedge;
                    break;
                case "dynamics": typ = DirectionType.dynamics;
                    break;
                default: typ = DirectionType.other;
                    break;
            }
        }
    }
    enum DirectionType
    {
        other,
        wedge,
        dynamics,
       // words, not now
    }
}
