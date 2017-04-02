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
using MusicXMLScore.Model.MeasureItems.Attributes;

namespace MusicXMLScore.LayoutControl
{
    class MeasureSegmentController
    {
        private SegmentPanel segmentPanel;
        private int systemIndex =0;
        private int pageIndex;
        private int stavesCount = 1;
        private int maxDuration = 1;
        private PartProperties partProperties;
        Dictionary<string, SegmentPanelContainers.MeasureItemsContainer> staffs;
        Dictionary<string, SegmentPanelContainers.MeasureAttributesContainer> attributesContainer;
        public MeasureSegmentController(Model.ScorePartwisePartMeasureMusicXML measure, string partID, int stavesCount, int systemIndex, int pageIndex)
        {
            this.systemIndex = systemIndex;
            this.pageIndex = pageIndex;
            this.stavesCount = stavesCount;
            partProperties = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partID];
            Stopwatch stopWatch;

            segmentPanel = new SegmentPanel(partID, measure.Number, systemIndex, pageIndex);
            stopWatch = new Stopwatch();

            stopWatch.Start();
            var currentDivisions = partProperties.GetDivisionsMeasureId(measure.Number);
            var currentTime = ViewModel.ViewModelLocator.Instance.Main.CurrentScoreProperties.GetTimeSignature(measure.Number);
            double denominator = currentDivisions ==1 ? 1 : currentTime.GetDenominator();
            double numerator = currentDivisions == 1 ? 1 : currentTime.GetNumerator();
            maxDuration = (int)((4 / (double)denominator/*currentTime.GetDenominator()*/) * ( currentDivisions * numerator/*currentTime.GetNumerator()*/));
            int durationCursor = 0;
            var measureItems = measure.Items;
            staffs = new Dictionary<string, SegmentPanelContainers.MeasureItemsContainer>();

            for (int i = 0; i < stavesCount; i++)
            {
                staffs.Add((i + 1).ToString(), new SegmentPanelContainers.MeasureItemsContainer(measure.Number, partID, i + 1));
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
                            //int tempDuration = n.IsChord() ? durationCursor : durationCursor - n.GetDuration();
                            staffs[staffNumber].AppendNote(new SegmentPanelContainers.Notes.NoteContainerItem(n, durationCursor, partID, measure.Number, staffNumber), durationCursor, voice);
                        }
                        durationCursor += !n.IsChord() ? n.GetDuration() : 0;
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
            for (int i = 1; i <= stavesCount; i++) // missing - remainders for each systemSegment beginning measure
            {
                if (partProperties.ClefChanges.ContainsKey(measureNumber))
                {
                    var clefList = partProperties.ClefChanges[measureNumber].ClefsChanges.Where(x => x.Item1 == i.ToString()).ToList();
                    if (clefList.Count != 0)
                    {
                        foreach (var clef in clefList)
                        {
                            ClefContainerItem clefContainer = new ClefContainerItem(clef.Item1, clef.Item2, clef.Item3);
                            staffs[clef.Item1].AppendAttribute(clefContainer, clef.Item2);
                        }
                    }
                }
                if (partProperties.KeyChanges.ContainsKey(measureNumber))
                {
                    var keyList = partProperties.KeyChanges[measureNumber].KeysChanges.Where(x => x.Item1 == i.ToString() || x.Item1 == null).ToList();
                    if (keyList.Count != 0)
                    {
                        foreach (var key in keyList)
                        {
                            string staffNumber = key.Item1 != null ? key.Item1 : i.ToString();
                            KeyContainerItem keyContainer = new KeyContainerItem(key.Item3, key.Item2, measureNumber, partID, staffNumber);
                            staffs[staffNumber].AppendAttribute(keyContainer, key.Item2);
                        }
                    }
                }
                if (partProperties.TimeChanges.ContainsKey(measureNumber))
                {
                    var timeList = partProperties.TimeChanges[measureNumber].TimesChanges.Where(x => x.Item1 == i.ToString() || x.Item1 == null).ToList();
                    if (timeList.Count != 0)
                    {
                        foreach (var time in timeList)
                        {
                            string staffNumber = time.Item1 != null ? time.Item1 : i.ToString();
                            TimeSignatureContainerItem timeContainer = new TimeSignatureContainerItem(time.Item1, time.Item2, time.Item3);
                            staffs[staffNumber].AppendAttribute(timeContainer, time.Item2);
                        }
                    }
                }
            }
        }

        private void ArrangeContainers(double availableWidth, int maxDuration)
        {
            foreach (var item in staffs)
            {
                item.Value.ArrangeItemsByDuration(availableWidth, maxDuration);
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
