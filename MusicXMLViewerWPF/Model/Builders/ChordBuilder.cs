using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.MeasureItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.Model.Builders
{

    class ChordBuilder
    {
        private List<NoteMusicXML> notes;

        public ChordBuilder()
        {
            notes = new List<NoteMusicXML>();

        }

        public ChordBuilder AddNote(NoteMusicXML note)
        {

            if (note == null)
            {
                throw new ArgumentException("ChordBuilder::AddNote Argument value 'note' cannot be null");
            }
            if (note.GetNoteType() == NoteChoiceTypeMusicXML.grace)
            {
                throw new ArgumentException("ChordBuilder::AddNote Grace notes currently not supported in chords");
            }
            if (notes.Count > 0)
            {
                if (notes[0].GetDuration() != note.GetDuration())
                {
                    Console.WriteLine("ChordBuilder::AddNote Added note to chord has different duration than chord main note, duration will be corrected to main note");
                    note.SetDuration(notes[0].GetDuration());
                }

            }
            notes.Add(note);
            return this;
        }
        public List<NoteMusicXML> Build()
        {
            if (notes.Count < 2)
            {
                throw new Exception("ChordBuilder::Build Chord should contains at least 2 notes!");
            }
            MakeChord();
            return notes;
        }

        private void MakeChord()
        {
            if (notes.Count > 1)
            {
                for (int i = 0; i < notes.Count; i++)
                {
                    if (notes[i].Items.Length != notes[i].ItemsElementName.Length)
                    {
                        throw new Exception("Something went wrong, .Items and .ItemsElementName Arrays lengths should be equal");
                    }
                    if (notes[i].ItemsElementName.Any(item => item == NoteChoiceTypeMusicXML.chord))
                    {
                        var index = Array.IndexOf(notes[i].ItemsElementName, NoteChoiceTypeMusicXML.chord);

                        var list = notes[i].ItemsElementName.ToList();
                        list.Remove(NoteChoiceTypeMusicXML.chord);
                        notes[i].ItemsElementName = list.ToArray();

                        var items = notes[i].Items.ToList();
                        items.RemoveAt(index);
                        notes[i].Items = items.ToArray();
                    }
                    if (i != 0)
                    {
                        var note = notes[i].ItemsElementName.ToList();
                        note.Insert(0, NoteChoiceTypeMusicXML.chord);
                        notes[i].ItemsElementName = note.ToArray();

                        var noteItems = notes[i].Items.ToList();
                        noteItems.Insert(0, new EmptyMusicXML());
                        notes[i].Items = noteItems.ToArray();
                    }
                }

            }
        }
    }
}
