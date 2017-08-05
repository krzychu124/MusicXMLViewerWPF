using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MusicXMLScore.Helpers;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.ViewModel;
using MusicXMLScore.Model;
using MusicXMLScore.Converters;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.Model.MeasureItems.Attributes;
using System.ComponentModel;

namespace MusicXMLScore.DrawingHelpers
{
    class MeasureDrawing : INotifyPropertyChanged
    {
        //TODO Refactor and move to MeasureSegmentController
        #region Fields

        private string id;
        private bool invalidated = false;
        private LayoutControl.LayoutGeneral layout;
        private double measureHeight = 60.0;
        private ScorePartwisePartMeasureMusicXML measureSerializable;
        private double measureWidth = 0.0;
        private PageProperties pageProperies;
        private string partId;
        private Size size;
        private double[] staffLinesCoords;
        private uint stavesCount = 1;
        private double stavesDistance = 0.0;
        private DrawingVisualHost visualObject;
        private Dictionary<int, double[]> staffLinesYpositions = new Dictionary<int, double[]>();
        private Dictionary<int, double> avaliableIndexLinePositions = new Dictionary<int, double>();

        //! test
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion Fields

        #region Constructors

        public MeasureDrawing(string measureId, string partId, double staffDistance, int stavesCount)
        {
            //PropertyChanged += OnWidthChanged;
            stavesDistance = staffDistance;
            this.stavesCount = (uint)stavesCount;
            layout = ViewModelLocator.Instance.Main.CurrentLayout;
            pageProperies = layout.PageProperties;
            this.id = measureId;
            this.partId = partId;

            GetMeasureProperties();
            //! CreateStaffLine();
            CreateVisualObject();
        }

        #endregion Constructors

        #region Properties

        public DrawingVisualHost BaseObjectVisual
        {
            get
            {
                if (Invalidated)
                {
                    UpdateVisualObject();
                }
                return visualObject;
            }
            private set
            {
                visualObject = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(BaseObjectVisual)));
            }
        }

        public bool Invalidated { get { return invalidated; } private set { invalidated = value; } }

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
        /// Adds loaded drawable object to main canvas visual
        /// </summary>
        private void ArrangeMeasureLayout()
        {
            Invalidated = false;
        }

        private void CreateStaffLine()
        {
            Point p = new Point(0, layout.PageProperties.StaffHeight.MMToWPFUnit());
            staffLinesCoords = new double[stavesCount];
            staffLinesYpositions = new Dictionary<int, double[]>();
            if (visualObject.Count != 0)
            {
                visualObject.ClearVisuals();
            }
            for (uint i = 0; i < stavesCount; i++)
            {
                p.Y += (stavesDistance.TenthsToWPFUnit() + layout.PageProperties.StaffHeight.MMToWPFUnit()) * i;
                DrawableStaffLine staff = new DrawableStaffLine(layout.PageProperties, measureWidth, offsetPoint: p);
                staffLinesYpositions.Add((int)i+1, staff.LinesYpositions);
                staffLinesCoords[i] = p.Y;
                visualObject.AddVisual(staff.PartialObjectVisual);
            }
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
            ArrangeMeasureLayout();
        }

        private void GetMeasureProperties()
        {
            visualObject = new DrawingVisualHost();
            measureSerializable = ViewModelLocator.Instance.Main.CurrentSelectedScore.Part[partId.GetPartIdIndex()].Measure.FirstOrDefault(x=>x.Number ==id);
            measureSerializable.PropertyChanged += OnWidthChanged;

            measureHeight = layout.PageProperties.StaffHeight.MMToWPFUnit() * stavesCount + (stavesDistance.TenthsToWPFUnit() * (stavesCount - 1));
            measureWidth = measureSerializable.CalculatedWidth.TenthsToWPFUnit();

            size = new Size(measureWidth, measureHeight);
        }

        /// <summary>
        /// Updates VisualObject - currently whole object is cleared and instatiated again from scratch
        /// </summary>
        private void UpdateVisualObject()
        {
            CreateVisualObject();
        }

        private void OnWidthChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScorePartwisePartMeasureMusicXML.CalculatedWidth))
            {
                //! Update staff Line visual if measure width changed
                ScorePartwisePartMeasureMusicXML m = sender as ScorePartwisePartMeasureMusicXML;
                measureWidth = m.CalculatedWidth.TenthsToWPFUnit();
                UpdateVisualObject();
            }
        }
        #endregion Methods
    }
}
