using MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent.AbstractClasses;
using System;
using System.Linq;
using MusicXMLScore.Model.MeasureItems.Attributes;
using MusicXMLScore.DrawingHelpers;
using System.Windows;
using MusicXMLScore.Helpers;
using MusicXMLScore.Converters;

namespace MusicXMLScore.ScoreLayout.MeasureLayouts.MeasureContent
{
    class MeasureTime : AbstractTime
    {
        private char[] beatTimeChars;
        private string[] beatTimeSymbols;
        private char[] beatTypeChars;
        private string[] beatTypeSymbols;
        private string symbol;
        private double width;
        private bool isVisible =true;

        public MeasureTime(string beatTime, string beatType, AbstractStaff staff) : base(beatTime, beatType, staff)
        {
            if (beatTime.Contains("+"))
            {
                throw new ArgumentException("Compound Time not supported by this class! Use MeasureCompoundTime class.");
            }
            GenerateBeatTimeSymbol();
            GenerateBeatTypeSymbol();
        }

        public MeasureTime(string beatTime, string beatType, TimeSymbolMusicXML timeSymbol, AbstractStaff staff) : base(beatTime, beatType, timeSymbol, staff)
        {
            if (beatTime.Contains("+"))
            {
                throw new ArgumentException("Compound Time not supported by this class! Use MeasureCompoundTime class.");
            }
            GenerateBeatTimeSymbol();
            GenerateBeatTypeSymbol();
            GenerateTimeSymbol();
        }

        public bool IsVisible { get => isVisible; set => isVisible = value; }

        public override double GetVisualWidth()
        {
            return width;
        }

        public override void Update()
        {
            GetVisualsContainer().ClearVisuals();
            Draw();
        }

        private void Draw()
        {
            if (IsVisible)
            {
                if (TimeSymbol != TimeSymbolMusicXML.normal)
                {
                    GetVisualsContainer().AddCharacterGlyph(new Point(0, Staff[Staff.LinesCount / 2 + 1, 1]), symbol);
                    width = DrawingMethods.GetTextWidth(symbol, TypeFaces.GetMusicFont());
                }
                else
                {
                    // digit separator width (scaling independent)
                    var digitSeparator = 2.0.TenthsToWPFUnit();
                    var timeWidths = GetSymbolWidths(beatTimeSymbols);
                    var timeTypeWidths = GetSymbolWidths(beatTypeSymbols);
                    var lengthTime = timeWidths.Sum() + digitSeparator * (timeWidths.Length - 1);
                    var lengthTimeType = timeTypeWidths.Sum() + digitSeparator * (timeTypeWidths.Length - 1);
                    double tempTimeTypeOffset = 0;
                    double tempTimeOffset = 0;
                    if (lengthTime > lengthTimeType)
                    {
                        tempTimeTypeOffset = (Math.Max(lengthTimeType, lengthTime) - Math.Min(lengthTimeType, lengthTime)) / 2;
                    }
                    else
                    {
                        tempTimeOffset = (Math.Max(lengthTimeType, lengthTime) - Math.Min(lengthTimeType, lengthTime)) / 2;
                    }
                    for (int i = 0; i < beatTimeSymbols.Length; i++)
                    {
                        GetVisualsContainer().AddCharacterGlyph(new Point(tempTimeOffset, Staff[4, 1]), beatTimeSymbols[i]);
                        tempTimeOffset += timeWidths[i];
                    }
                    for (int j = 0; j < beatTypeSymbols.Length; j++)
                    {
                        GetVisualsContainer().AddCharacterGlyph(new Point(tempTimeTypeOffset, Staff[2, 1]), beatTypeSymbols[j]);
                        tempTimeTypeOffset += timeTypeWidths[j];
                    }
                    width = lengthTime > lengthTimeType ? lengthTime : lengthTimeType;
                }
            }
        }

        private void GenerateBeatTimeSymbol()
        {
            beatTimeChars = BeatTime.ToCharArray();
            beatTimeSymbols = NumberSymbolArray();
        }

        private void GenerateBeatTypeSymbol()
        {
            beatTypeChars = BeatType.ToCharArray();
            beatTypeSymbols = new string[beatTypeChars.Length];
            for (int i = 0; i < beatTypeChars.Length; i++)
            {
                beatTypeSymbols[i] = MusicSymbols.GetCustomTimeNumber(beatTypeChars[i].ToString());
            }
        }

        private void GenerateTimeSymbol()
        {
            if (TimeSymbol != TimeSymbolMusicXML.normal)
            {
                switch (TimeSymbol)
                {
                    case TimeSymbolMusicXML.common:
                        symbol = MusicSymbols.CommonTime;
                        break;
                    case TimeSymbolMusicXML.cut:
                        symbol = MusicSymbols.CutTime;
                        break;
                    case TimeSymbolMusicXML.singlenumber:
                        symbol = NumberSymbolArray().Aggregate((item1, item2) => item1 + " " + item2);
                        break;
                    case TimeSymbolMusicXML.note:
                        symbol = NumberSymbolArray().Aggregate((item1, item2) => item1 + " " + item2);
                        Console.WriteLine($"Not implemented visual representation of time symbol {TimeSymbol}, displayed {BeatTime} instead");
                        break;
                    case TimeSymbolMusicXML.dottednote:
                        symbol = NumberSymbolArray().Aggregate((item1, item2) => item1 + " " + item2);
                        Console.WriteLine($"Not implemented visual representation of time symbol {TimeSymbol}, displayed {BeatTime} instead");
                        break;
                }
            }
        }

        private double[] GetSymbolWidths(string[] symbols)
        {
            var widths = new double[symbols.Length];
            for (int i = 0; i < symbols.Length; i++)
            {
                widths[i] = DrawingMethods.GetTextWidth(symbols[i], TypeFaces.GetMusicFont());
            }
            return widths;
        }

        private string[] NumberSymbolArray()
        {
            var tempSymbolsArray = new string[beatTimeChars.Length];
            for (int i = 0; i < beatTimeChars.Length; i++)
            {
                tempSymbolsArray[i] = MusicSymbols.GetCustomTimeNumber(beatTimeChars[i].ToString());
            }
            return tempSymbolsArray;
        }
    }
}
