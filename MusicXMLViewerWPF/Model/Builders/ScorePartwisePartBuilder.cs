using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicXMLScore.Model.Builders
{
    public class ScorePartwisePartBuilder
    {
        private readonly ScorePartwisePartMusicXML part;
       

        public ScorePartwisePartBuilder()
        {
            part = new ScorePartwisePartMusicXML
            {
                Measure = new List<ScorePartwisePartMeasureMusicXML>()
            };
        }

        public ScorePartwisePartMusicXML Build()
        {
            if (part.Id == null)
            {
                part.Id = "P0";
            }
            return part;
        }

        public ScorePartwisePartBuilder AddMeasure(ScorePartwisePartMeasureMusicXML measure)
        {
            CheckMeasureId(measure);
            part.Measure.Add(measure);
            return this;
        }

        private void CheckMeasureId(ScorePartwisePartMeasureMusicXML measure)
        {
            if (measure.Number == null)
            {
                if (part.Measure.Count() == 0)
                {
                    measure.Number = 1 + "";
                }else
                if (int.TryParse(part.Measure.LastOrDefault().Number, out int number))
                {
                    // set next number
                    measure.Number = ++number + "";
                }
                else
                {
                    Console.WriteLine("Part Builder measure number parse failed: " + measure.Number);
                }
            }
        }
    }
}
