using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.Defaults
{
    class Scale
    {
        private float millimeters;
        private float tenths;

        public float Millimeters { get { return millimeters; } }
        public float Tenths {  get { return tenths; } }

        public Scale()
        {
            millimeters = 7.05556f;
            tenths = 40f;
        }

        public Scale(XElement x)
        {
            InitScale(x);
        }

        public void Set(float new_scale_tenths)
        {
            tenths = new_scale_tenths;
            millimeters = new_scale_tenths / 0.176389f;
        }

        public void InitScale(XElement x)
        {
            millimeters = float.Parse(x.Element("millimeters").Value, CultureInfo.InvariantCulture);
            tenths = float.Parse(x.Element("tenths").Value, CultureInfo.InvariantCulture);
        }
    }
}
