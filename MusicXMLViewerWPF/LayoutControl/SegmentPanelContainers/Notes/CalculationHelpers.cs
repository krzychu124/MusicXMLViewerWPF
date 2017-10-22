using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;

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
            int divisions = ViewModel.ViewModelLocator.Instance.Main.PartsProperties[partId].GetDivisionsMeasureId(measureId);
            double durationFactor = duration / (double)divisions;
            int quarterDivider = (int)((int)NoteDurationValuesInverted.quarter * durationFactor);
            var convertedDuration = quarterDivider;
            bool hasDot = !convertedDuration.IsPowerOfTwo();
            if (!hasDot)
            {
                NoteDurationValuesInverted baseNoteDuration;
                Enum.TryParse(convertedDuration.ToString(), out baseNoteDuration);
                result = baseNoteDuration.GetNoteTypeFromNoteDuration();
            }
            else
            {
                int flooredDuration = FloorPowerOfTwo(convertedDuration);
                NoteDurationValuesInverted baseNoteDuration;
                Enum.TryParse(flooredDuration.ToString(), out baseNoteDuration);
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
            Enum.TryParse(value.ToString(), out result);
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

        /// <summary>
        /// Converts pitch value from PitchMusciXML object into index of position at staff line using clef info
        /// </summary>
        /// <param name="pitch">PitchMusicXML object to convert</param>
        /// <param name="clef">ClefMusisXML object as helper for calculations</param>
        /// <returns>Index of position at Staff Line </returns>
        public static int GetPitchIndexStaffLine(this Model.MeasureItems.PitchMusicXML pitch, Model.MeasureItems.Attributes.ClefMusicXML clef)
        {
            int pitchOctave = int.Parse(pitch.Octave);
            int pitchStep = (int)pitch.Step; //0 = A, +5 from C
            int clefOctave = clef.GetMiddleCIndexFromClef();
            int caclulatedStaffLineShift = (clefOctave -(pitchStep +2)) - (-7 * (4 - pitchOctave));
            return caclulatedStaffLineShift;
        }

        public static int GetMiddleCIndexFromClef(this Model.MeasureItems.Attributes.ClefMusicXML clef)
        {
            int index = 6; // G L=2
            switch (clef.Sign)
            {
                case Model.MeasureItems.Attributes.ClefSignMusicXML.G:
                    var lineG = int.Parse(clef.Line);
                    index = 10 - ((lineG - 1) * 2) + 4 +(int.Parse(clef.ClefOctaveChange) * 7);
                    break;
                case Model.MeasureItems.Attributes.ClefSignMusicXML.F:
                    var lineF = int.Parse(clef.Line);
                    index = 10 - ((lineF - 1) * 2) - 4 +(int.Parse(clef.ClefOctaveChange) * 7);
                    break;
                case Model.MeasureItems.Attributes.ClefSignMusicXML.C:
                    var lineC = int.Parse(clef.Line);
                    index = 10 - ((lineC - 1) * 2) + (int.Parse(clef.ClefOctaveChange) * 7);
                    break;
                case Model.MeasureItems.Attributes.ClefSignMusicXML.percussion:
                    index = 6;
                    break;
                case Model.MeasureItems.Attributes.ClefSignMusicXML.none:
                    index = 6;
                    break;
            }
            return index;
        }
    }
}
