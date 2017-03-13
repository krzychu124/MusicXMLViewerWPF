using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    class KeyContainerItem : Canvas, IAttributeItemVisual
    {
        private double itemWidth;
        private Rect itemRectBounds;
        public KeyContainerItem(DrawingHelpers.MeasureVisual.KeySignatureVisualObject key)
        {
        }
        public Rect ItemRectBounds
        {
            get
            {
                return itemRectBounds;
            }

            set
            {
                itemRectBounds = value;
            }
        }

        public double ItemWidth
        {
            get
            {
                return itemWidth;
            }
        }
    }
}
