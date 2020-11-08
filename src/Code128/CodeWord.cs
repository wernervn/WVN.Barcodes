using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace WVN.Barcodes.Code128
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
            var bits = new BitArray(new int[] { Numeric });
            for (var i = 10; i >= 0; i--)
            {
                System.Diagnostics.Trace.Write(bits[i] ? "1" : "0");
                yield return bits[i] ? Brushes.Black : Brushes.White;
            }
        }
    }

    internal sealed class StartWordA : CodeWord
    {
        public StartWordA() : base(0x684) { }
    }

    internal sealed class StartWordB : CodeWord
    {
        public StartWordB() : base(0x690) { }
    }

    internal sealed class StartWordC : CodeWord
    {
        public StartWordC() : base(0x69c) { }
    }

    internal sealed class EndWord : CodeWord
    {
        public EndWord() : base(0x18eb) { }
        public override IEnumerable<Brush> PatternBrushes()
        {
            var bits = new BitArray(new int[] { Numeric });
            for (var i = 12; i >= 0; i--)
            {
                yield return bits[i] ? Brushes.Black : Brushes.White;
            }
        }
    }
}
