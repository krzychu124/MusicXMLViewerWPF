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

namespace MusicXMLScore.DrawingHelpers
{
    class DrawableMeasure : IDrawableObjectBase
    {

        #region Private Fields

        private bool invalidated = true;
        private Measure measure;
        private double measureHeight = 60.0;
        private double measureWidth = 0.0;
        private uint stavesCount =1;
        private double stavesTopMargin = 1.3;
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
        #endregion Private Fields

        #region Public Constructors
        public DrawableMeasure()
        {
            this.measure = new Measure();
        }

        public DrawableMeasure(Measure measure)
        {
            MainWindowViewModel mwvm = ViewModelLocator.Instance.Main;
            pageProperies = (PageProperties)ViewModel.ViewModelLocator.Instance.Main.CurrentPageProperties;
            this.measure = measure;
            PagesControllerViewModel pcvm = mwvm.SelectedTabItem.DataContext as PagesControllerViewModel ;
            stavesCount = (uint)pcvm.MusicScore.Parts.ElementAt(0).Value.StavesCount;
            measureHeight = measureHeight * stavesCount;
            DrawAttributes();
            CreateVisualObject();
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

        public double MeasureWidth { get { return measure.Width; } }
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
            
            CreateStaffLine();
            CreateVisualObjectChilds();
        }

        private void CreateStaffLine()
        {
            CanvasList MeasureCanvas = new CanvasList(measureWidth,measureHeight);
            Point p = new Point();
            staffLinesCoords = new Point[stavesCount];
            for (uint i = 0; i < stavesCount; i++)
            {
                p.Y = p.Y + (60 * stavesTopMargin) * i;
                DrawableStaffLine staff = new DrawableStaffLine(this.pageProperies, measureWidth, offsetPoint: p);
                staffLinesCoords[i] = p;
                MeasureCanvas.AddVisual(staff.PartialObjectVisual);
            }
            visualObject = MeasureCanvas;
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
            foreach (var item in measure.Barlines)
            {
                BarlineVisualObject barline = new BarlineVisualObject(this, item);
                barlineVisuals.Add(barline);
            }
            foreach (var item in barlineVisuals)
            {
                //visualObject.AddVisual(item.BaseObjectVisual);
            }
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
