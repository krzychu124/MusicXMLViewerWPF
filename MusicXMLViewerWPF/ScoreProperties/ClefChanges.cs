using MusicXMLScore.Model.MeasureItems.Attributes;

namespace MusicXMLScore.ScoreProperties
{
    public class ClefChanges : MeasureAttributeChanges<ClefMusicXML>
    {
        public void Add(string staff, int timeFraction, ClefMusicXML clef)
        {
            base.Add(new ClefChange(staff, timeFraction, clef));
        }
    }

    public class ClefChange : AttributeChange<ClefMusicXML>
    {
        public ClefChange(string staff, int timeFraction, ClefMusicXML clef) : base(staff, timeFraction, clef)
        {
        }
    }

    public class ClefChangesDictionary : AttributeChangesDictionary<ClefChanges, ClefMusicXML>
    {
        /// <summary>
        /// Adds or appends clefChanges if dictionary contains key
        /// </summary>
        /// <param name="measureId"></param>
        /// <param name="clefChanges"></param>
        public new void Add(string measureId, ClefChanges clefChanges)
        {
            if (ContainsKey(measureId))
            {
                ClefChanges clefs;
                TryGetValue(measureId, out clefs);
                clefs?.AttributeChanges.AddRange(clefChanges.AttributeChanges);
            }
            else
            {
                base.Add(measureId, clefChanges);
            }
        }
    }
}