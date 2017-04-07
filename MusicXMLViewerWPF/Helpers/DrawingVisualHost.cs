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
            AddVisualChild(_containerVisual);
        }
        
        protected override Visual GetVisualChild(int index)
        {
            return _containerVisual;
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return _containerVisual == null ? 0:1;
            }
        }
        public void AddVisual(Visual visual)
        {
            _containerVisual.Children.Add(visual);
        }

        /// <summary>
        /// Removes visual from Visual, VisualChild and LogicalChild collection
        /// </summary>
        /// <param name="visual"></param>
        public void DeleteVisual(Visual visual)
        {
            visuals.Remove(visual);

            RemoveVisualChild(visual);
        }

        /// <summary>
        /// Clears all Visual Collections
        /// </summary>
        public void ClearVisuals()
        {
            _containerVisual.Children.Clear();
        }
    }
}
