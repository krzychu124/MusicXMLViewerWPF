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
                foreach (var beamSegment in item)
                {
                    beamSegment.Draw(positionPerFractionTable);
                    beamsVisuals.AddRange(beamSegment.BeamVisuals);
                }
            }
        }
    }

    class BeamSegment
    {
        Dictionary<int, BeamItem> beamedStems;
        string voice;
        List<DrawingVisualHost> beamVisuals = new List<DrawingVisualHost>();
        Dictionary<int, List<BeamItem>> beamsList;
        int maxBeams; //! max beams of all beamed notes
        double slope; //! not real slope...  ==> interval in staffspaces between first and last beamed note
        double slopeValue; //! real slope... :)
        bool isSlopeUpward; //! slope direction
        Dictionary<int, double> positionsTable;
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
            beamedStems = beamedNotesFractions;
            InitBeamSegment();
        }

        private void InitBeamSegment()
        {
            beamsList = new Dictionary<int, List<BeamItem>>();
            maxBeams = beamedStems.Select(x => x.Value.Beams.Count).Max();
            for (int i = 1; i <= maxBeams; i++)
            {
                List<BeamItem> tempList = beamedStems.Values.Select(x => x).Where(x => x.Beams.ContainsKey(i)).Select(x=>x).ToList();
                beamsList.Add(i, tempList);
            }
        }

        public void Draw(Dictionary<int, double> positionsTable)
        {
            Dictionary<int, Point> stemPositions = new Dictionary<int, Point>();
            CalculateSlopes(positionsTable);
            CorrectStems();
            foreach (var item in beamedStems)
            {
                int direction = item.Value.Stem.IsDirectionDown() ? -1 : 1;
                double offset = 2.5.TenthsToWPFUnit() * direction;
                Dictionary<string, Point> beamPositions = new Dictionary<string, Point>();
                Point position = new Point(positionsTable[item.Key] + beamedStems[item.Key].Stem.GetStemEndCalculated().X, beamedStems[item.Key].Stem.GetStemEndCalculated().Y);
                double tempYOffset = offset;
                for (int i = 1; i <= item.Value.Beams.Count; i++)
                {
                    beamPositions.Add(i.ToString(), new Point(position.X, position.Y + tempYOffset));
                    tempYOffset += 7.5.TenthsToWPFUnit() * direction;
                }
                stemPositions.Add(item.Key, new Point(positionsTable[item.Key] + beamedStems[item.Key].Stem.GetStemEndCalculated().X, beamedStems[item.Key].Stem.GetStemEndCalculated().Y + offset));
            }
            DrawingVisual beamsDrawingVisual = new DrawingVisual();
            var fractionKeys = stemPositions.Keys.ToArray();
            int dir = beamedStems.FirstOrDefault().Value.Stem.IsDirectionDown() ? -1 : 1;
            using (DrawingContext dc = beamsDrawingVisual.RenderOpen())
            {
                for (int i = 1; i < stemPositions.Count; i++)
                {
                    //! skip counting beam hooks
                    int beamsCount = beamedStems[fractionKeys[i - 1]].Beams.Where(
                        x=>x.Value != Model.Helpers.SimpleTypes.BeamValueMusicXML.forwardhook ||
                        x.Value != Model.Helpers.SimpleTypes.BeamValueMusicXML.backwardhook).Count();

                    for (int j = 0; j < beamsCount; j++)
                    {
                        if (beamedStems[fractionKeys[i]].Beams.ContainsKey(j+1))//? necessary to skip beam drawing if current beam is end
                        {
                            dc.DrawLine(new Pen(beamedStems.FirstOrDefault().Value.Stem.GetColor(), 5.0.TenthsToWPFUnit()), new Point(stemPositions[fractionKeys[i - 1]].X, stemPositions[fractionKeys[i - 1]].Y + j * (7.5.TenthsToWPFUnit() * dir)), new Point(stemPositions[fractionKeys[i]].X, stemPositions[fractionKeys[i]].Y + j * (7.5.TenthsToWPFUnit() * dir)));
                        }
                        if (beamedStems[fractionKeys[i]].Beams.ContainsValue(Model.Helpers.SimpleTypes.BeamValueMusicXML.backwardhook) && j +1 == beamsCount)
                        {
                            DrawBeamHooks(dc, i, j, stemPositions, Model.Helpers.SimpleTypes.BeamValueMusicXML.backwardhook);
                        }
                        if (beamedStems[fractionKeys[i-1]].Beams.ContainsValue(Model.Helpers.SimpleTypes.BeamValueMusicXML.forwardhook) && j+1 == beamsCount)
                        {
                            //? j-1 temp, need tests (without decrementing hook is drawn with one beamSpacing lower
                            DrawBeamHooks(dc, i, j-1, stemPositions, Model.Helpers.SimpleTypes.BeamValueMusicXML.forwardhook);
                        }
                    }
                    
                }
            }
            DrawingVisualHost drawingVisualHost = new DrawingVisualHost();
            drawingVisualHost.AddVisual(beamsDrawingVisual);
            beamVisuals.Add(drawingVisualHost);
        }

        private void DrawBeamHooks(DrawingContext dc, int i, int j, Dictionary<int,Point> positions, Model.Helpers.SimpleTypes.BeamValueMusicXML hookType)
        {
            int isForward = hookType == Model.Helpers.SimpleTypes.BeamValueMusicXML.forwardhook ? 1 : 0; // forwardHook need previous stem position
            int dir = beamedStems.FirstOrDefault().Value.Stem.IsDirectionDown() ? -1 : 1;
            int slopeDir = isSlopeUpward ? -1 : 1;
            var keys = positions.Keys.ToArray();
            int hookCount = beamedStems[keys[i - isForward]].Beams.Select(x => x.Value).Where(x => x == hookType).Count();
            double hookLength = hookType == Model.Helpers.SimpleTypes.BeamValueMusicXML.forwardhook ? 10.0.TenthsToWPFUnit() : -10.0.TenthsToWPFUnit(); 
            for (int h = 1; h <= hookCount; h++)
            {
                Point stemEndPoint = new Point(positions[keys[i - isForward]].X, positions[keys[i- isForward]].Y + (j + h) * (7.5.TenthsToWPFUnit() * dir));
                double hookX = stemEndPoint.X + hookLength;
                double hookY = FindStemEndY(slopeDir * slopeValue, stemEndPoint, hookX);
                dc.DrawLine(new Pen(beamedStems.FirstOrDefault().Value.Stem.GetColor(), 5.0.TenthsToWPFUnit()), new Point(hookX, hookY), stemEndPoint);
            }
        }

        private void CorrectStems()
        {
            List<BeamItem> mainBeam = beamsList[1];
            bool isStemDirestionDown = mainBeam.FirstOrDefault().Stem.IsDirectionDown();
            //! higher pitchedPosition ==> lower note pitch
            Point p = new Point(0, 22);
            Point p1 = new Point(10, slope * 10.0.TenthsToWPFUnit());
            double test = FindStemEndY(slopeValue, p1, p.X);
            var firstNoteBeamPitch = mainBeam.Where(x => x.Beams[1] == Model.Helpers.SimpleTypes.BeamValueMusicXML.begin).FirstOrDefault().Stem.NoteReference.PitchedPosition.FirstOrDefault().Value;
            var lastNoteBeamPitch = mainBeam.Where(x => x.Beams[1] == Model.Helpers.SimpleTypes.BeamValueMusicXML.end).FirstOrDefault().Stem.NoteReference.PitchedPosition.FirstOrDefault().Value;

            if (isStemDirestionDown)
            {
                //? correct stem lengths using calculated slope(distance difference between stem endPoints
                if (mainBeam.Count == 2) 
                {
                    if (firstNoteBeamPitch == lastNoteBeamPitch)
                    {
                        return;
                    }
                    if (firstNoteBeamPitch > lastNoteBeamPitch)
                    {
                        isSlopeUpward = true;
                        double endPointFirstStem = mainBeam[0].Stem.GetStemEndCalculated().Y;
                        mainBeam[1].Stem.SetEndPointY(endPointFirstStem - (slope * 10.0.TenthsToWPFUnit()) +2.5.TenthsToWPFUnit()); //add some to stem length to compensate beams
                    }
                    else
                    {
                        double endPointLastStem = mainBeam[1].Stem.GetStemEndCalculated().Y;
                        mainBeam[0].Stem.SetEndPointY(endPointLastStem - (slope * 10.0.TenthsToWPFUnit()) +2.5.TenthsToWPFUnit());
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
                        AdvancedStemCorrection(isStemDirestionDown, true);
                    }
                    if (firstNoteBeamPitch < lastNoteBeamPitch)
                    {
                        AdvancedStemCorrection(isStemDirestionDown, false);
                    }
                }
            }
            else //! If stem is Up
            {
                //? correct stem lengths using calculated slope(distance difference between stem endPoints
                if (mainBeam.Count == 2) 
                {
                    if (firstNoteBeamPitch == lastNoteBeamPitch)
                    {
                        return;
                    }
                    if (firstNoteBeamPitch > lastNoteBeamPitch)
                    {
                        isSlopeUpward = true;
                        double endPointFirstStem = mainBeam[1].Stem.GetStemEndCalculated().Y;
                        mainBeam[0].Stem.SetEndPointY(endPointFirstStem + (slope * 10.0.TenthsToWPFUnit()) + 2.5.TenthsToWPFUnit());
                    }
                    else
                    {
                        double endPointLastStem = mainBeam[0].Stem.GetStemEndCalculated().Y;
                        mainBeam[1].Stem.SetEndPointY(endPointLastStem +(slope * 10.0.TenthsToWPFUnit()) + 2.5.TenthsToWPFUnit());
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
                        AdvancedStemCorrection(isStemDirestionDown, true);
                    }
                    if (firstNoteBeamPitch < lastNoteBeamPitch)
                    {
                        AdvancedStemCorrection(isStemDirestionDown, false);
                    }
                }
            }
        }

        private void AdvancedStemCorrection(bool isStemDownwardsDirection, bool slopeUpward)
        {
            List<BeamItem> mainBeamsList = beamsList[1];
            //List<BeamItem> midBeamsList = mainBeamsList.Skip(1).Take(mainBeamsList.Count - 2).ToList();
            isSlopeUpward = slopeUpward;
            var pitchesList = GetPitchesFromBeamList(mainBeamsList);
            var iterationsPerPitch = PitchesListConvertToCounts(pitchesList);
            bool horizontalBeam = false;
            var pitchSequenceRepeats = iterationsPerPitch.GroupBy(group => group.Value).Where(group => group.Count() > 1).Where(x=>x.Any(z=>z.Value != 1)).ToList();
            
            //! if found any pitch iterations set beam to hotizontal/ no slope
            if (iterationsPerPitch.Any(x=>x.Value > 2) || pitchSequenceRepeats.Count !=0 )
            {
                horizontalBeam = true;
                if (isStemDownwardsDirection)
                {
                    var lowestStemEnd = mainBeamsList.Select(x => x).Where(x => x.Stem.NoteReference.PitchedPosition.Values.FirstOrDefault() == pitchesList.Max(y => y.Value)).FirstOrDefault().Stem.GetStemEndCalculated().Y;
                    foreach (var item in mainBeamsList)
                    {
                        //! update stem
                        item.Stem.SetEndPointY(lowestStemEnd);
                    }
                }
                else
                {
                    var highestStemEnd = mainBeamsList.Select(x => x).Where(x => x.Stem.NoteReference.PitchedPosition.Values.FirstOrDefault() == pitchesList.Min(y => y.Value)).FirstOrDefault().Stem.GetStemEndCalculated().Y;
                    foreach (var item in mainBeamsList)
                    {
                        //! update stem
                        item.Stem.SetEndPointY(highestStemEnd);
                    }
                }
            }
            //! if stem lengths set in horizonlal section, just skip other calculations
            if (!horizontalBeam)
            {
                if (isStemDownwardsDirection)
                {
                    //! find lowest pitch, set sortest stem then recalculate other stems according to calculated slope
                    var highestNote = mainBeamsList.Select(x => x).Where(x => x.Stem.NoteReference.PitchedPosition.Values.FirstOrDefault() == pitchesList.Max(y => y.Value)).FirstOrDefault();
                    double highestStemEnd = highestNote.Stem.GetStemEndCalculated().Y;
                    double highestStemEndX = positionsTable[highestNote.FractionPosition];
                    if (slopeUpward)
                    {
                        foreach (var item in mainBeamsList)
                        {
                            double calculatedStemEndX = positionsTable[item.FractionPosition] - highestStemEndX;
                            double stemEndY = FindStemEndY(-slopeValue, new Point(0, highestStemEnd), calculatedStemEndX);
                            //! update stem
                            item.Stem.SetEndPointY(stemEndY);
                        }
                    }
                    else
                    {
                        foreach (var item in mainBeamsList)
                        {
                            double calculatedStemEndX = positionsTable[item.FractionPosition] - highestStemEndX;
                            double stemEndY = FindStemEndY(slopeValue, new Point(0, highestStemEnd), calculatedStemEndX);
                            //! update stem
                            item.Stem.SetEndPointY(stemEndY);
                        }
                    }
                }
                else
                {
                    //! find highest pitch, set sortest stem then recalculate other stems according to calculated slope
                    var highestNote = mainBeamsList.Select(x => x).Where(x => x.Stem.NoteReference.PitchedPosition.Values.FirstOrDefault() == pitchesList.Min(y => y.Value)).FirstOrDefault();
                    double highestStemEnd = highestNote.Stem.GetStemEndCalculated().Y;
                    double highestStemEndX = positionsTable[highestNote.FractionPosition];
                    if (slopeUpward)
                    {
                        foreach (var item in mainBeamsList)
                        {
                            double calculatedStemEndX = positionsTable[item.FractionPosition] - highestStemEndX;
                            double stemEndY = FindStemEndY(-slopeValue, new Point(0, highestStemEnd), calculatedStemEndX);
                            //! update stem
                            item.Stem.SetEndPointY(stemEndY);
                        }
                    }
                    else
                    {
                        foreach (var item in mainBeamsList)
                        {
                            double calculatedStemEndX = positionsTable[item.FractionPosition] - highestStemEndX;
                            double stemEndY = FindStemEndY(slopeValue, new Point(0, highestStemEnd), calculatedStemEndX);
                            //! update stem
                            item.Stem.SetEndPointY(stemEndY);
                        }
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
        private void CalculateSlopes(Dictionary<int, double> positionsTable)
        {
            this.positionsTable = positionsTable;
            List<BeamItem> mainBeam = beamsList[1];
            bool isDown = mainBeam.FirstOrDefault().Stem.IsDirectionDown();
            int begin = mainBeam.FirstOrDefault().Stem.NoteReference.PitchedPosition.FirstOrDefault().Value;
            int end = mainBeam.LastOrDefault().Stem.NoteReference.PitchedPosition.FirstOrDefault().Value;
            slope = GetSlope(begin, end);
            slopeValue = ConvertBeamSlopeToSlope(slope, GetDistanceBetweenFirstLastStem());
        }

        private double GetDistanceBetweenFirstLastStem()
        {
            double result = 0.0;
            List<BeamItem> mainBeam = beamsList[1];
            double firstX = positionsTable[mainBeam.FirstOrDefault().FractionPosition];
            double lastX = positionsTable[mainBeam.LastOrDefault().FractionPosition];
            result = lastX - firstX;
            return result;
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
        private double ConvertBeamSlopeToSlope(double beamSlope, double distanceBetweenFirstLastStem)
        {
            double result;
            Point last = new Point(distanceBetweenFirstLastStem, beamSlope * 10.0.TenthsToWPFUnit());
            Point first = new Point();
            result = CalculateSlopeValue(first, last);
            return result;
        }

        private double CalculateSlopeValue(Point first, Point last)
        {
            double result;
            if (first.X == last.X)
            {
                //points lying on vertical line, division by 0 
                return 0; 
            }
            result = (last.Y - first.Y) / (last.X - first.X);
            return result;
        }

        private double FindStemEndY(double slope, Point referencePoint, double stemX)
        {
            double result;
            if (referencePoint.X == stemX)
            {
                //division by 0; 
                //points have same x position, stem end.Y should be equal
                return referencePoint.Y;
            }
            result = -(slope * (referencePoint.X - stemX)) + referencePoint.Y;
            return result;
        }
    }
}