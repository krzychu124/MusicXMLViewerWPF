using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.LayoutControl;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

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
            if (AutoLayoutSupported)
            {
                CurrentScoreProperties.AddAttributes();
            }
            //scoreProperties.Init();
        }
        public void RemoveScore(string scoreID)
        {
            if (scoreID == null) return;

            if (scorePropertiesContainer.ContainsKey(scoreID))
            {
                scorePropertiesContainer.Remove(scoreID);
                scoreLayoutContainer.Remove(scoreID);
                SelectScore(scorePropertiesContainer.LastOrDefault().Key);
            }
        }
        public void SelectScore(string scoreID)
        {
            currentLayoutProperties = scoreLayoutContainer["default"];
            currentScoreProperties = scorePropertiesContainer["default"];
            if (scoreID == null)
            {
                return;
            }
            if (scorePropertiesContainer.ContainsKey(scoreID))
            {
                currentLayoutProperties = scoreLayoutContainer[scoreID];
                currentScoreProperties = scorePropertiesContainer[scoreID];
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
            GenerateAttributes();
        }

        private void GenerateAttributes()
        {
            if (partProperties == null) return;

            foreach (var part in partProperties.Values)
            {
                part.GenerateAttributes(timeSignatures);
            }
        }

        public void AddAttributes()
        {
            foreach (var item in partProperties.Values)
            {
                item.AddAttributesToEachSystem();
            }
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
        /// <param name="fractionPosition">Position inside measure (calculated sum of previous items durations</param>
        /// <returns>Closest Clef attributes attached to measureId in selected Part.Id</returns>
        public Model.MeasureItems.Attributes.ClefMusicXML GetClef(string measureId, string partId, int staffNumber, int fractionPosition)
        {
            Model.MeasureItems.Attributes.ClefMusicXML clef = new Model.MeasureItems.Attributes.ClefMusicXML() { Line = "2" };
            //gets all cleff changes of this staff
            ClefChangesDictionary clefsChanges = PartProperties[partId].ClefPerStaff[staffNumber.ToString()];
            // select all previous or same as passed measureId
            List<string> measureKeysList = clefsChanges.Keys.Where(id => int.Parse(id.Replace("X","")) <= int.Parse(measureId.Replace("X", ""))).ToList(); 
            if (measureKeysList.Contains(measureId))
            {
                // check if dictionary of clefchanges has any clef with measure beginning position (fractionPosition == 0)
                var measureClefBeginning = clefsChanges[measureId].ClefsChanges.Where(x => x.Item2 == 0);
                // check if this.[measureId] has clef with fraction position == 0 
                // if not, check if passed Items.fractionPosition is higher than first clef inside clefChanges list of this measureId
                if (measureClefBeginning.Count() == 0 && fractionPosition< clefsChanges[measureId].ClefsChanges.FirstOrDefault().Item2)
                {
                    int x =measureKeysList.Count -1; // 
                    // get last clef change from previous measure
                    clef = clefsChanges[measureKeysList.ElementAt(x-1)].ClefsChanges.LastOrDefault().Item3;
                }
                else
                {
                    //get last clef object with fractionPostion lower or equal than this Item.fracitonPosition
                    clef = clefsChanges[measureId].ClefsChanges.Where(x => x.Item2 <= fractionPosition).LastOrDefault()?.Item3;
                }
            }
            else
            {
                //get previous closest clef 
                clef = clefsChanges[measureKeysList.LastOrDefault()].ClefsChanges.LastOrDefault().Item3;
            }
            return clef?? new Model.MeasureItems.Attributes.ClefMusicXML() { Line = "2" };
        }

        /// <summary>
        /// Gets Key Signature from Measure.Where(measureId, partId)
        /// </summary>
        /// <param name="measureId">MeasureId token, which is Measure.Number</param>
        /// <param name="partId">PartId token, which is Part.Id in list of Part inside ScorePartwiseMusicXML</param>
        /// <returns>Key attributes attached to measureId in selected Part.Id</returns>
        public Model.MeasureItems.Attributes.KeyMusicXML GetKeySignature(string measureId, string partId, int fractionPosition = 0)
        {
            // gets all key changes inside score
            KeyChangesDictionary keyChanges = PartProperties[partId].KeyChanges;
            Model.MeasureItems.Attributes.KeyMusicXML key = null;// = partProperties[partId].KeyAttributes[measureId];

            //select all previous or same as passed measureId
            List<string> measureKeyList = keyChanges.Keys.Where(id => int.Parse(id.Replace("X", "")) <= int.Parse(measureId.Replace("X", ""))).ToList();
            if (measureKeyList.Contains(measureId))
            {
                //check if dictionary of keyChanges has any key with measure beginning position (fractionPosition = 0)
                var measureKeyBeginning = keyChanges[measureId].KeysChanges.Where(x => x.Item2 == 0);
                //check if this.[mesaureId] has key with fractionPosition == 0
                //if, not check if passed Items.fraction.Position is higher than first key inside keyChanges list of this measureId
                if (measureKeyBeginning.Count() == 0 && fractionPosition < keyChanges[measureId].KeysChanges.FirstOrDefault().Item2)
                {
                    int x = measureKeyList.Count - 1;
                    //get last key change from previous measure
                    key = keyChanges[measureKeyList.ElementAt(x - 1)].KeysChanges.LastOrDefault().Item3;
                }
                else
                {
                    //get last key with fractionPosition lower of equal than this Item.fractionPosition
                    key = keyChanges[measureId].KeysChanges.Where(x => x.Item2 <= fractionPosition).LastOrDefault().Item3;
                }
            }
            else
            {
                //get previous closest key
                key = keyChanges[measureKeyList.LastOrDefault()].KeysChanges.LastOrDefault().Item3;
            }
            return key?? new Model.MeasureItems.Attributes.KeyMusicXML();
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
