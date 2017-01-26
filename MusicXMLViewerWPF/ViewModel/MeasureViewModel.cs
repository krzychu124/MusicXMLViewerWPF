﻿using MusicXMLScore.Helpers;
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
    class MeasureViewModel //TODO_I implement collection of items placed in this measure (to draw inside CustomPanel)
    {
        private Measure measure;
        private StaffLineCanvas staffLineCanvas;
        private ObservableCollection<UIElement> measureContent;

        public Measure Measure { get { return measure; } }

        public StaffLineCanvas MeasureStaffLine { get { return staffLineCanvas; }  set { staffLineCanvas = value; } }

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

        public MeasureViewModel()
        {
            MeasureStaffLine = new StaffLineCanvas() { Width = 200 }; // Prototype test only (basic staff with 150px width)
            measure = new Measure(200);
            MeasureContent = new ObservableCollection<UIElement>();
            //MeasureContent.Add(measure.NotesList.ElementAt(0).DrawableMusicalObject);
            BuildAndDrawMeasure();
        }
        public MeasureViewModel(Measure measure)
        {
            this.measure = measure;
            BuildAndDrawMeasure();
        }
        private void BuildAndDrawMeasure()
        {
            foreach (var item in measure.NotesList)
            {
                if (item.DrawableObjectStatus == MusicXMLViewerWPF.Misc.DrawableMusicalObjectStatus.ready)
                MeasureContent.Add(item.DrawableMusicalObject);
            }
        }
    }
}
