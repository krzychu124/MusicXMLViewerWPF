using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF.Misc
{
    class ScorePage
    {
        /// <summary>
        /// List of measureSystems which are placed on page (list of lines which contains list of measures in them)
        /// </summary>
        public List<MeasureSystem> Pages = new List<MeasureSystem>();
    }
}
