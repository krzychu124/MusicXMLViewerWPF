using MusicXMLScore.Model.MeasureItems.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ScoreProperties
{
    public class TimeChanges
    {
        #region Fields

        private List<Tuple<string, int, TimeMusicXML>> timesChanges = new List<Tuple<string, int, TimeMusicXML>>();

        #endregion Fields

        #region Properties

        public List<Tuple<string, int, TimeMusicXML>> TimesChanges
        {
            get
            {
                return timesChanges;
            }

            set
            {
                timesChanges = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds time signature change to measure staff at cursor position(measure fraction)
        /// </summary>
        /// <param name="staff"></param>
        /// <param name="cursorPosition"></param>
        /// <param name="timeChange"></param>
        public void Add(string staff, int cursorPosition, TimeMusicXML timeChange)
        {
            timesChanges.Add(Tuple.Create(staff, cursorPosition, timeChange));
        }

        #endregion Methods
    }

    public class TimeChangesDictionary: Dictionary<string, TimeChanges>
    {
        #region Methods

        public new void Add(string measureID, TimeChanges timeChanges)
        {
            TimeChanges tempTimeChanges;
            if (TryGetValue(measureID, out tempTimeChanges))
            {
                tempTimeChanges.TimesChanges.AddRange(timeChanges.TimesChanges);
            }
            else
            {
                base.Add(measureID, timeChanges);   
            }
        }

        #endregion Methods
    }
}
