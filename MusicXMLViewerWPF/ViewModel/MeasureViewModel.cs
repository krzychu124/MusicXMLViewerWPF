using MusicXMLScore.Helpers;
using MusicXMLViewerWPF.ScoreParts.MeasureContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.ViewModel
{
    /// <summary>
    /// Displays default or passed to ctor. Measure object
    /// </summary>
    class MeasureViewModel //TODO_I implement collection of items placed in this measure (to draw inside CustomPanel)
    {
        private Measure measure;
        private StaffLineCanvas staffLineCanvas;

        public Measure Measure { get { return measure; } }

        public StaffLineCanvas MeasureContent { get { return staffLineCanvas; }  set { staffLineCanvas = value; } }

        public MeasureViewModel()
        {
            MeasureContent = new StaffLineCanvas() { Width = 150 }; // Prototype test only (basic staff with 150px width)
        }
        public MeasureViewModel(Measure measure)
        {
            this.measure = measure;
        }
    }
}
