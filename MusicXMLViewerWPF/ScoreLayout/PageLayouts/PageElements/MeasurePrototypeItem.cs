using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.ScoreLayout.PageLayouts.PageElements
{
    class MeasurePrototypeItem : IPageElementItem
    {
        private readonly Canvas canvas;
        private readonly DrawingVisualHost visualHost;

        public double Width
        {
            get => canvas.Width; set
            {
                canvas.Width = value;
                DrawNumber(number);
            }
        }

        public double Height { get => canvas.Height; set => canvas.Height = value; }
        private int number;

        public MeasurePrototypeItem(double height)
        {
            visualHost = new DrawingVisualHost();
            canvas = new Canvas
            {
                Height = height,
                Background = Helpers.DrawingHelpers.PickRandomBrush()
            };
            canvas.Children.Add(visualHost);
        }
        public double GetMinWidth()
        {
            return 100;
        }

        public FrameworkElement GetUIElement()
        {
            return canvas;
        }

        public void SetWidth(double width)
        {
            Width = width;
        }

        public void DrawNumber(int count)
        {
            number = count;
            visualHost.ClearVisuals();
            var dv = new DrawingVisual();
            var numberWidth = DrawingHelpers.DrawingMethods.GetTextWidth(count+"", TypeFaces.GetTextFont());
            using (DrawingContext dc = dv.RenderOpen())
            {
                var x = (float)(Width / 2 - (numberWidth / 2));
                var y = (float)(Height / 2 -20);
                Helpers.DrawingHelpers.DrawString(dc, number+"", TypeFaces.GetTextFont(), Brushes.Black,x,y, 30);
            }
            visualHost.AddVisual(dv);
        }

        public double GetWidth()
        {
            return Width;
        }
    }
}
