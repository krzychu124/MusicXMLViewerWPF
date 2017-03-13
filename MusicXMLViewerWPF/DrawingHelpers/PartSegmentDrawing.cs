﻿using MusicXMLScore.Converters;
using MusicXMLScore.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.DrawingHelpers
{
    public class PartSegmentDrawing
    {
        #region Fields

        private List<string> measuresList;
        private string partId;
        private PartProperties partProperties;
        private Canvas partSegmentCanvas;
        private Size size;
        private double staffDistance = 0.0;
        private int stavesCount = 1;

        #endregion Fields

        #region Constructors

        public PartSegmentDrawing(List<string> measuresList, string partId, PartProperties partProperites)
        {
            this.measuresList = measuresList;
            this.partId = partId;
            this.partProperties = partProperites;
            stavesCount = partProperties.NumberOfStaves;
            staffDistance = partProperites.StavesDistance;
            CalculateDimensions();
        }

        #endregion Constructors

        #region Properties

        public string PartId
        {
            get
            {
                return partId;
            }

            set
            {
                partId = value;
            }
        }

        /// <summary>
        /// Part size in Tenths, convert to WPFUnit when using for drawing dimensions
        /// </summary>
        public Size Size
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

        internal Canvas PartSegmentCanvas
        {
            get
            {
                return partSegmentCanvas;
            }

            set
            {
                partSegmentCanvas = value;
            }
        }

        #endregion Properties

        #region Methods

        public Size GenerateContent()
        {
            foreach (var measureId in measuresList)
            {
                MeasureDrawing measureCanvas = new MeasureDrawing(measureId, partId, staffDistance, stavesCount);
                ScorePartwisePartMeasureMusicXML measureSerializable = ViewModel.ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(partId.GetPartIdIndex()).MeasuresByNumber[measureId];
                LayoutControl.MeasureSegmentController measureSegment = new LayoutControl.MeasureSegmentController(measureSerializable, partId);
                LayoutControl.SegmentPanel spanel = measureSegment.GetContentPanel();
                spanel.AddAttributesContainer(new LayoutControl.SegmentPanelContainers.MeasureAttributesContainer(measureSerializable.Items.OfType<Model.MeasureItems.AttributesMusicXML>().FirstOrDefault()));

                Canvas.SetTop(spanel, 0/*size.Height*/);
                Canvas.SetLeft(spanel, partProperties.Coords[measureId].X);
                PartSegmentCanvas.Children.Add(spanel);
                Canvas.SetTop(measureCanvas.BaseObjectVisual, 0/*size.Height*/);
                Canvas.SetLeft(measureCanvas.BaseObjectVisual, partProperties.Coords[measureId].X);
                PartSegmentCanvas.Children.Add(measureCanvas.BaseObjectVisual);
            }
            return size;
        }

        private void CalculateDimensions()
        {
            double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.PageProperties.StaffHeight.MMToWPFUnit();
            double segmentHeight = (stavesCount * staffHeight) + ((stavesCount - 1) * staffDistance);
            double segmentWidth = measuresList.CalculateWidth(partId);
            partSegmentCanvas = new Canvas() { Width = segmentWidth, Height = segmentHeight };
            size = new Size(segmentWidth, segmentHeight);
        }

        #endregion Methods
    }
}