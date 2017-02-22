using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutStyle
{
    [Serializable]
    public class BarlineLayoutStyle
    {
        private double heavyLineThickness = 5;
        private double thinLineThickness = 0.75;
        private double spaceBetweenDoubleBarlines = 1;
        #region RepeatStyle
        private WingType defaultWingStyle = WingType.none;
        private double forwadRepeatDotOffset = 2;
        private double backwartRepeatDotOffset = 2;
        #endregion
        #region EndingStyles
        private double endingHeigth = 25;
        private double endingLineThickness = 0.75;
        private double endingHookLength = 20;
        private double endingVerticalTextOffset = 20;
        private double endingHorizontalTextOffset = 5;

        public double HeavyLineThickness
        {
            get
            {
                return heavyLineThickness;
            }

            set
            {
                heavyLineThickness = value;
            }
        }

        public double ThinLineThickness
        {
            get
            {
                return thinLineThickness;
            }

            set
            {
                thinLineThickness = value;
            }
        }

        public double SpaceBetweenDoubleBarlines
        {
            get
            {
                return spaceBetweenDoubleBarlines;
            }

            set
            {
                spaceBetweenDoubleBarlines = value;
            }
        }

        public WingType DefaultWingStyle
        {
            get
            {
                return defaultWingStyle;
            }

            set
            {
                defaultWingStyle = value;
            }
        }

        public double ForwadRepeatDotOffset
        {
            get
            {
                return forwadRepeatDotOffset;
            }

            set
            {
                forwadRepeatDotOffset = value;
            }
        }

        public double BackwartRepeatDotOffset
        {
            get
            {
                return backwartRepeatDotOffset;
            }

            set
            {
                backwartRepeatDotOffset = value;
            }
        }

        public double EndingHeigth
        {
            get
            {
                return endingHeigth;
            }

            set
            {
                endingHeigth = value;
            }
        }

        public double EndingLineThickness
        {
            get
            {
                return endingLineThickness;
            }

            set
            {
                endingLineThickness = value;
            }
        }

        public double EndingHookLength
        {
            get
            {
                return endingHookLength;
            }

            set
            {
                endingHookLength = value;
            }
        }

        public double EndingVerticalTextOffset
        {
            get
            {
                return endingVerticalTextOffset;
            }

            set
            {
                endingVerticalTextOffset = value;
            }
        }

        public double EndingHorizontalTextOffset
        {
            get
            {
                return endingHorizontalTextOffset;
            }

            set
            {
                endingHorizontalTextOffset = value;
            }
        }
        #endregion

    }
}
