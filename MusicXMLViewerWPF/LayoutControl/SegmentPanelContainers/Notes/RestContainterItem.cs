using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.Converters;


namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    class RestContainterItem : Canvas, INoteItemVisual
    {
        private NoteMusicXML noteItem;
        private int itemIndex;
        private bool measureRest = false;
        private double positionY = 0.0;
        private string symbol;
        private string partId;
        private string measureId;
        private int dotCount = 0;           
        public bool MeasureRest
        {
            get
            {
                return measureRest;
            }

            set
            {
                measureRest = value;
            }
        }

        public RestContainterItem(NoteMusicXML note, int itemIndex, string partId, string measureId)
        {
            noteItem = note;
            this.itemIndex = itemIndex;
            this.partId = partId;
            this.measureId = measureId;
            Draw(CheckIfMeasure());
        }

        private bool CheckIfMeasure()
        {
            if (noteItem.Items.OfType<RestMusicXML>().FirstOrDefault().MeasureSpecified)
            {
                return noteItem.Items.OfType<RestMusicXML>().FirstOrDefault().Measure == Model.Helpers.SimpleTypes.YesNoMusicXML.yes ? true : false;
            }
            return true; //false;
        }

        private void Draw(bool measure)
        {
            
            if (measure)
            {
                measureRest = true;
                GetSymbol();
                SetPosition();
                CanvasList rest = new CanvasList(10, 10);
                rest.AddCharacterGlyph(new Point(0, positionY), symbol);
                Children.Add(rest);
            }
            else
            {

            }
        }

        private void SetPosition(int customPosition = 0, bool useCustom = false)
        {
            var staffLine = ViewModel.ViewModelLocator.Instance.Main.CurrentPageProperties.AvaliableIndexLinePositions;
            if (!useCustom)
            {
                positionY = staffLine[4];
            }
            else
            {
                positionY = staffLine[customPosition];
            }
        }

        private void GetSymbol()
        {
            int duration = int.Parse(noteItem.Items.OfType<decimal>().FirstOrDefault().ToString());
            NoteTypeValueMusicXML value = CalculationHelpers.GetBaseDurationValue(duration, partId, measureId).Item1; // item1 =NoteType... item2 true if has dot/dots
            symbol = MusicSymbols.GetRestSymbolNoteType(value);
        }
    }
}
