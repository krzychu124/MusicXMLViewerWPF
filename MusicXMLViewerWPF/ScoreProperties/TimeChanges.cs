using MusicXMLScore.Model.MeasureItems.Attributes;
using System.Collections.Generic;

namespace MusicXMLScore.ScoreProperties
{
    public class TimeChanges : MeasureAttributeChanges<TimeMusicXML>
    {
        /// <summary>
        /// Adds time signature change to measure staff at cursor position(measure fraction)
        /// </summary>
        /// <param name="staffNumber"></param>
        /// <param name="timeFraction"></param>
        /// <param name="timeChange"></param>
        public void Add(string staffNumber, int timeFraction, TimeMusicXML timeChange)
        {
            base.Add(new TimeChange(staffNumber, timeFraction, timeChange));
        }
    }

    public class TimeChange : AttributeChange<TimeMusicXML>
    {
        public TimeChange(string staffNumber, int timeFraction, TimeMusicXML timeMusicXml) : base(staffNumber, timeFraction, timeMusicXml)
        {
        }
    }

    public class TimeChangesDictionary : Dictionary<string, TimeChanges>
    {
        public new void Add(string measureId, TimeChanges timeChanges)
        {
            TimeChanges tempTimeChanges;
            if (TryGetValue(measureId, out tempTimeChanges))
            {
                tempTimeChanges.AttributeChanges.AddRange(timeChanges.AttributeChanges);
            }
            else
            {
                base.Add(measureId, timeChanges);
            }
        }
    }
}