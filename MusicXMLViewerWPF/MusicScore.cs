using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF
{
    class MusicScore
    {
        protected static List<PartList> musicscoreparts = new List<PartList>();

        public static List<PartList> ScoreParts { get { return musicscoreparts; } }
    }
}
