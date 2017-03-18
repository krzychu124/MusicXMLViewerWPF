using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    interface INoteItemVisual
    {
        double ItemWidthMin { get; set; }
        double ItemWidthOpt { get; set; }
        double ItemWeight { get; }
    }
}
