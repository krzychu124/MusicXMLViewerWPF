using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers
{
    class BeamItemsController
    {
        /// <summary>
        /// Fraction position, [voice, {X,Y}]
        /// </summary>
        private Dictionary<int, double> positionPerFractionTable;
        private List<BeamItem> beams;
        private Dictionary<string, List<BeamSegment>> beamsSegmentsPerVoice;
        private List<DrawingVisualHost> beamsVisuals;

        public List<DrawingVisualHost> BeamsVisuals
        {
            get
            {
                return beamsVisuals;
            }

            set
            {
                beamsVisuals = value;
            }
        }

        public BeamItemsController(List<BeamItem> beams)
        {
            this.beams = beams;
        }

        private void GenerateBeams()
        {
            var voices = beams.Select(x => x.Voice).Distinct().ToList();
            //var fractions = positionPerFractionTable.Keys.Select(x=>x);
            if (voices.Count != 0)
            {
                beamsSegmentsPerVoice = new Dictionary<string, List<BeamSegment>>();
            }
            for (int i = 0; i < voices.Count; i++)
            {
                string voiceId = voices[i];
                
                List<BeamSegment> beamsSegmentsList = new List<BeamSegment>();
                Dictionary<int, BeamItem> fractions = new Dictionary<int, BeamItem>();
                foreach (var item in beams)
                {
                    if (item.Voice == voiceId)
                    {
                        var beam = item.Beams.Where(x => x.Key == 1).Select(x => x.Value).FirstOrDefault();
                        if (beam == Model.Helpers.SimpleTypes.BeamValueMusicXML.begin || beam == Model.Helpers.SimpleTypes.BeamValueMusicXML.@continue)
                        {
                            fractions.Add(item.FractionPosition, item);
                        }
                        if (beam == Model.Helpers.SimpleTypes.BeamValueMusicXML.end)
                        {
                            fractions.Add(item.FractionPosition, item);
                            BeamSegment bs = new BeamSegment(new Dictionary<int, BeamItem>(fractions), voiceId);
                            beamsSegmentsList.Add(bs);
                            fractions.Clear();
                        }
                    }
                }
                if (beamsSegmentsList.Count != 0)
                {
                    beamsSegmentsPerVoice.Add(voiceId, beamsSegmentsList);
                }
            }
        }

        public void Draw(Dictionary<int, double> durationTable)
        {
            
            positionPerFractionTable = durationTable;
            GenerateBeams();
            beamsVisuals = new List<DrawingVisualHost>();
            if (beamsSegmentsPerVoice == null)
            {
                return;
            }
            foreach (var item in beamsSegmentsPerVoice.Values)
            {
                foreach (var segment in item)
                {
                    segment.Draw(durationTable);
                    beamsVisuals.AddRange(segment.BeamVisuals);
                }
            }
        }
    }

    class BeamSegment
    {
        Dictionary<int, BeamItem> beamedNotes;
        string voice;
        List<DrawingVisualHost> beamVisuals = new List<DrawingVisualHost>();
        Dictionary<int, List<BeamItem>> beamsList;
        int maxBeams;
        double slope;
        public List<DrawingVisualHost> BeamVisuals
        {
            get
            {
                return beamVisuals;
            }

            set
            {
                beamVisuals = value;
            }
        }

        public BeamSegment(Dictionary<int, BeamItem> beamedNotesFractions, string voice)
        {
            this.voice = voice;
            beamedNotes = beamedNotesFractions;
            InitSegment();
        }

        private void InitSegment()
        {
            beamsList = new Dictionary<int, List<BeamItem>>();
            maxBeams = beamedNotes.Select(x => x.Value.Beams.Count).Max();
            for (int i = 1; i <= maxBeams; i++)
            {
                List<BeamItem> tempList = beamedNotes.Values.Select(x => x).Where(x => x.Beams.ContainsKey(i)).Select(x=>x).ToList();

                beamsList.Add(i, tempList);
            }
        }

        public void Draw(Dictionary<int, double> positionsTable)
        {
            Dictionary<int, Point> positions = new Dictionary<int, Point>();
            Dictionary<int, Dictionary<string, Point>> positionsAll = new Dictionary<int, Dictionary<string, Point>>();
            CalculateSlopes();
            CorrectStems();
            foreach (var item in beamedNotes)
            {
                int direction = item.Value.Stem.IsDirectionDown() ? -1 : 1;
                double offset = 2.5.TenthsToWPFUnit() * direction;
                Dictionary<string, Point> beamPositions = new Dictionary<string, Point>();
                Point position = new Point(positionsTable[item.Key] + beamedNotes[item.Key].Stem.GetStemEndCalculated().X, beamedNotes[item.Key].Stem.GetStemEndCalculated().Y);
                double tempYOffset = offset;
                for (int i = 1; i <= item.Value.Beams.Count; i++)
                {
                    beamPositions.Add(i.ToString(), new Point(position.X, position.Y + tempYOffset));
                    tempYOffset += 7.5.TenthsToWPFUnit() * direction;
                }
                positionsAll.Add(item.Key, beamPositions);
                positions.Add(item.Key, new Point(positionsTable[item.Key] + beamedNotes[item.Key].Stem.GetStemEndCalculated().X, beamedNotes[item.Key].Stem.GetStemEndCalculated().Y + offset));
            }
            DrawingVisual dv = new DrawingVisual();
            var keys = positions.Keys.ToArray();
            using (DrawingContext dc = dv.RenderOpen())
            {
                for (int i = 1; i < positions.Count; i++)
                {
                    dc.DrawLine(new Pen(beamedNotes.FirstOrDefault().Value.Stem.GetColor(), 5.0.TenthsToWPFUnit()), positions[keys[i - 1]], positions[keys[i]]);
                }
                //for (int i = 1; i < positionsAll.Count; i++)
                //{
                //    int index = beamedNotes[i - 1].FractionPosition;
                //    foreach (var item in positionsAll[index].Values)
                //    {
                //        //if (item.)
                //        dc.DrawLine(new Pen(beamedNotes.FirstOrDefault().Value.Stem.GetColor(), 5.0.TenthsToWPFUnit()), positionsAll[i - 1].ElementAt(keys[i - 1]).Value, positionsAll[i - 1].ElementAt(keys[i]).Value);
                //    }
                //}
            }
            DrawingVisualHost dvh = new DrawingVisualHost();
            dvh.AddVisual(dv);
            beamVisuals.Add(dvh);
        }

        private void CorrectStems()
        {
            List<BeamItem> mainBeam = beamsList[1];
            bool isDown = mainBeam.FirstOrDefault().Stem.IsDirectionDown();
            //higher pitchedPosition - lower note pitch
            if (isDown)
            {
                if (mainBeam.Count == 2) //correct stem lengths using calculated slope(distance difference between stem endPoints
                {
                    if (mainBeam[0].Stem.NoteReference.PitchedPosition[0] > mainBeam[1].Stem.NoteReference.PitchedPosition[0])
                    {
                        double endPointFirstStem = mainBeam[0].Stem.GetStemEndCalculated().Y;
                        mainBeam[1].Stem.SetEndPointY(endPointFirstStem - (slope * 10.0.TenthsToWPFUnit()) +2.5.TenthsToWPFUnit());
                    }
                    else
                    {
                        double endPointLastStem = mainBeam[1].Stem.GetStemEndCalculated().Y;
                        mainBeam[0].Stem.SetEndPointY(endPointLastStem - (slope * 10.0.TenthsToWPFUnit()) + 2.5.TenthsToWPFUnit());
                    }

                }
                else
                {

                }
            }
            else
            { 

            }
        }

        private void CalculateSlopes()
        {
            List<BeamItem> mainBeam = beamsList[1];
            bool isDown = mainBeam.FirstOrDefault().Stem.IsDirectionDown();
            int begin = mainBeam.FirstOrDefault().Stem.NoteReference.PitchedPosition.FirstOrDefault().Value;
            int end = mainBeam.LastOrDefault().Stem.NoteReference.PitchedPosition.FirstOrDefault().Value;
            slope = GetSlope(begin, end);
        }

        private double GetSlope(int begin, int end)
        {
            if (CheckIfLedgerLine(begin))
            {
                return 1;
            }
            if (CheckIfLedgerLine(end))
            {
                return 1;
            }
            double slope = 0.0;
            int difference = Math.Abs(begin - end);
            switch (difference)
            {
                case 0:
                    break;
                case 1:
                    slope = 0.5;
                    break;
                case 2 :
                    slope = 1;
                    break;
                case 3:
                    slope = 1;
                    break;
                case 4:
                    slope = 1;
                    break;
                case 5:
                    slope = 1.5;
                    break;
                case 6:
                    slope = 1.5;
                    break;
                case 7:
                    slope = 1.5;
                    break;
                default:
                    slope = 2;
                    break;
            }
            return slope;
        }

        private bool CheckIfLedgerLine(int pitch)
        {
            if (pitch > 10 || pitch < -2)
            {
                return true;
            }
            return false;
        }
    }
}