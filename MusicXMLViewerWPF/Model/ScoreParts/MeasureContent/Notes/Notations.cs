using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Notations 
    { 
       
        //protected int id;
        protected NotationTypes type;
        protected string type_s;
        protected bool ispartial;
        protected string noteid;
        //public int Id { get { return id; } }
        public NotationTypes NotationType {  get { return type; } }
        public bool IsPartialObject { get { return ispartial; } }
        public string NoteID { get { return noteid; } }
        protected void setNotationType(string s)
        {
            type_s = s;
            if (notations_dict.ContainsKey(s))
            {
                type = notations_dict[s];
                
            }
            else
            {
                type = notations_dict["other"];
            }
        }

        public Dictionary<string, NotationTypes> notations_dict = new Dictionary<string, NotationTypes>{
            {"other",NotationTypes.other },
            {"slur",NotationTypes.slur },
            {"fermata",NotationTypes.fermata },
            {"arpeggiate",NotationTypes.arpeggiate },
            {"non_arpeggiate",NotationTypes.non_arpeggiate },
            {"ornament",NotationTypes.ornament },
            {"slide",NotationTypes.slide },
            {"glissando",NotationTypes.glissando },
            {"tuplet",NotationTypes.tuplet },
            {"tied",NotationTypes.tied },
            {"articulation", NotationTypes.articulation}
        };
    }

    class Tuplet : Notations        // not testet yet // experimental
    { 
        private bool placement;
        private bool bracket;
        private bool show_number;
        private bool show_type;
        private int number;
        private TupletType _type;

        public bool Placement { get { return placement; } }
        public bool Bracket {  get { return bracket; } }
        public bool Show_number {  get { return show_number; } }
        public bool Show_type {  get { return show_type; } }
        public int Number { get { return number; } }
        public TupletType Tuplet_Type { get { return _type; } }

        public Tuplet(XElement x, string id)
        {
            setNotationType("tuplet");
            noteid = id;
            foreach (var item in x.Elements())
            {
                switch (item.Name.LocalName)
                {
                    default:
                        break;
                }
            }
        }
        public Tuplet(string type, int num = 1, bool bracket = false,string placement = "",bool shw_num = false, bool shw_tp = false)
        {
            _type = type == "start" ? TupletType.start : TupletType.stop;
            number = num;
            this.bracket = bracket;
            this.placement = placement == "" || placement == "below" ? false: true;
            show_number = shw_num;
            show_type = shw_tp;
        }

        public enum TupletType
        {
            start,
            stop
        }
        public override string ToString()
        {
            return $"{Number} {Placement} {Tuplet_Type.ToString()}";
        }
    }

    class Slur : Notations
    {
        private bool placement;
        private int level;
        private SlurType type_;

        public bool Placement { get { return placement; } }
        public int Level { get { return level; } }
        public SlurType Type { get { return type_; } }
        public string Type_S { get { return type_s; } }

        public Slur (XElement x, string id)
        {
            noteid = id;
            setNotationType("slur");
            level = int.Parse(x.Attribute("number").Value);
            type_ = getSlurType(x.Attribute("type").Value);
            if (x.Attribute("placement") != null)
            {
                placement = x.Attribute("placement").Value == "below" ? false : true;
            }
        }
        public Slur(int lvl, string t)
        {
            setNotationType("slur");
            level = lvl;
            type_ = getSlurType(t);
        }
        public Slur(int lvl, string t,string placement) : this(lvl,t)
        {
            
            this.placement = placement=="below"? false : true;
           
        }
        
        private SlurType getSlurType(string s)
        {
            switch (s)
            {
                case "start": return SlurType.start;
                case "continue": return SlurType.next;
                case "stop":   return SlurType.stop;
                default: return SlurType.unknown;
            }
            
        }
        public override string ToString()
        {
            return $"{Type_S} {Level} {Placement} {Type.ToString()}";
        }

        public enum SlurType
        {
            start,
            next,
            stop,
            unknown
        }
        public static void Draw(DrawingVisual visual, List<Slur> slurslist)
        {
            Slur slurbegin = slurslist.ElementAt(0);
            Slur slurend = slurslist.ElementAt(1);
            Note firstnote;
            Note lastnote;
            try
            {
                firstnote = (Note)Misc.ScoreSystem.GetSegment(slurbegin.NoteID);
                lastnote = (Note)Misc.ScoreSystem.GetSegment(slurend.NoteID);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occured {e.ToString()}");
                throw;
            }
            

            sbyte dir = slurbegin.Placement ? (sbyte)1 : (sbyte)-1; // set direction of slur
            Pen pen = new Pen(Brushes.Black, 0.5);
            Point p1 = firstnote.NoteHeadPosition;
            Point p2 = lastnote.NoteHeadPosition;
            if (dir == 1) //! slur ^
            {
                if (firstnote.Stem_dir)
                {
                    p1.X += 6;
                    p1.Y -= 2;
                }
                else
                {
                    p1.X += 6;
                    p1.Y -= 6;
                }
                if (lastnote.Stem_dir) //! 2nd note stem ^
                {
                    p2.X += 2;
                    p2.Y -= 20;
                }
                else //! 2nd note stem v
                {
                    p2.X -= 5;
                    p2.Y -= 5;
                }
            }
            else //! slur v
            {
                if (firstnote.Stem_dir)
                {
                    p1.X += 5;
                    p1.Y += 5;
                }
                else
                {
                    p1.X += 5;
                    p1.Y -= 5;
                }
                if (lastnote.Stem_dir)//! 2nd note stem ^
                {
                    p2.X -= 5;
                    p2.Y += 5;
                }
                else //! 2nd note stem v
                {
                    p2.X -= 5;
                    p2.Y -= 5;
                }
            }
            bool isDash = false;
            DrawingVisual slurs = new DrawingVisual();
            using (DrawingContext dc = slurs.RenderOpen())
            {
                StreamGeometry sg = new StreamGeometry();
                using (StreamGeometryContext sgc = sg.Open())
                {
                    sbyte offset = 6; // set offset of bezier according to notes // shape of curve
                    float distance = Calc.Distance(p1, p2);
                    Point outerNode = Calc.PerpendicularOffset(p1, p2, (dir * distance) / (offset * 0.6f)); // calculate pos of outer bezier steering node
                    Point innerNode = Calc.PerpendicularOffset(p1, p2, (dir * distance) / (offset * 0.72f)); // calculate pos of inner bezier steering node
                    if (isDash) // if dashed slur  - - - - -
                    {
                        pen.Thickness = 5;
                        pen.DashStyle = DashStyles.Dash;
                        sgc.BeginFigure(p1, false, true);
                        sgc.QuadraticBezierTo(innerNode, p2, true, true);
                    }
                    else // if regular 
                    {
                        sgc.BeginFigure(p1, true, true);
                        sgc.QuadraticBezierTo(outerNode, p2, true, true);
                        sgc.QuadraticBezierTo(innerNode, p1, true, true);
                    }

                }
                sg.Freeze(); // freez geometry stream
                dc.DrawGeometry(Brushes.Black, pen, sg); // draw on drawing context
            }
            visual.Children.Add(slurs); // add to canvas list
        }
    }

    class FermataTest : Notations // not tested //
    {
        bool placement;
        string symbol;
        
        public bool Placement { get { return placement; } }
        public string Symbol { get { return symbol; } }
        
        public FermataTest(string placement)
        {

            setNotationType("fermata");
            symbol = this.placement ? MusicalChars.FermataBelow : MusicalChars.Fermata;
            this.placement = placement == "uprighht" ? false : true;
        }

    }

    class Ornaments : Notations // not tested yet // experimental
    {
        private bool placement;
        private float relativeX;
        private float relativeY;
        private OrnamentTypes _type;
        private string symbol;
        public bool Placement {  get { return placement; } }
        public float RelPosX { get { return relativeX; } }
        public float RelPosY { get { return relativeY; } }
        public OrnamentTypes O_Type {  get { return _type; } }
        public string Symbol { get { return symbol; } }

        public Ornaments(string s, float x, float y, string place)
        {
            getOrnamentType(s);
            relativeX = x;
            relativeY = y;
            placement = place == "below" ? true : false;
        }
        private void getOrnamentType(string s)
        {
            if (ornament_dict.ContainsKey(s))
            {
                _type = ornament_dict[s];
            }
            else
            {
                _type = OrnamentTypes.other;
            }

        }

        private void getSymbol()
        {
            if (ornament_symbols_string.ContainsKey(_type))
            {
                symbol = ornament_symbols_string[_type];
            }
            else
            {
                symbol = ornament_symbols_string[OrnamentTypes.other];
            }
        }

        private Dictionary<string,OrnamentTypes> ornament_dict = new Dictionary<string, OrnamentTypes >()
        {
            {"other-ornament",OrnamentTypes.other },
            {"trill-mark", OrnamentTypes.trillmark },
            {"turn", OrnamentTypes.turn },
            {"delayed-turn", OrnamentTypes.delayed_turn },
            {"inverted-turn", OrnamentTypes.inverted_turn },
            {"delayed-inverted-turn", OrnamentTypes.delayed_inverted_turn },
            {"vertical-turn", OrnamentTypes.vertical_turn },
            {"shake", OrnamentTypes.shake },
            {"wavy-line", OrnamentTypes.wavy_line },
            {"mordent", OrnamentTypes.mordent },
            {"inverted-mordent", OrnamentTypes.inverted_mordent },
            {"schleifer", OrnamentTypes.schleifer },
            {"tremolo", OrnamentTypes.tremolo },
        };
        public Dictionary<OrnamentTypes, string> ornament_symbols_string = new Dictionary<OrnamentTypes, string>() {
            {OrnamentTypes.other, "?or" },
            {OrnamentTypes.trillmark, MusicalChars.trillmark },
            {OrnamentTypes.delayed_turn, MusicalChars.delayed_turn},
            {OrnamentTypes.inverted_turn, MusicalChars.inverted_turn},
            {OrnamentTypes.delayed_inverted_turn, MusicalChars.delayed_inverted_turn },
            {OrnamentTypes.turn, MusicalChars.turn},
            {OrnamentTypes.vertical_turn, MusicalChars.vertical_turn },
            {OrnamentTypes.shake, MusicalChars.shake },
            {OrnamentTypes.wavy_line, MusicalChars.wavy_line},
            {OrnamentTypes.mordent, MusicalChars.mordent },
            {OrnamentTypes.inverted_mordent, MusicalChars.inverted_mordent },
            {OrnamentTypes.schleifer, MusicalChars.schleifer},
            {OrnamentTypes.tremolo, MusicalChars.tremolo },
        };  // dictionary of symbols // first need to add them to MusChar.cs
    }

    enum OrnamentTypes
    {
        other,
        trillmark,
        turn,
        delayed_turn,
        inverted_turn,
        delayed_inverted_turn,
        vertical_turn,
        shake,
        wavy_line,
        mordent,
        inverted_mordent,
        schleifer,
        tremolo,
    }

    enum NotationTypes
    { 

        other,
        arpeggiate,
        articulation,
        fermata,
        glissando,
        non_arpeggiate,
        ornament,
        slide,
        slur,
        tied,
        tuplet,

    }
}

