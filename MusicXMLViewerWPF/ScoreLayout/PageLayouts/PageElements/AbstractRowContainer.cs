using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.ScoreLayout.PageLayouts.PageElements
{
    abstract class AbstractRowContainer :INotifyPropertyChanged
    {
        private double x;
        private double y;
        private double width;
        private double height;
        private readonly Canvas canvas;
        private AbstractRowContainer nextContainer;
        private AbstractRowContainer previousContainer;
        private double max;
        public double Y
        {
            get => y;
            set
            {
                y = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
            }
        }
        public double X
        {
            get => x;
            set
            {
                x = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
            }
        }
        public double Width
        {
            get => width; set
            {
                width = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
            }
        }
        public double Height
        {
            get
            {
                return height;
            } set
            {
                height = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
            }
        }

        private double widthLeft = 0;

        public double WidthWithMargin {
            get
            {
                var margin = Canvas.GetLeft(canvas);
                return width + margin;
            }
        }
        public double HeightWithMargin
        {
            get
            {
                var margin = Canvas.GetTop(canvas);
                return height + margin;
            }
        }

        public Canvas Canvas => canvas;

        internal AbstractRowContainer PreviousContainer { get => previousContainer; set => previousContainer = value; }
        internal AbstractRowContainer NextContainer { get => nextContainer; set => nextContainer = value; }

        private readonly Dictionary<Canvas, IPageElementItem> items;


        protected AbstractRowContainer(Rect bounds)
        {
            canvas = new Canvas();
            PropertyChanged += AbstractRowContainer_PropertyChanged;
            x = bounds.X;
            y = bounds.Y;
            width = bounds.Width;
            height = bounds.Height;
            widthLeft = width;
            max = width;
            canvas.MouseLeftButtonDown += Canvas_MouseDown;
            items = new Dictionary<Canvas, IPageElementItem>();
        }

        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddItem(new MeasurePrototypeItem(100));
        }

        public void AddItem(IPageElementItem item)
        {
            if (item.GetMinWidth() < widthLeft)
            {
                //--------------
                item.GetUIElement().MouseRightButtonDown += RemoveItem_MouseDown; ;

                //--------------
                items.Add(item.GetUIElement() as Canvas, item);
                canvas.Children.Add(item.GetUIElement());
                
                widthLeft -= item.GetMinWidth();
                UpdateWidths();
                item.DrawNumber(items.Count);
            }
            else
            {
                System.Console.WriteLine($"No available space left: {widthLeft} -> item min. width: {item.GetMinWidth()}");
                if (nextContainer != null)
                {
                    nextContainer.AddItem(item);
                } else
                {
                    System.Console.WriteLine("Row container is last on page or next is not set");
                }
            }
        }

        private void RemoveItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Canvas)
            {
                if (items.Count > 1)
                {
                    var canvas = sender as Canvas;
                    //double width = canvas.Width;
                    //widthLeft += width;
                    Canvas.Children.Remove(canvas);
                    items.Remove(canvas);
                    UpdateWidths();
                }
            }
        }

        private void UpdateWidths()
        {
            var rowMinWidth = items.Values.Sum(i => i.GetMinWidth());
            widthLeft = max - rowMinWidth;
            foreach (var item in items.Values)
            {
                var itemWidth = item.GetMinWidth();
                var factor = itemWidth / rowMinWidth * widthLeft;
                item.SetWidth(itemWidth + factor);
            }
            double currentStartX = 0;
            foreach (var item in items.Values)
            {
                Canvas.SetLeft(item.GetUIElement(), currentStartX);
                currentStartX += item.GetWidth();
            }
        }

        public void AbstractRowContainer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(X):
                    Canvas.SetLeft(canvas, X);
                    break;
                case nameof(Y):
                    Canvas.SetTop(canvas, Y);
                    break;
                case nameof(Width):
                    canvas.Width = Width;
                    UpdateWidths();
                    break;
                case nameof(Height):
                    canvas.Height = Height;
                    UpdateWidths();
                    break;
                default:
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}