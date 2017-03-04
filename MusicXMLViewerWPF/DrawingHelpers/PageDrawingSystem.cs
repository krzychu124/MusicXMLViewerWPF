using MusicXMLScore.Converters;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.Defaults;
using MusicXMLScore.Model.MeasureItems;
using MusicXMLScore.ViewModel;
using MusicXMLViewerWPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.DrawingHelpers
{
    class PageDrawingSystem
    {
        private CanvasList pageDrawingSystemCanvas;
        private double leftMargin;
        LayoutControl.LayoutGeneral pageLayout;
        List<string> partsIdToDraw;
        ScorePartwiseMusicXML score;
        List<string> firstMeasureIdPerSystem;
        Dictionary<string, double> partStafDistances;
        public CanvasList PageDrawingSystemCanvas
        {
            get
            {
                bool isNull = PageDrawingSystemCanvas == null ? true : false;
                if (isNull)
                {
                    Log.LoggIt.Log("Null PageDrawinSystemCanvas !!", Log.LogType.Warning); // temp
                    return new CanvasList(5, 5);
                }
                else
                {
                    return pageDrawingSystemCanvas;
                }
            }
            set
            {
                PageDrawingSystemCanvas = value;
            }
        }

        public double LeftMargin
        {
            get
            {
                return leftMargin;
            }

            set
            {
                leftMargin = value;
            }
        }

        public PageDrawingSystem(LayoutControl.LayoutGeneral layout, ScorePartwiseMusicXML score)
        {
            pageLayout = layout;
            this.score = score;
        }

        private void PrepareSystemProperties()
        {
            CalculateSystemDimensions();
        }

        private void CalculateSystemDimensions()
        {
            CalculateSystemWidth();
            CalculateSystemHeight();
        }

        private void CalculateSystemHeight()
        {
            double finalHeight = 0.0;
            foreach (var item in firstMeasureIdPerSystem)
            {
                foreach (var part in partsIdToDraw)
                {

                }
            }
        }

        private void CalculateSystemWidth()
        {
            throw new NotImplementedException();
        }

        public void AddPartsIdToDraw(List<string> partsId)
        {
            partsIdToDraw = partsId;
        }
        public void MeasureIdList(List<string> measureId)
        {
            firstMeasureIdPerSystem = measureId;
        }
        private void AddToCanvas(CanvasList canvas)
        {
            if(PageDrawingSystemCanvas == null)
            {
                PageDrawingSystemCanvas = new CanvasList(1,1);
                PageDrawingSystemCanvas.ClipToBounds = false;
            }
        }
    }
}
