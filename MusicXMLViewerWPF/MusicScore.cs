using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF
{
    class MusicScore
    {
        protected static List<ScorePart> scoreparts = new List<ScorePart>();
        public static List<ScorePart> ScoreParts { get { return scoreparts; } }
    }
}
