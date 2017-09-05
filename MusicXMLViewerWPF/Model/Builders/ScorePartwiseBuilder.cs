using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.Model.Builders
{
    class ScorePartwiseBuilder
    {

        private readonly ScorePartwiseMusicXML score;

        public ScorePartwiseBuilder()
        {
            score = new ScorePartwiseMusicXML
            {
                Partlist = new PartListMusicXML { ScoreParts = new List<ScorePartMusicXML>() },
                Part = new List<ScorePartwisePartMusicXML>()
            };
        }

        public ScorePartwiseBuilder AddPart(ScorePartwisePartMusicXML part, String partName)
        {
            if(!score.Part.Any(p=> p.Id.Equals(p.Id)))
            {
                score.Partlist.ScoreParts.Add(new ScorePartMusicXML { PartId = part.Id, PartName = partName });
                score.Part.Add(part);
            } else
            {
                Console.WriteLine($"Part with id {part.Id} already found!");
            }
            return this;
        }

        public ScorePartwiseMusicXML Build()
        {
            if(score.Part.Count == 0)
            {
                AddDefaultPart();
            }
            return score;
        }

        private void AddDefaultPart()
        {
            ScorePartwisePartBuilder partBuilder = new ScorePartwisePartBuilder();
            ScorePartwisePartMusicXML part = partBuilder.Build();
            this.AddPart(part, "Part 0");
        }
    }
}
