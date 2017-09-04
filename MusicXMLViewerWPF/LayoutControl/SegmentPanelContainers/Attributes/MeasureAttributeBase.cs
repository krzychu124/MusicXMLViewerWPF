using MusicXMLScore.Converters;
using MusicXMLScore.LayoutStyle.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes
{
    public abstract class MeasureAttributeBase
    {
        private Canvas itemCanvas;
        private int staffNumber;
        private int fractionPosition;
        private Brush color;
        private bool isVisible;
        private ItemsColorsStyle colorStyle;
        private readonly AttributeType attributeType;
        public double ItemLeftMargin { get; set; }
        public double ItemRightMargin { get; set; }

        protected MeasureAttributeBase(AttributeType type, int staffNumber, int fractionPosition)
        {
            attributeType = type;
            SetMargins();
            colorStyle = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.ItemsColorsStyle;
            itemCanvas = new Canvas();
            this.staffNumber = staffNumber;
            this.fractionPosition = fractionPosition;
            isVisible = true;
            color = colorStyle.DefaultColor;
        }

        private void SetMargins()
        {
            var measureStyle = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;
            switch (attributeType)
            {
                case AttributeType.clef:
                    ItemLeftMargin = measureStyle.ClefLeftOffset.TenthsToWPFUnit();
                    ItemRightMargin = measureStyle.ClefRightOffset.TenthsToWPFUnit();
                    break;
                case AttributeType.key:
                    ItemLeftMargin = measureStyle.KeySigLeftOffset.TenthsToWPFUnit();
                    ItemRightMargin = measureStyle.KeySigRightOffset.TenthsToWPFUnit();
                    break;
                case AttributeType.time:
                    ItemLeftMargin = measureStyle.TimeSigLeftOffset.TenthsToWPFUnit();
                    ItemRightMargin = measureStyle.TimeSigRightOffset.TenthsToWPFUnit();
                    break;
                default:
                    break;
            }
        }

        public Canvas ItemCanvas { get => itemCanvas; set => itemCanvas = value; }
        public int StaffNumber { get => staffNumber; set => staffNumber = value; }
        public int FractionPosition { get => fractionPosition; set => fractionPosition = value; }
        public Brush Color { get => color; set => color = value; }
        public bool IsVisible { get => isVisible; set => isVisible = value; }
        public ItemsColorsStyle ColorStyle { get => colorStyle; set => colorStyle = value; }

        public AttributeType AttributeType => attributeType;

        protected abstract void Update();

        
    }

    public enum AttributeType
        {
            clef,
            key,
            time
        }
}
