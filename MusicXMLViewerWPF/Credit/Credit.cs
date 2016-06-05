using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLViewerWPF.Credit
{
    class Credit 
    {
        #region Attributes
        private int page; // number of page where credit is presented
        #endregion

        #region Elements
        private CreditWords credit_words;
        private string credit_type;
        #endregion

        #region Public properties read-only
        public CreditWords CreditWords { get { return credit_words; } }
        public int Page { get { return page; } }
        public string CreditType { get { return credit_type; } }
        #endregion

        public Credit(System.Xml.Linq.XElement x)
        {
            page = int.Parse(x.Attribute("page").Value);
            credit_type = x.Element("credit-type") != null ? x.Element("credit-type").Value : null;
            credit_words = new CreditWords(x);
        }
    }

    internal class CreditWords : EmptyPrintStyle
    {
        private string value;

        public string Value { get { return value; } }

        public CreditWords(System.Xml.Linq.XElement x ) : base (x.Attributes())
        {
            value = x.Value;
        }
    }
}
