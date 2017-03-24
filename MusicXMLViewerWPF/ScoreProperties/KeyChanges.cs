using MusicXMLScore.Model.MeasureItems.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreProperties
{
    public class KeyChanges
    {
        #region Fields

        private List<Tuple<string, int, KeyMusicXML>> keys = new List<Tuple<string, int, KeyMusicXML>>();

        #endregion Fields

        #region Constructors

        public KeyChanges()
        {

        }

        #endregion Constructors

        #region Properties

        public List<Tuple<string, int, KeyMusicXML>> KeysChanges
        {
            get
            {
                return keys;
            }

            set
            {
                keys = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds Key Signature to measure
        /// </summary>
        /// <param name="staff"> Staff Number when position is other than default (all staffs)</param>
        /// <param name="position"> Measure fraction position from where key signature will be changed</param>
        /// <param name="key">Selected key signature</param>
        public void Add(string staff, int position, KeyMusicXML key)
        {
            keys.Add(Tuple.Create(staff, position, key));
        }

        #endregion Methods
    }
    public class KeyChangesDictionary: Dictionary<string, KeyChanges>
    {
        #region Methods

        /// <summary>
        /// Adds KeyChanges to dictionary or Appends if (measureID)Key found
        /// </summary>
        /// <param name="measureID"></param>
        /// <param name="keyChanges"></param>
        public new void Add(string measureID, KeyChanges keyChanges)
        {
            KeyChanges keys;
            if (TryGetValue(measureID, out keys))
            {
                keys.KeysChanges.AddRange(keyChanges.KeysChanges);
            }
            else
            {
                base.Add(measureID, keyChanges);
            }
        }

        #endregion Methods
    }
}
