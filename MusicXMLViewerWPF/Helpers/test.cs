using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    class test : CanvasList
    {
        public test()
        {
            Rest r = new Rest(SymbolDuration.MusSymbolToDurStr(MusSymbolDuration.Quarter), new Point());
            DrawingVisual dv = new DrawingVisual();
            r.Draw(dv);
            this.Width = r.Width;
            this.Height = r.Height;
            AddVisual(dv);
        }
    }
}
