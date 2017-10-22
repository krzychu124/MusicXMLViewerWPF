using MusicXMLScore.Helpers;
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
        private Brush color;
        public int Number { get; set; }
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

        public String Color { get => color.ToString();  }

        protected override Visual GetVisualChild(int index)
        {
            return visual;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            Redraw(sizeInfo.NewSize);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);//test 
        }

        private void MeasurePrototype_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine(e.PreviousSize + ", new: " + e.NewSize);
            Redraw(e.NewSize);
        }

        private void Redraw(Size newSize)
        {
           // visual.Children.Clear(); //test
            //Console.WriteLine("Was: " + color + " " + Number);
            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawRectangle(color, new Pen(), new Rect(0, 0, newSize.Width, newSize.Height));
                //debug
                //Console.WriteLine("Is: " + color + " " + Number);
                Helpers.DrawingHelpers.DrawString(dc, Number.ToString() + " min width: " + MinWidth.ToString(), TypeFaces.GetTextFont(), Brushes.Black, 5, 10, 10);
                Helpers.DrawingHelpers.DrawString(dc, "actual width: " + ActualWidth.ToString("#.0"), TypeFaces.GetTextFont(), Brushes.Black, 5, 22, 10);
            }
        }
    }
}
