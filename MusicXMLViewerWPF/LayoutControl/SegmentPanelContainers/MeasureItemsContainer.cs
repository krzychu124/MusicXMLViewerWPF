﻿using MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes;
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
using System.Windows;
using System.Windows.Media;
using MusicXMLScore.Helpers;

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
        private Canvas temporaryBarline;
        private Canvas temporaryStartBarline;

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
                        note.Stem?.SetStaffOffset(top); //! bug fix (nullable check - stem could be null)
                    }
                    Canvas.SetTop(item.Item2.ItemCanvas as Canvas, top);
                }
            }
            double tempHeight = staffsNumber == 1 ? 0.0 : staffDistance; //! temp
            DrawTempBarline(tempHeight + 40.0.TenthsToWPFUnit()); //! Temp barline (black)
            DrawTempStartBarline(tempHeight + 40.0.TenthsToWPFUnit()); //! Temp 0.X position barline (red)
        }

        public void ArrangeUsingDurationTable(Dictionary<int, double> durationTable, bool update = false)
        {
            //-----------------
            // Keys: -3, -2, -1 used for attributes: clef, key and time
            //-----------------
            var measureBeginningAttributes = itemsWithPosition.Where(x => x.Item1 == 0 && x.Item2 is IAttributeItemVisual).Select(x => x.Item2).ToList();
            if (measureBeginningAttributes.Count != 0)
            { 
                //! ------------------ CLefs position set--------------------
                var clef = measureBeginningAttributes.Where(x => x is ClefContainerItem).ToList();//as ClefContainerItem;
                if (clef.Count != 0)
                {
                    foreach (var c in clef)
                    {
                        if (durationTable.ContainsKey(-3))
                        {
                            ArrangeAttributes(c as ClefContainerItem, new Dictionary<string, double>(), durationTable[-3]);
                        }
                    }
                }
                //! ------------------ Key Signatures position set--------------------
                var key = measureBeginningAttributes.Where(x => x is KeyContainerItem).ToList();
                if (key.Count != 0)
                {
                    foreach (var item in key)
                    {
                        if (durationTable.ContainsKey(-2))
                        {
                            ArrangeAttributes(item as KeyContainerItem, new Dictionary<string, double>(), durationTable[-2]);
                        }
                    }
                }
                //! ------------------ Time Signatures position set--------------------
                var time = measureBeginningAttributes.Where(x => x is TimeSignatureContainerItem).ToList();
                if (time.Count != 0)
                {
                    foreach (var item in time)
                    {
                            if (durationTable.ContainsKey(-1))
                            {
                                ArrangeAttributes(item as TimeSignatureContainerItem, new Dictionary<string, double>(), durationTable[-1]);
                            }
                    }
                }
            }
            //? possible measure rest searching (if one and only one rest item per staff line)
            Dictionary<string, int> notesPerStaff = new Dictionary<string, int>();
            foreach (var item in itemsPositionsPerStaff)
            {
                int notesCount = item.Value.Select(x => x.Item2).Where(x => x as INoteItemVisual != null).Count();
                notesPerStaff.Add(item.Key, notesCount);
            }
            //! ------------------ Note items and additional Attribute items position set--------------------
            foreach (var item in itemsWithPosition)
            {
                if (item.Item2 is INoteItemVisual)
                {
                    INoteItemVisual note = item.Item2 as INoteItemVisual;
                    if (note.ItemDuration == 0)
                    {
                        //! grace notes has duration == 0
                        //! skipped due to possible wrong placement / overwriting previous position setting
                        continue; 
                    }
                    if (note is RestContainterItem)
                    {
                        if (notesPerStaff[note.ItemStaff] == 1) //! check if is rest and alone (same behaviour as measure rest)
                        {
                            //! calculate center position taking into account possible attributes width
                            //! durationTable contains Measure width atl highest key position
                            double centeredPosition = ((durationTable.Values.Max() - durationTable[0]) / 2.0) + durationTable[0];
                            SetLeft(item.Item2.ItemCanvas as Canvas, centeredPosition);
                            continue; //! skip further conditions
                        }
                    }
                    if (update)
                    {
                        if (note is NoteContainerItem)
                        {
                            var noteObject = item.Item2 as NoteContainerItem;
                            noteObject.UpdateStemsAndBeams();
                        }
                    }
                    SetLeft(item.Item2.ItemCanvas as Canvas, durationTable[item.Item1]);
                }

                //! additional attributes (midmeasure and others)
                if (item.Item2 is IAttributeItemVisual && item.Item1 >0) 
                {
                    SetLeft(item.Item2.ItemCanvas as Canvas, durationTable[item.Item1] - item.Item2.ItemWidth);
                }
            }
            SetLeft(temporaryBarline, durationTable.LastOrDefault().Value); //! Temp barline position test to end of measure (advanced layout visual helper)
        }
        private void DrawTempBarline(double staffHeight)
        {
            temporaryBarline = new Canvas();
            Point p1 = new Point();
            Point p2 = new Point(0,staffHeight);
            Pen pen = new Pen(Brushes.Black, 1.5.TenthsToWPFUnit());
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawLine(pen, p1, p2);
            }
            DrawingVisualHost dvh = new DrawingVisualHost();
            dvh.AddVisual(dv);
            temporaryBarline.Children.Add(dvh);
            Children.Add(temporaryBarline);
        }
        private void DrawTempStartBarline(double staffHeight)
        {
            temporaryStartBarline = new Canvas();
            Point p1 = new Point();
            Point p2 = new Point(0, staffHeight * 1.2);
            Pen pen = new Pen(Brushes.Red, 1.5.TenthsToWPFUnit());
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawLine(pen, p1, p2);
            }
            DrawingVisualHost dvh = new DrawingVisualHost();
            dvh.AddVisual(dv);
            temporaryStartBarline.Children.Add(dvh);
            Children.Add(temporaryStartBarline);
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
                    width += clef.ItemLeftMargin; 
                    SetLeft(clef.ItemCanvas, width);
                    width += clef.ItemWidth + clef.ItemRightMargin;
                    break;
                case 1:
                    KeyContainerItem key = attributeVisual as KeyContainerItem;
                    width += key.ItemLeftMargin;
                    SetLeft(key.ItemCanvas, width);
                    width += key.ItemWidth + (key.ItemWidth != 0 ? key.ItemRightMargin: 0);
                    break;
                case 2:
                    TimeSignatureContainerItem timeSig = attributeVisual as TimeSignatureContainerItem;
                    width += timeSig.ItemLeftMargin;
                    SetLeft(timeSig.ItemCanvas, width);
                    width += timeSig.ItemWidth + timeSig.ItemRightMargin;
                    break;
            }

            return width;
        }

        public double GetMinimalContentWidth()
        {
            var perStaffMinWidth = itemsPositionsPerStaff.Select(x => x.Value).Select(z => z.Where(y=>y.Item1 !=0 ||!(y.Item1 == 0 && y.Item2 is IAttributeItemVisual)).Select(x => x.Item2.ItemLeftMargin + x.Item2.ItemRightMargin + x.Item2.ItemWidth /*+ (x.Item2 is INoteItemVisual? 2.0.TenthsToWPFUnit():0.0)*/).Sum()).Max();
            return perStaffMinWidth;
        }

        public void AppendNoteWithStaffNumber(NoteContainerItem note, int cursorPosition, string voice, string staffNumber)
        {
            Tuple<int, IMeasureItemVisual> noteVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, note);
            AddNote(note);
            itemsWithPosition.Add(noteVisual);
            itemsPositionsPerStaff[staffNumber].Add(noteVisual);
        }
        public void AppendRestWithStaffNumber(RestContainterItem rest, int cursorPosition, string voice, string staffNumber)
        {
            Tuple<int, IMeasureItemVisual> restVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, rest);
            AddRest(rest);
            itemsWithPosition.Add(restVisual);
            itemsPositionsPerStaff[staffNumber].Add(restVisual);
        }
        public void AppendAttributeWithStaffNumber(IAttributeItemVisual attributeItem, int cursorPosition, string staffNumber)
        {
            Tuple<int, IMeasureItemVisual> attributesVisual = new Tuple<int, IMeasureItemVisual>(cursorPosition, attributeItem);
            AddAttribute(attributeItem);
            itemsWithPosition.Add(attributesVisual);
            itemsPositionsPerStaff[staffNumber].Add(attributesVisual);
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
            Children.Add(attributeVisual.ItemCanvas as Canvas); 
        }
    }
}
