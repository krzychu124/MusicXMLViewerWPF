using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl
{
    /// <summary>
    /// Controlls page content
    /// </summary>
    class PageController
    {
        PageProperties pageProperties;
        MusicXMLViewerWPF.MusicScore musicScore;
        public PageController(MusicXMLViewerWPF.MusicScore ms )
        {
            musicScore = ms;
            pageProperties = new PageProperties();
        }
        public PageProperties PageProperties { get { return pageProperties; } set { if (value != null) { pageProperties = value; } } }
    }
}
