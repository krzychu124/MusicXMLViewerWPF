using MusicXMLScore.Model;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.ViewModel;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.Converters
{
    public static class ExtensionMethods
    {
        public static double PxPerMM()
        {
            return 141 / 25.4; //? 141
        }
        public static double TenthsToMM(this double tenths)
        {
            double converterFactor = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.ConverterFactor;
            double result = tenths * converterFactor;
            return result;
        }

        public static double MMToTenths(this double MM)
        {
            double converterFactor = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.ConverterFactor;
            if (converterFactor == 0)
            {
                return 0.0;
            }
            return MM / converterFactor;
        }

        public static double TenthsToWPFUnit(this double tenths)
        {
            double converterFactor = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.ConverterFactor;
            double result = tenths * converterFactor * PxPerMM();
            return result;
        }

        public static double WPFUnitToTenths(this double WPFUnit)
        {
            if (WPFUnit == 0)
            {
                return 0.0;
            }
            double converterFactor = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.ConverterFactor;
            return WPFUnit / (converterFactor * PxPerMM());
        }
        public static double WPFUnitToMM(this double WPFUnit)
        {
            return WPFUnit * PxPerMM(); //! to test
        }
        public static double MMToWPFUnit(this double MM)
        {
            return MM * PxPerMM(); //! to test
        }
        public static double MMToInch(this double MM)
        {
            return MM / 25.4;
        }
        public static List<List<Tuple<string, string>>> TryGetLinesPerPage(this ScorePartwisePartMusicXML part)
        {
            List<Tuple<string, string>> measuresPerLine = new List<Tuple<string, string>>();
            int pagesCount = part.TryGetNumberOfPages();
            List<List<Tuple<string, string>>> linesPerPage = new List<List<Tuple<string, string>>>();
            string fistNumber= "";
            string lastNumber = "";
            foreach (var measure in part.Measure)
            {
                var print = measure.Items.OfType<PrintMusicXML>().FirstOrDefault();
                
                if (print != null)
                {
                    if (print.NewSystemSpecified)
                    {
                        if (print.NewSystem == Model.Helpers.SimpleTypes.YesNoMusicXML.yes)
                        {
                            lastNumber = part.Measure.ElementAt(part.Measure.IndexOf(measure)-1).Number;
                            Tuple<string, string> t = new Tuple<string, string>(fistNumber, lastNumber);
                            if (part.Measure.IndexOf(measure) != 0)
                            {
                                measuresPerLine.Add(t);
                            }
                            
                            fistNumber = measure.Number;
                        }
                    }
                    if (print.NewPageSpecified)
                    {
                        if (print.NewPage == Model.Helpers.SimpleTypes.YesNoMusicXML.yes)
                        {
                            lastNumber = part.Measure.ElementAt(part.Measure.IndexOf(measure) - 1).Number;
                            Tuple<string, string> t = new Tuple<string, string>(fistNumber, lastNumber);
                            measuresPerLine.Add(t);
                            linesPerPage.Add(new List<Tuple<string, string>>(measuresPerLine));
                            measuresPerLine.Clear();
                            fistNumber = measure.Number;
                            //measuresPerLine.Add(measure.Number);
                        }
                    }
                    if (part.Measure.IndexOf(measure) == 0)
                    {
                        fistNumber = measure.Number;
                        //measuresPerLine.Add(measure.Number);
                    }
                    if (part.Measure.IndexOf(measure) == part.Measure.Count - 1)
                    {
                        lastNumber = measure.Number;
                        Tuple<string, string> t = new Tuple<string, string>(fistNumber, lastNumber);
                        measuresPerLine.Add(t);
                        if (measuresPerLine.Count != 0)
                        {
                            linesPerPage.Add(new List<Tuple<string, string>>(measuresPerLine));
                        }
                    }
                }
                else
                {
                    if (part.Measure.IndexOf(measure) == 0)
                    {
                        fistNumber = measure.Number;
                        //measuresPerLine.Add(measure.Number);
                    }
                    if (part.Measure.IndexOf(measure) == part.Measure.Count - 1)
                    {
                        lastNumber = measure.Number;
                        Tuple<string, string> t = new Tuple<string, string>(fistNumber, lastNumber);
                        measuresPerLine.Add(t);
                        if (measuresPerLine.Count != 0)
                        {
                            linesPerPage.Add(new List<Tuple<string, string>>(measuresPerLine));
                        }
                    }
                    lastNumber = measure.Number;
                }
            }
            return linesPerPage;
        }
        public static int TryGetNumberOfPages(this ScorePartwisePartMusicXML part)
        {
            int result = 1;
            foreach (var measure in part.Measure)
            {
                var print = measure.Items.OfType<PrintMusicXML>().FirstOrDefault();
                if (print != null)
                {
                    var newPage = print.NewPageSpecified ? print.NewPage : Model.Helpers.SimpleTypes.YesNoMusicXML.no;
                    if (newPage == Model.Helpers.SimpleTypes.YesNoMusicXML.yes)
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        public static List<string> TryGetEveryPartId(this ScorePartwiseMusicXML score)
        {
            List<string> result = new List<string>();
            foreach (var part in score.Part)
            {
                if (score.Partlist.ScoreParts.Any(i=> i.PartId == part.Id))
                {
                    result.Add(part.Id);
                }
            }
            return result;
        }
        public static List<string> TryGetMeasuresIdRange(this ScorePartwisePartMusicXML part, Tuple<string, string> rangeOfMeasures)
        {
            List<string> measuresRange = new List<string>();
            int startindex = part.Measure.IndexOf(part.Measure.Select(i => i).Where(i => i.Number == rangeOfMeasures.Item1).FirstOrDefault());
            int endindex = part.Measure.IndexOf(part.Measure.Select(i => i).Where(i => i.Number == rangeOfMeasures.Item2).FirstOrDefault());
            int count = endindex -startindex +1;
            measuresRange = part.Measure.GetRange(startindex, count).Select(i=>i.Number).ToList();
            return measuresRange;
        }
        public static Point GetMeasurePosition(this ScorePartwisePartMeasureMusicXML measure, Dictionary<string, Point> measureCoordsList)
        {
            Point position = new Point();
            if (measureCoordsList != null)
            {
                if (measureCoordsList.ContainsKey(measure.Number))
                {
                    position = measureCoordsList[measure.Number];
                }
            }
            return position;
        }
        public static double GetLargestWidth(this ScorePartwiseMusicXML score, string measureId)
        {
            double result = score.Part.ElementAt(0).GetMeasureUsingId(measureId).Width;
            foreach (var part in score.Part)
            {
                double width = part.GetMeasureUsingId(measureId).Width;
                if (width > result)
                {
                    result = width;
                }
            }
            return result;
        }
        public static ScorePartwisePartMeasureMusicXML GetMeasureUsingId(this ScorePartwisePartMusicXML part, string measureId)
        {
            var measure = part.Measure.Select(i => i).Where(i => i.Number == measureId).FirstOrDefault();
            return measure;
        }
        public static void SetLargestWidth(this ScorePartwiseMusicXML score)
        {
            foreach (var measure in score.Part.ElementAt(0).Measure)
            {
                string id = measure.Number;
                var maxWidth = score.GetLargestWidth(id);
                if (maxWidth == 0)
                {
                    maxWidth = 60;
                }
                foreach (var part in score.Part)
                {
                    var m = part.GetMeasureUsingId(id);
                    m.CalculatedWidth = maxWidth;
                }
            }
        }
        public static Dictionary<string, DrawingHelpers.PartProperties> CorrectCoords(this Dictionary<string, DrawingHelpers.PartProperties> pp)
        {
            Dictionary<string, DrawingHelpers.PartProperties> result = new Dictionary<string, DrawingHelpers.PartProperties>();
            var coords = pp.ElementAt(0).Value.Coords;
            List<List<string>> systems = pp.ElementAt(0).Value.MeasuresPerSystem;
            double correction = 0;
            foreach (var coord in pp.Select(i => i).Where(i => i.Key != pp.ElementAt(0).Key))
            {
                correction += coord.Value.Coords.ElementAt(0).Value.Y;
            }
            correction -= coords.ElementAt(0).Value.Y;
            foreach (var measureline in systems)
            {
                if (systems.IndexOf(measureline) != 0)
                {
                    for (int i = 0; i < measureline.Count; i++)
                    {
                        coords[measureline[i]] = new Point(coords[measureline[i]].X, coords[measureline[i]].Y + correction);
                    }
                    correction = 0;
                    int index = coords.Keys.ToList().IndexOf(measureline.ElementAt(0));
                    foreach (var coord in pp.Select(i => i).Where(i => i.Key != pp.ElementAt(0).Key))
                    {
                        correction += coord.Value.Coords.ElementAt(index).Value.Y;
                    }
                    correction -= coords.ElementAt(index).Value.Y;
                }
            }

            return result;
        }
        public static double CalculateWidth(this List<string> measuresIds, string partId)
        {
            double result = 0.0;
            var part = ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.Where(i => i.Id == partId).FirstOrDefault();
            foreach(var measureId in measuresIds)
            {
                result += part.MeasuresByNumber[measureId].CalculatedWidth;
            }

            return result;
        }
        public static int GetPartIdIndex(this string partID)
        {
            return ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.IndexOf(ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.Where(i => i.Id == partID).FirstOrDefault());
        }
        public static int GetMeasureIdIndex(this string measureId)
        {
            var measure = ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(0).MeasuresByNumber[measureId];
            return ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(0).Measure.IndexOf(measure);
        }

        public static ushort GetGlyphIndexOfCharacter(this string symbolCharacter)
        {
            int symbol = (int)symbolCharacter.ToCharArray().FirstOrDefault();
            GlyphTypeface glyph;
            GlyphTypeface typeface = Helpers.TypeFaces.GetMusicFont().TryGetGlyphTypeface(out glyph) ? glyph : null;
            ushort glyphindex;
            glyph.CharacterToGlyphMap.TryGetValue(symbol, out glyphindex);
            return glyphindex;
        }
        public static int GetValueIndexFromObjectArray<T>(this T[] array, T type)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(type))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
