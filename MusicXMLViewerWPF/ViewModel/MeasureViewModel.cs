using GalaSoft.MvvmLight;
using MusicXMLScore.Helpers;
using MusicXMLViewerWPF.ScoreParts.MeasureContent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Displays default or passed to ctor. Measure object
    /// </summary>
    class MeasureViewModel : ViewModelBase //TODO_I implement collection of items placed in this measure (to draw inside CustomPanel)
    {
        #region Fields
        private Measure measure;
        private ObservableCollection<UIElement> measureContent;
        private double measureWidth;
        private StaffLineCanvas staffLineCanvas;
        #endregion
        public MeasureViewModel()
        {
            //MeasureStaffLine = new StaffLineCanvas() { Width = 200 }; // Prototype test only (basic staff with 150px width)
            //measure = new Measure(200);
            MeasureContent = new ObservableCollection<UIElement>();
            //BuildAndDrawMeasure();
        }

        public MeasureViewModel(Measure measure)
        {
            MeasureContent = new ObservableCollection<UIElement>();
            this.measure = measure;
            MeasureWidth = Measure.Width;
            BuildAndDrawMeasure();
        }

        public Measure Measure { get { return measure; } }
        public ObservableCollection<UIElement> MeasureContent
        {
            get
            {
                return measureContent;
            }
            set
            {
                measureContent = value;
            }
        }
        #region Properties
        public StaffLineCanvas MeasureStaffLine { get { return staffLineCanvas; } set { staffLineCanvas = value; } }
        public double MeasureWidth { get { return measureWidth; } set { measureWidth = value; } }
        #endregion
        private void BuildAndDrawMeasure() //TODO_I implement drawing to rest of drawable objects
        {
            

            //if (Measure.Attributes != null)
            //{
            //    if (Measure.Attributes.Clef != null)
            //    {
            //        MeasureContent.Add(Measure.Attributes.Clef.DrawableMusicalObject);
            //    }
            //    if (Measure.Attributes.Key != null)
            //    {
            //        MeasureContent.Add(Measure.Attributes.Key.DrawableMusicalObject);
            //    }
            //    if (Measure.Attributes.Time != null)
            //    {
            //        MeasureContent.Add(Measure.Attributes.Time.DrawableMusicalObject);
            //    }
            //}

            MusicXMLScore.DrawingHelpers.MeasureDrawing dm = new MusicXMLScore.DrawingHelpers.MeasureDrawing(Measure);
            MeasureContent.Add(dm.BaseObjectVisual);
            //foreach (var item in Measure.NotesList)
            //{
            //    if (item.DrawableObjectStatus == MusicXMLViewerWPF.Misc.DrawableMusicalObjectStatus.ready)
            //        MeasureContent.Add(item.DrawableMusicalObject);
            //}
            //foreach (var item in Measure.Barlines)
            //{
            //    MeasureContent.Add(item.DrawableMusicalObject);
            //}
        }
    }
}
