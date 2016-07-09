using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.Defaults
{
    class Scale : IXMLExtract
    {
        private float millimeters;
        private float tenths;

        public float Millimeters { get { return millimeters; } }
        public float Tenths {  get { return tenths; } }

        public Scale()
        {
            var x = XMLExtractor();
            foreach (var item in x)
            {
                XMLFiller(item);
            }
            
        }

        public Scale(XElement x)
        {
            XMLFiller(x);
        }
        public IEnumerable<XElement> XMLExtractor()
        {
            var x = LoadDocToClasses.Document;
            var el = from z in x.Elements("defaults") select z;
            return el;
        }
        public void XMLFiller(XElement x)
        {
            millimeters = float.Parse(x.Element("millimeters").Value, CultureInfo.InvariantCulture);
            tenths = float.Parse(x.Element("tenths").Value, CultureInfo.InvariantCulture);
        }
    }
}
