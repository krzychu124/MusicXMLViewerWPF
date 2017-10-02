using MusicXMLScore.LayoutControl;
using MusicXMLScore.ScoreLayout.MeasureLayouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicXMLScore.Prototypes
{
    class MeasureVisualTest : Panel
    {
        private readonly MeasureSegmentController measureSegment;
        private SharedMeasureProperties sharedProperties;

        public MeasureVisualTest(MeasureSegmentController measureSegment)
        {
            this.measureSegment = measureSegment;
            SetProperties();
        }

        protected override int VisualChildrenCount => 1;

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement item in InternalChildren)
            {
                item.Arrange(new Rect(0, 0, measureSegment.MinimalWidth, Height));
            }
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return measureSegment.GetMeasureCanvas();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            Redraw(sizeInfo.NewSize);
        }

        private void AddVisuals()
        {
            Children.Add(measureSegment.GetMeasureCanvas());
        }

        private void GenerateMeasureProperties()
        {
            sharedProperties = new SharedMeasureProperties(measureSegment.MeasureId);
            int duration = measureSegment.GetMinDuration();

            sharedProperties.AddAntiCollisionHelper("P1", measureSegment.GetContentItemsProperties(duration));
            sharedProperties.AddMeasureAttributesWidths(measureSegment.GetAttributesWidths());
            sharedProperties.GenerateFractionPositions();
        }
        private void Redraw(Size newSize)
        {
            measureSegment.MinimalWidth = newSize.Width;
            sharedProperties.SharedWidth = newSize.Width;
            UpdateMeasureLayout();
        }

        private void SetProperties()
        {
            GenerateMeasureProperties();
            Height = 120;
            MinWidth = sharedProperties.MinimalSharedWidth;
            measureSegment.MinimalWidth = MinWidth;
            UpdateMeasureLayout();
            AddVisuals();
        }

        private void UpdateMeasureLayout()
        {
            var fractions = sharedProperties.SharedFractions.ToDictionary(x => x.Key, x => x.Value.Position);
            measureSegment.ArrangeUsingDurationTable(fractions, false);
            if (measureSegment.BeamsController != null)
            {
                measureSegment.BeamsController.Draw(fractions);
                if (measureSegment.BeamsController.BeamsVisuals != null)
                {
                    measureSegment.AddBeams(measureSegment.BeamsController.BeamsVisuals);
                }
            }
        }
    }
}
