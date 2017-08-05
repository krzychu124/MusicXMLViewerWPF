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
        private int _pageIndex; //! index of page
        private double _pageContentWidth; //! width available for content (page width - margins)
        private double _pageContentHeight; //! height available for content (page height - margins)
        //private bool keepEqualMeasureCount = true; //!todo implementation
        private int _lastSystemIndex; //! index of last added system 

        private double _defaultSystemDistance;
        private double _defaultTopSystemDistance;
        private Dictionary<int, double> _systemDistances;
        private Dictionary<int, double> _systemHeights;
        private List<LayoutSystemInfo> _systemDimensionsInfo;
        private double _availableHeight;
        private List<double> _systemsYPositions;
        private List<double> _systemsXPositions;
        private LayoutStyle.PageLayoutStyle _pageLayoutStyle;

        /// <summary>
        /// System position on page
        /// </summary>
        /// <param name="index">Index of system</param>
        /// <returns></returns>
        public Point SystemPosition(int index) => new Point(_systemsXPositions[index], _systemsYPositions[index]);
        public double SystemVerticalPosition(uint index) => index < _systemsYPositions.Count ? _systemsYPositions[(int)index] : 0.0;
        public double SystemHorizontalPosition(uint index) => index < _systemsXPositions.Count ? _systemsXPositions[(int)index] : 0.0;

        /// <summary>
        /// Number of systems generated inside page
        /// </summary>
        public int SystemsCount => _systemDimensionsInfo?.Count ?? 0;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Page width without margins (L+R)
        /// </summary>
        public double PageContentWidth
        {
            get
            {
                return _pageContentWidth;
            }

            set
            {
                _pageContentWidth = value;
            }
        }

        /// <summary>
        /// Systems layout information collection
        /// </summary>
        internal List<LayoutSystemInfo> SystemDimensionsInfo
        {
            get
            {
                return _systemDimensionsInfo;
            }

            set
            {
                if (_systemDimensionsInfo != value)
                {
                    _systemDimensionsInfo = value;
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
                return _pageIndex;
            }

            set
            {
                _pageIndex = value;
            }
        }

        public LayoutPageContentInfo(int pageIndex)
        {
            _pageIndex = pageIndex;
            SetDefaultDistances();
            GetPageContentHeight();
            GetPageContentWidth();
            _availableHeight = _pageContentHeight; //! CalculateAvailableHeight();
            _pageLayoutStyle = ViewModel.ViewModelLocator.Instance.Main.CurrentLayout.LayoutStyle.PageStyle;
            _pageLayoutStyle.PropertyChanged += LayoutPageContentInfo_PropertyChangedHandler;
            PropertyChanged += LayoutPageContentInfo_PropertyChangedHandler;
        }

        private void LayoutPageContentInfo_PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LayoutStyle.PageLayoutStyle.StretchSystemToPageWidth):
                    if (sender is LayoutStyle.PageLayoutStyle)
                    {
                        ArrangeSystems();
                    }
                    break;
                case nameof(LayoutStyle.PageLayoutStyle.StretchLastSystemOnPage):
                    if (sender is LayoutStyle.PageLayoutStyle)
                    {
                        ArrangeSystems();
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
            return _pageLayoutStyle.StretchSystemToPageWidth;
        }

        /// <summary>
        /// Returns true if last system on page should be stretched to fill page content width
        /// </summary>
        /// <returns></returns>
        private bool LastSystemStretched()
        {
            return _pageLayoutStyle.StretchLastSystemOnPage;
        }

        /// <summary>
        /// Adds Complete Collection of LayoutSystemInfo to LayoutPageContent
        /// </summary>
        /// <param name="systemInfo"></param>
        public void AddSystemDimensionsInfo(List<LayoutSystemInfo> systemInfo)
        {
            _systemDimensionsInfo = systemInfo;
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
            if (_systemDimensionsInfo == null)
            {
                _systemDimensionsInfo = new List<LayoutSystemInfo>();
            }
            if (_availableHeight < systemInfo.SystemHeight)
            {
                return false;
            }
            //!todo more tests 
            _systemDimensionsInfo.Add(systemInfo);
            _lastSystemIndex = _systemDimensionsInfo.Count - 1;
            CalculateAvailableHeight();
            return true;
        }

        public void ArrangeSystems()
        {
            int index = 0;
            foreach (var system in _systemDimensionsInfo)
            {
                if (index == _lastSystemIndex && !LastSystemStretched()) //! is last system in page
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
            _systemsXPositions = new List<double>();
            _systemsYPositions = new List<double>();

            for (int i = 0; i < _systemDimensionsInfo.Count; i++)
            {
                if (i == 0)
                {
                    currentY = _systemDistances[i];
                }
                _systemsXPositions.Add(currentX);
                _systemsYPositions.Add(currentY);
                currentY += _systemDistances[i] + _systemHeights[i];
                _systemDimensionsInfo[i].UpdateLayout = true;
            }
        }

        /// <summary>
        /// Recalculates height of unused space on page 
        /// </summary>
        private void CalculateAvailableHeight()
        {
            GenerateSystemHeights();
            GenerateSystemDistances();
            _availableHeight = _pageContentHeight;
            for (int i = 0; i < _systemDistances.Count; i++)
            {
                _availableHeight -= _systemDistances[i];
                _availableHeight -= _systemHeights[i];
                if (_availableHeight < 0)
                {
                    _availableHeight = 0;
                    Log.LoggIt.Log("Available height is lower than all systems heights");
                }
            }
        }

        /// <summary>
        /// Sets page content width
        /// </summary>
        private void GetPageContentWidth()
        {
            _pageContentWidth = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.GetContentWidth();
        }

        /// <summary>
        /// Sets page content height
        /// </summary>
        private void GetPageContentHeight()
        {
            _pageContentHeight = ViewModel.ViewModelLocator.Instance.Main.CurrentPageLayout.GetContentHeight();
        }

        /// <summary>
        /// Generates heights of all systems added to page
        /// </summary>
        private void GenerateSystemHeights()
        {
            if (_systemHeights == null)
            {
                _systemHeights = new Dictionary<int, double>();
            }
            if (_systemHeights.Count != 0) //? maight be refactored to skip heights instead of creating collection from scratch
            {
                _systemHeights.Clear();
            }

            foreach (var system in _systemDimensionsInfo)
            {
                int index = _systemDimensionsInfo.IndexOf(system);
                _systemHeights.Add(index, system.SystemHeight);
            }
        }

        /// <summary>
        /// Generates distance between systems
        /// </summary>
        private void GenerateSystemDistances()// todo add option to get distances from loaded score (if some available)
        {
            if (_systemDistances == null)
            {
                _systemDistances = new Dictionary<int, double>();
            }
            if (_systemDistances.Count != 0)//? maight be refactored to skip heights instead of creating collection from scratch
            {
                _systemDistances.Clear();
            }

            foreach (var system in _systemDimensionsInfo)
            {
                int index = _systemDimensionsInfo.IndexOf(system);
                if (index == 0)
                {
                    _systemDistances.Add(index, _defaultTopSystemDistance);
                }
                else
                {
                    _systemDistances.Add(index, _defaultSystemDistance);
                }
            }
        }

        /// <summary>
        /// Calculates and sets user default distances between systems on page
        /// </summary>
        private void SetDefaultDistances()
        {
            var layout = ViewModel. ViewModelLocator.Instance.Main.CurrentLayout;
            _defaultSystemDistance = 3 * layout.PageProperties.StaffHeight.MMToTenths();
            _defaultTopSystemDistance = 3.5 * layout.PageProperties.StaffHeight.MMToTenths();
        }
    }
}
