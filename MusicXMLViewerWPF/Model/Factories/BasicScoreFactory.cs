using MusicXMLScore.Model.Builders;
using MusicXMLScore.Model.MeasureItems;
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

            var partBuilder = new ScorePartwisePartBuilder();
            //---
            var measureBuilder = new ScorePartwisePartMeasureBuilder();
            measureBuilder.AddBaseAttributes(
                new MeasureItems.AttributesMusicXML
                {
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
            var note1 = new NoteMusicXML
            {
                Items = new object[] { 128, new PitchMusicXML
                    {
                        Octave = "4",
                        Step = Helpers.SimpleTypes.StepMusicXML.C
                    }
                },
                ItemsElementName = new NoteChoiceTypeMusicXML[] { NoteChoiceTypeMusicXML.duration, NoteChoiceTypeMusicXML.pitch },
                Voice = "1",
                Stem = new MeasureItems.NoteItems.StemMusicXML
                {
                    Value = MeasureItems.NoteItems.StemValueMusicXML.up
                }
            };
            measureBuilder.AddNote(note1);
            partBuilder.AddMeasure(measureBuilder.Build());
            //===
            var r = new Random();
            for (int i = 0; i < 32; i++)
            {
                var measureBuilder2 = new ScorePartwisePartMeasureBuilder();
                for (int j = 0; j < 8; j++)
                {
                    var randOctave = 3 + j %( r.Next(4) +1);
                    var builder = new NoteBuilder();
                    var step = (Helpers.SimpleTypes.StepMusicXML) r.Next(7); //random step
                    var noteX = builder
                        .SetPitch(step, randOctave)
                        .SetStem(randOctave >4 ?MeasureItems.NoteItems.StemValueMusicXML.down : MeasureItems.NoteItems.StemValueMusicXML.up)
                        .SetDuration(16)
                        .SetVoice(1)
                        .Build();
                    measureBuilder2.AddNote(noteX);
                }
                partBuilder.AddMeasure(measureBuilder2.Build());
            }
            var scoreBuilder = new ScorePartwiseBuilder();
            return scoreBuilder.AddPart(partBuilder.Build(), "Part 0").Build();
        }
    }
}
