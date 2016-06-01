﻿using System;
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
        private float milimeters;
        private float tenths;

        public float Milimeters { get { return milimeters; } }
        public float Tenths {  get { return tenths; } }

        public Scale()
        {
            var x = XMLExtractor();
            foreach (var item in x)
            {
                XMLFiller(item);
            }
            
        }
        public IEnumerable<XElement> XMLExtractor()
        {
            var x = LoadDocToClasses.Document;
            var el = from z in x.Elements("defaults") select z;
            return el;
        }
        public void XMLFiller(XElement x)
        {
            milimeters = float.Parse(x.Element("milimeters").Value, CultureInfo.InvariantCulture);
            tenths = float.Parse(x.Element("tenths").Value, CultureInfo.InvariantCulture);
        }
    }
}