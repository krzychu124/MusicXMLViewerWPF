﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLViewerWPF
{
    [Serializable]
    public class Work // Done
    {
        private string work_number;
        private string work_title;
        [XmlElement("work-number")]
        public string WorkNumber
        {
            get { return work_number; }
            set { work_number = value; }
        }
        [XmlElement("work-title")]
        public  string WorkTitle
        {
            get { return work_title; }
            set { work_title = value; }
        }
        public Work()
        {

        }
        public Work(System.Xml.Linq.XElement x)
        {
            work_number = x.Element("work-number") != null ? x.Element("work-number").Value : "0" ;
            work_title = x.Element("work-title").Value;
        }
    }
}
