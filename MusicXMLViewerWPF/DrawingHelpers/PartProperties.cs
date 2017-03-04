using MusicXMLScore.Converters;
using MusicXMLScore.Model.Defaults;
using MusicXMLScore.Model.MeasureItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.DrawingHelpers
{
    class PartProperties
    {
        List<SystemLayoutMusicXML> systemLayout = new List<SystemLayoutMusicXML>();
        double defaultSystemDistance = 0.0;
        double defaultTopSystemDistance = 0.0;
        double defaultStaffDistance = 0.0;
        List<StaffLayoutMusicXML> staffLayout = new List<StaffLayoutMusicXML>();
        List<MeasureNumberingMusicXML> measureNumbering = new List<MeasureNumberingMusicXML>();
        int numberOfStaves = 1;
        List<List<string>> partSysemsInPages;
        int partIndex = 0;
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
                foreach (var item in partSysemsInPages)
                {
                    int currentSystemIndex = 0;
                    foreach (var measureNumber in item)
                    {
                        var measure = part.First(i => i.Number == measureNumber);
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
                            systemLayout.Add(systemLayout.LastOrDefault());
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
