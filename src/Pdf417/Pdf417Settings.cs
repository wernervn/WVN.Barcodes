using System.Drawing.Imaging;

namespace WVN.Barcodes.Pdf417
{
    public sealed class Pdf417Settings : BarcodeSettings
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int ErrorCorrectionLevel { get; set; }
        public int ModuleWidth { get; set; }

        public Pdf417Settings() : this(0, 0, -1) { }
        public Pdf417Settings(int errorCorrectionLevel) : this(0, 0, errorCorrectionLevel) { }
        public Pdf417Settings(int rows, int cols) : this(rows, cols, -1) { }
        public Pdf417Settings(int rows, int cols, int errorCorrectionLevel)
        {
            Rows = rows;
            Columns = cols;
            ErrorCorrectionLevel = errorCorrectionLevel;
            ModuleWidth = 1;
            ImageFormat = ImageFormat.Png;
            VerticalDPI = 96f;
            HorizontalDPI = 96f;
        }
    }
}
