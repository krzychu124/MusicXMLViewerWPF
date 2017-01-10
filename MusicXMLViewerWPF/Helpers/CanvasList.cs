using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    class CanvasList : Panel
    {
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
        public int Count { get { return visuals.Count; } }
    }
}
