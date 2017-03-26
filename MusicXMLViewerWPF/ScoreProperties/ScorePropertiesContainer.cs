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

        public bool AutoLayoutSupported
        {
            get
            {
                return currentScoreProperties.AutoLayoutSupportByScore;
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
        private ScorePartwiseMusicXML score;
        private TimeSignatures timeSignatures;
        private bool autoLayoutSupportByScore = false;
        private string id;
        public ScoreProperties(ScorePartwiseMusicXML score)
        {
            this.score = score;
            autoLayoutSupportByScore = this.score.LayoutInsideScore;
            id = this.score.ID;
            if (score != null)
            {
                InitScoreProperties();
            }
        }
        public void InitScoreProperties()
        {
            InitParts(this.score);
            InitTimeSignatures(this.score);
        }
        private void InitParts(ScorePartwiseMusicXML score)
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
        private void InitTimeSignatures(ScorePartwiseMusicXML score)
        {
            timeSignatures = new TimeSignatures(score);
        }

        /// <summary>
        /// Gets Clef attributes form Measure.Where(measureId ,partId and staffNumber)
        /// </summary>
        /// <param name="measureId">MeasureId token, which is Measure.Number</param>
        /// <param name="partId">PartId token, which is Part.Id in list of Part inside ScorePartwiseMusicXML</param>
        /// <param name="staffNumber">Staff number (counted from 1 TopDown), if part contains more than one staff line. Default=1</param>
        /// <returns>Clef attributes attached to measureId in selected Part.Id</returns>
        public Model.MeasureItems.Attributes.ClefMusicXML GetClef(string measureId, string partId, int staffNumber = 1)
        {
            Model.MeasureItems.Attributes.ClefMusicXML clef = partProperties[partId].ClefAttributes[measureId].ElementAt(staffNumber - 1);
            return clef; //? new Model.MeasureItems.Attributes.ClefMusicXML();
        }
        public Model.MeasureItems.Attributes.ClefMusicXML GetClef(string measureId, string partId, int staffNumber, int fractionPosition)
        {
            Model.MeasureItems.Attributes.ClefMusicXML clef = new Model.MeasureItems.Attributes.ClefMusicXML() { Line = "2" };
            var clefs = PartProperties[partId].ClefChanges;
            var measureKeys = clefs.Keys.Where(i => int.Parse(i) <= int.Parse(measureId)); // only previous or same as passed measureId
            var zz = clefs.Where(x => measureKeys.Contains(x.Key)).Select((k, l) => new { key = k.Key, value = k.Value }).Select((x, v) => new { x = x.value.ClefsChanges.Select(b => b).Where(b => b.Item1 == staffNumber.ToString()).Where(b=>b!=null), v = x.key }).Where(x=>x != null).ToDictionary(item=>item.v, item=>item.x);
            //var xx = zz.SelectMany(v => v.Value.ClefsChanges);
            if (zz.Keys.LastOrDefault() == measureId )
            {
                var test2 = zz;
                if (zz.LastOrDefault().Value.All(x => x.Item2 > fractionPosition))
                {
                    clef = test2.Reverse().Skip(1).LastOrDefault().Value.LastOrDefault().Item3;
                }
                else
                {
                    //test2 = test2.Select(z => z).Except(zz.Where(z => !z.Value.Any()));
                    //var test =zz.Select(z => z).Except(zz.Where(z => z.Key == measureId).Where(z=>z.Value.Select(x=>x).All(x=>x.Item2>fractionPosition)));
                    clef = test2.LastOrDefault().Value.Where(x => x.Item2 <= fractionPosition).LastOrDefault().Item3;
                }
            }
            else
            {
                var test = zz.Select(z => z).Except(zz.Where(z => !z.Value.Any()));
                clef = test.Select(x => x).LastOrDefault().Value.Where(x=>x!= null).LastOrDefault().Item3;
            }
            string m = measureId;
             //(x=>x.ClefsChanges.Where(x=> x.Item1 == staffNumber.ToString()).Where(x => x.Item2 <= fractionPosition).LastOrDefault().Item3;
            
               // clef = zz.Value.ClefsChanges.Where(x => x.Item1 == staffNumber.ToString()).LastOrDefault().Item3;
            
            //var c = from cl in clefs from cla in cl.Value.ClefsChanges where cla.Item1 == staffNumber.ToString() where int.Parse(cl.Key) <= int.Parse(measureId) select cl ;
            //var zz = measureKeys.SelectMany(x => clefs).Where(x => x.Value.ClefsChanges.Select(z => z.Item1).FirstOrDefault() == staffNumber.ToString());
            return clef;
        }

        /// <summary>
        /// Gets Key Signature from Measure.Where(measureId, partId)
        /// </summary>
        /// <param name="measureId">MeasureId token, which is Measure.Number</param>
        /// <param name="partId">PartId token, which is Part.Id in list of Part inside ScorePartwiseMusicXML</param>
        /// <returns>Key attributes attached to measureId in selected Part.Id</returns>
        public Model.MeasureItems.Attributes.KeyMusicXML GetKeySignature(string measureId, string partId)
        {
            Model.MeasureItems.Attributes.KeyMusicXML key = partProperties[partId].KeyAttributes[measureId];
            return key; //? new Model.MeasureItems.Attributes.KeyMusicXML();
        }

        /// <summary>
        /// Gets Time Signature of selected measureId
        /// </summary>
        /// <param name="measureId">MeasureId token, which is Measure.Number</param>
        /// <returns>Time attributes attached to measureId, PartId is not necessary due to all parts shares the same time signature</returns>
        public Model.MeasureItems.Attributes.TimeMusicXML GetTimeSignature(string measureId)
        {
            Model.MeasureItems.Attributes.TimeMusicXML time = timeSignatures.TimeSignaturesDictionary[measureId];
            return time; //? new Model.MeasureItems.Attributes.TimeMusicXML();
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

        public bool AutoLayoutSupportByScore
        {
            get
            {
                return autoLayoutSupportByScore;
            }

            set
            {
                autoLayoutSupportByScore = value;
            }
        }
    }

}
