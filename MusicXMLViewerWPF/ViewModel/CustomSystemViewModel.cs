using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.ViewModel
{
    class CustomSystemViewModel
    {
        private ObservableCollection<UIElement> uielementlist = new ObservableCollection<UIElement>();

        public ObservableCollection<UIElement> UIElementsList { get { return uielementlist; } }

        public CustomSystemViewModel()
        {
            Rest r = new Rest(SymbolDuration.MusSymbolToDurStr(MusSymbolDuration.Quarter),new Point());
            MusicXMLScore.Helpers.CanvasList cl = new Helpers.CanvasList(300, 200);
            DrawingVisual dv = new DrawingVisual();
            r.Draw(dv);
            cl.AddVisual(dv);
            //uielementlist.Add(cl);
            //CustomSystemPanel csp1 = new CustomSystemPanel();
            //CustomSystemPanel csp2 = new CustomSystemPanel();
            //uielementlist.Add(csp1);
            //uielementlist.Add(csp2);
        }
    }
}
