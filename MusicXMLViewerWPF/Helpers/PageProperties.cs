using MusicXMLScore.Converters;
using MusicXMLScore.Model;
using MusicXMLScore.Model.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.Helpers
{
    [Serializable]
    public class PageProperties : ISerializable
    {
        private string id;
        private PageDimensions pageDimensions;
        private List<PageMarginsMusicXML> loadedPageMargins;
        private PageMarginsMusicXML pageMarginEven;
        private PageMarginsMusicXML pageMarginOdd;
        private PageMarginsMusicXML pageMarginBoth;
        private SystemLayoutMusicXML systemLayout;
        private List<StaffLayoutMusicXML> staffLayout = new List<StaffLayoutMusicXML>() { new StaffLayoutMusicXML() };
        private static double DPI = 96;//144; // def 96, may be changed in the future
        private double scale = 40; // in tenths
        private double staffHeight = 7.0556; // in mm
        private double staffSpace = 1.764; // staffheight / 4
        private double default_width = 1195.62; // 210mm in tenths 
        private double default_height = 1683.67;  // 297mm in tenths
        private double converterFactor = 0.1764; // staffheight / scale
        private PageType pagetype = PageType.A4;
        private PageOrientation pageOrientation = PageOrientation.portait;
        private Dictionary<int, double> avaliableIndexLinePositions = new Dictionary<int, double>();
        private Dictionary<int, double> staffLineCoords;
        public double Scale
        {
            get
            {
                return scale;
            }
        }
        /// <summary>
        /// Height of staff in millimeters, use converter extension method: MMToWPFUnit() when used for drawing 
        /// </summary>
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
        /// <summary>
        /// Space between staff lines in MM, convert to WPFUnit to use for coordinates or lenght in canvas
        /// </summary>
        public double StaffSpace
        {
            get
            {
                return staffSpace;
            }
        }

        //public List<PageMarginsMusicXML> PageMargins
        //{
        //    get
        //    {
        //        return pageMargins;
        //    }

        //    set
        //    {
        //        pageMargins = value;
        //    }
        //}

        public PageMarginsMusicXML PageMarginEven
        {
            get
            {
                return pageMarginEven ?? PageMarginBoth;
            }
        }

        public PageMarginsMusicXML PageMarginOdd
        {
            get
            {
                return pageMarginOdd ?? PageMarginBoth;
            }
        }

        public PageMarginsMusicXML PageMarginBoth
        {
            get
            {
                return pageMarginBoth ?? GetDefaultMargins();
            }
        }

        public Dictionary<int, double> IndexStaffLinePositions
        {
            get
            {
                return avaliableIndexLinePositions;
            }

            set
            {
                avaliableIndexLinePositions = value;
            }
        }

        public Dictionary<int, double> StaffLineCoords
        {
            get
            {
                return staffLineCoords;
            }

            set
            {
                staffLineCoords = value;
            }
        }

        public Dictionary<int, double> AvaliableIndexLinePositions
        {
            get
            {
                return avaliableIndexLinePositions;
            }

            set
            {
                avaliableIndexLinePositions = value;
            }
        }

        public PageProperties()
        {
            CalculateConverterFactor();
            CalculatePageDimensions();
            GenerateAvaliableLinePositions();
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
            GenerateAvaliableLinePositions();
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
                loadedPageMargins = defaults.PageLayout.PageMargins;
                SetPageMargins(loadedPageMargins);
                systemLayout = defaults.SystemLayout;
                staffLayout = new List<StaffLayoutMusicXML>(defaults.StaffLayout){ };
            }
            GenerateAvaliableLinePositions();
            GenerateStaffLine();
        }

        private void GenerateStaffLine()
        {
            double line = staffHeight.MMToWPFUnit();
            double spaceBetween = StaffSpace.MMToWPFUnit();
            staffLineCoords = new Dictionary<int, double>();
            staffLineCoords.Add(0, 0);
            for (int i = 1; i < 5; i++)
            {
                staffLineCoords.Add(i, line);
                line -= spaceBetween;
            }
            staffLineCoords.Add(5, 0);
        }

        private void GenerateAvaliableLinePositions()
        {
            int lowestPositionIndex = -30;
            int highestPositionIndex = 40;
            double halfLineSpacing = StaffSpace.MMToWPFUnit() / 2;
            // length to point in center between staff lines
            for (int i = lowestPositionIndex; i <= highestPositionIndex; i++)
            {
                avaliableIndexLinePositions.Add(i, i * halfLineSpacing);
            }
        }
        public double GetContentWidth()
        {
            return pageDimensions.GetPageDimensionsInTenths().X.TenthsToWPFUnit() - (pageMarginBoth.LeftMargin.TenthsToWPFUnit() + pageMarginBoth.RightMargin.TenthsToWPFUnit());
        }

        private void SetPageMargins(List<PageMarginsMusicXML> marginsList)
        {
            if (marginsList == null)
            {
                SetDefaultMargins();
                return;
            }

            var notSpecified = marginsList.Any(i => i.MarginTypeSpecified == false);
            if (notSpecified)
            {
                pageMarginBoth = marginsList.ElementAtOrDefault(0);
                pageMarginBoth.MarginType = MarginTypeMusicXML.both;
                pageMarginBoth.MarginTypeSpecified = true;
             }
            else
            {
                foreach (var item in marginsList)
                {
                    switch (item.MarginType)
                    {
                        case MarginTypeMusicXML.odd:
                            pageMarginOdd = item;
                            break;
                        case MarginTypeMusicXML.even:
                            pageMarginEven = item;
                            break;
                        case MarginTypeMusicXML.both:
                            pageMarginBoth = item;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void SetDefaultMargins()
        {
            PageDefaults defaults = new PageDefaults();
            Size currentPageDimensions = this.PageDimensions.Dimensions;
            double left = currentPageDimensions.Width * defaults.Leftmargin;
            double right = currentPageDimensions.Width * defaults.RightMargin;
            double top = currentPageDimensions.Height * defaults.TopMargin;
            double bottom= currentPageDimensions.Height * defaults.BottomMargin;
            pageMarginBoth = new PageMarginsMusicXML()
            {
                MarginTypeSpecified = true,
                MarginType = MarginTypeMusicXML.both,
                LeftMargin = left,
                RightMargin = right,
                TopMargin = top,
                BottomMargin = bottom
            };
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

        private PageMarginsMusicXML GetDefaultMargins()
        {
            SetDefaultMargins();
            return pageMarginBoth;
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
            return Converters.ExtensionMethods.PxPerMM(); //? return DPI / 25.4;
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
        public class PageDefaults
        {
            private double leftmargin = 0.05; // percent of width, dimensions+scale dependent
            private double rightMargin = 0.05; // percent of width, dimensions+scale dependent
            private double topMargin = 0.05; // percent of height, dimensions+scale dependent
            private double bottomMargin = 0.05; // percent of height, dimensions+scale dependent
            private double scale = 40;
            private double staffHeight = 7; // in mm may be used converter
            private double defaultWidth = 210; //in mm
            private double defaultHeight = 297; //in mm

            /// <summary>
            /// Percent of Width
            /// </summary>
            public double Leftmargin
            {
                get
                {
                    return leftmargin;
                }

                set
                {
                    leftmargin = value;
                }
            }
            /// <summary>
            /// Percent of width
            /// </summary>
            public double RightMargin
            {
                get
                {
                    return rightMargin;
                }

                set
                {
                    rightMargin = value;
                }
            }
            /// <summary>
            /// Percent of height
            /// </summary>
            public double TopMargin
            {
                get
                {
                    return topMargin;
                }

                set
                {
                    topMargin = value;
                }
            }
            /// <summary>
            /// Percent of height
            /// </summary>
            public double BottomMargin
            {
                get
                {
                    return bottomMargin;
                }

                set
                {
                    bottomMargin = value;
                }
            }
            /// <summary>
            /// In tenths, for proper MusicXML scalling 40 is used
            /// </summary>
            public double Scale
            {
                get
                {
                    return scale;
                }
            }
            /// <summary>
            /// In mm, use extension method to convert from other supported Unit
            /// </summary>
            public double StaffHeight
            {
                get
                {
                    return staffHeight;
                }

                set
                {
                    staffHeight = value;
                }
            }
            /// <summary>
            /// In mm. use extension method to convert from other supported Unit
            /// </summary>
            public double DefaultWidth
            {
                get
                {
                    return defaultWidth;
                }

                set
                {
                    defaultWidth = value;
                }
            }
            /// <summary>
            /// In mm, use extension method to convert from other supported Unit
            /// </summary>
            public double DefaultHeight
            {
                get
                {
                    return defaultHeight;
                }

                set
                {
                    defaultHeight = value;
                }
            }
            //private PageType pageType
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
            //return new Point(temp.X * pxpermm, temp.Y * pxpermm);
            return new Point(width.TenthsToWPFUnit(), height.TenthsToWPFUnit());
        }
        public Size Dimensions
        {
            get
            {
                return new Size(width.TenthsToWPFUnit(), height.TenthsToWPFUnit());
            }
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
