using MusicXMLScore.Model.Builders;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.Model.Factories
{
    class BasicScoreFactory
    {
        public static ScorePartwiseMusicXML GetScorePartwise()
        {
            var measureBuilder = new ScorePartwisePartMeasureBuilder();
            measureBuilder.AddBaseAttributes(
                new MeasureItems.AttributesMusicXML {
                    Divisions = 32,
                    DivisionsSpecified = true,
                    Clef = new List<MeasureItems.Attributes.ClefMusicXML>
                    {
                        new MeasureItems.Attributes.ClefMusicXML
                        {
                            Sign = MeasureItems.Attributes.ClefSignMusicXML.G,
                            Line = "2"
                        }
                    },
                    Key = new List<MeasureItems.Attributes.KeyMusicXML>
                    {
                        new MeasureItems.Attributes.KeyMusicXML
                        {
                            Items = new object[]{ 2.ToString() },
                            ItemsElementName = new MeasureItems.Attributes.KeyChoiceTypes[]{ MeasureItems.Attributes.KeyChoiceTypes.fifths},
                             Number = "1"
                        }
                    },
                    Time = new List<MeasureItems.Attributes.TimeMusicXML>
                    {
                        new MeasureItems.Attributes.TimeMusicXML
                        {
                            Number = "1",
                            Items = new object[]
                            {
                                4.ToString(),
                                4.ToString()
                            },
                            ItemsElementName = new MeasureItems.Attributes.TimeChoiceTypeMusicXML[]
                            {
                                MeasureItems.Attributes.TimeChoiceTypeMusicXML.beats,
                                MeasureItems.Attributes.TimeChoiceTypeMusicXML.beattype
                            }
                           
                        }
                    }
                });
            measureBuilder.AddRest(
                new MeasureItems.RestMusicXML
                {
                    MeasureSpecified = true,
                    Measure = Helpers.SimpleTypes.YesNoMusicXML.yes
                });
            var partBuilder = new ScorePartwisePartBuilder();
            partBuilder.AddMeasure(measureBuilder.Build());
            var scoreBuilder = new ScorePartwiseBuilder();
            return scoreBuilder.AddPart(partBuilder.Build(), "Part 0").Build();
        }
    }
}
