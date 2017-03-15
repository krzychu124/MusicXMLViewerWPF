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
        public MeasureSegmentController(Model.ScorePartwisePartMeasureMusicXML measure, string partID)
        {
            segmentPanel = new SegmentPanel(partID);
            segmentPanel.AddAttributesContainer(new SegmentPanelContainers.MeasureAttributesContainer(measure.Items.OfType<Model.MeasureItems.AttributesMusicXML>().FirstOrDefault(), measure.Number, partID));
        }
        public SegmentPanel GetContentPanel()
        {
            return segmentPanel;
        }
    }
}
