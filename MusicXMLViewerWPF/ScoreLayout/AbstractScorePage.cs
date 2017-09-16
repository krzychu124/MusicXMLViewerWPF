using System.Windows;

namespace MusicXMLScore.ScoreLayout
{
    abstract class AbstractScorePage : IScorePage
    {
        public abstract UIElement GetContent();

        public abstract void UpdateContent();
    }
}
