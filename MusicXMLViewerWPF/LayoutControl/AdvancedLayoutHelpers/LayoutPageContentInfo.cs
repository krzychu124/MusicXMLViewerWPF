using MusicXMLScore.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.LayoutControl
{
    class LayoutPageContentInfo
    {
        //! missing custom system distance setter
        private int pageIndex;
        private double pageContentWidth;
        private double pageContentHeight;
        private bool stretchSystemsToWidth;

        private double defaultStaffDistance;
        private double defaultSystemDistance;
        private double defaultTopSystemDistance;
        Dictionary<int, double> systemDistances;
        Dictionary<int, double> systemHeights;
        List<LayoutSystemInfo> systemDimensionsInfo;
        double availableHeight;
        List<double> systemsYPositions;
        List<double> systemsXPositions;
        public Point SystemPosition(int index) => new Point(systemsXPositions[index], systemsYPositions[index]);
        public double SystemVerticalPosition(int index) =>  index < systemsYPositions.Count ? systemsYPositions[index] : 0.0;
        public double SystemHorizontalPosition(int index) => index < systemsXPositions.Count ? systemsXPositions[index] : 0.0;
        public bool StretchSystemsToWidth
        {
            get
            {
                return stretchSystemsToWidth;
            }

            set
            {
                stretchSystemsToWidth = value;
            }
        }

        public LayoutPageContentInfo(int pageIndex)
        {
            this.pageIndex = pageIndex;
            SetDefaultDistances();
            GetPageContentHeight();
            GetPageContentWidth();
            availableHeight = pageContentHeight; //! CalculateAvailableHeight();
        }
        public LayoutPageContentInfo(int pageIndex, double staffDistance, double systemDistance, double topSystemDistance)
        {
            this.pageIndex = pageIndex;
            defaultStaffDistance = staffDistance;
            defaultSystemDistance = systemDistance;
            defaultTopSystemDistance = topSystemDistance;
            GetPageContentHeight();
            GetPageContentWidth();
            availableHeight = pageContentHeight; //! CalculateAvailableHeight();
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
                //! need tests 
                //? availableHeight -= systemInfo.SystemHeight + defaultSystemDistance.TenthsToWPFUnit();
                systemDimensionsInfo.Add(systemInfo);
                CalculateAvailableHeight();
                return true;
            }
        }

        private void ArrangeSystems()
        {
            double currentX = 0.0;
            double currentY = 0.0;
            if (systemsXPositions == null || systemsYPositions == null)
            {
                systemsXPositions = new List<double>();
                systemsYPositions = new List<double>();
            }
            for (int i = 0; i < systemDimensionsInfo.Count; i++)
            {
                systemsXPositions.Add(currentX);
                systemsYPositions.Add(currentY);
                currentY += systemDistances[i] + systemHeights[i];
                currentX = systemDimensionsInfo[i].SystemWidth;
                if (stretchSystemsToWidth)
                {
                    if (currentX < pageContentWidth)
                    {
                        //? systemDimensionsInfo[i].UpdateSystemWidth(pageContentWidth);
                    }
                }
            }
        }

        private void CalculateAvailableHeight()
        {
            GenerateSystemHeights();
            GenerateSystemDistances();
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
            defaultStaffDistance = 1.7 * layout.PageProperties.StaffHeight.MMToTenths();
        }
    }
}
