using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLViewerWPF.Misc
{
    interface IDrawableMusicalObject
    {
        CanvasList DrawableMusicalObject { get; set; }
        DrawableMusicalObjectStatus DrawableObjectStatus { get; }
        bool Loaded { get; }
        void InitDrawableObject();
        void ReloadDrawableObject();
    }
    enum DrawableMusicalObjectStatus
    {
        notready,
        reload,
        ready
    }
}
