using System;
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
    class Measure : IXMLExtract, IDrawable // UNDONE finish reworking class
    {
        private int number;
        private float width;
        private bool hasNumberInvisible;
        private MeasureCoordinates measure_pos;

        private List<Note> notes_list = new List<Note>(); // experimental
        private List<Barline> barlines;
        private Print print_properties;
        private List<Direction> direction;
        private Attributes attributes;

        public int Number { get { return number; } }
        public float Width { get { return width; } }
        public bool NumberVisible { get { return hasNumberInvisible; } }
        public MeasureCoordinates Position { get { return measure_pos; } }

        public List<Note> NotesList { get { return notes_list; } } // Not complete
        public List<Barline> Barlines { get { return barlines; } }
        public Print PrintProperties { get { return print_properties; } }
        public List<Direction> Direction { get { return direction; } }
        public Attributes Attributes { get { return attributes; } }

        public Measure()
        {

        }
        public Measure(XElement x)
        {
            XMLFiller(x);
            barlines = new List<Barline>();
            barlines.Add(new Barline() { Style = Barline.BarStyle.regular, Location = Barline.BarlineLocation.right }); // adding default barline for drawing later
            if (x.Element("barline") != null)
            {
                
                var bars = x.Elements("barline");

                foreach (var item in bars)
                {
                    barlines.Add(new Barline(item)); // << adding irregular barline object like coda,segno, fermata, repeats or endings
                } 
            }
            print_properties = x.Element("print") != null ? new Print(x) : null; // TODO_H test, need deep tests !!!
            direction = null;
            if (x.Element("direction") != null)
            {
                direction = new List<MusicXMLViewerWPF.Direction>();
                var directions = x.Elements("direction");
                foreach (var item in directions)
                {
                    direction.Add(new Direction(item));// TODO_L tests
                }
            }
            
            attributes = x.Element("attributes") != null ? new Attributes(x) : null;  // TODO_L tests
        }

        public IEnumerable<XElement> XMLExtractor()
        {
            var x = Misc.LoadFile.Document;
            var extracted = from extr in x.Descendants("measure") select extr;
            return extracted;
        }

        public void XMLFiller(XElement x)
        {
            width = x.Attribute("width") != null ? float.Parse(x.Attribute("width").Value, CultureInfo.InvariantCulture) : 0f;
            if (width == 0f) Logger.Log($"Measure has no width: {width}");
            bool t = int.TryParse(x.Attribute("number").Value, out number);
            if (t == false) Logger.Log($"Measure number is: {x.Attribute("number").Value}");
            //number = Convert.ToInt32(x.Attribute("number").Value);
            hasNumberInvisible = x.Attribute("implicit") != null ? x.Attribute("implicit").Value == "yes" ? true : false : false; // TODO_L not sure if itll work - very rare usage
        }

        public void Draw(CanvasList surface,Point p) // drawing method
        {
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
                Attributes.Draw(visual,p); // visual will be opened inside, good results, maybe changed in the future
            }
            if (Direction != null)
            {
                foreach (var item in Direction)
                {
                    if (item.DirectionList != null)
                    {
                        foreach (var item2 in item.DirectionList)
                        {
                            if (item2.Dynamics != null)
                            {
                                if (Attributes != null && Attributes.Key != null)
                                {
                                    p.X =  p.X + 50f;
                                }
                                item2.Dynamics.Draw(visual, p);
                            }
                        }
                       
                    }
                }
            } 
                //Draw_Directions(dc2, p); // TODO_H missing implementation

            
            surface.AddVisual(visual);

        }

        public void Draw(DrawingVisual visual, Point p)
        {
            // maybe rework to use that approach
        }

        private void Draw_Directions(DrawingContext dc2, Point p)
        {
            
        }

        private void Draw_Attributes(DrawingContext dc2, Point p)
        {
            
        }

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

        private float GetMeasureLength(float length)
        {
            float num = Convert.ToInt32(Math.Floor(length / 24));
            return num;
        }

        private float GetStaffLinesFilling(float l)
        {
            float fill = l % 24;
            return fill;
        }
    }
}
