using System.Diagnostics;

namespace MusicXMLScore.LayoutControl
{
    [DebuggerDisplay("{DebugDisplay, nq}")]
    class AntiCollisionHelper //temporary name
    {
        private int _factionPosition;
        private double _fractionDuration;
        private double _spacingFactor;
        private double _elementWidth;
        private double _leftMinWidth;
        private double _rightMinWidth;
        private double _fractionPosX;
        private double _fractionStretch;

        public int FactionPosition
        {
            get { return _factionPosition; }

            set { _factionPosition = value; }
        }

        public double FractionDuration
        {
            get { return _fractionDuration; }

            set { _fractionDuration = value; }
        }

        public double ElementWidth
        {
            get { return _elementWidth; }

            set { _elementWidth = value; }
        }

        public double LeftMinWidth
        {
            get { return _leftMinWidth; }

            set { _leftMinWidth = value; }
        }

        public double RightMinWidth
        {
            get { return _rightMinWidth; }

            set { _rightMinWidth = value; }
        }

        private string DebugDisplay => string.Format("{0}, {1:N2}, {2:N2}, {3:N2}, {4:N2} {5:N2} FS:{6:N2}", FactionPosition, FractionDuration, SpacingFactor,
            ElementWidth, LeftMinWidth, RightMinWidth, FractionStretch);

        public double SpacingFactor
        {
            get { return _spacingFactor; }

            set { _spacingFactor = value; }
        }

        public double FractionPosX
        {
            get { return _fractionPosX; }

            set { _fractionPosX = value; }
        }

        public double FractionStretch
        {
            get { return _fractionStretch; }

            set { _fractionStretch = value; }
        }

        public AntiCollisionHelper(int fractionPosition, double fractionDuration, double spacingFactor, double elementWidth, double leftMin,
            double rightMin)
        {
            FactionPosition = fractionPosition;
            FractionDuration = fractionDuration;
            SpacingFactor = spacingFactor;
            ElementWidth = elementWidth;
            LeftMinWidth = leftMin;
            RightMinWidth = rightMin;
        }
    }
}