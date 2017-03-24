using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Model.MeasureItems;
using System.Diagnostics;

namespace MusicXMLScore.LayoutControl
{
    class MeasureSegmentController
    {
        private SegmentPanel segmentPanel;
        private int systemIndex =0;
        private int pageIndex;
        private PartProperties partProperties;
        Dictionary<string, SegmentPanelContainers.MeasureNotesContainer> staffs;
        public MeasureSegmentController(Model.ScorePartwisePartMeasureMusicXML measure, string partID, int stavesCount, int systemIndex, int pageIndex)
        {
            this.systemIndex = systemIndex;
            this.pageIndex = pageIndex;
            partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partID];
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            segmentPanel = new SegmentPanel(partID, measure.Number, systemIndex, pageIndex);
            for (int i = 1; i <= stavesCount; i++)
            {
                //segmentPanel.AddAttributesContainer(new SegmentPanelContainers.MeasureAttributesContainer(measure.Items.OfType<AttributesMusicXML>().FirstOrDefault(), measure.Number, partID, i), i);
                //segmentPanel.AddNotesContainer(new SegmentPanelContainers.MeasureNotesContainer(measure, partID, i), i);
            }
            stopWatch.Stop();
            Log.LoggIt.Log($"Measure content (SegmentPanel){measure.Number} processig done in: {stopWatch.ElapsedTicks}", Log.LogType.Warning);
            stopWatch = new Stopwatch();
            //if (!measure.Items.OfType<AttributesMusicXML>().Any())
            //{
            //    int test = 0;
            //}
            stopWatch.Start();
            var currentDivisions = partProperties.GetDivisionsMeasureId(measure.Number);
            var currentTime = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetTimeSignature(measure.Number);
            double denominator = currentTime.GetDenominator();
            double numerator = currentTime.GetNumerator();
            int maxDuration = (int)((4 / (double)currentTime.GetDenominator()) * ( currentDivisions * currentTime.GetNumerator()));
            int durationCursor = 0;
            var measureItems = measure.Items;
            staffs = new Dictionary<string, SegmentPanelContainers.MeasureNotesContainer>();
          //  attributesContainers = new Dictionary<int, Dictionary<>>
            for (int i = 0; i < stavesCount; i++)
            {
                staffs.Add((i + 1).ToString(), new SegmentPanelContainers.MeasureNotesContainer(measure.Number, partID, i + 1));
            }
            for (int i = 0; i < measure.Items.Length; i++)
            {
                string typeName = measure.Items[i].GetType().Name;
                switch (typeName)
                {
                    case nameof(AttributesMusicXML):
                        AttributesMusicXML a = (AttributesMusicXML)measureItems[i];
                        var attributesContainer = new SegmentPanelContainers.MeasureAttributesContainer(a, measure.Number, partID);
                        break;
                    case nameof(BackupMusicXML):
                        BackupMusicXML b = (BackupMusicXML)measureItems[i];
                        durationCursor -= int.Parse(b.Duration.ToString());
                        break;
                    case nameof(ForwardMusicXML):
                        ForwardMusicXML f = (ForwardMusicXML)measureItems[i];
                        durationCursor += int.Parse(f.Duration.ToString());
                        break;
                    case nameof(NoteMusicXML):
                        NoteMusicXML n = (NoteMusicXML)measureItems[i];
                        string staffNumber = n.Staff;
                        string voice = n.Voice;
                        if (n.IsRest())
                        {
                            staffs[staffNumber].AppendRest(new SegmentPanelContainers.Notes.RestContainterItem(n, durationCursor, partID, measure.Number, staffNumber), durationCursor, voice);
                        }
                        else
                        {
                            staffs[staffNumber].AppendNote(new SegmentPanelContainers.Notes.NoteContainerItem(n, durationCursor, partID, measure.Number, staffNumber), durationCursor, voice);
                        }
                        durationCursor += n.GetDuration();
                        break;
                    default:
                        break;
                }
            }
            ArrangeContainers(measure.CalculatedWidth.TenthsToWPFUnit(), maxDuration);
            AppendContainersToSegment();

            stopWatch.Stop();
            Log.LoggIt.Log($"Measure content (Switch) processig done in: {stopWatch.ElapsedMilliseconds}", Log.LogType.Warning);

        }

        private void ArrangeContainers(double availableWidth, int maxDuration)
        {
            foreach (var item in staffs)
            {
                item.Value.ArrangeNotesByDuration(availableWidth, maxDuration);
            }
        }

        private void AppendContainersToSegment()
        {
            foreach (var item in staffs)
            {
                segmentPanel.AddNotesContainer(item.Value, int.Parse(item.Key));
            }
        }

        public SegmentPanel GetContentPanel()
        {
            return segmentPanel;
        }
    }
}
