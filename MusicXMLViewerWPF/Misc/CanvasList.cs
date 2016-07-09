using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLViewerWPF
{
    class CanvasList : Grid
    { 
        private List<Visual> visuals = new List<Visual>();

        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return visuals.Count;
            }
        }
        public string Count_()
        {
           return visuals.Count.ToString();
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
            for( int i = 0; i<x; i++)
            {
                DeleteVisual(visuals[0]);
            }
            Logger.Log($"Deleted {x} objects from drawing window");
        }
    }
}
