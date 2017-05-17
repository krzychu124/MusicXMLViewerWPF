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
        /// <summary>
        /// Adds Visual to Visuals Container Host
        /// </summary>
        /// <param name="visual"></param>
        public void AddVisual(Visual visual)
        {
            _containerVisual.Children.Add(visual);
        }

        /// <summary>
        /// Removes visual from Visuals Container host
        /// </summary>
        /// <param name="visual"></param>
        public void DeleteVisual(Visual visual)
        {
            if (visuals.Contains(visual))
            {
                _containerVisual.Children.Remove(visual);
                visuals.Remove(visual);
            }
            else
            {
                Log.LoggIt.Log("Selected visual not found for removal", Log.LogType.Exception);
            }
        }

        /// <summary>
        /// Clears all Visuals from Visual Container Host
        /// </summary>
        public void ClearVisuals()
        {
            _containerVisual.Children.Clear();
        }

        /// <summary>
        /// Count of visuals inside Visuals Container Host
        /// </summary>
        public int Count { get { return _containerVisual.Children.Count; } }
    }
}
