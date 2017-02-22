using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Helpers.SimpleTypes
{
    public enum FontStyleMusicXML
    {
        normal,
        italic
    }
    public enum FontWeightMusicXML
    {
        normal,
        bold
    }
    public enum StartStopMusicXML
    {
        start,
        stop
    }
    public enum YesNoMusicXML
    {
        yes,
        no
    }
    public enum NoteSizeTypeMusicXML
    {
        large,
        cue,
        grace
    }
    [Serializable]
    [XmlType(TypeName ="accidental-value")]
    public enum AccidentalValueMusicXML
    {
        natural,
        sharp,
        flat,
        //etc...
    }
    [Serializable]
    [XmlType(TypeName ="step")]
    public enum StepMusicXML
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G
    }
    [Serializable]
    [XmlType(TypeName = "cancel-location")]
    public enum CancelLocationMusicXML
    {
        left,
        right,
        [XmlEnum("before-barline")]
        beforebarline,
    }
}
