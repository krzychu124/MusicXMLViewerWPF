using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    public class Direction : MusicalChars
    {
        #region Attributes
        private string placement;
        private string directive;
        #endregion
        #region Elements
        private Dynamics dynamics; // TODO_L will be moved to directions; // Done
        private List<Directions> directionList = new List<Directions>();
        private DirectionType typ; // TODO_L rework to class, then add all attributes
        private int posY; //  unusable, during rework...
        private int relX; //  unusable, during rework...
        private float offset;
        private int staff;
        #endregion

        public Dynamics Dynamics { get { return dynamics; } }
        public List<Directions> DirectionList { get { return directionList; } }
        public int Staff { get { return staff; } }
        public float Offset { get { return offset; } }
        public string Placement { get { return placement; } }
        public string Directive { get { return directive; } }

        public Direction(XElement x)
        {
            var directions = x;
            placement = null;
            directive = null;
            if (directions.HasAttributes) // seems to be done for now
            {
                placement = directions.Attribute("placement") != null ? directions.Attribute("placement").Value : null;
                directive = directions.Attribute("directive") != null ? directions.Attribute("directive").Value : null;
            }
            if (directions.HasElements)
            {
                var elements = x.Elements();
                foreach (var item in elements)
                {
                    string name = item.Name.LocalName;
                    switch (name)
                    {
                        case "staff":
                            staff = int.Parse(item.Value);
                            break;
                        case "direction-type":
                            var direction = new Directions(item);
                            directionList.Add(direction);
                            break;
                        case "offset":
                            offset = float.Parse(item.Value);
                            break;
                        default:
                            Logger.Log($"[direcion-element] not implemented switch '{name}' element");
                            break;
                    }
                }
            }
        }
        #region Direction constructors - not usage currently
        public Direction(Dynamics dynamic, string placement)
        {
            typ = DirectionType.dynamics;
            dynamics = dynamic;
            this.placement = placement;
        }
        
        public Direction(string placement, string direct_type, int posY, string type)
        {
            this.placement = placement;
            this.posY = posY;
            switch (direct_type)
            {
                case "wedge":
                    typ = DirectionType.wedge;
                    break;
                case "dynamics":
                    typ = DirectionType.dynamics;
                    break;
                default:
                    typ = DirectionType.other;
                    break;
            }
        }
        #endregion

        public void Draw(DrawingVisual visual, Point p)
        {

            foreach (var item in directionList)
            {
                if(item.Coda != null)
                {
                    DrawingVisual codaVisual = new DrawingVisual();
                    item.Coda.Draw(codaVisual, p);
                    visual.Children.Add(codaVisual);
                }

                if (item.Rehearsal != null)
                {
                    DrawingVisual rehearsalVisual = new DrawingVisual();
                    item.Rehearsal.Draw(rehearsalVisual, p);
                    visual.Children.Add(rehearsalVisual);
                }

                if (item.Segno != null)
                {
                    DrawingVisual segnoVisual = new DrawingVisual();
                    item.Segno.Draw(segnoVisual, p);
                    visual.Children.Add(segnoVisual);
                }

                if (item.Dynamics != null)
                {
                    DrawingVisual dynamicsVisual = new DrawingVisual();
                    item.Dynamics.Draw(dynamicsVisual, p);
                    visual.Children.Add(dynamicsVisual);
                }

                if (item.Other != null)
                {
                    DrawingVisual otherVisual = new DrawingVisual();
                    item.Other.Draw(otherVisual, p);
                    visual.Children.Add(otherVisual);
                }
            }
        }
    }

    public class Directions
    {
        private Wedge wedge = null;
        private Segno segno = null;
        private Rehearsal rehearsal = null;
        private Coda coda = null;
        private Dynamics dynamics = null;
        private OtherDirection other = null;
        private Words words = null;

        public Wedge Wedge { get { return wedge; } }
        public Segno Segno { get { return segno; } }
        public Rehearsal Rehearsal { get { return rehearsal; } }
        public Coda Coda { get { return coda; } }
        public Dynamics Dynamics { get { return dynamics; } }
        public OtherDirection Other { get { return other; } }

        public Directions(XElement x)
        {
            var dir = x.Elements();
            foreach (var item in dir)
            {

                string name = item.Name.LocalName;
                switch (name)
                {
                    case "wedge":
                        wedge = new Wedge(x);
                        break;
                    case "rehearsal":
                        rehearsal = new Rehearsal(x);
                        break;
                    case "segno":
                        segno = new Segno(x.Attributes());
                        break;
                    case "coda":
                        coda = new Coda(x.Attributes());
                        break;
                    case "dynamics":
                        dynamics = new Dynamics(x);
                        break;
                    case "words":
                        words = new Words(x);
                        break;
                    case "other-direction":
                        other = new OtherDirection(x);
                        break;
                    default:
                        break;
                }
            }
        }
        
    }

    public class OtherDirection : EmptyPrintStyle, IDirections
    {
        private string id = "other-direction";
        private string value;

        public string Id { get { return id; } }
        public string Value { get { return value; } }

        public OtherDirection(XElement x) : base (x.Attributes())
        {
            value =  x.Value;
        }

        public void Draw(DrawingVisual visual, Point p)
        {
            DrawingVisual other_dir = new DrawingVisual();
            using (DrawingContext dc = other_dir.RenderOpen())
            {
                Misc.DrawingHelpers.DrawText(dc, Value, p, this.font_size, withsub: false);
            }
            visual.Children.Add(other_dir);
        }
    }

    enum DirectionType //TODO_L may be not usable due to rework whole class
    {
        other,
        wedge,
        dynamics,
       // words, not now
    }
    public class Wedge : PositionHelper, IDirections
    {
        private string id= "wedge";
        private WedgeType type;
        private int number;
        private float spread;
        private float dash_length;
        private float space_length;

        public string Id { get { return id; } }
        public WedgeType Type { get { return type; } }
        public int Number { get { return number; } }
        public float Spread { get { return spread; } }
        public float DLength { get { return dash_length;  } }
        public float SLength { get { return space_length; } }

        public Wedge(XElement x) : base (x.Attributes())
        {
            var attr = x.Attributes();
            foreach (var item in attr)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "type":
                        type = item.Value == "crescendo" ? WedgeType.crescendo : item.Value == "diminuendo" ? WedgeType.diminuendo : item.Value == "stop" ? WedgeType.stop : WedgeType.next;
                        break;
                    case "number":
                        number = int.Parse(item.Value);
                        break;
                    case "spread":
                        spread = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "dash-length":
                        dash_length = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "space-length":
                        space_length = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    default:
                        break;
                }
            }
        }
        public enum WedgeType
        {
            crescendo,
            diminuendo,
            stop,
            next
        }
        public void Draw(DrawingVisual visual, Point p)
        {
            DrawingVisual wedge = new DrawingVisual();
            using(DrawingContext dc = wedge.RenderOpen())
            {
                //incomplete
            }
            visual.Children.Add(wedge);
        }
        
    }
    public class Rehearsal : EmptyPrintStyle, IDirections
    {
        private string id = "rehearsal";
        private string value;

        public string Id { get { return id; } }
        public string Value { get { return value; } }

        public Rehearsal(XElement x) : base (x.Attributes())
        {
            value = x.Value;
        }

        public void Draw(DrawingVisual visual, Point p)
        {
            Point rehearsalPos = new Point(p.X - this.def_x, p.Y - this.def_y);
            Point rect1 = new Point(rehearsalPos.X - 8, rehearsalPos.Y - 8);
            Point rect2 = new Point(rehearsalPos.X + 8, rehearsalPos.Y + 8);
            Pen pen = new Pen(Brushes.Black, 1);
            Rect rectangle = new Rect(rect1, rect2);
            DrawingVisual rehearsal = new DrawingVisual();
            using(DrawingContext dc = rehearsal.RenderOpen())
            {
                dc.DrawRectangle(Brushes.Transparent, pen, rectangle);
                Misc.DrawingHelpers.DrawText(dc, Value, rehearsalPos, this.FontSize, Halign.center, Valign.middle, font_weight:this.font_weight, withsub:false);
            }
            visual.Children.Add(rehearsal);
        }
    }
    public interface IDirections
    {
        string Id { get; }
        void Draw(DrawingVisual visual, Point point);
    }

    public enum LineType
    {
        solid,
        dashed,
        dotted,
        wavy
    }
    public class Words : EmptyPrintStyle
    {
        private string name = "words";
        
        public string Name { get { return name; } }

        public Words(XElement x): base(x.Attributes())
        {

        }
    }

    public class PositionHelper
    {
        private float def_x;
        private float def_y;
        private float rel_x;
        private float rel_y;

        public float DefX { get { return def_x; } }
        public float DefY { get { return def_y; } }
        public float RelX { get { return rel_x; } }
        public float RelY { get { return rel_y; } }

        public PositionHelper(IEnumerable<XAttribute> x) // TODO_L check if it's working
        {
            foreach (var item in x)
            {
                string name = item.Name.LocalName;
                switch (name)
                {
                    case "default-x":
                        def_x = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "default-y":
                        def_y = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "relative-x":
                        rel_x = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                    case "relative-y":
                        rel_y = float.Parse(item.Value, CultureInfo.InvariantCulture);
                        break;
                }
            }
        }
    }
}
