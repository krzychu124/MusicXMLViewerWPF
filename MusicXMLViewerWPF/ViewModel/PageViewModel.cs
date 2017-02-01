
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
        #endregion

        #region Contructors
        public PageViewModel() // todo_l get view dimensions !!!
        {
            TestCommand = new RelayCommand(OnTestCommand);

            //TOdo collection of PartSegmentViews to represent as Page
            //! here default page which contains one ParSegmentView for simple test
            AddPartSegment();
            AddPartSegment();
        }
        public PageViewModel(List<Part> partList)
        {
            TestCommand = new RelayCommand(OnTestCommand);
            PartList = partList; //TODO_I test, improve, continue
            CreatePartSegment();
            FillPartSegment();
        }
        #endregion

        #region Properties
        public List<Part> PartList { get { return partList; } set { partList = value; } }
        public ObservableCollection<UIElement> PartsSegments { get { return partsSegments; } set { partsSegments = value; } }
        public RelayCommand TestCommand { get; set; }
        #endregion

        #region Methods
        private void AddPartSegment()
        {
            View.PartsSegmentView psv = new View.PartsSegmentView();
            psv.SetValue(CustomPartsSegmentPanel.TopMarginProperty, 20.0);
            PartsSegments.Add(psv);
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
            var part = PartList.ElementAt(0);
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
            foreach(var indexOfFirstInLine in firstItemInLineIndexes)
            {
                partList = new List<Part>();
                Part tempPart = new Part(part.PartId);
                for (int i=index ; i < indexOfFirstInLine; i++)
                {
                    tempPart.AddMeasure(part.MeasureList.ElementAt(i));
                }
                PartList.Add(tempPart);
                partSegmentsList.Add(PartList);
                index = indexOfFirstInLine;
            }
        }
        private void FillPartSegment()
        {
            foreach (var partSegment in partSegmentsList)
            {
                AddPartSegment(partSegment);
            }
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
