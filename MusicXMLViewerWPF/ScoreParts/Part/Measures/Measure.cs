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

        public List<Note> NotesList = new List<Note>(); // experimental
        public Barline Barline;
        public Print PrintProperties;
        public Direction Direction;
        public Attributes Attributes;

        public int Number { get { return number; } }
        public float Width { get { return width; } }
        public bool NumberVisible { get { return hasNumberInvisible; } }

        public Measure(XElement x)
        {
            XMLFiller(x);
            Barline = x.Element("barline") != null ? new Barline(x) : new Barline() { Style = Barline.BarStyle.regular, Location = Barline.BarlineLocation.right }; // seems to be done for now // set default barline style to regular if not present other
            PrintProperties = x.Element("print") != null ? new Print(x) : null; // seems to be done for now
            Direction = x.Element("direction") != null ? new Direction(x) : null; // TODO_H missing logic
            Attributes = x.Element("attributes") != null ? new Attributes(x) : null;  // seems to be done for now
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

        public void Draw(CanvasList surface,Point p)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                DrawMeasure(dc, p);
            }
            surface.AddVisual(visual);
        }

        private void DrawMeasure(DrawingContext dc, Point StartPoint)
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
            if (Barline != null)
            {
                if(Barline.Style == Barline.BarStyle.regular)
                {
                    if( Barline.Location == Barline.BarlineLocation.right)
                    {
                        Misc.DrawingHelpers.DrawString(dc, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, X + Width, Y, Scale);
                        Logger.Log($"Regular barline: right at {X + Width} {Y}");
                    }
                    if( Barline.Location == Barline.BarlineLocation.left)
                    {
                        Misc.DrawingHelpers.DrawString(dc, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, X, Y, Scale);
                        Logger.Log($"Regular barline: left at {X} {Y}");
                    }
                }
                if(Barline.Style == Barline.BarStyle.light_heavy)
                {
                    if ( Barline.Location == Barline.BarlineLocation.right)
                    {
                        Misc.DrawingHelpers.DrawString(dc, MusChar.FinalBar, TypeFaces.NotesFont, Brushes.Black, X + Width -5, Y, Scale);
                        Logger.Log($"Light-heavy barline: left at {X + Width - 5} {Y}");
                    }
                }
            }
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
