
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using MusicXMLScore.DrawingHelpers;
using MusicXMLScore.Helpers;
using MusicXMLViewerWPF;
using System.ComponentModel;
using MusicXMLScore.Prototypes;
using System.Collections.Generic;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Contains PartSegmentViewModels as collection
    /// </summary>
    class PageViewModel : ViewModelBase
    {
        PageDrawingSystem newPage;
        private DrawingVisualHost page = new DrawingVisualHost();
        private ObservableCollection<UIElement> pageCanvas = new ObservableCollection<UIElement>();
        private double pageHeight;
        int pageIndex;
        private double pageWidth;
        Random random = new Random();
        private ScoreLayout.AbstractScorePage scorePage;

        public PageViewModel()
        {
            TestCommand = new RelayCommand(OnTestCommand);

            //! here default page which contains one ParSegmentView for simple test
            Point dimensions = ViewModelLocator.Instance.Main.CurrentPageLayout.PageDimensions.GetPageDimensionsInPx();
            PageWidth = dimensions.X;
            PageHeight = dimensions.Y;
            AddPartSegment();
        }

        public PageViewModel(int pageIndex) : this()
        {
            this.pageIndex = pageIndex;
        }

        public PageViewModel(Canvas page)
        {
            Point dimensions = ViewModelLocator.Instance.Main.CurrentPageLayout.PageDimensions.GetPageDimensionsInPx();
            PageWidth = dimensions.X;
            PageHeight = dimensions.Y;
            page.Background = Brushes.WhiteSmoke;
            PageCanvas.Add(page);
        }

        public PageViewModel(ScoreLayout.AbstractScorePage page)
        {
            AdvancedPageViewContent = new AdvancedPageViewModel("asd");
            TestCommand = new RelayCommand(OnTestCommand);
            PageWidth = page.Width;
            PageHeight = page.Height;
            scorePage = page;
            var sPage = page as ScoreLayout.StandardScorePage;
            if (sPage != null)
            {
                sPage.AddListener(ScorePagePropertyChanged);
            }
            var canvas = page.GetContent() as Canvas;
            if (canvas != null)
            {
                canvas.Background = Brushes.WhiteSmoke;
            }
            PageCanvas.Add(canvas);
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

        public AdvancedPageViewModel AdvancedPageViewContent { get; set; }
        public ContextMenu ContextMenu { get; set; }
        public DrawingVisualHost Page { get { return page; } set { Set(nameof(Page), ref page, value); } }
        public ObservableCollection<UIElement> PageCanvas { get { return pageCanvas; } set { pageCanvas = value; } }
        public double PageHeight { get { return pageHeight; } set { Set(nameof(PageHeight), ref pageHeight, value); } }
        public string PageNumber { get; set; } = "0";
        public TextAlignment PageNumberAlignment { get; set; } = TextAlignment.Left;
        public double PageWidth { get { return pageWidth; } set { Set(nameof(PageWidth), ref pageWidth, value); } }
        public RelayCommand TestCommand { get; set; }

        public void ScorePagePropertyChanged(object o, PropertyChangedEventArgs a)
        {
            switch (a.PropertyName)
            {
                case nameof(scorePage.Width):
                    PageWidth = scorePage.Width;
                    break;
                case nameof(scorePage.Height):
                    PageHeight = scorePage.Height;
                    break;
                default:
                    break;
            }
        }

        private void AddPartSegment()
        {
            PageCanvas.Add(page);
        }

        private void OnTestCommand()
        {
            if (PageHeight > 300)
            {
                if (scorePage != null)
                {
                    scorePage.Height = scorePage.Height - 30;
                    PageHeight = scorePage.Height;
                    scorePage.Width = scorePage.Width - 30;
                    PageWidth = scorePage.Width;
                }
            }
        }
    }
}
