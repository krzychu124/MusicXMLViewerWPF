using MusicXMLViewerWPF;
using MusicXMLViewerWPF.ScoreParts.MeasureContent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace MusicXMLScore.Helpers
{
    class DrawableMeasure : CustomCanvas
    {
        private bool drawbounds = true; //! needs rework to dependency
        private Measure m;
        private CanvasList above_staffline = new CanvasList();
        private CanvasList below_Staffline = new CanvasList();
        private ObservableCollection<CanvasList> lyricslist = new ObservableCollection<CanvasList>();
        private CanvasList staffline = new CanvasList();
        private CustomSystemWrapPanel measure_content = new CustomSystemWrapPanel() { Orientation = Orientation.Horizontal };
        private double measurestaffYmulti = 0; // 0.18;
        private double directionsabovemulti = 0.05;
        private double directionsbelowmulti = 0.7;
        private double lyricsmulti = 0.85;
        public DrawableMeasure(Measure m)
        {
            this.SizeChanged += CustomSystemPanel_SizeChanged;
            this.m = m;
            this.Width = m.Width;
            this.Height = 60;
            //AddMeasure(s);
            GenerateStaffLine(staffline); //! Draws staffline of measure
            Children.Add(staffline);
            //AddFewObjects();
            measure_content.VerticalAlignment = VerticalAlignment.Top;
            AddNotes(measure_content);
            Children.Add(measure_content);
            FillMeasure(); //! no usage for now
            this.Measure(new Size());
        }

        private void FillMeasure()
        {
            //throw new NotImplementedException();
        }

        private void CustomSystemPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width != e.NewSize.Width)
            {
                GenerateStaffLine(staffline); //! Updates staffline width
                if (drawbounds)
                {
                    if (FindVisualByTag("bounds") != null)
                    {
                        DeleteVisual(FindVisualByTag("bounds"));
                    }
                    DrawingVisualPlus db = new DrawingVisualPlus();
                    db.Tag = "bounds";
                    //CustomSystemPanel csp = e.Source as CustomSystemPanel;
                    Rect r = new Rect(0, 0, measure_content.DesiredSize.Width, measure_content.DesiredSize.Height);
                    MusicXMLViewerWPF.Misc.DrawingHelpers.DrawRectangle(db, r);
                    CanvasList cl = new CanvasList() { Width = r.Width, Height = r.Height };
                    cl.AddVisual(db);
                    Children.Add(cl);
                }
                else
                {
                    if (FindVisualByTag("bounds") != null)
                    {
                        DeleteVisual(FindVisualByTag("bounds"));
                    }
                }
            }
        }
        string s = @"<measure number=""2"" width=""203"">
        <print new-system=""no""/>
        <note>
        <rest measure=""yes""/>
        <duration>12</duration>
        <voice>1</voice>
      </note></measure>";

        private void AddNotes(Panel sp) //TODO_I refactoring
        {
            if (m.Attributes != null)
            {
                if (m.Attributes.Clef != null)
                {
                    //sp.Children.Add(m.Attributes.Clef.DrawableMusicalObject);
                }
                if (m.Attributes.Key != null)
                {
                    sp.Children.Add(m.Attributes.Key.DrawableMusicalObject);
                }
                if (m.Attributes.Time != null)
                {
                    sp.Children.Add(m.Attributes.Time.DrawableMusicalObject);
                }
            }
            foreach (var item in m.NotesList)
            {
                sp.Children.Add(item.DrawableMusicalObject);
            }
        }

        private void AddFewObjects()
        {
            m.AddBarline();
            m.AddClef(new ClefType(ClefType.Clef.GClef));
            m.AddKeySig();
            m.AddNote(new Pitch("B", 4), 16);
            m.AddNote(new Pitch("F", 4), 16);
            m.AddRest("", MusSymbolDuration.Eight);
            m.AddNote(new Pitch("B", 4), 16);
            m.AddNote(new Pitch("c", 4), 16);
            m.AddTimeSig(4,4);
        }

        private void AddMeasure(string s)
        {
            XElement xel = XElement.Parse(s);
            m = new Measure(xel);
            this.Width = m.Width;
            this.Height = 60;
        }
        /// <summary>
        /// Measure staff generator for drawing
        /// </summary>
        /// <param name="cl"></param>
        private void GenerateStaffLine(CanvasList cl)
        {
            if (cl.FindVisualByTag("staffline") != null)
            {
                cl.DeleteVisual(cl.FindVisualByTag("staffline"));
            }
            double w = ActualWidth;
            //Measure m = new MusicXMLViewerWPF.ScoreParts.Part.Measures.Measure(w);
            DrawingVisualPlus dv = new DrawingVisualPlus() { Tag = "staffline" };
            m.Draw_Measure(dv, new Point(0, measurestaffYmulti* ActualHeight));
            cl.Width = ActualWidth;
            cl.Height = ActualHeight;
            cl.AddVisual(dv);
        }

    }
}
