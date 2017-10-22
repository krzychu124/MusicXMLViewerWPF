using System;
using System.Xml.Serialization;

namespace MusicXMLScore.Model.Helpers.SimpleTypes
{
    [Serializable]
    [XmlType(TypeName = "font-style")]
    public enum FontStyleMusicXML
    {
        normal,
        italic
    }

    [Serializable]
    [XmlType(TypeName = "font-weight")]
    public enum FontWeightMusicXML
    {
        normal,
        bold
    }

    [Serializable]
    [XmlType(TypeName = "start-stop")]
    public enum StartStopMusicXML
    {
        start,
        stop
    }

    [Serializable]
    [XmlType(TypeName = "start-stop-continue")]
    public enum StartStopContinueMusicXML //todo Check for continue string value
    {       
        start,
        stop,
        @continue,
    }

    [Serializable]
    [XmlType(TypeName = "over-under")]
    public enum OverUnderMusicXML
    {
        over,
        under,
    }

    [Serializable]
    [XmlType(TypeName = "left-center-right")]
    public enum LeftCenterRightMusicXML
    {
        left,
        center,
        right,
    }

    [Serializable]
    [XmlType(TypeName = "yes-no")]
    public enum YesNoMusicXML
    {
        yes,
        no,
    }

    [Serializable]
    [XmlType(TypeName = "note-size-type")]
    public enum NoteSizeTypeMusicXML
    {
        large,
        cue,
        grace
    }

    [Serializable]
    [XmlType(TypeName = "step")]
    public enum StepMusicXML
    {
        C,
        D,
        E,
        F,
        G,
        A,
        B
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
    [XmlType(TypeName = "symbol-size")]
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

    [Serializable()]
    [XmlType(TypeName = "backward-forward")]
    public enum BackwardForwardMusicXML
    {
        backward,
        forward,
    }

    [Serializable]
    [XmlType(TypeName ="above-below")]
    public enum AboveBelowMusicXML
    {
        above,
        below,
    }

    [Serializable]
    [XmlType(TypeName = "up-down")]
    public enum UpDownMusicXML
    {
        up,
        down,
    }


    public enum UpDownStopContinueMusicXML
    {
        up,
        down,
        stop,
        @continue,
    }

    [Serializable]
    [XmlType(TypeName = "line-shape")]
    public enum LineShapeMusicXML
    {
        straight,
        curved,
    }

    [Serializable]
    [XmlType(TypeName = "line-type")]
    public enum LineTypeMusicXML
    {
        solid,
        dashed,
        dotted,
        wavy,
    }

    [Serializable]
    [XmlType(TypeName = "accidental-value")]
    public enum AccidentalValueMusicXML
    {
        sharp,
        natural,
        flat,
        [XmlEnum("double-sharp")]
        doublesharp,
        [XmlEnum("sharp-sharp")]
        sharpsharp,
        [XmlEnum("flat-flat")]
        flatflat,
        [XmlEnum("natural-sharp")]
        naturalsharp,
        [XmlEnum("natural-flat")]
        naturalflat,
        //! not implemented
        [XmlEnum("quarter-flat")]
        quarterflat,
        [XmlEnum("quarter-sharp")]
        quartersharp,
        [XmlEnum("three-quarters-flat")]
        threequartersflat,
        [XmlEnum("three-quarters-sharp")]
        threequarterssharp,
        [XmlEnum("sharp-down")]
        sharpdown,
        [XmlEnum("sharp-up")]
        sharpup,
        [XmlEnum("natural-down")]
        naturaldown,
        [XmlEnum("natural-up")]
        naturalup,
        [XmlEnum("flat-down")]
        flatdown,
        [XmlEnum("flat-up")]
        flatup,
        [XmlEnum("triple-sharp")]
        triplesharp,
        [XmlEnum("triple-flat")]
        tripleflat,
        [XmlEnum("slash-quarter-sharp")]
        slashquartersharp,
        [XmlEnum("slash-sharp")]
        slashsharp,
        [XmlEnum("slash-flat")]
        slashflat,
        [XmlEnum("double-slash-flat")]
        doubleslashflat,
        [XmlEnum("sharp-1")]
        sharp1,
        [XmlEnum("sharp-2")]
        sharp2,
        [XmlEnum("sharp-3")]
        sharp3,
        [XmlEnum("sharp-5")]
        sharp5,
        [XmlEnum("flat-1")]
        flat1,
        [XmlEnum("flat-2")]
        flat2,
        [XmlEnum("flat-3")]
        flat3,
        [XmlEnum("flat-4")]
        flat4,
        sori,
        koron,
        none,
    }

    [Serializable]
    [XmlType(TypeName = "beam-value")]
    public enum BeamValueMusicXML
    {
        begin,
        @continue,
        end,
        [XmlEnum("forward hook")]
        forwardhook,
        [XmlEnum("backward hook")]
        backwardhook,
    }
    /// <summary>
    /// Notes with shorter duration than eighth has prefix "Item" in name
    /// </summary>
    public enum NoteDurationValues
    {
        @long = 1,
        breve = 2,
        whole = 4,
        half = 8,
        quarter = 16,
        eighth = 32,
        Item16th = 64,
        Item32nd = 128,
        Item64th = 256,
        Item128th= 512,
        Item256th = 1024,
        Item512th = 2048,
        Item1024th = 4096,
    }

    public enum NoteDurationValuesInverted
    {
        @long = 4096,
        breve = 2048,
        whole = 1024,
        half = 512,
        quarter = 256,
        eighth = 128,
        Item16th = 64,
        Item32nd = 32,
        Item64th = 16,
        Item128th = 8,
        Item256th = 4,
        Item512th = 2,
        Item1024th = 1,
    }
}
