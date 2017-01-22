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
        public static void DrawFormattedString(DrawingContext dc, FormattedText ft, Point position)
        {
            dc.DrawText(ft, position);
        }
        public static void DrawText(DrawingContext dc, string text, Point position, float font_size, Halign align = Halign.right, Valign valign = Valign.middle, string font_weight = null, bool withsub = true, Brush color = null)
        {
            Page p = new Page();
            color = color == null ? Brushes.Black : color;
            Point page = new Point(p.Width, p.Height); //todo test
            Point page_margins = new Point(0, p.Margins.Bottom);
            Point calculated_margins = CalculatePosition(page, page_margins); // test
            if (withsub)
            {
                position = CalculatePosition(calculated_margins, new Point(page.X - position.X, position.Y)); // (page, //
            }
            //! debug: Logger.Log($"Added \"{text}\" at position {position.X}, {position.Y}, {align}");
            FormattedText ft = new FormattedText(text, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, TypeFaces.TextFont, font_size * 1.4, color);
            //DrawString(dc, "||", TypeFaces.TextFont, Brushes.Black, (float)position.X, (float)position.Y, 18); // visual debug position helper
           
            if (font_weight != null)
            {
                if (font_weight == "bold")
                {
                    ft.SetFontWeight(FontWeights.Bold);
                }
            }
            VerticalAlign(position, ft, valign);
            //switch (valign)
            //{
            //    case Valign.top:
            //        position.Y = position.Y - (ft.Height/2);
            //        break;
            //    case Valign.middle:
            //        break;
            //    case Valign.bottom:
            //        position.Y = position.Y + (ft.Height / 2);
            //        break;
            //    case Valign.baseline:
            //        position.Y = position.Y + (ft.Height / 2);
            //        break;
            //    default:
            //        break;
            //}
            HorizontalAlign(ft, align);
            //switch (align)
            //{ 
            //    case Halign.center:
            //        ft.TextAlignment = TextAlignment.Center;
            //        break;
            //    case Halign.right:
            //        ft.TextAlignment = TextAlignment.Right;
            //        break;
            //    case Halign.left:
            //        ft.TextAlignment = TextAlignment.Left;
            //        break;
            //}
            dc.DrawText(ft, position);
        }

        public static void VerticalAlign(Point position, FormattedText ft, Valign valign)
        {
            switch (valign)
            {
                case Valign.top:
                    position.Y = position.Y - (ft.Height / 2);
                    break;
                case Valign.middle:
                    break;
                case Valign.bottom:
                    position.Y = position.Y + (ft.Height / 2);
                    break;
                case Valign.baseline:
                    position.Y = position.Y + (ft.Height / 2);
                    break;
                default:
                    break;
            }
        }

        public static void SetFontWeight(FormattedText ft, string font_weight)
        {
            if (font_weight != null)
            {
                if (font_weight == "bold")
                {
                    ft.SetFontWeight(FontWeights.Bold);
                }
                if (font_weight == "normal")
                {
                    ft.SetFontWeight(FontWeights.Normal);
                }
                if (font_weight == "regular")
                {
                    ft.SetFontWeight(FontWeights.Regular);
                }
            }
        }

        public static FormattedText HorizontalAlign(FormattedText ft, Halign align)
        {
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
                case Halign.none:
                    ft.TextAlignment = TextAlignment.Justify;
                    break;
            }
            return ft;
        }

        public static Point CalculatePosition(Point one, Point two)
        {
            Point result = new Point();
            result = new Point(one.X - two.X, one.Y - two.Y);
            return result;
        }

        public static void DrawRectangle(DrawingVisual visual, Point one, Point two, Brush color = null, DashStyle dash = null )
        {
            if (color == null)
            {
                color = Brushes.Black;
            }
            if (dash == null)
            {
                dash = DashStyles.Dash;
            }
            DrawingVisual rectangle = new DrawingVisual();
            using (DrawingContext dc = rectangle.RenderOpen())
            {
                Pen pen = new Pen(color, 1.2);
                pen.DashStyle =dash;
                dc.DrawRectangle(Brushes.Transparent, pen, new Rect(one, two));
            }
            visual.Children.Add(rectangle);
        }
        public static void DrawRectangle(DrawingVisual visual, Rect r, Brush color = null, DashStyle dash = null)
        {
            if (color == null)
            {
                color = Brushes.Black;
            }
            if (dash == null)
            {
                dash = DashStyles.Dash;
            }
            DrawingVisual rectangle = new DrawingVisual();
            using (DrawingContext dc = rectangle.RenderOpen())
            {
                Pen pen = new Pen(color, 1.2);
                pen.DashStyle = dash;
                dc.DrawRectangle(Brushes.Transparent, pen, r);
            }
            visual.Children.Add(rectangle);
        }
        public static void DrawLine(DrawingVisual visual, Point start, Point end, float thickness = 1f, float beamthickness = 3f) //! WorkInProgress
        {
            using (DrawingContext dc = visual.RenderOpen())
            {
                Pen pen = new Pen(Brushes.Black, thickness);
                float offset = beamthickness;
                Point s2 = new Point(start.X, start.Y + offset);         //
                Point e2 = new Point(end.X, end.Y + offset);         //

                StreamGeometry sg = new StreamGeometry();
                using (StreamGeometryContext sgc = sg.Open())
                {
                    sgc.BeginFigure(start, true, true);
                    PointCollection points = new PointCollection { end, e2, s2 };
                    sgc.PolyLineTo(points, true, true);
                }
                sg.Freeze();
                dc.DrawGeometry(Brushes.Black, pen, sg);
            }
        }
    }
}
