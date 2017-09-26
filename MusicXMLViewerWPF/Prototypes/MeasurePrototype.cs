using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.Prototypes
{
    class MeasurePrototype : FrameworkElement
    {
        DrawingVisual visual;
        Brush color;
        public MeasurePrototype()
        {
            visual = new DrawingVisual();
            color = Helpers.DrawingHelpers.PickRandomBrush();
            Height = 50;
            AddVisualChild(visual);
            AddLogicalChild(visual);
            SizeChanged += MeasurePrototype_SizeChanged;
           
        }
        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return visual;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
           Redraw(sizeInfo.NewSize);
        }

        private void MeasurePrototype_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine(e.PreviousSize + ", new: "+e.NewSize);
            Redraw(e.NewSize);
        }

        private void Redraw(Size newSize)
        {
            visual.Children.Clear();
            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawRectangle(color, new Pen(), new Rect(0, 0, newSize.Width, newSize.Height));
            }
        }
    }
}
