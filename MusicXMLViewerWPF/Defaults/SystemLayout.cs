using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.Defaults
{

    public class SystemLayout // looks good //
    {
        private float left_margin;
        private float right_margin;
        private float system_distance;
        private float top_system_distance;
        private SystemDivider system_dividers;

        public float LeftRelative { get { return left_margin; } }
        public float RightRelative { get { return right_margin; } }
        public float SystemDistance { get { return system_distance; } }
        public float TopSystemDistance { get { return top_system_distance; } }
        public SystemDivider Dividers { get { return system_dividers; } }
        public SystemLayout()
        {
            getSystemLayout();
        }

        public SystemLayout(XElement x)
        {
            getSystemLayout(x);
        }

        public SystemLayout(float l, float r, float s, float t, SystemDivider d)
        {
            left_margin = l;
            right_margin = r;
            system_distance = s;
            top_system_distance = t;
            system_dividers = d;
        }
        private void getSystemLayout()
        {
            XDocument doc = LoadDocToClasses.Document;
            var s = from z in doc.Descendants("defaults") select z;
            var sl = from x in s.Elements("system-layout") select x;
            foreach (var item in sl)
            {
                left_margin = (float)Convert.ToDouble(item.Element("system-margins").Element("left-margin").Value);
                right_margin = (float)Convert.ToDouble(item.Element("system-margins").Element("right-margin").Value);
                system_distance = (float)Convert.ToDouble(item.Element("system-distance").Value);
                top_system_distance = (float)Convert.ToDouble(item.Element("top-system-distance").Value);
            }
        }
        private void getSystemLayout(XElement x)
        {

            var sl = x.Elements();
            foreach (var item in sl)
            {
                if (item.Name.LocalName == "system-margins")
                {
                    left_margin = (float)Convert.ToDouble(item.Element("left-margin").Value);
                    right_margin = (float)Convert.ToDouble(item.Element("right-margin").Value);
                }
                if (item.Name.LocalName == "system-distance")
                {
                    system_distance = (float)Convert.ToDouble(item.Value);
                }
                if (item.Name.LocalName == "top-system-distance")
                {
                    top_system_distance = (float)Convert.ToDouble(item.Value);
                }
            }
        }

        public class SystemDivider // implemented but no use curently // visible object which represent point where group of measures are divided //
        {
            private float left_divider;
            private float right_divider;

            public float Left { get { return left_divider; } }
            public float Right { get { return right_divider; } }

            public SystemDivider(float l, float r)
            {
                left_divider = l;
                right_divider = r;
            }
        }

    }
}
