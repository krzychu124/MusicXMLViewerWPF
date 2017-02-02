using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF.Defaults
{

    public class SystemLayout //! looks good 
    {
        private float leftMargin;
        private float rightMargin;
        private float systemDistance;
        private float topSystemDistance;
        private SystemDivider systemDividers;

        public float LeftRelative { get { return leftMargin; } }
        public float RightRelative { get { return rightMargin; } }
        public float SystemDistance { get { return systemDistance; } }
        public float TopSystemDistance { get { return topSystemDistance; } }
        public SystemDivider Dividers { get { return systemDividers; } }

        public SystemLayout()
        {
            initDefaultValues();
        }

        public SystemLayout(XElement x)
        {
            GetSystemLayout(x);
        }

        private void initDefaultValues()
        {
            leftMargin = 0;
            rightMargin = 0;
            systemDistance = 0;
            topSystemDistance = 0;
        }

        public SystemLayout(float l, float r, float s, float t, SystemDivider d)
        {
            leftMargin = l;
            rightMargin = r;
            systemDistance = s;
            topSystemDistance = t;
            systemDividers = d;
        }

        private void GetSystemLayout(XElement x)
        {

            var sl = x.Elements();
            foreach (var item in sl)
            {
                if (item.Name.LocalName == "system-margins")
                {
                    leftMargin = float.Parse(item.Element("left-margin").Value, CultureInfo.InvariantCulture);
                    rightMargin = float.Parse(item.Element("right-margin").Value, CultureInfo.InvariantCulture);
                }
                if (item.Name.LocalName == "system-distance")
                {
                    systemDistance = float.Parse(item.Value, CultureInfo.InvariantCulture);
                }
                if (item.Name.LocalName == "top-system-distance")
                {
                    topSystemDistance = float.Parse(item.Value, CultureInfo.InvariantCulture);
                }
            }
        }

        public class SystemDivider //! implemented but no use curently // visible object which represent point where group of measures are divided //
        {
            private float leftDivider;
            private float rightDivider;

            public float Left { get { return leftDivider; } }
            public float Right { get { return rightDivider; } }

            public SystemDivider(float left, float right)
            {
                leftDivider = left;
                rightDivider = right;
            }
        }

    }
}
