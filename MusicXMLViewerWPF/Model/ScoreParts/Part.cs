using MusicXMLViewerWPF.ScoreParts.MeasureContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Part
    {

        #region Private Fields
        private List<Measure> measureList = new List<Measure>();
        private string part_id;

        #endregion Private Fields

        #region Public Constructors

        public Part(XElement x)
        {
            PartId = x.Attribute("id").Value;
            var measures = x.Elements();
            for (int i = 0; i < measures.Count(); i++)
            {
                XElement item = measures.ElementAt(i);
                MeasureList.Add(new Measure(item));
            }
        }
        public Part(string id)
        {
            PartId = id;
        }

        #endregion Public Constructors

        #region Public Properties

        public List<Measure> MeasureList { get { return measureList; } private set { measureList = value; } }
        public string PartId { get { return part_id; } private set { part_id = value; } }

        #endregion Public Properties

        #region Public Methods

        public void AddMeasure(Measure measure)
        {
            MeasureList.Add(measure); //Todo add info about firt measure number in part
        }

        #endregion Public Methods

        //private void SortNotations() //TODO_l improve, add drawing
        //{
        //    foreach (var item in MeasureSegmentList)
        //    {
        //        foreach (var note in item.NotesList)
        //        {
        //            if (note.NotationsList != null)
        //            {
        //                if (nlist == null) nlist = new List<List<Notations>>();
        //                nlist.Add(note.NotationsList);
        //            }
        //        }
        //    }
        //    if (nlist != null)
        //    {
        //        slist = new List<List<Slur>>();
        //        List<Slur> slur = new List<Slur>();
        //        for (int i = 0; i < nlist.Count; i++)
        //        {
        //            var item = nlist.ElementAt(i);
        //            foreach (Slur ele in item)
        //            {
        //                slur.Add(ele);
        //            }
        //        }
        //        var nums = slur.Select(u => u.Level).Distinct().ToList();
        //        foreach (var item in nums)
        //        {
        //            var c = slur.Select(o => o).Where(o => o.Level == item);
        //            List<Slur> tmp = new List<Slur>();
        //            foreach (var it in c)
        //            {
        //                if (it.Type == Slur.SlurType.start)
        //                {
        //                    tmp.Add(it);
        //                }
        //                if (it.Type == Slur.SlurType.stop)
        //                {
        //                    tmp.Add(it);
        //                    slist.Add(new List<Slur>(tmp));
        //                    tmp.Clear();
        //                }
        //            }
        //        }
        //    }
        //}
        //public void DrawMeasures(CanvasL surface)
        //{
        //    Point start = new Point();
        //    if (measure_margin_helper.Count != 0)
        //    {
        //        start.X = measure_margin_helper.ElementAt(0).Value.X;
        //        start.Y = measure_margin_helper.ElementAt(0).Value.Y;
        //    }

        //    //Point current = new Point();
        //    foreach ( var measure in measure_list)
        //    {
        //        if (measure_margin_helper.ContainsKey(measure.Number))
        //        {
        //            start = measure_margin_helper[measure.Number];
        //           // MusicScore.AddBreak( MusicScore.Defaults.Page.Width - (float)start.X, (float)start.Y, "line");
        //        }
        //        //if (measure.PrintProperties != null)
        //        //{
        //        //    if (measure.PrintProperties.NewPage)
        //        //    {
        //        //        start.X = current.X;
        //        //        start.Y = current.Y;
        //        //    }
        //        //}
        //        measure.Draw(surface, start);
        //        start.X += measure.Width;

        //    }
        //}
    }
}
