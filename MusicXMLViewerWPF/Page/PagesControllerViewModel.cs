using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.Page
{
    class PagesControllerViewModel
    {
        private string header;

        public string Header {  get { return header; } }
        public object Content {  get { return new object(); } }
        public PagesControllerViewModel()
        {
               
        }
    }
}
