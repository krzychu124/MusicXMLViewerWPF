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
    class CustomCanvas : Canvas
    {
        private List<Visual> visuals = new List<Visual>();

        protected override Visual GetVisualChild(int index)
        {
            return base.GetVisualChild(index);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return base.VisualChildrenCount;
            }
        }

        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);
            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        public void DeleteVisual(Visual visual)
        {
            visuals.Remove(visual);
            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }

        public void ClearVisuals()
        {
            int x = VisualChildrenCount;
            for (int i = 0; i < x; i++)
            {
                DeleteVisual(visuals[0]);
            }
        }

        public Visual FindVisualByTag(string tag)
        {
            foreach (var item in visuals)
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

        protected override Size MeasureOverride(Size constraint)
        {
            foreach (UIElement item in InternalChildren)
            {
                item.Measure(constraint);
            }
            return new Size();
        }
    }
}
