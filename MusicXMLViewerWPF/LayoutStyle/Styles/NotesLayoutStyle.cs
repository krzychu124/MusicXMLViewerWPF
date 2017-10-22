namespace MusicXMLScore.LayoutStyle
{
    public class NotesLayoutStyle
    {
        private double ledgerLineWidth =10;
        private double ledgerLineThickness = 0.75;

        private double minStemLength = 22;
        private double normalStemLength = 35;
        private double maxStemLength = 45;
        private double stemThickness = 1.2;

        private double dotStandardOffset = 5; //! test
        private double dotIfFlagOffset = 5; //! test

        #region ArticulationStyle
        private double articulationToNoteOffset = 2;
        private double articualtionToOtherOffset = 2;
        #endregion

        #region AccidentalStyle
        private double accidentalToNoteSpace = 3;
        private double accidentalToAccidentalSpace = 1;
        #endregion

        private double cueNoteSize = 0.70;

        #region GraceNoteStyle
        private double graceNoteSize = 60;
        private double graceNoteRightOffset = 1;
        private double graceNoteSlashThickness = 0.45;

        public double LedgerLineWidth
        {
            get
            {
                return ledgerLineWidth;
            }

            set
            {
                ledgerLineWidth = value;
            }
        }

        public double LedgerLineThickness
        {
            get
            {
                return ledgerLineThickness;
            }

            set
            {
                ledgerLineThickness = value;
            }
        }

        public double MinStemLength
        {
            get
            {
                return minStemLength;
            }

            set
            {
                minStemLength = value;
            }
        }

        public double NormalStemLength
        {
            get
            {
                return normalStemLength;
            }

            set
            {
                normalStemLength = value;
            }
        }

        public double MaxStemLength
        {
            get
            {
                return maxStemLength;
            }

            set
            {
                maxStemLength = value;
            }
        }

        public double StemThickness
        {
            get
            {
                return stemThickness;
            }

            set
            {
                stemThickness = value;
            }
        }

        public double DotStandardOffset
        {
            get
            {
                return dotStandardOffset;
            }

            set
            {
                dotStandardOffset = value;
            }
        }

        public double DotIfFlagOffset
        {
            get
            {
                return dotIfFlagOffset;
            }

            set
            {
                dotIfFlagOffset = value;
            }
        }

        public double ArticulationToNoteOffset
        {
            get
            {
                return articulationToNoteOffset;
            }

            set
            {
                articulationToNoteOffset = value;
            }
        }

        public double ArticualtionToOtherOffset
        {
            get
            {
                return articualtionToOtherOffset;
            }

            set
            {
                articualtionToOtherOffset = value;
            }
        }

        public double AccidentalToNoteSpace
        {
            get
            {
                return accidentalToNoteSpace;
            }

            set
            {
                accidentalToNoteSpace = value;
            }
        }

        public double AccidentalToAccidentalSpace
        {
            get
            {
                return accidentalToAccidentalSpace;
            }

            set
            {
                accidentalToAccidentalSpace = value;
            }
        }

        public double CueNoteSize
        {
            get
            {
                return cueNoteSize;
            }

            set
            {
                cueNoteSize = value;
            }
        }

        public double GraceNoteSize
        {
            get
            {
                return graceNoteSize;
            }

            set
            {
                graceNoteSize = value;
            }
        }

        public double GraceNoteRightOffset
        {
            get
            {
                return graceNoteRightOffset;
            }

            set
            {
                graceNoteRightOffset = value;
            }
        }

        public double GraceNoteSlashThickness
        {
            get
            {
                return graceNoteSlashThickness;
            }

            set
            {
                graceNoteSlashThickness = value;
            }
        }
        #endregion
    }
}
