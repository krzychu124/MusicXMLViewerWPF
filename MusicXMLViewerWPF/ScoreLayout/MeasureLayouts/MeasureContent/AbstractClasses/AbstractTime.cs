using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses
{
    abstract class AbstractTime : IVisual
    {
        private DrawingVisual visual;
        public DrawingVisual GetVisual()
        {
            return visual;
        }

        public abstract double GetVisualWidth();

        public void Update()
        {
            Console.WriteLine("Time Updated");
        }
    }
}
