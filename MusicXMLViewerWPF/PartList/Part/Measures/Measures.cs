using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using System.Drawing;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using MusicXMLViewerWPF.PartList.Part.Measures;

namespace MusicXMLViewerWPF
{
    class Measures : LoadCharsToViewPort, INotifyPropertyChanged // TODO_H Hard rework
    {

        public static List<Measure> MeasuresList = new List<Measure>(); // list of measures
        public static List<XElement> measuresOfXel = new List<XElement>();
        public static List<Measures> MeasureList = new List<Measures>();
        public static List<Barline> barlineList = new List<Barline>(); // list of defined balines

        private static List<MeasuresLines> helperList = new List< MeasuresLines>();
        private int measure_id;
        private float width;
        private Point s;
        private Point e;
        private bool isFirstInLine;
        private bool isLastInLine;
        private static float scale = 40;
        private float linespacing;
        private static bool m_list_l = false;


        public int MeasureId { get { return measure_id; } }
        public float Width { get { return width; } }
        public Point Start { get { return s; } }
        public Point End { get { return e; } }
        public bool FirstInLine { get { return isFirstInLine; } }
        public bool LastInLine { get { return isLastInLine; } }
        public static float Scale { get { return scale; } set { if (value != 0) { scale = value; } else { scale = 40; } } }
        public float Linespacing { get { return linespacing;  } set { linespacing = scale * 1.5f; } }
        public event PropertyChangedEventHandler PropertyChanged;
        public bool MeasureList_Loaded { get { return m_list_l; } set { if (value != m_list_l) { m_list_l = value; NotifyPropertyChanged(); }  } }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public object DrawVisual { get; private set; }

        // public List<Note> notes;
        public Measures()
        {

        }
        public Measures(XElement x,int num, float width)
        {
            if(x!=null)
            {
                
               measuresOfXel.Add(x);
               
            }
            else
            { 
                Console.WriteLine(" No measures !!!");
            //    tb.writeLineToTextBlock(" No measures !!!");
            }
            
            measure_id = num;
            this.width = width;
            
        }
        public Measures( int id, float width,Point s, Point e,bool start,bool end)
        {
            this.measure_id = id;
            this.width = width;
            this.s = s;
            this.e = e;
            isFirstInLine = start;
            isLastInLine = end;

        }

        public void GetMeasures() // temporary // need reworking if score has more that 1 part //
        {
            //Measure m = new Measure();
            XDocument x = Misc.LoadFile.Document;
            var extr = from measure in x.Descendants("measure") select measure;
            foreach (var item in extr)
            {
                Measure m = new Measure(item);
                MeasuresList.Add(m);
            }
        }

        public static void CoordedMeasures()
        {
            bool st = false;
            bool ed = false;
            GetMeasureLines();
            Measures measure;

            for (int i = 0; i < helperList.Count; i++)
            {
                if (i == 0)
                {
                    int id = i;
                    float w = (float)helperList.ElementAt(i).width;
                    float m = (float)helperList.ElementAt(i).margin;
                    float s = (float)helperList.ElementAt(i).spacing;
                    Point start = new Point(m,s);
                    Point end = new Point(m + w, s);
                    st = true;
                    ed = false;
                    measure = new Measures(id, w, start, end, st, ed);
                    MeasureList.Add(measure);
                }
                else
                {
                    int id = i;
                    float w = (float)helperList.ElementAt(i).width;
                    var sx = MeasureList.ElementAt(i - 1).e.X;
                    var ex = sx + w;
                    var y = MeasureList.ElementAt(i - 1).e.Y;
                    Point start = new Point(sx, y);
                    Point end = new Point(ex, y);
                    if (i + 1 >= helperList.Count)
                    {
                        st = false;
                        ed = true;
                    }
                    else
                    {
                        if (helperList.ElementAt(i + 1).isNewLine)
                        {
                            st = false;
                            ed = true;
                        }

                        else
                        {
                            if (MeasureList.ElementAt(i - 1).isLastInLine)
                            {
                                st = true;
                                ed = false;
                                id = i;
                                w = (float)helperList.ElementAt(i).width;
                                sx = helperList.ElementAt(i).margin;
                                ex = sx + w;
                                y = helperList.ElementAt(i).spacing + MeasureList.ElementAt(i - 1).s.Y;
                                start = new Point(sx, y);
                                end = new Point(ex, y);
                            }
                            else
                            {
                                st = false;
                                ed = false;

                            }
                        }
                    }
                    measure = new Measures(id,w,start,end,st,ed);
                    MeasureList.Add(measure);
                }
            }
        }

        public static Point getCoordsOfMeasure(int num)
        {
            if (helperList.Count == 0) { GetMeasureLines(); }
            Point p = new Point(helperList.ElementAt(num-1).margin, (helperList.ElementAt(num-1).spacing));
            
            return p;
        }
        public void DisplayMeasure(int n=0)
        {
            if (measure_id != 0)
            {
                Logger.Log(measure_id.ToString());
            }
            else
            {
                MessageBox.Show("Wpisz numer taktu !");
            }
            
        }
        // dodawanie brakujacej pieciolini jesli niepełny takt
        private float GetFilling(float l)
        {
            float fill = l % 32;
            return fill;
        }
        // ilosc napisów do wypelnienia taktu
        private float GetMeasureLength(float length)
        {
            float num = Convert.ToInt32(Math.Floor(length / 32));
            return num;
        }
        private static void GetMeasureLines()
        {
            MeasuresLines m;
            bool b = false;
            float l_margin = 5;
            float spacing = 0;
            foreach (var i in measuresOfXel)
            {
                
                float x = float.Parse((i.Attribute("width").Value), CultureInfo.InvariantCulture);
                if(i.Element("print")!=null)
                {
                    b = true;
                   
                    //spacing = Convert.ToInt32(i.Element("print").Element("system-layout").Element("top-system-distance").Value);
                    if( (i.Element("print").HasAttributes ==true ) && i.Element("print").HasElements == true)
                    {
                        spacing = float.Parse((i.Element("print").Element("system-layout").Element("system-distance").Value), CultureInfo.InvariantCulture);
                        l_margin = float.Parse((i.Element("print").Element("system-layout").Element("system-margins").Element("left-margin").Value), CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        if((i.Element("print").HasAttributes == true) && i.Element("print").HasElements == false)
                        {

                        }
                        else
                        {
                            if (i.Element("print").Element("system-layout").Element("system-margins") != null)
                            {
                                l_margin = float.Parse((i.Element("print").Element("system-layout").Element("system-margins").Element("left-margin").Value),CultureInfo.InvariantCulture);
                            }
                        spacing = float.Parse((i.Element("print").Element("system-layout").Element("top-system-distance").Value), CultureInfo.InvariantCulture);
                          b = false;
                        }
                    }
                    m = new MeasuresLines(x, b, l_margin, spacing);
                }
                else
                {
                    b = false;
                    m = new MeasuresLines(x, b, l_margin,spacing);
                }
               
                helperList.Add(m);
                m_list_l = true;
            }
            
        }
        public void DrawStaffLine(DrawingContext dc,float posS, float posE)
        {
            Pen pen = new Pen(Brushes.Black, 1.5);
           
            Line l = new Line();
            int posY = 40;
            
            for (int i = 0; i < 5; i++)
            {
                dc.DrawLine(pen,new Point(posS,posY),new Point(posE,posY));
                posY += 8;
            }
           
        }

        public void DrawBarlines(DrawingContext dc)
        {
            int posY = -30;
            float start = -20;
           // float size = 50; 
            foreach (var item in helperList)
            {
                posY = item.isNewLine == true ? posY + 70 : posY;
                start = item.isNewLine == true ? start = item.width : start += item.width;
              //  DrawString(dc, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, start, posY, size);
             }
        }
        public void getKey(DrawingContext dc, int spoint, int size, int alt = 0, int num = 0, bool isSharp = true)
        {
            //alt = 0; // 0= Gclef 4= Cclef 8= Fclef
            float x = spoint;
            float[] sharp = new float[] { 2, 12, -2, 8, 20, 4, 16 };
            float[] flat = new float[] { 16, 4, 20, 8, 24, 12, 28 };
            int padding = isSharp ? 8 : 6;
            float[] test = isSharp ? sharp : flat;
            string key = isSharp ? MusChar.Sharp : MusChar.Flat;
            for (int i = 0; i < num; i++)
            {
                DrawString(dc, key, TypeFaces.NotesFont, Brushes.Black, x + padding * i, test[i] + alt, size);
            }
        }
        public void DrawMeasures(DrawingVisual visual)
        {

            GetMeasureLines();
            
            float x = helperList.ElementAt(0).margin; 
            float margin_up = helperList.ElementAt(0).spacing - 200;
            float spacing = 60f;
            using(DrawingContext dc=visual.RenderOpen())
                foreach (var item in helperList)
                {
                   spacing = item.spacing;
                   if (item.isNewLine == true)
                   {
                        x = item.margin;
                        margin_up += item.spacing;
                        DrawMeasurex(dc, new Point(x, margin_up), item.width ,true);
                        // measure number DrawString(dc, x.ToString(), TypeFaces.MeasuresFont, Brushes.Blue, x, margin_up, 15);
                        x += item.width;
                    }
                    else
                    {
                       DrawMeasurex(dc, new Point(x, margin_up), item.width,true,true);
                       //measure number DrawString(dc, x.ToString(), TypeFaces.MeasuresFont, Brushes.Black, x, margin_up, 20);
                       x += item.width;
                    }
                    
                }
            /* int posEnd = 0;
            //int c = 0;
            //int s=0;
            //for (int i = 0; i < measuresList.Count; i++)
            //{
            //    c += Convert.ToInt32(measuresList[i].width);
            //    if (measuresList[i].isNewLine == true)
            //    {
            //        s = c;
            //        posEnd = c;
            //        if(c>s)
            //        {
            //            posEnd = c;
            //            c = 0;
            //        }
            //        c = 0;
                    
            //    }
            //} 
            


            f = f != 1 ? f : f;
            float start = 10f;
            int interline = 80;
            int size = 50;
            float posY = 40;
            int length = 300
            */
            // DrawingVisual visual = new DrawingVisual();
            /*
             using (DrawingContext dc = visual.RenderOpen())
             {
                 float u;
                 float z;
                 float v=-4;
                 // DrawStaffLine(dc,start,300f);
                 for (int i = x; i <= 2; i++)
                 {
                     for (int j = 0; j < (length / 30); j++)
                     {
                         DrawString(dc, MusChar.Staff5L, TypeFaces.NotesFont, Brushes.Black, start, posY, size);
                         DrawString(dc, MusChar.Quarter, TypeFaces.NotesFont, Brushes.Black, u=start+size*0.8f, z=posY+v*7, size);
                         start += size *0.80f;
                     }
                     start = 10;
                     posY += size *1.5f;
                 }
                 DrawString(dc, MusChar.FClef, TypeFaces.NotesFont, Brushes.Black, 10, 40, size);
                // getKey(dc, 40, 27, 30,4,false);
                // DrawBarlines(dc);

             }*/
             /*
            int size = 40;
            float width=40;
            float num = GetMeasureLength(width);
            float fill = GetFilling(width);
            float len = num * 32 + fill;
            using (DrawingContext dc = visual.RenderOpen())
            {
                DrawString(dc, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, start, posY - 10, size);
                DrawString(dc, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, start + width, posY - 10, size);
                DrawString(dc, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, start + width * 2, posY - 10, size);
                float s = start;
                float ss = width;
                float line = posY;
                linespacing = 60;
                for (int k = 0; k < x; k++)
                {

                    line = posY + (linespacing * k);
                    for (int j = 0; j < 20; j++)
                    {
                        s = (width * j);
                        s += start;
                        for (int i = 0; i < num; i++)
                        {
                            DrawString(dc, MusChar.Staff5L, TypeFaces.NotesFont, Brushes.Black, s, line, size);
                            s += 32;
                        }

                        if (fill != 0)
                        {
                            DrawString(dc, MusChar.Staff5L, TypeFaces.NotesFont, Brushes.Black, -32 + s + fill, line, size);
                        }
                        DrawString(dc, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, s + fill, line, size);


                        }
                    }
                }

            */
        }
        public void DrawMeasures(CanvasList Surface, DrawingVisual visual, int num)
        {

            if (MeasureList_Loaded)
            {
                for (int i = 0; i < num; i++)
                {
                    
                    using (DrawingContext dc = visual.RenderOpen())
                    {
                        GetMeasureDraw(dc, i);
                    }
                    Surface.AddVisual(visual);
                }
            }
            else
            {
                Console.WriteLine("empty measure list!!! cant draw measures!");
            }
            
        }
        public void DrawMeasurex(DrawingContext dc, Point p,float l, bool left_bar = false, bool right_bar = false)
        {

            float x = (float)p.X;
            float y = (float)p.Y;
            float num = GetMeasureLength(l);
            float fill = GetFilling(l);
            
                for (int i = 0; i < num; i++)
                {
                    DrawString(dc, MusChar.Staff5L, TypeFaces.NotesFont, Brushes.Black, x, y, scale);
                    x += 32;
                }

                if (fill != 0)
                {
                    DrawString(dc, MusChar.Staff5L, TypeFaces.NotesFont, Brushes.Black, -32 + x + fill, y, scale);
                }
            if (left_bar == true)
            {
                DrawString(dc, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, (float)p.X, y, scale);
            }
            if (right_bar == true)
            {
                DrawString(dc, MusChar.SingleBar, TypeFaces.NotesFont, Brushes.Black, (float)p.X + l, y, scale);
            }
        }
        public void GetMeasureDraw(DrawingContext dc, int num)
        {
            Measures m = MeasureList.ElementAt(num);
            float n = GetMeasureLength(m.Width);
            float f = GetFilling(m.Width);
            float x = (float)m.Start.X;
            int s = 0;
            for (int i = 0; i < n; i++)
            {
                DrawString(dc, MusChar.Staff5L, TypeFaces.MeasuresFont, Brushes.Black, x +s,(float)m.Start.Y, Scale);
                DrawString(dc, MusChar.Staff5Ls, TypeFaces.MeasuresFont, Brushes.Black, x + s+24, (float)m.Start.Y, Scale);
                s += 32;
            }
            if (f != 0)
            {
                DrawString(dc, MusChar.Staff5L, TypeFaces.MeasuresFont, Brushes.Black, (float)m.e.X - 32, (float)m.e.Y, Scale);
                DrawString(dc, MusChar.Staff5Ls, TypeFaces.MeasuresFont, Brushes.Black, (float)m.e.X - 8, (float)m.Start.Y, Scale);
            }
            DrawString(dc,MusChar.SingleBar,TypeFaces.MeasuresFont,Brushes.Black,m.Start,Scale);
            DrawString(dc, MusChar.SingleBar, TypeFaces.MeasuresFont, Brushes.Black, m.End, Scale);
            if (m.LastInLine) DrawString(dc, MusChar.SingleBar, TypeFaces.MeasuresFont, Brushes.Black, m.End, Scale);



        }

        
        //public void Add_obj_to_measure(MusicalChars m)
        //{
        //    chars.Add(m);
        //}
        //public void Add_measure_to_measures_list(List<MusicalChars> l)
        //{
        //    measuresOfobj.Add(l);
        //}
        //public int NumberOfMusChars()
        //{
        //    foreach (var i in chars)
        //    {

        //    }
        //    return 
        //}

    }
    class MeasuresLines
    {
        public float width;
        public bool isNewLine;
        public float margin;
        public float spacing;
        public MeasuresLines(float w , bool x,float m=10f,  float s=50f)
        {
            width = w;
            isNewLine = x;
            margin = m;
            spacing = s;
        }
    }
}
