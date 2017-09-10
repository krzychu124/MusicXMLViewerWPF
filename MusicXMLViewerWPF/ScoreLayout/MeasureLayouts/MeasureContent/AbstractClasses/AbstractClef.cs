using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses
{
    abstract class AbstractClef : IVisualHost
    {

        public abstract DrawingVisualHost GetVisualsContainer();
        public abstract double GetVisualWidth();
        public abstract void Update();

    }
}
