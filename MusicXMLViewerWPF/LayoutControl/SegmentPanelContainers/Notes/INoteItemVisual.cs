using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    interface INoteItemVisual : IMeasureItemVisual
    {
        int ItemDuration { get; }
        
        double ItemWeight { get; }
    }
}
