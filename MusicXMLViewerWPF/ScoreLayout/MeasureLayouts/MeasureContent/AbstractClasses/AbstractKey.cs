﻿using MusicXMLScore.Model.Helpers.SimpleTypes;
using MusicXMLScore.Model.MeasureItems.Attributes;
using System;
using MusicXMLScore.Helpers;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses
{
    abstract class AbstractKey : IVisualHost
    {
        protected int keyFifths;
        protected readonly DrawingVisualHost drawingVisualHost;
        protected AccidentalValueMusicXML[] keyAccidentals;
        protected int[] keyStaffSpaceIndex;
        protected AbstractStaff staff;

        protected AbstractKey(ClefSignMusicXML clefSign, int line, int keyFifths)
        {
            drawingVisualHost = new DrawingVisualHost();
            keyAccidentals = GetKeyAccidentals(keyFifths);
            keyStaffSpaceIndex = GetKeyStaffSpaceIndex(clefSign, line, keyFifths >0 ? false:true);
            this.keyFifths = keyFifths;
        }

        public void SetStaff(AbstractStaff staff)
        {
            this.staff = staff;
        }

        protected AccidentalValueMusicXML[] GetKeyAccidentals(int keyFifths)
        {
            var result = new AccidentalValueMusicXML[] { AccidentalValueMusicXML.none };
            if (keyFifths > -8 && keyFifths < 8 && keyFifths !=0)
            {
                var symbol = keyFifths > 0 ? AccidentalValueMusicXML.sharp : AccidentalValueMusicXML.flat;
                var count = Math.Abs(keyFifths);
                result = new AccidentalValueMusicXML[count];
                for (int i = 0; i < count; i++)
                {
                    result[i] = symbol;
                }
            }
            return result;
        }

        public int[] GetKeyStaffSpaceIndex(ClefSignMusicXML clefSign, int line, bool isFlat)
        {
            var result = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            switch (clefSign)
            {
                case ClefSignMusicXML.G:
                    switch (line)
                    {
                        case 2:
                            result = isFlat ? new int[] { 5, 2, 6, 3, 7, 4, 8 } : new int[] { 1, 4, 0, 3, 6, 2, 5 };
                            break;
                        case 1:
                            result = isFlat ? new int[] { 7, 4, 8, 5, 9, 6, 10 } : new int[] { 3, 6, 3, 5, 8, 4, 7 };
                            break;
                        default:
                            Console.WriteLine($"Invalid line index: {line} for clef {clefSign}");
                            break;
                    }
                    break;
                case ClefSignMusicXML.F:
                    switch (line)
                    {
                        case 3:
                            result = isFlat ? new int[] { 2, 6, 3, 7, 4, 8, 5 } : new int[] { 5, 1, 4, 0, 3, 6, 2 };
                            break;
                        case 4:
                            result = isFlat ? new int[] { 7, 4, 8, 5, 9, 6, 10 } : new int[] { 3, 6, 3, 5, 8, 4, 7 };
                            break;
                        case 5:
                            result = isFlat ? new int[] { 5, 2, 6, 3, 7, 4, 8 } : new int[] { 1, 4, 0, 3, 6, 2, 5 };
                            break;
                        default:
                            Console.WriteLine($"Invalid line index: {line} for clef {clefSign}");
                            break;
                    }
                    break;
                case ClefSignMusicXML.C:
                    switch (line)
                    {
                        case 1:
                            result = isFlat ? new int[] { 3, 7, 4, 8, 5, 9, 6 } : new int[] { 6, 2, 5, 1, 4, 0, 3 };
                            break;
                        case 2:
                            result = isFlat ? new int[] { 1, 5, 2, 6, 3, 7, 4 } : new int[] { 4, 7, 3, 6, 2, 5, 1 };
                            break;
                        case 3:
                            result = isFlat ? new int[] { 6, 3, 7, 4, 8, 5, 9 } : new int[] { 2, 5, 1, 4, 7, 3, 6 };
                            break;
                        case 4:
                            result = isFlat ? new int[] { 4, 1, 5, 2, 6, 3, 7 } : new int[] { 7, 3, 6, 2, 5, 1, 4 };
                            break;
                        case 5:
                            result = isFlat ? new int[] { 2, 6, 3, 7, 4, 8, 5 } : new int[] { 5, 1, 4, 0, 3, 6, 2 };
                            break;
                        default:
                            Console.WriteLine($"Invalid line index: {line} for clef {clefSign}");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine($"Invalid clef sign: {clefSign}");
                    break;
            }
            return result;
        }

        public DrawingVisualHost GetVisualsContainer()
        {
            return drawingVisualHost;
        }

        public abstract double GetVisualWidth();

        public abstract void Update();

        protected abstract void ChangeKeyAccidentals(AccidentalValueMusicXML[] newAccidentals, ClefSignMusicXML newClefSign, int newClefLine, bool newIsFlatKey);
    }
}
