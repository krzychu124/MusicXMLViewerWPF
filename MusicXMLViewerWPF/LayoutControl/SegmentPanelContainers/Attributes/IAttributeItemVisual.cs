using System.Windows;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    interface IAttributeItemVisual : IMeasureItemVisual
    {
        #region Properties
        AttributeType AttributeType { get; }
        bool Empty { get; set; }
        Rect ItemRectBounds { get; set; }

        #endregion Properties
    }
}
