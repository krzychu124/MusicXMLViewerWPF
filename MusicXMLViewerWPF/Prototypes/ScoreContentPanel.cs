using MusicXMLScore.Helpers;
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

        // Using a DependencyProperty as the backing store for Bottom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomProperty =
            DependencyProperty.RegisterAttached("Bottom", typeof(bool), typeof(ScoreContentPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        // Using a DependencyProperty as the backing store for FullWidthItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FullWidthItemProperty =
            DependencyProperty.RegisterAttached("FullWidthItem", typeof(bool), typeof(ScoreContentPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        // Using a DependencyProperty as the backing store for NextPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextPanelProperty =
            DependencyProperty.Register(nameof(NextPanel), typeof(ScoreContentPanel), typeof(ScoreContentPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        // Using a DependencyProperty as the backing store for StretchLastRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchLastRowProperty =
            DependencyProperty.Register(nameof(StretchLastRow), typeof(bool), typeof(ScoreContentPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
        private readonly List<UIElement> childsRow = new List<UIElement>();
        private RowTopMargins margins;

        public ScoreContentPanel()
        {
            margins = new RowTopMargins(this);

        }


        public ScoreContentPanel NextPanel
        {
            get { return (ScoreContentPanel)GetValue(NextPanelProperty); }
            set { SetValue(NextPanelProperty, value); }
        }



        public RowTopMargins RowTopMargins { get => margins; set => margins = value; }



        public bool StretchLastRow
        {
            get { return (bool)GetValue(StretchLastRowProperty); }
            set { SetValue(StretchLastRowProperty, value); }
        }



        public static bool GetBottom(DependencyObject obj)
        {
            return (bool)obj.GetValue(BottomProperty);
        }

        public static bool GetFullWidthItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(FullWidthItemProperty);
        }

        public static void SetBottom(DependencyObject obj, bool value)
        {
            obj.SetValue(BottomProperty, value);
        }

        public static void SetFullWidthItem(DependencyObject obj, bool value)
        {
            obj.SetValue(FullWidthItemProperty, value);
        }

        public bool HasNextPanel() => NextPanel != null;

        public void InsertChild(UIElement child)
        {
            this.Children.Insert(0, child);
            Console.WriteLine("Child inserted");
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double cursorX = 0;
            double cursorY = GetMargin(0);
            childsRow.Clear();
            int currentRowIndex = 0;
            foreach (UIElement child in InternalChildren)
            {
                //var measure = child as MeasurePrototype;
                //if(measure != null)
                //{
                //    Console.WriteLine("A_" + measure.Number + "_" + measure.Color);
                //}
                var fullWidthItem = child.GetValue(FullWidthItemProperty);
                var bottomPlaced = child.GetValue(BottomProperty);
                if ((bool)bottomPlaced)
                {
                    child.Arrange(new Rect(0, finalSize.Height - child.DesiredSize.Height, finalSize.Width, child.DesiredSize.Height));
                    continue;
                }
                if ((bool)fullWidthItem)
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
                if (cursorX + width > finalSize.Width)
                {
                    double rowHeight = child.DesiredSize.Height;
                    if (childsRow.Count != 0)
                    {
                        rowHeight = childsRow.Max(i => i.DesiredSize.Height);
                        RearrangeRow(childsRow, finalSize.Width, cursorY);
                    }
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
            Console.WriteLine("Arranged");
            return base.ArrangeOverride(finalSize);
        }



        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);
            bool hasNextPanel = HasNextPanel();
            double cursorX = 0;
            double cursorY = GetMargin(0);
            int currentRowIndex = 0;
            double currentRowHeight = 0;
            foreach (UIElement child in InternalChildren)
            {
                //var measure = child as MeasurePrototype;
                //if (measure != null)
                //{
                //    Console.WriteLine("M_" + measure.Number +"_"+measure.Color);
                //}
                var fullWidthItem = child.GetValue(FullWidthItemProperty);
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                currentRowHeight = child.DesiredSize.Height > currentRowHeight ? child.DesiredSize.Height : currentRowHeight;
                if ((bool)fullWidthItem)
                {
                    double calculatedHeight = cursorY + currentRowHeight + GetMargin(++currentRowIndex);
                    if (calculatedHeight > availableSize.Height)
                    {
                        if (HasNextPanel())
                        {
                            var sourceArray = new UIElement[InternalChildren.Count];
                            InternalChildren.CopyTo(sourceArray, 0);
                            int index = InternalChildren.IndexOf(child);
                            var array = new UIElement[InternalChildren.Count - index];
                            Array.Copy(sourceArray, index, array, 0, InternalChildren.Count - index);
                            Array.Reverse(array);
                            foreach (var item in array)
                            {
                                InternalChildren.Remove(item);
                                NextPanel.InsertChild(item);
                            }
                            return size;
                        }
                    }
                    cursorY += currentRowHeight + GetMargin(++currentRowIndex);
                    cursorX = 0;
                }
                else
                {
                    if (cursorX + child.DesiredSize.Width > availableSize.Width)
                    {
                        double calculatedHeight = cursorY + currentRowHeight + GetMargin(++currentRowIndex);
                        if (calculatedHeight + child.DesiredSize.Height > availableSize.Height)
                        {
                            if (HasNextPanel())
                            {
                                var sourceArray = new UIElement[InternalChildren.Count];
                                InternalChildren.CopyTo(sourceArray, 0);
                                int index = InternalChildren.IndexOf(child);
                                var array = new UIElement[InternalChildren.Count - index];
                                Array.Copy(sourceArray, index, array, 0, InternalChildren.Count - index);
                                Array.Reverse(array);
                                foreach (var item in array)
                                {
                                    InternalChildren.Remove(item);
                                    NextPanel.InsertChild(item);
                                }
                                return size;
                            }
                        }
                        cursorX = child.DesiredSize.Width;
                        cursorY = calculatedHeight;
                    }
                    else
                    {
                        cursorX += child.DesiredSize.Width;
                    }
                }
            }
            Console.WriteLine("Measured");
            return size;
        }
        private double GetMargin(int index)
        {
            return margins.Count > index ? margins[index].TopMargin : 0.0;
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
    }
}
