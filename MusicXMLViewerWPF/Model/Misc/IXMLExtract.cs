using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    interface IXMLExtract // todo remove/refactor
    {
        //IEnumerable<XElement> XMLExtractor(); //extract xelement from xml
        void ExtractXElement(XElement x);     //extract values,attributes // make and fill class instance with them
    }
}
