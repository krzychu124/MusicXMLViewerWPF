using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicXMLViewerWPF
{
    public static class TypeFaces
    {
        public static Typeface NotesFont = new Typeface(new FontFamily("Bravura Text"), FontStyles.Normal, FontWeights.Regular, FontStretches.Normal);
       //public static Typeface NotesFont = new Typeface("Bravura Text");
       public static Typeface MeasuresFont =  new Typeface(new FontFamily("Bravura Text"), FontStyles.Normal, FontWeights.Regular, FontStretches.Normal);
       public static Typeface GraceNoteFont = new Typeface(new FontFamily(new Uri("pack://application:,,,/"), "./resources/Fonts/#Bravura Text"), FontStyles.Normal, FontWeights.Regular, FontStretches.Normal);
       public static Typeface TextFont = new Typeface(new FontFamily("Times New Roman"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
       public static Typeface TimeNumbers = new Typeface(new FontFamily("Bravura Text"), FontStyles.Normal, FontWeights.Regular, FontStretches.Normal);
    }
}
