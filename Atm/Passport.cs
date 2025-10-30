using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace MobileWallet.Desktop.Atm
{
    public class Passport
    {
        // Read for errors here
        public List<string> errors = new List<string> ();
        
        // Read for fields here
        public Dictionary<string, object> dynamicObject { get; set; }
        
        private DispatcherTimer timer;
        delegate int InitIDCard([MarshalAs(UnmanagedType.LPWStr)] String userID, int nType, [MarshalAs(UnmanagedType.LPWStr)] String lpDirectory);
        delegate int GetFieldNameEx(int nAttribute, int nIndex, [MarshalAs(UnmanagedType.LPWStr)] String ArrBuffer, ref int nBufferLen);
        delegate int GetRecogResultEx(int nAttribute, int nIndex, [MarshalAs(UnmanagedType.LPWStr)] String lpBuffer, ref int nBufferLen);
        delegate int GetCurrentDevice([MarshalAs(UnmanagedType.LPWStr)] String ArrDeviceName, int nLength);
        delegate void GetVersionInfo([MarshalAs(UnmanagedType.LPWStr)] String ArrVersion, int nLength);
        delegate bool CheckDeviceOnline();
        delegate void ResetIDCardID();
        delegate int AddIDCardID(int nMainID, int[] nSubID, int nSubIdCount);
        delegate int DetectDocument();
        delegate void SetRecogDG(int nDG);
        delegate void SetRecogVIZ(bool bRecogVIZ);
        delegate void SetSaveImageType(int nImageType);
        delegate int SetConfigByFile([MarshalAs(UnmanagedType.LPWStr)] String strConfigFile);
        delegate void FreeIDCard();
        delegate int GetDeviceSN([MarshalAs(UnmanagedType.LPWStr)] String ArrSn, int nLength);
        delegate int SaveImageEx([MarshalAs(UnmanagedType.LPWStr)] String lpFileName, int nType);
        delegate int AutoProcessIDCard(ref int nCardType);
        delegate int GetBarcodeCount();
        delegate int GetBarcodeRecogResult(int nIndex, [MarshalAs(UnmanagedType.LPWStr)] String lpBuffer, ref int nBufferLen, [MarshalAs(UnmanagedType.LPWStr)] String resultType, ref int nTypeLenth);
        delegate int RecogBarCode([MarshalAs(UnmanagedType.LPWStr)] String lpBuffer, ref int nLen);
        delegate int RecogCellPhoneBarCode();
        delegate int GetGrabSignalType();

        InitIDCard pInitIDCard;
        GetFieldNameEx pGetFieldNameEx;
        GetCurrentDevice pGetCurrentDevice;
        GetVersionInfo pGetVersionInfo;
        CheckDeviceOnline pCheckDeviceOnline;
        ResetIDCardID pResetIDCardID;
        AddIDCardID pAddIDCardID;
        DetectDocument pDetectDocument;
        GetGrabSignalType pGetGrabSignalType;
        SetRecogDG pSetRecogDG;
        SetRecogVIZ pSetRecogVIZ;
        SetConfigByFile pSetConfigByFile;
        GetBarcodeCount pGetBarcodeCount;
        GetBarcodeRecogResult pGetBarcodeRecogResult;
        SetSaveImageType pSetSaveImageType;
        FreeIDCard pFreeIDCard;
        GetDeviceSN pGetDeviceSN;
        SaveImageEx pSaveImageEx;
        AutoProcessIDCard pAutoProcessIDCard;
        GetRecogResultEx pGetRecogResultEx;
        RecogBarCode pRecogBarCode;
        RecogCellPhoneBarCode pRecogCellPhoneBarCode;

        delegate int SDT_OpenPort(int iPort);
        delegate int SDT_ClosePort(int iPort);
        delegate int SDT_StartFindIDCard(int iPort, ref byte pRAPDU, int iIfOpen);
        delegate int SDT_SelectIDCard(int iPort, ref byte pRAPDU, int iIfOpen);
        delegate int SDT_ReadBaseMsg(int iPort, ref byte pucCHMsg, ref int puiCHMsgLen, ref byte pucPHMsg, ref int puiPHMsgLen, int iIfOpen);
        delegate int SDT_ReadNewAppMsg(int iPort, ref byte pucAppMsg, ref int puiAppMsgLen, int iIfOpen);
        delegate int GetBmp(string filename, int nType);
        delegate int SDT_GetSAMIDToStr(int iPortID, ref byte pcSAMIDStr, int iIfOpen);
        delegate int SDT_GetSAMID(int iPortID, ref byte pcSAMID, int iIfOpen);

        SDT_OpenPort pSDT_OpenPort;
        SDT_ClosePort pSDT_ClosePort;
        SDT_StartFindIDCard pSDT_StartFindIDCard;
        SDT_SelectIDCard pSDT_SelectIDCard;
        SDT_ReadBaseMsg pSDT_ReadBaseMsg;
        SDT_ReadNewAppMsg pSDT_ReadNewAppMsg;
        GetBmp pGetBmp;
        SDT_GetSAMIDToStr pSDT_GetSAMIDToStr;
        SDT_GetSAMID pSDT_GetSAMID;

        bool m_bLoad = false;
        //int m_nOpenPort = 0;
        bool m_bDevice = false;
        bool m_bIsIDCardLoaded = false;
        bool bEnd = false;
        int ncount = 0;
        int nSaveimg = 1;
        string m_strLogPath = AppDomain.CurrentDomain.BaseDirectory;

        public static string currentAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string currentAssemblyParentPath = Path.GetDirectoryName(currentAssemblyPath);
        public static string dllPath = currentAssemblyPath + "";
        public static string configPath = currentAssemblyPath + "\\IDCardConfig.ini";

        

        public Passport() 
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Adjust the interval as needed
            timer.Tick += new System.EventHandler(this.AutoClassAndRecognize);
            // btnLoadKernal_Click();

            // Start the timer
            //timer.Start();
        }

        public static class MyDll
        {
            [DllImport("Kernel32.dll", SetLastError = true)]
            public static extern IntPtr LoadLibrary(string path);

            [DllImport("Kernel32.dll")]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("Kernel32.dll")]
            private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            public static Delegate LoadFunction<T>(IntPtr hModule, string functionName)
            {
                IntPtr functionAddress = GetProcAddress(hModule, functionName);
                if (functionAddress.ToInt64() == 0)
                {
                    return null;
                }
                return Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(T));
            }
        }

        public bool LoadDllIDCard()
        {
            string DllPath = dllPath; // set path here
            DllPath += "\\IDCard.dll";

            IntPtr hModule = MyDll.LoadLibrary(@DllPath);
            if (hModule.ToInt64() == 0)
            {
                int errorCode = Marshal.GetLastWin32Error();
                MessageBox.Show("Load " + DllPath + " failed.");
                errors.Add("Load " + DllPath + " failed.");
                return false;
            }

            pInitIDCard = (InitIDCard)MyDll.LoadFunction<InitIDCard>(hModule, "InitIDCard");
            pGetFieldNameEx = (GetFieldNameEx)MyDll.LoadFunction<GetFieldNameEx>(hModule, "GetFieldNameEx");
            pGetCurrentDevice = (GetCurrentDevice)MyDll.LoadFunction<GetCurrentDevice>(hModule, "GetCurrentDevice");
            pGetVersionInfo = (GetVersionInfo)MyDll.LoadFunction<GetVersionInfo>(hModule, "GetVersionInfo");
            pCheckDeviceOnline = (CheckDeviceOnline)MyDll.LoadFunction<CheckDeviceOnline>(hModule, "CheckDeviceOnline");
            pResetIDCardID = (ResetIDCardID)MyDll.LoadFunction<ResetIDCardID>(hModule, "ResetIDCardID");
            pAddIDCardID = (AddIDCardID)MyDll.LoadFunction<AddIDCardID>(hModule, "AddIDCardID");
            pDetectDocument = (DetectDocument)MyDll.LoadFunction<DetectDocument>(hModule, "DetectDocument");
            pGetGrabSignalType = (GetGrabSignalType)MyDll.LoadFunction<GetGrabSignalType>(hModule, "GetGrabSignalType");
            pSetRecogVIZ = (SetRecogVIZ)MyDll.LoadFunction<SetRecogVIZ>(hModule, "SetRecogVIZ");
            pSetConfigByFile = (SetConfigByFile)MyDll.LoadFunction<SetConfigByFile>(hModule, "SetConfigByFile");
            pSetRecogDG = (SetRecogDG)MyDll.LoadFunction<SetRecogDG>(hModule, "SetRecogDG");
            pSetSaveImageType = (SetSaveImageType)MyDll.LoadFunction<SetSaveImageType>(hModule, "SetSaveImageType");
            pGetDeviceSN = (GetDeviceSN)MyDll.LoadFunction<GetDeviceSN>(hModule, "GetDeviceSN");
            pSaveImageEx = (SaveImageEx)MyDll.LoadFunction<SaveImageEx>(hModule, "SaveImageEx");
            pAutoProcessIDCard = (AutoProcessIDCard)MyDll.LoadFunction<AutoProcessIDCard>(hModule, "AutoProcessIDCard");
            pGetBarcodeCount = (GetBarcodeCount)MyDll.LoadFunction<GetBarcodeCount>(hModule, "GetBarcodeCount");
            pGetBarcodeRecogResult = (GetBarcodeRecogResult)MyDll.LoadFunction<GetBarcodeRecogResult>(hModule, "GetBarcodeRecogResult");
            pRecogBarCode = (RecogBarCode)MyDll.LoadFunction<RecogBarCode>(hModule, "RecogBarCode");
            pRecogCellPhoneBarCode = (RecogCellPhoneBarCode)MyDll.LoadFunction<RecogCellPhoneBarCode>(hModule, "RecogCellPhoneBarCode");
            pGetRecogResultEx = (GetRecogResultEx)MyDll.LoadFunction<GetRecogResultEx>(hModule, "GetRecogResultEx");
            pFreeIDCard = (FreeIDCard)MyDll.LoadFunction<FreeIDCard>(hModule, "FreeIDCard");

            if (pInitIDCard == null || pGetCurrentDevice == null || pGetVersionInfo == null || pCheckDeviceOnline == null ||
                pResetIDCardID == null || pAddIDCardID == null || pDetectDocument == null || pGetDeviceSN == null ||
                pSaveImageEx == null || pFreeIDCard == null || pAutoProcessIDCard == null || pGetRecogResultEx == null ||
                pGetFieldNameEx == null || pSetRecogDG == null || pSetSaveImageType == null || pSetRecogVIZ == null ||
                pSetConfigByFile == null || pGetBarcodeCount == null || pGetBarcodeRecogResult == null || pRecogBarCode == null || pRecogCellPhoneBarCode == null || pGetGrabSignalType == null)
            {
                MyDll.FreeLibrary(hModule);
                MessageBox.Show("Export IDCard.dll API failed");
                errors.Add("Export IDCard.dll API failed");
                return false;
            }
            return true;
        }

        public void btnLoadKernal_Click()
        {
            if (!Global.UseHardware)
            {
                return;
            }
            string testString = "";
            //Load Kernel

            char[] arr_UserID = "66915733240645127938".ToCharArray();
            string UserID = new string(arr_UserID);
            char[] arr_DllPath = dllPath.ToCharArray();//dll path

            if (!LoadDllIDCard())
            {
                return;
            }
            int nRet;
            int nConFig;
            string strTextConFigPath = configPath;
            nRet = pInitIDCard(UserID, 1, dllPath);//dll directory
            if (nRet != 0)
            {
                switch (nRet)
                {
                    case 1:
                        errors.Add("UserID error.");
                        testString = "UserID error.\n";
                        break;
                    case 2:
                        errors.Add("Device initialization failed.");
                        testString = "Device initialization failed.\n";
                        break;
                    case 3:
                        errors.Add("Failed to initialize the certificate core.");
                        testString = "Failed to initialize the certificate core.\n";
                        break;
                    case 4:
                        errors.Add("The authorization file was not found.");
                        testString = "The authorization file was not found.\n";
                        break;
                    case 5:
                        errors.Add("Failed to load template file.");
                        testString = "Failed to load template file.\n";
                        break;
                    case 6:
                        errors.Add("Failed to initialize card reader.");
                        testString = "Failed to initialize card reader.\n";
                        break;
                    default:
                        break;
                }
                return;
            }
            nConFig = pSetConfigByFile(strTextConFigPath);
            m_bIsIDCardLoaded = true;
            testString = "The recognition engine is loaded successfully.\r\n";

            LoadDLL();
            if (m_bLoad)
            {
                testString += "The stdapi.dll is loaded successfully.\r\n";
            }
            else
            {
                errors.Add("The stdapi.dll is loaded failed.");
                testString += "The stdapi.dll is loaded failed.\r\n";
                return;
            }
            btnGetDeviceRFIDSN_Click();
            m_strLogPath = AppDomain.CurrentDomain.BaseDirectory + "\\InterFaceLog.log";

            bEnd = false;
            timer.Start();

        }

        private void btnGetDeviceRFIDSN_Click()
        {
            string testString = "";
            if (!m_bLoad)
            {
                testString = "Please successful loading recognition engine.！";
                return;
            }

            /*******************************************************************/


            byte[] szHexBuf = new byte[64];
            byte[] sHexBufSrc = new byte[129];

        }

        private void LoadDLL()
        {
            if (m_bLoad == false)
            {
                if (!LoadDllapi())
                {
                    return;
                }
                else
                {
                    m_bLoad = true;
                }
            }

            if (m_bLoad != true)
            {
                errors.Add("Open port failed！");
            }
        }

        public bool LoadDllapi()
        {
            string DllPath = dllPath;
            DllPath += "\\sdtapi.dll";

            IntPtr hModuleadi = MyDll.LoadLibrary(@DllPath);
            if (hModuleadi.ToInt64() == 0)
            {
                errors.Add("Load sdtapi.dll failed");
                //MessageBox.Show("Load sdtapi.dll failed \n");
                return false;
            }
            pSDT_OpenPort = (SDT_OpenPort)MyDll.LoadFunction<SDT_OpenPort>(hModuleadi, "SDT_OpenPort");
            pSDT_ClosePort = (SDT_ClosePort)MyDll.LoadFunction<SDT_ClosePort>(hModuleadi, "SDT_ClosePort");
            pSDT_StartFindIDCard = (SDT_StartFindIDCard)MyDll.LoadFunction<SDT_StartFindIDCard>(hModuleadi, "SDT_StartFindIDCard");
            pSDT_SelectIDCard = (SDT_SelectIDCard)MyDll.LoadFunction<SDT_SelectIDCard>(hModuleadi, "SDT_SelectIDCard");
            pSDT_ReadBaseMsg = (SDT_ReadBaseMsg)MyDll.LoadFunction<SDT_ReadBaseMsg>(hModuleadi, "SDT_ReadBaseMsg");
            pSDT_ReadNewAppMsg = (SDT_ReadNewAppMsg)MyDll.LoadFunction<SDT_ReadNewAppMsg>(hModuleadi, "SDT_ReadNewAppMsg");
            pSDT_GetSAMIDToStr = (SDT_GetSAMIDToStr)MyDll.LoadFunction<SDT_GetSAMIDToStr>(hModuleadi, "SDT_GetSAMIDToStr");
            pSDT_GetSAMID = (SDT_GetSAMID)MyDll.LoadFunction<SDT_GetSAMID>(hModuleadi, "SDT_GetSAMID");

            DllPath = dllPath + "\\WltRS.dll";

            IntPtr hModuleWltRS = MyDll.LoadLibrary(@DllPath);
            if (hModuleWltRS.ToInt64() == 0)
            {
                errors.Add("Load WltRS.dll failed");
                MessageBox.Show("Load WltRS.dll failed ");
                return false;
            }
            pGetBmp = (GetBmp)MyDll.LoadFunction<GetBmp>(hModuleWltRS, "GetBmp");


            if (pSDT_OpenPort == null || pSDT_ClosePort == null || pSDT_StartFindIDCard == null || pSDT_SelectIDCard == null || pSDT_ReadBaseMsg == null ||
                pSDT_ReadNewAppMsg == null || pSDT_GetSAMIDToStr == null || pSDT_GetSAMID == null)
            {
                MyDll.FreeLibrary(hModuleadi);
                errors.Add("Export sdtapi.dll API failed.");
                MessageBox.Show("Export sdtapi.dll API failed.");
                return false;
            }

            if (pGetBmp == null)
            {
                MyDll.FreeLibrary(hModuleWltRS);
                errors.Add("Export sdtapi.dll API failed.");
                MessageBox.Show("Export sdtapi.dll API failed. ");
                return false;
            }

            return true;
        }

        int AscllToInt(int nVal)
        {
            int n = 0;
            if ((nVal + 48) >= '0' && (nVal + 48) <= '9')
                n = nVal + 48;
            //if ((nVal+97) >= 'a'&&(nVal+97) <= 'f')
            //	n = nVal + 97;
            if ((nVal + 55) >= 'A' && (nVal + 55) <= 'F')
                n = nVal + 55;
            return n;
        }

        private void btnCheckDeviceOnLine_Click(object sender, EventArgs e)
        {
            string test = "";
            if (!m_bIsIDCardLoaded)
            {
                test = "Please successful loading recognition engine.";
                return;
            }

            bool bRet = pCheckDeviceOnline();
            if (bRet)
            {
                test = "The device is online.";
                m_bDevice = true;
            }
            else
            {
                test = "The device is not offline.";
                m_bDevice = false;
                ncount++;
            }

        }

        private void AutoClassAndRecognize(object sender, EventArgs e)
        {
            string test = "";
            if (!m_bIsIDCardLoaded)
                return;
            if (bEnd)
                return;
            bEnd = true;
            int nRet = 0;
            btnCheckDeviceOnLine_Click(sender, e);
            if (!m_bDevice)
            {
                bEnd = false;
                return;
            }

            nRet = pDetectDocument();
            if (nRet != 0)
                Console.WriteLine(nRet);
            if (nRet == 1)
            {
                //get param
                int nDG = 6150;


                int nSaveImage = 3;


                bool bVIZ = true;

                pSetRecogDG(nDG);
                pSetSaveImageType(nSaveImage);
                pSetRecogVIZ(bVIZ);

                int nCardType = 0;
                nRet = pAutoProcessIDCard(ref nCardType);

                dynamicObject = new Dictionary<string, object>();

                if (nRet > 0)
                {
                    GetContent(dynamicObject);
                    //show DG info                 
                    if (1 == nCardType)
                        GetDGContent();
                    else
                        test = "no any DG info";
                }
                else
                {
                    errors.Add("fail to read the card");
                    bEnd = false;
                    return;
                }
                //SaveImage 
                string currentAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string currentAssemblyParentPath = Path.GetDirectoryName(currentAssemblyPath);

                string strImagePath = currentAssemblyParentPath;
                strImagePath += "\\SampleDemo.jpg";
                nRet = pSaveImageEx(strImagePath, nSaveImage);
                nSaveimg = -nRet;

                dynamicObject["image"] = strImagePath;
            }
            else if (nRet == 3)
            {
                nRet = pRecogCellPhoneBarCode();
                if (nRet > 0)
                {
                    test = "Success to read the card\r\n";
                    int codeNumber = pGetBarcodeCount();
                    for (int i = 0; i < codeNumber; i++)
                    {
                        int MAX_CH_NUM = 512;
                        int nValueLen = MAX_CH_NUM * sizeof(byte);
                        int nTypeLen = MAX_CH_NUM * sizeof(byte);
                        String barcodeValue = new String('\0', MAX_CH_NUM);
                        String barcodeType = new String('\0', MAX_CH_NUM);

                        int rRet = pGetBarcodeRecogResult(i, barcodeValue, ref nValueLen, barcodeType, ref nTypeLen);
                        string type = barcodeType.Replace("\0", "");
                        string vlaue = barcodeValue.Replace("\0", "");
                        test += type;

                        test += vlaue;
                        test += "\r\n";

                    }
                }
            }
            bEnd = false;
        }

        JObject jo;
        private Dictionary<string, object> GetContent(Dictionary<string, object> dynamicObject)
        {
            string test = "";
            int MAX_CH_NUM = 128;
            int nBufLen = MAX_CH_NUM * sizeof(byte);
            test = "Success to read the card\r\n";
            jo = new JObject();
            for (int i = 0; ; i++)
            {
                String cArrFieldValue = new String('\0', MAX_CH_NUM);
                String cArrFieldName = new String('\0', MAX_CH_NUM);
                int nRet = pGetRecogResultEx(1, i, cArrFieldValue, ref nBufLen);
                if (nRet == 3)
                    break;
                nBufLen = MAX_CH_NUM * sizeof(byte);

                pGetFieldNameEx(1, i, cArrFieldName, ref nBufLen);
                nBufLen = MAX_CH_NUM * sizeof(byte);
                jo.Add(cArrFieldName, cArrFieldValue);
                dynamicObject[cArrFieldName.TrimEnd('\0')] = cArrFieldValue.TrimEnd('\0');
                test += cArrFieldName;
                test += ":";
                test += cArrFieldValue;
                test += "\r\n";
            }
            return dynamicObject;
        }

        private void GetDGContent()
        {
            string test = "";
            test = "Success to read the card\r\n";
            int MAX_CH_NUM = 128;
            int nBufLen = 42000;
            for (int j = 0; ; j++)
            {
                String ArrFieldValue = new String('\0', nBufLen);
                String ArrFieldName = new String('\0', MAX_CH_NUM);

                int nResu = pGetRecogResultEx(0, j, ArrFieldValue, ref nBufLen);
                if (nResu == 3)
                    break;
                nBufLen = MAX_CH_NUM * sizeof(byte);
                pGetFieldNameEx(0, j, ArrFieldName, ref nBufLen);
                test += ArrFieldName;
                test += ":";
                test += ArrFieldValue;
                test += "\r\n";
                nBufLen = 42000;
            }
        }
    }
}
