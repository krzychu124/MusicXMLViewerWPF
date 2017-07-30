using System.Collections.Generic;
using System.Linq;
using MusicXMLScore.DrawingHelpers;
using MusicXMLViewerWPF;

namespace MusicXMLScore.ScoreProperties
{
    class ScoreProperties
    {
        private Dictionary<string, PartProperties> _partProperties;
        private ScorePartwiseMusicXML _score;
        private TimeSignatures _timeSignatures;

        public ScoreProperties(ScorePartwiseMusicXML score)
        {
            _score = score;
            AutoLayoutSupportByScore = _score.LayoutInsideScore;
            Id = _score.ID;
            if (score != null)
            {
                InitScoreProperties();
            }
        }

        private void InitScoreProperties()
        {
            InitParts(_score);
            InitTimeSignatures(_score);
            GenerateAttributes();
        }

        private void GenerateAttributes()
        {
            if (_partProperties == null) return;

            foreach (var part in _partProperties.Values)
            {
                part.GenerateAttributes(_timeSignatures);
            }
        }

        public void AddAttributes()
        {
            foreach (var item in _partProperties.Values)
            {
                item.AddAttributesToEachSystem();
            }
        }

        private void InitParts(ScorePartwiseMusicXML score)
        {
            if (score.Part != null)
            {
                _partProperties = new Dictionary<string, PartProperties>();
                foreach (var part in score.Part)
                {
                    _partProperties.Add(part.Id, new PartProperties(score, part.Id));
                }
            }
        }

        private void InitTimeSignatures(ScorePartwiseMusicXML score)
        {
            _timeSignatures = new TimeSignatures(score);
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
            Model.MeasureItems.Attributes.ClefMusicXML clef; 
            //gets all cleff changes of this staff
            ClefChangesDictionary clefsChanges = PartProperties[partId].ClefPerStaff[staffNumber.ToString()];
            // select all previous or same as passed measureId
            List<string> measureKeysList =
                clefsChanges.Keys.Where(id => int.Parse(id.Replace("X", "")) <= int.Parse(measureId.Replace("X", ""))).ToList();
            if (measureKeysList.Contains(measureId))
            {
                // check if dictionary of clefchanges has any clef with measure beginning position (fractionPosition == 0)
                var measureClefBeginning = clefsChanges[measureId].AttributeChanges.Where(x => x.TimeFraction == 0);
                // check if this.[measureId] has clef with fraction position == 0 
                // if not, check if passed Items.fractionPosition is higher than first clef inside clefChanges list of this measureId
                if (!measureClefBeginning.Any() && fractionPosition < clefsChanges[measureId].AttributeChanges.FirstOrDefault().TimeFraction)
                {
                    int x = measureKeysList.Count - 1; // 
                    // get last clef change from previous measure
                    clef = clefsChanges[measureKeysList.ElementAt(x - 1)].AttributeChanges.LastOrDefault().AttributeEntity;
                }
                else
                {
                    //get last clef object with fractionPostion lower or equal than this Item.fracitonPosition
                    clef = clefsChanges[measureId].AttributeChanges.Where(x => x.TimeFraction <= fractionPosition).LastOrDefault()?.AttributeEntity;
                }
            }
            else
            {
                //get previous closest clef 
                clef = clefsChanges[measureKeysList.LastOrDefault()].AttributeChanges.LastOrDefault().AttributeEntity;
            }
            return clef ?? new Model.MeasureItems.Attributes.ClefMusicXML {Line = "2"};
        }

        /// <summary>
        /// Gets Key Signature from Measure.Where(measureId, partId)
        /// </summary>
        /// <param name="measureId">MeasureId token, which is Measure.Number</param>
        /// <param name="partId">PartId token, which is Part.Id in list of Part inside ScorePartwiseMusicXML</param>
        /// <param name="fractionPosition">Time position in measure</param>
        /// <returns>Key attributes attached to measureId in selected Part.Id</returns>
        public Model.MeasureItems.Attributes.KeyMusicXML GetKeySignature(string measureId, string partId, int fractionPosition = 0)
        {
            // gets all key changes inside score
            KeyChangesDictionary keyChanges = PartProperties[partId].KeyChanges;
            Model.MeasureItems.Attributes.KeyMusicXML key;

            //select all previous or same as passed measureId
            List<string> measureKeyList =
                keyChanges.Keys.Where(id => int.Parse(id.Replace("X", "")) <= int.Parse(measureId.Replace("X", ""))).ToList();
            if (measureKeyList.Contains(measureId))
            {
                //check if dictionary of keyChanges has any key with measure beginning position (fractionPosition = 0)
                var measureKeyBeginning = keyChanges[measureId].AttributeChanges.Where(x => x.TimeFraction == 0);
                //check if this.[mesaureId] has key with fractionPosition == 0
                //if, not check if passed Items.fraction.Position is higher than first key inside keyChanges list of this measureId
                if (!measureKeyBeginning.Any() && fractionPosition < keyChanges[measureId].AttributeChanges.FirstOrDefault().TimeFraction)
                {
                    int x = measureKeyList.Count - 1;
                    //get last key change from previous measure
                    key = keyChanges[measureKeyList.ElementAt(x - 1)].AttributeChanges.LastOrDefault().AttributeEntity;
                }
                else
                {
                    //get last key with fractionPosition lower of equal than this Item.fractionPosition
                    key = keyChanges[measureId].AttributeChanges.Where(x => x.TimeFraction <= fractionPosition).LastOrDefault().AttributeEntity;
                }
            }
            else
            {
                //get previous closest key
                key = keyChanges[measureKeyList.LastOrDefault()].AttributeChanges.LastOrDefault().AttributeEntity;
            }
            return key ?? new Model.MeasureItems.Attributes.KeyMusicXML();
        }

        /// <summary>
        /// Gets Time Signature of selected measureId
        /// </summary>
        /// <param name="measureId">MeasureId token, which is Measure.Number</param>
        /// <returns>Time attributes attached to measureId, PartId is not necessary due to all parts shares the same time signature</returns>
        public Model.MeasureItems.Attributes.TimeMusicXML GetTimeSignature(string measureId)
        {
            Model.MeasureItems.Attributes.TimeMusicXML time = _timeSignatures.TimeSignaturesDictionary[measureId];
            return time; //? new Model.MeasureItems.Attributes.TimeMusicXML();
        }

        public Dictionary<string, PartProperties> PartProperties
        {
            get { return _partProperties; }
            set { _partProperties = value; }
        }

        public ScorePartwiseMusicXML Score
        {
            get { return _score; }
            set { _score = value; }
        }

        public string Id { get;  }

        public bool AutoLayoutSupportByScore { get; set; }
    }
}