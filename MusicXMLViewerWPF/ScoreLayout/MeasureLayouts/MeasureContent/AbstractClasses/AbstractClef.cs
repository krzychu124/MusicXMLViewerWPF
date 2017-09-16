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
