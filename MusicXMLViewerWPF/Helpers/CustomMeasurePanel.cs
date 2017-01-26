using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.Helpers
{
    public class CustomMeasurePanel : Panel
    {
        public static readonly DependencyProperty StaticPositionProperty = DependencyProperty.RegisterAttached(
            "StaticPosition",
            typeof(bool),
            typeof(CustomMeasurePanel),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        protected override Size MeasureOverride(Size availableSize)
        {
            Size childsConstraint = new Size() { Width = availableSize.Width, Height = availableSize.Height };
            Size panelSize = new Size(availableSize.Width, 0);
            Size lineSize = new Size();
            int lineStart = 0;
            int lineEnd = 0;

            UIElementCollection childrens = InternalChildren;
            int count = childrens.Count;
            for (int i = 0; i < count; i++)
            {
                UIElement child = childrens[i];
                if (child == null) continue;

                child.Measure(availableSize);

                bool staticPos = false;
                var staticPosition = (bool)child.GetValue(CustomMeasurePanel.StaticPositionProperty);
                if (staticPosition == true)
                {
                    staticPos = true;
                }
                Size childSize = child.DesiredSize;

                if (ComparisonHelpers.GreaterThan(lineSize.Width + childSize.Width, childsConstraint.Width) || staticPos == true)
                {
                    panelSize.Height += lineSize.Height;
                    double offset = panelSize.Width - lineSize.Width;
                    MeasureAgain(panelSize, lineSize, lineStart, lineEnd);
                    lineSize = childSize;
                    lineStart = i;
                }
                else
                {
                    lineSize.Width += childSize.Width;
                    lineSize.Height = Math.Max(childSize.Height, lineSize.Height);
                    lineEnd = i;
                }
            }
            if (lineSize.Width < panelSize.Width)
            {
                MeasureAgain(panelSize, lineSize, lineStart, lineEnd);
                panelSize.Height += lineSize.Height;
            }
            panelSize.Height += lineSize.Height;
            return new Size(0, 0); //! panelSize;
        }

        private void MeasureAgain(Size paneSize, Size lineSize, int lineStart, int lineEnd)
        {
            double offset = paneSize.Width - lineSize.Width;
            double divisor = lineEnd - lineStart != 0 ? lineEnd - lineStart : 0;
            double childoffset = offset / ++divisor;
            for (int i = lineStart; i < lineEnd; i++)
            {
                UIElement child = InternalChildren[i];
                if (child == null) continue;
                Size childSize = child.DesiredSize;
                //child.Measure(new Size());
                double calculated = childSize.Width + offset;
                child.Measure(new Size(calculated, childSize.Height));
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // return base.ArrangeOverride(finalSize);
            int lineStart = 0;
            Size currentLineSize = new Size();
            Size maxLineSize = new Size(finalSize.Width, finalSize.Height);
            double accumulatedheight = 0;
            int lineEnd = 0;
            UIElementCollection children = InternalChildren;
            int count = children.Count;
            for (int i = 0; i < count; i++)
            {
                lineEnd = i;
                UIElement child = children[i] as UIElement;
                if (child == null) continue;
                bool statposition = false;
                object staticposition = child.GetValue(CustomMeasurePanel.StaticPositionProperty);
                if (staticposition != null)
                {
                    statposition = (bool)staticposition;
                }
                currentLineSize.Height = child.DesiredSize.Height;
                if (ComparisonHelpers.GreaterThan(currentLineSize.Width + child.DesiredSize.Width, maxLineSize.Width))
                {
                    double offset = maxLineSize.Width - currentLineSize.Width;
                    ArrangeLine(accumulatedheight, child.DesiredSize.Height, lineStart, lineEnd, false, 0.0, true, offset);
                    accumulatedheight += child.DesiredSize.Height;
                    currentLineSize.Width = child.DesiredSize.Width;
                    lineStart = lineEnd;
                }
                else
                {
                    currentLineSize.Width += child.DesiredSize.Width;
                }
            }
            if (lineStart < count)
            {
                ArrangeLine(accumulatedheight, currentLineSize.Height, lineStart, count, false, 0.0, true, maxLineSize.Width - currentLineSize.Width);
            }
            return new Size(maxLineSize.Width, accumulatedheight);
        }

        private void ArrangeLine(double height, double lineHeight, int start, int end, bool useItemU, double itemU, bool scale = false, double lineOffset = 0)
        {
            //TODO_I needs imrpvements if items count with staticposition is higher than width of panel
            double width = 0;
            int tempend = end;
            UIElementCollection children = InternalChildren;
            foreach (UIElement item in children)
            {
                if ((bool)item.GetValue(StaticPositionProperty))
                {
                    tempend -= 1;
                    //lineOffset -= item.DesiredSize.Width;
                }
            }
            double offset = tempend - start != 0 ? tempend - start : 1;
            double itemOffset = lineOffset / offset;
            for (int i = start; i < end; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child != null)
                {
                    double tempItemOffset = itemOffset;
                    Size childSize = new Size(child.DesiredSize.Width, child.DesiredSize.Height);
                    if ((bool)child.GetValue(StaticPositionProperty))
                    {
                        tempItemOffset = 0;
                    }
                    child.Arrange(new Rect(
                        width, height,
                        childSize.Width + tempItemOffset, lineHeight));
                    width += childSize.Width + tempItemOffset;
                }
            }
        }
    }
}
