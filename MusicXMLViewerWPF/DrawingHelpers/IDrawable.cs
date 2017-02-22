using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLScore.DrawingHelpers
{
    interface IDrawable
    {
        Brush Color { get; set; }
        void Draw();
    }
}
