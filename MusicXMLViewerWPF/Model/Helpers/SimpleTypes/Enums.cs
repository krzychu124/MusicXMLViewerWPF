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

    [Serializable]
    [XmlType(TypeName ="symbol-size")]
    public enum SymbolSizeMusicXML
    {
        full,
        cue,
        large
    }
    [Serializable()]
    [XmlType(TypeName = "note-type-value")]
    public enum NoteTypeValueMusicXML
    {
        [XmlEnum("1024th")]
        Item1024th,
        [XmlEnum("512th")]
        Item512th,
        [XmlEnum("256th")]
        Item256th,
        [XmlEnum("128th")]
        Item128th,
        [XmlEnum("64th")]
        Item64th,
        [XmlEnum("32nd")]
        Item32nd,
        [XmlEnum("16th")]
        Item16th,
        eighth,
        quarter,
        half,
        whole,
        breve,
        @long,
        maxima,
    }
}
