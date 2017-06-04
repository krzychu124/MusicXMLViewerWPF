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
        private int pageIndex; //! index of page
        private double pageContentWidth; //! width available for content (page width - margins)
        private double pageContentHeight; //! height available for content (page height - margins)
        //private bool keepEqualMeasureCount = true; //!todo implementation
        private int lastSystemIndex; //! index of last added system 
        
        private double defaultSystemDistance;
        private double defaultTopSystemDistance;
        private Dictionary<int, double> systemDistances;
        private Dictionary<int, double> systemHeights;
        private List<LayoutSystemInfo> systemDimensionsInfo;
        private double availableHeight;
        private List<double> systemsYPositions;
        private List<double> systemsXPositions;
        private LayoutStyle.PageLayoutStyle pageLayoutStyle;

        /// <summary>
        /// System position on page
        /// </summary>
        /// <param name="index">Index of system</param>
        /// <returns></returns>
        public Point SystemPosition(int index) => new Point(systemsXPositions[index], systemsYPositions[index]);
        public double SystemVerticalPosition(uint index) => index < systemsYPositions.Count ? systemsYPositions[(int)index] : 0.0;
        public double SystemHorizontalPosition(uint index) => index < systemsXPositions.Count ? systemsXPositions[(int)index] : 0.0;

        /// <summary>
        /// Number of systems generated inside page
        /// </summary>
        public int SystemsCount => systemDimensionsInfo?.Count ?? 0;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        /// <summary>
        /// Page width without margins (L+R)
        /// </summary>
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

        /// <summary>
        /// Systems layout information collection
        /// </summary>
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

        /// <summary>
        /// Index of current page
        /// </summary>
        public int PageIndex
        {
            get
            {
                return pageIndex;
            }

            set
            {
                pageIndex = value;
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
        
        /// <summary>
        /// Returns true if All Systems should be stretched to fill page content width
        /// </summary>
        /// <returns></returns>
        private bool SystemStretchedToWidth()
        {
            return pageLayoutStyle.StretchSystemToPageWidth;
        }

        /// <summary>
        /// Returns true if last system on page should be stretched to fill page content width
        /// </summary>
        /// <returns></returns>
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
                systemDimensionsInfo[i].UpdateLayout = true;
                //currentX = systemDimensionsInfo[i].SystemWidth;
            }
        }

        /// <summary>
        /// Recalculates height of unused space on page 
        /// </summary>
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

        /// <summary>
        /// Sets page content width
        /// </summary>
        private void GetPageContentWidth()
        {
            pageContentWidth = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.GetContentWidth();
        }

        /// <summary>
        /// Sets page content height
        /// </summary>
        private void GetPageContentHeight()
        {
            pageContentHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.GetContentHeight();
        }

        /// <summary>
        /// Generates heights of all systems added to page
        /// </summary>
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

        /// <summary>
        /// Generates distance between systems
        /// </summary>
        private void GenerateSystemDistances()// todo add option to get distances from loaded score (if some available)
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

        /// <summary>
        /// Calculates and sets user default distances between systems on page
        /// </summary>
        private void SetDefaultDistances()
        {
            var layout = ViewModel. ViewModelLocator.Instance.Main.CurrentLayout;
            defaultSystemDistance = 2.5 * layout.PageProperties.StaffHeight.MMToTenths();
            defaultTopSystemDistance = 3 * layout.PageProperties.StaffHeight.MMToTenths();
        }

        /// <summary>
        /// Generates positions of every system on page
        /// </summary>
        /// <returns>Collection of positions of every system on page</returns>
        public List<Point> AllSystemsPositions()
        {
            List<Point> resultList = new List<Point>();
            foreach (var index in systemDistances.Keys)
            {
                resultList.Add(this.SystemPosition(index));
            }
            return resultList;
        }

        /// <summary>
        /// Generates measures positions on page (id/Number as key)
        /// </summary>
        /// <returns>Positions of all measures on page</returns>
        public Dictionary<string, Point> AllMeasureCoords()
        {
            Dictionary<string, Point> coords = new Dictionary<string, Point>();
            foreach (var item in systemDimensionsInfo)
            {
                foreach (var c in item.Measures)
                {
                    string measureID = c.MeasureId;
                    Point measureCoords = item.FirstPartMeasureCoords(measureID);
                    coords.Add(measureID, measureCoords);
                }
            }
                return coords;
        }


    }
}
