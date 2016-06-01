using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.PartList.Part.Measures
{
    class Measure : IXMLExtract, IDrawable // UNDONE finish reworking class
    {
        private int number;
        private float width;
        private bool hasNumberVisible;

        public List<Note> NotesList = new List<Note>(); // experimental
        public Barline Barline;
        public Print PrintProperties;
        public Directions Direction;
        public Attributes Attributes;

        public int Number { get { return number; } }
        public float Width { get { return width; } }
        public bool NumberVisible { get { return hasNumberVisible; } }

        public Measure(XElement x)
        {
            XMLFiller(x);
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

        }
        public void Draw(CanvasList surface)
        {

        }
    }
}
