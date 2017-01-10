using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLViewerWPF;
using System.Windows.Controls;
using System.Windows;
using MusicXMLViewerWPF.ScoreParts.Part.Measures;

namespace MusicXMLScore.Helpers
{
    class PreviewSettings : CanvasList
    {
        public PreviewSettings()
        {
            Measure m = new Measure(100);
            m.AddClef(new ClefType(ClefType.Clef.GClef));
            Canvas c = new Canvas();
            AddVisual(m.Attributes.Clef.DrawableMusicalObject);
            this.Width = 30;
        }
    }
}
