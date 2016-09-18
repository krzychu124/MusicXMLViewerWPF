using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLViewerWPF.Misc
{
    interface IDrawableMusicalChar
    {
       SegmentType CharacterType { get; }
        string ID { get; }
        void Draw(DrawingVisual visual);
    }
}
