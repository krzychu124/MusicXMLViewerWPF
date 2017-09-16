using MusicXMLScore.Model.MeasureItems;
using System.Collections.Generic;
using System.Linq;

namespace MusicXMLScore.Model.Builders
{
    public class ScorePartwisePartMeasureBuilder
    {
        private readonly ScorePartwisePartMeasureMusicXML measure;

        public ScorePartwisePartMeasureBuilder()
        {
            measure = new ScorePartwisePartMeasureMusicXML
            {
                Items = new object[0]
            };
        }

        public ScorePartwisePartMeasureBuilder AddBaseAttributes(AttributesMusicXML attributes)
        {
            measure.AppendNewItem(attributes);
            return this;
        }

        public ScorePartwisePartMeasureBuilder AddNote(NoteMusicXML note)
        {
            measure.AppendNewItem(note);
            return this;
        }

        public ScorePartwisePartMeasureBuilder AddRest(RestMusicXML rest)
        {
            measure.AppendNewItem(rest);
            return this;
        }

        public ScorePartwisePartMeasureBuilder AddChord(List<NoteMusicXML> notesChord)
        {
            notesChord.ForEach(item => measure.AppendNewItem(item));
            return this;
        }

        public ScorePartwisePartMeasureMusicXML Build()
        {
            CheckItemsOrder();
            return measure;
        }

        //TODO add direction
        //TODO set number (if none - auto generated)
        //TODO set barline
        //TODO add backup, forward
        //TODO add print info (layout manager information - measure on new page/new line etc.)
        
        private void CheckItemsOrder()
        {
            bool containsAttributes = measure.Items.Any(item => item.GetType() == typeof(AttributesMusicXML));
            if (containsAttributes)
            {
                if (measure.Items[0].GetType() != typeof(AttributesMusicXML))
                {
                    var list = measure.Items.ToList();
                    int attributeIndex = list.FindIndex(item => item.GetType() == typeof(AttributesMusicXML));
                    var attributes = list.ElementAt(attributeIndex) as AttributesMusicXML;
                    list.RemoveAt(attributeIndex);
                    list.Insert(0, attributes);
                }
            }
        }
    }
}
