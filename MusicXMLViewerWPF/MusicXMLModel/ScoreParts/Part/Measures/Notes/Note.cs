using MusicXMLViewerWPF.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Note : Segment, IAutoPosition
    { //Need to be reworked !! 1)little improvements done
        #region fields
        protected Accidental accidental;
        protected Beam beam;
        protected bool hasBeams;
        protected bool hasDot;
        protected bool hasNotations;
        protected bool isCustomStem;
        protected bool isGraceNote = false;
        protected bool isRest;
        protected bool stem_dir;
        protected float defaultStem = 30f;
        protected float posX;
        protected float posY;
        protected float stem_f;
        protected int clefalter;
        protected int dot;
        protected int duration;
        protected int id;
        protected string measure_id;
        protected int voice;
        protected List<Beam> beamlist;
        protected List<Notations> notationsList;
        protected Lyrics lyrics;
        protected MusSymbolDuration symbol_type;
        protected Pitch pitch;
        protected Point noteheadposition;
        protected Stem stem;
        protected string symbol;
        protected string symbol_value;
        protected XElement xmldefinition;
        public event PropertyChangedEventHandler NotePropertyChanged;
        #endregion

        #region properties
        public Accidental Accidental { get { return accidental; } }
        public Beam Beam { get { return beam; } protected set { beam = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Beam))); } }
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
        public int ClefAlter { get { return clefalter; } set { clefalter = value; } }
        public int Dot { get { return dot; } protected set { dot = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dot))); } }
        public int Duration { get { return duration; } protected set { duration = value; } }
        public int Id { get { return id; } protected set { } }
        public string MeasureId { get { return measure_id; } set { measure_id = value; } }
        public int Voice { get { return voice; } protected set { voice = value; } }
        public List<Beam> BeamsList { get { return beamlist; } }
        public List<Notations> NotationsList { get { return notationsList; } protected set { notationsList = value; } }
        public Lyrics Lyrics { get { return lyrics; } protected set { lyrics = value; } }
        public MusSymbolDuration SymbolType { get { return symbol_type; } set { symbol_type = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SymbolType))); } }
        public Pitch Pitch { get { return pitch; } protected set { pitch = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pitch))); } }
        public Point NoteHeadPosition { get { return noteheadposition; } }
        public Stem Stem { get { return stem; } protected set { stem = value; } }
        public string Symbol { get { return symbol; } protected set { symbol = value; } }
        public string SymbolXMLValue { get { return symbol_value; } set { symbol_value = value; } }
        public XElement XMLDefinition { get { return xmldefinition; } }
        public Point NoteHeadLeftLink { get { return new Point(NoteHeadPosition.X - (MusicScore.Defaults.Scale.Tenths * 0.225f) / 2, NoteHeadPosition.Y); } }
        public Point NoteHeadRightLink { get { return new Point(NoteHeadPosition.X + (MusicScore.Defaults.Scale.Tenths * 0.225f) / 2, NoteHeadPosition.Y); } }
        //public new Point Relative { get { return base.Relative; } set { base.Relative = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Relative))); } }
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
            this.measure_id = measure_id.ToString();
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
            c = (Measures.Scale * 0.75f); //! 30
            return c;
        }

        private Point GetNoteHeadPosition(string v)
        {
            float offset = 0.225f * MusicScore.Defaults.Scale.Tenths; // default offset at scale 40 is 9 so 0.225

            Point result = new Point();
            if (v == "left")
            {
                result = new Point(NoteHeadPosition.X - offset / 2, NoteHeadPosition.Y);
            }
            if (v == "right")
            {
                result = new Point(NoteHeadPosition.X + offset / 2, NoteHeadPosition.Y);
            }
            return result;
        }

        public Note(XElement x, int clefalter, string id)
        {

            xmldefinition = x;
            ClefAlter = clefalter;
            MeasureId = id;
            NotePropertyChanged += Note_PropertyChanged;
            //NotePropertyChanged += segment_Properties_Ready;
            PropertyChanged += Note_PropertyChanged;
            SetSegmentColor(Brushes.DarkOliveGreen);
            Segment_type = SegmentType.Chord;
            this.ID = RandomGenerator.GetRandomHexNumber();
            //Width = 10f; //! CalculateWidth();
            foreach (var item in x.Elements())
            {
                //! int z = ((IXmlLineInfo)item).LineNumber;
                switch (item.Name.LocalName)
                {
                    case "pitch":
                        Pitch = new Pitch(item, ClefAlter); //? { ClefAlter = this.ClefAlter };
                        Logger.Log($"{ID} Note Pitch set to s:{Pitch.Step}, o:{Pitch.Octave}, a:{Pitch.Alter}");
                        break;
                    case "duration":
                        Duration = int.Parse(item.Value);
                        break;
                    case "voice":
                        Voice = int.Parse(item.Value);
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
                        StemF = item.HasAttributes == true ? float.Parse(item.Attribute("default-y").Value, CultureInfo.InvariantCulture) : 30f;
                        Stem = new Stem(StemF, item.Value);
                        Logger.Log($"{ID} Note Stem set to {Stem.Direction} {Stem.Length.ToString("0.#")}");
                        break;
                    case "notations":
                        GetNotations(item);
                        Logger.Log($"{ID} Note Notations not implemented");
                        break;
                    case "dot":
                        HasDot = true;
                        Dot++;
                        Logger.Log($"{ID} Note Dot not implemented but seems to have one or more");
                        break;
                    case "beam":
                        IsCustomStem = true;
                         if (beamlist == null) beamlist = new List<Beam>();
                        Beam temp_beam = new Beam(item, this.ID);
                        BeamsList.Add(temp_beam);
                        //? beamlist.Add(temp_beam);
                        HasBeams = true;
                        Logger.Log($"{ID} Note Beam set to {temp_beam.BeamNumber} {temp_beam.BeamType.ToString()} of id {temp_beam.NoteId}");
                        break;
                    case "accidental":
                        accidental = new Accidental(item) { NoteID = ID };
                        Logger.Log($"{ID} Note Accidental set to {Accidental.Symbol} {Accidental.AccidentalType.ToString()}");
                        break;
                    case "staff":
                        Logger.Log($"{ID} Note Staff not implemented");
                        break;
                    case "lyric":
                        Lyrics = new Lyrics(item, ID);
                        Logger.Log($"{ID} Note Lyric set to {Lyrics.Text}");
                        break;
                    default:
                        break;
                }
            }
            SetCalculatedNotePosition();
            if (Stem == null)
            {
                if (Pitch.CalculatedStep <= -7)
                {
                    Stem_dir = false;
                }
                else
                {
                    Stem_dir = true;
                }
            }
            //! Logger.Log(XMLDefinition.ToString());
            //
        }

        private void GetNotations(XElement notations)
        {
            if (NotationsList == null) NotationsList = new List<Notations>();
            foreach (var item in notations.Elements())
            {
                switch (item.Name.LocalName)
                {
                    case "slur":
                        Slur slur = new Slur(item, this.ID);
                        NotationsList.Add(slur);
                        Logger.Log("Slur added");
                        break;
                    case "tuple":
                        Tuplet tuplet = new Tuplet(item, ID);
                        NotationsList.Add(tuplet);
                        Logger.Log("Tuplet added");
                        break;
                    default:
                        Logger.Log($"Notation ctor. not implemented for {item.Name.LocalName}");
                        break;
                }
            }
        }

        private void SetCalculatedNotePosition()
        {
            Calculated_y = Pitch.CalculatedStep * (MusicScore.Defaults.Scale.Tenths * 0.1f) + MusicScore.Defaults.Scale.Tenths * 0.6f;
            noteheadposition = new Point(Relative_x + Spacer_L + 5, Relative_y + Calculated_y + 30);
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
                case "Relative":
                    SetCalculatedNotePosition();
                    Logger.Log("Recalculated NoteSymbol and Notehead positions");
                    break;
                default:
                    Logger.Log($"NotePorpertyChanged for {e.PropertyName} not implenmented");
                    break;
            }
        }
        #endregion

        public virtual void Draw(DrawingVisual visual)
        {
            float notepositionX = Relative_x + Spacer_L;
            float notepositionY = Relative_y + Calculated_y;
            float dot_placement = Pitch.CalculatedStep % 2 == 0 ? 3.5f : 0;
            if (IsCustomStem) //! If default stem length //
            {
                DrawingVisual note = new DrawingVisual();
                using (DrawingContext dc = note.RenderOpen())
                {
                    //Relative_y = 310;  //! Pitch.CalculatedStep * (MusicScore.Defaults.Scale.Tenths * 0.1f)) + MusicScore.Defaults.Scale.Tenths * 0.6f
                    Misc.DrawingHelpers.DrawString(dc, MusChar.QuarterDot, TypeFaces.NotesFont, Color, Relative_x + Spacer_L, Relative_y + Calculated_y, MusicScore.Defaults.Scale.Tenths);
                }
                visual.Children.Add(note);
                DrawAdditionalLines(visual);
                Stem.Draw(visual, new Point(Relative_x + Spacer_L, Relative_y), new Point(Relative_x + Spacer_L, Relative_y + Calculated_y));
                //visual.Children.Add(stemvisual);
                if (HasDot) //! ignoring more than one dot //temp//
                {
                    DrawingVisual dot = new DrawingVisual();
                    using (DrawingContext dc = dot.RenderOpen())
                    {
                        DrawingHelpers.DrawString(dc, MusChar.Dot, TypeFaces.NotesFont, Brushes.Black, notepositionX + 15, notepositionY - dot_placement, MusicScore.Defaults.Scale.Tenths);
                    }
                    visual.Children.Add(dot);
                }
                if (Lyrics != null)
                {
                    DrawingVisual lyrics = new DrawingVisual();
                    Lyrics.Draw(lyrics);
                    visual.Children.Add(lyrics);
                }
                if (Accidental != null)
                {
                    DrawingVisual acc_symbol = new DrawingVisual();
                    Accidental.Draw(acc_symbol);
                    visual.Children.Add(acc_symbol);
                }
            }
            else //! If custom stem length - got from XML file
            {
                DrawingVisual note = new DrawingVisual();
                using (DrawingContext dc = note.RenderOpen())
                {
                    //Relative_y = 310;
                    Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.NotesFont, Color, Relative_x + Spacer_L, Relative_y + Calculated_y, MusicScore.Defaults.Scale.Tenths);
                }
                visual.Children.Add(note);
                DrawAdditionalLines(visual);
                if (HasDot) //! ignoring more than one dot //temp//
                {
                    DrawingVisual dot = new DrawingVisual();
                    using (DrawingContext dc = dot.RenderOpen())
                    {
                        DrawingHelpers.DrawString(dc, MusChar.Dot, TypeFaces.NotesFont, Brushes.Black, notepositionX + 15, notepositionY - dot_placement, MusicScore.Defaults.Scale.Tenths);
                    }
                    visual.Children.Add(dot);
                }
                if (Lyrics != null)
                {
                    DrawingVisual lyrics = new DrawingVisual();
                    Lyrics.Draw(lyrics);
                    visual.Children.Add(lyrics);
                }
                if (Accidental != null)
                {
                    DrawingVisual acc_symbol = new DrawingVisual();
                    Accidental.Draw(acc_symbol);
                    visual.Children.Add(acc_symbol);
                }
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
        /// <summary>
        /// Draws additional lines if note is placed above or below regular 5Lines Staff
        /// </summary>
        /// <param name="visual"></param>
        private void DrawAdditionalLines(DrawingVisual visual)
        {
            if (Pitch.CalculatedStep >= 0 || Pitch.CalculatedStep <= -12)
            {
                int numofaddlines = Math.Abs(Pitch.CalculatedStep);
                int lines = (numofaddlines / 2) + 1;

                if (lines > 0)
                {
                    int modaddlines = numofaddlines % 2;
                    DrawingVisual missinglines = new DrawingVisual();
                    int inverter = 1;
                    int oddevenstep = Pitch.CalculatedStep;
                    if (oddevenstep <= -12) //! while additional lines should be above measure
                    {
                        inverter = -1;
                        lines -= 6;
                    }
                    if (Math.Abs(oddevenstep) % 2 == 1)
                    {
                        if (Pitch.CalculatedStep <= -12)//! invert placement of line (from above to below of noteDot
                        {
                            oddevenstep = Pitch.CalculatedStep + 1;
                        }
                        else
                        {
                            oddevenstep = Pitch.CalculatedStep - 1;
                        }

                    }
                    for (int i = 0; i < lines; i++)
                    {
                        int step = oddevenstep - (i * 2) * inverter;
                        DrawingVisual addlinesbetween = new DrawingVisual();
                        using (DrawingContext dc = addlinesbetween.RenderOpen())
                        {
                            Misc.DrawingHelpers.DrawString(dc, MusChar.NoteLine, TypeFaces.NotesFont, Color, Relative_x + Spacer_L * 0.8f, (Relative_y + step * (MusicScore.Defaults.Scale.Tenths * 0.1f)) + MusicScore.Defaults.Scale.Tenths * 0.6f, MusicScore.Defaults.Scale.Tenths);
                        }
                        missinglines.Children.Add(addlinesbetween);
                    }
                    visual.Children.Add(missinglines);
                }
                //visual.Children.Add(additionalline);
            }
        }
        public override string ToString()
        {
            string result = $"Position {Relative_x.ToString("0.#")}X, {Relative_y.ToString("0.#")}Y, Width: {Width.ToString("0.#")} {Pitch.Step}{Pitch.Octave}";
            return result;
        }
        //! changed/refactored
        //private void AddBeam(Beam beam)
        //{
        //    if (BeamsList == null)
        //    {
        //        beamlist = new Dictionary<int, Beam>();
        //    }
        //    beamlist[beam.BeamNumber] = beam;
        //}
    }
    public class Accidental
    {
        private XElement xmldefinition;
        private bool hasparentheses = false;
        private AccidentalText accidentaltype;
        private bool iscautionary = false;
        private string symbol = "??";
        private string noteid;

        public XElement XMLDefinition { get { return xmldefinition; } }
        public bool HasParentheses { get { return hasparentheses; } }
        public AccidentalText AccidentalType { get { return accidentaltype; } }
        public bool IsCautionary { get { return iscautionary; } }
        public string Symbol { get { return symbol; } }
        public string NoteID { get { return noteid; } set { noteid = value; } }

        public Accidental(XElement x)
        {
            xmldefinition = x;
            if (x.HasAttributes)
            {
                if (x.Attribute("cautionary") != null)
                {
                    iscautionary = x.Attribute("cautionary").Value == "yes" ? true : false;
                }
                if (x.Attribute("parentheses") != null)
                {
                    hasparentheses = x.Attribute("parentheses").Value == "yes" ? true : false;
                }
            }
            GetAccidentalType(x.Value);
            GetAccidentalSymbol();
        }
        private void GetAccidentalSymbol()
        {
            if (AccidentalType != AccidentalText.other)
            switch (accidentaltype)
            {
                case AccidentalText.natural:
                    symbol = MusChar.Natural;
                    break;
                case AccidentalText.flat:
                    symbol = MusChar.Flat;
                    break;
                case AccidentalText.sharp:
                    symbol = MusChar.Sharp;
                    break;
                case AccidentalText.doublesharp:
                    symbol = MusChar.DoubleSharp;
                    break;
                case AccidentalText.flatflat:
                    symbol = MusChar.DoubleFlat;
                    break;
                default:
                    break;
            }
        }
        private void GetAccidentalType(string name)
        {
            accidentaltype = AccidentalText.other;
            switch (name)
            {
                case "natural":
                    accidentaltype = AccidentalText.natural;
                    break;
                case "flat":
                    accidentaltype = AccidentalText.flat;
                    break;
                case "sharp":
                    accidentaltype = AccidentalText.sharp;
                    break;
                case "double-sharp":
                    accidentaltype = AccidentalText.doublesharp;
                    break;
                case "flat-flat":
                    accidentaltype = AccidentalText.flatflat;
                    break;
                default:
                    accidentaltype = AccidentalText.other;
                    Logger.Log($"Accidental not found <{name}>");
                    break;
            }
        }
        public void Draw(DrawingVisual visual)
        {
            Note noteposition = (Note)Misc.ScoreSystem.GetSegment(NoteID);
            Point accidentalposition = new Point(noteposition.NoteHeadPosition.X - 14, noteposition.NoteHeadPosition.Y - 30);
            using (DrawingContext dc = visual.RenderOpen())
            {
                Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.NotesFont, Brushes.Black, (float)accidentalposition.X, (float)accidentalposition.Y, MusicScore.Defaults.Scale.Tenths);
            }
        }
    }

    public class Beam
    {
        
        private Beam_type typ;
        private Dictionary<int,int> beamlist = new Dictionary<int, int>();
        private float pos;
        private int number;
        private string noteid;

        public Beam_type BeamType { get { return typ; } }
        public Dictionary<int,int> NoteBeamList { get { return beamlist; } }
        public float Position { get { return pos; } }
        public int BeamNumber { get { return number; } }
        public int NoteBeamsCount { get { return beamlist.Count; } }
        public string NoteId { get { return noteid; } }
        
        /// <summary>
        /// OBSOLETE .ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="n"></param>
        /// <param name="p"></param>
        /// <param name="l"></param>
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
        public Beam(XElement x, string id)
        {
            if (x.HasAttributes)
            {
                number = int.Parse(x.Attribute("number").Value);
            }
            typ = getBeamType(x.Value);
            noteid = id;
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
        
        public static void Draw(DrawingVisual beam, List<string> beams)
        {
            List<List<Beam>> beamlist = new List<List<Beam>>();
            List<Note> notelist = new List<Note>();
            try
            {
                foreach (var item in beams)
                {
                    Note n = (Note)Misc.ScoreSystem.GetSegment(item);
                    notelist.Add(n);
                    beamlist.Add(n.BeamsList);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Exception thrown {e.ToString()}", "Warning - Exception thrown", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
            var l = beamlist.Select(z => z.Select(u => u.BeamNumber).Max()).ToList().Distinct().Max();
            float offset = 6f;
            for (int i = 0; i < l; i++) //TODO improve offset while stem_dir_down
            {
                int beamnumber = i + 1;
                Point previous = new Point();
                Point current = new Point();
                DrawingVisual segment = new DrawingVisual();

                for (int j = 0; j < notelist.Count; j++)
                {
                    float x_with_offset = notelist.ElementAt(j).Stem_dir ? (float)notelist.ElementAt(j).NoteHeadRightLink.X :(float) notelist.ElementAt(j).NoteHeadLeftLink.X;
                    float y_offset = notelist.ElementAt(j).Stem_dir ? notelist.ElementAt(j).Stem.Length + 14 : notelist.ElementAt(j).Stem.Length + 15; //! this should be substr. from relative_y of note
                    y_offset -= 25;
                    if (notelist.ElementAt(j).Stem_dir == false)
                    {
                        if (offset > 0)
                        {
                            offset = offset * -1;
                        }
                    }
                    else
                    {
                        offset = Math.Abs(offset);
                    }
                    foreach (var item in notelist.ElementAt(j).BeamsList)
                    {
                        if (item.BeamNumber == beamnumber)
                        {
                            if (item.BeamType == Beam_type.start)
                            {
                                previous.X = x_with_offset;
                                previous.Y = notelist.ElementAt(j).Relative_y - y_offset;
                                previous.Y += offset * i;
                            }
                            else{
                                if (item.BeamType == Beam_type.forward)
                                {

                                }
                                else
                                {
                                    if (item.BeamType == Beam_type.backward)
                                    {

                                    }
                                    else
                                    {
                                        current.X = x_with_offset;
                                        current.Y = notelist.ElementAt(j).Relative_y - y_offset;
                                        current.Y += offset * i;
                                        DrawingVisual beam_segment = new DrawingVisual();
                                        Misc.DrawingHelpers.DrawLine(beam_segment, previous, current);
                                        segment.Children.Add(beam_segment);
                                        previous.X = x_with_offset;
                                        previous.Y = notelist.ElementAt(j).Relative_y - y_offset;
                                       
                                    }
                                }
                            }
                        }
                    }
                }
                beam.Children.Add(segment);
            }
        }

        public override string ToString()
        {
            string objecttostring = "";
            objecttostring = $"{NoteId}: {BeamNumber}, {Position}, {BeamType.ToString()}";
            return objecttostring;
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

    public class Stem //TODO_H add auto calculation of stem for beams
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
        public void Draw(DrawingVisual visual, Point startingpoint, Point endingpoint)
        {
            DrawingVisual stem_dv = new DrawingVisual();
            using(DrawingContext dc = stem_dv.RenderOpen())
            {

                Pen pen = new Pen(Brushes.Black, 1f);
                Point startpoint =new Point(startingpoint.X, startingpoint.Y - Length);
                Point endpoint = new Point(endingpoint.X, endingpoint.Y + 30);
                if (StemDirection == NoteStem.down)
                {
                    //endpoint.Y -= len;
                    startpoint.Y += 9;
                }
                else
                {
                    startpoint.X += 9;
                    startpoint.Y += 14;
                    endpoint.X += 9;
                    //endpoint.Y += len;
                }
                dc.DrawLine(pen, startpoint, endpoint);
            }
            visual.Children.Add(stem_dv);
        }
    }

    public enum NoteStem
    {
        none = 0,
        up = 1,
        down = 2
    }
  
}
