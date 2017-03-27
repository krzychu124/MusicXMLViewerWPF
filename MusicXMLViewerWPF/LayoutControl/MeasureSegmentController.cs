using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Converters;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Model.MeasureItems;
using System.Diagnostics;
using MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes;

namespace MusicXMLScore.LayoutControl
{
    class MeasureSegmentController
    {
        private SegmentPanel segmentPanel;
        private int systemIndex =0;
        private int pageIndex;
        private int stavesCount = 1;
        private PartProperties partProperties;
        Dictionary<string, SegmentPanelContainers.MeasureNotesContainer> staffs;
        Dictionary<string, SegmentPanelContainers.MeasureAttributesContainer> attributesContainer;
        public MeasureSegmentController(Model.ScorePartwisePartMeasureMusicXML measure, string partID, int stavesCount, int systemIndex, int pageIndex)
        {
            this.systemIndex = systemIndex;
            this.pageIndex = pageIndex;
            this.stavesCount = stavesCount;
            partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partID];
            Stopwatch stopWatch;// = new Stopwatch();
            //stopWatch.Start();
            segmentPanel = new SegmentPanel(partID, measure.Number, systemIndex, pageIndex);
            //for (int i = 1; i <= stavesCount; i++)
            //{
            //    //segmentPanel.AddAttributesContainer(new SegmentPanelContainers.MeasureAttributesContainer(measure.Items.OfType<AttributesMusicXML>().FirstOrDefault(), measure.Number, partID, i), i);
            //    //segmentPanel.AddNotesContainer(new SegmentPanelContainers.MeasureNotesContainer(measure, partID, i), i);
            //}
            //stopWatch.Stop();
            //Log.LoggIt.Log($"Measure content (SegmentPanel){measure.Number} processig done in: {stopWatch.ElapsedMilliseconds}", Log.LogType.Warning);
            stopWatch = new Stopwatch();
            //if (!measure.Items.OfType<AttributesMusicXML>().Any())
            //{
            //    int test = 0;
            //}
            stopWatch.Start();
            var currentDivisions = partProperties.GetDivisionsMeasureId(measure.Number);
            var currentTime = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetTimeSignature(measure.Number);
            double denominator = currentDivisions ==1 ? 1 : currentTime.GetDenominator();
            double numerator = currentDivisions == 1 ? 1 : currentTime.GetNumerator();
            int maxDuration = (int)((4 / (double)denominator/*currentTime.GetDenominator()*/) * ( currentDivisions * numerator/*currentTime.GetNumerator()*/));
            int durationCursor = 0;
            var measureItems = measure.Items;
            staffs = new Dictionary<string, SegmentPanelContainers.MeasureNotesContainer>();
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
                        if (CheckIfElementsOtherThanClefKeyTime(a, durationCursor))
                        {

                        }
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
            GenerateAndAddAttributesContainers(measure.Number, partID);
            ArrangeContainers(measure.CalculatedWidth.TenthsToWPFUnit(), maxDuration);
            AppendContainersToSegment();

            stopWatch.Stop();
            Log.LoggIt.Log($"Measure content {measure.Number} (Switch) processig done in: {stopWatch.ElapsedMilliseconds}", Log.LogType.Warning);

        }

        private bool CheckIfElementsOtherThanClefKeyTime(AttributesMusicXML a, int currentFraction)
        {
            if (a.DivisionsSpecified)
            {

            }
            if (a.MeasureStyle.Count != 0)
            {

            }
            if (a.PartSymbol != null)
            {

            }
            if (a.StaffDetails.Count != 0)
            {

            }
            if (a.Transpose.Count != 0)
            {

            }
            return false;
        }

        private void GenerateAndAddAttributesContainers(string measureNumber, string partID)
        {
            attributesContainer = new Dictionary<string, SegmentPanelContainers.MeasureAttributesContainer>();
            SegmentPanelContainers.MeasureAttributesContainer ma;
            for (int i = 1; i <= stavesCount; i++) // add remainders for each systemSegment beginning measure
            {
                if (partProperties.ClefChanges.ContainsKey(measureNumber))
                {
                    foreach (var clef in partProperties.ClefChanges[measureNumber].ClefsChanges)
                    {
                        ClefContainerItem clefContainer = new ClefContainerItem(clef.Item1, clef.Item2, clef.Item3);
                        //ma = new SegmentPanelContainers.MeasureAttributesContainer( clef.Item2, measureNumber, partID, clef.Item1);
                        //ma.AddClef(clefContainer);
                        //ma.ArrangeWithSharedPositions(true);
                        //attributesContainer.Add(clef.Item1, ma);
                    }
                }
                if (partProperties.KeyChanges.ContainsKey(measureNumber))
                {
                    foreach (var key in partProperties.KeyChanges[measureNumber].KeysChanges)
                    {
                        KeyContainerItem keyContainer = new KeyContainerItem(key.Item1, key.Item2, key.Item3);
                    }
                }
                if (partProperties.TimeChanges.ContainsKey(measureNumber))
                {
                    foreach (var time in partProperties.TimeChanges[measureNumber].TimesChanges)
                    {
                        TimeSignatureContainerItem timeContainer = new TimeSignatureContainerItem(time.Item1, time.Item2, time.Item3);
                    }
                }
            }
            if (attributesContainer.Count != 0)
            {
                foreach (var item in attributesContainer)
                {
                    segmentPanel.AddAttributesContainer(item.Value, int.Parse(item.Key));
                }
            }
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
