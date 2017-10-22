using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.Helpers
{
    /// <summary>
    /// Panel behavior similar to Vertical StackPanel but if TopMarginProperty set it adds smth like topmargin to Child Element with this property attached 
    /// </summary>
    public class CustomPartsSegmentPanel : Panel
    {
        public CustomPartsSegmentPanel() : base()
        {
            VerticalAlignment = VerticalAlignment.Top;
        }
        #region Dependency Properties
        public static readonly DependencyProperty TopMarginProperty = DependencyProperty.RegisterAttached(
            "TopMargin",
            typeof(double),
            typeof(CustomPartsSegmentPanel),
            new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static void SetTopMargin(UIElement element, double value)
        {
            element.SetValue(TopMarginProperty, value);
        }

        public static double GetTopMargin(UIElement element)
        {
            return (double)element.GetValue(TopMarginProperty);
        }
        #endregion

        protected override Size MeasureOverride(Size availableSize)
        {
            UIElementCollection childrens = InternalChildren;
            if (childrens == null) { return new Size(); } //! check if no childs
            double w = availableSize.Width;
            double h = 0;
            foreach (UIElement child in childrens)
            {
                child.Measure(availableSize);
                double t = (double)child.GetValue(TopMarginProperty);
                h += child.DesiredSize.Height + t;
            }
            Size calculatedSize = new Size(w, h);
            if (h == double.PositiveInfinity || w == double.PositiveInfinity) //! null return protection, temp solution
            {
                calculatedSize = new Size(0, 0);
            }
            return calculatedSize; // availableSize; //! base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var childrens = InternalChildren;
            if (childrens == null) { return finalSize; }
            int count = childrens.Count;
            double childWidth = 0;
            double childHeight = 0;
            double accumulatedHeight = 0;
            double width = 0;
            double maxWidth = 0;
            for (int i = 0; i < count; i++)
            {
                double topmargin = 0;
                UIElement child = childrens[i];
                if ((double)child.GetValue(TopMarginProperty) != 0)
                {
                    topmargin = (double)child.GetValue(TopMarginProperty);
                }
                childWidth = child.DesiredSize.Width;
                childHeight = child.DesiredSize.Height;
                if (childWidth > maxWidth)
                {
                    maxWidth = childWidth;
                }
                child.Arrange(new Rect(width, accumulatedHeight + topmargin,
                                       childWidth, accumulatedHeight + topmargin + childHeight));
                //TODO_WIP accumulatedHeight += childHeight + topmargin;
            }
            return new Size(maxWidth, accumulatedHeight); //! base.ArrangeOverride(finalSize);
        }
    }
}
