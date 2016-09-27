using MusicXMLViewerWPF.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Note : Segment, IAutoPosition
    { //Need to be reworked !! 1)little improvements done
        #region fields
        protected Beam beam;
        protected bool hasBeams;
        protected bool hasDot;
        protected bool hasNotations;
        protected bool isCustomStem;
        protected bool isGraceNote = false;
        protected bool isRest;
        protected bool stem_dir;
        protected float defaultStem;
        protected float posX;
        protected float posY;
        protected float stem_f;
        protected int dot;
        protected int duration;
        protected int id;
        protected int measure_id;
        protected int voice;
        protected List<Notations> notationsList;
        protected MusSymbolDuration symbol_type;
        protected Pitch pitch;
        protected Stem stem;
        protected string symbol;
        protected string symbol_value;
        public event PropertyChangedEventHandler NotePropertyChanged;
        #endregion

        #region properties
        public Beam Beam { get { return beam; } protected set { beam = value; } }
        public bool HasBeams { get { return hasBeams; } protected set { hasBeams = value; } }
        public bool HasDot { get { return hasDot; } protected set { hasDot = value; } }
        public bool HasNotations { get { return hasNotations; } protected set { hasNotations = value; } }
        public bool IsCustomStem { get { return isCustomStem; } protected set { isCustomStem = value; } }
        public bool IsGraceNote { get { return isGraceNote; } protected set { isGraceNote = value; } }
        public bool IsRest { get { return isRest; } protected set { isRest = value; } }
        public bool Stem_dir { get { return stem_dir; } protected set { stem_dir = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stem_dir))); } }
        public Brush Color { get { return Brushes.Black; } }
        public float DefaultStem { get { return defaultStem; } protected set { defaultStem = value; } }
        public float PosX { get { return posX; } protected set { } }
        public float PosY { get { return posY; } protected set { } }
        public float StemF { get { return stem_f; } protected set { stem_f = value; } }
        public int Dot { get { return dot; } protected set { } }
        public int Duration { get { return duration; } protected set { } }
        public int Id { get { return id; } protected set { } }
        public int MeasureId { get { return measure_id; } protected set { } }
        public int Voice { get { return voice; } protected set { } }
        public List<Notations> NotationsList { get { return notationsList; } protected set { } }
        public MusSymbolDuration SymbolType { get { return symbol_type; } set { symbol_type = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SymbolType))); } }
        public Pitch Pitch { get { return pitch; } protected set { } }
        public Stem Stem { get { return stem; } protected set { stem = value; } }
        public string Symbol { get { return symbol; } protected set { symbol = value; } }
        public string SymbolXMLValue { get { return symbol_value; } set { symbol_value = value; } }
        #endregion

        /*
       public Note( int measure_id, int id,float pos, Pitch p, int dur,int v, string t, float s, string dir,bool r, int dot, bool notations)
       {
           this.measure_id = measure_id;
           this.type = MusSymbolType.Note;
           this.id = id;
           posX = pos;
           CalculatePitch(p);
           pitch = p;
           duration = dur;
           voice = v;
           stem_dir = dir == "up" ? false : true;
           symbol_type =SymbolDuration.d_type(t);
           symbol = MusChar.getNoteSymbol(t,stem_dir);
           isRest = r;
           this.dot = dot;
           hasDot = dot != 0 ? true : false;
           stem = s;
           hasNotations = notations;

       } */

        
        public Note(int measure_id, int id, float pos, Pitch p, int dur, int v, string t, float s, string dir, bool hasStemVal, bool r, int num, string bm, Dictionary<int, string> beamList, int dot, bool notations, List<Notations> n_list)
        {
            isCustomStem = hasStemVal ? true : false;
            this.measure_id = measure_id;
            //Segment_type = SegmentType.Note;
            this.id = id;
            posX = pos;
            CalculatePitch(p);
            pitch = p;
            duration = dur;
            voice = v;
            stem_dir = dir == "up" ? false : true;
            symbol_type = SymbolDuration.d_type(t);
            symbol = MusChar.getNoteSymbol(t, stem_dir);
            isRest = r;
            stem_f = s;
            this.dot = dot;
            hasDot = dot != 0 ? true : false;
            beam = new Beam(bm, num, pos, beamList);
            hasBeams = true;
            //beam.Add(b);
            defaultStem = CalculateStem();
            hasNotations = notations;
            notationsList = n_list;
        }

        private void CalculatePitch(Pitch p)
        {
            int o = p.Octave;
            float scale = Measures.Scale;
            this.posY = (p.CalculatedStep * 3.95f) + scale * 0.6f;
        }

        private float CalculateStem()
        {
            float c;
            c = (Measures.Scale * 0.75f);
            return c;
        }

        public Note(XElement x) //TODO_H not finished
        {
            NotePropertyChanged += Note_PropertyChanged;
            SetSegmentColor(Brushes.DarkOliveGreen);
            Segment_type = SegmentType.Chord;
            this.ID = RandomGenerator.GetRandomHexNumber();
            //Width = 10f; //! CalculateWidth();
            foreach (var item in x.Elements())
            {
                switch (item.Name.LocalName)
                {
                    case "pitch":
                        pitch = new Pitch(item);
                        Logger.Log($"{ID} Note Pitch set to s:{Pitch.Step}, o:{Pitch.Octave}, a:{Pitch.Alter}");
                        break;
                    case "duration":
                        duration = int.Parse(item.Value);
                        break;
                    case "voice":
                        voice = int.Parse(item.Value);
                        Logger.Log($"{ID} Note Voice set to {Voice}");
                        break;
                    case "type":
                        SymbolXMLValue = item.Value;
                        SymbolType = SymbolXMLValue != null ? SymbolDuration.d_type(SymbolXMLValue) : MusSymbolDuration.Unknown;
                        Logger.Log($" {ID} Note Type set to {SymbolXMLValue}");
                        break;
                    case "stem":
                        Stem_dir = item.Value == "up" ? true : false;
                        //! stem = new Stem(item);
                        StemF = item.HasAttributes == true ? float.Parse(item.Attribute("default-y").Value, CultureInfo.InvariantCulture) : 20f;
                        Stem = new Stem(StemF, item.Value);
                        IsCustomStem = true;
                        Logger.Log($"{ID} Note Stem set to {Stem.Direction} {Stem.Length.ToString("0.#")}");
                        break;
                    case "notations":
                        Logger.Log($"{ID} Note Notations not implemented");
                        break;
                    case "dot":
                        Logger.Log($"{ID} Note Dot not implemented but seems to have one or more");
                        break;
                    case "beam":
                        Logger.Log($"{ID} Note Beam not implemented");
                        break;
                    case "accidental":
                        Logger.Log($"{ID} Note Accidental not implemented");
                        break;
                    case "staff":
                        Logger.Log($"{ID} Note Staff not implemented");
                        break;
                    case "lyric":
                        Logger.Log($"{ID} Note Lyric not implemented");
                        break;
                    default:
                        break;
                }
            }
            //
        }

        public Note()
        {
            //PropertyChanged += Note_PropertyChanged;

        }
        #region PropertyChanged options
        protected void Note_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SymbolType":
                    Logger.Log($"{sender.ToString()} SymbolType set");
                    break;
                case "Stem_dir":
                    Symbol = MusChar.getNoteSymbol(SymbolXMLValue, Stem_dir);
                    Logger.Log($"{sender.ToString()}, Stem direction set");
                    break;
                default:
                    break;
            }
        }
        #endregion

        public virtual void Draw(DrawingVisual visual)
        {
            if (IsCustomStem) //! If custom stem length - got from XML file
            {
                DrawingVisual note = new DrawingVisual();
                using (DrawingContext dc = note.RenderOpen())
                {
                    //Relative_y = 310;
                    Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.NotesFont, Color, Relative_x + Spacer_L, Relative_y - 16, MusicScore.Defaults.Scale.Tenths);
                }
                visual.Children.Add(note);
            }
            else //! If default stem length
            {

            }
        }

        protected void SetSegmentColor(Brush brush)
        {
            base.Color = brush;
        }
        private void CalculateWidth()
        {
            //TODO calulation of with neccessary for segment drawing ( with is calculated according to aditional properties of note: added dots, signs(flat,shaph))
        }
        public override string ToString()
        {
            string result = $"Position {Relative_x.ToString("0.#")}X, {Relative_y.ToString("0.#")}Y, Width: {Width.ToString("0.#")}";
            return result;
        }
    }

    public class Beam
    {
        
        private Beam_type typ;
        private Dictionary<int,int> beamlist = new Dictionary<int, int>();
        private float pos;
        private int number;

        public Beam_type BeamType { get { return typ; } }
        public Dictionary<int,int> NoteBeamList { get { return beamlist; } }
        public float Position { get { return pos; } }
        public int BeamNumber { get { return number; } }
        public int NoteBeamsCount { get { return beamlist.Count; } }
        
        public Beam( string type, int n, float p, Dictionary<int, string> l)
        {
            number = n;
            pos = p;
            typ = getBeamType(type);
            foreach (var item in l)
            {
                Beam_type b = getBeamType(item.Value);
                beamlist.Add(item.Key,Convert.ToInt32(b));
            }
            
        }
        private Beam_type getBeamType(string type)
        {
            Beam_type t = Beam_type.stop;
            switch (type)
            {
                case "begin":
                    t = Beam_type.start;
                    break;
                case "continue":
                    t = Beam_type.next;
                    break;
                case "end":
                    t = Beam_type.stop;
                    break;
                case "forward hook":
                    t = Beam_type.forward;
                    break;
                case "backward hook":
                    t = Beam_type.backward;
                    break;

            }
            return t;
        }

        public enum Beam_type
        {
            start,
            next,
            stop,
            forward,
            backward
         }
    }

    public enum DurationType
    {
        unknown,
        whole = 1,
        half = 2,
        quarter = 4,
        eight = 8,
        d16th = 16,
        d32nd = 32,
        d64th = 64
           
    }

    public class Stem
    {
        private float length;
        private string direction;
        private NoteStem stemDirection;
        public float Length { get { return length; } set { length = value; } }
        public string Direction { get { return direction; } set { direction = value; } }
        public NoteStem StemDirection { get { return stemDirection; } set { stemDirection = value; } }
        public Stem(XElement x)
        {
            //? Testing
        }
        public Stem(float length, string direction)
        {
            Length = length;
            Direction = direction;
            StemDirection = Direction == "up" ? NoteStem.up : Direction == "down" ? NoteStem.down : NoteStem.none;
        }
    }

    public enum NoteStem
    {
        none = 0,
        up = 1,
        down = 2
    }
  
}
