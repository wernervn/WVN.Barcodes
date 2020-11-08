using System.Drawing.Imaging;

namespace WVN.Barcodes.Code128
{
    public sealed class Code128Settings : BarcodeSettings
    {
        public int Height { get; set; }
        public int ModuleWidth { get; set; }

        public Code128Settings() : this(30) { }

        public Code128Settings(int height)
        {
            Height = height;
            ModuleWidth = 1;
            ImageFormat = ImageFormat.Png;
            VerticalDPI = 96f;
            HorizontalDPI = 96f;
        }
    }
}
