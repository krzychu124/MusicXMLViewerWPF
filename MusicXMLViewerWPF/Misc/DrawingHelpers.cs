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
        public static void DrawBarline(Barline.BarStyle style, Barline.BarlineLocation location)
        {

        }
        public static void DrawText(DrawingContext dc, string text, Point position, float font_size, Halign align = Halign.left, string font_weight = null)
        {
            Point page = new Point(MusicScore.Defaults.Page.Width, MusicScore.Defaults.Page.Height);
            position = SubstractPoint(page, position);
            FormattedText ft = new FormattedText(text, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, TypeFaces.TextFont, font_size, Brushes.Black);
            if (font_weight != null)
            {
                if (font_weight == "bold")
                {
                    ft.SetFontWeight(FontWeights.Bold);
                }
            }
            
            switch (align)
            {
                case Halign.center:
                    ft.TextAlignment = TextAlignment.Center;
                    break;
                case Halign.right:
                    ft.TextAlignment = TextAlignment.Right;
                    break;
                case Halign.left:
                    ft.TextAlignment = TextAlignment.Left;
                    break;
            }
            dc.DrawText(ft, position);
        }
        public static Point SubstractPoint(Point one, Point two)
        {
            Point result = new Point();
            result = new Point(one.X - two.X, one.Y - two.Y);
            return result;
        }
    }
}
