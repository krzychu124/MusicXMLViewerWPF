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
        #region Properties

        bool Empty { get; set; }
        Rect ItemRectBounds { get; set; }
        double ItemWidth { get; }
        bool Visible { get; set; }

        #endregion Properties
    }
}
