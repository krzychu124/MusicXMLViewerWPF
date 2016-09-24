using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            //  LogBox.FontFamily = new FontFamily("BravuraText,FreeSerif");
            //LogBox.FontSize = 40;
            // Console.WriteLine("\U0001D122");
            // LogBox.Text += "-> \uE050 <-";
            //LogBox.DataContext = Logger.Text;
            //!Logger.LogAdded += new EventHandler(MyLogger_LogAdded);
            //? Logger.LogAdded += new EventHandler(MyLogger_TxtBox_Add);
            //! Logger.LogCleared += new EventHandler(MyLogger_LogClear);
            Logger.LogCleared += new EventHandler(MyLogger_TxtBox_Clear);
            int c = 0;
            //Logger.Log("check");
            // Measures m = new Measures();
            // m.MeasureList_Loaded = true;
            // LoadCharsToViewPort l = new LoadCharsToViewPort(drawingSurface);
        }

        

        /*    public void test()
   {
       Typeface typeface = new Typeface(new FontFamily("Arial"),
                               FontStyles.Italic,
                               FontWeights.Normal,
                               FontStretches.Normal);

       GlyphTypeface glyphTypeface;
if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
throw new InvalidOperationException("No glyphtypeface found");

       string text = "Hello, world!";
       double size = 40;

       ushort[] glyphIndexes = new ushort[text.Length];
       double[] advanceWidths = new double[text.Length];

       double totalWidth = 0;

for (int n = 0; n<text.Length; n++)
{
ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];
       glyphIndexes[n] = glyphIndex;

double width = glyphTypeface.AdvanceWidths[glyphIndex] * size;
       advanceWidths[n] = width;

totalWidth += width;
}

   Point origin = new Point(50, 50);
       DrawingContext dc = new DrawingContext();
   GlyphRun glyphRun = new GlyphRun(glyphTypeface, 0, false, size,
       glyphIndexes, origin, advanceWidths, null, null, null, null,
       null, null);

   dc.DrawGlyphRun(Brushes.Black, glyphRun);

double y = origin.Y;
   dc.DrawLine(new Pen(Brushes.Red, 1), new Point(origin.X, y), 
new Point(origin.X + totalWidth, y));

y -= (glyphTypeface.Baseline* size);
   dc.DrawLine(new Pen(Brushes.Green, 1), new Point(origin.X, y), 
new Point(origin.X + totalWidth, y));

y += (glyphTypeface.Height* size);
   dc.DrawLine(new Pen(Brushes.Blue, 1), new Point(origin.X, y), 
new Point(origin.X + totalWidth, y));
}
*/
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
           // Button b = new Button();
           
            //.SetValue(20, sender);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "MusicXML files|*.xml";
            if (dialog.ShowDialog() == true)
            {
                //LoadDocToClasses.list.Clear();
                //LoadDocToClasses.MeasuresList.Clear();
                //LoadCharsToViewPort.x.Clear();
                XmlRead xmlReader = new XmlRead();
                //viewer.LoadFile(dialog.FileName);
                xmlReader.File_path = dialog.FileName;
                Logger.Log("Loading: "+dialog.FileName);
                Console.WriteLine("Loaded file>> "+ dialog.FileName);
           //     textBlock.Text += "\n Loadind file ... Processing  ";
                
                XDocument Doc = XmlRead.GetXmlInventory(dialog.FileName);
                //LoadDocToClasses.Document = Doc;
                //Misc.LoadFile.LoadDocument(Doc);
                MusicScore mus_score = new MusicScore(Doc);
                /*
                LoadDocToClasses.AddMeasuresToXListV(Doc);
                //   textBlock.Text += "\n File imported to measures list \n Press Load button to process";
                List<MusicalChars> list;
                LoadDocToClasses.LoadCharsFromMeasures();
                list = LoadDocToClasses.list;
                Logger.Log("XML Loaded");
                */
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MusicScore.isLoaded)
            {
                MusicScore.Draw(drawingSurface);
                Logger.Log($"Drawn {drawingSurface.Count_()} visuals");
            }
            else
            {
                Logger.Log("Please load XML file first");
            }
            
            //Page p = new Page();
            //PartList s = new PartList(); // tests
            // textBlock.Text += "\n Characters added to program";
            //  list = LoadDocToClasses.list;
            // textBlock.Text += "\n Added: " +list.Count.ToString();

        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
       // private static int c = 2;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Measures m = new Measures();
            DrawingVisual visual = new DrawingVisual();
            m.DrawMeasures(visual);
            drawingSurface.AddVisual(visual);
            Logger.Log($"Drawn {drawingSurface.Count_()} visuals");
            //m.DrawMeasures(visual,4,8);

            //m.DrawMeasure(visual, new Point(10, 40), 200);
            //drawingSurface.AddVisual(visual);
            //visual = new DrawingVisual();
            // m.DrawMeasure(visual,new Point(215, 40), 200,true);

            //tblock t = new tblock();
            //DrawingVisual visual = new DrawingVisual();
            //test(visual,c);
            //c += 12;
            //drawingSurface.AddVisual(visual);
            //t.writeToTextBlock(c.ToString());

            //Console.WriteLine(drawingSurface.Count_());
            //// page.Refresh();
            ////p.InvalidateVisual();
            //Console.WriteLine( drawingSurface.ActualWidth.ToString());
        }
        private void test (DrawingVisual visual, int x)
        {
            Brush drawingBrush2 = Brushes.Black;
            using (DrawingContext dc = visual.RenderOpen())
            {
                FormattedText t = new FormattedText("test", Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("FreeSefrif"), 20.0, drawingBrush2);
                dc.DrawText(t, new Point(5f, 10.0f));
               // DrawString(dc, MusChar.FClef, TypeFaces.NotesFont, Brushes.Black, x, 10.0f, 40.0f);
                // page.AddVisual(dc);
            }
        }

        private void load_M_Click(object sender, RoutedEventArgs e)
        {
            Measures m = new Measures();
            m.DisplayMeasure();
        }

        private void test1_Click(object sender, RoutedEventArgs e)
        {
            if (MusicScore.isLoaded)
            {
                //! drawing page 
                DrawingVisual visual = new DrawingVisual();
                MusicScore.DrawPageRectangle(visual);
                drawingSurface.AddVisual(visual);
                //! drawing margins rectangle
                DrawingVisual visual2 = new DrawingVisual();
                MusicScore.DrawMusicScoreMargins(visual2);
                drawingSurface.AddVisual(visual2);
            }
            else
            {
                Logger.Log("Please load XML file first");
            }
        }

        private void test2_Click(object sender, RoutedEventArgs e)
        {
            
            if (MusicScore.isLoaded)
            {
                drawingSurface.Width = MusicScore.Defaults.Page.Width;
                drawingSurface.Height = MusicScore.Defaults.Page.Height;
                //drawingSurface.InvalidateVisual();
                //...
                //LoadCharsToViewPort c = new LoadCharsToViewPort();
                //c.AddClef(visual);
                //drawingSurface.AddVisual(visual);

            }
            else
            {
                Logger.Log("Please load XML file first");
            }
            
        }
        private void test3_Click(object sender, RoutedEventArgs e)
        {
            if (MusicScore.isLoaded)
            {
                DrawingVisual visual = new DrawingVisual();
                //...
                MusicScore.DrawMusicScoreTitleSpace(visual);
                drawingSurface.AddVisual(visual);
            }
            else
            {
                Logger.Log("Please load XML file first");
            }
        }

        private void test4_Click(object sender, RoutedEventArgs e)
        {
            if (MusicScore.isLoaded)
            {
                DrawingVisual visual = new DrawingVisual();
                //...
                MusicScore.DrawBreaks(visual);
                drawingSurface.AddVisual(visual);
            }
            else
            {
                Logger.Log("Please load XML file first");
            }
        }

        public void test5_Click(object sender, RoutedEventArgs e)
        {

            MusicScore m = new MusicScore();
            if (m.ContentSpaceCalculated == true)
            {
                DrawingVisual visual = new DrawingVisual();
                MusicScore.DrawMusicScoreMeasuresContentSpace(visual);
                drawingSurface.AddVisual(visual);
            }
            else
            {
                Logger.Log("Please Calculate Content Space first");
            }

        }

        private void test6_Click(object sender, RoutedEventArgs e)
        {
            if (MusicScore.isLoaded)
            {
                //LoadCharsToViewPort sur = new LoadCharsToViewPort(drawingSurface);
                //sur.AddNotes();
                Page page = MusicScore.Defaults.Page;
                string credit = Credit.Credit.Titlesegment.Rectangle.ToString();
                Logger.Log("Space inside margins "+page.ContentSpace_str); //? content space inside margins
                Logger.Log("Space without titlle "+ page.MeasuresContentSpace_str); //? content space avaliable for drawing measures
                Logger.Log("Space for title, credits " + credit); //? credit space dimensions
                foreach (var item in MusicScore.Parts)
                {
                    foreach (var item2 in item.Value.MeasureSegmentList)
                    {
                        DrawingVisual segment_vis = new DrawingVisual();
                        item2.Draw(segment_vis, Brushes.Blue, DashStyles.Solid);
                        //! DEBUG
                        /* DrawingVisual debug_coords = new DrawingVisual();
                       using (DrawingContext dc = debug_coords.RenderOpen()) //! Debug info
                       {
                           Misc.DrawingHelpers.DrawText(dc, item.Relative_str, new Point(item.Relative.X + 10f, item.Relative.Y + 15f), 10, font_weight: "regular", withsub:false, align: Halign.left);
                           Misc.DrawingHelpers.DrawText(dc, item.Width.ToString() + "px width", new Point(item.Relative.X + 10f, item.Relative.Y + 28f), 10, withsub: false, color: Brushes.DarkGray, align: Halign.left);
                       }
                       drawingSurface.AddVisual(debug_coords);
                       */
                        DrawingVisual item_vis = new DrawingVisual();
                        item2.Draw(item_vis);
                        drawingSurface.AddVisual(item_vis);
                        drawingSurface.AddVisual(segment_vis);
                    }
                    DrawingVisual credits = new DrawingVisual();
                    MusicScore.DrawCredits(credits);
                    drawingSurface.AddVisual(credits);
                }
            }
            else
            {
                Logger.Log("Please load XML file first");
            }
        }

        private void clearAll_Click(object sender, RoutedEventArgs e)
        {
            MusicScore.Clear();
            drawingSurface.ClearVisuals();
        }
            //Defaults.Appearance app = new Defaults.Appearance();
            ////app.initLineWidths(LoadDocToClasses.Document);

            //Logger.Log("test log string");
            //DrawingVisual visual = new DrawingVisual();
            //using (DrawingContext dc = visual.RenderOpen())
            //{
            //    Pen pen = new Pen(Brushes.Black, 4);
            //    List<double> dots = new List<double>() {0.3,4};
            //    List<double> dashes = new List<double>() { 3, 2.5 };
            //    DashStyle d = new DashStyle(dashes, 0);

            //    pen.DashStyle = d;
            //    StreamGeometry sg = new StreamGeometry();
            //    using(StreamGeometryContext sgc = sg.Open())
            //    {

            //        float offset = 4f;
            //        Point s = new Point(100,100);
            //        Point p2 = new Point(100,150);
            //        float distance = Calc.Distance(s, p2);
            //        Point p1 = Calc.PerpendicularOffset(s, p2, -distance/(offset * 0.6f));
            //        Point p3 = Calc.PerpendicularOffset(s, p2, -distance/(offset * 0.9f));                  //new Point(250,50);
            //        sgc.BeginFigure(s, false, false);
            //        sgc.QuadraticBezierTo(p1, p2, true, true);
            //       // sgc.QuadraticBezierTo(p3, s, true, true);
            //    }
            //    sg.Freeze();
            //    dc.DrawGeometry(Brushes.Black, pen, sg);
            //}
            //drawingSurface.AddVisual(visual);
        
        private Point MidPoint(Point p1, Point p2)
        {
            Point Mid;
            Mid= new Point((p1.X + p2.X)/2, (p1.Y + p2.Y)/2 );
            return Mid;
        }
        private float slope( Point p1, Point p2)
        {
            float slope = (float)((p2.Y - p1.Y) / (p2.X - p1.X));
            return slope;
        }

        private Point tst(Point p1, Point p2 , float distance)
        {
            Point M = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
            Point p = new Point(p1.X- p2.X, p1.Y - p2.Y);
            Point n = new Point(-p.Y, p.X);
            int norm_length = (int)Math.Sqrt((n.X * n.X) + (n.Y * n.Y));
            n.X /= norm_length;
            n.Y /= norm_length;
            return new Point(M.X + (distance * n.X), M.Y + (distance * n.Y));
        }
        private float dist(Point p1, Point p2)
        {
            float dist = (float)Math.Sqrt( Math.Pow((p1.X-p2.X),2) + Math.Pow((p1.Y - p2.Y),2));
            return dist;
        }
        //? TextBlock delegates
        //void MyLogger_LogAdded(object sender, EventArgs e)
        //{
        //    LogBox.Text = LogBox.Text + Environment.NewLine + Logger.GetLastLog();
        //}

        //void MyLogger_LogClear(object sender, EventArgs e)
        //{
        //    LogBox.Text = "";
        //}

        void MyLogger_TxtBox_Add(object sender, EventArgs e)
        {
            textBox.Text = textBox.Text + Environment.NewLine + Logger.GetLastLog();
        }
        void MyLogger_TxtBox_Clear(object sender, EventArgs e)
        {
            textBox.Text = "";
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Logger.ClearLog();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            Logger.LogAdded += new EventHandler(MyLogger_TxtBox_Add);
            //Logger.LogCleared += new EventHandler(MyLogger_TxtBox_Clear);
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Logger.LogAdded -= MyLogger_TxtBox_Add;
        }
            //public void CreateALine()
            //{
            //    DrawingGroup d = new DrawingGroup();
            //    DrawingVisual visual = new DrawingVisual();
            //    using (DrawingContext dc = visual.RenderOpen())
            //    {
            //        // Create a Line
            //        Line redLine = new Line();
            //        redLine.X1 = 50;
            //        redLine.Y1 = 50;
            //        redLine.X2 = 200;
            //        redLine.Y2 = 200;
            //       // DrawingContext dc;
            //        DrawString(dc, MusChar.FClef, TypeFaces.NotesFont, Brushes.Black, 5f, 10.0f, 40.0f);
            //        // Create a red Brush
            //        SolidColorBrush redBrush = new SolidColorBrush();
            //        redBrush.Color = Colors.Red;

            //        // Set Line's width and color
            //        redLine.StrokeThickness = 4;
            //        redLine.Stroke = redBrush;
            //    }

            //    // Add line to the Grid.
            //can.Children.Add();
            //}
            //private DrawingVisual CreateDrawingVisualRectangle()
            //{
            //    DrawingVisual drawingVisual = new DrawingVisual();

            //    // Retrieve the DrawingContext in order to create new drawing content.
            //    DrawingContext drawingContext = drawingVisual.RenderOpen();

            //    // Create a rectangle and draw it in the DrawingContext.
            //    Rect rect = new Rect(new System.Windows.Point(160, 100), new System.Windows.Size(320, 80));
            //    drawingContext.DrawRectangle(System.Windows.Media.Brushes.LightBlue, (System.Windows.Media.Pen)null, rect);

            //    // Persist the drawing content.
            //    drawingContext.Close();

            //    return drawingVisual;
            //}

        }

    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Input, EmptyDelegate);
        }
    }
}
