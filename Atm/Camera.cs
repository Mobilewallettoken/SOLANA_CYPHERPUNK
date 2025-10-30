using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace MobileWallet.Desktop.Atm
{
    public class Camera
    {
        private const string DllName = @"AppLibs\DevCapture.dll";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDeviceCount();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDeviceName(int index, byte[] nbuf);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetResolutionCount(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetResolution(int R_index, ref int width, ref int height);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetResolution(int stillIndex, int previewIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenDevice(
            int index,
            int width,
            int height,
            IntPtr mhwnd,
            Boolean isDisplay
        );

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseDevice();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CaptureFromPreview(byte[] Imagepath, int type);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCutType(int cutType);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetFormatType(int format);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDpi(int type, int val);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetColorType(int type);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ShowCameraSettingWindow();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RecogQrBarCode(int type, byte[] result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetRotateAngle(int angle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BestSize();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TrueSize();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ZoomIn();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ZoomOut();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ManualFocus();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDelBgColor(int flag);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDelBlackEdge(int flag);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AddImagePath(byte[] Imagepath);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MergeImages(byte[] desPath, int direction, int sp);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AddImageFileToPDF(byte[] Imagepath);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CombineToPDF(byte[] desPath);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadCard();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GeCardInfo(int index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetJpgQuality(int val);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int StartREC(byte[] path, int fps);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int StopREC();

        bool isOpenCam = false;

        public int OpenTheCamera()
        {
            if (!Global.UseHardware)
            {
                return 1;
            }
            CloseDevice();
            int iRest = OpenDevice(0, 1920, 1080, (IntPtr)0, false);
            if (iRest != 0)
            {
                return 0;
            }
            else
            {
                isOpenCam = true;
                return 1;
            }
        }

        public void captureImage()
        {
            string fFormatStr = ".jpg";

            fFormatStr = ".png";
            string ImgName = DateTime.Now.ToString("yyyyMMddhhmmssfff");
            string imgpath = "D:\\" + ImgName + fFormatStr;
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            byte[] pBuf = System.Text.Encoding.GetEncoding("gb2312").GetBytes(imgpath);
            IntPtr namePtr = CaptureFromPreview(pBuf, 0);
            imgpath = Marshal.PtrToStringAnsi(namePtr);

            //Show in image list
            if (File.Exists(imgpath))
            {
                string previewImgPath = imgpath;
            }

            //string fFormatStr = ".png";
            //string ImgName = DateTime.Now.ToString("yyyyMMddhhmmssfff");
            //string imgDirectoryName = "Capture";
            //string imgpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imgDirectoryName, ImgName + fFormatStr);
            //System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //byte[] pBuf = System.Text.Encoding.GetEncoding("gb2312").GetBytes(imgpath);
            //IntPtr namePtr = CaptureFromPreview(pBuf, 0);
            //imgpath = Marshal.PtrToStringAnsi(namePtr);
            //// Show in image list
            //if (File.Exists(imgpath))
            //{
            //    string previewImgPath = imgpath;
            //    MessageBox.Show(previewImgPath);
            //}
        }

        public int CloseTheCamera()
        {
            try
            {
                CloseDevice();
                isOpenCam = false;
                return 1;
            }
            catch (Exception e)
            {
                App.AppLogger.Error(e,e.Message);
                return 0;
            }
        }
    }
}
