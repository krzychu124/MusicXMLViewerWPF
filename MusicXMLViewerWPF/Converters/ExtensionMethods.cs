using MusicXMLScore.Model;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.Converters
{
    public static class ExtensionMethods
    {
        public static double PxPerMM()
        {
            return 141 / 25.4;
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
        public static List<List<string>> TryGetLinesPerPage(this ScorePartwisePartMusicXML part)
        {
            List<string> measuresPerLine = new List<string>();
            int pagesCount = part.TryGetNumberOfPages();
            List<List<string>> linesPerPage = new List<List<string>>();
            foreach (var measure in part.Measure)
            {
                var print = measure.Items.OfType<PrintMusicXML>().FirstOrDefault();
                if (print != null)
                {
                    if (print.NewSystemSpecified)
                    {
                        if (print.NewSystem == Model.Helpers.SimpleTypes.YesNoMusicXML.yes)
                        {
                            measuresPerLine.Add(measure.Number);
                        }
                    }
                    if (print.NewPageSpecified)
                    {
                        if (print.NewPage == Model.Helpers.SimpleTypes.YesNoMusicXML.yes)
                        {
                            linesPerPage.Add(new List<string>(measuresPerLine));
                            measuresPerLine.Clear();
                            measuresPerLine.Add(measure.Number);
                        }
                    }
                    if (part.Measure.IndexOf(measure) == 0)
                    {
                        measuresPerLine.Add(measure.Number);
                    }
                    if (part.Measure.IndexOf(measure) == part.Measure.Count - 1)
                    {
                        if (measuresPerLine.Count != 0)
                        {
                            linesPerPage.Add(new List<string>(measuresPerLine));
                        }
                    }
                }
                else
                {
                    if (part.Measure.IndexOf(measure) == 0)
                    {
                        measuresPerLine.Add(measure.Number);
                    }
                    if (part.Measure.IndexOf(measure) == part.Measure.Count - 1)
                    {
                        if (measuresPerLine.Count != 0)
                        {
                            linesPerPage.Add(new List<string>(measuresPerLine));
                        }
                    }
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
    }
}
