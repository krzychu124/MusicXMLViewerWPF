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
        }
        public void Draw(Dictionary<int, double> positionsTable)
        {
            Dictionary<int, Point> positions = new Dictionary<int, Point>();
            foreach (var item in beamedNotes)
            {
                positions.Add(item.Key, new Point(positionsTable[item.Key] + beamedNotes[item.Key].Stem.GetStemEnd().X, beamedNotes[item.Key].Stem.GetStemEnd().Y));
            }
            DrawingVisual dv = new DrawingVisual();
            var keys = positions.Keys.ToArray();
            using (DrawingContext dc = dv.RenderOpen())
            {
                for (int i = 1; i < positions.Count; i++)
                {
                    dc.DrawLine(new Pen(beamedNotes.FirstOrDefault().Value.Stem.GetColor(), 5.0.TenthsToWPFUnit()), positions[keys[i-1]], positions[keys[i]]);
                }
            }
            DrawingVisualHost dvh = new DrawingVisualHost();
            dvh.AddVisual(dv);
            beamVisuals.Add(dvh);
        }
    }
}