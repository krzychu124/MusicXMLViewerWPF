
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
    class PageViewModel : ObservableObject //TODO_I collection<PartsSegmentViewModel>
    {
        #region Private Fiels
        private ObservableCollection<UIElement> partsSegments = new ObservableCollection<UIElement>();
        private List<Part> partList;
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
            this.partList = partList; //TODO_I test, improve, continue
            //TODO_I Generate_PartSegments();
        }
        #endregion
        
        #region Properties
        public RelayCommand TestCommand { get; set; }
        public ObservableCollection<UIElement> PartsSegments { get { return partsSegments; }  set { partsSegments = value; } }

        public List<Part> PartList
        {
            get
            {
                return partList;
            }

            set
            {
                partList = value;
            }
        }
        #endregion

        #region Methods
        private void AddPartSegment()
        {
            View.PartsSegmentView psv = new View.PartsSegmentView();
            psv.SetValue(CustomPartsSegmentPanel.TopMarginProperty, 20.0);
            PartsSegments.Add(psv);
        }
        #endregion

        #region Commands, Action<>, Func<>
        private void OnTestCommand()
        {
        }
        #endregion
    }
}
