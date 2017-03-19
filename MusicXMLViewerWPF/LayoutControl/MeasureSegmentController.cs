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
        private int pageIndex;
        public MeasureSegmentController(Model.ScorePartwisePartMeasureMusicXML measure, string partID, int stavesCount, int systemIndex, int pageIndex)
        {
            this.systemIndex = systemIndex;
            this.pageIndex = pageIndex;
            segmentPanel = new SegmentPanel(partID, measure.Number, systemIndex, pageIndex);
            for (int i = 1; i <= stavesCount; i++)
            {
                segmentPanel.AddAttributesContainer(new SegmentPanelContainers.MeasureAttributesContainer(measure.Items.OfType<Model.MeasureItems.AttributesMusicXML>().FirstOrDefault(), measure.Number, partID, i), i);
                segmentPanel.AddNotesContainer(new SegmentPanelContainers.MeasureNotesContainer(measure, partID, i), i);

            }
            //segmentPanel.AddAttributesContainer(new SegmentPanelContainers.MeasureAttributesContainer(measure.Items.OfType<Model.MeasureItems.AttributesMusicXML>().FirstOrDefault(), measure.Number, partID), stavesCount);
        }
        public SegmentPanel GetContentPanel()
        {
            return segmentPanel;
        }
    }
}
