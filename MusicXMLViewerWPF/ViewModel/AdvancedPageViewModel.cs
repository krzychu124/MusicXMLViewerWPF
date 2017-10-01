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

namespace MusicXMLScore.ViewModel
{
    class AdvancedPageViewModel : ViewModelBase
    {
        private double pageHeight;
        private double pageWidth;

        public double PageWidth { get { return pageWidth; } set { Set(nameof(PageWidth), ref pageWidth, value); } }
        public double PageHeight { get { return pageHeight; } set { Set(nameof(PageHeight), ref pageHeight, value); } }
        public Thickness Margin { get; set; }

        public ContextMenu ContextMenu { get; set; }
        public RelayCommand TestCommand { get; set; }
        public ScoreContentPanel CurrentPanel { get; set; }
        public ScoreContentPanel NextPanel { get; set; }

        private readonly Random random = new Random();

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
            foreach (MeasurePrototype item in childs)
            {
                CurrentPanel.Children.Add(item);
                item.Number = CurrentPanel.Children.IndexOf(item as UIElement);
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

        private void OnAddMeasure()
        {
            if (CurrentPanel != null)
            {
                CurrentPanel.Children.Add(new MeasurePrototype { MinWidth = random.Next(150) + 200 });
            }
            Console.WriteLine("added measure");
        }


        private void SetDefaultRows()
        {
            var rowCollection = new RowTopMargins(CurrentPanel)
            {
                new RowTopMargin { TopMargin = 40 },
                new RowTopMargin { TopMargin = 40 },
                new RowTopMargin { TopMargin = 40 },
                new RowTopMargin { TopMargin = 40 },
                new RowTopMargin { TopMargin = 40 },
                new RowTopMargin { TopMargin = 40 },
                new RowTopMargin { TopMargin = 40 },
                new RowTopMargin { TopMargin = 40 }
            };
            CurrentPanel.RowTopMargins = rowCollection;
        }

    }
}
