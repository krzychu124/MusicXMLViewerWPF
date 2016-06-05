using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF.Work
{
    class Work // Done
    {
        private string work_number;
        private string work_title;

        public string WorkNumber { get { return work_number; } }
        public  string WorkTitle { get { return work_title; } }

        public Work(System.Xml.Linq.XElement x)
        {
            work_number = x.Element("work-number").Value;
            work_title = x.Element("work-title").Value;
        }
    }
}
