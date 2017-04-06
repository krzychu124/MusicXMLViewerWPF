using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    interface INoteItemVisual : IMeasureItemVisual
    {
        int ItemDuration { get; }
        void DrawSpace(double length, bool red=false);
        double ItemWeight { get; }
    }
}
