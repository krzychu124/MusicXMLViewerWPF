using MusicXMLScore.LayoutControl;
using MusicXMLViewerWPF;
using System.Collections.Generic;
using System.Linq;

namespace MusicXMLScore.ScoreProperties
{
    internal class ScorePropertiesContainer
    {
        private readonly Dictionary<string, ScoreProperties> _scorePropertiesContainer;
        private readonly Dictionary<string, LayoutGeneral> _scoreLayoutContainer;

        internal ScoreProperties CurrentScoreProperties { get; private set; }

        public LayoutGeneral CurrentLayoutProperties { get; private set; }

        public bool AutoLayoutSupported => CurrentScoreProperties.AutoLayoutSupportByScore;

        public ScorePropertiesContainer()
        {
            _scoreLayoutContainer = new Dictionary<string, LayoutGeneral>();
            CurrentLayoutProperties = new LayoutGeneral();
            _scoreLayoutContainer.Add("default", CurrentLayoutProperties);
            _scorePropertiesContainer = new Dictionary<string, ScoreProperties> {{"default", new ScoreProperties(new ScorePartwiseMusicXML())}};
            SelectScore("default");
        }

        public void AddScore(ScorePartwiseMusicXML score)
        {
            //! Layout init before scoreProperties
            LayoutGeneral layout = new LayoutGeneral(score);
            CurrentLayoutProperties = layout;
            _scoreLayoutContainer.Add(score.ID, layout);

            ScoreProperties scoreProperties = new ScoreProperties(score);
            _scorePropertiesContainer.Add(score.ID, scoreProperties);
            SelectScore(score.ID);
            if (AutoLayoutSupported)
            {
                CurrentScoreProperties.AddAttributes();
            }
        }

        public void RemoveScore(string scoreId)
        {
            if (scoreId == null) return;

            if (_scorePropertiesContainer.ContainsKey(scoreId))
            {
                _scorePropertiesContainer.Remove(scoreId);
                _scoreLayoutContainer.Remove(scoreId);
                SelectScore(_scorePropertiesContainer.LastOrDefault().Key);
            }
        }

        public void SelectScore(string scoreId)
        {
            CurrentLayoutProperties = _scoreLayoutContainer["default"];
            CurrentScoreProperties = _scorePropertiesContainer["default"];
            if (scoreId == null)
            {
                return;
            }
            if (_scorePropertiesContainer.ContainsKey(scoreId))
            {
                CurrentLayoutProperties = _scoreLayoutContainer[scoreId];
                CurrentScoreProperties = _scorePropertiesContainer[scoreId];
            }
        }
    }

}