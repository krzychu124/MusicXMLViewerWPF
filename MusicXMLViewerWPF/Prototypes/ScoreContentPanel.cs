using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.Prototypes
{
    class ScoreContentPanel : Panel
    {
        private RowTopMargins margins;
        private readonly List<UIElement> childsRow = new List<UIElement>();

        public ScoreContentPanel()
        {
            margins = new RowTopMargins(this);
        }



        public static bool GetBottom(DependencyObject obj)
        {
            return (bool)obj.GetValue(BottomProperty);
        }

        public static void SetBottom(DependencyObject obj, bool value)
        {
            obj.SetValue(BottomProperty, value);
        }

        // Using a DependencyProperty as the backing store for Bottom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomProperty =
            DependencyProperty.RegisterAttached("Bottom", typeof(bool), typeof(ScoreContentPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));



        public RowTopMargins RowTopMargins { get => margins; set => margins = value; }

        public static bool GetFullWidthItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(FullWidthItemProperty);
        }

        public static void SetFullWidthItem(DependencyObject obj, bool value)
        {
            obj.SetValue(FullWidthItemProperty, value);
        }

        // Using a DependencyProperty as the backing store for FullWidthItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FullWidthItemProperty =
            DependencyProperty.RegisterAttached("FullWidthItem", typeof(bool), typeof(ScoreContentPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));



        public bool StretchLastRow
        {
            get { return (bool)GetValue(StretchLastRowProperty); }
            set { SetValue(StretchLastRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StretchLastRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchLastRowProperty =
            DependencyProperty.Register("StretchLastRow", typeof(bool), typeof(ScoreContentPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));

       

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = availableSize;

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double cursorX = 0;
            double cursorY = GetMargin(0);
            childsRow.Clear();
            int currentRowIndex = 0;
            foreach (UIElement child in InternalChildren)
            {

                var fullWidthItem = child.GetValue(FullWidthItemProperty);
                var bottomPlaced = child.GetValue(BottomProperty);
                if(bottomPlaced != null &(bool)bottomPlaced)
                {
                    child.Arrange(new Rect(0,finalSize.Height - child.DesiredSize.Height, finalSize.Width, child.DesiredSize.Height));
                    continue;
                }
                if (fullWidthItem != null && (bool)fullWidthItem)
                {
                    if (childsRow.Count != 0)
                    {
                        var rowHeight = childsRow.Max(i => i.DesiredSize.Height);
                        RearrangeRow(childsRow, finalSize.Width, cursorY);
                        cursorY += rowHeight + GetMargin(++currentRowIndex);
                    }
                    cursorX = 0;
                    child.Arrange(new Rect(cursorX, cursorY, finalSize.Width, child.DesiredSize.Height));
                    cursorY += child.DesiredSize.Height + GetMargin(++currentRowIndex);
                    continue;
                }
                double width = child.DesiredSize.Width;
                double height = child.DesiredSize.Height;
                if ((bool)bottomPlaced)
                {

                }
                if (cursorX + width > finalSize.Width)
                {
                    var rowHeight = childsRow.Max(i => i.DesiredSize.Height);
                    RearrangeRow(childsRow, finalSize.Width, cursorY);
                    cursorX = 0;
                    cursorY += rowHeight + GetMargin(++currentRowIndex);
                }
                child.Arrange(new Rect(cursorX, cursorY, width, height));
                childsRow.Add(child);
                cursorX += width;
            }
            if (childsRow.Count != 0 && StretchLastRow)
            {
                RearrangeRow(childsRow, finalSize.Width, cursorY);
            }
            return base.ArrangeOverride(finalSize);
        }

        private void RearrangeRow(List<UIElement> childsRow, double width, double cursorY)
        {
            double currentWidth = childsRow.Sum(i => i.DesiredSize.Width);
            double widthLeft = width - currentWidth;
            double cursorX = 0;
            foreach (var item in childsRow)
            {
                double newWidth = item.DesiredSize.Width + (item.DesiredSize.Width / currentWidth * widthLeft);
                item.Arrange(new Rect(cursorX, cursorY, newWidth, item.DesiredSize.Height));
                cursorX += newWidth;
            }
            childsRow.Clear();
        }
        private double GetMargin(int index)
        {
            return margins.Count > index ? margins[index].TopMargin : 0.0;
        }
    }
}
