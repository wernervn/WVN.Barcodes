using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace WVN.Barcodes.Pdf417
{
    internal class CodeWord
    {
        public int Numeric { get; }

        public CodeWord(int num)
        {
            Numeric = num;
        }

        public virtual IEnumerable<Brush> PatternBrushes()
        {
            // we start with 1
            yield return Brushes.Black;

            var bits = new BitArray(new int[] { Numeric });
            for (var i = 14; i >= 0; i--)
            {
                yield return bits[i] ? Brushes.Black : Brushes.White;
            }

            // we end with 0
            yield return Brushes.White;
        }
    }

    internal sealed class StartWord : CodeWord
    {
        public StartWord() : base(0x7f54) { }
    }

    internal sealed class EndWord : CodeWord
    {
        public EndWord() : base(0x7e8a) { }

        public override IEnumerable<Brush> PatternBrushes()
        {
            // we start with 1
            yield return Brushes.Black;

            var bits = new BitArray(new int[] { Numeric });
            for (var i = 14; i >= 0; i--)
            {
                yield return bits[i] ? Brushes.Black : Brushes.White;
            }

            // we end with 0
            yield return Brushes.White;

            // end word's got an extra 1
            yield return Brushes.Black;
        }
    }

}
