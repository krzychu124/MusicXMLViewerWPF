using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl
{

    [DebuggerDisplay("{DebugDisplay, nq}")]
    class AntiCollisionHelper //temporary name
    {
        private int factionPosition;
        private double fractionDuration;
        private double spacingFactor;
        private double elementWidth;
        private double leftMinWidth;
        private double rightMinWidth;
        private double fractionPosX;
        private double fractionStretch;
        public int FactionPosition
        {
            get
            {
                return factionPosition;
            }

            set
            {
                factionPosition = value;
            }
        }

        public double FractionDuration
        {
            get
            {
                return fractionDuration;
            }

            set
            {
                fractionDuration = value;
            }
        }

        public double ElementWidth
        {
            get
            {
                return elementWidth;
            }

            set
            {
                elementWidth = value;
            }
        }

        public double LeftMinWidth
        {
            get
            {
                return leftMinWidth;
            }

            set
            {
                leftMinWidth = value;
            }
        }

        public double RightMinWidth
        {
            get
            {
                return rightMinWidth;
            }

            set
            {
                rightMinWidth = value;
            }
        }

        private string DebugDisplay
        {
            get { return string.Format("{0}, {1:N2}, {2:N2}, {3:N2}, {4:N2} {5:N2} FS:{6:N2}", FactionPosition, FractionDuration, SpacingFactor, ElementWidth, LeftMinWidth, RightMinWidth, FractionStretch); }
        }

        public double SpacingFactor
        {
            get
            {
                return spacingFactor;
            }

            set
            {
                spacingFactor = value;
            }
        }

        public double FractionPosX
        {
            get
            {
                return fractionPosX;
            }

            set
            {
                fractionPosX = value;
            }
        }

        public double FractionStretch
        {
            get
            {
                return fractionStretch;
            }

            set
            {
                fractionStretch = value;
            }
        }

        public AntiCollisionHelper(int fractionPosition, double fractionDuration, double spacingFactor, double elementWidth, double leftMin, double rightMin)
        {
            this.FactionPosition = fractionPosition;
            this.FractionDuration = fractionDuration;
            this.spacingFactor = spacingFactor;
            this.ElementWidth = elementWidth;
            this.LeftMinWidth = leftMin;
            this.RightMinWidth = rightMin;
        }
    }
}
