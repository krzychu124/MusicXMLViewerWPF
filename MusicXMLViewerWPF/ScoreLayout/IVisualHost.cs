using MusicXMLScore.Helpers;

namespace MusicXMLScore.ScoreLayout
{
    interface IVisualHost
    {
        DrawingVisualHost GetVisualsContainer();
        double GetVisualWidth();
        
    }
}
