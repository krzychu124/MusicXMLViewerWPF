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
    public class DrawingVisualHost : FrameworkElement
    {
        private List<Visual> visuals;
        private ContainerVisual _containerVisual;
        public DrawingVisualHost()
        {
            _containerVisual = new ContainerVisual();
            visuals = new List<Visual>();
            //IsHitTestVisible = true;
            AddVisualChild(_containerVisual);
        }
        
        protected override Visual GetVisualChild(int index)
        {
            return _containerVisual;// visuals[index];
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return _containerVisual == null ? 0:1;//.Children.Count; //visuals.Count;
            }
        }
        public void AddVisual(Visual visual)
        {
            _containerVisual.Children.Add(visual);
            //visuals.Add(visual);
            //AddVisualChild(visual);
        }

        /// <summary>
        /// Removes visual from Visual, VisualChild and LogicalChild collection
        /// </summary>
        /// <param name="visual"></param>
        public void DeleteVisual(Visual visual)
        {
            visuals.Remove(visual);

            RemoveVisualChild(visual);
            //base.RemoveLogicalChild(visual);
        }

        /// <summary>
        /// Clears all Visual Collections
        /// </summary>
        public void ClearVisuals()
        {
            _containerVisual.Children.Clear();
            //int x = VisualChildrenCount;
            //for (int i = 0; i < x; i++)
            //{
            //    DeleteVisual(visuals[0]);
            //}
        }
    }
}
