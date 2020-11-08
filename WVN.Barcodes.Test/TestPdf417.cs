using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using BC = WVN.Barcodes.Pdf417;

namespace WVN.Barcodes.Test
{
    [TestFixture]
    public class TestPdf417
    {
        private const string ToEncode =
            "BBD-0A64044C-1Q9URN-H|AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA|BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB|19000101\r\n" +
            "1234567812345678123456781234567812345678123456781234567812345678|00000.00\r\n" +
            "1234567812345678123456781234567812345678123456781234567812345678|00000.00\r\n" +
            "1234567812345678123456781234567812345678123456781234567812345678|00000.00\r\n" +
            "1234567812345678123456781234567812345678123456781234567812345678|00000.00\r\n";

        private const string ToEncodeUTF8 = "AYŞE";

        private const string ToEncodeBase64 =
            "BBD-0A64044C-1Q9URN-H|{0}|{1}|19000101|1|9910101010081\r\n" +
            "1234567812345678123456781234567812345678123456781234567812345678|00000.00\r\n" +
            "1234567812345678123456781234567812345678123456781234567812345678|00000.00\r\n" +
            "1234567812345678123456781234567812345678123456781234567812345678|00000.00\r\n" +
            "1234567812345678123456781234567812345678123456781234567812345678|00000.00\r\n";

        [TestCase(ToEncode, "PDF417TestBarcode.png")]
        public void GenerateBitmap(string toEncode, string testFile)
        {
            // Arrange
            const double scale = 1D;
            var settings = GetPdf417Settings(50, 6, 2, 1);

            // Act
            using (IBarcode barcode = new BC.Pdf417())
            {
                using var bmp = barcode.Encode(toEncode, settings);
                using var bmp2 = new Bitmap((int)(bmp.Width * scale), (int)(bmp.Height * scale));
                using (var gfx = Graphics.FromImage(bmp2))
                {
                    gfx.DrawImage(bmp, new Rectangle(0, 0, bmp2.Width, bmp2.Height));
                }
                bmp2.Save(testFile, ImageFormat.Png);
            }

            //Assert
            FileAssert.Exists(testFile);
        }

        [TestCase(ToEncode, "PDF417HiResTestBarcode.png")]
        public void GenerateHiResBitmap(string toEncode, string testFile)
        {
            // Arrange
            const double scale = 1D;
            var settings = GetPdf417Settings(20, 15, 2, 1);
            settings.VerticalDPI = settings.HorizontalDPI = 300;

            // Act
            using (IBarcode barcode = new BC.Pdf417())
            {
                using var bmp = barcode.Encode(toEncode, settings);
                using var bmp2 = new Bitmap((int)(bmp.Width * scale), (int)(bmp.Height * scale));
                using (var gfx = Graphics.FromImage(bmp2))
                {
                    gfx.DrawImage(bmp, new Rectangle(0, 0, bmp2.Width, bmp2.Height));
                }
                bmp2.Save(testFile, ImageFormat.Png);
            }

            //Assert
            FileAssert.Exists(testFile);
        }

        [TestCase(ToEncodeUTF8, "PDF417TestBarcodeUTF8.png")]
        public void TestUTF8Barcode(string toEncode, string testFile)
        {
            // Arrange
            const double scale = 1D;
            var settings = GetPdf417Settings(3, 5, 2, 1);

            // Act
            using (IBarcode barcode = new BC.Pdf417())
            {
                // This is where the trick lies. Problem is that readers should decode to UTF8, too
                var bytes = Encoding.UTF8.GetBytes(toEncode);

                using var bmp = barcode.Encode(bytes, settings);
                using var bmp2 = new Bitmap((int)(bmp.Width * scale), (int)(bmp.Height * scale));
                using (var gfx = Graphics.FromImage(bmp2))
                {
                    gfx.DrawImage(bmp, new Rectangle(0, 0, bmp2.Width, bmp2.Height));
                }
                bmp2.Save(testFile, ImageFormat.Png);
            }

            //Assert
            FileAssert.Exists(testFile);
        }

        [TestCase(ToEncodeBase64, "PDF417TestBarcodeBase64.png")]
        public void TestBase64Barcode(string toEncodeBase64, string testFile)
        {
            // Arrange
            const double scale = 1D;
            var name = Convert.ToBase64String(Encoding.UTF8.GetBytes("AYŞEAYŞE"));
            var surname = Convert.ToBase64String(Encoding.UTF8.GetBytes("AYŞEEŞYAAYŞEEŞYA"));
            var toEncode = string.Format(toEncodeBase64, name, surname);
            var settings = GetPdf417Settings(20, 15, 2, 1);

            // Act
            using (IBarcode barcode = new BC.Pdf417())
            {
                using var bmp = barcode.Encode(toEncode, settings);
                using var bmp2 = new Bitmap((int)(bmp.Width * scale), (int)(bmp.Height * scale));
                using (var gfx = Graphics.FromImage(bmp2))
                {
                    gfx.DrawImage(bmp, new Rectangle(0, 0, bmp2.Width, bmp2.Height));
                }
                bmp2.Save(testFile, ImageFormat.Png);
            }

            //Assert
            FileAssert.Exists(testFile);
        }

        [TestCase(ToEncode, "PDF417TestBarcodeToPrint.html")]
        public void GeneratePrintableHtml(string toEncode, string testFile)
        {
            // Arrange
            var settings = GetPdf417Settings(20, 15, 2, 1);
            settings.ImageFormat = ImageFormat.Png;

            // Act
            using (IBarcode barcode = new BC.Pdf417())
            {
                var b = Convert.ToBase64String(barcode.EncodeToBytes(toEncode, settings));
                var doc = new XDocument(
                  new XElement("html",
                    new XElement("body",
                      new XElement("img", new XAttribute("src", $"data:image/png;base64,{b}"))
                      )
                    )
                );
                doc.Save(testFile);
            }

            //Assert
            FileAssert.Exists(testFile);
        }

        private static BC.Pdf417Settings GetPdf417Settings(int rows, int columns, int errorCorrectionLevel, int moduleWidth)
        {
            return new BC.Pdf417Settings(rows: rows, cols: columns, errorCorrectionLevel: errorCorrectionLevel)
            {
                ModuleWidth = moduleWidth
            };
        }
    }
}
