using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    interface IDrawable
    {
        //void Draw(CanvasList surface,Point p);
        void Draw(DrawingVisual visual, Point p); // tests only for now
        //void DeleteDraw();
        //void ReDraw();
    }
}
