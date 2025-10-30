// Decompiled with JetBrains decompiler
// Type: MPOST.IAcceptor_API
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("4922B689-4FB6-42e2-A8B8-09F07AC00E90")]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  public interface IAcceptor_API
  {
    [DispId(1)]
    bool CapAdvBookmark { get; }

    [DispId(2)]
    bool CapApplicationID { get; }

    [DispId(3)]
    bool CapApplicationPN { get; }

    [DispId(4)]
    bool CapAssetNumber { get; }

    [DispId(5)]
    bool CapAudit { get; }

    [DispId(6)]
    bool CapBarCodes { get; }

    [DispId(7)]
    bool CapBarCodesExt { get; }

    [DispId(8)]
    bool CapBNFStatus { get; }

    [DispId(9)]
    bool CapBookmark { get; }

    [DispId(10)]
    bool CapBootPN { get; }

    [DispId(11)]
    bool CapCalibrate { get; }

    [DispId(12)]
    bool CapCashBoxTotal { get; }

    [DispId(13)]
    bool CapCouponExt { get; }

    [DispId(14)]
    bool CapDevicePaused { get; }

    [DispId(15)]
    bool CapDeviceSoftReset { get; }

    [DispId(16)]
    bool CapDeviceType { get; }

    [DispId(17)]
    bool CapDeviceResets { get; }

    [DispId(18)]
    bool CapDeviceSerialNumber { get; }

    [DispId(19)]
    bool CapEasitrax { get; }

    [DispId(20)]
    bool CapEscrowTimeout { get; }

    [DispId(21)]
    bool CapFlashDownload { get; }

    [DispId(22)]
    bool CapNoPush { get; }

    [DispId(23)]
    bool CapNoteRetrieved { get; }

    [DispId(24)]
    bool CapOrientationExt { get; }

    [DispId(25)]
    bool CapPupExt { get; }

    [DispId(26)]
    bool CapSetBezel { get; }

    [DispId(27)]
    bool CapTestDoc { get; }

    [DispId(28)]
    bool CapVariantID { get; }

    [DispId(29)]
    bool CapVariantPN { get; }

    [DispId(30)]
    bool CapClearAudit { get; }

    [DispId(40)]
    string ApplicationID { get; }

    [DispId(41)]
    string ApplicationPN { get; }

    [DispId(42)]
    string AssetNumber { get; set; }

    [DispId(43)]
    int[] AuditLifeTimeTotals { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)] get; }

    [DispId(44)]
    int[] AuditPerformance { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)] get; }

    [DispId(45)]
    int[] AuditQP { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)] get; }

    [DispId(46)]
    bool AutoStack { get; set; }

    [DispId(47)]
    string BarCode { get; }

    [DispId(48)]
    Bill Bill { get; }

    [DispId(49)]
    Bill[] BillTypes { get; }

    [DispId(50)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BOOL)]
    bool[] GetBillTypeEnables();

    [DispId(51)]
    void SetBillTypeEnables([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BOOL)] ref bool[] billTypeEnables);

    [DispId(52)]
    Bill[] BillValues { get; }

    [DispId(53)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BOOL)]
    bool[] GetBillValueEnables();

    [DispId(54)]
    void SetBillValueEnables([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BOOL)] ref bool[] billValueEnables);

    [DispId(55)]
    BNFStatus BNFStatus { get; }

    [DispId(56)]
    string BootPN { get; }

    [DispId(57)]
    bool CashBoxAttached { get; }

    [DispId(58)]
    bool CashBoxFull { get; }

    [DispId(59)]
    int CashBoxTotal { get; }

    [DispId(60)]
    bool Connected { get; }

    [DispId(61)]
    Coupon Coupon { get; }

    [DispId(62)]
    bool DebugLog { get; set; }

    [DispId(63)]
    string DebugLogPath { get; set; }

    [DispId(64)]
    bool DeviceBusy { get; }

    [DispId(65)]
    int DeviceCRC { get; }

    [DispId(66)]
    bool DeviceFailure { get; }

    [DispId(67)]
    bool DeviceJammed { get; }

    [DispId(68)]
    int DeviceModel { get; }

    [DispId(69)]
    bool DevicePaused { get; }

    [DispId(70)]
    string DevicePortName { get; }

    [DispId(71)]
    PowerUp DevicePowerUp { get; }

    [DispId(72)]
    int DeviceResets { get; }

    [DispId(73)]
    int DeviceRevision { get; }

    [DispId(74)]
    string DeviceSerialNumber { get; }

    [DispId(75)]
    bool DeviceStalled { get; }

    [DispId(76)]
    State DeviceState { get; }

    [DispId(77)]
    string DeviceType { get; }

    [DispId(78)]
    DocumentType DocType { get; }

    [DispId(79)]
    int DownloadTimeout { get; set; }

    [DispId(80)]
    int TransactionTimeout { get; set; }

    [DispId(81)]
    int DisconnectTimeout { get; set; }

    [DispId(82)]
    bool EnableAcceptance { get; set; }

    [DispId(83)]
    bool EnableBarCodes { get; set; }

    [DispId(84)]
    bool EnableBookmarks { get; set; }

    [DispId(85)]
    bool EnableCouponExt { get; set; }

    [DispId(86)]
    bool EnableNoPush { get; set; }

    [DispId(87)]
    Orientation EscrowOrientation { get; }

    [DispId(88)]
    bool HighSecurity { get; set; }

    [DispId(89)]
    OrientationControl OrientationCtl { get; set; }

    [DispId(90)]
    OrientationControl OrientationCtlExt { get; set; }

    [DispId(91)]
    string[] VariantNames { get; }

    [DispId(92)]
    string VariantID { get; }

    [DispId(93)]
    string VariantPN { get; }

    [DispId(94)]
    string APIVersion { get; }

    [DispId(95)]
    BNFErrorStatus LastBNFError { get; }

    [DispId(96)]
    BanknoteClassification EscrowClassification { get; }

    [DispId(97)]
    void SetCustomerConfigurationOption(ConfigurationIndex index, byte value);

    [DispId(98)]
    byte QueryCustomerConfigurationOption(ConfigurationIndex index);

    [DispId(100)]
    void Open(string portName);

    [DispId(101)]
    void Open(string portName, PowerUp powerUp);

    [DispId(102)]
    void Close();

    [DispId(103)]
    void Calibrate();

    [DispId(104)]
    void EscrowReturn();

    [DispId(105)]
    void EscrowStack();

    [DispId(106)]
    void FlashDownload(string filePath);

    [DispId(107)]
    void ClearCashBoxTotal();

    [DispId(108)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
    byte[] RawTransaction([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] ref byte[] payload);

    [DispId(109)]
    void SetAssetNumber(string asset);

    [DispId(110)]
    void SetBezel(Bezel bezel);

    [DispId(111)]
    void SoftReset();

    [DispId(112)]
    void SpecifyEscrowTimeout(int billTimeout, int barcodeTimeout);

    [DispId(113)]
    bool DisableCashboxCleanlinessReporting();

    [DispId(114)]
    bool EnableCashboxCleanlinessReporting();

    [DispId(115)]
    bool CancelAdvancedBookmarkMode();

    [DispId(116)]
    bool EnterAdvancedBookmarkMode();

    [DispId(117)]
    void StopDownload();

    [DispId(118)]
    bool ClearAudit();

    [DispId(119)]
    long GetRFIDSerialNumber();

    [DispId(120)]
    IDocument getDocument();
  }
}
