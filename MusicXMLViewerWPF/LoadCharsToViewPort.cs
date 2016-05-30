//#define ASD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MusicXMLViewerWPF
{

    class LoadCharsToViewPort  // temporary class for drawing on canvas but thinking about making interface to do this and attach to each class object could be drawn eg. note, measure, key etc... // that could be useful when i want to add any mouse action on drawn object
    {
        class temp // temporary class for calculations // no idea how to rework this for now
        {
            public int id { get; set; }
            public int o { get; set; }
            public Point p { get; set; }
            public bool start { get; set; }
            public temp(int id,int operation, Point p, bool isS)
            {
                this.id = id;
                o = operation;
                this.p = p;
                start = isS;
            }
        }
        CanvasList surface;
        
        public CanvasList Surface { get { return surface; } set{ surface = value; } }
        public LoadCharsToViewPort()
        { }
        public LoadCharsToViewPort(CanvasList list)
        {
            Surface = list;
        }

        public static List<MusicalChars> x = LoadDocToClasses.list;
        public void AddClef(DrawingVisual visual)
        {
            

            var z = x.OfType<Clef>();
            int line = z.ElementAt(0).Line;
            string s = z.ElementAt(0).Sign.Symbol;
            int num = z.ElementAt(0).MeasureId;
            List<Point> clefList = new List<Point>();
            foreach (var item in Measures.MeasureList) // add clef to begining of each line of measures
            {
                if (item.FirstInLine)
                {
                    clefList.Add(item.Start);
                }
            }
            using (DrawingContext dc = visual.RenderOpen())
            {
                foreach (var item in clefList)
                {
                    DrawString(dc, s, TypeFaces.NotesFont, Brushes.Black, item, Measures.Scale);
                }
                
            }

        }
        public void AddKey(DrawingVisual visual)
        {
            //var m = from i in x where i.type == MusSymbolType.Key select i;
            var z = x.OfType<Key>();
            var c = x.OfType<Clef>();
            ClefType clef = c.ElementAt(0).Sign;
            int num = z.ElementAt(0).MeasureId;
            int f = (int)z.ElementAt(0).Fifths;
            using (DrawingContext dc = visual.RenderOpen())
            {
                Draw_Key(dc,Measures.getCoordsOfMeasure(num),Measures.Scale,clef,f);
            }

        }
        public void AddMeasures(int num)
        {
            Measures m = new Measures();
            for (int i = 0; i < num; i++)
            {
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen())
                {
                    m.GetMeasureDraw(dc, i);
                }
                surface.AddVisual(visual);
            }
        }
        public void AddNotes()
        {
            var n = x.OfType<Note>(); // get all object of type Note
            List<temp> Prev_list = new List<temp>(); // temporary list of beams
            List<temp> Notations_list = new List<temp>(); // list of notations
            int beamseparator;
            float hook;
            Pen pen = new Pen(Brushes.Black, 1);
            
            for (int i = 0; i < n.Count(); i++)
            {
                beamseparator = n.ElementAt(i).Stem_dir ? -5 : 5;
                hook = n.ElementAt(i).Stem_dir ? 12 : -12;  
                int num = n.ElementAt(i).MeasureId;    // get measure number
                int measx = (int)Measures.MeasureList.ElementAt(num-1).Start.X; // X pos of current measure 
                int measy = (int)Measures.MeasureList.ElementAt(num-1).Start.Y; // y pos of current measure
                float x = n.ElementAt(i).PosX + measx; //calc. xpos of note
                float y = n.ElementAt(i).PosY + measy; // calc. ypos of note according to measure
                
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen())
                {
                    if (n.ElementAt(i).Pitch.HasAddedLine == true) // check for notes which are placed below or upper regular 5LineStaff
                    {
                        float temp = y;
                        for (int j = 0; j < n.ElementAt(i).Pitch.AdditionalLines; j++) //loop for additional lines
                        {
                            if (n.ElementAt(i).Stem_dir)
                            {

                                if (n.ElementAt(i).Pitch.isLineUnderNote) //additional line below note
                                {
                                    AdditionalLine(dc, new Point(x, temp + 1));
                                }
                                else  //through note
                                {
                                    AdditionalLine(dc, new Point(x, temp - 3));
                                }
                                temp += 7;  //next additional line padding
                            }
                            else
                            {
                                if (n.ElementAt(i).Pitch.isLineUnderNote) // additional line through note
                                {
                                    AdditionalLine(dc, new Point(x, temp - 7));
                                }
                                else //through note
                                {
                                    AdditionalLine(dc, new Point(x, temp - 3));
                                }
                                temp += -7;  //next additional line padding
                            }

                        }
                    }
                    if (n.ElementAt(i).HasDot) //has dot
                    {
                        int z = Math.Abs(n.ElementAt(i).Pitch.CalculatedStep) % 2 == 1 ? 0 : 3; // calcutale Y offset of dot // if note at line or between
                        DrawString(dc, MusChar.Dot, TypeFaces.NotesFont, Brushes.Black, new Point(x + 14, y + z), Measures.Scale); //draw
                    }
                    if (n.ElementAt(i).HasBeams) //has beams
                    {
                        if (n.ElementAt(i).isDefault_Stem)
                        {
                            measy = measy + (int)n.ElementAt(i).DefaultStem;
                        }
                        float y_p = n.ElementAt(i).PosY + measy;
                        float stemlength = n.ElementAt(i).Stem_dir ? measy + 5 - n.ElementAt(i).Stem : measy + 12 - n.ElementAt(i).Stem;
                        float xoff = n.ElementAt(i).Stem_dir ? 1f : 9f;
                        float yoff = n.ElementAt(i).Stem_dir ? 0f : -29f;
                        float yl = n.ElementAt(i).Stem_dir ? y + 29 : y + 29;
                        float x_ = x + xoff;


                        if (n.ElementAt(i).SymbolType == MusSymbolDuration.Half) // check if half-note // draw note dot
                        {
                            DrawString(dc, MusChar.HalfDot, TypeFaces.NotesFont, Brushes.Black, new Point(x, y), Measures.Scale);
                            dc.DrawLine(pen, new Point(x + xoff, yl), new Point(x + xoff, measy - stemlength + yoff));
                        }
                        else
                        {

                            DrawString(dc, MusChar.QuarterDot, TypeFaces.NotesFont, Brushes.Black, new Point(x, y), Measures.Scale);
                            dc.DrawLine(pen, new Point(x + xoff, yl), new Point(x + xoff, stemlength)); 
                            
                        }

                        //for (int b = 0; b < n.ElementAt(i).Beam.NoteBeamsCount; b++) 
                        foreach (var beam in n.ElementAt(i).Beam.NoteBeamList)  // each beam
                        {

                            switch (beam.Value)   //// switch (n.ElementAt(i).Beam.BeamType)
                            {
                                case 1: //Beam.Beam_type.next:
                                    //Prev_list.Add(new temp(n.ElementAt(i).Beam.NoteBeamList.ElementAt(b).Key, n.ElementAt(i).Beam.NoteBeamList.ElementAt(b).Value, new Point(x_, stemlength), false));
                                    Prev_list.Add(new temp(beam.Key, beam.Value, new Point(x_, stemlength), false));
                                    break;
                                case 3: //Beam.Beam_type.forward:
                                    //  DrawBeam(dc, new Point(x + xoff + beamseparator, y + yoff), new Point(x + xoff + hook, y + yoff ), n.ElementAt(i).Stem_dir);
                                    break;
                                case 4: //Beam.Beam_type.backward:
                                    float padd = n.ElementAt(i).Stem_dir ? beam.Key * -7 : -(beam.Key * -7);  // padding according to stem direction
                                    float x_p = n.ElementAt(i - 1).PosX + measx;
                                    float stemlength_p = n.ElementAt(i - 1).Stem_dir ? measy + 5 - n.ElementAt(i - 1).Stem : measy + 12 - n.ElementAt(i - 1).Stem;
                                    float x__p = x_p + xoff;
                                    float tem = stemlength - (0.25f * (stemlength - stemlength_p));
                                    hook = x_ - (0.25f * (x_ - x__p));
                                    Draw_Beam(dc, new Point(hook, padd + tem), new Point(x_, stemlength + padd), n.ElementAt(i).Stem_dir);
                                    //Prev_list.RemoveAll(it => it.id == dist);
                                    // DrawBeam(dc, new Point(x+xoff-10,stemlength+beamseparator), new Point(x + xoff, stemlength + beamseparator), n.ElementAt(i).Stem_dir);
                                    break;                           //hook
                                case 0: // Beam.Beam_type.start:
                                    //Prev_list.Add(new temp(n.ElementAt(i).Beam.NoteBeamList.ElementAt(b).Key, n.ElementAt(i).Beam.NoteBeamList.ElementAt(b).Value, new Point(x_, stemlength), true));
                                    Prev_list.Add(new temp(beam.Key, beam.Value, new Point(x_, stemlength), true));
                                    // PreviousBeamPos.Add(new Point(x + xoff, stemlength));
                                    break;
                                case 2: //Beam.Beam_type.stop:
                                    //Prev_list.Add(new temp(n.ElementAt(i).Beam.NoteBeamList.ElementAt(b).Key, n.ElementAt(i).Beam.NoteBeamList.ElementAt(b).Value, new Point(x_, stemlength), false));
                                    Prev_list.Add(new temp(beam.Key, beam.Value, new Point(x_, stemlength), false));
                                    break;
                            }

                            var m = Prev_list.Select(item => item.id).Distinct().ToList();
                            bool e = false;
                            foreach (var dist in m)
                            {
                                //float padding = n.ElementAt(i).Stem_dir ? dist * -7 : -(dist * -7);
                                if (Prev_list.Exists(u => u.o == 4))
                                {
                                    //float x_p = n.ElementAt(i - 1).PosX + measx;
                                    //float stemlength_p = n.ElementAt(i - 1).Stem_dir ? measy + 5 - n.ElementAt(i - 1).Stem : measy + 12 - n.ElementAt(i - 1).Stem;
                                    //float x__p = x_p + xoff;
                                    //float tem = stemlength - (0.25f * (stemlength - stemlength_p));
                                    //hook = x_ - (0.25f * (x_ - x__p));
                                    //DrawBeam(dc, new Point(hook, padding + tem), new Point(x_, stemlength + padding), n.ElementAt(i).Stem_dir);
                                    Prev_list.RemoveAll(it => it.id == dist);
                                }
                                if (Prev_list.Exists(u => u.o == 3))
                                {
                                    //float x_p = n.ElementAt(i - 1).PosX + measx;
                                    //float stemlength_p = n.ElementAt(i + 1).Stem_dir ? measy + 5 - n.ElementAt(i + 1).Stem : measy + 12 - n.ElementAt(i + 1).Stem;
                                    //float x__p = x_p + xoff;
                                    //float tem = stemlength - (0.25f * (stemlength - stemlength_p));
                                    //hook = x_ - (0.25f * (x_ - x__p));
                                    //DrawBeam(dc, new Point(x_, stemlength + padding + tem), new Point(hook, padding + tem), n.ElementAt(i).Stem_dir);
                                    Prev_list.RemoveAll(it => it.id == dist);
                                }
                                var plist = Prev_list.Select(item => item).Where(item => item.id == dist).ToList();
                                if (plist.Count != 1 && plist.Exists(it => it.o == 2) && plist.Exists(ix => ix.o == 2))
                                {
                                    //if (n.ElementAt(i).SymbolType == MusSymbolDuration.Sixteen) ;
                                    //float padding = n.ElementAt(i).Stem_dir ? dist * -7 : -(dist * -7);
                                    //var plist = Prev_list.Select(item => item).Where(item => item.id == dist); ///var plist = Prev_list.Select(item => item).Where(item => item.id == dist);

                                    for (int z = plist.Count() - 1; e == false; z--)
                                    {
                                        int di = plist.ElementAt(z).id;
                                        float padding = n.ElementAt(i).Stem_dir ? di * -7 : -(di * -7);
                                        Draw_Beam(dc, new Point(plist.ElementAt(z - 1).p.X, plist.ElementAt(z - 1).p.Y + padding), new Point(plist.ElementAt(z).p.X, plist.ElementAt(z).p.Y + padding), n.ElementAt(i).Stem_dir);
                                        // if(Prev_list.ElementAt(z).id)
                                        if (plist.ElementAt(z - 1).o == 0)
                                        {
                                            e = true;
                                            plist.RemoveAll(item => item.id == dist);
                                            Prev_list.RemoveAll(item => item.id == dist);
                                        }

                                    }
                                }
                            }
                        }
                        if (n.ElementAt(i).HasNotations) // check if element has additional notations
                        {
                            float slurYoffset = n.ElementAt(i).Stem_dir ? -7f : 7f;   // y offset related to note dot
                            float slurXoffset = n.ElementAt(i).Stem_dir ? 5f : 5f;    // x offset related to note dot
                            var s = n.ElementAt(i).NotationsList.Where(z => z is Slur).Select(z => (Slur)z).ToList(); // list of Slurs
                            float note_X = x + slurXoffset;  //x pos with offset
                            float note_Y = yl + slurYoffset; //y pos with offset
                            foreach (var item in s) //loop on slurs to add each to list
                            {

                                int operation = (int)item.Type;    //0 -start, 1- next, 2 - stop
                                bool b = operation == 0 ? true : false; // is start
                                Notations_list.Add(new temp(item.Level, operation, new Point(note_X, note_Y), b));
                            }
                            var ids = Notations_list.Select(xx => xx.id).Distinct().ToList(); // get id's of slurs
                            foreach (var idn in ids) // loop on each id of slur
                            {
                                var nlist = Notations_list.Select(cc => cc).Where(cc => cc.id == idn).ToList(); // list of slurs with given id
                                for (int nlx = 0; nlx < nlist.Count; nlx++)
                                {
                                    if (nlist.ElementAt(nlx).o == 2)  // loop until slur.o == 2 - Stop  -- last on list
                                    {
                                        Draw_Slur(surface, nlist.ElementAt(nlx - 1).p, nlist.ElementAt(nlx).p, n.ElementAt(i).Stem_dir); // draw
                                        Notations_list.RemoveAll(zz => zz.id == idn); // delete slurs with this.id

                                    }

                                }
                            }
                        }  
                    }
                     else
                         {
                             DrawString(dc, n.ElementAt(i).Symbol, TypeFaces.NotesFont, Brushes.Black, new Point(x, y), Measures.Scale);
                        //DrawString(dc, "-", TypeFaces.TextFont, Brushes.Yellow, new Point(x, y), Measures.Scale);
                    }
                    //  DrawString(dc, n.ElementAt(i).Symbol, TypeFaces.NotesFont, Brushes.Black, new Point(x, y), Measures.Scale);
                    //AdditionalLine(dc, new Point(x, y));
#if ASD
                        DrawString(dc, x.ToString() + "\n" + y.ToString(), TypeFaces.NotesFont, Brushes.Black, new Point(x, y + 40), Measures.Scale / 4);
#endif
                }
            surface.AddVisual(visual);
            }
        }

        public void AddRests() // need to be reworked // now works only if rest has given position in measure // have to add method to calculate if pos is not given
        {
            Rest.ExtractRests();
            int num = Rest.RestList.Count;
            for (int i = 0; i < num; i++)
            {
                int m_id = Rest.RestList.ElementAt(i).MeasureId;
                float r_pos = Rest.RestList.ElementAt(i).X;
                string symbol = Rest.RestList.ElementAt(i).Symbol;
                Point p = Measures.MeasureList.ElementAt(m_id-1).Start;
                DrawingVisual visual = new DrawingVisual();
                float x = (float)p.X + r_pos;
                using (DrawingContext dc = visual.RenderOpen())
                {
                    DrawString(dc, symbol, TypeFaces.NotesFont, Brushes.Black, new Point(x,p.Y), Measures.Scale);
                    if (Rest.RestList.ElementAt(i).HasDot)
                        DrawString(dc, ".", TypeFaces.NotesFont, Brushes.Black, new Point(x +12, p.Y -16),Measures.Scale);
#if ASD
                    DrawString(dc, p.X.ToString() + "\n" + p.Y.ToString(), TypeFaces.NotesFont, Brushes.Black, new Point(x, p.Y + 35), Measures.Scale / 4);
#endif
                }
                Surface.AddVisual(visual);
            }
        }

        public void AddTimeSig(DrawingVisual visual)
        {
            var t = x.OfType<TimeSignature>();
            if (t.Count() != 0)
            {
                string b = t.ElementAt(0).BeatStr;
                string b_t = t.ElementAt(0).BeatTypeStr;
                int num = t.ElementAt(0).MeasureNum;
                using (DrawingContext dc = visual.RenderOpen())
                {
                    Draw_TimeSig(dc, b, b_t, Measures.getCoordsOfMeasure(num), Measures.Scale);
                }
            }
        }
        public static void Draw_Beam(DrawingContext d,Point s,Point e,bool upsidedown)
        {
            Pen pen = new Pen(Brushes.Black,2);             //
            float offset = upsidedown? 3f: -3f;             //   draw rectangle between stems
            Point s2 = new Point(s.X, s.Y +offset);         //
            Point e2 = new Point(e.X, e.Y +offset);         //
            
            StreamGeometry sg = new StreamGeometry();
            using (StreamGeometryContext sgc = sg.Open())
            {
                sgc.BeginFigure(s, true, true);
                PointCollection points = new PointCollection { e, e2, s2 };
                sgc.PolyLineTo(points, true, true);
            }
            sg.Freeze();
            d.DrawGeometry(Brushes.Black, pen, sg);
        }

        public static void Draw_Slur(CanvasList surface,Point p1, Point p2, bool direction, bool isDash = false)
        {
            sbyte dir = direction ? (sbyte) 1 : (sbyte) -1; // set direction of slur
            Pen pen = new Pen(Brushes.Black, 1);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                StreamGeometry sg = new StreamGeometry();
                using (StreamGeometryContext sgc = sg.Open())
                {
                    sbyte offset = 5; // set offset of bezier according to notes // shape of curve
                    float distance = Calc.Distance(p1, p2);
                    Point outerNode = Calc.PerpendicularOffset(p1, p2, (dir * distance) / (offset * 0.6f)); // calculate pos of outer bezier steering node
                    Point innerNode = Calc.PerpendicularOffset(p1, p2, (dir * distance) / (offset * 0.75f)); // calculate pos of inner bezier steering node
                    if (isDash) // if dashed slur  - - - - -
                    {
                        pen.Thickness = 5;
                        pen.DashStyle = DashStyles.Dash;
                        sgc.BeginFigure(p1, false, true);
                        sgc.QuadraticBezierTo(innerNode, p2, true, true);
                    }
                    else // if regular 
                    {
                        sgc.BeginFigure(p1, true, true);
                        sgc.QuadraticBezierTo(outerNode, p2, true, true);
                        sgc.QuadraticBezierTo(innerNode, p1, true, true);
                    }
                    
                }
                sg.Freeze(); // freez geometry stream
                dc.DrawGeometry(Brushes.Black, pen, sg); // draw on drawing context
            }
            surface.AddVisual(visual); // add to canvas list
        }
        public static void AdditionalLine (DrawingContext d, Point p)
        {
            Pen pen = new Pen(Brushes.Black,1);
            
            d.DrawLine(pen, new Point(p.X - 4, p.Y + 32), new Point(p.X + 15, p.Y + 32)); // draw additional line
        }
        public static void DrawString(DrawingContext d, string text, Typeface f, Brush b, float xPos, float yPos, float emSize)
        {

            d.DrawText(new FormattedText(text, Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, f, emSize, b), new Point(xPos, yPos));

        }
        //public static void DrawStringR(DrawingContext d, string text, Typeface f, Brush b, Point p, float emSize )
        //{
        //   // d.PushTransform(new RotateTransform(180,p.X+6.5,p.Y+32));
        //    d.DrawText(new FormattedText(text, Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight, f, emSize, b), p);
        //  //  d.Pop();
        //}

        public static void DrawString(DrawingContext d, string text, Typeface f, Brush b, Point p, float emSize)
        {
            DrawString(d, text, f, b, (float)p.X, (float)p.Y, emSize);
        }

        public static void Draw_TimeSig(DrawingContext d, string b,string b_t, Point p, float emSize)
        {
            DrawString( d, b , TypeFaces.TimeNumbers, Brushes.Black, (float)p.X+50, (float)p.Y, emSize); // draw upper number of time sign
            DrawString( d, b_t, TypeFaces.TimeNumbers, Brushes.Black, (float)p.X +50, (float)p.Y, emSize); // draw lower numbuer of time sign
        }
        
        public static void Draw_Key(DrawingContext dc, Point p , float size, ClefType sign, int num =1)
        {
           // num = 4;// test
            
            if (num == 0)
            {
                // do nothing if key is sharp/flat-less
            }
            else
            {
                int alt = sign.Sign == ClefType.Clef.GClef ? -16 : sign.Sign == ClefType.Clef.CClef ? -12 : -8; // 0= Gclef 4= Cclef 8= Fclef
                bool isSharp = num > 0 ? true : false;  //check if sharp or flat key
                float x = (float)p.X; // x pos of measure
                float y = (float)p.Y; // y pos of measure
                float[] sharp = new float[] { 2, 12, -2, 8, 20, 4, 16 }; // y pos of each sharp symbol
                float[] flat = new float[] { 16, 4, 20, 8, 24, 12, 28 }; // y pos of each flat symbol
                int padding = isSharp ? 8 : 6; // different padding // difference in width of symbol
                float[] test = isSharp ? sharp : flat; // assign table o possitions
                string key = isSharp ? MusChar.Sharp : MusChar.Flat; // assign unicode symbol
                for (int i = 0; i < Math.Abs(num); i++)
                {
                    DrawString(dc, key, TypeFaces.NotesFont, Brushes.Black, x + 25 + padding * i, y + (test[i] + alt), size ); // draw
                }
            }
        }
    }
}
