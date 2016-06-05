using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class MusicScore
    {
        protected static Defaults.Defaults defaults; 
        protected static Dictionary<string, ScoreParts.Part.Part> parts = new Dictionary<string, ScoreParts.Part.Part>() { };
        protected static Identification.Identification identification; // TODO not implemented
        protected static List<Credit.Credit> credits = new List<Credit.Credit>();
        protected static List<PartList> musicscoreparts;// = new List<PartList>(); // TODO tests
        protected static Work.Work work; 
        protected static XElement file; // <<Loaded file>>

        public static Defaults.Defaults Defaults { get { return defaults; } }
        public static Dictionary<string, ScoreParts.Part.Part> Parts { get { return parts; } }
        public static Identification.Identification Identification { get { return identification; } }
        public static List<Credit.Credit> CreditList { get { return credits; } }
        public static List<PartList> ScoreParts { get { return musicscoreparts; } }
        public static Work.Work Work { get { return work; } }
        public static XElement File { get { return file; } }

        public MusicScore(XDocument x)
        {
            file = x.Element("score-partwise");
            if (file != null) { Logger.Log("File Loaded"); } else { Logger.Log("Problem with loading file"); }
            LoadToClasses();
        }
        private void LoadToClasses()
        {
            work = file.Element("work") != null ? new Work.Work(file.Element("work")) : null;
            defaults = new Defaults.Defaults(file.Element("defaults")); 
            identification = new Identification.Identification(file.Element("identification")); 
            foreach (var item in file.Elements("credit"))
            {
                credits.Add(new Credit.Credit(item));
            }
            foreach (var item in file.Elements("part"))
            {
                 parts.Add(item.Attribute("id").Value, new ScoreParts.Part.Part(item));
            }
        }
    }
}
