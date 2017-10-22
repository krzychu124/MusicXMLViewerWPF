using MusicXMLScore.Converters;
using MusicXMLScore.ScoreLayout.MeasureLayouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.Prototypes
{
    class MeasureVisualPrototype : FrameworkElement
    {
        private readonly RegularStaff staff;
        private readonly DrawingVisual visual;
        public MeasureVisualPrototype()
        {
            visual = new DrawingVisual();
            Height = 50;
            staff = new RegularStaff(5, Height, MinWidth);

            AddVisualChild(visual);
            AddLogicalChild(visual);
            AddVisualChild(staff.GetVisualsContainer());
            AddLogicalChild(staff.GetVisualsContainer());
        }

        protected override int VisualChildrenCount => 2;

        protected override Visual GetVisualChild(int index)
        {
            return index == 0 ? visual : (Visual)staff.GetVisualsContainer();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            Redraw(sizeInfo.NewSize);
        }
        private void Redraw(Size newSize)
        {
            staff.DesiredWidth = newSize.Width;
            staff.Update();
            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawLine(new Pen(Brushes.Black, 1.5), new Point(newSize.Width, 0), new Point(newSize.Width, staff.GetYOfLine(1, 0)));
            }

        }
    }
}
