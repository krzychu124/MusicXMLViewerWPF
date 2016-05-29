using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MusicXMLTestViewerWPF
{
    public class Prototype : FrameworkElement
    {
        public FontFamily FontFamily { get; set; }
        public FontWeight FontWeight { get; set; }
        public FontStyle FontStyle { get; set; }
        public int FontSize { get; set; }
        public int Stroke { get; set; }

        public SolidColorBrush Background { get; set; }
        public SolidColorBrush Foreground { get; set; }
        public SolidColorBrush BorderBrush { get; set; }

        private Typeface Typeface;
        private VisualCollection Visuals;
        private Action RenderTextAction;
        private DispatcherOperation CurrentDispatcherOperation;

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                if (String.Equals(text, value, StringComparison.CurrentCulture))
                    return;

                text = value;
                QueueRenderText();
            }
        }

        public Prototype()
        {
            Visuals = new VisualCollection(this);

            FontFamily = new FontFamily("Century");
            FontWeight = FontWeights.Bold;
            FontStyle = FontStyles.Normal;
            FontSize = 20;
            Stroke = 1;
            Typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal);

            Foreground = Brushes.Black;
            BorderBrush = Brushes.Gold;


            RenderTextAction = () => { RenderText(); };
            Loaded += (o, e) => { QueueRenderText(); };

        }

        private void QueueRenderText()
        {
            if (CurrentDispatcherOperation != null)
                CurrentDispatcherOperation.Abort();

            CurrentDispatcherOperation = Dispatcher.BeginInvoke(RenderTextAction, DispatcherPriority.Render, null);

            CurrentDispatcherOperation.Aborted += (o, e) => { CurrentDispatcherOperation = null; };
            CurrentDispatcherOperation.Completed += (o, e) => { CurrentDispatcherOperation = null; };
        }

        private void RenderText()
        {
            Visuals.Clear();

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                LoadCharsToViewPort.DrawString(dc, MusChar.CClef, TypeFaces.NotesFont, Brushes.Black, 0f, 10.0f, 40.0f);

                FormattedText ft = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, FontSize, Foreground);
                Geometry geometry = ft.BuildGeometry(new Point(0.0, 0.0));
                dc.DrawText(ft, new Point(0.0, 0.0));
                dc.DrawGeometry(null, new Pen(BorderBrush, Stroke), geometry);

            }
            
            Visuals.Add(visual);
        }

        protected override Visual GetVisualChild(int index)
        {
            return Visuals[index];
        }

        protected override int VisualChildrenCount
        {
            get { return Visuals.Count; }
        }
        
    }
    //class Prototype : FrameworkElement
    //{
    //    // A collection of all the visuals we are building.
    //    VisualCollection theVisuals;

    //    public Prototype()
    //    {
    //        theVisuals = new VisualCollection(this);
    //        theVisuals.Add(AddCircle());
    //    }

    //    private Visual AddCircle()
    //    {
    //        DrawingVisual drawingVisual = new DrawingVisual();
    //    Typeface NotesFont = new Typeface("FreeSerif");
    //    // Retrieve the DrawingContext in order to create new drawing content.
    //    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
    //        {
    //        drawingContext.DrawText(new FormattedText("test", Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, NotesFont, 2f, Brushes.Black), new Point(20, 40));
    //        // Create a circle and draw it in the DrawingContext.
    //        //Rect rect = new Rect(new Point(160, 100), new Size(320, 80));
    //        drawingContext.DrawEllipse(Brushes.DarkBlue, null, new Point(70, 90), 40, 50);
    //    }
    //        return drawingVisual;
    //    }
    //}  
}
