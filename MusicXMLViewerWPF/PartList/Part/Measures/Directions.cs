using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Directions : MusicalChars
    {
        //attributes//
        private string placement;
        private string directive;
        //additional settings//
        private Dynamics dynamics;
        private DirectionType typ; // TODO_H rework to class, then add all attributes
        private int posY;
        private int relX;
        private float offset;
        private int staff;

        public Directions(XElement x)
        {
            var directions = x.Element("direction");
            placement = null;
            directive = null;
            if (directions.HasAttributes)
            {
                placement = directions.Attribute("placement") != null ? directions.Attribute("placement").Value : null;
                directive = directions.Attribute("directive") != null ? directions.Attribute("directive").Value : null;
            }
            if (directions.HasElements)
            {

            }
        }
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
