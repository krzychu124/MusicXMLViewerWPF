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
        /// <summary>
        /// Gets WPFUnit - Millimeter conversion factor
        /// </summary>
        /// <returns></returns>
        public static double PxPerMM()
        {
            return 141 / 25.4; //? 141
        }

        /// <summary>
        /// Converst MusicXML Tenths to Millimeters
        /// </summary>
        /// <param name="tenths"></param>
        /// <returns></returns>
        public static double TenthsToMM(this double tenths)
        {
            double converterFactor = ViewModelLocator.Instance.Main.CurrentPageLayout.ConverterFactor;
            double result = tenths * converterFactor;
            return result;
        }

        /// <summary>
        /// Converts Millimeters to MusicXML Tenths
        /// </summary>
        /// <param name="MM"></param>
        /// <returns></returns>
        public static double MMToTenths(this double MM)
        {
            double converterFactor = ViewModelLocator.Instance.Main.CurrentPageLayout.ConverterFactor;
            if (converterFactor == 0)
            {
                return 0.0;
            }
            return MM / converterFactor;
        }

        /// <summary>
        /// Converts MusicXML Tenths to WPFUnits(highly related to DPI)
        /// </summary>
        /// <param name="tenths"></param>
        /// <returns></returns>
        public static double TenthsToWPFUnit(this double tenths)
        {
            double converterFactor = ViewModelLocator.Instance.Main.CurrentPageLayout.ConverterFactor;
            double result = tenths * converterFactor * PxPerMM();
            return result;
        }

        /// <summary>
        /// Converts WPFUnit to MusicXML Tenths
        /// </summary>
        /// <param name="WPFUnit"></param>
        /// <returns></returns>
        public static double WPFUnitToTenths(this double WPFUnit)
        {
            if (WPFUnit == 0)
            {
                return 0.0;
            }
            double converterFactor = ViewModelLocator.Instance.Main.CurrentPageLayout.ConverterFactor;
            return WPFUnit / (converterFactor * PxPerMM());
        }
        public static double WPFUnitToMM(this double WPFUnit)
        {
            return WPFUnit * PxPerMM(); //! to test
        }
        
        /// <summary>
        /// Converts Millimeters to WPFUnits
        /// </summary>
        /// <param name="MM"></param>
        /// <returns></returns>
        public static double MMToWPFUnit(this double MM)
        {
            return MM * PxPerMM(); //! to test
        }

        /// <summary>
        /// Converts Millimeters to Inches
        /// </summary>
        /// <param name="MM"></param>
        /// <returns></returns>
        public static double MMToInch(this double MM)
        {
            return MM / 25.4;
        }

        /// <summary>
        /// Extension method, search for layout elements inside Part object and convers them to list of pages
        /// </summary>
        /// <param name="part"></param>
        /// <returns>Collection of Pages(Collections of system(first measure number, last measure number)</returns>
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
                        }
                    }
                    if (part.Measure.IndexOf(measure) == 0)
                    {
                        fistNumber = measure.Number;
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

        /// <summary>
        /// Search for new-page layout element of measure to calculate Pages count (if loaded score supports layout elements and elements were set)
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets Parts Id list
        /// </summary>
        /// <param name="score"></param>
        /// <returns>Collection of Part Id</returns>
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

        /// <summary>
        /// Generates measure Number between rangeOfMeasures(first,last) of selected Part object
        /// </summary>
        /// <param name="part"></param>
        /// <param name="rangeOfMeasures"></param>
        /// <returns>Collection of measure Numbers between first and last measure Number of range</returns>
        public static List<string> TryGetMeasuresIdRange(this ScorePartwisePartMusicXML part, Tuple<string, string> rangeOfMeasures)
        {
            List<string> measuresRange = new List<string>();
            int startindex = part.Measure.IndexOf(part.Measure.Select(i => i).Where(i => i.Number == rangeOfMeasures.Item1).FirstOrDefault());
            int endindex = part.Measure.IndexOf(part.Measure.Select(i => i).Where(i => i.Number == rangeOfMeasures.Item2).FirstOrDefault());
            int count = endindex -startindex +1;
            measuresRange = part.Measure.GetRange(startindex, count).Select(i=>i.Number).ToList();
            return measuresRange;
        }

        [Obsolete("Refactored")]
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

        /// <summary>
        /// Finds Largest measure width of passed measureNumber among all parts of Score
        /// </summary>
        /// <param name="score"></param>
        /// <param name="measureNumber"></param>
        /// <returns>Largest Measure width among all parts</returns>
        public static double GetLargestWidth(this ScorePartwiseMusicXML score, string measureNumber)
        {
            double result = score.Part.ElementAt(0).GetMeasureUsingId(measureNumber).Width;
            foreach (var part in score.Part)
            {
                double width = part.GetMeasureUsingId(measureNumber).Width;
                if (width > result)
                {
                    result = width;
                }
            }
            return result;
        }

        /// <summary>
        /// Search for measure object in this part using measure Number
        /// </summary>
        /// <param name="part">Selected Part for searching</param>
        /// <param name="measureNumber">Number value (unique for each measure inside one part)</param>
        /// <returns></returns>
        public static ScorePartwisePartMeasureMusicXML GetMeasureUsingId(this ScorePartwisePartMusicXML part, string measureNumber)
        {
            var measure = part.Measure.Select(i => i).Where(i => i.Number == measureNumber).FirstOrDefault();
            return measure;
        }

        /// <summary>
        /// Find and set largest measure width among all parts 
        /// </summary>
        /// <param name="score"></param>
        public static void SetLargestMeasureWidth(this ScorePartwiseMusicXML score)
        {
            foreach (var measure in score.Part.ElementAt(0).Measure)
            {
                string id = measure.Number;
                var maxWidth = score.GetLargestWidth(id);
                if (maxWidth == 0)
                {
                    maxWidth = 120;
                }
                foreach (var part in score.Part)
                {
                    var m = part.GetMeasureUsingId(id);
                    m.CalculatedWidth = maxWidth;
                }
            }
        }
        
        [Obsolete("Refactored")]
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

        /// <summary>
        /// Aggregates all measure widths of selected part Id
        /// </summary>
        /// <param name="measuresIds">Collection of measure Id's</param>
        /// <param name="partId">Selected part ID</param>
        /// <returns>Aggregated measure width</returns>
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

        /// <summary>
        /// Gets index of part using part Id
        /// </summary>
        /// <param name="partID"></param>
        /// <returns></returns>
        public static int GetPartIdIndex(this string partID)
        {
            return ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.IndexOf(ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.Where(i => i.Id == partID).FirstOrDefault());
        }

        /// <summary>
        /// Gets measure index using measure Id (unique Number) 
        /// </summary>
        /// <param name="measureId"></param>
        /// <returns></returns>
        public static int GetMeasureIdIndex(this string measureId)
        {
            var measure = ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(0).MeasuresByNumber[measureId];
            return ViewModelLocator.Instance.Main.CurrentSelectedScore.Part.ElementAt(0).Measure.IndexOf(measure);
        }

        /// <summary>
        /// Finds index of char symbol from GlyphMap
        /// </summary>
        /// <param name="symbolCharacter"></param>
        /// <returns></returns>
        public static ushort GetGlyphIndexOfCharacter(this string symbolCharacter)
        {
            int symbol = (int)symbolCharacter.ToCharArray().FirstOrDefault();
            GlyphTypeface glyph;
            GlyphTypeface typeface = Helpers.TypeFaces.GetMusicFont().TryGetGlyphTypeface(out glyph) ? glyph : null;
            ushort glyphindex;
            glyph.CharacterToGlyphMap.TryGetValue(symbol, out glyphindex);
            return glyphindex;
        }

        /// <summary>
        /// Gets index of type from this Array[Type] to find value in Array[Type.Value], more or less ;)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculates visual width of passed characters array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] GetCharsVisualWidth(this char[] array)
        {
            double[] widths = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                widths[i] = DrawingHelpers.DrawingMethods.GetTextWidth(array[i].ToString(), Helpers.TypeFaces.GetMusicFont());
            }
            return widths;
        }
    }
}
