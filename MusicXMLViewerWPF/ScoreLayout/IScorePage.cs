using System.Windows;

namespace MusicXMLScore.ScoreLayout
{
    interface IScorePage
    {
        UIElement GetContent();
        void UpdateContent();
    }
}
