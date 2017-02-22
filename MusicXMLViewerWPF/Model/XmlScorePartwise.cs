﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MusicXMLViewerWPF;
using System.Xml.Serialization;
using MusicXMLScore.Model;
using MusicXMLViewerWPF.Identification;
using MusicXMLViewerWPF.Defaults;

namespace MusicXMLViewerWPF
{
    [Serializable()]
    [XmlRoot("score-partwise", Namespace ="", IsNullable =false)]
    public class XmlScorePartwise
    {
        private Work work; //! Done
        private string movementNumber; //! Done
        private string movementTitle; //! Done
        private XmlIdentification identification;
        private DefaultsMusicXML defaults;
        private List<Credit> credits = new List<Credit>(); //! Done
        private XmlPartList partlist;
        private List<Part> part;
        private string version;
        
        [XmlElement(elementName:"work")]
        public Work Work
        {
            get
            {
                return work;
            }

            set
            {
                work = value;
            }
        }
        [XmlElement("movement-number", typeof(string))]
        public string MovementNumber
        {
            get
            {
                return movementNumber;
            }

            set
            {
                movementNumber = value;
            }
        }

        [XmlElement("movement-title", typeof(string))]
        public string MovementTitle
        {
            get
            {
                return movementTitle;
            }

            set
            {
                movementNumber = value;
            }
        }
        [XmlElement("credit")]
        public List<Credit> Credits
        {
            get { return credits; }
            set
            {
                credits = value;
            }
        }
        [XmlAttribute("version")]
        [System.ComponentModel.DefaultValue("1.0")]
        public string Version
        {
            get
            {
                return version;
            }

            set
            {
                version = value;
            }
        }
        //[XmlIgnore]
        [XmlElement("part-list")]
        public XmlPartList Partlist
        {
            get
            {
                return partlist;
            }

            set
            {
                partlist = value;
            }
        }
        [XmlElement("identification")]
        public XmlIdentification Identification
        {
            get
            {
                return identification;
            }

            set
            {
                identification = value;
            }
        }
        [XmlElement("defaults")]
        public DefaultsMusicXML Defaults
        {
            get
            {
                return defaults;
            }

            set
            {
                defaults = value;
            }
        }

        public XmlScorePartwise()
        {
            this.version = "1.0";
        }
    }
}
