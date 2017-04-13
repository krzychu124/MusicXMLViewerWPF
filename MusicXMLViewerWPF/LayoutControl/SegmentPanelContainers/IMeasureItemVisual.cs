using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers
{
    interface IMeasureItemVisual
    {
        string ItemStaff { get; set; }
        Canvas ItemCanvas { get; set; }
        double ItemWidth { get; }
        double ItemWidthMin { get; set; }
    }
}
