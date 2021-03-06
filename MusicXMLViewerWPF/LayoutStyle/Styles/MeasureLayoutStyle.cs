﻿namespace MusicXMLScore.LayoutStyle
{
    public class MeasureLayoutStyle
    {
        #region Fields
        
        private double attributesRightOffset = 12;
        private double notesGap = 2;

        private double clefLeftOffset = 6;
        private double clefRightOffset = 1;

        private double keySigLeftOffset = 1;
        private double keySigRightOffset = 1;
        private double keySigSpacingOffset = 2;

        private double maxMeasureWidth = 300;
        private double minMeasureContentWidth = 80;
        
        private double smallClefSize = 80;

        private double staffSpaceLegth = 10;
        private double staffLineThickness = 1.2;
        private double stavesDistance = 70;

        private double timeSigLeftOffset = 1;
        private double timeSigRightOffset = 3;

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

        public double MinMeasureContentWidth
        {
            get
            {
                return minMeasureContentWidth;
            }

            set
            {
                minMeasureContentWidth = value;
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

        public double StaffLineThickness
        {
            get
            {
                return staffLineThickness;
            }

            set
            {
                staffLineThickness = value;
            }
        }

        #endregion Properties
    }
}
