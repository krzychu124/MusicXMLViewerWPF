using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Converters;

namespace MusicXMLScore.DrawingHelpers
{
    class PartPropertiesContainer //TODO_WIP check usage and remove in no
    {
        Dictionary<string, PartProperties> partProperties = new Dictionary<string, PartProperties>();
        public PartPropertiesContainer(MusicXMLViewerWPF.ScorePartwiseMusicXML score)
        {
            GetParts(score);
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

        private void GetParts(MusicXMLViewerWPF.ScorePartwiseMusicXML score)
        {
            foreach (var part in score.Part)
            {
                partProperties.Add(part.Id, new PartProperties(score, part.Id));
            }
        }
    }
}
