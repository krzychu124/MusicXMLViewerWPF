using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutStyle
{
    public class MeasureLayoutStyle
    {
        #region Fields
        
        private double attributesRightOffset = 1.5;

        private double clefLeftOffset = 8;
        private double clefRightOffset = 2;

        private double keySigLeftOffset = 2;
        private double keySigRightOffset = 8;
        private double keySigSpacingOffset = 2;

        private double maxMeasureWidth = 300;
        private double minMeasureWidth = 70;
        
        private double smallClefSize = 80;

        private double staffSpaceLegth = 10;
        private double stavesDistance = 70;

        private double timeSigLeftOffset = 2;
        private double timeSigRightOffset = 2;

        #endregion Fields

        #region Properties

        public double AttributesRightOffset
        {
            get
            {
                return attributesRightOffset;
            }

            set
            {
                attributesRightOffset = value;
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

        /// <summary>
        /// Percent size of NormalClefSize
        /// </summary>
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

        #endregion Properties
    }
}
