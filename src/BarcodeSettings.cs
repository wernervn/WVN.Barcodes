using System.Drawing.Imaging;

namespace WVN.Barcodes
{
    public abstract class BarcodeSettings
    {
        public ImageFormat ImageFormat { get; set; }
        public float VerticalDPI { get; set; }
        public float HorizontalDPI { get; set; }
    }
}
