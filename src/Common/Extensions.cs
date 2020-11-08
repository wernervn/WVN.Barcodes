using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WVN.Barcodes.Common
{
	internal static class Extensions
	{
		public static byte[] ToByteArray(this Bitmap bmp, ImageFormat format)
		{
			byte[] arr;
			using (var stream = new MemoryStream())
			{
				bmp.Save(stream, format);
				arr = new byte[stream.Length];
				Array.Copy(stream.ToArray(), arr, stream.Length);
			}
			return arr;
		}
	}
}
