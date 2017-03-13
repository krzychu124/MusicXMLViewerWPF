using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXMLScore.Helpers;
using MusicXMLScore.Model.MeasureItems.Attributes;
using MusicXMLScore.Converters;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLScore.DrawingHelpers.MeasureVisual
{
    class ClefVisualObject : IDrawableObjectBase
    {
        private CanvasList objectVisual;
        private bool invalidated = false;
        private bool isSmall = false;
        private ClefMusicXML clef;
        private double[] staffCoords;
        private string clefSymbol;
        private int number = 1;
        private double linePosition;
        public ClefVisualObject(ClefMusicXML clefInfo, double[] staffLineCoords)
        {
            clef = clefInfo;
            number = int.Parse(clef?.Number ?? "1");
            staffCoords = staffLineCoords;
            GetClefSymbol();
            InitObjectVisual();
        }

        private void InitObjectVisual()
        {
            objectVisual = new CanvasList(new Size(10, 10));
            objectVisual.SetToolTipText(clef.Sign.ToString());
            DrawingVisual visual = new DrawingVisual();
            DrawingHelpers.DrawingMethods.DrawCharacterGlyph(visual, new Point(0, linePosition), clefSymbol.GetGlyphIndexOfCharacter());
            objectVisual.AddVisual(visual);
        }

        private void GetClefSymbol()
        {
            int octave =0;
            if (clef.ClefOctaveChange != null)
            {
                int.TryParse(clef.ClefOctaveChange, out octave);
            }
            int line = 0;
            if (clef.Line != null)
            {
                int.TryParse(clef.Line, out line);
            }
            SetSymbol(clef.Sign, line, octave);
        }

        private void SetSymbol(ClefSignMusicXML sign, int line, int octaveChange)
        {
            switch (sign)
            {
                case ClefSignMusicXML.G:
                    clefSymbol = MusicSymbols.GClef;
                    break;
                case ClefSignMusicXML.F:
                    clefSymbol = MusicSymbols.FClef;
                    break;
                case ClefSignMusicXML.C:
                    clefSymbol = MusicSymbols.CClef;
                    break;
                case ClefSignMusicXML.percussion:
                    clefSymbol = MusicSymbols.Percussion;
                    //linePosition = staffCoords[line];
                    break;
                case ClefSignMusicXML.TAB:
                    clefSymbol = MusicSymbols.TAB;
                    break;
                case ClefSignMusicXML.jianpu:
                    throw new NotImplementedException();
                case ClefSignMusicXML.none:
                    clefSymbol = "";
                    break;
            }
            if (octaveChange != 0)
            {
                if (clef.Sign == ClefSignMusicXML.G)
                {
                    switch (octaveChange)
                    {
                        case -2:
                            clefSymbol = MusicSymbols.GClef15Down;
                            break;
                        case -1:
                            clefSymbol = MusicSymbols.GClef8Down;
                            break;
                        case 1:
                            clefSymbol = MusicSymbols.GClef8Up;
                            break;
                        case 2:
                            clefSymbol = MusicSymbols.GClef15Up;
                            break;
                    }
                }
                if (clef.Sign == ClefSignMusicXML.F)
                {
                    switch (octaveChange)
                    {
                        case -2:
                            clefSymbol = MusicSymbols.FClef15Down;
                            break;
                        case -1:
                            clefSymbol = MusicSymbols.FClef8Down;
                            break;
                        case 1:
                            clefSymbol = MusicSymbols.FClef8Up;
                            break;
                        case 2:
                            clefSymbol = MusicSymbols.FClef15Up;
                            break;
                    }
                }
                if (clef.Sign == ClefSignMusicXML.C)
                {
                    switch (octaveChange)
                    {
                        case -1:
                            clefSymbol = MusicSymbols.CClef8Down;
                            break;
                    }
                }
            }
            if (line == 0)
            {
                if (clef.Sign == ClefSignMusicXML.G)
                {
                    linePosition = staffCoords[2];
                }
                if (clef.Sign == ClefSignMusicXML.C)
                {
                    linePosition = staffCoords[3];
                }
                if (clef.Sign == ClefSignMusicXML.F)
                {
                    linePosition = staffCoords[4];
                }
                if (clef.Sign == ClefSignMusicXML.percussion)
                {
                    linePosition = staffCoords[2];
                }
            }
            else
            {
                if (clef.Sign == ClefSignMusicXML.percussion)
                {
                    linePosition = staffCoords[2];
                }
                else
                {
                    linePosition = staffCoords[line - 1];
                }
            }
        }

        public CanvasList BaseObjectVisual
        {
            get
            {
                return objectVisual;
            }
        }

        public void InvalidateVisualObject()
        {
            invalidated = true;
        }
    }
}
