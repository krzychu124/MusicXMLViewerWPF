using MusicXMLScore.Converters;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutControl.SegmentPanelContainers.Notes
{
    public static class CalculationHelpers
    {
        /// <summary>
        /// Converts duration of Note/Rest according to divisions of measure.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="partId"></param>
        /// <param name="measureId"></param>
        /// <returns>Tuple of NoteTypeValue; true if has dot, otherwise false</returns>
        public static Tuple<NoteTypeValueMusicXML,bool> GetBaseDurationValue(int duration, string partId, string measureId)
        {
            NoteTypeValueMusicXML result = NoteTypeValueMusicXML.whole;
            int divisions = ViewModel.ViewModelLocator.Instance.Main.CurrentPartsProperties[partId].GetDivisionsMeasureId(measureId);
            double durationFactor = (double)duration / (double)divisions;
            int quarterDivider = (int)((int)NoteDurationValuesInverted.quarter * durationFactor);
            var convertedDuration = quarterDivider;
            bool hasDot = !convertedDuration.IsPowerOfTwo();
            if (!hasDot)
            {
                NoteDurationValuesInverted baseNoteDuration;
                Enum.TryParse<NoteDurationValuesInverted>(convertedDuration.ToString(), out baseNoteDuration);
                result = baseNoteDuration.GetNoteTypeFromNoteDuration();
            }
            else
            {
                int flooredDuration = FloorPowerOfTwo(convertedDuration);
                NoteDurationValuesInverted baseNoteDuration;
                Enum.TryParse<NoteDurationValuesInverted>(flooredDuration.ToString(), out baseNoteDuration);
                result = baseNoteDuration.GetNoteTypeFromNoteDuration();
            }
            return new Tuple<NoteTypeValueMusicXML, bool>(result, hasDot);
        }

        /// <summary>
        /// Returns highest previous power of 2 e.g. 43>>32, 130>>128 etc...
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int FloorPowerOfTwo(int number)
        {
            int result = CeilingPowerOfTwo(number);
            result >>= 1;
            return result;
        }
        /// <summary>
        /// Return lowest power of 2 value but higher than input value e.g. 43>>64, 130>>256
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int CeilingPowerOfTwo(int number)
        {
            int result = 1;
            while (result < number)
            {
                result <<= 1;
            }
            return result;
        }
        /// <summary>
        /// Converts NoteDurationValue to NoteTypeValue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NoteTypeValueMusicXML GetNoteTypeFromNoteDuration(this NoteDurationValuesInverted value)
        {
            NoteTypeValueMusicXML result;
            Enum.TryParse<NoteTypeValueMusicXML>(value.ToString(), out result);
            return result;
        }

        /// <summary>
        /// Check if number is a power of 2
        /// </summary>
        /// <param name="x">Value to check</param>
        /// <returns>false if number is power of 2</returns>
        public static bool IsPowerOfTwo(this int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }
    }
}
