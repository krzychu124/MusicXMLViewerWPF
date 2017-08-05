using MusicXMLScore.Model.Defaults;
using MusicXMLScore.Model.Helpers;
using MusicXMLScore.Model.Helpers.SimpleTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems
{
    [Serializable]
    [XmlType(TypeName = "print")]
    public class PrintMusicXML
    {
        private PageLayoutMusicXML pageLayout;
        private SystemLayoutMusicXML systemLayout;
        private List<StaffLayoutMusicXML> staffLayout;
        private MeasureLayoutMusicXML measureLayout;
        private MeasureNumberingMusicXML measureNumbering;
        private NameDisplayMusicXML partNameDisplay;
        private NameDisplayMusicXML partAbbreviationDisplay;
        private double staffSpacing;
        private bool staffSpacingSpecified;
        private YesNoMusicXML newSystem;
        private bool newSystemSpecified;
        private YesNoMusicXML newPage;
        private bool newPageSpecified;
        private string blankPage;
        private string pageNumber;

        public PrintMusicXML()
        {

        }

        [XmlElement("page-layout")]
        public PageLayoutMusicXML PageLayout
        {
            get
            {
                return pageLayout;
            }

            set
            {
                pageLayout = value;
            }
        }

        [XmlElement("system-layout")]
        public SystemLayoutMusicXML SystemLayout
        {
            get
            {
                return systemLayout;
            }

            set
            {
                systemLayout = value;
            }
        }

        [XmlElement("staff-layout")]
        public List<StaffLayoutMusicXML> StaffLayout
        {
            get
            {
                return staffLayout;
            }

            set
            {
                staffLayout = value;
            }
        }

        [XmlElement("measure-layout")]
        public MeasureLayoutMusicXML MeasureLayout
        {
            get
            {
                return measureLayout;
            }

            set
            {
                measureLayout = value;
            }
        }

        [XmlElement("measure-numbering")]
        public MeasureNumberingMusicXML MeasureNumbering
        {
            get
            {
                return measureNumbering;
            }

            set
            {
                measureNumbering = value;
            }
        }

        [XmlElement("part-name-display")]
        public NameDisplayMusicXML PartNameDisplay
        {
            get
            {
                return partNameDisplay;
            }

            set
            {
                partNameDisplay = value;
            }
        }

        [XmlElement("part-abbreviation-display")]
        public NameDisplayMusicXML PartAbbreviationDisplay
        {
            get
            {
                return partAbbreviationDisplay;
            }

            set
            {
                partAbbreviationDisplay = value;
            }
        }

        [XmlAttribute("staff-spacing")]
        public double StaffSpacing
        {
            get
            {
                return staffSpacing;
            }

            set
            {
                staffSpacing = value;
            }
        }

        [XmlIgnore]
        public bool StaffSpacingSpecified
        {
            get
            {
                return staffSpacingSpecified;
            }

            set
            {
                staffSpacingSpecified = value;
            }
        }

        [XmlAttribute("new-system")]
        public YesNoMusicXML NewSystem
        {
            get
            {
                return newSystem;
            }

            set
            {
                newSystem = value;
            }
        }

        [XmlIgnoreAttribute]
        public bool NewSystemSpecified
        {
            get
            {
                return newSystemSpecified;
            }

            set
            {
                newSystemSpecified = value;
            }
        }

        [XmlAttribute("new-page")]
        public YesNoMusicXML NewPage
        {
            get
            {
                return newPage;
            }

            set
            {
                newPage = value;
            }
        }

        [XmlIgnore]
        public bool NewPageSpecified
        {
            get
            {
                return newPageSpecified;
            }

            set
            {
                newPageSpecified = value;
            }
        }

        [XmlAttribute("blank-page")]
        public string BlankPage
        {
            get
            {
                return blankPage;
            }

            set
            {
                blankPage = value;
            }
        }

        [XmlAttribute("page-number")]
        public string PageNumber
        {
            get
            {
                return pageNumber;
            }

            set
            {
                pageNumber = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName = "measure-layout")]
    public class MeasureLayoutMusicXML
    {
        private double measureDistance;
        private bool measureDistanceSpecified;

        [XmlElement("measure-distance")]
        public double MeasureDistance
        {
            get
            {
                return measureDistance;
            }

            set
            {
                measureDistance = value;
            }
        }

        [XmlIgnore]
        public bool MeasureDistanceSpecified
        {
            get
            {
                return measureDistanceSpecified;
            }

            set
            {
                measureDistanceSpecified = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName ="measure-numbering")]
    [DebuggerDisplay("Value = {value}")]
    public class MeasureNumberingMusicXML
    {
        private MeasureNumberingValueMusicXML value;

        [XmlText]
        public MeasureNumberingValueMusicXML Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
    }

    [Serializable]
    [XmlType(TypeName ="measure-numbering-value")]
    public enum MeasureNumberingValueMusicXML
    {
        none,
        measure,
        system
    }
}