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
        private Dictionary<string, LayoutGeneral> scoreLayoutContainer;
        private ScoreProperties currentScoreProperties;
        private LayoutGeneral currentLayoutProperties;
        internal ScoreProperties CurrentScoreProperties
        {
            get
            {
                return currentScoreProperties;
            }
        }

        public LayoutGeneral CurrentLayoutProperties
        {
            get
            {
                return currentLayoutProperties;
            }

            set
            {
                currentLayoutProperties = value;
            }
        }

        public ScorePropertiesContainer()
        {
            scoreLayoutContainer = new Dictionary<string, LayoutGeneral>();
            currentLayoutProperties = new LayoutGeneral();
            scoreLayoutContainer.Add("default", currentLayoutProperties);
            scorePropertiesContainer = new Dictionary<string, ScoreProperties>();
            scorePropertiesContainer.Add("default", new ScoreProperties(new ScorePartwiseMusicXML()));
            SelectScore("default");
        }
        public void AddScore(ScorePartwiseMusicXML score)
        { //! Layout init before scoreProperties
            LayoutGeneral layout = new LayoutGeneral(score);
            currentLayoutProperties = layout;
            scoreLayoutContainer.Add(score.ID, layout);

            ScoreProperties scoreProperties = new ScoreProperties(score);
            scorePropertiesContainer.Add(score.ID, scoreProperties);
            SelectScore(score.ID);
            //scoreProperties.Init();
        }
        public void RemoveScore(string scoreID)
        {
            if (scorePropertiesContainer.ContainsKey(scoreID))
            {
                scorePropertiesContainer.Remove(scoreID);
                scoreLayoutContainer.Remove(scoreID);
                SelectScore(scorePropertiesContainer.LastOrDefault().Key);
            }
        }
        public void SelectScore(string scoreID)
        {
            if (scorePropertiesContainer.ContainsKey(scoreID))
            {
                currentLayoutProperties = scoreLayoutContainer[scoreID];
                currentScoreProperties = scorePropertiesContainer[scoreID];
            }
            else
            {
                currentLayoutProperties = scoreLayoutContainer["default"];
                currentScoreProperties = scorePropertiesContainer["default"];
            }
        }
    }

    class ScoreProperties
    {
        private Dictionary<string, PartProperties> partProperties;
        //private LayoutGeneral layout;
        private ScorePartwiseMusicXML score;
        private TimeSignatures timeSignatures;
        private string id;
        public ScoreProperties(ScorePartwiseMusicXML score)
        {
            this.score = score;
            id = this.score.ID;
           // layout = new LayoutGeneral();
            if (score != null)
            {
                Init();//layout = new LayoutGeneral(score);
            }
        }
        public void Init()
        {
            GetParts(this.score);
            GetTimeSignatures(this.score);
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
        private void GetTimeSignatures(ScorePartwiseMusicXML score)
        {
            timeSignatures = new TimeSignatures(score);
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

        //public LayoutGeneral Layout
        //{
        //    get
        //    {
        //        return layout;
        //    }
        //    set
        //    {
        //        layout = value;
        //    }
        //}

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

        public string Id
        {
            get
            {
                return id;
            }
        }

        public TimeSignatures TimeSignatures
        {
            get
            {
                return timeSignatures;
            }

            set
            {
                timeSignatures = value;
            }
        }
    }

}
