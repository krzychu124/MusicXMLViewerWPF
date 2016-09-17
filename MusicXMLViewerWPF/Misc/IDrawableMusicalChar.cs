using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF.Misc
{
    interface IDrawableMusicalChar
    {
       SegmentType CharacterType { get; }
    }
}
