using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace MusicXMLScore.Helpers
{
    public class CustomWrapPanel : Panel
    {
        private Orientation _orientation;
        public CustomWrapPanel():base()
        {
            _orientation = Orientation.Horizontal;
        }

        public Orientation Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        #region Dependency Properties
        public static readonly DependencyProperty StretchItemsInRowProperty =
            DependencyProperty.Register(
                "StretchItemsInRow",
                typeof(bool),
                typeof(CustomWrapPanel),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStretchItemsInRowChanged), new CoerceValueCallback(CorerceStretchItemsInRow)),
                new ValidateValueCallback(IsStretchItemsInRowValid)
                );

        public static readonly DependencyProperty LastItemRowStretchProperty =
            DependencyProperty.Register(
                "LastItemRowStretch",
                typeof(bool),
                typeof(CustomWrapPanel),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnLastItemRowStretchChanged), new CoerceValueCallback(CoerceLastItemRowStretch)),
                new ValidateValueCallback(IsLastRowItemStretchValid));

        public static readonly DependencyProperty ItemHeightCProperty =
                DependencyProperty.Register(
                        "ItemHeight",
                        typeof(double),
                        typeof(CustomWrapPanel),
                        new FrameworkPropertyMetadata( Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
                        new ValidateValueCallback(IsWidthHeightValid));

        public static readonly DependencyProperty ItemWidthCProperty =
                DependencyProperty.Register(
                        "ItemWidth",
                        typeof(double),
                        typeof(CustomWrapPanel),
                        new FrameworkPropertyMetadata(
                                Double.NaN,
                                FrameworkPropertyMetadataOptions.AffectsMeasure),
                        new ValidateValueCallback(IsWidthHeightValid));
        #endregion

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightCProperty); }
            set { SetValue(ItemHeightCProperty, value); }
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthCProperty); }
            set { SetValue(ItemWidthCProperty, value); }
        }

        public bool LastItemRowStretch
        {
            get { return (bool)GetValue(LastItemRowStretchProperty); }
            set { SetValue(LastItemRowStretchProperty, value); }
        }

        public bool StretchItemsInRow
        {
            get { return (bool)GetValue(StretchItemsInRowProperty); }
            set { SetValue(StretchItemsInRowProperty, value); }
        }

        private static bool IsWidthHeightValid(object value)
        {
            double v = (double)value;
            return (double.IsNaN(v)) || (v >= 0.0d && !Double.IsPositiveInfinity(v));
        }

        private static bool IsLastRowItemStretchValid(object value)
        {
            return value is bool;
        }

        private static bool IsStretchItemsInRowValid(object value)
        {
            return value is bool;
        }

        private static void OnStretchItemsInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomWrapPanel value = (CustomWrapPanel)d;
            d.CoerceValue(LastItemRowStretchProperty);
        }

        private static void OnLastItemRowStretchChanged(DependencyObject d , DependencyPropertyChangedEventArgs e)
        {
            CustomWrapPanel value = (CustomWrapPanel)d;
            d.CoerceValue(StretchItemsInRowProperty);
        }

        private static object CorerceStretchItemsInRow(DependencyObject d, object value)
        {
            CustomWrapPanel c = (CustomWrapPanel)d;
            bool b = (bool)value;
            if (b == true && c.LastItemRowStretch == b)
            {
                return false;
            }
            else
            {
                return value;
            }
        }

        private static object CoerceLastItemRowStretch(DependencyObject d, object value)
        {
            CustomWrapPanel c = (CustomWrapPanel)d;
            bool b = (bool)value;
            if (b == true && c.StretchItemsInRow == b)
            {
                return false;
            }
            else
            {
                return value;
            }
        }

        private struct UVSize
        {
            internal UVSize(Orientation orientation, double width, double height)
            {
                U = V = 0d;
                _orientation = orientation;
                Width = width;
                Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                U = V = 0d;
                _orientation = orientation;
            }

            internal double U;
            internal double V;
            private Orientation _orientation;

            internal double Width
            {
                get { return (_orientation == Orientation.Horizontal ? U : V); }
                set { if (_orientation == Orientation.Horizontal) U = value; else V = value; }
            }
            internal double Height
            {
                get { return (_orientation == Orientation.Horizontal ? V : U); }
                set { if (_orientation == Orientation.Horizontal) V = value; else U = value; }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size curLineSize = new Size();
            Size panelSize = new Size();
            Size givenConstraint = new Size(constraint.Width, constraint.Height);
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool itemWidthSet = !double.IsNaN(itemWidth);
            bool itemHeightSet = !double.IsNaN(itemHeight);

            Size childConstraint = new Size(
                (itemWidthSet ? itemWidth : constraint.Width),
                (itemHeightSet ? itemHeight : constraint.Height));

            UIElementCollection children = InternalChildren;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child == null) continue;

                //Flow passes its own constrint to children
                child.Measure(childConstraint);

                //this is the size of the child in UV space
                Size sz = new Size(
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (GreaterThan(curLineSize.Width + sz.Width, givenConstraint.Width)) //need to switch to another line
                {
                    panelSize.Width = Math.Max(curLineSize.Width, panelSize.Width);
                    panelSize.Height += curLineSize.Height;
                    curLineSize = sz;

                    if (GreaterThan(sz.Width, givenConstraint.Width)) //the element is wider then the constrint - give it a separate line                    
                    {
                        panelSize.Width = Math.Max(sz.Width, panelSize.Width);
                        panelSize.Height += sz.Height;
                        curLineSize = new Size();
                    }
                }
                else //continue to accumulate a line
                {
                    curLineSize.Width += sz.Width;
                    curLineSize.Height = Math.Max(sz.Height, curLineSize.Height);
                }
            }

            //the last line size, if any should be added
            panelSize.Width = Math.Max(curLineSize.Width, panelSize.Width);
            panelSize.Height += curLineSize.Height;

            //go from UV space to W/H space
            return new Size(panelSize.Width, panelSize.Height);
        }

        /// <summary>
        /// <see cref="FrameworkElement.ArrangeOverride"/>
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int firstInLine = 0;
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            double accumulatedV = 0;
            double itemU = (Orientation == Orientation.Horizontal ? itemWidth : itemHeight);
            Size curLineSize = new Size();
            Size givenFinalSize = new Size(finalSize.Width, finalSize.Height);
            bool itemWidthSet = !double.IsNaN(itemWidth);
            bool itemHeightSet = !double.IsNaN(itemHeight);
            bool useItemU = (Orientation == Orientation.Horizontal ? itemWidthSet : itemHeightSet);

            UIElementCollection children = InternalChildren;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child == null) continue;

                Size sz = new Size(
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (GreaterThan(curLineSize.Width + sz.Width, givenFinalSize.Width)) //need to switch to another line
                {
                    arrangeLine(accumulatedV, curLineSize.Height, firstInLine, i, useItemU, itemU);

                    accumulatedV += curLineSize.Height;
                    curLineSize = sz;

                    if (GreaterThan(sz.Width, givenFinalSize.Width)) //the element is wider then the constraint - give it a separate line                    
                    {
                        //switch to next line which only contain one element
                        arrangeLine(accumulatedV, sz.Height, i, ++i, useItemU, itemU);

                        accumulatedV += sz.Height;
                        curLineSize = new Size();
                    }
                    firstInLine = i;
                }
                else //continue to accumulate a line
                {
                    curLineSize.Width += sz.Width;
                    curLineSize.Height = Math.Max(sz.Height, curLineSize.Height);
                }
            }

            //arrange the last line, if any
            if (firstInLine < children.Count)
            {
                arrangeLine(accumulatedV, curLineSize.Height, firstInLine, children.Count, useItemU, itemU);
            }

            return finalSize;
        }

        private void arrangeLine(double v, double lineV, int start, int end, bool useItemU, double itemU)
        {
            double u = 0;
            bool isHorizontal = (Orientation == Orientation.Horizontal);

            UIElementCollection children = InternalChildren;
            for (int i = start; i < end; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child != null)
                {
                    Size childSize = new Size(child.DesiredSize.Width, child.DesiredSize.Height);
                    double layoutSlotU = (useItemU ? itemU : childSize.Width);
                    child.Arrange(new Rect(
                        (isHorizontal ? u : v),
                        (isHorizontal ? v : u),
                        (isHorizontal ? layoutSlotU : lineV),
                        (isHorizontal ? lineV : layoutSlotU)));
                    u += layoutSlotU;
                }
            }
        }
        public static bool GreaterThan(double value1, double value2)
        {
            return (value1 > value2) && !AreClose(value1, value2);
        }
        public static bool AreClose(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            if (value1 == value2) return true;
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }
        const double DBL_EPSILON = 2.2204460492503131e-016;
    }
}
