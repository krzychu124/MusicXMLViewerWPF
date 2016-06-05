using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            width = float.Parse(x.Attribute("width").Value, CultureInfo.InvariantCulture);
            number = Convert.ToInt32(x.Attribute("number").Value);
            hasNumberInvisible = x.Attribute("implicit") != null ? x.Attribute("implicit").Value == "yes" ? true : false : false; // TODO_L not sure if itll work - very rare usage
        }
        public void Draw(CanvasList surface)
        {

        }
    }
}
