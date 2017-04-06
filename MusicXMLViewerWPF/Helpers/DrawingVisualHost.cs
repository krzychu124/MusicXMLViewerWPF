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
    public class DrawingVisualHost : UIElement
    {
        ToolTip t = new ToolTip();
        private List<Visual> visuals;
        public List<Visual> Visuals { get { return visuals; } }
        Point pt = new Point();

        public DrawingVisualHost()
        {
            visuals = new List<Visual>();
            IsHitTestVisible = false;
        }

        public DrawingVisualHost(double width, double height)
        {
            visuals= new List<Visual>();
            IsHitTestVisible = false;
            //this.MouseEnter += new MouseEventHandler(MyVisualHost_MouseEnter);
            //this.MouseLeave += new MouseEventHandler(MyVisualHost_MouseLeave);
        }

        private void MyVisualHost_MouseLeave(object sender, MouseEventArgs e)
        {
            if (t.IsOpen == true)
                t.IsOpen = false;
        }

        private void MyVisualHost_MouseEnter(object sender, MouseEventArgs e)
        {
            pt = e.GetPosition((UIElement)sender);
            EllipseGeometry expandedHitTestArea = new EllipseGeometry(pt, 1.0, 1.0);
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
            }
            return HitTestResultBehavior.Stop;
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
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= VisualChildrenCount)
            {
                throw new ArgumentOutOfRangeException();
            }
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
            //base.AddLogicalChild(visual);
        }

        /// <summary>
        /// Removes visual from Visual, VisualChild and LogicalChild collection
        /// </summary>
        /// <param name="visual"></param>
        public void DeleteVisual(Visual visual)
        {
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            //base.RemoveLogicalChild(visual);
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
    }
}
