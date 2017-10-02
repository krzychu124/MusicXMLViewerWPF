using GalaSoft.MvvmLight;
using MusicXMLScore.Helpers;
using MusicXMLScore.Prototypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MusicXMLScore.LayoutControl;

namespace MusicXMLScore.ViewModel
{
    class AdvancedPageViewModel : ViewModelBase
    {
        private double pageHeight;
        private double pageWidth;

        private readonly Random random = new Random();


        public AdvancedPageViewModel(int number)
        {
            TestCommand = new RelayCommand(OnTestCommand);
            RefreshPageCommand = new RelayCommand(OnRefreshPageCommand);
            CurrentPanel = new ScoreContentPanel()
            {
                Width = 1200,
                Height = 1000,
                StretchLastRow = true
            };

            PageWidth = CurrentPanel.Width;
            PageHeight = CurrentPanel.Height;
            SetDefaultRows();
        }

        public AdvancedPageViewModel(string test)
        {
            TestCommand = new RelayCommand(OnTestCommand);

            var cMenu = new ContextMenu();
            var menuItem = new MenuItem() { Header = "Add measure", Command = new RelayCommand(OnAddMeasure) };
            cMenu.Items.Add(menuItem);
            ContextMenu = cMenu;
            var childs = Model.Factories.AdvancedLayoutTestFactory.GetLayout();
            if (test.Equals("empty"))
            {
                childs = new List<UIElement>();
            }
            CurrentPanel = new ScoreContentPanel()
            {
                Width = 1200,
                Height = 1000,
                StretchLastRow = true
            };
            PageWidth = CurrentPanel.Width;
            PageHeight = CurrentPanel.Height;
            SetDefaultRows();
            foreach (MeasureVisualPrototype item in childs)
            {
                CurrentPanel.Children.Add(item);
                //item.Number = CurrentPanel.Children.IndexOf(item as UIElement);
            }

        }

        public ContextMenu ContextMenu { get; set; }
        public ScoreContentPanel CurrentPanel { get; set; }
        public Thickness Margin { get; set; }
        public ScoreContentPanel NextPanel { get; set; }
        public double PageHeight { get { return pageHeight; } set { Set(nameof(PageHeight), ref pageHeight, value); } }

        public double PageWidth { get { return pageWidth; } set { Set(nameof(PageWidth), ref pageWidth, value); } }
        public RelayCommand RefreshPageCommand { get; set; }
        public RelayCommand TestCommand { get; set; }
        internal void AddAllNextMeasures(List<MeasureSegmentController> list)
        {
            foreach (var measure in list)
            {
                AddNextMeasure(measure);
            }
        }

        internal void AddNextMeasure(MeasureSegmentController measureSegmentController)
        {
            CurrentPanel.Children.Add(new MeasureVisualTest(measureSegmentController));
            Console.WriteLine("added measure: " + measureSegmentController.MeasureId);
        }

        private void OnAddMeasure()
        {
            if (CurrentPanel != null)
            {
                CurrentPanel.Children.Add(new MeasureVisualPrototype { MinWidth = random.Next(300) + 100 });
            }
            Console.WriteLine("added measure");
        }

        private void OnRefreshPageCommand()
        {
            if (CurrentPanel != null)
            {
                CurrentPanel.InvalidateMeasure();
            }
        }

        private void OnTestCommand()
        {
            if (PageHeight > 300)
            {
                CurrentPanel.Height = CurrentPanel.Height - 30;
                PageHeight = CurrentPanel.Height;

                CurrentPanel.Width = CurrentPanel.Width - 30;
                PageWidth = CurrentPanel.Width;
            }
        }


        private void SetDefaultRows()
        {
            //TODO dynamic margin setting 
            var rowCollection = new RowTopMargins(CurrentPanel)
            {
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 },
                new RowTopMargin { TopMargin = 100 }
            };
            CurrentPanel.RowTopMargins = rowCollection;
        }
    }
}
