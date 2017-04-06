using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.Helpers
{
    public class SimpleWrapPanel : Panel
    {
        double panelWidth = 0.0;
        double panelHeight = 0.0;
        protected override Size MeasureOverride(Size availableSize)
        {
            Size panelSize = availableSize;
            panelWidth = 0.0;
            panelHeight = 0.0;
            double height = 0.0;
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(availableSize);
            }
            foreach (UIElement child in InternalChildren)
            {
                if (child.DesiredSize.Width > panelWidth)
                {
                    panelWidth = child.DesiredSize.Width;
                }
                height = child.DesiredSize.Height;
            }
            if (InternalChildren.Count > 1)
            {
                panelWidth = panelWidth * 2 + 20;
                height = height * (((InternalChildren.Count -1) /2) +1);
            }
            panelHeight = height;
            return new Size(panelWidth, height);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            double panelWidth = finalSize.Width;
            double panelHeight = 0;// finalSize.Height;
            Size size = new Size(panelWidth, panelHeight);
            double currentTop = 0.0;
            double currentLeft = 0.0;
            foreach (UIElement child in InternalChildren)
            {
                double currentWidth = child.DesiredSize.Width;
                double currentHeight = child.DesiredSize.Height;
                int index = InternalChildren.IndexOf(child);
                if (index % 2 == 1)
                {
                    child.Arrange(new Rect(currentLeft, currentTop, currentWidth, currentHeight));
                    
                    currentTop += currentHeight + 20;
                    currentLeft = 0;
                }
                else
                {
                    child.Arrange(new Rect(currentLeft, currentTop, currentWidth, currentHeight));
                    currentLeft += currentWidth + 20;
                }
                
            }
            size.Height = this.panelHeight;
            return size;
        }
    }
}
