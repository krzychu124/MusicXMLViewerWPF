using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    class PreviewCanvas : DrawingVisualHost
    {
        public DrawingVisual Visual = new DrawingVisual();
        public PreviewCanvas()
        {
            ConstructVisualPreview();
        }

        private void ConstructVisualPreview()
        {
            DrawingVisual vis = new DrawingVisual();
            using (DrawingContext dc = vis.RenderOpen())
            {
                DrawingHelpers.DrawString(dc, "test", TypeFaces.TextFont, Brushes.Black, 20f, 15f, 20f);
            }
            this.AddVisual(vis);
        }

        public void StaffLine()
        {
            string s = "\ue01a";
            float scale = 45;
            FormattedText text = new FormattedText(s, System.Threading.Thread.CurrentThread.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, TypeFaces.BravuraTextFont, scale, Brushes.Black);
            DrawingVisual staffline = new DrawingVisual();
            Point point = new Point(1, 20);
            using(DrawingContext dc = staffline.RenderOpen())
            {
                for (int c = 0; c < 5; c++)
                {
                    dc.DrawText(text, point);
                    point.X += scale * 0.55;
                }
            }
            AddVisual(staffline);
        }

        public void AddVis()
        {
            DrawingVisual vis = new DrawingVisual();
            using (DrawingContext dc = vis.RenderOpen())
            {
                DrawingHelpers.DrawString(dc, "test2", TypeFaces.TextFont, Brushes.Black, 35f, 45f, 20f);
            }
            //this.AddVisual(vis);
        }
    }
}
