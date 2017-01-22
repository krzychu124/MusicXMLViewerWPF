using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace MusicXMLScore.Helpers
{
    public class CustomSystemWrapPanel : Panel
    {
        private Orientation _orientation;
        public CustomSystemWrapPanel():base()
        {
            _orientation = Orientation.Horizontal;
        }

        public Orientation Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        #region Dependency Properties
        /// <summary>
        /// Items in row will be stretched equally to fill avaliable row space. Don't use along with StretchLastItemProperty
        /// </summary>
        public static readonly DependencyProperty StretchItemsInRowProperty =
            DependencyProperty.Register(
                "StretchItemsInRow",
                typeof(bool),
                typeof(CustomSystemWrapPanel),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnStretchItemsInRowChanged), new CoerceValueCallback(CorerceStretchItemsInRow)),
                new ValidateValueCallback(IsStretchItemsInRowValid)
                );
        /// <summary>
        /// Last item in every row will be stretched to fill avaliable row space. Don't use along with StretchItemsInRowProperty
        /// </summary>
        public static readonly DependencyProperty StretchLastItemInRowProperty =
            DependencyProperty.Register(
                "StretchLastItemInRow",
                typeof(bool),
                typeof(CustomSystemWrapPanel),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnStretchLastItemInRowChanged), new CoerceValueCallback(CoerceStretchLastItemInRow)),
                new ValidateValueCallback(IsLastRowItemStretchValid));

        public static readonly DependencyProperty ItemHeightProperty =
                DependencyProperty.Register(
                        "ItemHeight",
                        typeof(double),
                        typeof(CustomSystemWrapPanel),
                        new FrameworkPropertyMetadata( Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
                        new ValidateValueCallback(IsWidthHeightValid));

        public static readonly DependencyProperty ItemWidthProperty =
                DependencyProperty.Register(
                        "ItemWidth",
                        typeof(double),
                        typeof(CustomSystemWrapPanel),
                        new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
                        new ValidateValueCallback(IsWidthHeightValid));
        /// <summary>
        /// Stops stretching this item content
        /// </summary>
        public static DependencyProperty PreventItemMeasureArrangeProperty =
                DependencyProperty.RegisterAttached(
                        "PreventItemMeasureArrange",
                        typeof(bool),
                        typeof(CustomSystemWrapPanel),
                        new FrameworkPropertyMetadata(
                                false, 
                                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        /// <summary>
        /// Breaks row and moves item to next row
        /// </summary>
        public static DependencyProperty BreakSystemRowProperty =
            DependencyProperty.RegisterAttached(
                "BreakSystemRow",
                typeof(bool),
                typeof(CustomSystemWrapPanel),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
               //? new ValidateValueCallback(IsBreakRowValid));
        #endregion

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public bool StretchLastItemInRow
        {
            get { return (bool)GetValue(StretchLastItemInRowProperty); }
            set { SetValue(StretchLastItemInRowProperty, value); }
        }

        public bool StretchItemsInRow
        {
            get { return (bool)GetValue(StretchItemsInRowProperty); }
            set { SetValue(StretchItemsInRowProperty, value); }
        }

        public static void SetPreventItemMeasureArrange(UIElement element, bool value)
        {
            element.SetValue(PreventItemMeasureArrangeProperty, value);
        }

        public static bool GetPreventItemMeasureArrange(UIElement element)
        {
            return (bool)element.GetValue(PreventItemMeasureArrangeProperty);
        }

        public static void SetBreakSystemRow(UIElement element, bool value)
        {
            element.SetValue(BreakSystemRowProperty, value);
        }

        public static bool GetBreakSystemRow(UIElement element)
        {
            return (bool)element.GetValue(BreakSystemRowProperty);
        }

        public void AddChild(UIElement child)
        {
            this.Children.Add(child);
        }

        private static bool IsBreakRowValid(object value) //! Validation suspended for now
        {
            return value is bool;
        }

        private static bool PreventItemMeasureArrange(object value)
        {
            return value is bool;
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
            CustomSystemWrapPanel value = (CustomSystemWrapPanel)d;
            d.CoerceValue(StretchLastItemInRowProperty);
            value.InvalidateVisual();
        }

        private static void OnStretchLastItemInRowChanged(DependencyObject d , DependencyPropertyChangedEventArgs e)
        {
            CustomSystemWrapPanel value = (CustomSystemWrapPanel)d;
            d.CoerceValue(StretchItemsInRowProperty);
            value.InvalidateVisual();
        }

        private static object CorerceStretchItemsInRow(DependencyObject d, object value)
        {
            CustomSystemWrapPanel c = (CustomSystemWrapPanel)d;
            bool b = (bool)value;
            if (b == true && c.StretchLastItemInRow == b)
            {
                return value;
            }
            else
            {
                return value;
            }
        }
        private int x = 100;
        private static object CoerceStretchLastItemInRow(DependencyObject d, object value)
        {
            CustomSystemWrapPanel c = (CustomSystemWrapPanel)d;
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


        //private struct UVSiz
        //{
        //    internal UVSiz(Orientation orientation, double width, double height)
        //    {
        //        U = V = 0d;
        //        _orientation = orientation;
        //        Width = width;
        //        Height = height;
        //    }

        //    internal UVSiz(Orientation orientation)
        //    {
        //        U = V = 0d;
        //        _orientation = orientation;
        //    }

        //    internal double U;
        //    internal double V;
        //    private Orientation _orientation;

        //    internal double Width
        //    {
        //        get { return (_orientation == Orientation.Horizontal ? U : V); }
        //        set { if (_orientation == Orientation.Horizontal) U = value; else V = value; }
        //    }
        //    internal double Height
        //    {
        //        get { return (_orientation == Orientation.Horizontal ? V : U); }
        //        set { if (_orientation == Orientation.Horizontal) V = value; else U = value; }
        //    }
        //}

        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            Size childsconstraint = new Size() { Width = constraint.Width, Height = constraint.Height };
            Size panelsize = new Size(constraint.Width, 0);
            Size linesize = new Size();
            int linestart = 0;
            int lineend = 0;

            UIElementCollection childrens = InternalChildren;
            int count = childrens.Count;
            for (int i = 0; i < count; i++)
            {
                UIElement child = childrens[i];
                if (child == null) continue;

                child.Measure(constraint);
                bool childbreaksrow = false;
                object breakrow = child.GetValue(CustomSystemWrapPanel.BreakSystemRowProperty);
                if (breakrow != null)
                {
                    childbreaksrow = (bool)breakrow;
                }
                
                Size childSize = child.DesiredSize;
                if(GreaterThan(linesize.Width + childSize.Width, childsconstraint.Width) || childbreaksrow == true)
                {
                    panelsize.Height += linesize.Height;
                    double offset = panelsize.Width - linesize.Width;
                    magain(panelsize, linesize, linestart, lineend);
                    linesize = childSize;
                    linestart = i;
                }
                else
                {
                    linesize.Width += childSize.Width;
                    linesize.Height = Math.Max(childSize.Height, linesize.Height);
                    lineend = i;
                }
            }
            if (linesize.Width < panelsize.Width)
            {
                magain(panelsize, linesize, linestart, lineend);
                panelsize.Height += linesize.Height;
            }
            panelsize.Height += linesize.Height;
            return panelsize;
            //return MO(constraint);

            //Orientation o = Orientation;
            //OrientedSize curLineSize = new OrientedSize(o);
            //OrientedSize panelSize = new OrientedSize(o);
            //OrientedSize maxSize = new OrientedSize(o, constraint.Width, constraint.Height);
            //int linebegin = 0;
            //int lineend = 0;
            //double itemWidth = ItemWidth;
            //double itemHeight = ItemHeight;
            //bool itemWidthSet = !double.IsNaN(itemWidth); //! has fixed width
            //bool itemHeightSet = !double.IsNaN(itemHeight); //! has fixed height

            //Size childConstraint = new Size(
            //    (itemWidthSet ? itemWidth : constraint.Width),
            //    (itemHeightSet ? itemHeight : constraint.Height));

            //UIElementCollection children = InternalChildren;
            //lineend = children.Count;
            //for (int i = 0, count = children.Count; i < count; i++)
            //{
            //    UIElement child = children[i] as UIElement;
            //    if (child == null) continue;

            //    //Flow passes its own constrint to children
            //    child.Measure(childConstraint);
            //    //this is the size of the child
            //    OrientedSize elementSize = new OrientedSize(o, 
            //        (itemWidthSet ? itemWidth : child.DesiredSize.Width),
            //        (itemHeightSet ? itemHeight : child.DesiredSize.Height));

            //    if (GreaterThan(curLineSize.Direct + elementSize.Direct, maxSize.Direct)) //need to switch to another line
            //    {
            //        panelSize.Direct = Math.Max(curLineSize.Direct, panelSize.Direct);
            //        panelSize.Indirect += curLineSize.Indirect;
            //        MeasureAgain(linebegin, count, maxSize.Direct, maxSize.Direct - curLineSize.Direct);
            //        curLineSize = elementSize;
            //        //child.Measure(childConstraint);
            //        if (GreaterThan(elementSize.Direct, maxSize.Direct)) //the element is wider then the constrint - give it a separate line                    
            //        {
            //            panelSize.Direct = Math.Max(elementSize.Direct, panelSize.Direct);
            //            panelSize.Indirect += elementSize.Indirect;
            //            //MeasureAgain(linebegin, ++count, maxSize.Direct, maxSize.Direct - curLineSize.Direct);
            //            curLineSize = new OrientedSize(o);
            //        }
            //        linebegin = i;
            //    }
            //    else //continue to accumulate a line
            //    {
            //        curLineSize.Direct += elementSize.Direct;
            //        curLineSize.Indirect = Math.Max(elementSize.Indirect, curLineSize.Indirect);
                    
            //    }
            //}

            ////the last line size, if any should be added
            //panelSize.Direct = Math.Max(curLineSize.Direct, panelSize.Direct);
            //panelSize.Indirect += curLineSize.Indirect;

            ////go from UV space to W/H space
            ////? panelSize.Width, panelSize.Height
            //return new Size(panelSize.Width, panelSize.Height);
        }
        private void magain(Size panelsize, Size linesize, int linestart, int lineend)
        {
            double offset = panelsize.Width - linesize.Width;
            double divisor = lineend - linestart != 0 ? lineend - linestart : 0;
            double childoffset = offset / ++divisor;
            for (int i = linestart; i < lineend; i++)
            {
                UIElement child = InternalChildren[i];
                if (child == null) continue;
                Size childSize = child.DesiredSize;
                //child.Measure(new Size());
                double calculated = childSize.Width + offset;
                child.Measure(new Size(calculated, childSize.Height));
            }
        }

        protected override Size ArrangeOverride(Size finalSize) //TODO_I needs improvements WiP
        {
            //int lineBegin = 0;
            //Orientation o = Orientation;
            //OrientedSize curLineSize = new OrientedSize(o);
            //OrientedSize maxSize = new OrientedSize(o, finalSize.Width, finalSize.Height);

            //double itemWidth = ItemWidth;
            //double itemHeight = ItemHeight;
            //double accumulatedHeight = 0;
            //double itemU = (Orientation == Orientation.Horizontal ? itemWidth : itemHeight);

            //bool itemWidthSet = !double.IsNaN(itemWidth);
            //bool itemHeightSet = !double.IsNaN(itemHeight);
            //bool useItemU = (Orientation == Orientation.Horizontal ? itemWidthSet : itemHeightSet);

            //double indirectOffset = 0.0;
            //double? directDelta = (o == Orientation.Horizontal) ?
            //    (itemWidthSet ? (double?)itemWidth : null) :
            //    (itemHeightSet ? (double?)ItemHeight : null);

            //UIElementCollection children = InternalChildren;
            //int count = children.Count;
            //for (int lineEnd = 0; lineEnd < count; lineEnd++)
            //{
            //    UIElement child = children[lineEnd] as UIElement;
            //    if (child == null) continue;

            //    OrientedSize elementSize = new OrientedSize(o, 
            //        (itemWidthSet ? itemWidth : child.DesiredSize.Width),
            //        (itemHeightSet ? itemHeight : child.DesiredSize.Height));

            //    if (GreaterThan(curLineSize.Direct + elementSize.Direct, maxSize.Direct)) //need to switch to another line
            //    {
            //        if (StretchItemsInRow)
            //        {
            //            arrangeLine(accumulatedHeight, curLineSize.Height, lineBegin, lineEnd, useItemU, itemU);
            //        }
            //        else
            //        {
            //            arrangeLineWithScale(lineBegin, lineEnd, directDelta, maxSize.Direct, indirectOffset, curLineSize.Indirect);
            //        }

            //        indirectOffset += curLineSize.Indirect;
            //        accumulatedHeight += curLineSize.Indirect;
            //        curLineSize = elementSize;

            //        if (GreaterThan(elementSize.Direct, maxSize.Direct)) //the element is wider then the constraint - give it a separate line                    
            //        {
            //            //switch to next line which only contain one element
            //            if (StretchItemsInRow)
            //            {
            //                arrangeLine(accumulatedHeight, elementSize.Height, lineEnd, ++lineEnd, useItemU, itemU);
            //            }
            //            else
            //            {
            //                arrangeLineWithScale(lineEnd, ++lineEnd, directDelta, maxSize.Direct, indirectOffset, elementSize.Indirect);
            //            }
            //            indirectOffset += elementSize.Indirect;
            //            accumulatedHeight += elementSize.Indirect;
            //            curLineSize = new OrientedSize(o);
            //        }
            //        lineBegin = lineEnd;
            //    }
            //    else //continue to accumulate a line
            //    {
            //        curLineSize.Direct += elementSize.Direct;
            //        curLineSize.Indirect = Math.Max(elementSize.Indirect, curLineSize.Indirect);
            //    }
            //}

            ////arrange the last line, if any
            //if (lineBegin < children.Count)
            //{
            //    if (StretchItemsInRow)
            //    {
            //        arrangeLine(accumulatedHeight, curLineSize.Height, lineBegin, children.Count, useItemU, itemU);
            //    }
            //    else
            //    {
            //        arrangeLineWithScale(lineBegin, count, directDelta, maxSize.Direct, indirectOffset, curLineSize.Indirect);
            //    }
            //}

            //return new Size(finalSize.Width, finalSize.Height);
            int linestart = 0;
            Size currentlinesize = new Size();
            Size maxlinesize = new Size(finalSize.Width, finalSize.Height);
            double accheight = 0;
            int lineend = 0;
            UIElementCollection children = InternalChildren;
            int count = children.Count;
            for (int i = 0; i < count; i++)
            {
                lineend = i;
                UIElement child = children[i] as UIElement;
                if (child == null) continue;
                bool childbreaksrow = false;
                object breakrow = child.GetValue(CustomSystemWrapPanel.BreakSystemRowProperty);
                if (breakrow != null)
                { 
                    childbreaksrow = (bool)breakrow;
                }
                currentlinesize.Height = child.DesiredSize.Height;
                if (GreaterThan(currentlinesize.Width + child.DesiredSize.Width, maxlinesize.Width) || childbreaksrow == true)
                {
                    double offset = maxlinesize.Width - currentlinesize.Width;
                    arrangeLine(accheight, child.DesiredSize.Height, linestart, lineend, false, 0.0, true, offset);
                    accheight += child.DesiredSize.Height;
                    currentlinesize.Width = child.DesiredSize.Width;
                    linestart = lineend;
                }
                else
                {
                    currentlinesize.Width += child.DesiredSize.Width;
                }
            }
            if (linestart < count)
            {
                arrangeLine(accheight, currentlinesize.Height, linestart, count, false, 0.0, true, maxlinesize.Width - currentlinesize.Width);
            }
            return new Size(maxlinesize.Width, accheight);
            //return AO(finalSize);
        }

        private void AL()
        {
          
        }
        private Size MO(Size constraint)
        {
            Orientation o = Orientation;
            OrientedSize curLineSize = new OrientedSize(o);
            OrientedSize panelSize = new OrientedSize(o);
            OrientedSize maxSize = new OrientedSize(o, constraint.Width, constraint.Height);
            int linebegin = 0;
            int lineend = 0;
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool itemWidthSet = !double.IsNaN(itemWidth); //! has fixed width
            bool itemHeightSet = !double.IsNaN(itemHeight); //! has fixed height

            Size childConstraint = new Size(
                (itemWidthSet ? itemWidth : constraint.Width),
                (itemHeightSet ? itemHeight : constraint.Height));

            UIElementCollection children = InternalChildren;
            lineend = children.Count;
            for (int i = 0, count = children.Count; i < count; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child == null) continue;

                //Flow passes its own constrint to children
                child.Measure(childConstraint);
                //this is the size of the child
                OrientedSize elementSize = new OrientedSize(o,
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (GreaterThan(curLineSize.Direct + elementSize.Direct, maxSize.Direct)) //need to switch to another line
                {
                    panelSize.Direct = Math.Max(curLineSize.Direct, panelSize.Direct);
                    panelSize.Indirect += curLineSize.Indirect;
                    MeasureAgain(linebegin, count, maxSize.Direct, maxSize.Direct - curLineSize.Direct);
                    curLineSize = elementSize;
                    //child.Measure(childConstraint);
                    if (GreaterThan(elementSize.Direct, maxSize.Direct)) //the element is wider then the constrint - give it a separate line                    
                    {
                        panelSize.Direct = Math.Max(elementSize.Direct, panelSize.Direct);
                        panelSize.Indirect += elementSize.Indirect;
                        //MeasureAgain(linebegin, ++count, maxSize.Direct, maxSize.Direct - curLineSize.Direct);
                        curLineSize = new OrientedSize(o);
                    }
                    linebegin = i;
                }
                else //continue to accumulate a line
                {
                    curLineSize.Direct += elementSize.Direct;
                    curLineSize.Indirect = Math.Max(elementSize.Indirect, curLineSize.Indirect);

                }
            }

            //the last line size, if any should be added
            panelSize.Direct = Math.Max(curLineSize.Direct, panelSize.Direct);
            panelSize.Indirect += curLineSize.Indirect;

            //go from UV space to W/H space
            //? panelSize.Width, panelSize.Height
            return new Size(panelSize.Width, panelSize.Height);
        }

        private Size AO(Size finalSize)
        {
            int lineBegin = 0;
            Orientation o = Orientation;
            OrientedSize curLineSize = new OrientedSize(o);
            OrientedSize maxSize = new OrientedSize(o, finalSize.Width, finalSize.Height);

            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            double accumulatedHeight = 0;
            double itemU = (Orientation == Orientation.Horizontal ? itemWidth : itemHeight);

            bool itemWidthSet = !double.IsNaN(itemWidth);
            bool itemHeightSet = !double.IsNaN(itemHeight);
            bool useItemU = (Orientation == Orientation.Horizontal ? itemWidthSet : itemHeightSet);

            double indirectOffset = 0.0;
            double? directDelta = (o == Orientation.Horizontal) ?
                (itemWidthSet ? (double?)itemWidth : null) :
                (itemHeightSet ? (double?)ItemHeight : null);

            UIElementCollection children = InternalChildren;
            int count = children.Count;
            for (int lineEnd = 0; lineEnd < count; lineEnd++)
            {
                UIElement child = children[lineEnd] as UIElement;
                if (child == null) continue;

                OrientedSize elementSize = new OrientedSize(o,
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (GreaterThan(curLineSize.Direct + elementSize.Direct, maxSize.Direct)) //need to switch to another line
                {
                    if (StretchItemsInRow)
                    {
                        arrangeLine(accumulatedHeight, curLineSize.Height, lineBegin, lineEnd, useItemU, itemU);
                    }
                    else
                    {
                        arrangeLineWithScale(lineBegin, lineEnd, directDelta, maxSize.Direct, indirectOffset, curLineSize.Indirect);
                    }

                    indirectOffset += curLineSize.Indirect;
                    accumulatedHeight += curLineSize.Indirect;
                    curLineSize = elementSize;

                    if (GreaterThan(elementSize.Direct, maxSize.Direct)) //the element is wider then the constraint - give it a separate line                    
                    {
                        //switch to next line which only contain one element
                        if (StretchItemsInRow)
                        {
                            arrangeLine(accumulatedHeight, elementSize.Height, lineEnd, ++lineEnd, useItemU, itemU);
                        }
                        else
                        {
                            arrangeLineWithScale(lineEnd, ++lineEnd, directDelta, maxSize.Direct, indirectOffset, elementSize.Indirect);
                        }
                        indirectOffset += elementSize.Indirect;
                        accumulatedHeight += elementSize.Indirect;
                        curLineSize = new OrientedSize(o);
                    }
                    lineBegin = lineEnd;
                }
                else //continue to accumulate a line
                {
                    curLineSize.Direct += elementSize.Direct;
                    curLineSize.Indirect = Math.Max(elementSize.Indirect, curLineSize.Indirect);
                }
            }

            //arrange the last line, if any
            if (lineBegin < children.Count)
            {
                if (StretchItemsInRow)
                {
                    arrangeLine(accumulatedHeight, curLineSize.Height, lineBegin, children.Count, useItemU, itemU);
                }
                else
                {
                    arrangeLineWithScale(lineBegin, count, directDelta, maxSize.Direct, indirectOffset, curLineSize.Indirect);
                }
            }

            return new Size(finalSize.Width, finalSize.Height);
        }

        private void MeasureAgain(int b, int e, double width, double offset)
        {
            UIElementCollection children = InternalChildren;
            double d_w = 0.0;
            double d_h = 0.0;
            double accumulatedwidth = 0.0;
            for (int i = b; i < e; i++)
            {
                UIElement child = children[i];
               // accumulatedwidth += child.DesiredSize.Width;
            }
            if (accumulatedwidth > width)
            {
                int u = 0;
            }
            for (int i = b; i < e; i++)
            {
                UIElement child = children[i];
                //child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                d_w = double.PositiveInfinity; //! child.DesiredSize.Width;
                d_h = child.DesiredSize.Height;
                child.Measure(new Size(d_w + offset, d_h));
                accumulatedwidth += child.DesiredSize.Width;
                if (accumulatedwidth > width) return;
            }
        }


        private void arrangeLine(double height, double lineHeight, int start, int end, bool useItemU, double itemU, bool scale = false, double lineoffset = 0)
        {
            if (scale == false)
            {
                double width = 0;
                bool isHorizontal = (Orientation == Orientation.Horizontal);

                UIElementCollection children = InternalChildren;
                for (int i = start; i < end; i++)
                {
                    UIElement child = children[i] as UIElement;
                    if (child != null)
                    {
                        Size childSize = new Size(child.DesiredSize.Width, child.DesiredSize.Height);
                        double layoutSlotWidth = (useItemU ? itemU : childSize.Width);
                        child.Arrange(new Rect(
                            (isHorizontal ? width : height),
                            (isHorizontal ? height : width),
                            (isHorizontal ? layoutSlotWidth : lineHeight),
                            (isHorizontal ? lineHeight : layoutSlotWidth)));
                        width += layoutSlotWidth;
                    }
                }
            }
            else
            {
                double width = 0;
                UIElementCollection children = InternalChildren;
                double offset = end - start != 0 ? end - start : 1;
                double itemoffset = lineoffset / offset;
                for (int i = start; i < end; i++)
                {
                    UIElement child = children[i] as UIElement;
                    if (child != null)
                    {
                        Size childSize = new Size(child.DesiredSize.Width, child.DesiredSize.Height);
                        child.Arrange(new Rect(
                            width, height,
                            childSize.Width + itemoffset, lineHeight
                            ));
                        width += childSize.Width + itemoffset;
                    }
                }
            }
        }

        private void arrangeLineWithScale(int lineBegin, int lineEnd, double? directDelta, double directMaximum, double indirectOffset, double indirectGrowth)
        {
            //double width = 0;
            //bool isHorizontal = (Orientation == Orientation.Horizontal);

            //UIElementCollection children = InternalChildren;
            //for (int i = start; i < end; i++)
            //{
            //    UIElement child = children[i] as UIElement;
            //    if (child != null)
            //    {
            //        Size childSize = new Size(child.DesiredSize.Width, child.DesiredSize.Height);
            //        double layoutSlotWidth = (useItemU ? itemU : childSize.Width);
            //        child.Arrange(new Rect(
            //            (isHorizontal ? width : height),
            //            (isHorizontal ? height : width),
            //            (isHorizontal ? layoutSlotWidth : lineHeight),
            //            (isHorizontal ? lineHeight : layoutSlotWidth)));
            //        width += layoutSlotWidth;
            //    }
            //}

            Orientation o = Orientation;
            bool isHorizontal = o == Orientation.Horizontal;
            UIElementCollection childrens = Children;
            double expectedLength = 0.0;
            double itemCount = 0.0;
            double itemLength = isHorizontal ? ItemWidth : ItemHeight;

            if (StretchLastItemInRow && !double.IsNaN(itemLength))
            {
                itemCount = Math.Floor(directMaximum / itemLength);
                expectedLength = itemCount * itemLength;
            }
            else
            {
                itemCount = lineEnd - lineBegin;
                for (int index= lineBegin; index < lineEnd; index++)
                {
                    UIElement element = childrens[index];
                    OrientedSize elementSize = new OrientedSize(o, element.DesiredSize.Width, element.DesiredSize.Height);

                    double directGrowth = directDelta != null ?
                        directDelta.Value : elementSize.Direct;
                    expectedLength += directGrowth;
                }
            }

            // define extra space
            double directExtraSpace = directMaximum - expectedLength;
            double directExtraSpacePart = directExtraSpace / (itemCount + 1.0);
            double directOffset = directExtraSpacePart;

            for (int index = lineBegin; index < lineEnd; index++)
            {
                UIElement element = childrens[index];
                OrientedSize elementSize = new OrientedSize(o, element.DesiredSize.Width, element.DesiredSize.Height);

                double directGrowth = directDelta != null ?
                    directDelta.Value : elementSize.Direct;

                Rect rectangle = isHorizontal ?
                    new Rect(directOffset, indirectOffset, directGrowth, indirectGrowth) :
                    new Rect(indirectOffset, directOffset, indirectGrowth, directGrowth);
                element.Arrange(rectangle);

                directOffset += directGrowth + directExtraSpacePart;
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
