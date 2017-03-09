using MusicXMLScore.Converters;
using MusicXMLScore.Model.Defaults;
using MusicXMLScore.Model.MeasureItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace MusicXMLScore.DrawingHelpers
{
    public class PartProperties //TODO_H Clean up unneccessary methods, field and properties
    {
        List<SystemLayoutMusicXML> systemLayout = new List<SystemLayoutMusicXML>();
        List<List<SystemLayoutMusicXML>> systemLayoutPerPage = new List<List<SystemLayoutMusicXML>>();
        double defaultSystemDistance = 0.0;
        double defaultTopSystemDistance = 0.0;
        double defaultStaffDistance = 0.0;
        List<StaffLayoutMusicXML> staffLayout = new List<StaffLayoutMusicXML>();
        List<List<StaffLayoutMusicXML>> staffLayoutPerPage = new List<List<StaffLayoutMusicXML>>();
        List<MeasureNumberingMusicXML> measureNumbering = new List<MeasureNumberingMusicXML>();
        int numberOfStaves = 1;
        double stavesDistance = 0.0;
        double partHeight = 0;
        List<List<Tuple<string, string>>> partSysemsInPages;
        List<Tuple<string, string>> measuresRangeInPartSystem = new List<Tuple<string, string>>();
        Dictionary<string, Point> coords;
        int partIndex = 0;
        List<List<string>> measuresPerSystem = new List<List<string>>();
        List<List<List<string>>> measuresPerSystemPerPage = new List<List<List<string>>>();
        public Dictionary<string, Point> Coords
        {
            get
            {
                return coords;
            }

            set
            {
                coords = value;
            }
        }

        public List<List<string>> MeasuresPerSystem
        {
            get
            {
                return measuresPerSystem;
            }

            set
            {
                measuresPerSystem = value;
            }
        }

        public List<StaffLayoutMusicXML> StaffLayout
        {
            get
            {
                return staffLayout;
            }

            set
            {
                staffLayout = value;
            }
        }

        public List<List<Tuple<string, string>>> PartSysemsInPages
        {
            get
            {
                return partSysemsInPages;
            }

            set
            {
                partSysemsInPages = value;
            }
        }

        public int NumberOfStaves
        {
            get
            {
                return numberOfStaves;
            }

            set
            {
                numberOfStaves = value;
            }
        }

        public List<SystemLayoutMusicXML> SystemLayout
        {
            get
            {
                return systemLayout;
            }

            set
            {
                systemLayout = value;
            }
        }

        public double StavesDistance
        {
            get
            {
                return stavesDistance;
            }

            set
            {
                stavesDistance = value;
            }
        }
        /// <summary>
        /// Part height in Tenths
        /// </summary>
        public double PartHeight
        {
            get
            {
                return partHeight;
            }

            set
            {
                partHeight = value;
            }
        }

        public List<List<List<string>>> MeasuresPerSystemPerPage
        {
            get
            {
                return measuresPerSystemPerPage;
            }

            set
            {
                measuresPerSystemPerPage = value;
            }
        }

        public List<List<StaffLayoutMusicXML>> StaffLayoutPerPage
        {
            get
            {
                return staffLayoutPerPage;
            }

            set
            {
                staffLayoutPerPage = value;
            }
        }

        public List<List<SystemLayoutMusicXML>> SystemLayoutPerPage
        {
            get
            {
                return systemLayoutPerPage;
            }

            set
            {
                systemLayoutPerPage = value;
            }
        }

        public PartProperties(MusicXMLViewerWPF.ScorePartwiseMusicXML score, string partId)
        {
            SetDefaultDistances(score);

            partIndex = score.Part.FindIndex(i => i.Id == partId);
            partSysemsInPages = score.Part.ElementAt(partIndex).TryGetLinesPerPage();
            var part = score.Part.ElementAt(partIndex).Measure;
            var staves = part.ElementAt(0)?.Items?.OfType<AttributesMusicXML>()?.FirstOrDefault()?.Staves ?? "1";
            if (staves != null)
            {
                numberOfStaves = int.Parse(staves);
            }
            if (partIndex != null)
            {
                int currPageListIndex = 0;
                int previousPageListIndex = 0;
                foreach (var item in partSysemsInPages)
                {
                    int currentSystemIndex = 0;
                    List<SystemLayoutMusicXML> tempSysLayouts = new List<SystemLayoutMusicXML>();
                    foreach (var measureNumber in item)
                    {
                        var measure = part.First(i => i.Number == measureNumber.Item1);
                        var printLayouts = measure.Items.OfType<PrintMusicXML>().FirstOrDefault();
                        if (printLayouts != null)
                        {
                            if (printLayouts.SystemLayout != null)
                            {
                                UpdateListWithObjectsOfType(printLayouts.SystemLayout, ref systemLayout, 0);
                            }
                            if (printLayouts.MeasureNumbering != null)
                            {
                                UpdateListWithObjectsOfType(printLayouts.MeasureNumbering, ref measureNumbering, 0);
                            }
                            if (printLayouts.StaffLayout != null)
                            {
                                if (printLayouts.StaffLayout.Count != 0)
                                {
                                    UpdateListWithObjectsOfType(printLayouts.StaffLayout.ElementAtOrDefault(0), ref staffLayout, 0);
                                }
                            }
                        }
                        if (measureNumbering.Count == 0)
                        {
                            measureNumbering.Add(new MeasureNumberingMusicXML() { Value = MeasureNumberingValueMusicXML.system });
                        }
                        if (measureNumbering.Count < currentSystemIndex + 1)
                        {
                            measureNumbering.Add(measureNumbering.LastOrDefault());
                        }
                        if (systemLayout.Count == 0)
                        {
                            systemLayout.Add(new SystemLayoutMusicXML() { SystemDistance = defaultSystemDistance, TopSystemDistance = defaultTopSystemDistance });
                        }
                        if (systemLayout.Count < currentSystemIndex + 1)
                        {
                            systemLayout.Add(new SystemLayoutMusicXML() { SystemDistance = defaultSystemDistance, SystemMargins = systemLayout.LastOrDefault().SystemMargins });
                        }
                        if (staffLayout.Count == 0)
                        {
                            staffLayout.Add(new StaffLayoutMusicXML() { Number = staves, StaffDistance = defaultStaffDistance });
                        }
                        if (staffLayout.Count < currentSystemIndex + 1)
                        {
                            staffLayout.Add(staffLayout.LastOrDefault());
                        }
                        currentSystemIndex++;
                    }
                    currPageListIndex = systemLayout.Count - previousPageListIndex;
                    tempSysLayouts = systemLayout.GetRange(previousPageListIndex, currPageListIndex);
                    systemLayoutPerPage.Add(new List<SystemLayoutMusicXML>(tempSysLayouts));
                    staffLayoutPerPage.Add(staffLayout.GetRange(previousPageListIndex, currPageListIndex));
                    previousPageListIndex = currPageListIndex;
                }
            }
            stavesDistance = staffLayout.ElementAt(0).StaffDistance;
            SetSystemMeasureRanges(score);
            SetPartHeight();
        }

        private void SetPartHeight()
        {
            double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.StaffHeight.MMToTenths();
            partHeight = staffHeight * numberOfStaves + (stavesDistance * (numberOfStaves - 1));
        }

        private void SetSystemMeasureRanges(MusicXMLViewerWPF.ScorePartwiseMusicXML score)
        {
            LayoutControl.LayoutGeneral currentLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentTabLayout;
            double defaultLeftMargin = currentLayout.PageMargins.LeftMargin;
            double defaultTopMargin = currentLayout.PageMargins.TopMargin;
            var part = score.Part.ElementAt(partIndex);
            measuresPerSystem = new List<List<string>>();
            foreach (var page in partSysemsInPages)
            {
                List<List<string>> tempList = new List<List<string>>();
                foreach (var item2 in page)
                {
                    measuresPerSystem.Add(part.TryGetMeasuresIdRange(item2));
                    tempList.Add(part.TryGetMeasuresIdRange(item2));
                }
                measuresPerSystemPerPage.Add(new List<List<string>>(tempList));
            }
            var measures = score.Part.ElementAt(partIndex).Measure;
            score.SetLargestWidth();
            double previousWidth = 0.0;
            double currentLineY = 0.0;
            coords = new Dictionary<string, Point>();
            foreach (var measuresLine in measuresPerSystem)
            {
                int systemIndex = measuresPerSystem.IndexOf(measuresLine);
                if (systemIndex == 0)
                {
                    double topMargin = 0.0;
                    double marginL = defaultLeftMargin + systemLayout.ElementAt(systemIndex).SystemMargins.LeftMargin;
                    if (partIndex == 0)
                    {
                        topMargin = defaultTopMargin + systemLayout.ElementAt(systemIndex).TopSystemDistance;
                        // if part is first use topsystemdistance else staffdistance
                    }
                    else
                    {
                        topMargin = defaultTopMargin + staffLayout.ElementAt(systemIndex).StaffDistance;
                    }
                    previousWidth = 0.0; //! test marginL.TenthsToWPFUnit();
                    currentLineY = topMargin.TenthsToWPFUnit();
                    foreach (var measureId in measuresLine)
                    {
                        coords.Add(measureId, new Point(previousWidth, currentLineY));
                        previousWidth += part.GetMeasureUsingId(measureId).Width.TenthsToWPFUnit();
                    }
                }
                else
                {
                    if (partIndex == 0)
                    {
                        currentLineY += systemLayout.ElementAt(systemIndex).SystemDistance.TenthsToWPFUnit() + currentLayout.PageProperties.StaffHeight.MMToWPFUnit();
                    }
                    else
                    {
                        currentLineY += staffLayout.ElementAt(systemIndex).StaffDistance.TenthsToWPFUnit() + currentLayout.PageProperties.StaffHeight.MMToWPFUnit();
                    }
                    previousWidth = 0.0; //! test (defaultLeftMargin + systemLayout.ElementAt(systemIndex).SystemMargins.LeftMargin).TenthsToWPFUnit();
                    foreach (var measureId in measuresLine)
                    {
                        coords.Add(measureId, new Point(previousWidth, currentLineY));
                        previousWidth += part.GetMeasureUsingId(measureId).Width.TenthsToWPFUnit();
                    }
                }
            }
        }

        private void SetDefaultDistances(MusicXMLViewerWPF.ScorePartwiseMusicXML score)
        {
            var layout = ViewModel.ViewModelLocator.Instance.Main.CurrentTabLayout;
            defaultSystemDistance = 2.5 * layout.PageProperties.StaffHeight.TenthsToWPFUnit();
            defaultTopSystemDistance = 3 * layout.PageProperties.StaffHeight.TenthsToWPFUnit();
            defaultStaffDistance = 1.5 * layout.PageProperties.StaffHeight.TenthsToWPFUnit();
            if (score.Defaults != null)
            {
                if (score.Defaults.StaffLayout != null)
                {
                    if (score.Defaults.StaffLayout.Count != 0)
                    {
                        defaultStaffDistance = score.Defaults.StaffLayout.ElementAt(0).StaffDistance;
                    }
                }
                if (score.Defaults.SystemLayout != null)
                {
                    if (score.Defaults.SystemLayout.SystemDistanceSpecified)
                    {
                        defaultSystemDistance = score.Defaults.SystemLayout.SystemDistance;
                    }
                    if (score.Defaults.SystemLayout.TopSystemDistanceSpecified)
                    {
                        defaultTopSystemDistance = score.Defaults.SystemLayout.TopSystemDistance;
                    }
                }
            }
        }

        /// <summary>
        /// Compare and update differences
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newerLayout"></param>
        /// <param name="listToUpdate"></param>
        /// <param name="currentLayoutIndex"></param>
        private void UpdateListWithObjectsOfType<T>(T newerLayout, ref List<T> listToUpdate, int currentLayoutIndex)
        {
            T currentLayout = Activator.CreateInstance<T>();
            T oldSystemLayout = listToUpdate.LastOrDefault();
            if (listToUpdate.Count == 0)
            {
                listToUpdate.Add(newerLayout);
                return;
            }
            if (ReferenceEquals(oldSystemLayout, newerLayout))
            {
                T copy = currentLayout;
                if (copy != null)
                {
                    XmlSerializer xmls = new XmlSerializer(copy.GetType());
                    System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                    xmls.Serialize(memStream, copy);

                    memStream.Position = 0;
                    xmls = new XmlSerializer(copy.GetType());
                    copy = ((T)xmls.Deserialize(memStream));
                }
                listToUpdate.Add(copy);
                return;
            }
            else
            {
                foreach (var property in oldSystemLayout.GetType().GetProperties())
                {
                    var currentObjValue = property.GetValue(oldSystemLayout);
                    var newObjValue = property?.GetValue(newerLayout) ?? currentObjValue;
                    if (currentObjValue?.Equals(newObjValue) ?? true)
                    {
                        property.SetValue(currentLayout, currentObjValue);
                    }
                    else
                    {
                        property.SetValue(currentLayout, newObjValue);
                    }
                }
                listToUpdate.Add(currentLayout);
            }
        }

    }
}
