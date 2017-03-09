
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
using GalaSoft.MvvmLight.Messaging;
using MusicXMLViewerWPF.ScoreParts.MeasureContent;
using MusicXMLScore.Model;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Contains PartSegmentViewModels as collection
    /// </summary>
    class PageViewModel : ViewModelBase //TODO_I collection<PartsSegmentViewModel>
    {
        #region Private Fiels
        private List<Part> partList;
        private List<List<Part>> partSegmentsList = new List<List<Part>>();
        private ObservableCollection<UIElement> partsSegments = new ObservableCollection<UIElement>();
        private CanvasList page = new CanvasList();
        private double pageHeight = 0.0;
        private double pageWidth = 0.0;
        #endregion
        #region NewConcept
        DrawingHelpers.PageDrawingSystem newPage;
        int pageIndex = 0;
        #endregion

        #region Contructors
        public PageViewModel() // todo_l get view dimensions !!!
        {
            TestCommand = new RelayCommand(OnTestCommand);

            //TOdo collection of PartSegmentViews to represent as Page
            //! here default page which contains one ParSegmentView for simple test
            Point dimensions = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.PageDimensions.GetPageDimensionsInPx();
            Page.Width = dimensions.X;
            Page.Height = dimensions.Y;
            PageWidth = dimensions.X;
            PageHeight = dimensions.Y;
            AddPartSegment();
        }
        public PageViewModel(List<Part> partList)
        {
            TestCommand = new RelayCommand(OnTestCommand);
            PartList = partList; //TODO_I test, improve, continue
            CreatePartSegment();
            FillPartSegment();
        }

        public PageViewModel(ScorePartwiseMusicXML scorePartwise, int index)
        {
            pageIndex = index;
            PrimitivePageGenerator p = new PrimitivePageGenerator(scorePartwise);
            newPage = new DrawingHelpers.PageDrawingSystem(scorePartwise, pageIndex);
            Point dimensions = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.PageDimensions.GetPageDimensionsInPx();
            PageWidth = dimensions.X;
            PageHeight = dimensions.Y;
            //Page = p.Page;
            //partsSegments.Add(page);
            PartsSegments.Add(p.PageHost);
            PartsSegments.Add(newPage.PageCanvas);
        }
        #endregion

        #region Properties
        public List<Part> PartList { get { return partList; } set { partList = value; } }
        public ObservableCollection<UIElement> PartsSegments { get { return partsSegments; } set { partsSegments = value; } }
        public CanvasList Page { get { return page; } set { Set(nameof(Page), ref page, value); } }
        public RelayCommand TestCommand { get; set; }
        public double PageHeight { get { return pageHeight; } set { Set(nameof(PageHeight), ref pageHeight, value); } }
        public double PageWidth { get { return pageWidth; } set { Set(nameof(PageWidth), ref pageWidth, value); } }
        #endregion

        #region Methods
        private void AddPartSegment()
        {
            //! part segment canvas...
            //View.PartsSegmentView psv = new View.PartsSegmentView();
            //psv.SetValue(CustomPartsSegmentPanel.TopMarginProperty, 20.0);
            PartsSegments.Add(page);
        }
        private void AddPartSegment(List<Part> segmentPartList)
        {
            View.PartsSegmentView psv = new View.PartsSegmentView() { DataContext = new PartsSegmentViewModel(segmentPartList) };
            psv.SetValue(CustomPartsSegmentPanel.TopMarginProperty, 20.0);
            PartsSegments.Add(psv);
        }
        private void CreatePartSegment()//! Temporary solution for prototype visualization
        {
            if (PartList == null) { return; }
            // var vm = ViewModelLocator.Instance.Main.SelectedTabItem.DataContext as PagesControllerViewModel;
            // Recover from file using new system property of measure in part
            for (int p = 0; p < PartList.Count; p++)
            {
                var part = PartList.ElementAt(p);
                List<Part> partlist = new List<Part>();
                List<int> firstItemInLineIndexes = new List<int>();
                for (int i = 0; i < part.MeasureList.Count; i++)
                {
                    if (part.MeasureList.ElementAt(i).IsFirstInLine)
                    {
                        firstItemInLineIndexes.Add(i);
                    }
                }
                if (firstItemInLineIndexes.LastOrDefault() != part.MeasureList.Count)
                {
                    firstItemInLineIndexes.Add(part.MeasureList.Count);
                }
                int index = 0;
                partlist = new List<Part>();
                foreach (var indexOfFirstInLine in firstItemInLineIndexes)
                {

                    Part tempPart = new Part(part.PartId);
                    for (int i = index; i < indexOfFirstInLine; i++)
                    {
                        tempPart.AddMeasure(part.MeasureList.ElementAt(i));
                    }
                    partlist.Add(tempPart);

                    index = indexOfFirstInLine;
                }
                partSegmentsList.Add(partlist);
            }
        }
        private void FillPartSegment()
        { 
            List<List<Part>> llp = new List<List<Part>>();
            for (int j = 0; j < partSegmentsList.ElementAt(0).Count; j++)
            {
                List<Part> lp = new List<Part>();
                for (int i = 0; i < partSegmentsList.Count; i++)
                {
                    lp.Add(partSegmentsList.ElementAt(i).ElementAt(j));
                }
                llp.Add(lp);
            }
            foreach (var ps in llp)
            {
                AddPartSegment(ps);
            }
            //foreach (var partSegment in partSegmentsList)
            //{
            //    AddPartSegment(partSegment);
            //}
        }
        #endregion

        #region Commands, Action<>, Func<>
        private void OnTestCommand()
        {
            List<Part> lp = new List<Part>();
            MessengerInstance.Send<GenericMessage<List<Part>>>(new GenericMessage<List<Part>>(this, lp), "toNextPage");
            Console.WriteLine("test command invoked");
        }
        #endregion
    }
}
