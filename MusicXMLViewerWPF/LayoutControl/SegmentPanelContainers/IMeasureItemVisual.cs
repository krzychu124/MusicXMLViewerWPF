using System.Windows.Controls;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers
{
    interface IMeasureItemVisual
    {
        string ItemStaff { get; set; }
        Canvas ItemCanvas { get; set; }
        double ItemWidth { get; }
        double ItemWidthMin { get; set; }
        double ItemLeftMargin { get; }
        double ItemRightMargin { get; }
    }
}
