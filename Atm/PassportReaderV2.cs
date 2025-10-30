using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.Atm;

public class PassportReaderV2
{
    private const string DllName = @"AppLibs\MttekReaderAPI.dll";

    // API Method Imports
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int MttekReaderStartEngine();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int MttekReaderStopEngine();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int MttekReaderInitialize();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int MttekReaderDeInitialize();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool MttekReaderIsInitialized();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int MttekReaderSetAutoMode(bool autoMode);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool MttekReaderGetAutoMode();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int MttekReaderRegisterPassportCallback(
        PassportCallbackResultData callback,
        IntPtr context
    );

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int MttekReaderRegisterNotifyCallback(
        NotifyCallbackResultData callback,
        IntPtr context
    );

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int MttekReaderManualRead();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MttekReaderGetPortname();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int MttekReaderFastRead(string mrz1, string mrz2, string mrz3);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void MttekReaderGetNotifyConfig(ref NotifyConfig config);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void MttekReaderSetNotifyConfig(ref NotifyConfig config);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int MttekReaderGetScannerInfo(ref ScannerInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void MttekReaderSetDataGroupConfig(ref DataGroupConfig config);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void MttekReaderGetNotifyConfig(ref DataGroupConfig config);

    // Delegates for Callback Functions
    public delegate void PassportCallbackResultData(IntPtr context, ref PassportInfo passportInfo);
    public delegate void NotifyCallbackResultData(
        IntPtr context,
        int notifyId,
        int errCode,
        IntPtr data
    );
    public GCHandle PassportDelegateHandle;
    public GCHandle NotificationDelegateHandle;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DataGroupConfig
    {
        public bool needOCR; // Enable or disable OCR
        public bool needRFID; // Enable or disable RFID
        public bool needMRZ; // Enable or disable MRZ
    }

    // Structures
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MRZInfo
    {
        public int DocumentClass;
        public int DocumentSizeType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
        public string DocumentType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Surname;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string GivenName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string Sex;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string Nationality;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string BirthDate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string BirthDateCheckdigit;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string DocumentNo;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string DocumentNoCheckdigit;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string PersonalNumber;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string IssuePlace;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string IssueDate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string ExpiryDate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string ExpiryDateCheckdigit;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string OptionalData;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string OptionalDataCheckdigit;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string OverallCheckdigit;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] ChineseName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string chMRZ1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string chMRZ2;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string chMRZ3;

        public int totallines;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PassportInfo
    {
        public MRZInfo MRZInfo;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DataGroupInfo
    {
        public IntPtr buf; // Pointer to data buffer
        public int buflen; // Length of the data buffer

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] sha256FromSOD; // SHA-256 hash stored in SOD

        public bool present; // Whether the data group is present
        public bool needread; // Whether the data group needs to be read
        public bool needsavefile; // Whether the data group needs to be saved to a file
        public bool needsavebuf; // Whether the data group buffer needs to be saved

        public int readstatus; // Reading status
        public ulong readtime; // Time taken to read the data group

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] hashCalculated; // Hash calculated from content

        public DataGroupDesc desc; // Description of the data group
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DataGroupDesc
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string datagroupname; // Data group display name

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string efname; // File name inside the passport

        public byte shortfileId; // Short file ID
        public ushort fid; // File ID
        public byte tag; // Tag

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string content; // Description

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string filename; // Output file name
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NotifyConfig
    {
        public ulong ReadTime;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ScannerInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Version;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string OCRVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string SerialNumber;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string ProductInfo;
    }

    // Public Methods Wrapping API
    public bool StartEngine() => MttekReaderStartEngine() == 0;

    public bool StopEngine() => MttekReaderStopEngine() == 0;

    public bool Initialize() => MttekReaderInitialize() == 0;

    public void SetDataConfig(DataGroupConfig config) => MttekReaderSetDataGroupConfig(ref config);

    public bool DeInitialize()
    {
        if (NotificationDelegateHandle.IsAllocated)
        {
            NotificationDelegateHandle.Free();
        }
        if (PassportDelegateHandle.IsAllocated)
        {
            PassportDelegateHandle.Free();
        }
        return MttekReaderDeInitialize() == 0;
    }

    public bool IsInitialized() => MttekReaderIsInitialized();

    public bool SetAutoMode(bool autoMode) => MttekReaderSetAutoMode(autoMode) == 0;

    public bool GetAutoMode() => MttekReaderGetAutoMode();

    private PassportCallbackResultData _passportCallback;

    public bool RegisterPassportCallback(PassportCallbackResultData callback)
    {
        _passportCallback = callback;
        return MttekReaderRegisterPassportCallback(_passportCallback, IntPtr.Zero) == 0;
    }

    private NotifyCallbackResultData _notifyCallbackResult;

    public bool RegisterNotifyCallback(NotifyCallbackResultData callback)
    {
        _notifyCallbackResult = callback;
        return MttekReaderRegisterNotifyCallback(_notifyCallbackResult, IntPtr.Zero) == 0;
    }

    public bool ManualRead() => MttekReaderManualRead() == 0;

    public string GetPortname()
    {
        IntPtr ptr = MttekReaderGetPortname();
        return Marshal.PtrToStringAnsi(ptr);
    }

    public PassportInfo GetPassportInfoFromMRZ(string mrz1, string mrz2, string mrz3)
    {
        Console.WriteLine($"MRZ Line 1: {mrz1}");
        Console.WriteLine($"MRZ Line 2: {mrz2}");
        if (!string.IsNullOrWhiteSpace(mrz3))
            Console.WriteLine($"MRZ Line 3: {mrz3}");

        PassportInfo passportInfo = new PassportInfo() { MRZInfo = new MRZInfo() };

        if (mrz1.StartsWith("P<")) // TD3 (passport)
        {
            string[] nameParts = mrz1.Substring(2).Split(new[] { "<<" }, StringSplitOptions.None);
            passportInfo.MRZInfo.Surname = nameParts[0].Replace("<", " ").Trim();
            passportInfo.MRZInfo.GivenName =
                nameParts.Length > 1 ? nameParts[1].Replace("<", " ").Trim() : "";

            passportInfo.MRZInfo.DocumentNo = mrz2.Substring(0, 9).Replace("<", "").Trim();
            passportInfo.MRZInfo.Nationality = mrz2.Substring(10, 3).Replace("<", "").Trim();
            passportInfo.MRZInfo.BirthDate = mrz2.Substring(13, 6).Trim(); // Format: YYMMDD
            passportInfo.MRZInfo.Sex = mrz2.Substring(20, 1).Trim();
            passportInfo.MRZInfo.ExpiryDate = mrz2.Substring(21, 6).Trim(); // Format: YYMMDD
        }
        return passportInfo;
    }

    public bool FastRead(string mrz1, string mrz2, string mrz3)
    {
        return MttekReaderFastRead(mrz1, mrz2, mrz3) == 0;
    }

    public void GetNotifyConfig(ref NotifyConfig config) => MttekReaderGetNotifyConfig(ref config);

    public void SetNotifyConfig(ref NotifyConfig config) => MttekReaderSetNotifyConfig(ref config);

    public bool GetScannerInfo(ref ScannerInfo info) => MttekReaderGetScannerInfo(ref info) == 0;

    // Helper Methods
    public static string DisplayScannerInfo(ScannerInfo info)
    {
        return $"Version: {info.Version}, OCR Version: {info.OCRVersion}, "
            + $"Serial Number: {info.SerialNumber}, Product Info: {info.ProductInfo}";
    }
}
