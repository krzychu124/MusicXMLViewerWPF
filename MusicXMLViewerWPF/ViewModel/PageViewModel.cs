﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.Helpers;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Reflection;
using MusicXMLViewerWPF;
using GalaSoft.MvvmLight;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Contains PartSegmentViewModels as collection
    /// </summary>
    class PageViewModel : ViewModelBase 
    {
        #region Private Fiels
        private ObservableCollection<UIElement> partsSegments = new ObservableCollection<UIElement>();
        private DrawingVisualHost page = new DrawingVisualHost();
        private double pageHeight = 0.0;
        private double pageWidth = 0.0;
        #endregion
        #region NewConcept
        DrawingHelpers.PageDrawingSystem newPage;
        int pageIndex = 0;
        #endregion

        #region Contructors
        public PageViewModel()
        {
            TestCommand = new RelayCommand(OnTestCommand);
            
            //! here default page which contains one ParSegmentView for simple test
            Point dimensions = ViewModelLocator.Instance.Main.CurrentPageLayout.PageDimensions.GetPageDimensionsInPx();
            PageWidth = dimensions.X;
            PageHeight = dimensions.Y;
            AddPartSegment();
        }

        public PageViewModel(ScorePartwiseMusicXML scorePartwise, int index)
        {
            pageIndex = index;
            newPage = new DrawingHelpers.PageDrawingSystem(scorePartwise, pageIndex);
            Point dimensions = ViewModelLocator.Instance.Main.CurrentPageLayout.PageDimensions.GetPageDimensionsInPx();
            PageWidth = dimensions.X;
            PageHeight = dimensions.Y;
            PartsSegments.Add(newPage.PageCanvas);
        }
        #endregion

        #region Properties
        public ObservableCollection<UIElement> PartsSegments { get { return partsSegments; } set { partsSegments = value; } }
        public DrawingVisualHost Page { get { return page; } set { Set(nameof(Page), ref page, value); } }
        public RelayCommand TestCommand { get; set; }
        public double PageHeight { get { return pageHeight; } set { Set(nameof(PageHeight), ref pageHeight, value); } }
        public double PageWidth { get { return pageWidth; } set { Set(nameof(PageWidth), ref pageWidth, value); } }
        #endregion

        #region Methods
        private void AddPartSegment()
        {
            PartsSegments.Add(page);
        }
        
        #endregion

        #region Commands, Action<>, Func<>
        private void OnTestCommand()
        {
            Console.WriteLine("test command invoked");
        }
        #endregion
    }
}
