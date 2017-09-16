using System;
using System.Xml.Serialization;

namespace MusicXMLViewerWPF
{
    [Serializable]
    public class WorkMusicXML // Done
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

        public WorkMusicXML()
        {

        }

        public WorkMusicXML(System.Xml.Linq.XElement x)
        {
            work_number = x.Element("work-number") != null ? x.Element("work-number").Value : "0" ;
            work_title = x.Element("work-title").Value;
        }
    }
}
