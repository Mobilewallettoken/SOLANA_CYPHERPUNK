using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using ZXing;
using ZXing.Common;

namespace MobileWallet.Desktop.Extensions;

public static class DeepCopyExtensions
{
    public static List<T>? DeepCopy<T>(this List<T> source)
    {
        var serialized = JsonConvert.SerializeObject(source);
        return JsonConvert.DeserializeObject<List<T>>(serialized);
    }
}

public static class MatExtensions
{
    public static string? ToQrCode(this Mat mat)
    {
        // Convert BitmapSource to Bitmap
        Bitmap bitmap = mat.ToBitmap();
        // Create a barcode reader instance
        var _barcodeReader = new BarcodeReaderGeneric
        {
            Options = { PossibleFormats = [BarcodeFormat.QR_CODE] },
        };
        // Decode the QR code
        var result = _barcodeReader.Decode(mat.ToImage<Bgr, byte>());
        // Dispose of the Bitmap to free memory
        bitmap.Dispose();
        // Return the result text, or null if no QR code was found
        return result?.Text;
    }

    public static BitmapSource ToBitmapSource(this Bitmap mat)
    {
        var bitmap = mat;
        IntPtr hBitmap = bitmap.GetHbitmap(); // Create HBitmap
        try
        {
            var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
            return source;
        }
        finally
        {
            DeleteObject(hBitmap);
            bitmap.Dispose();
        }
    }

    public static BitmapSource ToBitMapSource(this BitMatrix bitMatrix)
    {
        int width = bitMatrix.Width;
        int height = bitMatrix.Height;

        // Create a pixel array (1-bit depth, black and white)
        byte[] pixels = new byte[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // BitMatrix returns true for black and false for white
                pixels[y * width + x] = (byte)(bitMatrix[x, y] ? 0 : 255); // 0 = Black, 255 = White
            }
        }

        // Create BitmapSource
        var bitmapSource = BitmapSource.Create(
            width,
            height,
            96, // DPI X
            96, // DPI Y
            PixelFormats.Gray8, // 8-bit grayscale
            null, // No palette
            pixels,
            width // Stride (width * bytes per pixel)
        );

        return bitmapSource;
    }

    public static BitmapSource ToBitmapSource(this Mat mat)
    {
        var bitmap = mat.ToBitmap();
        IntPtr hBitmap = bitmap.GetHbitmap(); // Create HBitmap

        try
        {
            var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
            return source;
        }
        finally
        {
            DeleteObject(hBitmap);
            bitmap.Dispose();
        }
    }

    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    [return: System.Runtime.InteropServices.MarshalAs(
        System.Runtime.InteropServices.UnmanagedType.Bool
    )]
    private static extern bool DeleteObject(IntPtr hObject);
}
