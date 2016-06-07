using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLViewerWPF.Misc
{
    class DrawingHelpers // helper methods
    {

        public static void DrawString(DrawingContext dc, string text, Typeface t_f, Brush brush, float xPos, float yPos, float element_size)
        {
            dc.DrawText(new FormattedText(text, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, t_f, element_size, brush), new Point(xPos, yPos));
        }
    }
}
