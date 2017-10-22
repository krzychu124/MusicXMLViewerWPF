using MusicXMLScore.Helpers;
using System;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.DrawingHelpers
{
    enum MeasureLineCount
    {
        one = 1,
        two = 2,
        three = 3,
        four = 4,
        five = 5,
        six = 6
    }

    class DrawableStaffLine //: IDrawableObjectPart
    {
        #region Fields

        private Brush color = Brushes.Black;
        private MeasureLineCount measureLines = MeasureLineCount.five;
        private Point position = new Point();
        private PageProperties pageProperties;
        private DrawingVisual visualObject;
        private double width;
        private double[] linesYpositions;

        #endregion Fields

        #region Constructors

        public DrawableStaffLine(PageProperties pageProperties, double width, MeasureLineCount linesCount = MeasureLineCount.five, Point offsetPoint = new Point(), Brush color = null)
        {
            this.pageProperties = pageProperties;
            StaffLineWidth = width;
            Position = offsetPoint;
            Color = color ?? Brushes.Black;
            MeasureLines = linesCount;
            Draw();
        }

        #endregion Constructors

        #region Properties

        public Brush Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }

        public DrawingVisual PartialObjectVisual
        {
            get
            {
                return visualObject;
            }
        }

        public Point Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public double StaffLineWidth
        {
            get
            {
                return width;
            }

            set
            {
                width = value;
            }
        }

        internal MeasureLineCount MeasureLines
        {
            get
            {
                return measureLines;
            }

            set
            {
                measureLines = value;
            }
        }

        public double[] LinesYpositions
        {
            get
            {
                return linesYpositions;
            }

            set
            {
                linesYpositions = value;
            }
        }

        #endregion Properties

        #region Methods

        public void Draw()
        {
            visualObject = new DrawingVisual();
            using(DrawingContext dc = visualObject.RenderOpen())
            {
                GenerateGenericStaffLine(dc);
            }
        }

        private void GenerateGenericStaffLine(DrawingContext dc)//scale dependent
        {
            Brush color = Brushes.Black;
            double factor = PageProperties.PxPerMM(); // scalefactor 1mm to px
            double lineThickness = pageProperties.TenthToPx(1/*.4583*/);
            Pen pen = new Pen(color, lineThickness);
            double t = pageProperties.StaffSpace * factor;
            int Lines = (int)measureLines; // default is 5;
            linesYpositions = new double[Lines];
            int currentLineIndex = 0;
            Point shiftedPosition = GetCenteredStaffPosition(position); //! move y position to center staffline while stafflines < 5

            Point startPosition = shiftedPosition;
            Point endPosition = new Point(StaffLineWidth, shiftedPosition.Y);
            while (currentLineIndex < Lines)
            {
                linesYpositions[currentLineIndex] = startPosition.Y;
                dc.DrawLine(pen, startPosition, endPosition);
                startPosition = new Point(startPosition.X, startPosition.Y - t);
                endPosition = new Point(StaffLineWidth, endPosition.Y - t);
                ++currentLineIndex;
            }
        }

        /// <summary>
        /// Deprecated, use GenerateGenericStaffLine()
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="StartPoint"></param>
        /// <param name="color"></param>
        /// 
        [Obsolete("Use genericStaffLine generator for staffline drawing", true)]
        private void GenerateGlyphStaffLine(DrawingContext dc, Point StartPoint, Brush color = null)
        {
            if (color == null) color = Brushes.Black;
            float Scale = 40f;  //! MusicScore.Defaults.Scale.Tenths;
            float num = GetMeasureLength(StaffLineWidth);
            double filling = GetStaffLinesFilling(StaffLineWidth);
            float X = (float)StartPoint.X;
            float Y = (float)StartPoint.Y;
            int s = 0;
            for (int i = 0; i < num; i++)
            {
                Helpers.DrawingHelpers.DrawString(dc, MusicSymbols.mediumStaffFiveSymbol, Helpers.TypeFaces.BravuraMusicFont, color, X + s, Y, Scale);
                s += 29;
            }
            if (filling != 0)
            {
                Helpers.DrawingHelpers.DrawString(dc, MusicSymbols.longStaffOneSymbol, Helpers.TypeFaces.BravuraTextFont, color, (float)(X + (StaffLineWidth - 29)), Y, Scale);
            }
        }
        /// <summary>
        /// Get center point of measure staff height, used to center staff with less number of lines than standard: 5
        /// </summary>
        /// <param name="pointToShift"></param>
        /// <returns></returns>
        private Point GetCenteredStaffPosition(Point pointToShift)
        {
            double shift = (pageProperties.StaffHeight - (((int)measureLines - 1)* pageProperties.StaffSpace))/ 2;

            return new Point(pointToShift.X, pointToShift.Y + (shift *PageProperties.PxPerMM()));
        }
        /// <summary>
        /// Deprecated since custom drawing using GenerateGenericStaffLine()
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private int GetMeasureLength(double length)
        {
            int symbolCount = Convert.ToInt32(Math.Floor(length / 24));
            return symbolCount;
        }
        /// <summary>
        /// Deprecated since custom drawing GenerateGenericStaffLine()
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private float GetStaffLinesFilling(double l)
        {
            float filling = (float)l % 24;
            return filling;
        }

        #endregion Methods
    }
}
