using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    /// <summary>
    /// DrawingVisual class with Tag support
    /// </summary>
    class DrawingVisualPlus : DrawingVisual
    {
        /// <summary>
        /// Can be use as searching tag when object is added as a child to other UIElement
        /// </summary>
        public string Tag { get; set; }
    }
}
