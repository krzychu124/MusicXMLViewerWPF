using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    interface IAttributeItemVisual
    {
        double ItemWidth { get; }
        Rect ItemRectBounds { get; set; }
    }
}
