using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Model.MeasureItems.NoteItems;
using System;
using System.Linq;

namespace MusicXMLScore.Model.Builders
{
    class NoteBuilder
    {

        private NoteMusicXML note;

        public NoteBuilder()
        {
            note = new NoteMusicXML()
            {
                Items = new object[0]
            };
        }

        public NoteBuilder SetRest(RestMusicXML rest)
        {
            if (rest == null)
            {
                throw new ArgumentNullException("NoteBuilder::SetRest() - Rest Argument cannot be null");
            }
            AppendItem(NoteChoiceTypeMusicXML.rest, rest);
            return this;
        }

        public NoteBuilder SetLengthValue(NoteTypeValueMusicXML noteValue)
        {
            note.Type = new NoteTypeMusicXML { Value = noteValue };
            return this;
        }

        public NoteBuilder SetPitch(StepMusicXML step, int octave)
        {
            if (octave <0 || octave > 9)
            {
                throw new Exception("NoteBuilder::SetPitch() -Octave Pitch have to be in between 0 and 9");
            }
            var pitch = new PitchMusicXML
            {
                Step = step,
                Octave = octave.ToString()
            };
            AppendItem(NoteChoiceTypeMusicXML.pitch, pitch);
            return this;
        }

        public NoteBuilder SetDuration(int duration)
        {
            if (duration < 0)
            {
                throw new Exception("NoteBuilder::SetDuration() - Note Duration have to be 0 or positive number");
            }
            AppendItem(NoteChoiceTypeMusicXML.duration, duration);
            return this;
        }

        public NoteBuilder SetStem(StemValueMusicXML stemDirection)
        {
            note.Stem = new StemMusicXML { Value = stemDirection };
            return this;
        }

        public NoteBuilder SetVoice(int voiceNumber)
        {
            if(voiceNumber <1 || voiceNumber >8)
            {
                throw new Exception("NoteBuilder::SetVoice - Voice number have to be in between 1 and 8");
            }
            note.Voice = voiceNumber.ToString();
            return this;
        }

        //TODO set beam...
        //TODO set Notation...

        public NoteMusicXML Build()
        {
            if (!note.ItemsElementName.Any(item => item == NoteChoiceTypeMusicXML.duration))
            {
                throw new Exception("NoteBuilder::Note Duration have to be set!");
            }
            if (!note.ItemsElementName.Any(item=> item == NoteChoiceTypeMusicXML.pitch || item == NoteChoiceTypeMusicXML.unpitched))
            {
                throw new Exception("NoteBuilder::Note Pitch have to be set!");
            }
            if (!(note.Beam != null ^ note.Stem != null))
            {
                throw new Exception("NoteBuilder::Note Beam or Stem have to be set! Either Beam or Stem but not both");
            }
            if (note.Voice == null)
            {
                throw new Exception("NoteBuilder::Voice cannot be null!");
            }
            return note;
        }

        private void AppendItem(NoteChoiceTypeMusicXML noteType, object value)
        {
            if (note.ItemsElementName == null)
            {
                note.ItemsElementName = new NoteChoiceTypeMusicXML[] { noteType };
                note.Items = new object[] { value };
            }
            else if (note.ItemsElementName.Any(item => item == noteType))
            {
                Console.WriteLine($"NoteBuilder::Note Items Array already contains this type of item ({noteType.ToString()}), item of found noteType will be overwritten");
                note.Items[Array.IndexOf(note.ItemsElementName, noteType)] = value;
            }
            else
            {
                object[] temp = note.Items;
                Array.Resize(ref temp, temp.Length + 1);
                temp[temp.Length - 1] = value;
                note.Items = temp;
                var tempElementName = note.ItemsElementName;
                Array.Resize(ref tempElementName, tempElementName.Length +1);
                tempElementName[tempElementName.Length - 1] = noteType;
                note.ItemsElementName = tempElementName;
            }
            if (note.Items.Length != note.ItemsElementName.Length)
            {
                throw new Exception("Something went wrong! .Items and .ItemsElementName arrays lengths should be equal!");
            }
        }
    }
}
