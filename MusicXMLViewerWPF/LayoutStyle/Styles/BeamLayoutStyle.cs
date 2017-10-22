using System;

namespace MusicXMLScore.LayoutStyle
{
    [Serializable]
    public class BeamLayoutStyle
    {
        private double beamThickenss = 5;
        private double shortBeamLength = 7.5; //! temp
        private double beamSeparation = 2.5;
        private double maxBeamSlope = 5; //! test 

        public double BeamThickenss
        {
            get
            {
                return beamThickenss;
            }

            set
            {
                beamThickenss = value;
            }
        }

        public double ShortBeamLength
        {
            get
            {
                return shortBeamLength;
            }

            set
            {
                shortBeamLength = value;
            }
        }

        public double BeamSeparation
        {
            get
            {
                return beamSeparation;
            }

            set
            {
                beamSeparation = value;
            }
        }

        public double MaxBeamSlope
        {
            get
            {
                return maxBeamSlope;
            }

            set
            {
                maxBeamSlope = value;
            }
        }
    }
}
