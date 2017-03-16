using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl
{
    class MeasureSegmentController
    {
        private SegmentPanel segmentPanel;
        private int systemIndex =0;
        public MeasureSegmentController(Model.ScorePartwisePartMeasureMusicXML measure, string partID, int stavesCount, int systemIndex)
        {
            this.systemIndex = systemIndex;
            segmentPanel = new SegmentPanel(partID, systemIndex);
            for (int i = 1; i <= stavesCount; i++)
            {
                segmentPanel.AddAttributesContainer(new SegmentPanelContainers.MeasureAttributesContainer(measure.Items.OfType<Model.MeasureItems.AttributesMusicXML>().FirstOrDefault(), measure.Number, partID, i), i);
            }
            //segmentPanel.AddAttributesContainer(new SegmentPanelContainers.MeasureAttributesContainer(measure.Items.OfType<Model.MeasureItems.AttributesMusicXML>().FirstOrDefault(), measure.Number, partID), stavesCount);
        }
        public SegmentPanel GetContentPanel()
        {
            return segmentPanel;
        }
    }
}
