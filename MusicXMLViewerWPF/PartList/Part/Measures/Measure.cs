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
        private bool hasNumberInvisible;

        public List<Note> NotesList = new List<Note>(); // experimental
        public Barline Barline;
        public Print PrintProperties;
        public Directions Direction;
        public Attributes Attributes;

        public int Number { get { return number; } }
        public float Width { get { return width; } }
        public bool NumberVisible { get { return hasNumberInvisible; } }

        public Measure(XElement x)
        {
            XMLFiller(x);
            Barline = new Barline(x); // seems to be done for now
            PrintProperties = new Print(x); // seems to be done for now
            Direction = new Directions(x);
            Attributes = new Attributes(x); //UNDONE incorrect/missing implementation 
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
            hasNumberInvisible = x.Attribute("implicit") != null ? x.Attribute("implicit").Value == "yes" ? true : false : false; // _NOTE not sure if itll work - very rare usage
        }
        public void Draw(CanvasList surface)
        {

        }
    }
}
