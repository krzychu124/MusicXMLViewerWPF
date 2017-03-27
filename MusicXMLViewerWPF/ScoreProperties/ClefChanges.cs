using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Model.MeasureItems.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreProperties
{
    public class ClefChanges 
    {
        #region Fields

        private List<Tuple<string, int, ClefMusicXML>> clefs = new List<Tuple<string, int, ClefMusicXML>>();

        #endregion Fields

        #region Properties

        public List<Tuple<string, int, ClefMusicXML>> ClefsChanges
        {
            get
            {
                return clefs;
            }

            set
            {
                clefs = value;
            }
        }

        #endregion Properties

        #region Methods

        public void Add(string staff, int position, ClefMusicXML clef)
        {
            clefs.Add(Tuple.Create(staff, position, clef));
        }

        #endregion Methods
    }

    public class ClefChangesDictionary : Dictionary<string, ClefChanges>
    {
        #region Methods

        /// <summary>
        /// Adds or appends clefChanges if dictionary contains key
        /// </summary>
        /// <param name="measureID"></param>
        /// <param name="clefChanges"></param>
        public new void Add(string measureID, ClefChanges clefChanges)
        {
            if (base.ContainsKey(measureID))// 
            {
                ClefChanges clefs;
                base.TryGetValue(measureID, out clefs);
                clefs.ClefsChanges.AddRange(clefChanges.ClefsChanges);
            }
            else
            {
                base.Add(measureID, clefChanges);
            }
        }

        #endregion Methods
    }
}
