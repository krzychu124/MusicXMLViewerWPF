using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.LayoutControl;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreProperties
{
    class ScorePropertiesContainer
    {
        private Dictionary<string, ScoreProperties> scorePropertiesContainer;
        private static ScoreProperties currentScoreProperties;

        internal ScoreProperties CurrentScoreProperties
        {
            get
            {
                return currentScoreProperties;
            }
        }

        public ScorePropertiesContainer()
        {
            scorePropertiesContainer = new Dictionary<string, ScoreProperties>();
            scorePropertiesContainer.Add("default", new ScoreProperties(new ScorePartwiseMusicXML()));
            SelectScore("default");
        }
        public void AddScore(ScorePartwiseMusicXML score)
        {
            ScoreProperties scoreProperties = new ScoreProperties(score);
            scorePropertiesContainer.Add(score.ID, scoreProperties);
            SelectScore(score.ID);
        }
        public void RemoveScore(string scoreID)
        {
            if (scorePropertiesContainer.ContainsKey(scoreID))
            {
                scorePropertiesContainer.Remove(scoreID);
                SelectScore(scorePropertiesContainer.LastOrDefault().Key);
            }
        }
        public void SelectScore(string scoreID)
        {
            if (scorePropertiesContainer.ContainsKey(scoreID))
            {
                currentScoreProperties = scorePropertiesContainer[scoreID];
            }
            else
            {
                currentScoreProperties = scorePropertiesContainer["default"];
            }
        }
    }

    class ScoreProperties
    {
        private Dictionary<string, PartProperties> partProperties;
        private LayoutGeneral layout;
        private ScorePartwiseMusicXML score;
        private string id;
        public ScoreProperties(ScorePartwiseMusicXML score)
        {
            this.score = score;
            id = this.score.ID;
            layout = new LayoutGeneral();
            if (score != null)
            {
                layout = new LayoutGeneral(score);
                GetParts(score);
            }
        }
        private void GetParts(ScorePartwiseMusicXML score)
        {
            if (score.Part != null)
            {
                partProperties = new Dictionary<string, PartProperties>();
                foreach (var part in score.Part)
                {
                    partProperties.Add(part.Id, new PartProperties(score, part.Id));
                }
            }
        }
        public Dictionary<string, PartProperties> PartProperties
        {
            get
            {
                return partProperties;
            }
            set
            {
                partProperties = value;
            }
        }

        public LayoutGeneral Layout
        {
            get
            {
                return layout;
            }
            set
            {
                layout = value;
            }
        }

        public ScorePartwiseMusicXML Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }
        public Helpers.PageProperties PageProperties
        {
            get
            {
                return layout.PageProperties;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
        }
    }

}
