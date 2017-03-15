using MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers
{
    class MeasureAttributesContainer : Canvas
    {
        private List<IAttributeItemVisual> attributes;
        private double clefWidth = 0.0;
        private double keySignatureWidth = 0.0;
        private double timeSignatureWidth = 0.0;
        private double containerWidth = 0.0;
        private Model.MeasureItems.AttributesMusicXML currentAttributes;
        private int staveNumber = 1;
        private string measureId;
        private string partId;
        public double ContainerWidth
        {
            get
            {
                return containerWidth;
            }

            set
            {
                containerWidth = value;
            }
        }

        public MeasureAttributesContainer(Model.MeasureItems.AttributesMusicXML attributesXML, string measureId, string partId, int stave = 1)
        {
            attributes = new List<IAttributeItemVisual>();
            this.measureId = measureId;
            this.partId = partId;
            currentAttributes = attributesXML;
            if (currentAttributes != null)
            {
            InitAtributes();
            }
        }

        private void InitAtributes()
        {
            if (currentAttributes.Clef.Count != 0)
            {
                int clefCount = currentAttributes.Clef.Count;
                if (clefCount == 1 || currentAttributes.Clef.Where(i=> i.Number == "1").FirstOrDefault() != null)
                {
                    var currentClef = currentAttributes.Clef.ElementAt(0);
                    ClefContainerItem clefItem = new ClefContainerItem(currentClef);
                    clefWidth = clefItem.ItemWidth;
                    AddClef(clefItem);
                }
            }
            if (currentAttributes.Key.Count != 0)
            {
                int keyCount = currentAttributes.Key.Count;
                if (keyCount == 1)
                {
                    var currentKey = currentAttributes.Key.ElementAt(0);
                    KeyContainerItem keyItem = new KeyContainerItem(currentKey, measureId, partId);
                    keySignatureWidth = keyItem.ItemWidth;
                    AddKeySignature(keyItem);
                }
            }
            if (currentAttributes.Time.Count != 0)
            {
                int timeCount = currentAttributes.Time.Count;
                if (timeCount == 1)
                {
                    var currentTime = currentAttributes.Time.ElementAt(0);
                    TimeSignatureContainerItem timeItem = new TimeSignatureContainerItem(currentTime);
                    timeSignatureWidth = timeItem.ItemWidth;
                    AddTimeSignature(timeItem);
                }
            }
            ArrangeAttribures();
        }

        private void ArrangeAttribures()
        {
            double offset = 0;
            if (clefWidth != 0)
            {
                offset += attributes.OfType<ClefContainerItem>().FirstOrDefault().ItemWidth;
            }
            if (keySignatureWidth != 0)
            {
                SetLeft(attributes.OfType<KeyContainerItem>().FirstOrDefault(), offset);
                offset += attributes.OfType<KeyContainerItem>().FirstOrDefault().ItemWidth;
            }
            if (timeSignatureWidth != 0)
            {
                SetLeft(attributes.OfType<TimeSignatureContainerItem>().FirstOrDefault(), offset);
            }
        }

        private void CalculateContainerWidth()
        {
            ContainerWidth = clefWidth + keySignatureWidth + timeSignatureWidth;
        }

        public void AddClef(ClefContainerItem clefVisualItem)
        {
            clefWidth = clefVisualItem.ItemWidth;
            attributes.Add(clefVisualItem);
            Children.Add(clefVisualItem);
        }
        public void AddKeySignature(KeyContainerItem keyVisualItem)
        {
            keySignatureWidth = keyVisualItem.ItemWidth;
            attributes.Add(keyVisualItem);
            Children.Add(keyVisualItem);
        }
        public void AddTimeSignature(TimeSignatureContainerItem timeSignatureVisualItem)
        {
            timeSignatureWidth = timeSignatureVisualItem.ItemWidth;
            attributes.Add(timeSignatureVisualItem);
            Children.Add(timeSignatureVisualItem);
        }
    }
}
