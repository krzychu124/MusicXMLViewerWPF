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
using MusicXMLScore.Helpers;

namespace MusicXMLViewerWPF
{
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

    public enum NoteStem
    {
        none = 0,
        up = 1,
        down = 2
    }

    public class Accidental
    {
        #region Private Fields

        private AccidentalText accidentaltype;
        private bool hasparentheses = false;
        private bool iscautionary = false;
        private string noteid;
        private string symbol = "??";
        private XElement xmldefinition;

        #endregion Private Fields

        #region Public Constructors

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

        #endregion Public Constructors

        #region Public Properties

        public AccidentalText AccidentalType { get { return accidentaltype; } }
        public bool HasParentheses { get { return hasparentheses; } }
        public bool IsCautionary { get { return iscautionary; } }
        public string NoteID { get { return noteid; } set { noteid = value; } }
        public string Symbol { get { return symbol; } }
        public XElement XMLDefinition { get { return xmldefinition; } }

        #endregion Public Properties

        #region Public Methods
        //TODO Refactor Draw()
        /*? 
        public void Draw(DrawingVisual visual)
        {
            Note noteposition = (Note)Misc.ScoreSystem.GetSegment(NoteID);
            Point accidentalposition = new Point(noteposition.NoteHeadPosition.X - 14, noteposition.NoteHeadPosition.Y - 30);
            using (DrawingContext dc = visual.RenderOpen())
            {
                Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.NotesFont, Brushes.Black, (float)accidentalposition.X, (float)accidentalposition.Y, 40);
            }
        }*/

        #endregion Public Methods

        #region Private Methods

        private void GetAccidentalSymbol()
        {
            if (AccidentalType != AccidentalText.other)
                switch (accidentaltype)
                {
                    case AccidentalText.natural:
                        symbol = MusicalChars.Natural;
                        break;
                    case AccidentalText.flat:
                        symbol = MusicalChars.Flat;
                        break;
                    case AccidentalText.sharp:
                        symbol = MusicalChars.Sharp;
                        break;
                    case AccidentalText.doublesharp:
                        symbol = MusicalChars.DoubleSharp;
                        break;
                    case AccidentalText.flatflat:
                        symbol = MusicalChars.DoubleFlat;
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

        #endregion Private Methods
    }

    public class Beam
    {

        #region Private Fields

        private Dictionary<int, int> beamlist = new Dictionary<int, int>();
        private string noteid;
        private int number;
        private float pos;
        private Beam_type typ;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// OBSOLETE .ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="n"></param>
        /// <param name="p"></param>
        /// <param name="l"></param>
        public Beam(string type, int n, float p, Dictionary<int, string> l)
        {
            number = n;
            pos = p;
            typ = getBeamType(type);
            foreach (var item in l)
            {
                Beam_type b = getBeamType(item.Value);
                beamlist.Add(item.Key, Convert.ToInt32(b));
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

        #endregion Public Constructors

        #region Public Enums

        public enum Beam_type
        {
            start,
            next,
            stop,
            forward,
            backward
        }

        #endregion Public Enums

        #region Public Properties

        public int BeamNumber { get { return number; } }
        public Beam_type BeamType { get { return typ; } }
        public Dictionary<int, int> NoteBeamList { get { return beamlist; } }
        public int NoteBeamsCount { get { return beamlist.Count; } }
        public string NoteId { get { return noteid; } }
        public float Position { get { return pos; } }

        #endregion Public Properties

        #region Public Methods

        public static void Draw(DrawingVisual beam, List<string> beams) //TODO_I Refactor necessary - disabled for now
        {
            List<List<Beam>> beamlist = new List<List<Beam>>();
            List<Note> notelist = new List<Note>();
            try
            {
                foreach (var item in beams)
                {
                    Note n = null; //?(Note)Misc.ScoreSystem.GetSegment(item);
                    //notelist.Add(n);
                   // beamlist.Add(n.BeamsList);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Exception thrown {e.ToString()}", "Warning - Exception thrown", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var l = beamlist.Select(z => z.Select(u => u.BeamNumber).Max()).ToList().Distinct().Max();
            float offset = 6f;
            for (int i = 0; i < l; i++) //TODO_L improve offset while stem_dir_down
            {
                int beamnumber = i + 1;
                Point previous = new Point();
                Point current = new Point();
                DrawingVisual segment = new DrawingVisual();

                for (int j = 0; j < notelist.Count; j++)
                {
                    float x_with_offset = notelist.ElementAt(j).Stem_dir ? (float)notelist.ElementAt(j).NoteHeadRightLink.X : (float)notelist.ElementAt(j).NoteHeadLeftLink.X;
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
                            else
                            {
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

        #endregion Public Methods

        #region Private Methods

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

        #endregion Private Methods
    }

    public class Stem //TODO_L add auto calculation of stem for beams
    {
        #region Private Fields

        private string direction;
        private float length;
        private NoteStem stemDirection;

        #endregion Private Fields

        #region Public Constructors

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

        #endregion Public Constructors

        #region Public Properties

        public string Direction { get { return direction; } set { direction = value; } }
        public float Length { get { return length; } set { length = value; } }
        public NoteStem StemDirection { get { return stemDirection; } set { stemDirection = value; } }

        #endregion Public Properties

        #region Public Methods

        public void Draw(DrawingVisual visual, Point startingpoint, Point endingpoint)
        {
            DrawingVisual stem_dv = new DrawingVisual();
            using (DrawingContext dc = stem_dv.RenderOpen())
            {

                Pen pen = new Pen(Brushes.Black, 1f);
                Point startpoint = new Point(startingpoint.X, startingpoint.Y - Length);
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

        #endregion Public Methods
    }

    class Note : Segment, IDrawableMusicalObject
    { //Need to be reworked !! 1)little improvements done

        #region Protected Fields

        protected Accidental accidental;
        protected Beam beam;
        protected List<Beam> beamlist;
        protected int clefalter;
        protected float defaultStem = 30f;
        protected int dot;
        protected int duration;
        protected bool hasBeams;
        protected bool hasDot;
        protected bool hasNotations;
        protected int id;
        protected bool isCustomStem;
        protected bool isGraceNote = false;
        protected bool isRest;
        protected Lyrics lyrics;
        protected string measure_id;
        protected List<Notations> notationsList;
        protected Point noteheadposition;
        protected Pitch pitch;
        protected float posX;
        protected float posY;
        protected Stem stem;
        protected bool stem_dir;
        protected float stem_f;
        protected string symbol;
        protected MusSymbolDuration symbol_type;
        protected string symbol_value;
        protected int voice;
        protected XElement xmldefinition;

        #endregion Protected Fields

        #region Private Fields

        private CanvasList drawablemusicalobject;

        private DrawableMusicalObjectStatus drawableobjectstatus;

        private bool loadstatus;
        private bool isUnpitched;

        #endregion Private Fields

        #region Public Constructors
        
        //public new Point Relative { get { return base.Relative; } set { base.Relative = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Relative))); } }
        public Note(Pitch p, int duration)
        {
            NotePropertyChanged += Note_PropertyChanged;
            Pitch = p;
            Duration = duration;
            Segment_type = SegmentType.Chord;
            SymbolXMLValue = "quarter";
            SymbolType = SymbolDuration.DurStrToMusSymbol(SymbolXMLValue);
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
            SetCalculatedNotePosition();
            Loaded = true;
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
                    case "unpitched":
                        Pitch = new Pitch(item.Element("display-step").Value, int.Parse(item.Element("display-octave").Value), ClefAlter);
                        Unpitched = true;
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
                        SymbolType = SymbolXMLValue != null ? SymbolDuration.DurStrToMusSymbol(SymbolXMLValue) : MusSymbolDuration.Unknown;
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
                        //IsCustomStem = true;
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
            Loaded = true;
        }

        public Note()
        {
            //PropertyChanged += Note_PropertyChanged;

        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler NotePropertyChanged = delegate { };

        #endregion Public Events

        #region Public Properties

        public Accidental Accidental { get { return accidental; } }
        public Beam Beam { get { return beam; } protected set { beam = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Beam))); } }
        public List<Beam> BeamsList { get { return beamlist; } }
        public int ClefAlter { get { return clefalter; } set { clefalter = value; } }
        public Brush Color { get { return Brushes.Black; } }
        public float DefaultStem { get { return defaultStem; } protected set { defaultStem = value; } }
        public int Dot { get { return dot; } protected set { dot = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dot))); } }
        public virtual CanvasList DrawableMusicalObject { get { return drawablemusicalobject; } set { drawablemusicalobject = value; } }
        public virtual DrawableMusicalObjectStatus DrawableObjectStatus { get { return drawableobjectstatus; } set { if (drawableobjectstatus != value) drawableobjectstatus = value; NotePropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(DrawableObjectStatus))); } }
        public int Duration { get { return duration; } protected set { duration = value; } }
        public bool HasBeams { get { return hasBeams; } protected set { hasBeams = value; } }
        public bool HasDot { get { return hasDot; } protected set { hasDot = value; } }
        public bool HasNotations { get { return hasNotations; } protected set { hasNotations = value; } }
        public int Id { get { return id; } protected set { } }
        public bool IsCustomStem { get { return isCustomStem; } protected set { isCustomStem = value; } }
        public bool IsGraceNote { get { return isGraceNote; } protected set { isGraceNote = value; } }
        public bool IsRest { get { return isRest; } protected set { isRest = value; } }
        public bool Loaded { get { return loadstatus; } private set { if (loadstatus != value) loadstatus = value; NotePropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Loaded))); } }
        public Lyrics Lyrics { get { return lyrics; } protected set { lyrics = value; } }
        public string MeasureId { get { return measure_id; } set { measure_id = value; } }
        public List<Notations> NotationsList { get { return notationsList; } protected set { notationsList = value; } }
        public Point NoteHeadLeftLink { get { return new Point(NoteHeadPosition.X - (40 * 0.225f) / 2, NoteHeadPosition.Y); } }
        public Point NoteHeadPosition { get { return noteheadposition; } }
        //TODO MusicScore.Defaults.Scale.Tenths
        public Point NoteHeadRightLink { get { return new Point(NoteHeadPosition.X + (40 * 0.225f) / 2, NoteHeadPosition.Y); } }

        public Pitch Pitch { get { return pitch; } protected set { pitch = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pitch))); } }
        public float PosX { get { return posX; } protected set { } }
        public float PosY { get { return posY; } protected set { } }
        public Stem Stem { get { return stem; } protected set { stem = value; } }
        public bool Stem_dir { get { return stem_dir; } protected set { stem_dir = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stem_dir))); } }
        public float StemF { get { return stem_f; } protected set { stem_f = value; } }
        public string Symbol { get { return symbol; } protected set { symbol = value; } }
        public MusSymbolDuration SymbolType { get { return symbol_type; } set { symbol_type = value; NotePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SymbolType))); } }
        public string SymbolXMLValue { get { return symbol_value; } set { symbol_value = value; } }
        public int Voice { get { return voice; } protected set { voice = value; } }
        public XElement XMLDefinition { get { return xmldefinition; } }
        public bool Unpitched { get { return isUnpitched; } set { isUnpitched = value; } }
        
        #endregion Public Properties

        #region Public Methods

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
                    Misc.DrawingHelpers.DrawString(dc, MusicalChars.QuarterDot, TypeFaces.NotesFont, Color, Relative_x + Spacer_L, Relative_y + Calculated_y, 40); //TODO MusicScore.Defaults.Scale.Tenths
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
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.Dot, TypeFaces.NotesFont, Brushes.Black, notepositionX + 15, notepositionY - dot_placement, 40); //TODO MusicScore.Defaults.Scale.Tenths
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
                    //Accidental.Draw(acc_symbol);
                    visual.Children.Add(acc_symbol);
                }
            }
            else //! If custom stem length - got from XML file
            {
                DrawingVisual note = new DrawingVisual();
                using (DrawingContext dc = note.RenderOpen())
                {
                    //Relative_y = 310;
                    Misc.DrawingHelpers.DrawString(dc, Symbol, TypeFaces.NotesFont, Color, Relative_x + Spacer_L, Relative_y + Calculated_y, 40);
                }
                visual.Children.Add(note);
                DrawAdditionalLines(visual);
                if (HasDot) //! ignoring more than one dot //temp//
                {
                    DrawingVisual dot = new DrawingVisual();
                    using (DrawingContext dc = dot.RenderOpen())
                    {
                        Misc.DrawingHelpers.DrawString(dc, MusicalChars.Dot, TypeFaces.NotesFont, Brushes.Black, notepositionX + 15, notepositionY - dot_placement, 40);
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
                    //Accidental.Draw(acc_symbol);
                    visual.Children.Add(acc_symbol);
                }
            }

        }

        public void InitDrawableObject()
        {
            if (Loaded)
            {
                DrawableMusicalObject = new CanvasList(this.Width, this.Height);
                DrawingVisual note = new DrawingVisual();
                Draw(note);
                DrawableMusicalObject.AddVisual(note);
                DrawableObjectStatus = DrawableMusicalObjectStatus.ready;
            }
        }

        public void ReloadDrawableObject()
        {
            DrawableMusicalObject.ClearVisuals();
            DrawableObjectStatus = DrawableMusicalObjectStatus.notready;
            InitDrawableObject();
        }

        //public override string ToString()
        //{
        //    string result = $"Position {Relative_x.ToString("0.#")}X, {Relative_y.ToString("0.#")}Y, Width: {Width.ToString("0.#")} {Pitch.Step}{Pitch.Octave}";
        //    return result;
        //}

        #endregion Public Methods

        #region Protected Methods

        protected void Note_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SymbolType":
                    Logger.Log($"{sender.ToString()} SymbolType set");
                    break;
                case "Stem_dir":
                    Symbol = MusicalChars.getNoteSymbol(SymbolXMLValue, Stem_dir);
                    Logger.Log($"{sender.ToString()}, Stem direction set");
                    break;
                case "Relative":
                    SetCalculatedNotePosition();
                    Logger.Log("Recalculated NoteSymbol and Notehead positions");
                    break;
                case "Loaded":
                    if (Loaded)
                    {
                        InitDrawableObject();
                    }
                    break;
                case "DrawableObjectStatus":
                    if (DrawableObjectStatus == DrawableMusicalObjectStatus.reload)
                    {
                        ReloadDrawableObject();
                    }
                    break;
                default:
                    Logger.Log($"NotePorpertyChanged for {e.PropertyName} not implenmented");
                    break;
            }
        }

        protected void SetSegmentColor(Brush brush)
        {
            base.Color = brush;
        }

        #endregion Protected Methods

        #region Private Methods
        
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
                            Misc.DrawingHelpers.DrawString(dc, MusicalChars.NoteLine, TypeFaces.NotesFont, Color, Relative_x + Spacer_L * 0.8f, (Relative_y + step * (40 * 0.1f)) + 40 * 0.6f, 40); //todo MusicScore.Defaults.Scale.Tenths where: 40
                        }
                        missinglines.Children.Add(addlinesbetween);
                    }
                    visual.Children.Add(missinglines);
                }
                //visual.Children.Add(additionalline);
            }
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

        private Point GetNoteHeadPosition(string v)
        {
            float offset = 0.225f * 40; //TODO Scale // default offset at scale 40 is 9 so 0.225

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
        private void SetCalculatedNotePosition()
        {
            Calculated_y = Pitch.CalculatedStep * (40 * 0.1f) + 40 * 0.6f; //TODO MusicScore.Defaults.Scale.Tenths
            noteheadposition = new Point(Relative_x + Spacer_L + 5, Relative_y + Calculated_y + 30);
        }

        #endregion Private Methods

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
}
