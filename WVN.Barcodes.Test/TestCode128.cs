using System.Drawing.Imaging;
using NUnit.Framework;
using BC = WVN.Barcodes.Code128;

namespace WVN.Barcodes.Test
{
    [TestFixture]
    public class TestCode128
    {
        [TestCase("PCK05220544", "Code128TestBarcode.png")]
        [TestCase("123456789", "Code128TestBarcodeCodeC.png")] // Numbers-only "123456789"; // length % 2 == 1
        public void GenerateBitmap(string toEncode, string testFile)
        {
            // Assert
            using IBarcode barcode = new BC.Code128();
            var settings = GetCode128Settings();

            // Act
            using var bmp = barcode.Encode(toEncode, settings);
            bmp.Save(testFile, ImageFormat.Png);

            //Assert
            FileAssert.Exists(testFile);
        }

        [TestCase("High Res (300dpi)", "Code128HiResTestBarcode.png")]
        public void GenerateHighResBitmap(string toEncode, string testFile)
        {
            // Assert
            using IBarcode barcode = new BC.Code128();
            var settings = GetCode128Settings();
            settings.VerticalDPI = settings.HorizontalDPI = 300f;

            // Act
            using var bmp = barcode.Encode(toEncode, settings);
            bmp.Save(testFile, ImageFormat.Png);

            //Assert
            FileAssert.Exists(testFile);
        }

        private static BC.Code128Settings GetCode128Settings(int height = 70, int moduleWidth = 2)
        {
            return new BC.Code128Settings(height)
            {
                ModuleWidth = moduleWidth
            };
        }
    }
}