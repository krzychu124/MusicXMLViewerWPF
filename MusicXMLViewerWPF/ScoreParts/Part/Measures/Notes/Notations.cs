using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLViewerWPF
{
    class Notations : MusicalChars
    { 
       
        //protected int id;
        protected new NotationTypes type;
        protected string type_s;
        protected bool ispartial;
        
        //public int Id { get { return id; } }
        public NotationTypes NotationType {  get { return type; } }
        public bool IsPartialObject { get { return ispartial; } }
        
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
    }

    class Slur : Notations
    {
        private bool placement;
        private int level;
        private SlurType type_;
        

        public bool Placement { get { return placement; } }
        public int Level { get { return level; } }
        new public SlurType Type { get { return type_; } }
        public string Type_S { get { return type_s; } }

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


        public enum SlurType
        {
            start,
            next,
            stop,
            unknown
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
            symbol = this.placement ? MusChar.FermataBelow : MusChar.Fermata;
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
            {OrnamentTypes.trillmark, MusChar.trillmark },
            {OrnamentTypes.delayed_turn, MusChar.delayed_turn},
            {OrnamentTypes.inverted_turn, MusChar.inverted_turn},
            {OrnamentTypes.delayed_inverted_turn, MusChar.delayed_inverted_turn },
            {OrnamentTypes.turn, MusChar.turn},
            {OrnamentTypes.vertical_turn, MusChar.vertical_turn },
            {OrnamentTypes.shake, MusChar.shake },
            {OrnamentTypes.wavy_line, MusChar.wavy_line},
            {OrnamentTypes.mordent, MusChar.mordent },
            {OrnamentTypes.inverted_mordent, MusChar.inverted_mordent },
            {OrnamentTypes.schleifer, MusChar.schleifer},
            {OrnamentTypes.tremolo, MusChar.tremolo },
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

