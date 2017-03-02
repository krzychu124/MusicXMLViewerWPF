using MusicXMLScore.Model;
using MusicXMLScore.Model.Defaults;
using MusicXMLScore.ViewModel;
using MusicXMLViewerWPF;
using MusicXMLViewerWPF.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.Helpers
{
    [Serializable]
    public class PageProperties : ISerializable
    {
        private string id;
        private PageDimensions pageDimensions;
        private List<PageMarginsMusicXML> pageMargins;
        private SystemLayoutMusicXML systemLayout;
        private List<StaffLayoutMusicXML> staffLayout = new List<StaffLayoutMusicXML>() { new StaffLayoutMusicXML() };
        private static double DPI = 144; // def 96, may be changed in the future
        private double scale = 40; // in tenths
        private double staffHeight = 7.0556; // in mm
        private double staffSpace = 1.764; // staffheight / 4
        private double default_width = 1195.62; // 210mm in tenths 
        private double default_height = 1683.67;  // 297mm in tenths
        private double converterFactor = 0.1764; // staffheight / scale
        private PageType pagetype = PageType.A4;
        private PageOrientation pageOrientation = PageOrientation.portait;

        public double Scale
        {
            get
            {
                return scale;
            }
        }

        public double StaffHeight
        {
            get
            {
                return staffHeight;
            }
        }
        
        internal PageDimensions PageDimensions
        {
            get
            {
                return pageDimensions;
            }
        }

        public double ConverterFactor
        {
            get
            {
                return converterFactor;
            }
        }

        public double StaffSpace
        {
            get
            {
                return staffSpace;
            }
        }

        public List<PageMarginsMusicXML> PageMargins
        {
            get
            {
                return pageMargins;
            }

            set
            {
                pageMargins = value;
            }
        }

        public PageProperties()
        {
            CalculateConverterFactor();
            CalculatePageDimensions();
        }
        internal PageProperties(MusicScore ms)
        {
           this.scale = ms.Defaults.Scale.Tenths;
            this.staffHeight = ms.Defaults.Scale.Millimeters;
            default_width = ms.Defaults.Page.Width;
            default_height = ms.Defaults.Page.Height;
            SetOrientation();
            CalculateConverterFactor();
            CalculateStaffSpace();
            CalculatePageDimensions();
        }
        public PageProperties(double scale, double staffLineHeight, double widthTenths, double heightTenths)
        {
            this.scale = scale;
            this.staffHeight = staffLineHeight;
            default_width = widthTenths;
            default_height = heightTenths;
            SetOrientation();
            CalculateConverterFactor();
            CalculateStaffSpace();
            CalculatePageDimensions();
        }
        public PageProperties(SerializationInfo info, StreamingContext context)
        {
            scale = (double)info.GetValue("Scale", typeof(double));
            staffHeight = (double)info.GetValue("StaffHeight", typeof(double));
            default_width = (double)info.GetValue("Width", typeof(double));
            default_height = (double)info.GetValue("Height", typeof(double));
            Enum.TryParse((string)info.GetValue("PageFormat", typeof(string)), out pagetype);
            Enum.TryParse((string)info.GetValue("PageOrientation", typeof(string)), out pageOrientation);

            CalculateConverterFactor();
            CalculatePageDimensions();
        }

        public PageProperties(DefaultsMusicXML defaults)
        {
            if (defaults == null)
            {
                CalculateConverterFactor();
                CalculatePageDimensions();
            }
            else
            {
                staffHeight = defaults.Scaling.Millimeters;
                scale = defaults.Scaling.Tenths;
                CalculateStaffSpace();
                CalculateConverterFactor();
                SetPageDimensions(defaults.PageLayout.PageWidth, defaults.PageLayout.PageHeight);
                pageMargins = defaults.PageLayout.PageMargins;
                systemLayout = defaults.SystemLayout;
                staffLayout = new List<StaffLayoutMusicXML>(defaults.StaffLayout){ };
            }
        }

        public void SetPageDimensions(double width, double height)
        {
            SetOrientation(width, height);
            pageDimensions = new PageDimensions(width, height, converterFactor);
        }

        private void SetOrientation(double width = 0, double height = 0)
        {
            double tempWidth = 0;
            double tempHeight = 0;

            if (width == 0 || height == 0)
            {
                tempWidth = default_width;
                tempHeight = default_height;
            }
            else
            {
                tempWidth = width;
                tempHeight = height;
            }

            if (tempWidth > tempHeight)
            {
                pageOrientation = PageOrientation.landscape;
            }
            else
            {
                pageOrientation = PageOrientation.portait;
            }
        }

        public void SwitchOrientation()
        {
            if (pageOrientation == PageOrientation.portait)
            {
                pageOrientation = PageOrientation.landscape;
                SwitchDimensions();
                CalculatePageDimensions();
            }
            else
            {
                pageOrientation = PageOrientation.portait;
                SwitchDimensions();
                CalculatePageDimensions();
            }
        }
        private void SwitchDimensions()
        {
            double tempWidth = default_width;
            default_width = default_height;
            default_height = tempWidth;
        }
        private void CalculateConverterFactor()
        {
            converterFactor = staffHeight / scale;
        }
        private void CalculateStaffSpace()
        {
            staffSpace = staffHeight / (scale / 10);
        }
        private void CalculatePageDimensions()
        {
            pageDimensions = new PageDimensions(default_width, default_height, converterFactor);
        }
        public static double PxPerMM()
        {
            return ScaleExtensions.PxPerMM(); //? return DPI / 25.4;
        }
        public double TenthToPx(double tenths)
        {
            return tenths * converterFactor * PxPerMM();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Scale", Scale, typeof(double));
            info.AddValue("StaffHeight", staffHeight, typeof(double));
            info.AddValue("Width", default_width, typeof(double));
            info.AddValue("Height", default_height, typeof(double));
            info.AddValue("PageFormat", pagetype.ToString());
            info.AddValue("PageOrientation", pageOrientation.ToString());
        }
    }
    class PageDimensions
    {
        double width; // in tenths
        double height;
        double converterFactor;
        public readonly double staticFactor = 36;
        public PageDimensions(double width, double height, double converterFactor)
        {
            this.width = width;
            this.height = height;
            this.converterFactor = converterFactor;
        }
        public Point GetPageDimensionsInMM()
        {
            return new Point(width * converterFactor, height * converterFactor);
        }
        public Point GetPageDimensionsInTenths()
        {
            return new Point(width, height);
        }
        public Point GetPageDimensionsInInches()
        {
            return new Point((width / 25.4) * converterFactor, (height / 25.4) * converterFactor);
        }
        public Point GetPageDimensionsInPx()
        {
            Point temp = GetPageDimensionsInMM();
            double pxpermm = PageProperties.PxPerMM();
            return new Point(temp.X * pxpermm, temp.Y * pxpermm);
        }
        public Size Dimensions
        {
            get
            {
                return new Size(width, height);
            }
        }
    }

    public static class ScaleExtensions
    {
        public static double PxPerMM()
        {
            return 144 / 25.4;
        }
        public static double TenthsToMM(double tenths)
        {
            double converterFactor = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.ConverterFactor;
            double result = tenths * converterFactor;
            return result;
        }

        public static double MMToTenths(double MM)
        {
            double converterFactor = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.ConverterFactor;
            if (converterFactor == 0)
            {
                return 0.0;
            }
            return MM / converterFactor;
        }

        public static double TenthsToWPFUnit(double tenths)
        {
            double converterFactor = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.ConverterFactor;
            double result = tenths * converterFactor * PxPerMM();
            return result;
        }

        public static double WPFUnitToTenths(double WPFUnit)
        {
            if (WPFUnit == 0)
            {
                return 0.0;
            }
            double converterFactor = ViewModelLocator.Instance.Main.CurrentTabLayout.PageProperties.ConverterFactor;
            return WPFUnit / (converterFactor * PxPerMM());
        }
        public static double WPFUnitToMM(double WPFUnit)
        {
            return WPFUnit * PxPerMM(); //! to test
        }
        public static double MMToWPFUnit(double MM)
        {
            return MM / PxPerMM(); //! to test
        }
        public static double MMToInch(double MM)
        {
            return MM / 25.4;
        }
    }
    enum PageType
    {
        A4,
        A3,
        Custom
    }
    enum PageOrientation
    {
        portait,
        landscape,
    }
}
