﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.ScoreParts.Part.Measures
{
    class Measure : Segment, IXMLExtract, IDrawable // UNDONE finish reworking class
    {
        #region Fields for drawing

        #endregion
        #region Fields
        //--------------------
        private int number;
        private float width; // default value is 100 may change later
        //--------------------
        private Attributes attributes;
        private bool hasNumberInvisible;
        private DrawingVisual visual;
        private int elements_count;
        private MeasureCoordinates measure_pos;
        private Point pos;
        private Print print_properties;
        private bool is_first_in_line = false;
        #endregion

        #region Collections
        private List<Barline> barlines = new List<Barline>();
        private List<Direction> direction = new List<MusicXMLViewerWPF.Direction>();
        private List<Note> notes_list = new List<Note>(); // experimental
        private List<Segment> music_characters = new List<Segment>();
        #endregion

        #region Properties
        public Attributes Attributes { get { return attributes; } }
        public bool NumberVisible { get { return hasNumberInvisible; } }
        public DrawingVisual Visual { get { return visual; } set { if (value != null) visual = value; } }
        public new float Width { get { return width; } set { if (value >= 0) { width = value; } else { width = 100f; Logger.Log("width is negative here"); } } }
        public int ElementsCount { get { return elements_count; } }
        public int Number { get { return number; } }
        public List<Barline> Barlines { get { return barlines; } }
        public List<Direction> Direction { get { return direction; } }
        public List<Note> NotesList { get { return notes_list; } } // Not complete
        public List<Segment> MusicCharacters { get { return music_characters; } }
        public MeasureCoordinates MeasurePosition { get { return measure_pos; } }
        public Point Position { get { return pos; } set { if (value != null) pos = value; } }
        public Print PrintProperties { get { return print_properties; } }
        public bool IsFirstInLine { get { return is_first_in_line; } set { is_first_in_line = value; }  }
        #endregion

        public Measure()
        {

        }

        public Measure(XElement x, Point position)
        {

        }

        public Measure(XElement x)
        {
            XMLFiller(x);

            if (x.Elements("direction").Any() == false)
            {
                direction = null;
            }
            
            foreach (var element in x.Elements())
            {
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
                
            }
            //if (barlines.Count ==0 || barlines.Where(b => b.Style != Barline.BarStyle.regular).Any()) 
            //{
                barlines.Add(new Barline() { Style = Barline.BarStyle.regular, Location = Barline.BarlineLocation.right }); // adding default barline for drawing later
            //}
            //foreach (var item in Misc.ScoreSystem.Segments)
            //{
            //    Logger.Log($"Added segment {item.ToString()}");
            //}
        }
        /// <summary>
        /// Extract barlines from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractBarlines(XElement x)
        {
            barlines.Add(new Barline(x));
            Misc.ScoreSystem.Segments.Add(new Segment() { Segment_type = SegmentType.Barline, Color = Brushes.Gold });
        }
        /// <summary>
        /// Extract measure attributes (Clef, KeySig, Timesig) from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractAttributes(XElement x)
        {
            attributes =  new Attributes(x);
            if (attributes.Clef != null)
            {
                Misc.ScoreSystem.Segments.Add(new Segment() { Segment_type = SegmentType.Clef, Color = Brushes.Brown });
                music_characters.Add(new Segment() { Segment_type = SegmentType.Clef, Color = Brushes.Brown });
            }
            if (attributes.Key != null)
            {
                Misc.ScoreSystem.Segments.Add(new Segment() { Segment_type = SegmentType.KeySig, Color = Brushes.Coral });
                music_characters.Add(new Segment() { Segment_type = SegmentType.KeySig, Color = Brushes.Coral });
            }
            if (attributes.Time != null)
            {
                Misc.ScoreSystem.Segments.Add(new Segment() { Segment_type = SegmentType.TimeSig, Color = Brushes.Lavender });
                music_characters.Add(new Segment() { Segment_type = SegmentType.TimeSig, Color = Brushes.Lavender });
            }
        }
        /// <summary>
        /// Extract directions (wedge, dynamics, coda etc.) from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractDirections(XElement x)
        {
            direction.Add(new Direction(x));// TODO_L tests
            Misc.ScoreSystem.Segments.Add(new Segment() { Segment_type = SegmentType.Direction});
            music_characters.Add(new Segment() { Segment_type = SegmentType.Direction, Color = Brushes.DarkTurquoise });
        }
        /// <summary>
        /// Extract rests from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractRests(XElement x)
        {
            Logger.Log("missing impl for this rest");
            Misc.ScoreSystem.Segments.Add(new Segment() { Segment_type = SegmentType.Rest, Color = Brushes.DarkMagenta });
            music_characters.Add(new Segment() { Segment_type = SegmentType.Rest, Color = Brushes.DarkMagenta });
        }
        /// <summary>
        /// Extract notes from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractNotes(XElement x)
        {
            Logger.Log("missing impl for this note");
            Misc.ScoreSystem.Segments.Add(new Segment() { Segment_type = SegmentType.Chord, Color = Brushes.DarkOliveGreen});
            music_characters.Add(new Segment() { Segment_type = SegmentType.Chord, Color = Brushes.DarkOliveGreen });
        }
        /// <summary>
        /// Extract visual properties (custom margins, spacers, new_page, new_line etc.) from XML elemnet
        /// </summary>
        /// <param name="x"></param>
        private void XMLExtractPrint(XElement x)
        {
            print_properties = new Print(x);
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
        public void XMLFiller(XElement x)
        {
            width = x.Attribute("width") != null ? float.Parse(x.Attribute("width").Value, CultureInfo.InvariantCulture) : 0f;
            if (width == 0f)
            {
                Logger.Log($"Measure {number} has no width: {width}");
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
        /// Method calculating measure width according to number and type of elements which contains
        /// </summary>
        /// <param name="n"></param>
        private void AutoMeasureWidth(int n)
        {
            width = 100f;
            float temp_width = 0f;
            foreach (var item in music_characters)
            {
                temp_width += item.Width;
            }
            if (temp_width > width) width = temp_width;
        }
        /// <summary>
        /// Drawing method used to draw measure with all contating elements (motes, rest, directions etc.)
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="p"></param>
        public void Draw(CanvasList surface,Point p) // drawing method
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
                    Attributes.Draw(visual, p, width, true);
                }
                else
                {
                    Attributes.Draw(visual, p, width); // visual will be opened inside, good results, maybe changed in the future
                }
            }
            if (Direction != null)
            {

                foreach (var item in Direction)
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
                Point temp = p;
                temp.Y += 8f;
                DrawingVisual segments = new DrawingVisual();
                foreach (var character in MusicCharacters)
                {
                    if (character.Width == 0)
                    {
                        character.CalculateDimensions();
                        character.SetRelativePos(temp);
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
            // maybe rework to use that approach
        }

        private void Draw_Directions(DrawingContext dc2, Point p)
        {
            //TODO_L missing implementation
        }

        private void Draw_Attributes(DrawingContext dc2, Point p)
        {
            //TODO_L missing implementation
        }
        /// <summary>
        /// Method for drawing staff of measure using width begining from StartPoint coords
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="StartPoint"></param>
        private void Draw_Measure(DrawingContext dc, Point StartPoint)
        {
            float Scale = MusicScore.Defaults.Scale.Tenths;
            float num = GetMeasureLength(Width);
            float filling = GetStaffLinesFilling(Width);
            float X = (float)StartPoint.X;
            float Y = (float)StartPoint.Y;
            int s = 0;

            for (int i = 0; i < num; i++)
            {
                Misc.DrawingHelpers.DrawString(dc, MusChar.Staff5L, TypeFaces.NotesFont, Brushes.Black, X + s, Y, Scale);
                s += 24;
            }

            if (filling != 0)
            {
                Misc.DrawingHelpers.DrawString(dc, MusChar.Staff5L, TypeFaces.NotesFont, Brushes.Black, X + (Width-24), Y, Scale);
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
    }
}
