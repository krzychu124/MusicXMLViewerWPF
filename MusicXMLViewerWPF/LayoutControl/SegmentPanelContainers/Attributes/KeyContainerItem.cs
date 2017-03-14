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
        private bool empty = false;
        private bool visible = true;
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

        public bool Empty
        {
            get
            {
                return empty;
            }

            set
            {
                empty = value;
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }
    }
}
