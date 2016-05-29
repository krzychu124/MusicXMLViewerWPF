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
using System.Windows.Shapes;
using System.Drawing;

namespace MusicXMLTestViewerWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    
    public partial class Window1 : UserControl
    {
        public static void InsertLine()
        {
            Console.WriteLine("=====================================================================================================");
        }

        private string file_path = null;
        public Window1()
        {
            InitializeComponent();
            
        }

        internal void LoadFile(string fileName)
        {
            file_path = fileName;
            //XmlRead.GetXmlInventory();
        }
        /*
        public static void DrawString(DrawingContext d, string text, Typeface f, Brush b, float xPos, float yPos, float emSize)
        {
            //This function mimics Graphics.DrawString functionality
           d.DrawText(new FormattedText(text, Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, f, emSize, b), new Point(xPos, yPos));
            
        }
        
        
        

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            
            Pen pen = new Pen(Brushes.Black, 0.1f);

            int[] lines = new int[5];

            const int paddingTop = 20;
            const int lineSpacing = 6;
            //Draw staff lines / Rysuj pięciolinię
            string staff = MusChar.Staff5L;
            for (int i = 0; i < Width / 20; i++)
                staff = staff + MusChar.Staff5L;

            Point startPoint = new Point(0, paddingTop);
            Point endPoint = new Point(Width, paddingTop);

            //for (int i = 0; i < 5; i++)
            //{
             drawingContext.DrawLine(pen, startPoint, endPoint);
            //    lines[i] = paddingTop + i * lineSpacing;
            //    startPoint.Y += lineSpacing;
            //    endPoint.Y += lineSpacing;

            //}
            
           

            DrawString(drawingContext, staff, TypeFaces.NotesFont, Brushes.Black, 0f, 10.0f, 40.0f);
            DrawString(drawingContext, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, 0f, 10.0f, 40.0f);
            DrawString(drawingContext, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, 225f, 10.0f, 40.0f);
            //time
            DrawString(drawingContext, MusChar.FClef, TypeFaces.NotesFont, Brushes.Black, 5f, 10.0f, 40.0f);
            DrawString(drawingContext, "6", TypeFaces.TimeNumbers, Brushes.Black, 80f, 15.0f, 19.0f);
            DrawString(drawingContext, MusChar.eight, TypeFaces.TimeNumbers, Brushes.Black, 80f, 15.0f, 19.0f);
            DrawString(drawingContext, "2", TypeFaces.TimeNumbers, Brushes.Black, 95f, 15.0f, 19.0f);
            DrawString(drawingContext, MusChar.four, TypeFaces.TimeNumbers, Brushes.Black, 95f, 15.0f, 19.0f);
            DrawString(drawingContext, "3", TypeFaces.TimeNumbers, Brushes.Black, 110f, 15.0f, 19.0f);
            DrawString(drawingContext, MusChar.four, TypeFaces.TimeNumbers, Brushes.Black, 110f, 15.0f, 19.0f);
            DrawString(drawingContext, MusChar.CommonTime, TypeFaces.NotesFont, Brushes.Black, 120f, 10.0f, 40.0f);
            DrawString(drawingContext, MusChar.CutTime, TypeFaces.NotesFont, Brushes.Black, 140f, 10.0f, 40.0f);
            //key
            //DrawString(drawingContext, MusChar.Sharp, TypeFaces.NotesFont, Brushes.Black, 40f, 2.0f, 20.0f);
            //DrawString(drawingContext, MusChar.Sharp, TypeFaces.NotesFont, Brushes.Black, 50f, 12.0f, 20.0f);
            //DrawString(drawingContext, MusChar.Sharp, TypeFaces.NotesFont, Brushes.Black, 60f, -2.0f, 20.0f);
            //DrawString(drawingContext, MusChar.Sharp, TypeFaces.NotesFont, Brushes.Black, 70f, 8.0f, 20.0f);
            //DrawString(drawingContext, MusChar.Sharp, TypeFaces.NotesFont, Brushes.Black, 80f, 20.0f, 20.0f);
            //DrawString(drawingContext, MusChar.Sharp, TypeFaces.NotesFont, Brushes.Black, 90f, 4.0f, 20.0f);
            //DrawString(drawingContext, MusChar.Sharp, TypeFaces.NotesFont, Brushes.Black, 100f, 16.0f, 20.0f);
            //float x = 40f;
            ////float y = f;
            //float[] sharp = new float[] { 2,12,-2,8,20,4,16};
            //float[] flat = new float[] { 16,4,20,8,24,12,28};
            //int alt = 0; // 4= Cclef 8= Fclef
            //for (int i = 0; i < 7; i++)
            //{

            //    DrawString(drawingContext, MusChar.Sharp, TypeFaces.NotesFont, Brushes.Black, x + 8 * i, sharp[i]+alt, 20.0f);

            //}
            //getKey(drawingContext,0,4,false);

        }*/
        
    }
}
