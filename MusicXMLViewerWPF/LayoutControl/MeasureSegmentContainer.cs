using System.Collections.Generic;
using System.Linq;
using MusicXMLViewerWPF;

namespace MusicXMLScore.LayoutControl
{
    class MeasureSegmentContainer
    {
        Dictionary<string, List<MeasureSegmentController>> _measureSegments;

        public MeasureSegmentContainer()
        {
            MeasureSegments = new Dictionary<string, List<MeasureSegmentController>>();
        }

        public List<MeasureSegmentController> this[string key] => _measureSegments[key];

        /// <summary>
        /// MeasureSegments PartID as Key, List of MeasureSegmentsControllers as Value
        /// </summary>
        internal Dictionary<string, List<MeasureSegmentController>> MeasureSegments
        {
            get { return _measureSegments; }

            set { _measureSegments = value; }
        }

        /// <summary>
        /// Returns list of Part id's
        /// </summary>
        internal List<string> PartIDsList => _measureSegments.Keys.ToList();

        private void InitPartIDs(List<string> partIDsList)
        {
            foreach (var partId in partIDsList)
            {
                _measureSegments.Add(partId, new List<MeasureSegmentController>());
            }
        }

        public void AddNewPartId(string partId)
        {
            if (_measureSegments.ContainsKey(partId))
            {
                Log.LoggIt.Log($"MeasureSegments dictionary contains this part ID: {partId}", Log.LogType.Warning);
            }
            else
            {
                _measureSegments.Add(partId, new List<MeasureSegmentController>());
            }
        }

        public void AddMeasureSegmentController(MeasureSegmentController measureSegment, string partId)
        {
            if (_measureSegments.ContainsKey(partId))
            {
                _measureSegments[partId].Add(measureSegment);
            }
            else
            {
                Log.LoggIt.Log($"Wrong partID while trying to add measureSegmentController to measureSegments Dictionary {partId}",
                    Log.LogType.Warning);
            }
        }

        public void UpdateMeasureWidths()
        {
            var partIDs = PartIDsList;
            foreach (var item in partIDs) //! temp test, updates measures widths
            {
                ViewModel.ViewModelLocator.Instance.Main.PartsProperties[item].SetSystemMeasureRanges();
            }
        }

        internal void GenerateMeasureSegments(ScorePartwiseMusicXML scoreFile, bool allParts = true, List<string>partIDs = null)
        {
            if (!allParts)
            {
                if (partIDs != null)
                {
                    InitPartIDs(partIDs);
                }
                else
                {
                    Log.LoggIt.Log("List of part ID's was empty. Generated Measure Segments for all parts instead");
                    InitPartIDs(scoreFile.Part.Select(x => x.Id).ToList());
                }
            }
            else
            {
                InitPartIDs(scoreFile.Part.Select(x => x.Id).ToList());
            }

            foreach (var part in scoreFile.Part)
            {
                int stavesCount = part.GetStavesCount();
                foreach (var measure in part.Measure)
                {
                    MeasureSegmentController measureSegmentController = new MeasureSegmentController(measure, part.Id, stavesCount);
                    AddMeasureSegmentController(measureSegmentController, part.Id);
                }
            }
        }
    }
}