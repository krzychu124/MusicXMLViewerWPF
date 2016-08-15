using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF
{
    public enum SegmentType
    {
        Unknown = 0,
        Clef,
        KeySig,
        TimeSig,
        Chord,
        Rest,
        Direction,
        Barline,
        Title
    }
}
