using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.Atm;

public unsafe class PassportReaderV3
{
    private const string DllName = @"AppLibs\TWRPassportAPI.dll";

    public enum TWPassportType
    {
        TW_TYPE_UNKNOW,
        TW_TYPE_PASSPORT, //---0
        TW_TYPE_PASSPORT_FOREIGN, //外国护照
        TW_TYPE_VISA, //签证--3
        TW_TYPE_TAIWAN_PASSPORT, //旧版台胞证-本式
        TW_TYPE_HMP_PASSPORT, //旧版港澳通行证-本式

        TW_TYPE_ID2, //二代证
        TW_TYPE_HVP, //旧版回乡证---卡
        TW_TYPE_MTP_CARD, //新台胞证-卡
        TW_TYPE_HVP_CARD, //新版回乡证-卡
        TW_TYPE_HMP_CARD, //新版港澳通行证
        TW_TYPE_TID, //临时身份证
        TW_TYPE_SZRP, //深圳居住证
        TW_TYPE_GDRP, //广东居住证
        TW_TYPE_DRIVERLICENSE, //驾照
        TW_TYPE_FPRC, //永久居留证---卡
        TW_TYPE_MRTTP_CARD, //大陆居民往来台湾通行证--卡
        TW_TYPE_MRTTP_PASSPORT, //大陆居民往来台湾通行证--本

        TW_TYPE_FR_PERMIT, //外国人居留许可证--本
        TW_TYPE_GATRP, //港澳台居住证
        TW_TYPE_GATRP_G, //港居住证
        TW_TYPE_GATRP_A, //澳居住证
        TW_TYPE_GATRP_T, //台居住证
        TW_TYPE_EAEP, //边境通行证--2
        TW_TYPE_ID1, //一代身份证--4
        TW_TYPE_ID2_ONE, //二代身份证--5
        TW_TYPE_ID2_TWO, //二代身份证反面--6  用5即可
        TW_TYPE_HVP_ONE, //回乡证正面--7
        TW_TYPE_HVP_TWO, //回乡证反面--8   用7即可
        TW_TYPE_COLOBIA, //Colobia---9
        TW_TYPE_MILITARYID, //军官证---10
        TW_TYPE_1DBARCODE, //---11--128
        TW_TYPE_2DBARCODE, //---12 ---QR

        TW_TYPE_APEC, //商务通行证---15
        TW_TYPE_CHNVISA, //中国签注--16
        TW_TYPE_CHNTRAVEL, //中国旅行证--17
        TW_TYPE_TAIWAN_VISA, //台胞证签注--18
        TW_TYPE_APPLICATION_SIGNATURE, //护照申请表签名
        TW_TYPE_HVP_NEW, //新版回乡证
        TW_TYPE_ID_G, //香港身份证
        TW_TYPE_ID_A, //澳门身份证
        TW_TYPE_ID_G_OLD, //香港身份证-旧
        TW_TYPE_AREID_ONE, //迪拜身份证正面
        TW_TYPE_AREID_TWO, //迪拜身份证反面
        TW_TYPE_LAWYER, //律师证
        TW_TYPE_ID_ROU, //罗马尼亚身份证
        TW_TYPE_ID_EUROPE, //欧洲身份证
        TW_TYPE_PDF417,
        TW_TYPE_CHILE_ONE, //智利Card
        TW_TYPE_HUKOU //户口本
        ,
    };

    public struct PassportInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Name_CH;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Name_EN;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Surname;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string GivenName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string MiddleName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string Sex;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string Nationality;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 70)]
        public string BirthPlace;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
        public string BirthDate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 38)]
        public string PassportNo;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 38)]
        public string OtherID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string IssuePlace;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string IssueDate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string ExpiryDate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 72)]
        public string Address;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Remark;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Remark1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Remark2;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string chMRZ1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string chMRZ2;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string chMRZ3;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string PhotoFileName;

        public Boolean bCheckSum;
    }

    public struct IDCardInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 120)]
        public string ENName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string Sex;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Nation;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
        public string Born;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 72)]
        public string Address;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 38)]
        public string IDCardNo;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string GrantDept;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
        public string UserLifeBegin;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
        public string UserLifeEnd;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 38)]
        public string reserved;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string PhotoFileName;
        public uint iType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
        public string pFinger;
        public uint iFingerLen;

        //通行证号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string PassportID;

        //签发次数
        public uint iIssueCount;

        //备注
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string remark;
        public uint ireserved; //保留
    }

    public struct DocData
    {
        public int iType;
        public void* BufferIR;
        public void* BufferColor;
        public void* BufferUV;
        public void* BufferFace;
        public uint FaceType;
        public uint FaceLength;
        public uint FaceWidth;
        public uint FaceHeight;
        public uint Width;
        public uint Height;
        public Boolean bRFID;
        public Boolean bID2;
        public void* BufferReserve;
        public int iReserve1;
        public int iReserve2;

        public PassportInfo tPasInfo;
    }

    public struct DocDataEX
    {
        public int iType;

        //public void* BufferIR;
        //public void* BufferColor;
        //public void* BufferUV;
        //public void* BufferFace;
        public uint FaceWidth;
        public uint FaceHeight;
        public uint Width;
        public uint Height;
        public Boolean bRFID;
        public Boolean bID2;

        //public void* BufferReserve;
        public int iReserve1;
        public int iReserve2;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string ColorFileName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string IRFileName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string UVFileName;
    }

    public struct TWReaderPropertyInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szSerial;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szVer;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szManDate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szServDate;
    }

    //Callback function which can get data of passport
    public delegate void TW_CallbackResultData(void* pContext, DocData image);

    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_RegisterCallbackResultImage",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern int TW_Capture_RegisterCallbackResultImage(
        TW_CallbackResultData callback,
        void* context
    );

    public static TW_CallbackResultData CallbackResultData = null;

    //Callback function which can get data of passportEX
    public delegate void TW_CallbackResultDataEX(
        IntPtr pContext,
        DocDataEX image,
        PassportInfo Info
    );

    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_RegisterCallbackResultImageEX",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern int TW_Capture_RegisterCallbackResultImageEX(
        TW_CallbackResultDataEX callback,
        IntPtr context
    );

    public static TW_CallbackResultDataEX CallbackResultDataEX = null;

    //Callback function which can get data of ID
    public delegate void TW_CallbackResultIDCard(IntPtr pContext, IDCardInfo IDInfo, int nFlag);

    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_RegisterCallbackResultIDCard",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern int TW_Capture_RegisterCallbackResultIDCard(
        TW_CallbackResultIDCard callback,
        IntPtr context
    );

    public static TW_CallbackResultIDCard CallbackResultIDCard = null;

    /**
     * 初始化
     */
    [DllImport(
        DllName,
        EntryPoint = "TW_Initialize",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_Initialize(int iSpeed = 1);

    [DllImport(
        DllName,
        EntryPoint = "TW_SetCapture_Start",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_SetCapture_Start();

    /**
     * 去初始化
     */
    [DllImport(
        DllName,
        EntryPoint = "TW_DeInitialize",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_DeInitialize();

    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_Manual",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_Capture_Manual(int iType = 0);

    [DllImport(
        DllName,
        EntryPoint = "TW_StopCapture",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_StopCapture();

    [DllImport(
        DllName,
        EntryPoint = "TW_InitIDCard",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_InitIDCard(string path, int[] com);

    [DllImport(
        DllName,
        EntryPoint = "TW_SetAutoModel",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_SetAutoModel(bool f);

    [DllImport(
        DllName,
        EntryPoint = "TW_MonitoringDoc",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_MonitoringDoc();

    //打开身份证侦测
    [DllImport(
        DllName,
        EntryPoint = "TW_CreateDetection_ID2Thread",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_CreateDetection_ID2Thread(int iWTime = 2);

    //关闭身份证侦测
    [DllImport(
        DllName,
        EntryPoint = "TW_StopDetection_ID2Thread",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_StopDetection_ID2Thread();

    //关闭/打开
    [DllImport(
        DllName,
        EntryPoint = "TW_SetLEDUV_Status",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_SetLEDUV_Status(Boolean bOpen);

    //得到电子芯片数组数据
    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_GetDGData",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_Capture_GetDGData(byte*[] pData, int[] dataLen);

    //图像平滑处理开关
    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_SetWatemark",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_Capture_SetWatemark(Boolean bOpen);

    //白光开关
    [DllImport(
        DllName,
        EntryPoint = "TW_SetLEDW_Status",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_SetLEDW_Status(Boolean bOpen);

    //RFID开关
    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_SetFunctions",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_Capture_SetFunctions(int iType, int iValue);

    //港澳身份证开关
    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_SupportDocTypeEX",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_Capture_SupportDocTypeEX(Boolean bOpen);

    //beep
    [DllImport(
        DllName,
        EntryPoint = "TW_Controls_Beep",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_Controls_Beep(Boolean bOpen);

    //snap
    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_Snapshot",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_Capture_Snapshot(int iLedKind, byte[] pimg, int* iW, int* iH);

    //GetDevice
    [DllImport(
        DllName,
        EntryPoint = "TW_GetDeviceInfo",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_GetDeviceInfo(IntPtr pInfo);

    //Barcode
    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_Barcode",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_Capture_Barcode(ref byte msg);

    [DllImport(
        DllName,
        EntryPoint = "TW_Capture_LoadCertificate",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl
    )]
    public static extern bool TW_Capture_LoadCertificate(char* chPath);

    private static bool IsIntialized = false;

    public bool IsInitialized()
    {
        return IsIntialized;
    }

    public bool StartEngine()
    {
        bool f = TW_Initialize(2);

        if (f)
        {
            int[] arr = new int[5];
            f = TW_InitIDCard("", arr);
        }

        return f;
    }

    public bool RegisterPassportCallback(
        TW_CallbackResultData onResultPassport,
        TW_CallbackResultIDCard onResultIdCard
    )
    {
        CallbackResultData = onResultPassport;
        CallbackResultIDCard = onResultIdCard;
        var r = TW_Capture_RegisterCallbackResultImage(CallbackResultData, null);
        r = TW_Capture_RegisterCallbackResultIDCard(CallbackResultIDCard, IntPtr.Zero);
        TW_SetLEDUV_Status(false);
        return true;
    }

    public void DeInitialize()
    {
        TW_StopCapture();
        TW_DeInitialize();
        IsIntialized = false;
    }

    public bool ManualRead()
    {
        TW_SetCapture_Start();
        TW_Capture_Manual();
        return true;
    }
}
