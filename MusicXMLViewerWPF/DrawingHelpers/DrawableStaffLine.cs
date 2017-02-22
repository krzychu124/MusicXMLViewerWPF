using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.DrawingHelpers
{
    class DrawableStaffLine : IDrawableObjectPart
    {
        private Brush color = Brushes.Black;
        private double width;
        private Point position = new Point();
        private MeasureLineCount measureLines = MeasureLineCount.five;
        private string symbol = "*";
        private DrawingVisual visualObject;
        private PageProperties pp;
        public DrawableStaffLine(PageProperties pp, double width, MeasureLineCount lineCount = MeasureLineCount.five, Point offsetPoint = new Point(), Brush color = null)
        {
            this.pp = pp;
            StaffLineWidth = width;
            Position = offsetPoint;
            Color = color != null ? color : Brushes.Black;
            MeasureLines = lineCount;
            Draw();
            //DrawingVisual dv_test1 = new DrawingVisual();
            //DrawingHelpers.DrawingMethods.DrawCharacterGlyph(dv_test1, new Point(40, 0), 70);
            //DrawingVisual rect1 = new DrawingVisual();
            //Rect r1 = dv_test1.DescendantBounds;
            //MusicXMLViewerWPF.Misc.DrawingHelpers.DrawRectangle(rect1, r1);
            //visualObject.Children.Add(dv_test1);
            //visualObject.Children.Add(rect1);
            //DrawingVisual dv_test2 = new DrawingVisual();
            //int index = (int)'\uE062';
            //GlyphTypeface glyph;
            //GlyphTypeface glyphTypeface = Helpers.TypeFaces.BravuraMusicFont.TryGetGlyphTypeface(out glyph) ? glyph : null;
            ////! charactertoGlyphFace;
            //ushort glyphindex;
            //glyph.CharacterToGlyphMap.TryGetValue(index, out glyphindex);
            //DrawingHelpers.DrawingMethods.DrawCharacterGlyph(dv_test2, new Point(0, 0), glyphindex);
            //DrawingVisual rect2 = new DrawingVisual();
            //Rect r2 = dv_test2.DescendantBounds;
            //MusicXMLViewerWPF.Misc.DrawingHelpers.DrawRectangle(rect2, r2, Brushes.Red);
            //visualObject.Children.Add(dv_test2);
            //visualObject.Children.Add(rect2);
        }

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

        public DrawingVisual PartialObjectVisual
        {
            get
            {
                return visualObject;
            }
        }

        public void Draw()
        {
            visualObject = new DrawingVisual();
            using(DrawingContext dc = visualObject.RenderOpen())
            {
                GenerateGenericStaffLine(dc);
            }
        }
        /// <summary>
        /// Deprecated, use GenerateGenericStaffLine()
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="StartPoint"></param>
        /// <param name="color"></param>
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
        
        private void GenerateGenericStaffLine(DrawingContext dc)//scale dependent
        {
            Brush color = Brushes.Black;
            double factor = PageProperties.PxPerMM(); // scalefactor 1mm to px
            Pen pen = new Pen(color, pp.TenthToPx(1.4583)); //todo getthickness from loaded file
            double t = pp.StaffSpace * factor;
            int Lines = (int)measureLines;//5;
            int currentLineIndex = 0;
            Point shiftedPosition = GetCenteredStaffPosition(position); //! move y position to center staffline while stafflines < 5
            
            Point startPosition = shiftedPosition;
            Point endPosition = new Point(StaffLineWidth, shiftedPosition.Y);
            while (currentLineIndex < Lines)
            {
                dc.DrawLine(pen, startPosition, endPosition);
                startPosition = new Point(startPosition.X, startPosition.Y + t);
                endPosition = new Point(StaffLineWidth, endPosition.Y + t);
                ++currentLineIndex;
            }
        }
        private Point GetCenteredStaffPosition(Point pointToShift)
        {
            double shift = (pp.StaffHeight - (((int)measureLines - 1)* pp.StaffSpace))/ 2;

            return new Point(pointToShift.X, pointToShift.Y + shift *PageProperties.PxPerMM());
        }
        /// <summary>
        /// Debug only... Vertical line from top to bottom of measure staff
        /// </summary>
        /// <param name="dc"></param>
        private void DebugTestLine(DrawingContext dc) 
        {
            //! vertical line with length of staffLine Height
            Pen pen = new Pen(Brushes.BlueViolet, 2);
            Point p = new Point(0, 0 + (pp.StaffHeight* PageProperties.PxPerMM()));
            dc.DrawLine(pen, new Point(0, 0), p);
            
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
    }
    enum MeasureLineCount
    {
        one = 1,
        two = 2,
        three = 3,
        four = 4,
        five = 5,
        six = 6
    }
}
