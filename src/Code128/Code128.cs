using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using WVN.Barcodes.Common;
using WVN.Barcodes.Exceptions;

namespace WVN.Barcodes.Code128
{
    public sealed class Code128 : IBarcode
    {
        #region Properties

        private readonly CodeWord EndPattern = new EndWord(); // only codeword with 7 parts
        private readonly List<TextEncodingTable> TextMatrices = new List<TextEncodingTable>();

        private int ModuleWidth { get; }
        private CodeWord StartPattern { get; }
        private EncodingTable EncodingTable { get; set; }

        #endregion

        #region ctor

        public Code128()
        {
            InitialiseTextMatrices();
            InitialiseEncodingTable();
            ModuleWidth = 1;
        }
        public void Dispose()
        {
            EncodingTable.Clear();
            EncodingTable = null;

            TextMatrices[0].Clear();
            TextMatrices[1].Clear();
            TextMatrices[2].Clear();

            TextMatrices[0] = null;
            TextMatrices[1] = null;
            TextMatrices[2] = null;
        }

        #endregion

        #region Private Methods

        #region Initialisation

        private void InitialiseTextMatrices()
        {
            // TODO: Complete text matrices (CodeA is incomplete)
            var code_a = new TextEncodingTable();
            var code_b = new TextEncodingTable();
            var code_c = new TextEncodingTable();

            Enumerable.Range(0, 100).ToList().ForEach(idx =>
            {
                code_a.Add(new TextEncoding(string.Empty, idx));
                code_b.Add(new TextEncoding(string.Empty, idx));
                code_c.Add(new TextEncoding(idx.ToString("00"), idx));
            });

            for (var i = 0; i < 64; i++)
            {
                code_a[i] = new TextEncoding(((char)(i + 32)).ToString(), i);
                code_b[i] = new TextEncoding(((char)(i + 32)).ToString(), i);
            }
            for (var i = 64; i < 94; i++)
            {
                code_b[i] = new TextEncoding(((char)(i + 32)).ToString(), i);
            }

            TextMatrices.Insert(0, code_a);
            TextMatrices.Insert(1, code_b);
            TextMatrices.Insert(2, code_c);
        }

        private void InitialiseEncodingTable()
        {
            #region Patterns

            var patterns = new List<int>
            {
                0x6cc,  0x66c,  0x666,  0x498,  0x48c,  0x44c,  0x4c8,  0x4c4,  0x464,  0x648,  0x644,  0x624,
                0x59c,  0x4dc,  0x4ce,  0x5cc,  0x4ec,  0x4e6,  0x672,  0x65c,  0x64e,  0x6e4,  0x674,  0x76e,
                0x74c,  0x72c,  0x726,  0x764,  0x734,  0x732,  0x6d8,  0x6c6,  0x636,  0x518,  0x458,  0x446,
                0x588,  0x468,  0x462,  0x688,  0x628,  0x622,  0x5b8,  0x58e,  0x46e,  0x5d8,  0x5c6,  0x476,
                0x776,  0x68e,  0x62e,  0x6e8,  0x6e2,  0x6ee,  0x758,  0x746,  0x716,  0x768,  0x762,  0x71a,
                0x77a,  0x642,  0x78a,  0x530,  0x50c,  0x4b0,  0x486,  0x42c,  0x426,  0x590,  0x584,  0x4d0,
                0x4c2,  0x434,  0x432,  0x612,  0x650,  0x7ba,  0x614,  0x47a,  0x53c,  0x4bc,  0x49e,  0x5e4,
                0x4f4,  0x4f2,  0x7a4,  0x794,  0x792,  0x6de,  0x6f6,  0x7b6,  0x578,  0x51e,  0x45e,  0x5e8,
                0x5e2,  0x7a8,  0x7a2,  0x5de,  0x5ee,  0x75e,  0x7ae
            };

            #endregion
            EncodingTable = new EncodingTable();
            EncodingTable.AddRange(patterns.Select(p => new CodeWord(p)));
            // Add StartCodes
            EncodingTable.Add(new StartWordA());
            EncodingTable.Add(new StartWordB());
            EncodingTable.Add(new StartWordC());
        }

        #endregion

        #region Helpers

        private int CalculateCheckDigit(List<int> indices)
        {
            var sum = indices[0]; // startcode value
            for (var i = 1; i < indices.Count; i++)
            {
                sum += (indices[i] * i);
            }
            var val = sum % 103;

            //val = val < 95 ? val + 32 : val + 105
            //System.Diagnostics.Debug.Assert(val < 127 || val > 199)

            return val;
        }
        private bool IsOfSubType(string chr, SubTypes subtype)
        {
            var idx = (int)subtype;
            return TextMatrices[idx].Any(enc => !enc.IsSwitch && enc.TextValue == chr);
        }

        #endregion

        #region TextEncoding

        // TODO: Implement CodeC subtype properly
        private List<int> GetTextIndices(string text) => GetTextEncodings(text);

        private List<int> GetTextEncodings(string text)
        {
            var currentSubType = GetSuitableSubType(text);
            var enc = 103 + (int)currentSubType;
            var encodings = new List<int> { enc }; // StartWord
            if (currentSubType == SubTypes.CodeC)
            {
                EncodeAllAsCodeC(text, encodings);
            }
            else
            {
                for (var idx = 0; idx < text.Length; idx++)
                {
                    currentSubType = GetEncodingMethod(currentSubType)(text, idx, encodings);
                }
            }
            var chk = CalculateCheckDigit(encodings);
            encodings.Add(chk);
            return encodings;
        }

        private void EncodeAllAsCodeC(string text, List<int> encodings)
        {
            var extra = string.Empty;
            if (text.Length % 2 != 0)
            {
                extra = text.Substring(text.Length - 1); // get the last char
                text = text.Substring(0, text.Length - 1); // strip it off
            }
            var mi = (int)SubTypes.CodeC;
            for (var idx = 0; idx < text.Length; idx += 2) // take 2 at a time
            {
                var check = text.Substring(idx, 2);
                encodings.Add(TextMatrices[mi].FirstOrDefault(enc => enc.TextValue == check).Encoding);
            }
            // if we have an extra char, encode it in CodeA
            if (!string.IsNullOrEmpty(extra))
            {
                mi = (int)SubTypes.CodeA;
                encodings.Add((int)Switch.CodeA);
                encodings.Add(TextMatrices[mi].FirstOrDefault(enc => enc.TextValue == extra).Encoding);
            }
        }

        private SubTypes GetSuitableSubType(string text)
        {
            var result = SubTypes.CodeA; // default
            var carr = text.ToCharArray();
            if (carr.Any(c => char.IsLower(c)))
            {
                result = SubTypes.CodeB;
            }

            if (carr.All(c => char.IsDigit(c)))
            {
                result = SubTypes.CodeC;
            }

            return result;
        }

        private Func<string, int, List<int>, SubTypes> GetEncodingMethod(SubTypes currentSubType)
        {
            switch (currentSubType)
            {
                case SubTypes.CodeA: return DoTextEncodingForCodeA;
                case SubTypes.CodeB: return DoTextEncodingForCodeB;
                case SubTypes.CodeC: return DoTextEncodingForCodeC;
                default:
                    throw new BarcodeException("Invalid SubType");
            }
        }

        private SubTypes DoTextEncodingForCodeA(string text, int idx, List<int> encodings)
        {
            var chr = text[idx].ToString();
            var mi = -1;
            var st = SubTypes.CodeA;

            if (IsOfSubType(chr, SubTypes.CodeA))
            {
                mi = (int)SubTypes.CodeA;
            }
            else if (IsOfSubType(chr, SubTypes.CodeB))
            {
                mi = (int)SubTypes.CodeB;
                // TODO: check if we should Shift or Jump
                encodings.Add((int)Switch.CodeB);
                st = SubTypes.CodeB;
            }
            else if (IsOfSubType(chr, SubTypes.CodeC))
            {
                mi = (int)SubTypes.CodeC;
                encodings.Add((int)Switch.CodeC);
                st = SubTypes.CodeC;
            }
            else
            {
                System.Diagnostics.Trace.WriteLine($"Invalid character: '{chr}'. Text = '{text}'. idx = {idx}");
                throw new BarcodeException($"Invalid character: '{chr}'");
            }

            encodings.Add(TextMatrices[mi].FirstOrDefault(enc => enc.TextValue == chr).Encoding);
            return st;
        }
        private SubTypes DoTextEncodingForCodeB(string text, int idx, List<int> encodings)
        {
            var chr = text[idx].ToString();
            var mi = -1;
            var st = SubTypes.CodeB;

            if (IsOfSubType(chr, SubTypes.CodeB))
            {
                mi = (int)SubTypes.CodeB;
            }
            else if (IsOfSubType(chr, SubTypes.CodeA))
            {
                mi = (int)SubTypes.CodeA;
                // TODO: check if we should Shift or Jump
                encodings.Add((int)Switch.CodeA);
                st = SubTypes.CodeA;
            }
            else if (IsOfSubType(chr, SubTypes.CodeC))
            {
                mi = (int)SubTypes.CodeC;
                encodings.Add((int)Switch.CodeC);
                st = SubTypes.CodeC;
            }
            else
            {
                throw new BarcodeException("Invalid character");
            }

            encodings.Add(TextMatrices[mi].FirstOrDefault(enc => enc.TextValue == chr).Encoding);
            return st;
        }
        private SubTypes DoTextEncodingForCodeC(string text, int idx, List<int> encodings)
        {
            var chr = text[idx].ToString();
            var mi = -1;
            var st = SubTypes.CodeC;

            if (IsOfSubType(chr, SubTypes.CodeC))
            {
                mi = (int)SubTypes.CodeC;
            }
            else if (IsOfSubType(chr, SubTypes.CodeA))
            {
                mi = (int)SubTypes.CodeA;
                encodings.Add((int)Switch.CodeA);
                st = SubTypes.CodeA;
            }
            else if (IsOfSubType(chr, SubTypes.CodeB))
            {
                mi = (int)SubTypes.CodeB;
                encodings.Add((int)Switch.CodeB);
                st = SubTypes.CodeB;
            }
            else
            {
                throw new BarcodeException("Invalid character");
            }

            encodings.Add(TextMatrices[mi].FirstOrDefault(enc => enc.TextValue == chr).Encoding);
            return st;
        }

        #endregion

        #region Builders

        private List<CodeWord> GetCodewords(List<int> indices) => indices.Select(idx => EncodingTable[idx]).ToList();

        private Bitmap DrawBarcode(List<int> data, Code128Settings settings)
        {
            var codewords = GetCodewords(data);
            var w = settings.ModuleWidth;
            var h = settings.Height;
            var q = w * 2; // quite zone
            var width = ((11 * data.Count) + 14) * w + 2 * q;
            var height = h + 2 * q;
            var bmp = new Bitmap(width, height);

            // set resolution
            bmp.SetResolution(settings.HorizontalDPI, settings.VerticalDPI);

            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.InterpolationMode = InterpolationMode.High;
                gfx.Clear(Color.White);

                var x = q;
                var y = q;
                // startcodes are encoded into the indices
                codewords.ForEach(cw => gfx.DrawCodeWord(cw, ref x, ref y, ref w, ref h));
                gfx.DrawCodeWord(EndPattern, ref x, ref y, ref w, ref h);
            }
            return bmp;
        }

        #endregion

        #endregion

        #region Public Methods

        public Bitmap Encode(string text) => Encode(text, new Code128Settings());

        public Bitmap Encode(string text, BarcodeSettings settings) => DrawBarcode(GetTextIndices(text), (Code128Settings)settings);

        public Bitmap Encode(long number) => Encode(number, new Code128Settings());

        // TODO: Do proper (CodeC subtype) number encoding
        public Bitmap Encode(long number, BarcodeSettings settings) => Encode(number.ToString(), settings);

        public Bitmap Encode(byte[] data) => throw new NotAvailableException("byte encoding is not available for Code128 barcodes");

        public Bitmap Encode(byte[] data, BarcodeSettings settings) => throw new NotAvailableException("byte encoding is not available for Code128 barcodes");
        public byte[] EncodeToBytes(byte[] data) => throw new NotAvailableException("byte encoding is not available for Code128 barcodes");

        public byte[] EncodeToBytes(string text) => EncodeToBytes(text, new Code128Settings());

        public byte[] EncodeToBytes(string text, BarcodeSettings settings) => Encode(text, settings).ToByteArray(settings.ImageFormat);

        public byte[] EncodeToBytes(long number) => EncodeToBytes(number, new Code128Settings());

        public byte[] EncodeToBytes(long number, BarcodeSettings settings) => Encode(number, settings).ToByteArray(settings.ImageFormat);

        #region Not Available for Code128

        public byte[] EncodeToBytes(byte[] data, BarcodeSettings settings) => throw new NotAvailableException("byte encoding is not available for Code128 barcodes");
        #endregion

        #endregion
    }
}
