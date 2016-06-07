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
            Barline = x.Element("barline") != null ? new Barline(x) : new Barline() { Style = Barline.BarStyle.regular }; // seems to be done for now // set default barline style to regular if not present other
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
            float length = GetMeasureLength(Width);
            float filling = GetStaffLinesFilling(Width);
            float X = (float)StartPoint.X;
            int s = 0;
            /*
            for (int i = 0; i < num; i++)
            {
                DrawString(dc, MusChar.Staff5L, TypeFaces.NotesFont, Brushes.Black, x, y, Scale);
                x += 32;
            }

            if (fill != 0)
            {
                DrawString(dc, MusChar.Staff5L, TypeFaces.NotesFont, Brushes.Black, -32 + x + fill, y, scale);
            }
            //for (int i = 0; i < length; i++)
            //{
            //    Misc.DrawingHelpers.DrawString(dc, MusChar.Staff5L, TypeFaces.MeasuresFont, Brushes.Black, X + s, (float)StartPoint.Y, Scale);
            //    Misc.DrawingHelpers.DrawString(dc, MusChar.Staff5Ls, TypeFaces.MeasuresFont, Brushes.Black, X + s + 24, (float)StartPoint.Y, Scale);
            //    s += 32;
            //}
            //if (filling != 0)
            //{
            //    Misc.DrawingHelpers.DrawString(dc, MusChar.Staff5L, TypeFaces.MeasuresFont, Brushes.Black, Width - 32, (float)StartPoint.Y, Scale);
            //    Misc.DrawingHelpers.DrawString(dc, MusChar.Staff5Ls, TypeFaces.MeasuresFont, Brushes.Black, Width - 8, (float)StartPoint.Y, Scale);
            //}
            */
        }

        private float GetMeasureLength(float length)
        {
            float num = Convert.ToInt32(Math.Floor(length / 32));
            return num;
        }

        private float GetStaffLinesFilling(float l)
        {
            float fill = l % 32;
            return fill;
        }
    }
}
