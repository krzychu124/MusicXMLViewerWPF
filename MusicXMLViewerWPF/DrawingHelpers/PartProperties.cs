using MusicXMLScore.Converters;
using MusicXMLScore.Model.Defaults;
using MusicXMLScore.Model.MeasureItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace MusicXMLScore.DrawingHelpers
{
    public class PartProperties //TODO_H Clean up unneccessary methods, field and properties
    {
        #region Fields

        private Dictionary<string, Point> coords;
        private double defaultStaffDistance = 0.0;
        private double defaultSystemDistance = 0.0;
        private double defaultTopSystemDistance = 0.0;
        private List<MeasureNumberingMusicXML> measureNumbering = new List<MeasureNumberingMusicXML>();
        private List<List<MeasureNumberingMusicXML>> measureNumberingPerPage = new List<List<MeasureNumberingMusicXML>>();
        private List<List<string>> measuresPerSystem = new List<List<string>>();
        private List<List<List<string>>> measuresPerSystemPerPage = new List<List<List<string>>>();
        private List<Tuple<string, string>> measuresRangeInPartSystem = new List<Tuple<string, string>>();
        private int numberOfStaves = 1;
        private double partHeight = 0;
        private int partIndex = 0;
        private List<List<Tuple<string, string>>> partSysemsInPages;
        private List<StaffLayoutMusicXML> staffLayout = new List<StaffLayoutMusicXML>();
        private List<List<StaffLayoutMusicXML>> staffLayoutPerPage = new List<List<StaffLayoutMusicXML>>();
        private double stavesDistance = 0.0;
        private List<SystemLayoutMusicXML> systemLayout = new List<SystemLayoutMusicXML>();
        private List<List<SystemLayoutMusicXML>> systemLayoutPerPage = new List<List<SystemLayoutMusicXML>>();

        #endregion Fields

        #region Constructors

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
                            else
                            {
                                if (item.IndexOf(measureNumber) == 0)
                                {
                                    systemLayout.Add(new SystemLayoutMusicXML() { SystemDistance = defaultSystemDistance, TopSystemDistance = defaultTopSystemDistance });
                                }
                            }
                            if (printLayouts.MeasureNumbering != null)
                            {
                                UpdateListWithObjectsOfType(printLayouts.MeasureNumbering, ref measureNumbering, 0);
                            }
                            if (printLayouts.StaffLayout.Count != 0)
                            {
                                if (printLayouts.StaffLayout.Count != 0)
                                {
                                    UpdateListWithObjectsOfType(printLayouts.StaffLayout.ElementAtOrDefault(0), ref staffLayout, 0);
                                }
                            }
                            else
                            {
                                if (item.IndexOf(measureNumber) == 0)
                                {
                                    staffLayout.Add(new StaffLayoutMusicXML() { Number = staves, StaffDistance = defaultStaffDistance });
                                }
                            }
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
                        if (measureNumbering.Count == 0)
                        {
                            measureNumbering.Add(new MeasureNumberingMusicXML() { Value = MeasureNumberingValueMusicXML.system });
                        }
                        if (measureNumbering.Count != systemLayout.Count)
                        {
                            measureNumbering.Add(measureNumbering.LastOrDefault());
                        }
                        currentSystemIndex++;
                    }
                    currPageListIndex = systemLayout.Count - previousPageListIndex;
                    systemLayoutPerPage.Add(systemLayout.GetRange(previousPageListIndex, currPageListIndex));
                    staffLayoutPerPage.Add(staffLayout.GetRange(previousPageListIndex, currPageListIndex));
                    previousPageListIndex = currPageListIndex;
                }
            }
            stavesDistance = staffLayout.ElementAt(0).StaffDistance;
            SetSystemMeasureRanges(score);
            SetPartHeight();
        }

        #endregion Constructors

        #region Properties

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

        #endregion Properties

        #region Methods

        private void SetDefaultDistances(MusicXMLViewerWPF.ScorePartwiseMusicXML score)
        {
            var layout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout;
            defaultSystemDistance = 2.5 * layout.PageProperties.StaffHeight.MMToTenths();
            defaultTopSystemDistance = 3 * layout.PageProperties.StaffHeight.MMToTenths();
            defaultStaffDistance = 1.7 * layout.PageProperties.StaffHeight.MMToTenths();
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

        private void SetPartHeight()
        {
            double staffHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.PageProperties.StaffHeight.MMToTenths();
            partHeight = staffHeight * numberOfStaves + (stavesDistance * (numberOfStaves - 1));
        }

        private void SetSystemMeasureRanges(MusicXMLViewerWPF.ScorePartwiseMusicXML score)
        {
            LayoutControl.LayoutGeneral currentLayout = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout;
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
                    //currentLineY = topMargin.TenthsToWPFUnit();
                    foreach (var measureId in measuresLine)
                    {
                        coords.Add(measureId, new Point(previousWidth, currentLineY));
                        previousWidth += part.GetMeasureUsingId(measureId).CalculatedWidth.TenthsToWPFUnit();
                    }
                }
                else
                {
                    if (partIndex == 0)
                    {
                        //currentLineY += systemLayout.ElementAt(systemIndex).SystemDistance.TenthsToWPFUnit() + currentLayout.PageProperties.StaffHeight.MMToWPFUnit();
                    }
                    else
                    {
                        //currentLineY += staffLayout.ElementAt(systemIndex).StaffDistance.TenthsToWPFUnit() + currentLayout.PageProperties.StaffHeight.MMToWPFUnit();
                    }
                    previousWidth = 0.0; //! test (defaultLeftMargin + systemLayout.ElementAt(systemIndex).SystemMargins.LeftMargin).TenthsToWPFUnit();
                    foreach (var measureId in measuresLine)
                    {
                        coords.Add(measureId, new Point(previousWidth, currentLineY));
                        previousWidth += part.GetMeasureUsingId(measureId).CalculatedWidth.TenthsToWPFUnit();
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

        #endregion Methods
    }
}