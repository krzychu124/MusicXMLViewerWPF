
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
namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Contains PartSegmentViewModels as collection
    /// </summary>
    class PageViewModel : INotifyPropertyChanged //TODO_I collection<PartsSegmentViewModel>
    {
        #region Private Fiels
        private CanvasList pagecontent = new CanvasList();
        private Orientation orientation = Orientation.Horizontal;
        private ObservableCollection<CustomPartSystem> partsystems = new ObservableCollection<CustomPartSystem>();
        private Random rndom = new Random();
        private bool fileloaded;
        private string tabViewName = "Name1";
        private ObservableCollection<TextBlock> tabContent = new ObservableCollection<TextBlock>();
        private ObservableCollection<UIElement> partsSegments = new ObservableCollection<UIElement>();

        #endregion

        #region Contructors
        public PageViewModel() // todo_l get view dimensions !!!
        {
            TestCommand = new RelayCommand(OnTestCommand);

            //TOdo collection of PartSegmentViews to represent as Page
            //! here default page which contains one ParSegmentView for simple test
            AddPartSegment();
        }

        private void AddPartSegment()
        {
            PartsSegments.Add(new View.PartsSegmentView());
        }

        #endregion

        #region Properties
        public string TabName { get { return tabViewName; } set { tabViewName = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(TabName))); } }
        public ObservableCollection<TextBlock> TabContent { get { return tabContent; } }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public CanvasList PageContent { get { return pagecontent; } set { pagecontent = value; } }
        public ObservableCollection<CustomPartSystem> List { get { return partsystems; } }
        public Orientation Orientation { get { return orientation; } set { orientation = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Orientation))); } }
        public RelayCommand TestCommand { get; set; }
        public bool FileLoaded { get { return fileloaded; } private set { if (fileloaded != value) fileloaded = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(FileLoaded))); } }
        public ObservableCollection<UIElement> PartsSegments { get { return partsSegments; }  set { partsSegments = value; } }
        #endregion

        #region Methods
        //private void GenerateCredits()
        //{
        //    if (MusicScore.CreditList.Count > 0)
        //    {
        //        DockPanel dp = new DockPanel();
        //        dp.MinWidth = 1500;
        //        dp.Height = 100;
        //        foreach (var item in MusicScore.CreditList)
        //        {
        //            if (item.Type == MusicXMLViewerWPF.Credit.CreditType.title)
        //            {
        //                TextBlock tb = TBlock(item.CreditWords.Value, dp.Width, item.Height, item.CreditWords.FontSize, String.IsNullOrEmpty(item.CreditWords.FontWeight)? "normal": item.CreditWords.FontWeight, HorizontalAlignment.Center, TextAlignment.Center);
        //                DockPanel.SetDock(tb, Dock.Top);
        //                dp.Children.Add(tb);
        //            }
        //            if (item.Type == MusicXMLViewerWPF.Credit.CreditType.subtitle)
        //            {
        //                TextBlock tb = TBlock(item.CreditWords.Value, dp.Width, item.Height, item.CreditWords.FontSize, String.IsNullOrEmpty(item.CreditWords.FontWeight) ? "normal" : item.CreditWords.FontWeight, HorizontalAlignment.Center, TextAlignment.Center);
        //                //DockPanel.SetDock(tb, Dock.Top);
        //                dp.Children.Add(tb);
        //            }
        //        }

        //        List.Add(dp);
        //    }
        //}
        //private void GenerateMeasures()
        //{
        //    if (MusicScore.Parts.Count != 0)
        //    {
        //        float scale = MusicScore.Defaults.Scale.Tenths;
        //        foreach (var item in MusicScore.Parts.ElementAt(0).Value.MeasureSegmentList)
        //        {
        //            Grid grid = new Grid();
        //            grid.Height = 100;
        //            grid.MinWidth = item.Width;
        //            if (item.IsFirstInLine)
        //            {
        //                grid.SetValue(CustomWrapPanel.BreakRowProperty, true);
        //            }
        //            CanvasList cl = new CanvasList(grid.Width, grid.Height);
        //            //cl.Background = Brushes.Transparent;
        //            DrawingVisual dv = new DrawingVisual();
        //            item.Draw_Measure(dv, new Point(0, 20));
        //            StaffLineCanvas slc = new StaffLineCanvas();
        //            cl.AddVisual(dv);
        //            grid.SizeChanged += Grid_SizeChanged;
        //            StackPanel stckp = new StackPanel();
        //            stckp.SizeChanged += Grid_SizeChanged;
        //            stckp.Tag = "sp";
        //            stckp.Orientation = Orientation.Horizontal;
        //            if (item.Attributes != null)
        //            {
        //                if (item.Attributes.Clef != null && item.IsFirstInLine)
        //                {
        //                    var c = item.Attributes.Clef;
        //                    CanvasList clef = new CanvasList(c.Width, grid.Height);
        //                    Point clefp = new Point(0.5 * scale, 0.5 * scale);
        //                    DrawingVisual cv = new DrawingVisual();
        //                    c.Relative = clefp;
        //                    c.Draw(cv);
        //                    clef.AddVisual(cv);
        //                    stckp.Children.Add(clef);
        //                }
        //                if (item.Attributes.Key != null)
        //                {
        //                    var k = item.Attributes.Key;
        //                    CanvasList key = new CanvasList(k.Width, grid.Height);
        //                    Point keyp = new Point(0.5 * scale, 0.5 * scale);
        //                    DrawingVisual kv = new DrawingVisual();
        //                    k.Relative = keyp;
        //                    k.Draw(kv);
        //                    key.AddVisual(kv);
        //                    stckp.Children.Add(key);
        //                }
        //                if (item.Attributes.Time != null && item.IsFirstInLine)
        //                {
        //                    var t = item.Attributes.Time;
        //                    CanvasList time = new CanvasList(t.Width, grid.Height);
        //                    Point timep = new Point(0.5 * scale, 0.5 * scale);
        //                    DrawingVisual tv = new DrawingVisual();
        //                    t.Relative = timep;
        //                    t.Draw(tv);
        //                    time.AddVisual(tv);
        //                    stckp.Children.Add(time);
        //                }
        //            }
        //            DrawingVisual gv = new DrawingVisual();
        //            DrawRect(gv, new Rect(new Point(), new Size(grid.Width, grid.Height)), Brushes.Black);
        //            cl.AddVisual(gv);
        //            //grid.Children.Add(slc);
        //            grid.Children.Add(cl);
        //            grid.Children.Add(stckp);
        //            List.Add(grid);
        //        }
        //        list2 = new ObservableCollection<UIElement>(List );
        //       // PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(List2)));
        //    }
        //}

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var x = sender.GetType();
            if (x.Name == "Grid")
            {

                var g = sender as Grid;
                var s = new Size(g.Width, g.Height);
                if (e.NewSize != s)
                {
                    //int i = 0;
                }
            }
            if (x.Name == "StackPanel")
            {
                var g = sender as StackPanel;
                var s = new Size(g.Width, g.Height);
                var nn = new Size(double.NaN, double.NaN);
                if (e.NewSize != s && s == nn)
                {
                    //int i = 0;
                }
            }
        }

        private TextBlock TBlock(string text, double w, double h, float size = 16, string fweight = "Normal", HorizontalAlignment hl = HorizontalAlignment.Left, TextAlignment txtalign = TextAlignment.Left) 
        {
            FontWeight fw = FontWeights.Normal;
            if (fweight.ToLower() == "bold")
            {
                fw = FontWeights.Bold;
            }
            TextBlock tblock = new TextBlock();
            tblock.Text = text;
            tblock.Width = w;
            tblock.Height = h;
            tblock.FontSize = size;
            tblock.FontWeight = fw;
            tblock.HorizontalAlignment = hl;
            tblock.TextAlignment = txtalign;
            return tblock;
        }


        public void DrawRect(DrawingVisual dv, Rect r, Brush b)
        {
            DrawingVisual f = new DrawingVisual();
            using (DrawingContext dc = f.RenderOpen())
            {
                dc.DrawRectangle(Brushes.Transparent, new Pen(b, 1), r);
            }
            dv.Children.Add(f);
        }
        
        public CanvasList Test(double w, double h)
        {
            CanvasList test = new CanvasList();
            //test.Width = w;
            test.Height = h;
            
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                DrawingHelpers.DrawString(dc, "<==>", Helpers.TypeFaces.TextFont, Brushes.Black, 5, 5, 12);
            }
            DrawRect(dv, dv.DescendantBounds, DrawingHelpers.PickBrush());
            DrawRect(dv, dv.ContentBounds, DrawingHelpers.PickBrush());
            DrawRect(dv, new Rect(0,0, w, h), DrawingHelpers.PickBrush());
            test.AddVisual(dv);
            test.Width = dv.DescendantBounds.Width;
           // test.Background = DrawingHelpers.PickBrush();
            return test;
        }

        public CanvasList MeasureCanvas()
        {
            CanvasList cl = new CanvasList();
            return cl;
        }
        #endregion

        #region Commands, Action<>, Func<>
        private void OnTestCommand()
        {
        }

        private void OnFileLoaded(object obj)
        {
            FileLoaded = (bool)obj;
        }
        #endregion
    }
}
