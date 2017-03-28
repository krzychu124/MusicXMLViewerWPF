using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    interface IAttributeItemVisual : IMeasureItemVisual
    {
        #region Properties
        int AttributeIndex { get; }
        bool Empty { get; set; }
        Rect ItemRectBounds { get; set; }
        bool Visible { get; set; }

        #endregion Properties
    }
}
