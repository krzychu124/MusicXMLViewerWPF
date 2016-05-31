using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicXMLViewerWPF
{
    class Part 
    {
        private string part_name;
        private string part_name_display;
        private string part_abbreviation;
        private string part_abbreviation_display;
        
        private ScoreInstrument instrument;
        public Part(XElement x)
        {
            var temp = x;//.Element("score-part");
            part_name = temp.Element("part-name").Value;
            part_name_display = temp.Element("part-name-display") != null ? temp.Element("part-name-display").Value : string.Empty;
            part_abbreviation = temp.Element("part-abbreviation") != null ? temp.Element("part-abbreviation").Value : string.Empty;
            part_abbreviation_display = temp.Element("part-abbreviation-display") != null ? temp.Element("part-abbreviation-display").Value : string.Empty;
        }

    }
    class ScoreInstrument
    {
        private string id;
        private string instrument_name;
        private string instrument_abbreviation;
        private string instrument_sound;

        public string ID { get { return id; } }
        public string InstrumentName {  get { return instrument_name; } }
        public string InstrumentAbbreviation { get { return instrument_abbreviation; } }
        public string InstrumentSound {  get { return instrument_sound; } }

        public ScoreInstrument(XElement x )
        {
            getScoreInstrument(x);
        }
        private void getScoreInstrument(XElement x)
        {
            id = x.Element("score-instrument").Attribute("id").Value;
            instrument_name = x.Element("score-instrument").Element("instrument-name").Value;
            instrument_abbreviation = x.Element("score-instrument").Element("instrument-abbreviation") != null? x.Element("score-instrument").Element("instrument-abbreviation").Value : string.Empty ;
            instrument_sound = x.Element("score-instrument").Element("instrument-sound") != null ? x.Element("score-instrument").Element("instrument-sound").Value : string.Empty;
        }
    }
}
