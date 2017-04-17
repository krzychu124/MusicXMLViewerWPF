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
        private List<IMeasureItemVisual> measureItemsVisuals;
        private List<NoteMusicXML> notesList;
        private List<Tuple<int, IMeasureItemVisual>> itemsWithPosition;
        private Dictionary<string, List<Tuple<int, IMeasureItemVisual>>> itemsPositionsPerStaff;
        private ScorePartwisePartMeasureMusicXML measure;
        private string measureId = "";
        private string partId;
        private int staveNumber;
        private int staffsNumber;
        internal List<Tuple<int, IMeasureItemVisual>> ItemsWithPostition
        {
            get
            {
                return itemsWithPosition;
            }

            set
            {
                itemsWithPosition = value;
            }
        }

        internal Dictionary<string, List<Tuple<int, IMeasureItemVisual>>> ItemsPositionsPerStaff
        {
            get
            {
                return itemsPositionsPerStaff;
            }

            set
            {
                itemsPositionsPerStaff = value;
            }
        }

        public MeasureItemsContainer(string measureId, string partId, int numberOfStave, string staffs)
        {
            measureItemsVisuals = new List<IMeasureItemVisual>();
            notesList = new List<NoteMusicXML>();
            itemsWithPosition = new List<Tuple<int, IMeasureItemVisual>>();
            InitPositionsPerStaff(staffs);
            this.measureId = measureId;
            this.partId = partId;
            staveNumber = numberOfStave;
            staffsNumber = int.Parse(staffs);
        }

        private void InitPositionsPerStaff(string staffs)
        {
            int numberOfStaffs = int.Parse(staffs);
            itemsPositionsPerStaff = new Dictionary<string, List<Tuple<int, IMeasureItemVisual>>>();
            for (int i = 0; i < numberOfStaffs; i++)
            {
                itemsPositionsPerStaff.Add( (i + 1).ToString(), new List<Tuple<int, IMeasureItemVisual>>());
            }
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

        internal void ArrangeStaffs(double staffDistance)
        {
            for (int i = 0; i < itemsPositionsPerStaff.Count; i++)
            {
                string staffNumber = (i+1).ToString();
                foreach (var item in itemsPositionsPerStaff[staffNumber])
                {
                    double top = staffNumber != "1" ? staffDistance : 0.0;
                    if (item.Item2 is NoteContainerItem)
                    {
                        var note = item.Item2 as NoteContainerItem;
                        note.Stem.SetStaffOffset(top);
                    }
                    Canvas.SetTop(item.Item2.ItemCanvas as Canvas, top);
                }
            }
        }

        public void ArrangeUsingDurationTable(Dictionary<int, double> durationTable)
        {
            //-----------------
            // Keys: -3, -2, -1 used for attributes: clef, key and time
            //-----------------
            var measureBeginningAttributes = itemsWithPosition.Where(x => x.Item1 == 0 && x.Item2 is IAttributeItemVisual).Select(x => x.Item2).ToList();
            if (measureBeginningAttributes.Count != 0)
            {
                //ClefContainerItem clef = measureBeginningAttributes.Where(x => x is ClefContainerItem).FirstOrDefault() as ClefContainerItem;
                var clef = measureBeginningAttributes.Where(x => x is ClefContainerItem).ToList();//as ClefContainerItem;
                if (clef.Count != 0)
                {
                    foreach (var c in clef)
                    {
                        ArrangeAttributes(c as ClefContainerItem, new Dictionary<string, double>(), durationTable[-3]);
                    }
                    //ArrangeAttributes(clef, new Dictionary<string, double>(),  durationTable[-3]);
                }
                //KeyContainerItem key = measureBeginningAttributes.Where(x => x is KeyContainerItem).FirstOrDefault() as KeyContainerItem;
                var key = measureBeginningAttributes.Where(x => x is KeyContainerItem).ToList();
                if (key.Count != 0)
                {
                    foreach (var item in key)
                    {
                        ArrangeAttributes(item as KeyContainerItem, new Dictionary<string, double>(), durationTable[-2]);
                    }
                   // ArrangeAttributes(key, new Dictionary<string, double>(), durationTable[-2]);
                }
                //TimeSignatureContainerItem time = measureBeginningAttributes.Where(x => x is TimeSignatureContainerItem).FirstOrDefault() as TimeSignatureContainerItem;
                var time = measureBeginningAttributes.Where(x => x is TimeSignatureContainerItem).ToList();
                if (time.Count != 0)
                {
                    foreach (var item in time)
                    {
                        ArrangeAttributes(item as TimeSignatureContainerItem, new Dictionary<string, double>(), durationTable[-1]);
                    }
                   // ArrangeAttributes(time, new Dictionary<string, double>(), durationTable[-1]);
                }
            }
            foreach (var item in itemsWithPosition)
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
            return itemsWithPosition.Select(x => x.Item1).Distinct().ToList();
        }
        
        public double ArrangeAttributes(IAttributeItemVisual attributeVisual, Dictionary<string, double> staffPositions, double currentPosition = 0.0)
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

        public void AppendNoteWithStaffNumber(NoteContainerItem note, int cursorPosition, string voice, string staffNumber)
        {
            Tuple<int, IMeasureItemVisual> noteVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, note);
            AddNote(note);
            itemsWithPosition.Add(noteVisual);
            itemsPositionsPerStaff[staffNumber].Add(noteVisual);
        }
        public void AppendNote(NoteContainerItem note, int cursorPosition, string voice = "1", string staffNumber = "1")
        {
            Tuple<int, IMeasureItemVisual> noteVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, note);
            AddNote(note);
            itemsWithPosition.Add(noteVisual);
        }
        public void AppendRestWithStaffNumber(RestContainterItem rest, int cursorPosition, string voice, string staffNumber)
        {
            Tuple<int, IMeasureItemVisual> restVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, rest);
            AddRest(rest);
            itemsWithPosition.Add(restVisual);
            itemsPositionsPerStaff[staffNumber].Add(restVisual);
        }
        public void AppendRest(RestContainterItem rest, int cursorPosition, string voice = "1")
        {
            Tuple<int, IMeasureItemVisual> restVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, rest);
            AddRest(rest);
            itemsWithPosition.Add(restVisual);
        }
        public void AppendAttributeWithStaffNumber(IAttributeItemVisual attributeItem, int cursorPosition, string staffNumber)
        {
            Tuple<int, IMeasureItemVisual> attributesVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, attributeItem);
            AddAttribute(attributeItem);
            itemsWithPosition.Add(attributesVisual);
            itemsPositionsPerStaff[staffNumber].Add(attributesVisual);
        }

        public void AppendAttribute(IAttributeItemVisual attributeItem, int cursorPosition)//temp
        {
            Tuple<int, IMeasureItemVisual> attributesVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, attributeItem);
            AddAttribute(attributeItem);
            itemsWithPosition.Add(attributesVisual);
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
