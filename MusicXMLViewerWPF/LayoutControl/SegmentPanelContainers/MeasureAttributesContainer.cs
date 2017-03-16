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
        private double sharedClefWidth = 0.0;
        private double sharedKeySignatureWidth = 0.0;
        private double sharedTimeSignatureWidth = 0.0;
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

        public double SharedClefWidth
        {
            get
            {
                return sharedClefWidth;
            }

            set
            {
                sharedClefWidth = value;
            }
        }

        public double SharedKeySignatureWidth
        {
            get
            {
                return sharedKeySignatureWidth;
            }

            set
            {
                sharedKeySignatureWidth = value;
            }
        }

        public double SharedTimeSignatureWidth
        {
            get
            {
                return sharedTimeSignatureWidth;
            }

            set
            {
                sharedTimeSignatureWidth = value;
            }
        }

        public MeasureAttributesContainer(Model.MeasureItems.AttributesMusicXML attributesXML, string measureId, string partId, int stave = 1)
        {
            attributes = new List<IAttributeItemVisual>();
            this.measureId = measureId;
            this.partId = partId;
            currentAttributes = attributesXML;
            staveNumber = stave;
            InitAtributes();
            if (currentAttributes != null)
            {
                
            }
        }

        private void InitAtributes()
        {
            DrawingHelpers.PartProperties partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partId];
            //Adding clef if clef.PrintObject = Yes;
            List<double> tempClefsWidth = new List<double>();
            foreach (var clef in partProperties.ClefAttributes[measureId])
            {
                if (clef.Number == staveNumber.ToString())
                {
                    if(clef.PrintObject == Model.Helpers.SimpleTypes.YesNoMusicXML.yes)
                    {
                        ClefContainerItem clefItem = new ClefContainerItem(clef);
                        tempClefsWidth.Add(clefItem.ItemWidth);
                        AddClef(clefItem);
                    }
                }
            }
            clefWidth = tempClefsWidth.Count != 0 ? tempClefsWidth.Max() : 0;
            
            //Adding keySignature if clef.PrintObject = Yes;
            Model.MeasureItems.Attributes.KeyMusicXML key = partProperties.KeyAttributes[measureId];
            if (key.PrintObject == Model.Helpers.SimpleTypes.YesNoMusicXML.yes)
            {
                KeyContainerItem keyItem = new KeyContainerItem(key, measureId, partId);
                keySignatureWidth =keyItem.ItemWidth;
                AddKeySignature(keyItem);
            }
            //Adding timeSignature if clef.PrintObject = Yes;
            Model.MeasureItems.Attributes.TimeMusicXML timeSignature = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.TimeSignatures.TimeSignaturesDictionary[measureId];
            if (timeSignature.PrintObject == Model.Helpers.SimpleTypes.YesNoMusicXML.yes)
            {
                TimeSignatureContainerItem timeItem = new TimeSignatureContainerItem(timeSignature);
                timeSignatureWidth = timeItem.ItemWidth;
                AddTimeSignature(timeItem);
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
        public Tuple<double, double> GetKeyTimeSigWidths()
        {
            return new Tuple<double, double>(keySignatureWidth, timeSignatureWidth);
        }
        public void SetSharedPositions(double keyOffset, double timeSigOffset)
        {
            sharedKeySignatureWidth = keyOffset;
            sharedTimeSignatureWidth = timeSigOffset;
        }
        public void ArrangeWithSharedPositions(bool useDefaultPosition = false)
        {
            if (!useDefaultPosition)
            {
                SetLeft(attributes.OfType<KeyContainerItem>().FirstOrDefault(), sharedKeySignatureWidth);
                SetLeft(attributes.OfType<TimeSignatureContainerItem>().FirstOrDefault(), sharedTimeSignatureWidth);
            }
            else
            {
                ArrangeAttribures();
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
