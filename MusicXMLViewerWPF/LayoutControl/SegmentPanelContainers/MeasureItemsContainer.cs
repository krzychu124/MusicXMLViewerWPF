using MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes;
using MusicXMLScore.Model;
using MusicXMLScore.Model.MeasureItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MusicXMLScore.Converters;
using MusicXMLScore.LayoutControl.SegmentPanelContainers.Attributes;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers
{
    class MeasureItemsContainer : Canvas
    {
        List<IMeasureItemVisual> measureItemsVisuals;
        private List<NoteMusicXML> notesList;
        private List<Tuple<int, IMeasureItemVisual>> itemsWithPostition;
        private ScorePartwisePartMeasureMusicXML measure;
        private string measureId = "";
        private string partId;
        private int staveNumber;
        internal List<Tuple<int, IMeasureItemVisual>> ItemsWithPostition
        {
            get
            {
                return itemsWithPostition;
            }

            set
            {
                itemsWithPostition = value;
            }
        }

        public MeasureItemsContainer(string measureId, string partId, int numberOfStave)
        {
            measureItemsVisuals = new List<IMeasureItemVisual>();
            notesList = new List<NoteMusicXML>();
            itemsWithPostition = new List<Tuple<int, IMeasureItemVisual>>();
            this.measureId = measureId;
            this.partId = partId;
            staveNumber = numberOfStave;
        }
        public MeasureItemsContainer(ScorePartwisePartMeasureMusicXML measure, string partId, int numberOfStave)
        {
            measureItemsVisuals = new List<IMeasureItemVisual>();
            notesList = new List<NoteMusicXML>();
            this.measure = measure;
            measureId = measure.Number;
            this.partId = partId;
            staveNumber = numberOfStave;
            notesList = measure.Items.OfType<NoteMusicXML>().ToList();
            List<NoteMusicXML> chordListTemp = new List<NoteMusicXML>();
            for (int i = 0; i < notesList.Count; i++)
            {
                NoteChoiceTypeMusicXML noteType = notesList[i].GetNoteType();
                NoteChoiceTypeMusicXML tempChoice = NoteChoiceTypeMusicXML.none;
                string staff = "1";
                if (int.Parse(notesList[i].Staff) != staveNumber)
                {
                    continue;
                }
                if (notesList[i].ItemsElementName.Contains(NoteChoiceTypeMusicXML.rest))
                {
                    if (chordListTemp.Count != 0)
                    {
                        staff = chordListTemp.ElementAt(0).Staff;
                        NoteContainerItem note = new NoteContainerItem(chordListTemp, i, partId, measure.Number, staff);
                        AddNote(note);
                        chordListTemp.Clear();
                    }
                    RestContainterItem rest = new RestContainterItem(notesList[i], i, partId, measure.Number, staff);
                    AddRest(rest);
                    continue;
                }
                if (notesList[i].ItemsElementName.Contains(NoteChoiceTypeMusicXML.chord))
                {
                    chordListTemp.Add(notesList[i]);
                }
                else
                {
                    if (chordListTemp.Count != 0)
                    {
                        staff = chordListTemp.ElementAt(0).Staff;
                        NoteContainerItem note = new NoteContainerItem(chordListTemp, i, partId, measure.Number, staff);
                        AddNote(note);
                        chordListTemp.Clear();
                        chordListTemp.Add(notesList[i]);
                    }
                    else
                    {
                        chordListTemp.Add(notesList[i]);
                    }
                    tempChoice = noteType;
                }
                if (chordListTemp.Count != 0 && i+1 == notesList.Count)
                {
                    staff = chordListTemp.ElementAt(0).Staff;
                    NoteContainerItem note = new NoteContainerItem(chordListTemp, i, partId, measure.Number, staff);
                    AddNote(note);
                    chordListTemp.Clear();
                }
            }
        }
        public void ArrangeUsingDurationTable(Dictionary<int, double> durationTable)
        {
            //-----------------
            // Keys: -3, -2, -1 used for attributes: clef, key and time
            //-----------------
            var measureBeginningAttributes = itemsWithPostition.Where(x => x.Item1 == 0 && x.Item2 is IAttributeItemVisual).Select(x => x.Item2).ToList();
            if (measureBeginningAttributes.Count != 0)
            {
                ClefContainerItem clef = measureBeginningAttributes.Where(x => x is ClefContainerItem).FirstOrDefault() as ClefContainerItem;
                if (clef != null)
                {
                    ArrangeAttributes(clef, durationTable[-3]);
                }
                KeyContainerItem key = measureBeginningAttributes.Where(x => x is KeyContainerItem).FirstOrDefault() as KeyContainerItem;
                if (key != null)
                {
                    ArrangeAttributes(key, durationTable[-2]);
                }
                TimeSignatureContainerItem time = measureBeginningAttributes.Where(x => x is TimeSignatureContainerItem).FirstOrDefault() as TimeSignatureContainerItem;
                if (time != null)
                {
                    ArrangeAttributes(time, durationTable[-1]);
                }
            }
            foreach (var item in itemsWithPostition)
            {
                if (item.Item2 is INoteItemVisual)
                {
                    INoteItemVisual note = item.Item2 as INoteItemVisual;
                    if (note.ItemDuration == 0)
                    {
                        continue;
                    }
                    SetLeft(item.Item2.ItemCanvas as Canvas, durationTable[item.Item1]);
                }
                if (item.Item2 is IAttributeItemVisual && item.Item1 >0)
                {
                    SetLeft(item.Item2.ItemCanvas as Canvas, durationTable[item.Item1] - item.Item2.ItemWidth);
                }
            }
        }

        public List<int> GetDurationIndexes()
        {
            return itemsWithPostition.Select(x => x.Item1).Distinct().ToList();
        }
        
        public double ArrangeAttributes(IAttributeItemVisual attributeVisual, double currentPosition = 0.0)
        {
            double width = currentPosition;
            LayoutStyle.MeasureLayoutStyle attributesLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.MeasureStyle;

            switch (attributeVisual.AttributeIndex)
            {
                case 0:
                    ClefContainerItem clef = attributeVisual as ClefContainerItem;
                    width += attributesLayout.ClefLeftOffset.TenthsToWPFUnit();
                    SetLeft(clef.ItemCanvas, width);
                    width += clef.ItemWidth + attributesLayout.ClefRightOffset.TenthsToWPFUnit();
                    break;
                case 1:
                    KeyContainerItem key = attributeVisual as KeyContainerItem;
                    width += attributesLayout.KeySigLeftOffset;
                    SetLeft(key.ItemCanvas, width);
                    width += key.ItemWidth + (key.ItemWidth != 0 ? attributesLayout.KeySigRightOffset.TenthsToWPFUnit() : 0);
                    break;
                case 2:
                    TimeSignatureContainerItem timeSig = attributeVisual as TimeSignatureContainerItem;
                    width += attributesLayout.TimeSigLeftOffset.TenthsToWPFUnit();
                    SetLeft(timeSig.ItemCanvas, width);
                    width += timeSig.ItemWidth + attributesLayout.TimeSigRightOffset.TenthsToWPFUnit();
                    break;
            }

            return width;
        }
        public void AppendNote(NoteContainerItem note, int cursorPosition, string voice = "1")
        {
            Tuple<int, IMeasureItemVisual> noteVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, note);
            AddNote(note);
            itemsWithPostition.Add(noteVisual);
        }

        public void AppendRest(RestContainterItem rest, int cursorPosition, string voice = "1")
        {
            Tuple<int, IMeasureItemVisual> restVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, rest);
            AddRest(rest);
            itemsWithPostition.Add(restVisual);
        }

        public void AppendAttribute(IAttributeItemVisual attributeItem, int cursorPosition)//temp
        {
            Tuple<int, IMeasureItemVisual> attributesVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, attributeItem);
            AddAttribute(attributeItem);
            itemsWithPostition.Add(attributesVisual);
        }
        public void AddNote(NoteContainerItem noteVisual)
        {
            measureItemsVisuals.Add(noteVisual);
            Children.Add(noteVisual.ItemCanvas);
        }
        public void AddRest(RestContainterItem restVisual)
        {
            measureItemsVisuals.Add(restVisual);
            Children.Add(restVisual.ItemCanvas);
        }
        public void AddAttribute(IAttributeItemVisual attributeVisual)//temp
        {
            measureItemsVisuals.Add(attributeVisual);
            Children.Add(attributeVisual.ItemCanvas as Canvas); //TODO_WiP TEST
        }
    }
}
