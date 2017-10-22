namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    interface INoteItemVisual : IMeasureItemVisual
    {
        int ItemDuration { get; }
        double HorizontalOffset { get; set; }
        double VerticalOffset { get; set; }
        //void DrawSpace(double length, bool red=false);
        //double ItemWeight { get; }
    }
}
