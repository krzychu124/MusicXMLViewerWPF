using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    public class CanvasList : Panel
    {
        ToolTip t = new ToolTip();
        //private VisualCollection visuals;
        private List<Visual> visuals = new List<Visual>();
        public List<Visual> Visuals { get { return visuals; } }
        public CanvasList() : base()
        {
        }
        /// <summary>
        /// Sets Width and Height of Canvas
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public CanvasList(double width, double height) : base()
        {
            this.Width = width;
            this.Height = height;
            this.Background = Brushes.Transparent;
            this.MouseEnter += new MouseEventHandler(MyVisualHost_MouseEnter);
            this.MouseLeave += new MouseEventHandler(MyVisualHost_MouseLeave);
        }

        private void MyVisualHost_MouseLeave(object sender, MouseEventArgs e)
        {
            if (t.IsOpen == true)
                t.IsOpen = false;
        }

        private void MyVisualHost_MouseEnter(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition((UIElement)sender);
            EllipseGeometry expandedHitTestArea = new EllipseGeometry(pt, 10.0, 10.0);
            VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(myCallback), new GeometryHitTestParameters(expandedHitTestArea));
        }

        private HitTestResultBehavior myCallback(HitTestResult result) //TODO more hit tests later
        {
            IntersectionDetail intersectionDetail = ((GeometryHitTestResult)result).IntersectionDetail;
            switch (intersectionDetail)
            {
                case IntersectionDetail.NotCalculated:
                    return HitTestResultBehavior.Continue;
                case IntersectionDetail.Empty:
                    return HitTestResultBehavior.Continue;
                case IntersectionDetail.FullyInside:
                    bool stop = OpenToolTip(result);
                    return stop ? HitTestResultBehavior.Stop : HitTestResultBehavior.Continue;
                case IntersectionDetail.FullyContains:
                    bool stop2 = OpenToolTip(result);
                    return stop2 ? HitTestResultBehavior.Stop : HitTestResultBehavior.Continue;
                case IntersectionDetail.Intersects:
                    bool stop3 = OpenToolTip(result);
                    return HitTestResultBehavior.Stop;
                default:
                    return HitTestResultBehavior.Stop;
            }
        }
        private bool OpenToolTip(HitTestResult result)
        {
            if (result.VisualHit.GetType() == typeof(DrawingVisual))
            {
                if (!t.IsOpen)
                {
                    if (t.Content == null)
                    {
                        return false;
                    }
                    t.IsOpen = true;
                    t.PlacementTarget = this;
                }

            }
            return true;
        }
        public CanvasList(Size size) : base()
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= VisualChildrenCount)
                throw new ArgumentOutOfRangeException();
            return visuals[index];
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return visuals.Count;
            }
        }
        /// <summary>
        /// Adds visual to Visual, VisualChild and LogicalChild collection 
        /// </summary>
        /// <param name="visual"></param>
        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
            //Measure(new Size(Width, Height));
        }

        public Visual FindVisualByTag(string tag)
        {
            foreach (var item in Visuals)
            {
                var dvp = item as DrawingVisualPlus;
                if (dvp != null)
                {
                    if (dvp.Tag == tag)
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Removes visual from Visual, VisualChild and LogicalChild collection
        /// </summary>
        /// <param name="visual"></param>
        public void DeleteVisual(Visual visual)
        {
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }

        /// <summary>
        /// Clears all Visual Collections
        /// </summary>
        public void ClearVisuals()
        {
            int x = VisualChildrenCount;
            for (int i = 0; i < x; i++)
            {
                DeleteVisual(visuals[0]);
            }
        }
        public void SetToolTipText(string text)
        {
            ToolTip tip = new ToolTip();
            tip.Content = text;
            tip.InvalidateVisual();
            //this.ToolTip = tip;
            this.t = tip;
            //t.Width = Double.NaN;
            //t.Height = Double.NaN;
        }
        public int Count { get { return visuals.Count; } }
    }
}
