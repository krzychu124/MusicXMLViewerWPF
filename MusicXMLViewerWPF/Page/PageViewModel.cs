
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
namespace MusicXMLScore.Page
{
    class PageViewModel : INotifyPropertyChanged
    {
        #region Private Fiels
        private CanvasList pagecontent = new CanvasList();
        private Orientation orientation = Orientation.Horizontal;
        private ObservableCollection<UIElement> list = new ObservableCollection<UIElement>();
        private Random rndom = new Random();
        private bool fileloaded;
        #endregion

        #region Contructors
        public PageViewModel() // todo get view dimensions !!!
        {
            TestCommand = new RelayCommand(OnTestCommand);
            Mediator.Register("IsFileLoaded", OnFileLoaded); // todo add unregister on view close
            //! ------------------------------------------
            if (FileLoaded)
            {

                GenerateMeasures();
            }
        }

        #endregion

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public CanvasList PageContent { get { return pagecontent; } set { pagecontent = value; } }
        public ObservableCollection<UIElement> List { get { return list; } }
        public Orientation Orientation { get { return orientation; } set { orientation = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Orientation))); } }
        public RelayCommand TestCommand { get; set; }
        public bool FileLoaded { get { return fileloaded; } private set { if (fileloaded != value) fileloaded = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(FileLoaded))); } }
        #endregion

        #region Methods
        private void GenerateCredits()
        {
            if (MusicScore.CreditList.Count > 0)
            {
                DockPanel dp = new DockPanel();
                dp.Width = MusicScore.Defaults.Page.Width;
                dp.Height = 100;
                foreach (var item in MusicScore.CreditList)
                {
                    if (item.Type == MusicXMLViewerWPF.Credit.CreditType.title)
                    {
                        TextBlock tb = TBlock(item.CreditWords.Value, dp.Width, item.Height, item.CreditWords.FontSize, item.CreditWords.FontWeight, HorizontalAlignment.Center, TextAlignment.Center);
                        DockPanel.SetDock(tb, Dock.Top);
                        dp.Children.Add(tb);
                    }
                    if (item.Type == MusicXMLViewerWPF.Credit.CreditType.subtitle)
                    {
                        TextBlock tb = TBlock(item.CreditWords.Value, dp.Width, item.Height, item.CreditWords.FontSize, item.CreditWords.FontWeight, HorizontalAlignment.Center, TextAlignment.Center);
                        //DockPanel.SetDock(tb, Dock.Top);
                        dp.Children.Add(tb);
                    }
                }

                List.Add(dp);
            }
        }
        private void GenerateMeasures()
        {
            if (MusicScore.Parts.Count != 0)
            {
                float scale = MusicScore.Defaults.Scale.Tenths;
                foreach (var item in MusicScore.Parts.ElementAt(0).Value.MeasureSegmentList)
                {
                    Grid grid = new Grid();
                    grid.Height = 100;
                    grid.Width = item.Width;
                    CanvasList cl = new CanvasList(grid.Width, grid.Height);
                    cl.Background = Brushes.Beige;
                    DrawingVisual dv = new DrawingVisual();
                    item.Draw_Measure(dv, new Point(0, 20));
                    cl.AddVisual(dv);
                    StackPanel stckp = new StackPanel();
                    stckp.Orientation = Orientation.Horizontal;
                    if (item.Attributes != null)
                    {
                        if (item.Attributes.Clef != null && item.IsFirstInLine)
                        {
                            var c = item.Attributes.Clef;
                            CanvasList clef = new CanvasList(c.Width, grid.Height);
                            Point clefp = new Point(0.5 * scale, 0.5 * scale);
                            DrawingVisual cv = new DrawingVisual();
                            c.Relative = clefp;
                            c.Draw(cv);
                            clef.AddVisual(cv);
                            stckp.Children.Add(clef);
                        }
                        if (item.Attributes.Key != null)
                        {
                            var k = item.Attributes.Key;
                            CanvasList key = new CanvasList(k.Width, grid.Height);
                            Point keyp = new Point(0.5 * scale, 0.5 * scale);
                            DrawingVisual kv = new DrawingVisual();
                            k.Relative = keyp;
                            k.Draw(kv);
                            key.AddVisual(kv);
                            stckp.Children.Add(key);
                        }
                        if (item.Attributes.Time != null && item.IsFirstInLine)
                        {
                            var t = item.Attributes.Time;
                            CanvasList time = new CanvasList(t.Width, grid.Height);
                            Point timep = new Point(0.5 * scale, 0.5 * scale);
                            DrawingVisual tv = new DrawingVisual();
                            t.Relative = timep;
                            t.Draw(tv);
                            time.AddVisual(tv);
                            stckp.Children.Add(time);
                        }
                    }
                    grid.Children.Add(cl);
                    grid.Children.Add(stckp);
                    List.Add(grid);
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
            test.Background = DrawingHelpers.PickBrush();
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
            GenerateCredits();
            GenerateMeasures();
            
            //if (Orientation == Orientation.Vertical)
            //    Orientation = Orientation.Horizontal;
            //else
            //    Orientation = Orientation.Vertical;
        }

        private void OnFileLoaded(object obj)
        {
            FileLoaded = (bool)obj;
        }
        #endregion
    }
}
