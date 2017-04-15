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
        int maxBeams; //max beams of all beamed notes
        double slope; // not real slope...  ==> interval in staffspaces between first and last beamed note
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
            }
            DrawingVisualHost dvh = new DrawingVisualHost();
            dvh.AddVisual(dv);
            beamVisuals.Add(dvh);
        }

        private void CorrectStems()
        {
            List<BeamItem> mainBeam = beamsList[1];
            bool isDown = mainBeam.FirstOrDefault().Stem.IsDirectionDown();
            //! higher pitchedPosition ==> lower note pitch

            //
            var firstNoteBeamPitch = mainBeam.Where(x => x.Beams[1] == Model.Helpers.SimpleTypes.BeamValueMusicXML.begin).FirstOrDefault().Stem.NoteReference.PitchedPosition.FirstOrDefault().Value;
            var lastNoteBeamPitch = mainBeam.Where(x => x.Beams[1] == Model.Helpers.SimpleTypes.BeamValueMusicXML.end).FirstOrDefault().Stem.NoteReference.PitchedPosition.FirstOrDefault().Value;
            if (isDown)
            {
                if (mainBeam.Count == 2) //? correct stem lengths using calculated slope(distance difference between stem endPoints
                {
                    if (firstNoteBeamPitch == lastNoteBeamPitch)
                    {
                        return;
                    }
                    if (firstNoteBeamPitch > lastNoteBeamPitch)
                    {
                        double endPointFirstStem = mainBeam[0].Stem.GetStemEndCalculated().Y;
                        mainBeam[1].Stem.SetEndPointY(endPointFirstStem - (slope * 10.0.TenthsToWPFUnit()) /*? +2.5.TenthsToWPFUnit()*/); //add some to stem length to compensate beams
                    }
                    else
                    {
                        double endPointLastStem = mainBeam[1].Stem.GetStemEndCalculated().Y;
                        mainBeam[0].Stem.SetEndPointY(endPointLastStem - (slope * 10.0.TenthsToWPFUnit()) /*? + 2.5.TenthsToWPFUnit()*/);
                    }

                }
                else
                {
                    if (firstNoteBeamPitch == lastNoteBeamPitch) //? 1st == last means beam is horizontal
                    {
                        int maxPitch = mainBeam.Max(x => x.Stem.NoteReference.PitchedPosition.FirstOrDefault().Value);
                        double shortestStem = mainBeam.Select(x => x.Stem).Max(x => x.GetStemEndCalculated().Y);
                        foreach (var item in mainBeam)
                        {
                            item.Stem.SetEndPointY(shortestStem);
                        }
                    }
                    if (firstNoteBeamPitch > lastNoteBeamPitch)
                    {
                        AdvancedStemCorrection(isDown, true);
                    }
                    if (firstNoteBeamPitch < lastNoteBeamPitch)
                    {
                        AdvancedStemCorrection(isDown, false);
                    }
                }
            }
            else //! If stem is Up
            {
                if (mainBeam.Count == 2) //? correct stem lengths using calculated slope(distance difference between stem endPoints
                {
                    if (firstNoteBeamPitch == lastNoteBeamPitch)
                    {
                        return;
                    }
                    if (firstNoteBeamPitch > lastNoteBeamPitch)
                    {
                        double endPointFirstStem = mainBeam[0].Stem.GetStemEndCalculated().Y;
                        mainBeam[1].Stem.SetEndPointY(endPointFirstStem - (slope * 10.0.TenthsToWPFUnit()) + 2.5.TenthsToWPFUnit());
                    }
                    else
                    {
                        double endPointLastStem = mainBeam[1].Stem.GetStemEndCalculated().Y;
                        mainBeam[0].Stem.SetEndPointY(endPointLastStem - (slope * 10.0.TenthsToWPFUnit()) + 2.5.TenthsToWPFUnit());
                    }

                }
                else //! if more than 2 beamed notes
                {
                    if (firstNoteBeamPitch == lastNoteBeamPitch) //? 1st == last => beam is horizontal
                    {
                        int maxPitch = mainBeam.Min(x => x.Stem.NoteReference.PitchedPosition.FirstOrDefault().Value);
                        double shortestStem = mainBeam.Select(x => x.Stem).Min(x => x.GetStemEndCalculated().Y);
                        foreach (var item in mainBeam)
                        {
                            item.Stem.SetEndPointY(shortestStem);
                        }
                    }
                    if (firstNoteBeamPitch > lastNoteBeamPitch)
                    {
                        AdvancedStemCorrection(isDown, true);
                    }
                    if (firstNoteBeamPitch < lastNoteBeamPitch)
                    {
                        AdvancedStemCorrection(isDown, false);
                    }
                }
            }
        }

        private void AdvancedStemCorrection(bool isStemDownwardsDirection, bool slopeUpward)
        {
            List<BeamItem> mainBeamsList = beamsList[1];
            List<BeamItem> midBeamsList = mainBeamsList.Skip(1).Take(mainBeamsList.Count - 2).ToList();
            var pitchesList = GetPitchesFromBeamList(mainBeamsList);
            var iterationsPerPitch = PitchesListConvertToCounts(pitchesList);
            bool horizontalBeam = false;
            var test = iterationsPerPitch.GroupBy(group => group.Value).Where(group => group.Count() > 1).Where(x=>x.Any(z=>z.Value != 1)).ToList();
            //if found any pitch iterations set beam to hotizontal/ no slope
            if (iterationsPerPitch.Any(x=>x.Value > 2) || test.Count !=0 )
            {
                horizontalBeam = true;
                if (isStemDownwardsDirection)
                {
                    var lowestStemEnd = mainBeamsList.Select(x => x).Where(x => x.Stem.NoteReference.PitchedPosition.Values.FirstOrDefault() == pitchesList.Max(y => y.Value)).FirstOrDefault().Stem.GetStemEndCalculated().Y;
                    foreach (var item in mainBeamsList)
                    {
                        item.Stem.SetEndPointY(lowestStemEnd);
                    }
                }
                else
                {
                    var highestStemEnd = mainBeamsList.Select(x => x).Where(x => x.Stem.NoteReference.PitchedPosition.Values.FirstOrDefault() == pitchesList.Min(y => y.Value)).FirstOrDefault().Stem.GetStemEndCalculated().Y;
                    foreach (var item in mainBeamsList)
                    {
                        item.Stem.SetEndPointY(highestStemEnd);
                    }
                }
            }
            //if stem lengths set in horizonlal section, just skip other calculations
            if (!horizontalBeam)
            {
                if (isStemDownwardsDirection)
                {
                    //find lowest pitch, set sortest stem then recalculate other stems according to calculated slope
                    if (slopeUpward)
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    //find highest pitch, set sortest stem then recalculate other stems according to calculated slope
                    if (slopeUpward)
                    {
                        // var 
                    }
                    else
                    {

                    }
                }
            }
        }

        public Dictionary<int, int> GetPitchesFromBeamList(List<BeamItem> beamItemList)
        {
            Dictionary<int, int> pitches = beamItemList.Select((x, y) => new { fraction = x.FractionPosition, pitch = x.Stem.NoteReference.PitchedPosition.FirstOrDefault().Value }).ToDictionary(item => item.fraction, item => item.pitch);
            return pitches;
        }

        public Dictionary<int, int> PitchesListConvertToCounts(Dictionary<int, int> pitches)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            foreach (var item in pitches)
            {
                if (result.ContainsKey(item.Value))
                {
                    result[item.Value] += 1;
                }
                else
                {
                    result.Add(item.Value, 1);
                }
            }
            return result;
        }
        private void CalculateSlopes()
        {
            List<BeamItem> mainBeam = beamsList[1];
            bool isDown = mainBeam.FirstOrDefault().Stem.IsDirectionDown();
            int begin = mainBeam.FirstOrDefault().Stem.NoteReference.PitchedPosition.FirstOrDefault().Value;
            int end = mainBeam.LastOrDefault().Stem.NoteReference.PitchedPosition.FirstOrDefault().Value;
            slope = GetSlope(begin, end);
        }

        /// <summary>
        /// Hard-coded slope based on begin and end beamed notes interval
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check if passed note pitch has ledger line/s
        /// </summary>
        /// <param name="pitch"></param>
        /// <returns>True if pitch has ledger line</returns>
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