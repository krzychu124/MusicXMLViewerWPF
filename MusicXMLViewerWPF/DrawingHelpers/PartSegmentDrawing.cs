using MusicXMLScore.Helpers;
using System;
using System.Collections.Generic;
using MusicXMLScore.Converters;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicXMLScore.DrawingHelpers
{
    public class PartSegmentDrawing
    {
        private Canvas partSegmentCanvas;
        private Point position;
        private PartProperties partProperties;
        private List<string> measuresList;
        private Dictionary<string, MeasureDrawing> measuresObjects;
        private string partId;
        private Size size;
        private int stavesCount = 1;
        private double staffDistance = 0.0;
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

        public PartSegmentDrawing(List<string> measuresList, string partId, PartProperties partProperites)
        {
            this.measuresList = measuresList;
            this.partId = partId;
            this.partProperties = partProperites;
            stavesCount = partProperties.NumberOfStaves;
            staffDistance = partProperites.StavesDistance;
            CalculateDimensions();
        }

        private void CalculateDimensions()
        {
            double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.StaffHeight.MMToWPFUnit();
            //staffDistance = partProperties.StaffLayout.ElementAt(measuresList.ElementAt(0).GetMeasureIdIndex()).StaffDistance.TenthsToWPFUnit();
            double segmentHeight = (stavesCount * staffHeight) + ((stavesCount-1)* staffDistance);
            double segmentWidth = measuresList.CalculateWidth(partId);
            partSegmentCanvas = new Canvas() { Width = segmentWidth, Height = segmentHeight };
            size = new Size(segmentWidth, segmentHeight);
        }

        public Size GenerateContent()
        {
            foreach (var measureId in measuresList)
            {
                MeasureDrawing measureCanvas = new MeasureDrawing(measureId, partId, staffDistance, stavesCount);
                Canvas.SetTop(measureCanvas.BaseObjectVisual, 0/*size.Height*/);
                Canvas.SetLeft(measureCanvas.BaseObjectVisual, partProperties.Coords[measureId].X);
                PartSegmentCanvas.Children.Add(measureCanvas.BaseObjectVisual);
            }
            return size;
        }
    }
}
