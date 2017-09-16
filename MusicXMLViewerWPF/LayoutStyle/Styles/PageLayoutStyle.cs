using System;
using System.ComponentModel;

namespace MusicXMLScore.LayoutStyle
{
    [Serializable]
    public class PageLayoutStyle : INotifyPropertyChanged
    {
        private PageMargins oddMargins;
        private PageMargins evenMargins;
        /// <summary>
        /// While TwoSided false - EvenMargins ignored
        /// </summary>
        private bool twoSided = false;
        #region SystemLayout
        private bool displayCourtesyClef = false;
        private bool displayCourtesyKey = false;
        private bool displayClefEachSystem = true;
        private bool displayKeyEachSystem = true;

        private bool stretchSystemToPageWidth = true;
        private bool stretchLastSystemOnPage = true;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public PageMargins OddMargins
        {
            get
            {
                return oddMargins;
            }

            set
            {
                oddMargins = value;
            }
        }

        public PageMargins EvenMargins
        {
            get
            {
                return evenMargins;
            }

            set
            {
                evenMargins = value;
            }
        }

        public bool TwoSided
        {
            get
            {
                return twoSided;
            }

            set
            {
                twoSided = value;
            }
        }

        public bool DisplayCourtesyClef
        {
            get
            {
                return displayCourtesyClef;
            }

            set
            {
                displayCourtesyClef = value;
            }
        }

        public bool DisplayCourtesyKey
        {
            get
            {
                return displayCourtesyKey;
            }

            set
            {
                displayCourtesyKey = value;
            }
        }

        public bool DisplayClefEachSystem
        {
            get
            {
                return displayClefEachSystem;
            }

            set
            {
                displayClefEachSystem = value;
            }
        }

        public bool DisplayKeyEachSystem
        {
            get
            {
                return displayKeyEachSystem;
            }

            set
            {
                displayKeyEachSystem = value;
            }
        }

        public bool StretchLastSystemOnPage
        {
            get
            {
                return stretchLastSystemOnPage;
            }

            set
            {
                if (stretchLastSystemOnPage != value)
                {
                    stretchLastSystemOnPage = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(StretchLastSystemOnPage)));
                }
            }
        }

        public bool StretchSystemToPageWidth
        {
            get
            {
                return stretchSystemToPageWidth;
            }

            set
            {
                if (stretchSystemToPageWidth != value)
                {
                    stretchSystemToPageWidth = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(StretchSystemToPageWidth)));
                }
            }
        }
        #endregion

        public PageLayoutStyle()
        {
            TwoSided = true;
            oddMargins = new PageMargins();
            EvenMargins = new PageMargins();
        }

        public PageMargins GetPageMargins(int pageNumber)
        {
            if (TwoSided)
            {
                if (pageNumber % 2 == 0)
                {
                    return EvenMargins;
                }
                else
                {
                    return oddMargins;
                }
            }
            else
            {
                return oddMargins;
            }
        }
    }

    [Serializable]
    public class PageMargins
    {
        private double leftMargin = 50;
        private double rightMargin = 50;
        private double topMargin = 60;
        private double bottomMargin = 120;

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
    }

    public enum PageMarginType
    {
        both,
        odd,
        even
    }
}
