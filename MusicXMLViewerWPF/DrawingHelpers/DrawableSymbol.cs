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
    class DrawableSymbol : IDrawable
    {
        #region Private Fields

        private DrawingVisual drawingVisual = new DrawingVisual();
        private Point point = new Point();
        private double size = 0.0;
        private string symbol = "";

        #endregion Private Fields

        #region Public Constructors

        public DrawableSymbol(string symbol, Point point = new Point(), Brush colorAsBrush = null)
        {
            Symbol = symbol;
            Point = point;
            Color = colorAsBrush != null ? colorAsBrush : Brushes.Black;
        }

        #endregion Public Constructors

        #region Public Properties

        public Brush Color
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public DrawingVisual DrawingVisual
        {
            get
            {
                return drawingVisual;
            }

            set
            {
                drawingVisual = value;
            }
        }

        public Point Point
        {
            get
            {
                return point;
            }

            set
            {
                point = value;
            }
        }

        public double Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        public string Symbol
        {
            get
            {
                return symbol;
            }

            set
            {
                symbol = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void Draw()
        {
            using(DrawingContext dc = DrawingVisual.RenderOpen())
            {
                dc.DrawText(PrepareSymbol(), Point);
            }
        }
        public DrawingVisual GetDrawnObject()
        {
            return DrawingVisual;
        }

        #endregion Public Methods

        #region Private Methods

        private FormattedText PrepareSymbol()
        {
            FormattedText ft = new FormattedText(Symbol, System.Threading.Thread.CurrentThread.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, TypeFaces.BravuraTextFont, Size, Color);
            return ft;
        }

        #endregion Private Methods
    }
}
