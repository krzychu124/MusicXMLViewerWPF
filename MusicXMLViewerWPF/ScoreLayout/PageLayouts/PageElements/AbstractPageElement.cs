using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout.PageLayouts.PageElements
{
    abstract class AbstractPageElement
    {
        private AbstractLayout layoutRoot;
        private double width;
        private double height;
        private double x;
        private double y;
        private Rect bounds;
        private readonly Canvas visualsContainer;
        protected readonly List<IPageElementItem> items;

        public Canvas VisualsContainer { get => visualsContainer; }
        public double Width
        {
            get => width; set
            {
                width = value;
                VisualsContainer.Width = value;
            }
        }
        public double Height
        {
            get => height; set
            {
                height = value;
                VisualsContainer.Height = value;
            }
        }
        public double X
        {
            get => x;
            set
            {
                x = value;
                Canvas.SetLeft(visualsContainer, x);
            }

        }
        public double Y
        {
            get => y; set
            {
                y = value;
                Canvas.SetTop(visualsContainer, y);
            }
        }

        internal AbstractLayout LayoutRoot { get => layoutRoot; set => layoutRoot = value; }

        protected AbstractPageElement(double width, double height)
        {
            this.width = width;
            this.height = height;
            bounds = new Rect(0, 0, width, height);
            visualsContainer = new Canvas();
            items = new List<IPageElementItem>();
        }

        protected AbstractPageElement(Size size)
        {
            this.width = size.Width;
            this.height = size.Height;
            bounds = new Rect(size);
            visualsContainer = new Canvas();
        }

        public abstract void Update();
        public abstract void UpdateDimensions(double width, double height);

        protected void AddVisual(FrameworkElement frameworkElement, double x = 0, double y = 0)
        {
            if (frameworkElement != null)
            {
                Canvas.SetTop(frameworkElement, y);
                Canvas.SetLeft(frameworkElement, x);
                visualsContainer.Children.Add(frameworkElement);
            }
        }
    }
}
