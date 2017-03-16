using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.LayoutControl.SegmentPanelContainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.LayoutControl
{
    class SegmentPanel : Panel
    {
        private Dictionary<int, MeasureAttributesContainer> attributesContainer;
        private Dictionary<int, MeasureNotesContainer> notesContainer;
        private double panelWidth = 0.0;
        private double panelHeight = 0.0;
        private Dictionary<int, double> stavesLines;
        private int staves = 1;
        private double defaultStavesDistance = 0.0;
        private LayoutGeneral layoutProperties;
        private string partID;
        private Dictionary<int, double> staffDistances = new Dictionary<int, double>() { { 1, 0.0 } };
        private PartProperties partProperties;
        private int systemIndex = 0;
        public SegmentPanel(string partID, int systemIndex)
        {
            this.partID = partID;
            this.systemIndex = systemIndex;
            partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[this.partID];
            staves = partProperties.NumberOfStaves;
            defaultStavesDistance = partProperties.StaffLayoutPerPage.ElementAt(0).ElementAt(systemIndex).StaffDistance;//? StavesDistance;
            SetHeight();
        }
        public void AddAttributesContainer(MeasureAttributesContainer measureAttributes, int numberOfStave = 1)
        {
            measureAttributes.Tag = numberOfStave.ToString();
            Canvas.SetLeft(measureAttributes, (3.0).TenthsToWPFUnit()); //adding left offset, refactor to %of staffline height, or add to each child element
            if (numberOfStave != 1)//works property if number of staves is 2, higher nuber will overlap with 2nd. stave, WiP
            {
                double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.PageProperties.StaffHeight.MMToTenths();
                Canvas.SetTop(measureAttributes, staffHeight + defaultStavesDistance.TenthsToWPFUnit());
            }
            Children.Add(measureAttributes);
            attributesContainer = new Dictionary<int, MeasureAttributesContainer>();
            attributesContainer.Add(numberOfStave, measureAttributes);
        }
        private void SetHeight()
        {
            panelHeight = partProperties.PartHeight.TenthsToWPFUnit();
            panelWidth = 40;
            Width = panelWidth;
            Height = panelHeight;
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            Size panelSize = availableSize;
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(availableSize);
            }
            return panelSize;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = new Size(panelWidth, panelHeight);
            foreach (UIElement child in InternalChildren)
            {
                double top = double.IsNaN(Canvas.GetTop(child)) ? 0.0 : Canvas.GetTop(child);
                double left = double.IsNaN(Canvas.GetLeft(child)) ? 0.0 : Canvas.GetLeft(child);
                child.Arrange(new Rect(left, top, size.Width, size.Height));
                size.Width += panelWidth;
            }
            return size;
        }
        // to support ClipToBounds for custom child items
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return ClipToBounds ? base.GetLayoutClip(layoutSlotSize) : null;
        }
    }
}
