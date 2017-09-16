using MusicXMLScore.LayoutStyle.Styles;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Windows.Media;

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
        private ItemsColorsStyle _itemsColorsStyle;
        private Dictionary<int, Brush> colors;
        public Layout()
        {
            barlineStyle = new BarlineLayoutStyle();
            beamStyle = new BeamLayoutStyle();
            measureStyle = new MeasureLayoutStyle();
            notesStyle = new NotesLayoutStyle();
            _itemsColorsStyle = new ItemsColorsStyle();
            pageStyle = new PageLayoutStyle();
            colors = new Dictionary<int, Brush>()
            {
                [1] = Brushes.Black,
                [2] = Brushes.Navy,
                [3] = Brushes.DarkSlateGray,
                [4] = Brushes.DarkRed,
                [5] = Brushes.Indigo,
                [6] = Brushes.DodgerBlue,
                [7] = Brushes.Green,
                [8] = Brushes.OrangeRed,
            };
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

        public Dictionary<int, Brush> Colors
        {
            get
            {
                return colors;
            }

            set
            {
                colors = value;
            }
        }

        public ItemsColorsStyle ItemsColorsStyle { get => _itemsColorsStyle; set => _itemsColorsStyle = value; }
    }
}
