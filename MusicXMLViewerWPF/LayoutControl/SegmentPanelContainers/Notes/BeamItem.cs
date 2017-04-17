using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems.NoteItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    class BeamItem
    {
        private Dictionary<int, BeamValueMusicXML> beams;
        private string voice;
        private int fractionPosition;
        private StemItem stem;
        public BeamItem(List<BeamMusicXML> beamsList, string voice, int position, StemItem stem)
        {
            this.stem = stem;
            this.voice = voice;
            beams = new Dictionary<int, BeamValueMusicXML>();
            fractionPosition = position;
            foreach (var item in beamsList)
            {
                beams.Add(int.Parse(item.Number), item.Value);
            }
        }

        public Dictionary<int, BeamValueMusicXML> Beams
        {
            get
            {
                return beams;
            }

            set
            {
                beams = value;
            }
        }

        public string Voice
        {
            get
            {
                return voice;
            }

            set
            {
                voice = value;
            }
        }

        public int FractionPosition
        {
            get
            {
                return fractionPosition;
            }

            set
            {
                fractionPosition = value;
            }
        }

        internal StemItem Stem
        {
            get
            {
                return stem;
            }

            set
            {
                stem = value;
            }
        }
    }
}
