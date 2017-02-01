using MusicXMLViewerWPF.ScoreParts.MeasureContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    class StaffLineCanvas : CanvasList
    {
        static Random r = new Random();
        private DrawingVisual dv = new DrawingVisual();
        public StaffLineCanvas():base()
        {
            MinWidth = 20;
            m = new Measure(Width);
            SizeChanged += StaffLineCanvas_SizeChanged;
        }
        private void StaffLineCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           if (e.WidthChanged)
            {
                if (this.ActualWidth != 0) { m.Width = (float)ActualWidth; }
                ClearVisuals();
                DrawingVisual mv = new DrawingVisual();
                using (DrawingContext dc = mv.RenderOpen())
                {
                    m.Draw_Measure(dc, new Point(0, 0)); //! /*r.Next(10, 30))*/, DrawingHelpers.PickBrush());
                }
                AddVisual(mv);
                /*? vv Debug visual border
            ////DrawingVisual dv = new DrawingVisual();
            //dv = new DrawingVisual();
            //MusicXMLViewerWPF.Misc.DrawingHelpers.DrawRectangle(dv, new Rect(0, 0, ActualWidth, ActualHeight));
            //AddVisual(dv); */
            }
        }

        public Measure m =new Measure();
        //protected override Size MeasureOverride(Size constraint)
        //{
        //   // InvalidateVisual();
        //    return base.MeasureOverride(constraint);
        //}

        //protected override Size ArrangeOverride(Size arrangeSize)
        //{
        //    Size res = base.ArrangeOverride(arrangeSize);
        //    return res;
        //}
    }
}
