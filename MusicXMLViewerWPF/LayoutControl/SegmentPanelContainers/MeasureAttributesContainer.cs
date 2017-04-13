﻿using MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers
{
    class MeasureAttributesContainer : Notes.INoteItemVisual
    {
        private Canvas itemCanvas;
        private List<IAttributeItemVisual> attributes;
        private double clefWidth = 0.0;
        private double keySignatureWidth = 0.0;
        private double timeSignatureWidth = 0.0;
        private double sharedClefWidth = 0.0;
        private double sharedKeySignatureWidth = 0.0;
        private double sharedTimeSignatureWidth = 0.0;
        private double containerWidth = 0.0;
        private Model.MeasureItems.AttributesMusicXML currentAttributes;
        private string itemStaff = "1";
        private string measureId;
        private string partId;
        private int fractionCursorPosition;
        public double ItemWidth
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

        public int ItemDuration
        {
            get
            {
                return 0;
            }
        }

        public double ItemWidthMin
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public double ItemWidthOpt
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public double ItemWeight
        {
            get
            {
                return 0;
            }
        }

        public Canvas ItemCanvas
        {
            get
            {
                return itemCanvas;
            }

            set
            {
                itemCanvas = value;
            }
        }

        public string ItemStaff
        {
            get
            {
                return itemStaff;
            }

            set
            {
                itemStaff = value;
            }
        }

        public MeasureAttributesContainer(int fractionCursorPosition, string measureId, string partId, string staffId)
        {
            attributes = new List<IAttributeItemVisual>();
            itemCanvas = new Canvas();
            this.measureId = measureId;
            this.partId = partId;
            this.fractionCursorPosition = fractionCursorPosition;
            this.itemStaff = staffId;
        }

        public MeasureAttributesContainer(Model.MeasureItems.AttributesMusicXML attributesXML, string measureId, string partId, string stave = "1")
        {
            attributes = new List<IAttributeItemVisual>();
            itemCanvas = new Canvas();
            this.measureId = measureId;
            this.partId = partId;
            currentAttributes = attributesXML;
            itemStaff = stave;
            InitAtributes();
            if (currentAttributes != null)
            {
                
            }
            CalculateContainerWidth();
        }

        private double GetContainerWidth()
        {
            double result = 0.0;
            result = attributes.Sum(i => i.ItemWidth);
            return result;
        }
        private void InitAtributes()
        {
            DrawingHelpers.PartProperties partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partId];
            //Adding clef if clef.PrintObject = Yes;
            List<double> tempClefsWidth = new List<double>();
            foreach (var clef in partProperties.ClefAttributes[measureId])
            {
                if (clef.Number == itemStaff.ToString())
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
                KeyContainerItem keyItem = new KeyContainerItem(key, 0, measureId, partId, itemStaff);
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
                Canvas.SetLeft(attributes.OfType<KeyContainerItem>().FirstOrDefault().ItemCanvas, offset);
                offset += attributes.OfType<KeyContainerItem>().FirstOrDefault().ItemWidth;
            }
            if (timeSignatureWidth != 0)
            {
                Canvas.SetLeft(attributes.OfType<TimeSignatureContainerItem>().FirstOrDefault().ItemCanvas, offset);
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
                Canvas.SetLeft(attributes.OfType<KeyContainerItem>().FirstOrDefault().ItemCanvas, sharedKeySignatureWidth);
                Canvas.SetLeft(attributes.OfType<TimeSignatureContainerItem>().FirstOrDefault().ItemCanvas, sharedTimeSignatureWidth);
            }
            else
            {
                ArrangeAttribures();
            }
        }

        private void CalculateContainerWidth()
        {
            ItemWidth = clefWidth + keySignatureWidth + timeSignatureWidth;
        }

        public void AddClef(ClefContainerItem clefVisualItem)
        {
            clefWidth = clefVisualItem.ItemWidth;
            attributes.Add(clefVisualItem);
            ItemCanvas.Children.Add(clefVisualItem.ItemCanvas);
        }
        public void AddKeySignature(KeyContainerItem keyVisualItem)
        {
            keySignatureWidth = keyVisualItem.ItemWidth;
            attributes.Add(keyVisualItem);
            ItemCanvas.Children.Add(keyVisualItem.ItemCanvas);
        }
        public void AddTimeSignature(TimeSignatureContainerItem timeSignatureVisualItem)
        {
            timeSignatureWidth = timeSignatureVisualItem.ItemWidth;
            attributes.Add(timeSignatureVisualItem);
            ItemCanvas.Children.Add(timeSignatureVisualItem.ItemCanvas);
        }

        public void DrawSpace(double length, bool red)
        {
        }
    }
}
