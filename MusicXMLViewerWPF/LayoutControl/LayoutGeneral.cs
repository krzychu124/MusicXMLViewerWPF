using MusicXMLScore.Helpers;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl
{
    /// <summary>
    /// Properties for page
    /// </summary>
    public class LayoutGeneral
    {
        private PageProperties pageProperites;
        private PageMargins pagePargins;
        public LayoutGeneral()
        {
            pageProperites = new PageProperties();
        }
        internal LayoutGeneral(MusicXMLViewerWPF.MusicScore musicScore)
        {
            pageProperites = new PageProperties(musicScore);
        }
        public PageProperties PageProperties { get { return pageProperites; } }
    }
}
