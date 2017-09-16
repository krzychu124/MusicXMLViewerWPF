using System.Windows.Media;

namespace MusicXMLScore.ScoreLayout
{
    interface IVisual
    {
        DrawingVisual GetVisual();
        void Update();
        double GetVisualWidth();
    }
}
