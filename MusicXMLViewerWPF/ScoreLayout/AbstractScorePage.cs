using System.Windows;

namespace MusicXMLScore.ScoreLayout
{
    abstract class AbstractScorePage : IScorePage
    {
        private readonly string id;
        protected AbstractScorePage(string id)
        {
            this.id = id;
        }

        public string Id => id;

        public abstract UIElement GetContent();

        public abstract void UpdateContent();
    }
}
