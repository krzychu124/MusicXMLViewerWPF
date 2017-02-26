﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.LayoutStyle
{
    [Serializable]
    public class PageLayoutStyle
    {
        private PageMargins oddMargins;
        private PageMargins evenMargins;
        /// <summary>
        /// While TwoSided false - EvenMargins doesn't take into account
        /// </summary>
        private bool twoSided = false;
        #region SystemLayout
        private bool displayCourtesyClef = false;
        private bool displayCourtesyKey = false;
        private bool displayClefEachSystem = true;
        private bool displayKeyEachSystem = true;
        
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