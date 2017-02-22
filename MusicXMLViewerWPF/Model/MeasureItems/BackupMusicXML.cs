using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.MeasureItems
{
    [Serializable]
    [XmlType("backup")]
    public class BackupMusicXML
    {
        private double duration;
        //formattedText footnote
        //level levelField;

        public BackupMusicXML()
        {

        }

        [XmlElement("duration")]
        public double Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }
    }
}