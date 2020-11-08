using System;
using System.Drawing;

namespace WVN.Barcodes
{
    public interface IBarcode : IDisposable
	{
		Bitmap Encode(string text);
		Bitmap Encode(string text, BarcodeSettings settings);

		Bitmap Encode(byte[] data);
		Bitmap Encode(byte[] data, BarcodeSettings settings);

		Bitmap Encode(long number);
		Bitmap Encode(long number, BarcodeSettings settings);

		byte[] EncodeToBytes(string text);
		byte[] EncodeToBytes(string text, BarcodeSettings settings);

		byte[] EncodeToBytes(byte[] data);
		byte[] EncodeToBytes(byte[] data, BarcodeSettings settings);

		byte[] EncodeToBytes(long number);
		byte[] EncodeToBytes(long number, BarcodeSettings settings);
	}
}
