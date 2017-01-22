﻿using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.ScoreParts.MeasureContent
{
    class Measure : Segment, IDrawable, INotifyPropertyChanged // NEEDIMPROVEMENTS: Finish reworking class
    {
        #region Fields for drawing

        #endregion
        #region Fields
        //--------------------
        private int number;
        //private float width; // default value is 100 may change later ?
        //--------------------
        private Attributes attributes;
        private bool hasNumberInvisible;
        private DrawingVisual visual;
        private int elements_count;
        private Point pos;
        private Print print_properties;
        //
        private bool firstinrow = false;
        private XElement xelementsource;
        private Segment segment;
        private DrawableMeasure drawableMeasure;
        private bool loaded;
        #endregion

        #region Collections
        private List<Barline> barlines = new List<Barline>();
        private List<Direction> direction = new List<MusicXMLViewerWPF.Direction>();
        private List<Note> notes_list = new List<Note>(); // experimental
        private List<Segment> music_characters = new List<Segment>();
        private Misc.Segments segments = new Misc.Segments();
        private Dictionary<string, List<Beam>> beams;
        private List<List<Notations>> nlist = new List<List<Notations>>();
        private Dictionary<int, List<Note>> notesbyvoice;
        private List<List<string>> sortedbeams;
       // private Dictionary<int,>
        #endregion

        #region Properties
        public new event PropertyChangedEventHandler PropertyChanged;
        public Dictionary<int, List<Note>> NotesByVoice { get { return notesbyvoice; } set { notesbyvoice = value; } }
        public Attributes Attributes { get { return attributes; } set { attributes = value; } }
        public bool NumberVisible { get { return hasNumberInvisible; } }
        public DrawingVisual Visual { get { return visual; } set { if (value != null) visual = value; } }
        public new float Width { get { return base.Width; } set { if (value >= 0) { base.Width = value; } else { base.Width = 100f; Logger.Log("width is negative here"); } } }
        public int ElementsCount { get { return elements_count; } }
        public int Number { get { return number; } }
        public List<Barline> Barlines { get { return barlines; } }
        public List<Direction> DirectionList { get { return direction; } }
        public List<Note> NotesList { get { return notes_list; } set { if (value != null) notes_list = value; } } // Not complete
        public List<Segment> MusicCharacters { get { return music_characters; } }
        public Point Position { get { return pos; } set { if (value != null) pos = value; } }
        public Print PrintProperties { get { return print_properties; } }
        public bool IsFirstInLine { get { return firstinrow; } set { firstinrow = value; }  }
        public new Point Relative { get { return base.Relative; } set { base.Relative = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Relative))); } }
        public XElement XElementSource { get { return xelementsource; } set { if (value != null) xelementsource = value; } }
        public Segment Segment { get { return segment; } set { if (value != null) segment = value; } }
        public DrawableMeasure DrawableMeasure { get { return drawableMeasure; } private set { drawableMeasure = value; } }
        public bool Loaded { get { return loaded; } private set { if (loaded != value) { loaded = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Loaded))); } } }
        #endregion

        public Measure()
        {

        }

        public Measure(double width)
        {
            this.Width = (float)width;
        }

        public Measure(XElement x)
        {
            ID = Misc.RandomGenerator.GetRandomHexNumber();
            XElementSource = x;
            XMLExtractMeasureInfo(x);
            //PropertyChanged += segment_Properties_Ready;
            PropertyChanged += Measure_PropertyChanged;
            if (x.Elements("direction").Any() == false)
            {
                direction = null;
            }
            int el_num = x.Elements().Count();
            int index = 0;
            
            foreach (var element in x.Elements())
            {
                index++;
                //XMLFiller(x);
                
                if (element.Name.LocalName == "print")
                {
                    XMLExtractPrint(element);
                }

                if (element.Name.LocalName == "note")
                {
                        if (element.Element("rest") != null)
                        {
                            XMLExtractRests(element);
                        }
                        else
                        {
                            XMLExtractNotes(element);
                        }
                }

                if (element.Name.LocalName == "direction")
                { 
                    XMLExtractDirections(element);
                }

                if (element.Name.LocalName == "attributes")
                {
                    XMLExtractAttributes(element);
                }

                if (element.Name.LocalName == "barline")
                {
                    XMLExtractBarlines(element);
                }
                if (index == el_num)
                {
                    //! vv Adds clef object to every first measure in line from previous measure 
                    if (Number != 1 && IsFirstInLine)
                    {
                        attributes = new Attributes(MusicXMLViewerWPF.Clef.ClefStatic);
                        music_characters.Insert(0,Attributes.Clef);
                    }
                    foreach (var item in MusicCharacters)
                    {
                        segments.Add(item);
                    }
                    RecalculateSegmentXPos();
                    //! Generate segments () ... 
                }
            }
            //if (barlines.Count ==0 || barlines.Where(b => b.Style != Barline.BarStyle.regular).Any()) 
            //{
                barlines.Add(new Barline() { Style = Barline.BarStyle.regular, Location = Barline.BarlineLocation.right }); // adding default barline for drawing later
            //}
            //foreach (var item in Misc.ScoreSystem.Segments)
            //{
            //    Logger.Log($"Added segment {item.ToString()}");
            //}
            SortNotations(); // missing impl
            SortNotesByVoice();
            SortBeamsByNumber();
            Loaded = true;
        }

        private void Measure_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Relative":
                    foreach (var item in MusicCharacters)
                    {
                        item.SetRelativePos(Relative_x + item.Calculated_x, Relative_y);
                    }
                    break;
                case "Loaded":
                    DrawableMeasure = new DrawableMeasure(this);
                    break;
                default:
                    break;
            }
        }

        private void SortNotations()
        {
            for (int i = 0; i < nlist.Count; i++)
            {

            }
        }

        private void SortNotesByVoice()
        {
            if (NotesList.Count != 0)
            {
                var voices = NotesList.Select(x => x.Voice).Distinct().ToList();
                if (voices.Last() != 1)
                {
                    notesbyvoice = new Dictionary<int, List<Note>>();
                    foreach (var voice in voices)
                    {
                        int v = voice;
                        List<Note> nlist = new List<Note>();
                        foreach (var item in NotesList)
                        {
                            if (item.Voice == v)
                            {
                                nlist.Add(item);
                            }
                        }
                        NotesByVoice.Add(v, nlist);
                    }
                }
            }
        }

        private void SortBeamsByNumber() //TODO_L sorting method not finished, get every beam of number from each note in measure, put to <<note.ID>, <<beam.number>, <list.beamsofnumbers>>> 
        {
            if (beams != null)
            {
                sortedbeams = new List<List<string>>();
                List<List<string>> sbeam;
                int beamseqcount = 0;
                foreach (var item in beams)
                {
                    foreach (var i in item.Value)
                    {
                        if (i.BeamNumber == 1 && i.BeamType == Beam.Beam_type.stop)
                        {
                            beamseqcount += 1;
                        }
                    }
                }
                List<string> noteids = new List<string>();
                sbeam = new List<List<string>>();
                int index = 1;
                foreach (var note in beams)
                {

                    foreach (var item in note.Value)
                    {
                        Beam beam = item;
                        if (beam.BeamNumber == 1)
                        {
                            if (beam.BeamType == Beam.Beam_type.next || beam.BeamType == Beam.Beam_type.start)
                            {
                                noteids.Add(beam.NoteId);
                            }
                            if (beam.BeamType == Beam.Beam_type.stop)
                            {
                                noteids.Add(beam.NoteId);
                                //sortedbeams[index] = new List<string>(noteids);
                                sortedbeams.Add(new List<string>(noteids));
                                index++;
                                noteids.Clear();
                            }
                        }
                    }

                }
            }
        }

        #region Simple Musical Object Add-Methods
        /// <summary>
        /// Adds Rest to measure
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="durationsymbol"></param>
        public void AddRest(string duration, MusSymbolDuration durationsymbol = MusSymbolDuration.Unknown)
        {
            if (durationsymbol != MusSymbolDuration.Unknown)
            {
                duration = SymbolDuration.MusSymbolToDurStr(durationsymbol);
            }
            Rest rest = new Rest(duration, new Point());
            NotesList.Add(rest);
            
        }
        
        public void AddNote(Pitch pitch, int duration)
        {
            Note n = new Note(pitch, duration);
            NotesList.Add(n);
        }

        public void AddClef(ClefType ct, int line = 0)
        {
            Clef c = new Clef(ct, line);
            if (Attributes != null)
            {
                Attributes.Clef = c;
            }
            else
            {
                Attributes = new Attributes(c);
            }
        }

        public void AddTimeSig(int beat, int beattype, SignatureType signaturetype = SignatureType.number)
        {
            TimeSignature ts = new TimeSignature(beat, beattype, signaturetype);
            if (Attributes != null)
            {
                Attributes.Time = ts;
            }
            else
            {
                Attributes = new Attributes(ts);
            }
        }

        public void AddBarline()
        {
           // throw new NotImplementedException();
        }

        public void AddKeySig()
        {
            Key ks = new Key(2, "major");
            if (Attributes != null)
            {
                Attributes.Key = ks;
            }
            else
            {
                Attributes = new Attributes(ks);
            }
        }
#endregion

        /// <summary>
        /// Extract barlines from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractBarlines(XElement x)
        {
            Barline barline = new Barline(x);
            barlines.Add(barline);
            Misc.ScoreSystem.Segments.Add(barline.ID, new Segment() { ID = barline.ID, Segment_type = SegmentType.Barline, Color = Brushes.Gold });
        }
        /// <summary>
        /// Extract measure attributes (Clef, KeySig, Timesig) from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractAttributes(XElement x)
        {
            attributes =  new Attributes(x);
            if (Attributes.Clef != null) 
            {
                /*
                Clef ClefSegment = Attributes.Clef;
                ClefSegment.Segment_type = SegmentType.Clef;
                ClefSegment.Color = Brushes.Brown;
                Segment s = new Segment() { Segment_type = SegmentType.Clef, Color = Brushes.Brown };
                var c = music_characters.LastOrDefault(); //! gets last obj from list
                Point p = new Point();
                if (c == null)
                {
                    //! p = Calculate_Current_Position(s);
                    p = Calculate_Current_Position(ClefSegment);
                }
                else
                {
                    p = Calculate_Current_Position(c);
                }
              //!  s.Relative = Calc.Add(Calculated, p);
              //!  s.CalculateDimensions();
                ClefSegment.Relative = Calc.Add(Calculated, p);
                ClefSegment.CalculateDimensions();
                
              //! Misc.ScoreSystem.Segments.Add(s); //! (new Segment() { Segment_type = SegmentType.Clef, Color = Brushes.Brown });

                //! music_characters.Add(s); //! (new Segment() { Segment_type = SegmentType.Clef, Color = Brushes.Brown });
                */

                if (Attributes.Clef.Number != 0)
                {
                    if (Attributes.Clef.Number == 1)
                    {
                        Misc.ScoreSystem.Segments.Add(Attributes.Clef.ID, Attributes.Clef);
                        music_characters.Add(Attributes.Clef);
                    }
                }
                else
                {
                    Misc.ScoreSystem.Segments.Add(Attributes.Clef.ID, Attributes.Clef);
                    music_characters.Add(Attributes.Clef);
                }
            }
            if (Attributes.Key != null)
            {
                Misc.ScoreSystem.Segments.Add(Attributes.Key.ID, new Segment() { ID = Attributes.Key.ID, Segment_type = SegmentType.KeySig, Color = Brushes.Coral });
                music_characters.Add(Attributes.Key); //! new Segment() { Segment_type = SegmentType.KeySig, Color = Brushes.Coral }
            }
            if (Attributes.Time != null)
            {
                Misc.ScoreSystem.Segments.Add(Attributes.Time.ID, new Segment() { ID = Attributes.Time.ID, Segment_type = SegmentType.TimeSig, Color = Brushes.Lavender });
                music_characters.Add(Attributes.Time); //! new Segment() { ID = Attributes.Time.ID, Segment_type = SegmentType.TimeSig, Color = Brushes.Lavender }
            }
        }
        /// <summary>
        /// Extract directions (wedge, dynamics, coda etc.) from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractDirections(XElement x)
        {
            Direction directionobject = new Direction(x, ID);
            this.direction.Add(directionobject);
            Misc.ScoreSystem.Segments.Add(directionobject.ID, new Segment() { ID = directionobject.ID, Segment_type = SegmentType.Direction});
           // music_characters.Add(new Segment() { Segment_type = SegmentType.Direction, Color = Brushes.DarkTurquoise });
        }
        /// <summary>
        /// Extract rests from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractRests(XElement x)
        {
            Rest rest = new Rest(x);
            NotesList.Add(rest);
            //! debug; Logger.Log("missing impl for this rest");
            Misc.ScoreSystem.Segments.Add(rest.ID, rest); //! temp_str, new Segment() { ID = temp_str, Segment_type = SegmentType.Rest, Color = Brushes.DarkMagenta });
            music_characters.Add(rest);//! (new Segment() { ID = temp_str, Segment_type = SegmentType.Rest, Color = Brushes.DarkMagenta });
        }
        /// <summary>
        /// Extract notes from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractNotes(XElement x)
        {
            Note note = new Note(x, Clef.ClefAlterNote, ID); //? { ClefAlter = Clef.ClefAlterNote, MeasureId = ID };
            NotesList.Add(note);
            if (note.HasBeams)
            {
                if (beams == null) beams = new Dictionary<string, List<Beam>>();
                beams.Add(note.ID, note.BeamsList);
                //?  beams.Add(note.ID, note.BeamsList);
            }
            if (note.NotationsList != null)
            {
                nlist.Add(note.NotationsList);
            }
            //! debug; Logger.Log("missing impl for this note");
            Misc.ScoreSystem.Segments.Add(note.ID, note); //! temp_str, new Segment() { ID = temp_str, Segment_type = SegmentType.Chord, Color = Brushes.DarkOliveGreen});
            music_characters.Add(note);//! new Segment() { ID = temp_str, Segment_type = SegmentType.Chord, Color = Brushes.DarkOliveGreen });
        }
        /// <summary>
        /// Extract visual properties (custom margins, spacers, new_page, new_line etc.) from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractPrint(XElement x)
        {
            print_properties = new Print(x);
            if (print_properties != null)
            {
                if (print_properties.NewSystem)
                {
                    IsFirstInLine = true;
                }
            }    
        }

        public IEnumerable<XElement> XMLExtractor()
        {
            var x = Misc.LoadFile.Document;
            var extracted = from extr in x.Descendants("measure") select extr;
            return extracted;
        }
        /// <summary>
        /// Extract basic info about measure from XML element. eg. width, isVisible number
        /// </summary>
        /// <param name="x"></param>
        public void XMLExtractMeasureInfo(XElement x)
        {
            Width = x.Attribute("width") != null ? float.Parse(x.Attribute("width").Value, CultureInfo.InvariantCulture) : 0f;
            if (Width == 0f)
            {
                Logger.Log($"Measure {number} has no width: {Width}");
                int num = music_characters.Count();
                elements_count = num;
                AutoMeasureWidth(num);
            }
            bool t = int.TryParse(x.Attribute("number").Value, out number);
            if (t == false) Logger.Log($"Measure number is: {x.Attribute("number").Value}");
            //number = Convert.ToInt32(x.Attribute("number").Value);
            hasNumberInvisible = x.Attribute("implicit") != null ? x.Attribute("implicit").Value == "yes" ? true : false : false; // TODO_L not sure if itll work - very rare usage
        }

        /// <summary>
        /// Check measure for errors ( to many notes/wrong durations )
        /// </summary>
        /// <returns></returns>
        private bool IntegrityCheck()
        {
            throw new NotImplementedException();
            //return true;
        }
        /// <summary>
        /// Method calculating measure width according to number and type of elements which contains
        /// </summary>
        /// <param name="n"></param>
        private void AutoMeasureWidth(int n)
        {
            Width = 100f;
            float temp_width = 0f;
            foreach (var item in music_characters)
            {
                temp_width += item.Width;
            }
            if (temp_width > Width) Width = temp_width;
        }
        /// <summary>
        /// Calculate position of segment according to previous
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private Point Calculate_Current_Position(Segment s)
        {
            Point position = Calculated;
            position.X += s.Width;
            return position;
        }

        #region All Drawing Methods usable,ususable,bugged
        /// <summary>
        /// Drawing method used to draw measure with all contating elements (motes, rest, directions etc.)
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="p"></param>
        public void Draw(CanvasL surface,Point p) // drawing method
        {
            Position = p;

            DrawingVisual visual = new DrawingVisual();
            DrawingVisual visualMeasure = new DrawingVisual();
            using (DrawingContext dc = visualMeasure.RenderOpen())
            {
                Draw_Measure(dc, p);
            }
            visual.Children.Add(visualMeasure);

            foreach (var item in Barlines)// works quite good, need deep tests later
            {
                DrawingVisual barline_visual = new DrawingVisual();
                barline_visual = item.DrawBarline(barline_visual, p, Width);
                visual.Children.Add(barline_visual);
            }

            var ending = Barlines.Select(i => i).Where(i => i.Ending != null);
            foreach (var item in ending) // works quite good, need deep tests later
            {
                DrawingVisual ending_visual = new DrawingVisual();
                ending_visual = item.Ending.DrawEnding(ending_visual, p, Width);
                visual.Children.Add(ending_visual);

            }//Draw_Barlines(dc, p); replaced with different

            if (Attributes != null) // works quite good, need deep tests later
            {
                if (Barlines.Exists(i => i.Repeat != null))
                {
                    Attributes.Draw(visual, p, Width, true);
                }
                else
                {
                    Attributes.Draw(visual, p, Width); // visual will be opened inside, good results, maybe changed in the future
                }
            }
            if (DirectionList != null)
            {

                foreach (var item in DirectionList)
                {

                    item.Draw(visual, p);
                }

                //foreach (var item in Direction)
                //{
                //    if (item.DirectionList != null)
                //    {
                //        foreach (var item2 in item.DirectionList)
                //        {
                //            if (item2.Dynamics != null)
                //            {
                //                if (Attributes != null && Attributes.Key != null)
                //                {
                //                    p.X = p.X + 50f;
                //                }
                //                item2.Dynamics.Draw(visual, p);
                //            }
                //        }

                //    }
                //}

            }
            //Draw_Directions(dc2, p); // TODO_H missing implementation
            
            if (MusicCharacters.Count != 0)
            {
                Point temp = new Point(p.X + 20f, p.Y);
                temp.Y += 8f;
                DrawingVisual segments = new DrawingVisual();
                foreach (var character in MusicCharacters)
                {
                    if (character.Width == 0)
                    {
                        character.CalculateDimensions();
                        character.Relative = temp;
                        temp.X = character.Width + temp.X + 2f;

                    }
                    if (character.Width != 0)
                    {
                        DrawingVisual segment = new DrawingVisual();
                        character.Draw(segment, character.Color);
                        segments.Children.Add(segment);
                    }
                }
                visual.Children.Add(segments);
            }
            Visual = visual;
            surface.AddVisual(visual);

        }

        public void Draw(DrawingVisual visual, Point p)
        {
            throw new NotImplementedException();// maybe rework to use that approach
        }
        /// <summary>
        /// Draw Mesure (inside segment) - test
        /// </summary>
        /// <param name="visual"></param>
        public new void Draw(DrawingVisual visual)
        {
            using (DrawingContext dc = visual.RenderOpen())
            {
                Point temp = new Point(Calculated_x, Calculated_y); //TODO_I changed for testbuild
                //Point temp = new Point(0,0);
                CalculateXPosCharacter(temp);
                Draw_Measure(dc, temp);
            }

            DrawingVisual attr_vis = new DrawingVisual();
            Draw_Attributes(attr_vis);
            visual.Children.Add(attr_vis);
            DrawingVisual allnotes_vis = new DrawingVisual();
            Draw_AllNotes(allnotes_vis);
            visual.Children.Add(allnotes_vis);
            /*
            if (MusicCharacters.Any(i => i.Segment_type == SegmentType.Clef))
            {
                var character = MusicCharacters.First(z => z.Segment_type == SegmentType.Clef);
                DrawingVisual vis = new DrawingVisual();
                character.Draw(vis, character.Color);
                visual.Children.Add(vis);
            }*/
            foreach (var item in MusicCharacters)
            {
                DrawingVisual vis = new DrawingVisual();
                item.Draw(vis, item.Color);
                visual.Children.Add(vis);
                if (item.Segment_type == SegmentType.Clef)
                {
                    List<IDrawable> test = new List<IDrawable>();
                    Measure t = new Measure();
                    test.Add(t);
                }

            }
            if (DirectionList != null)
            {
                foreach (var item in DirectionList)
                {
                    if (DirectionList != null)
                    {
                        item.Draw(visual, Relative);

                        //if (item.DirectionList != null)
                        //{
                        //    foreach (var it in item.DirectionList)
                        //    {
                        //        if (it.Dynamics != null)
                        //        {
                        //            it.Dynamics.Draw(visual);
                        //        }
                        //    }
                        //}
                    }
                }
            }
            if (beams != null)
            {
                foreach (var item in sortedbeams)
                {
                    Beam.Draw(visual, item);
                }
                
            }
        }

        private void Draw_AllNotes(DrawingVisual notes_vis)
        {
            if (NotesList.Count != 0)
            {
                //foreach(Rest item in NotesList)
                //{
                //    //item.Draw(rests_vis);
                //}
                foreach(var item in NotesList)
                {
                    if (item.Voice == 1 || item.Voice == 0)
                    {
                        item.Draw(notes_vis);
                    }
                }
                //var rests = NotesList.OfType<Rest>();
                //foreach (var item in rests)
                //{
                //    item.Draw(rests_vis);
                //}
            }
        }

        public void Draw_(DrawingVisual visual)
        {
            foreach (var item in MusicCharacters)
            {
                DrawingVisual vis = new DrawingVisual();
                item.Draw(vis, item.Color);
                visual.Children.Add(vis);
            }
        }
        private void Draw_Directions(DrawingContext dc2, Point p)
        {
            throw new NotImplementedException();
        }

        private void Draw_Attributes(DrawingVisual attributes_visual)
        {
            if (Attributes != null)
            {
                Attributes.Draw(attributes_visual);
            }
        }
        /// <summary>
        /// Method for drawing staff of measure using width begining from StartPoint coords
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="StartPoint"></param>
        public void Draw_Measure(DrawingContext dc, Point StartPoint, Brush color = null)
        {
            if (color == null) color = Brushes.Black;
            float Scale = 40f;  //! MusicScore.Defaults.Scale.Tenths;
            float num = GetMeasureLength(Width);
            float filling = GetStaffLinesFilling(Width);
            float X = (float)StartPoint.X;
            float Y = (float)StartPoint.Y;
            int s = 0;

            for (int i = 0; i < num; i++)
            {
                Misc.DrawingHelpers.DrawString(dc, MusicalChars.Staff5L, TypeFaces.NotesFont, color, X + s, Y, Scale);
                s += 24;
            }

            if (filling != 0)
            {
                Misc.DrawingHelpers.DrawString(dc, MusicalChars.Staff5L, TypeFaces.NotesFont, color, X + (Width-24), Y, Scale);
            }
            if (IsFirstInLine)
            {
                //Misc.DrawingHelpers.DrawText(dc, Number.ToString(), new Point(Relative_x + 5f, Relative_y - 5f), 10f, withsub: false, color: Brushes.Black, font_weight:"regular");
            }
        }
        public void Draw_Measure(DrawingVisual dv, Point p)
        {
            DrawingVisual m = new DrawingVisual();
            using (DrawingContext dc = m.RenderOpen())
            {
                Draw_Measure(dc, p);
            }
            dv.Children.Add(m);
        }
        
        public void DrawMeasureStaff(DrawingVisual dv)
        {
            using (DrawingContext dc = dv.RenderOpen())
            {
                Draw_Measure(dc, new Point(0, 0));
            }
        }
        /// <summary>
        /// Method for Drawing barlines !!DEPRECIATED!!
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="StartPoint"></param>
        private void Draw_Barlines(DrawingContext dc, Point StartPoint)
        {
            //float Scale = MusicScore.Defaults.Scale.Tenths;
            //float X = (float)StartPoint.X;
            //float Y = (float)StartPoint.Y;
            //if (Barline != null)
            //{
            //    if (Barline.Style == Barline.BarStyle.regular)
            //    {
            //        if (Barline.Location == Barline.BarlineLocation.right)
            //        {
            //            Misc.DrawingHelpers.DrawString(dc, Number.ToString()+"BR", TypeFaces.TextFont, Brushes.Black, X + Width, Y-12, Scale / 4); // debug numbers
            //            Misc.DrawingHelpers.DrawString(dc, MusChar.RegularBar, TypeFaces.NotesFont, Brushes.Black, X + Width, Y, Scale);
            //            Logger.Log($"Regular barline: right in {Number} at {X + Width} {Y}");
            //        }
            //        if (Barline.Location == Barline.BarlineLocation.left)
            //        {
            //            Misc.DrawingHelpers.DrawString(dc, Number.ToString()+"BL", TypeFaces.TextFont, Brushes.Black, X + Width, Y-12, Scale / 4); // debug numbers
            //            Misc.DrawingHelpers.DrawString(dc, MusChar.RegularBar, TypeFaces.NotesFont, Brushes.Black, X + Width, Y, Scale);
            //            Logger.Log($"Regular barline: left in {Number} at {X} {Y}");
            //        }
            //    }
            //    if (Barline.Style == Barline.BarStyle.light_heavy)
            //    {
            //        if (Barline.Location == Barline.BarlineLocation.right)
            //        {
            //            if (Barline.Repeat != null)
            //            {
            //                if (Barline.Repeat.Direction == Repeat.RepeatDirection.backward)
            //                {
            //                    Misc.DrawingHelpers.DrawString(dc, Number.ToString()+"RB", TypeFaces.TextFont, Brushes.Black, X + Width, Y+48, Scale / 4); // debug numbers
            //                    Misc.DrawingHelpers.DrawString(dc, MusChar.RightRepeatBar, TypeFaces.NotesFont, Brushes.Black, X + Width - 11, Y, Scale);
            //                }
            //                if (Barline.Repeat.Direction == Repeat.RepeatDirection.forward)
            //                {
            //                    Misc.DrawingHelpers.DrawString(dc, Number.ToString()+"RF", TypeFaces.TextFont, Brushes.Black, X + Width, Y+48, Scale / 4); // debug numbers
            //                    Misc.DrawingHelpers.DrawString(dc, MusChar.LeftRepeatBar, TypeFaces.NotesFont, Brushes.Black, X + Width - 5, Y, Scale);
            //                }
            //            }
            //            else
            //            {
            //                Misc.DrawingHelpers.DrawString(dc, Number.ToString() + "F", TypeFaces.TextFont, Brushes.Black, X + Width, Y+48, Scale / 4); // debug numbers
            //                Misc.DrawingHelpers.DrawString(dc, MusChar.LightHeavyBar, TypeFaces.NotesFont, Brushes.Black, X + Width - 5, Y, Scale);
            //                Logger.Log($"Light-heavy barline: left in {Number} at {X + Width - 5} {Y}");
            //            }
            //            //Misc.DrawingHelpers.DrawString(dc, MusChar.FinalBar, TypeFaces.NotesFont, Brushes.Black, X + Width - 5, Y, Scale);
            //        }
            //    }
            //    if (Barline.Style == Barline.BarStyle.heavy_light)
            //    {
            //        if (Barline.Location == Barline.BarlineLocation.left)
            //        {
            //            Misc.DrawingHelpers.DrawString(dc, Number.ToString() + "HL", TypeFaces.TextFont, Brushes.Black, X, Y+48, Scale / 4); // debug numbers
            //            Misc.DrawingHelpers.DrawString(dc, MusChar.LeftRepeatBar, TypeFaces.NotesFont, Brushes.Black, X, Y, Scale);
            //            Misc.DrawingHelpers.DrawString(dc, Number.ToString() + "BR", TypeFaces.TextFont, Brushes.Black, X + Width, Y - 12, Scale / 4); // debug numbers
            //            Misc.DrawingHelpers.DrawString(dc, MusChar.RegularBar, TypeFaces.NotesFont, Brushes.Black, X + Width, Y, Scale);
            //            Logger.Log($"Heavy-light barline: left in {Number} at {X} {Y}");
            //        }
            //    }
            //}
        }
#endregion

        /// <summary>
        /// Gets measure lenght for later calculations
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private float GetMeasureLength(float length)
        {
            float num = Convert.ToInt32(Math.Floor(length / 24));
            return num;
        }
        /// <summary>
        /// Calculate length of empty space left between drawn staff and end of measure 
        /// (measure staff is made from segment, if adding next segment would make measure too long it needs to calculate position of filler)
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private float GetStaffLinesFilling(float l)
        {
            float fill = l % 24;
            return fill;
        }
        /// <summary>
        /// Calculates MusicCharacters segment positions in measure
        /// </summary>
        /// <param name="measurePosition"></param>
        private void CalculateXPosCharacter(Point measurePosition)
        {
            float calc_notes_width = GetNotesWidth(MusicCharacters);
            float calc_no_scalable_width = GetNotesWidth(MusicCharacters, false);
            float width_to_scale = Width - calc_no_scalable_width;
            float calc_scaling_width = width_to_scale - calc_notes_width;
            List<Segment> segmentsToScale = GetNoteRestList(MusicCharacters);
            if (calc_scaling_width > 0)
            {
                SegmentsWidthLower(segmentsToScale, calc_scaling_width);
                RecalculateSegmentXPos();
            }
            else
            {
                SegmentsWidthHigher(calc_scaling_width);
                //! needs implementation/ further tests
            }
        }
        /// <summary>
        /// Calculated Width of all elements in list. True - width of notes/rests, False width without notes/rests.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="notesOnly"></param>
        /// <returns></returns>
        private float GetNotesWidth(List<Segment> list, bool notesOnly= true)
        {
            float width = 0f;
            if (notesOnly)
            {
                foreach (var item in list)
                {
                    if (item.Segment_type == SegmentType.Chord)
                    {
                        width += item.Width;
                    }
                    if (item.Segment_type == SegmentType.Rest)
                    {
                        width += item.Width;
                    }
                }
            }
            else
            {
                foreach (var item in list)
                {
                    width += item.Width;
                }
                float notesWidth = GetNotesWidth(list, true);
                width = width - notesWidth;
            }
            return width;
        }
        private List<Segment> GetNoteRestList(List<Segment> list)
        {
            List<Segment> resultList = new List<Segment>();
            var resultlist = from item in list where item.Segment_type == SegmentType.Chord || item.Segment_type == SegmentType.Rest select item;
            resultList = resultlist.ToList();
            return resultList;
        }
        /// <summary>
        /// Recalculate Character Spacers if segment width increased
        /// </summary>
        /// <param name="calc_width"></param>
        /// <param name="count"></param>
        private void SegmentsWidthLower(List<Segment> list, float calc_width)
        {
            int count = list.Count;
            float temp = calc_width / count;
            foreach (var item in list)
            {
                item.Width += temp;
                item.recalculate_spacers();
            }
        }
        /// <summary>
        /// Recalculate CHaracter Spacers if segment width decreased
        /// </summary>
        private void SegmentsWidthHigher(float length)
        {
            this.Width += Math.Abs(length); //! temporary fix for too tight measure width

            //? Ideas for now ...
            //! II options:
            //! lower width of every segment to match size of measure
            //! resize measure, check if whole line of measures fits in given space (length of line), 
            //! if measures < given length: try resize to every measure, 
            //! if measures > given length: move last measure to next line then repeat steps to check if every line of measures is <= given length of measures line
        }
        /// <summary>
        /// Sets Calculated and relative segment positions of each character in measure
        /// </summary>
        /// <returns></returns>
        private float RecalculateSegmentXPos()
        {
            int chars_count = MusicCharacters.Count;
            float temp_pos = 0.0f;
            for (int i = 0; i < chars_count; i++)
            {
                if (i == 0)
                {
                    temp_pos += MusicCharacters[i].Width;
                }
                else
                {
                    MusicCharacters[i].Calculated_x = temp_pos;
                    temp_pos += MusicCharacters[i].Width;
                }
                MusicCharacters[i].Relative = new Point(Calculated_x + MusicCharacters[i].Calculated_x, Calculated_y);
            }
            return temp_pos;
        }

        private void DrawNumber(DrawingVisual visual)
        {
            using(DrawingContext dc = visual.RenderOpen())
            {
                Misc.DrawingHelpers.DrawText(dc, Number.ToString(), new Point(Relative_x, Relative_y - 5f), 10f, withsub: false, color: Brushes.Black);
            }
        }
        /// <summary>
        /// Resets calculated position to X,Y(0,0); Remains width
        /// </summary>
        public void ResetPosition()
        {
            this.Calculated = new Point();
            this.Relative = new Point();
        }

        public override string ToString()
        {
            string result = $"<{ID}> |XY|<{Relative.X.ToString("0.#")}><{Relative.Y.ToString("0.#")}> |W|<{Width}>";
            return result;
        }
    }
}