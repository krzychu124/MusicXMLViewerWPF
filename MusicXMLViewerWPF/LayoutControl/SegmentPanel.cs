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
        private Dictionary<int, MeasureItemsContainer> notesContainer;
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
        private int pageIndex;
        private string measureId;
        public SegmentPanel(string partID, string measureId, int systemIndex, int pageIndex)
        {
            this.partID = partID;
            this.systemIndex = systemIndex;
            this.pageIndex = pageIndex;
            this.measureId = measureId;
            partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[this.partID];
            staves = partProperties.NumberOfStaves;
            defaultStavesDistance = partProperties.StaffLayoutPerPage.ElementAt(this.pageIndex).ElementAt(systemIndex).StaffDistance;//? StavesDistance;
            SetHeight();
        }
        public void AddAttributesContainer(MeasureAttributesContainer measureAttributes, int numberOfStave = 1)
        {
            measureAttributes.ItemCanvas.Tag = numberOfStave.ToString();
            Canvas.SetLeft(measureAttributes.ItemCanvas, 0); //adding left offset, refactor to %of staffline height, or add to each child element
            if (numberOfStave != 1)//works property if number of staves is 2, higher nuber will overlap with 2nd. stave, WiP
            {
                double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffHeight.MMToWPFUnit();
                Canvas.SetTop(measureAttributes.ItemCanvas, staffHeight + defaultStavesDistance.TenthsToWPFUnit());
            }
            Children.Add(measureAttributes.ItemCanvas);
            InitAttributesContainer();// attributesContainer = new Dictionary<int, MeasureAttributesContainer>();
            attributesContainer.Add(numberOfStave, measureAttributes);
        }
        public void AddNotesContainer(MeasureItemsContainer measureNotes, int numberOfStave = 1)
        {
            measureNotes.Tag = numberOfStave.ToString();
            //var t = attributesContainer.ElementAt(0).Value.ContainerWidth;
            //measureNotes.ArrangeNotes(panelWidth - t);
            //Canvas.SetLeft(measureNotes, t);
            if (numberOfStave != 1)
            {
                double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.StaffHeight.MMToWPFUnit();
                Canvas.SetTop(measureNotes, staffHeight + defaultStavesDistance.TenthsToWPFUnit());
            }
            Children.Add(measureNotes);
            InitNotesContainer();
            notesContainer.Add(numberOfStave, measureNotes);
        }
        private void InitAttributesContainer()
        {
            if (attributesContainer == null)
            {
                attributesContainer = new Dictionary<int, MeasureAttributesContainer>();
            }
        }
        private void InitNotesContainer()
        {
            if (notesContainer == null)
            {
                notesContainer = new Dictionary<int, MeasureItemsContainer>();
            }
        }
        private void SetHeight()
        {
            panelHeight = partProperties.PartHeight.TenthsToWPFUnit();
            panelWidth = ViewModel.ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.Where(i=> i.Id == partID).FirstOrDefault().MeasuresByNumber[measureId].CalculatedWidth.TenthsToWPFUnit();
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
