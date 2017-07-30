﻿
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLViewerWPF;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Contains PartSegmentViewModels as collection
    /// </summary>
    class PageViewModel : ViewModelBase 
    {
        #region Private Fiels
        private ObservableCollection<UIElement> pageCanvas = new ObservableCollection<UIElement>();
        private DrawingVisualHost page = new DrawingVisualHost();
        private double pageHeight;
        private double pageWidth;
        #endregion
        #region NewConcept
        PageDrawingSystem newPage;
        int pageIndex;
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
        public PageViewModel(int pageIndex):this()
        {
            this.pageIndex = pageIndex;
        }
        //! test
        public PageViewModel(Canvas page)
        {
            Point dimensions = ViewModelLocator.Instance.Main.CurrentPageLayout.PageDimensions.GetPageDimensionsInPx();
            PageWidth = dimensions.X;
            PageHeight = dimensions.Y;
            page.Background = Brushes.WhiteSmoke;
            PageCanvas.Add(page);
        }
        public PageViewModel(ScorePartwiseMusicXML scorePartwise, int index)
        {
            pageIndex = index;
            newPage = new PageDrawingSystem(scorePartwise, pageIndex);
            Point dimensions = ViewModelLocator.Instance.Main.CurrentPageLayout.PageDimensions.GetPageDimensionsInPx();
            PageWidth = dimensions.X;
            PageHeight = dimensions.Y;
            PageCanvas.Add(newPage.PageCanvas);
        }
        #endregion

        #region Properties
        public ObservableCollection<UIElement> PageCanvas { get { return pageCanvas; } set { pageCanvas = value; } }
        public DrawingVisualHost Page { get { return page; } set { Set(nameof(Page), ref page, value); } }
        public RelayCommand TestCommand { get; set; }
        public double PageHeight { get { return pageHeight; } set { Set(nameof(PageHeight), ref pageHeight, value); } }
        public double PageWidth { get { return pageWidth; } set { Set(nameof(PageWidth), ref pageWidth, value); } }
        #endregion

        #region Methods
        private void AddPartSegment()
        {
            PageCanvas.Add(page);
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
