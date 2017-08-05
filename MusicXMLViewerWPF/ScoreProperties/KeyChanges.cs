using MusicXMLScore.Model.MeasureItems.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreProperties
{
    public class KeyChanges : MeasureAttributeChanges<KeyMusicXML>
    {
        public void Add(string measureId, int timeFraction, KeyMusicXML key)
        {
            base.Add(new KeyChange(measureId, timeFraction,key));
        }
    }

    public class KeyChange : AttributeChange<KeyMusicXML>
    {
        public KeyChange(string measureId, int timeFraction, KeyMusicXML key):base(measureId, timeFraction, key)
        {
        }
    }

    public class KeyChangesDictionary : AttributeChangesDictionary<KeyChanges, KeyMusicXML>
    {
        /// <summary>
        /// Adds KeyChanges to dictionary or Appends if (measureId)Key found
        /// </summary>
        /// <param name="measureId"></param>
        /// <param name="keyChanges"></param>
        public new void Add(string measureId, KeyChanges keyChanges)
        {
            base.Add(measureId, keyChanges);
        }
    }
}