using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLViewerWPF;
using System.Windows.Controls;
using System.Windows;
using MusicXMLViewerWPF.ScoreParts.MeasureContent;

namespace MusicXMLScore.Helpers
{
    class PreviewSettings : DrawingVisualHost
    {
        public PreviewSettings()
        {
            Measure m = new Measure(100);
            m.AddClef(new ClefType(ClefType.Clef.GClef));
            AddVisual(m.Attributes.Clef.DrawableMusicalObject);
            //this.Width = 30;
        }
    }
}
