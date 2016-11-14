using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicXMLScore.Helpers
{
    class OrientedSize
    {
        private Orientation orientation;
        private double direct;
        private double indirect;

        public double Direct { get { return direct; } set { direct = value; } }
        public double Indirect { get { return indirect; } set { indirect = value; } }
        public Orientation Orientation { get { return orientation; } }
        public double Width
        {
            get
            {
                return (Orientation == Orientation.Horizontal) ? direct : indirect;
            }
            set
            {
                if (Orientation == Orientation.Horizontal)
                {
                    Direct = value;
                }
                else
                {
                    Indirect = value;
                }
            }
        }

        public double Height
        {
            get
            {
                return (Orientation != Orientation.Horizontal) ? direct : indirect;
            }
            set
            {
                if (Orientation != Orientation.Horizontal)
                {
                    Direct = value;
                }
                else
                {
                    Indirect = value;
                }
            }
        }

        public OrientedSize(Orientation orientation, double width, double height)
        {
            this.orientation = orientation;
            direct = 0.0;
            indirect = 0.0;
            Width = width;
            Height = height;
        }

        public OrientedSize(Orientation orientation):this(orientation, 0.0, 0.0)
        {

        }
    }
}
