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
        private BarlineMusicXML barline;
        private MeasureDrawing dm;

        private CanvasList baseObjectVisual;
        private bool invalidated = true;
        public bool Invalidated { get { return invalidated; } set { invalidated = value; } }
        public CanvasList BaseObjectVisual
        {
            get
            {
                return baseObjectVisual;
            }
        }

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

        //public BarlineVisualObject(Barline barline)
        //{
        //    this.Barline = barline;
        //}

        public BarlineVisualObject(MeasureDrawing dm, BarlineMusicXML barline)
        {
            this.Barline = barline;
            this.dm = dm;
            Draw();
        }

        private void Draw()
        {
            baseObjectVisual = new CanvasList(4, dm.PageProperties.StaffHeight * PageProperties.PxPerMM());
            //baseObjectVisual.Tag = Barline.ID;
            DrawBarline();
        }

        private void DrawBarline()
        {
            Point pos = new Point(dm.MeasureWidth, 0);
            DrawingVisualPlus dvp = new DrawingVisualPlus();
            
            double barlineThicknes = dm.PageProperties.TenthToPx(1.4583);
            pos.X -= barlineThicknes * 0.5;// * 2.5;
            Point startPoint = pos;
            Point endPoint = new Point(pos.X, +BaseObjectVisual.Height);
            Pen pen = new Pen(Brushes.Black, barlineThicknes); // thin 0.7487 thick 5
            using (DrawingContext dc = dvp.RenderOpen())
            {
                dc.DrawLine(pen, startPoint, endPoint);

            }
            baseObjectVisual.AddVisual(dvp);
        }
        public void InvalidateVisualObject()
        {
            Invalidated = true;
        }
    }
}
