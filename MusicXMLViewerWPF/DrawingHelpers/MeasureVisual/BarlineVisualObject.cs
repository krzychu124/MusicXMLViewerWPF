using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Helpers;
using MusicXMLViewerWPF;
using System.Windows.Media;
using System.Windows;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Converters;

namespace MusicXMLScore.DrawingHelpers.MeasureVisual
{
    class BarlineVisualObject : IDrawableObjectBase
    {
        #region Fields

        private BarlineMusicXML barline;
        private CanvasList baseObjectVisual;
        private MeasureDrawing dm;
        private bool invalidated = true;

        #endregion Fields

        #region Constructors

        public BarlineVisualObject(MeasureDrawing dm, BarlineMusicXML barline, double measureHeight)
        {
            this.Height = measureHeight;
            this.Barline = barline;
            this.dm = dm;
            Draw();
        }

        #endregion Constructors

        #region Properties

        public CanvasList BaseObjectVisual
        {
            get
            {
                return baseObjectVisual;
            }
        }

        public double Height { get; private set; }
        public bool Invalidated { get { return invalidated; } set { invalidated = value; } }
        internal BarlineMusicXML Barline
        {
            get
            {
                return barline;
            }

            set
            {
                barline = value;
            }
        }

        #endregion Properties

        #region Methods

        public void InvalidateVisualObject()
        {
            Invalidated = true;
        }

        private void Draw()
        {
            baseObjectVisual = new CanvasList(4, Height);
            DrawBarline();
        }

        private void DrawBarline()
        {
            Point pos = new Point(dm.MeasureWidth, 0);
            DrawingVisualPlus dvp = new DrawingVisualPlus();
            double barlineThicknes = dm.PageProperties.TenthToPx(1.4583);
            pos.X -= barlineThicknes * 0.5;
            Point startPoint = pos;
            Point endPoint = new Point(pos.X, +BaseObjectVisual.Height);
            Pen pen = new Pen(Brushes.Black, barlineThicknes); // thin 0.7487 thick 5 in thenths ofc... ;)
            using (DrawingContext dc = dvp.RenderOpen())
            {
                dc.DrawLine(pen, startPoint, endPoint);
            }
            baseObjectVisual.AddVisual(dvp);
        }

        #endregion Methods
    }
}
