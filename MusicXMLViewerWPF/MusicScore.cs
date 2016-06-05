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
        protected static Defaults.Defaults defaults; // TODO not implemented
        protected static Dictionary<string, ScoreParts.Part.Part> parts = new Dictionary<string, ScoreParts.Part.Part>() { };
        protected static Identyfication.Identification identyfication; // TODO not implemented
        protected static List<Credit.Credit> credits = new List<Credit.Credit>(); // TODO not implemented class
        protected static List<PartList> musicscoreparts = new List<PartList>(); // TODO tests
        protected static Work.Work work; // TODO not implemented
        protected static XDocument file; // <<Loaded file>>

        public static Defaults.Defaults Defaults { get { return defaults; } }
        public static Dictionary<string, ScoreParts.Part.Part> Parts { get { return parts; } }
        public static Identyfication.Identification Identification { get { return identyfication; } }
        public static List<Credit.Credit> CreditList { get { return credits; } }
        public static List<PartList> ScoreParts { get { return musicscoreparts; } }
        public static Work.Work Work { get { return work; } }
        public static XDocument File { get { return file; } }

        public MusicScore(XDocument x)
        {
            file = x;
            LoadToClasses();
        }
        private void LoadToClasses()
        {
            work = new Work.Work(file.Element("work"));
            defaults = new Defaults.Defaults(file.Element("defaults")); //TODO_H missing implementation
            identyfication = new Identyfication.Identification(file.Element("identificatino")); // TODO_H missing implementation
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
