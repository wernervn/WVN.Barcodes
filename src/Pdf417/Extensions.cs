using System.Drawing;

namespace WVN.Barcodes.Pdf417
{
    internal static class Extensions
    {
        public static void DrawCodeWord(this Graphics gfx, CodeWord cw, ref int x, ref int y, ref int w, ref int h)
        {
            foreach (var brush in cw.PatternBrushes())
            {
                gfx.FillRectangle(brush, x, y, w, h);
                x += w;
            }
        }
    }
}
