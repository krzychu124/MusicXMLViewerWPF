using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutStyle
{
    [Serializable()]
    public class Layout
    {
        private BarlineLayoutStyle barlineStyle;
        private BeamLayoutStyle beamStyle;
        private MeasureLayoutStyle measureStyle;
        private NotesLayoutStyle notesStyle;
        private PageLayoutStyle pageStyle;
        private SystemLayoutStyle systemStyle;

        public Layout()
        {
            barlineStyle = new BarlineLayoutStyle();
            beamStyle = new BeamLayoutStyle();
            measureStyle = new MeasureLayoutStyle();
            notesStyle = new NotesLayoutStyle();
            pageStyle = new PageLayoutStyle();
            systemStyle = new SystemLayoutStyle();
        }

        public Layout(ScorePartwiseMusicXML score):this()
        {
            
        }
        public BarlineLayoutStyle BarlineStyle
        {
            get
            {
                return barlineStyle;
            }

            set
            {
                barlineStyle = value;
            }
        }

        public BeamLayoutStyle BeamStyle
        {
            get
            {
                return beamStyle;
            }

            set
            {
                beamStyle = value;
            }
        }

        public MeasureLayoutStyle MeasureStyle
        {
            get
            {
                return measureStyle;
            }

            set
            {
                measureStyle = value;
            }
        }

        public NotesLayoutStyle NotesStyle
        {
            get
            {
                return notesStyle;
            }

            set
            {
                notesStyle = value;
            }
        }

        public PageLayoutStyle PageStyle
        {
            get
            {
                return pageStyle;
            }

            set
            {
                pageStyle = value;
            }
        }

        public SystemLayoutStyle SystemStyle
        {
            get
            {
                return systemStyle;
            }

            set
            {
                systemStyle = value;
            }
        }
    }
}
