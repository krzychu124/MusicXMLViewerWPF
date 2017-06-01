using MusicXMLScore.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.LayoutControl
{
    class LayoutPageContentInfo : INotifyPropertyChanged
    {
        //! missing custom system distance setter
        private int pageIndex;
        private double pageContentWidth;
        private double pageContentHeight;
        //private bool keepEqualMeasureCount = true; //!todo implementation
        private int lastSystemIndex;
        
        private double defaultSystemDistance;
        private double defaultTopSystemDistance;
        Dictionary<int, double> systemDistances;
        Dictionary<int, double> systemHeights;
        List<LayoutSystemInfo> systemDimensionsInfo;
        double availableHeight;
        List<double> systemsYPositions;
        List<double> systemsXPositions;
        private LayoutStyle.PageLayoutStyle pageLayoutStyle;
        public Point SystemPosition(int index) => new Point(systemsXPositions[index], systemsYPositions[index]);
        public double SystemVerticalPosition(int index) => index < systemsYPositions.Count ? systemsYPositions[index] : 0.0;
        public double SystemHorizontalPosition(int index) => index < systemsXPositions.Count ? systemsXPositions[index] : 0.0;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public double PageContentWidth
        {
            get
            {
                return pageContentWidth;
            }

            set
            {
                pageContentWidth = value;
            }
        }

        internal List<LayoutSystemInfo> SystemDimensionsInfo
        {
            get
            {
                return systemDimensionsInfo;
            }

            set
            {
                if (systemDimensionsInfo != value)
                {
                    systemDimensionsInfo = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(SystemDimensionsInfo)));
                }
            }
        }
        public LayoutPageContentInfo(int pageIndex)
        {
            this.pageIndex = pageIndex;
            SetDefaultDistances();
            GetPageContentHeight();
            GetPageContentWidth();
            availableHeight = pageContentHeight; //! CalculateAvailableHeight();
            pageLayoutStyle = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.PageStyle;
            pageLayoutStyle.PropertyChanged += LayoutPageContentInfo_PropertyChangedHandler;
            PropertyChanged += LayoutPageContentInfo_PropertyChangedHandler;
        }

        private void LayoutPageContentInfo_PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LayoutStyle.PageLayoutStyle.StretchSystemToPageWidth):
                    if (sender is LayoutStyle.PageLayoutStyle)
                    {
                        this.ArrangeSystems();
                    }
                    break;
                case nameof(LayoutStyle.PageLayoutStyle.StretchLastSystemOnPage):
                    if (sender is LayoutStyle.PageLayoutStyle)
                    {
                        this.ArrangeSystems();
                    }
                    break;
                default:
                    Log.LoggIt.Log($"No action for changed property {e.PropertyName}", Log.LogType.Exception);
                    break;
            }
        }
        
        private bool SystemStretchedToWidth()
        {
            return pageLayoutStyle.StretchSystemToPageWidth;
        }

        private bool LastSystemStretched()
        {
            return pageLayoutStyle.StretchLastSystemOnPage;
        }

        public LayoutPageContentInfo(int pageIndex, double systemDistance, double topSystemDistance)
        {
            this.pageIndex = pageIndex;
            defaultSystemDistance = systemDistance;
            defaultTopSystemDistance = topSystemDistance;
            GetPageContentHeight();
            GetPageContentWidth();
            availableHeight = pageContentHeight; //! CalculateAvailableHeight();
            PropertyChanged += LayoutPageContentInfo_PropertyChangedHandler;
        }
        /// <summary>
        /// Adds Complete Collection of LayoutSystemInfo to LayoutPageContent
        /// </summary>
        /// <param name="systemInfo"></param>
        public void AddSystemDimensionsInfo(List<LayoutSystemInfo> systemInfo)
        {
            systemDimensionsInfo = systemInfo;
            CalculateAvailableHeight();
        }

        /// <summary>
        /// Adds LayoutSystemInfo to this LayoutPageContent
        /// </summary>
        /// <param name="systemInfo">LayoutSystem instance which represents visual collection of measures arranged into System</param>
        /// <returns>False if LayoutSystem can't fit on page and should be added to new page</returns>
        public bool AddSystemDimensionInfo(LayoutSystemInfo systemInfo)
        {
            systemInfo.ArrangeSystem(SystemStretchedToWidth(), PageContentWidth); //! stretch test
            if (systemDimensionsInfo == null)
            {
                systemDimensionsInfo = new List<LayoutSystemInfo>();
            }
            if (availableHeight < systemInfo.SystemHeight)
            {
                return false;
            }
            else
            {
                //!todo  need tests 
                //? availableHeight -= systemInfo.SystemHeight + defaultSystemDistance.TenthsToWPFUnit();

                systemDimensionsInfo.Add(systemInfo);
                lastSystemIndex = systemDimensionsInfo.Count - 1;
                CalculateAvailableHeight();
                return true;
            }
            
        }

        public void ArrangeSystems()
        {
            int index = 0;
            foreach (var system in systemDimensionsInfo)
            {
                if (index == lastSystemIndex && !LastSystemStretched()) //! is last system in page
                {
                    system.ArrangeSystem(false, PageContentWidth);
                }
                else
                {
                    system.ArrangeSystem(SystemStretchedToWidth(), PageContentWidth);
                }
                index++;
            }
            double currentX = 0.0;
            double currentY = 0.0;
            systemsXPositions = new List<double>();
            systemsYPositions = new List<double>();

            for (int i = 0; i < systemDimensionsInfo.Count; i++)
            {
                if (i == 0)
                {
                    currentY = systemDistances[i];
                }
                systemsXPositions.Add(currentX);
                systemsYPositions.Add(currentY);
                currentY += systemDistances[i] + systemHeights[i];
                //currentX = systemDimensionsInfo[i].SystemWidth;
            }
        }

        private void CalculateAvailableHeight()
        {
            GenerateSystemHeights();
            GenerateSystemDistances();
            availableHeight = pageContentHeight;
            for (int i = 0; i < systemDistances.Count; i++)
            {
                availableHeight -= systemDistances[i];
                availableHeight -= systemHeights[i];
                if (availableHeight < 0)
                {
                    availableHeight = 0;
                    Log.LoggIt.Log("Available height is lower than all systems heights");
                }
            }
        }

        private void GetPageContentWidth()
        {
            pageContentWidth = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.GetContentWidth();
        }

        private void GetPageContentHeight()
        {
            pageContentHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.GetContentHeight();
        }

        private void GenerateSystemHeights()
        {
            if (systemHeights == null)
            {
                systemHeights = new Dictionary<int, double>();
            }
            if (systemHeights.Count != 0) //? maight be refactored to skip heights instead of creating collection from scratch
            {
                systemHeights.Clear();
            }

            foreach (var system in systemDimensionsInfo)
            {
                int index = systemDimensionsInfo.IndexOf(system);
                systemHeights.Add(index, system.SystemHeight);
            }
        }
        private void GenerateSystemDistances()
        {
            if (systemDistances == null)
            {
                systemDistances = new Dictionary<int, double>();
            }
            if (systemDistances.Count != 0)//? maight be refactored to skip heights instead of creating collection from scratch
            {
                systemDistances.Clear();
            }

            foreach (var system in systemDimensionsInfo)
            {
                int index = systemDimensionsInfo.IndexOf(system);
                if (index == 0)
                {
                    systemDistances.Add(index, defaultTopSystemDistance);
                }
                else
                {
                    systemDistances.Add(index, defaultSystemDistance);
                }
            }
        }
        private void SetDefaultDistances()
        {
            var layout = ViewModel. ViewModelLocator.Instance.Main.CurrentLayout;
            defaultSystemDistance = 2.5 * layout.PageProperties.StaffHeight.MMToTenths();
            defaultTopSystemDistance = 3 * layout.PageProperties.StaffHeight.MMToTenths();
        }

        public List<Point> AllSystemsPositions()
        {
            List<Point> resultList = new List<Point>();
            foreach (var index in systemDistances.Keys)
            {
                resultList.Add(this.SystemPosition(index));
            }
            return resultList;
        }
        public Dictionary<string, Point> AllMeasureCoords()
        {
            Dictionary<string, Point> coords = new Dictionary<string, Point>();
            foreach (var item in systemDimensionsInfo)
            {
                foreach (var c in item.Measures)
                {
                    var kvpair = item.MeasureCoords(c.MeasureId);
                    coords.Add(kvpair.Key, kvpair.Value);
                }
            }
                return coords;
        }


    }
}
