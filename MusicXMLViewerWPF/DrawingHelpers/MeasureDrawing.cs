using MusicXMLViewerWPF.ScoreParts.MeasureContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MusicXMLScore.Helpers;
using MusicXMLScore.DrawingHelpers.MeasureVisual;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.ViewModel;
using MusicXMLScore.Model;
using MusicXMLScore.Converters;

namespace MusicXMLScore.DrawingHelpers
{
    class MeasureDrawing : IDrawableObjectBase
    {

        #region Private Fields

        private bool invalidated = true;
        private Measure measure;
        private double measureHeight = 60.0;
        private double measureWidth = 0.0;
        private uint stavesCount =1;
        private double stavesTopMargin = 1.4;
        private CanvasList visualObject;
        private List<BarlineVisualObject> barlineVisuals;
        private List<DirectionVisualObject> directionVisuals;
        private List<NoteVisualObject> notesVisuals;
        private PageProperties pageProperies;
        private Point[] staffLinesCoords;
        #region Visability
        private bool measureNumberVisible = false;
        private bool clefVisible = false;
        private bool keyVisible = false;
        private bool timeSigVisible = false;
        #endregion
        #region Experimental
        LayoutControl.LayoutGeneral layout;
        string id;
        string partId;
        Size size;
        ScorePartwisePartMeasureMusicXML measureSerializable;
        double stavesDistance = 0.0;
        #endregion
        #endregion Private Fields

        #region Public Constructors
        public MeasureDrawing()
        {
            this.measure = new Measure();
        }

        public MeasureDrawing(Measure measure)
        {
            MainWindowViewModel mwvm = ViewModelLocator.Instance.Main;
            pageProperies = (PageProperties)ViewModel.ViewModelLocator.Instance.Main.CurrentPageProperties;
            this.measure = measure;
            PagesControllerViewModel pcvm = mwvm.SelectedTabItem.DataContext as PagesControllerViewModel ;
            //stavesCount = (uint)pcvm.MusicScore.Parts.ElementAt(0).Value.StavesCount;
            measureHeight = pageProperies.StaffHeight.MMToWPFUnit();
            //DrawAttributes();
            CreateVisualObject();
        }
        public MeasureDrawing(ScorePartwisePartMeasureMusicXML measure, LayoutControl.LayoutGeneral layout, string id)
        {
            partId = id;
            this.id = measure.Number;
            measureWidth = measure.Width;
            measureHeight = ViewModelLocator.Instance.Main.CurrentPageProperties.StaffHeight.MMToWPFUnit();
            this.layout = layout;
            PrimitiveRectangle();
            CreateVisualObjectChilds();
        }
        public MeasureDrawing(string measureId, string partId, double staffDistance, int stavesCount)
        {
            stavesDistance = staffDistance;
            this.stavesCount = (uint)stavesCount;
            layout = ViewModelLocator.Instance.Main.CurrentTabLayout;
            pageProperies = layout.PageProperties;
            this.id = measureId;
            this.partId = partId;
            GetMeasureProperties();
            //PrimitiveRectangle();
            CreateStaffLine();
        }

        private void GetMeasureProperties()
        {
            measureSerializable = ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(partId.GetPartIdIndex()).MeasuresByNumber[id];
            measureHeight = layout.PageProperties.StaffHeight.MMToWPFUnit() * stavesCount + (stavesDistance.TenthsToWPFUnit() * (stavesCount - 1));
            measureWidth = measureSerializable.CalculatedWidth.TenthsToWPFUnit();
            size = new Size(measureWidth, measureHeight);
            visualObject = new CanvasList(measureWidth, measureHeight);
        }

        #endregion Public Constructors

        #region Public Properties

        public CanvasList BaseObjectVisual
        {
            get
            {
                if (Invalidated)
                {
                    UpdateVisualObject();
                }
                return visualObject;
            }
        }

        public bool Invalidated { get { return invalidated; } private set { invalidated = value; } }

        public double MeasureWidth { get { return measureWidth; } }
        public int Number
        {
            get
            {
                return measure.Number;
            }
            set
            {
                measure.Number = value;
            }
        }

        public PageProperties PageProperties
        {
            get { return pageProperies; }
        }
        #endregion Public Properties

        #region Private Methods

        private void CreateVisualObject()
        {
            //
            PrimitiveRectangle();
            CreateStaffLine();
            CreateVisualObjectChilds();
        }

        private void CreateStaffLine()
        {
            
            CanvasList measureCanvas = new CanvasList(measureWidth,measureHeight);
            Point p = new Point();
            staffLinesCoords = new Point[stavesCount];
            for (uint i = 0; i < stavesCount; i++)
            {
                p.Y += (stavesDistance.TenthsToWPFUnit() + layout.PageProperties.StaffHeight.MMToWPFUnit()) * i;
                DrawableStaffLine staff = new DrawableStaffLine(layout.PageProperties, measureWidth, offsetPoint: p);
                staffLinesCoords[i] = p;
                measureCanvas.AddVisual(staff.PartialObjectVisual);
            }
            visualObject.AddVisual(measureCanvas);
        }

        private void DrawAttributes()
        {
            measureWidth = measure.Width;
            if (clefVisible)
            {

            }
            if (keyVisible)
            {

            }
            if (timeSigVisible)
            {

            }
            //stavesCount = measure.Attributes.Staves;
            
        }
        private void PrimitiveRectangle()
        {
            

            Point p = new Point(0, -layout.PageProperties.StaffHeight.MMToWPFUnit());
            for (uint i = 0; i < 1/*stavesCount*/; i++)
            {
                CanvasList MeasureCanvas = new CanvasList(measureWidth, measureHeight);
                p.Y = p.Y + (stavesDistance.TenthsToWPFUnit()) * i;
                Point l_t = new Point(p.X, p.Y);
                Point r_b = new Point(p.X + measureWidth, p.Y + measureHeight);
                Rect primitive = new Rect(l_t, r_b);
                DrawingVisual rectVis = new DrawingVisual();
                using (DrawingContext dc = rectVis.RenderOpen())
                {
                    Brush color = Helpers.DrawingHelpers.PickBrush();
                    dc.DrawRectangle(color, new Pen(color, 1), primitive);
                }
                MeasureCanvas.AddVisual(rectVis);
                visualObject.AddVisual(MeasureCanvas);
            }
            visualObject.SetToolTipText($"measure {id}, {partId} width: {measureWidth}");
        }


        /// <summary>
        /// Adds content from measure object to current measure
        /// </summary>
        private void CreateVisualObjectChilds()
        {
            //AddNotes();
            //AddDirections();
            AddBarlines();

            ArrangeMeasureLayout();
        }
        /// <summary>
        /// Adds loaded drawable object to main canvas visual
        /// </summary>
        private void ArrangeMeasureLayout()
        {
            Invalidated = false;
        }
        /// <summary>
        /// Add note to measure content
        /// </summary>
        private void AddNote()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Add Direction to measure content
        /// </summary>
        private void AddDirection()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Add Barlines to measure
        /// </summary>
        private void AddBarlines()
        {
            barlineVisuals = new List<BarlineVisualObject>();
            MusicXMLScore.Model.MeasureItems.BarlineMusicXML bar = new Model.MeasureItems.BarlineMusicXML();
                BarlineVisualObject barline = new BarlineVisualObject(this, bar);
                barlineVisuals.Add(barline);
                visualObject.AddVisual(barline.BaseObjectVisual);
            
        }
        /// <summary>
        /// Updates VisualObject - currently whole object is cleared and instatiated again from scratch
        /// </summary>
        private void UpdateVisualObject()
        {
            CreateVisualObject();//temp
        }
        /// <summary>
        /// Invalidate VisualObject and update
        /// </summary>
        public void InvalidateVisualObject()
        {
            Invalidated = true;
        }

        #endregion Private Methods

    }
}
