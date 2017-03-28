﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers
{
    interface IMeasureItemVisual
    {
        double ItemWidth { get; }
        double ItemWidthMin { get; set; }
    }
}
