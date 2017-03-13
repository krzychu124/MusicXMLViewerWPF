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
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Model.MeasureItems.Attributes;

namespace MusicXMLScore.DrawingHelpers
{
    class MeasureDrawing : IDrawableObjectBase
    {

        #region Fields

        private List<BarlineVisualObject> barlineVisuals;
        private bool clefVisible = false;
        private List<DirectionVisualObject> directionVisuals;
        private string id;
        private bool invalidated = true;
        private bool keyVisible = false;
        private LayoutControl.LayoutGeneral layout;
        private Measure measure;
        private double measureHeight = 60.0;
        private bool measureNumberVisible = false;
        private ScorePartwisePartMeasureMusicXML measureSerializable;
        private double measureWidth = 0.0;
        private List<NoteVisualObject> notesVisuals;
        private PageProperties pageProperies;
        private string partId;
        private Size size;
        private double[] staffLinesCoords;
        private uint stavesCount = 1;
        private double stavesDistance = 0.0;
        private bool timeSigVisible = false;
        private CanvasList visualObject;
        private Dictionary<int, double[]> staffLinesYpositions = new Dictionary<int, double[]>();
        private Dictionary<int, double> avaliableIndexLinePositions = new Dictionary<int, double>();
        #endregion Fields

        #region Constructors

        public MeasureDrawing()
        {
            this.measure = new Measure();
        }

        //public MeasureDrawing(Measure measure)
        //{
        //    MainWindowViewModel mwvm = ViewModelLocator.Instance.Main;
        //    pageProperies = (PageProperties)ViewModel.ViewModelLocator.Instance.Main.CurrentPageProperties;
        //    this.measure = measure;
        //    PagesControllerViewModel pcvm = mwvm.SelectedTabItem.DataContext as PagesControllerViewModel ;
        //    stavesCount = (uint)pcvm.MusicScore.Parts.ElementAt(0).Value.StavesCount;
        //    measureHeight = pageProperies.StaffHeight.MMToWPFUnit();
        //    DrawAttributes();
        //    CreateVisualObject();
        //}
        public MeasureDrawing(ScorePartwisePartMeasureMusicXML measure, LayoutControl.LayoutGeneral layout, string id)
        {
            partId = id;
            this.id = measure.Number;
            measureWidth = measure.CalculatedWidth;
            measureHeight = ViewModelLocator.Instance.Main.CurrentPageProperties.StaffHeight.MMToWPFUnit();
            this.layout = layout;
            //PrimitiveRectangle();
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
            CreateStaffLine();
            DrawAttributes();
        }

        

        #endregion Constructors

        #region Properties

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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Invalidate VisualObject and update
        /// </summary>
        public void InvalidateVisualObject()
        {
            Invalidated = true;
        }

        /// <summary>
        /// Add Barlines to measure
        /// </summary>
        private void AddBarlines()
        {
            barlineVisuals = new List<BarlineVisualObject>();
            Model.MeasureItems.BarlineMusicXML bar = new Model.MeasureItems.BarlineMusicXML();
            BarlineVisualObject barline = new BarlineVisualObject(this, bar, size.Height);
            barlineVisuals.Add(barline);
            visualObject.AddVisual(barline.BaseObjectVisual);
        }

        /// <summary>
        /// Add Direction to measure content
        /// </summary>
        private void AddDirection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add note to measure content
        /// </summary>
        private void AddNote()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds loaded drawable object to main canvas visual
        /// </summary>
        private void ArrangeMeasureLayout()
        {
            Invalidated = false;
        }

        private void CreateStaffLine()
        {

            CanvasList measureCanvas = new CanvasList(measureWidth, measureHeight);
            Point p = new Point(0, layout.PageProperties.StaffHeight.MMToWPFUnit());
            staffLinesCoords = new double[stavesCount];
            staffLinesYpositions = new Dictionary<int, double[]>();
            for (uint i = 0; i < stavesCount; i++)
            {
                p.Y += (stavesDistance.TenthsToWPFUnit() + layout.PageProperties.StaffHeight.MMToWPFUnit()) * i;
                DrawableStaffLine staff = new DrawableStaffLine(layout.PageProperties, measureWidth, offsetPoint: p);
                staffLinesYpositions.Add((int)i+1, staff.LinesYpositions);
                staffLinesCoords[i] = p.Y;
                measureCanvas.AddVisual(staff.PartialObjectVisual);
            }
            visualObject.AddVisual(measureCanvas);
            visualObject.SetToolTipText($"measure {id}, {partId} width: {measureWidth}");
        }

        private void CreateVisualObject()
        {
            CreateStaffLine();
            CreateVisualObjectChilds();
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

        private void DrawAttributes()
        {
            if (clefVisible)
            {

                //LayoutControl.SegmentPanel spanel = new LayoutControl.SegmentPanel(partId);
                //spanel.AddAttributesContainer(new LayoutControl.SegmentPanelContainers.MeasureAttributesContainer(measureSerializable.Items.OfType<AttributesMusicXML>().FirstOrDefault()));
                ////ClefMusicXML clef = measureSerializable.Items.OfType<AttributesMusicXML>().FirstOrDefault().Clef.ElementAt(0);
                ////int clefNumber = clef.Number != null? int.Parse(clef.Number) : 1;
                ////ClefVisualObject clefVisual = new ClefVisualObject(clef, staffLinesYpositions[clefNumber]);
                ////visualObject.AddVisual(clefVisual.BaseObjectVisual);
                //visualObject.Children.Add(spanel);
            }
            if (keyVisible)
            {

            }
            if (timeSigVisible)
            {
                //TimeMusicXML timeSig = measureSerializable.Items.OfType<AttributesMusicXML>().FirstOrDefault().Time.ElementAt(0);
                //int timeSigNumber = timeSig.Number != null ? int.Parse(timeSig.Number) : 1;
                //TimeSignatureVisualObject timeSigVisual = new TimeSignatureVisualObject(timeSig, staffLinesYpositions[timeSigNumber]);
                //visualObject.AddVisual(timeSigVisual.BaseObjectVisual);
            }
        }

        private void GetMeasureProperties()
        {
            measureSerializable = ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(partId.GetPartIdIndex()).MeasuresByNumber[id];
            if (measureSerializable.Items.OfType<AttributesMusicXML>().Count() != 0)
            {
                var attributes = measureSerializable.Items.OfType<AttributesMusicXML>().FirstOrDefault();
                clefVisible = attributes.Clef != null ? attributes.Clef.Count != 0 ? true : false : false;
                timeSigVisible = attributes.Time != null ? attributes.Time.Count != 0 ? true : false : false;
                if (attributes.StaffDetails != null)
                {
                    //attributes.StaffDetails.Select(i=> i).Where(i=> i) //TODO_LATER implement staffDetails class
                }
            }
            measureHeight = layout.PageProperties.StaffHeight.MMToWPFUnit() * stavesCount + (stavesDistance.TenthsToWPFUnit() * (stavesCount - 1));
            measureWidth = measureSerializable.CalculatedWidth.TenthsToWPFUnit();
            size = new Size(measureWidth, measureHeight);
            visualObject = new CanvasList(measureWidth, measureHeight);
        }
        //private void PrimitiveRectangle()
        //{
            

        //    Point p = new Point(0, -layout.PageProperties.StaffHeight.MMToWPFUnit());
        //    for (uint i = 0; i < 1/*stavesCount*/; i++)
        //    {
        //        CanvasList MeasureCanvas = new CanvasList(measureWidth, measureHeight);
        //        p.Y = p.Y + (stavesDistance.TenthsToWPFUnit()) * i;
        //        Point l_t = new Point(p.X, p.Y);
        //        Point r_b = new Point(p.X + measureWidth, p.Y + measureHeight);
        //        Rect primitive = new Rect(l_t, r_b);
        //        DrawingVisual rectVis = new DrawingVisual();
        //        using (DrawingContext dc = rectVis.RenderOpen())
        //        {
        //            Brush color = Helpers.DrawingHelpers.PickBrush();
        //            dc.DrawRectangle(color, new Pen(color, 1), primitive);
        //        }
        //        MeasureCanvas.AddVisual(rectVis);
        //        visualObject.AddVisual(MeasureCanvas);
        //    }
        //    visualObject.SetToolTipText($"measure {id}, {partId} width: {measureWidth}");
        //}
        /// <summary>
        /// Updates VisualObject - currently whole object is cleared and instatiated again from scratch
        /// </summary>
        private void UpdateVisualObject()
        {
            CreateVisualObject();//temp
        }

        #endregion Methods
    }
}
