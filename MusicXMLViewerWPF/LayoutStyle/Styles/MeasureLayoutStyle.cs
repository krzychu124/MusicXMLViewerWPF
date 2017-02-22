using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutStyle
{
    public class MeasureLayoutStyle
    {
        private double staffSpaceLegth;
        private double minMeasureWidth = 70;
        private double maxMeasureWidth = 300;
        
        private double stavesDistance = 70;
        /// <summary>
        /// If false - clef visible only on first staff system
        /// </summary>
        #region ClefStyle
        private double clefLeftOffset = 0.8;
        private double clefRightOffset = 0.2;
        /// <summary>
        /// Percent size of NormalClefSize
        /// </summary>
        private double smallClefSize = 70;
        #endregion
        #region KeySigStyle
        private double keySigLeftOffset= 0.2;
        private double keySigRightOffset = 0.8;
        private double keySigSpacingOffset = 0.2;
        #endregion
        #region TimeSigStyle
        private double timeSigLeftOffset = 0.2;
        private double timeSigRightOffset = 0.2;

        public double StaffSpaceLegth
        {
            get
            {
                return staffSpaceLegth;
            }

            set
            {
                staffSpaceLegth = value;
            }
        }

        public double MinMeasureWidth
        {
            get
            {
                return minMeasureWidth;
            }

            set
            {
                minMeasureWidth = value;
            }
        }

        public double MaxMeasureWidth
        {
            get
            {
                return maxMeasureWidth;
            }

            set
            {
                maxMeasureWidth = value;
            }
        }

        public double StavesDistacne
        {
            get
            {
                return stavesDistance;
            }

            set
            {
                stavesDistance = value;
            }
        }

        public double ClefLeftOffset
        {
            get
            {
                return clefLeftOffset;
            }

            set
            {
                clefLeftOffset = value;
            }
        }

        public double ClefRightOffset
        {
            get
            {
                return clefRightOffset;
            }

            set
            {
                clefRightOffset = value;
            }
        }

        public double SmallClefSize
        {
            get
            {
                return smallClefSize;
            }

            set
            {
                smallClefSize = value;
            }
        }

        public double KeySigLeftOffset
        {
            get
            {
                return keySigLeftOffset;
            }

            set
            {
                keySigLeftOffset = value;
            }
        }

        public double KeySigRightOffset
        {
            get
            {
                return keySigRightOffset;
            }

            set
            {
                keySigRightOffset = value;
            }
        }

        public double KeySigSpacingOffset
        {
            get
            {
                return keySigSpacingOffset;
            }

            set
            {
                keySigSpacingOffset = value;
            }
        }

        public double TimeSigLeftOffset
        {
            get
            {
                return timeSigLeftOffset;
            }

            set
            {
                timeSigLeftOffset = value;
            }
        }

        public double TimeSigRightOffset
        {
            get
            {
                return timeSigRightOffset;
            }

            set
            {
                timeSigRightOffset = value;
            }
        }
        #endregion
    }
}
