using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutStyle
{
    [Serializable]
    public class SystemLayoutStyle
    {
        private PartGroupSymbol partGroupStyle = PartGroupSymbol.line;
        private double groupSymbolOffset = 4;
        private double systemLeftMargin = 0;
        private double systemRightMargin = 0;
        private double systemDistance = 80;
        private double systemTopDistance = 170;

        public PartGroupSymbol PartGroupStyle
        {
            get
            {
                return partGroupStyle;
            }

            set
            {
                partGroupStyle = value;
            }
        }

        public double GroupSymbolOffset
        {
            get
            {
                return groupSymbolOffset;
            }

            set
            {
                groupSymbolOffset = value;
            }
        }

        public double SystemLeftMargin
        {
            get
            {
                return systemLeftMargin;
            }

            set
            {
                systemLeftMargin = value;
            }
        }

        public double SystemRightMargin
        {
            get
            {
                return systemRightMargin;
            }

            set
            {
                systemRightMargin = value;
            }
        }

        public double SystemDistance
        {
            get
            {
                return systemDistance;
            }

            set
            {
                systemDistance = value;
            }
        }

        public double SystemTopDistance
        {
            get
            {
                return systemTopDistance;
            }

            set
            {
                systemTopDistance = value;
            }
        }
    }
}
