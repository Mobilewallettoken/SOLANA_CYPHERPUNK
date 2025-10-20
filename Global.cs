using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Atm;

namespace MobileWallet.Desktop;

public class Global
{
    // public static int CameraIndex = 0;

    // public const string BaseUrl = "https://localhost:5001/";

 
    public const string BaseUrl = "https://localhost:5001/";


    public static string DefaultLanguage { get; set; } = "English";
    public static string CurrentAccountNumber = "";
    public static string Username { get; set; } = "";
    public static bool IsFrench => DefaultLanguage == "French";
    public static bool IsCrypto { get; set; } = false;
    public static bool UseHardware { get; set; } = true;
    public static bool IsCryptoAbove { get; set; } = false;
    public static CryptoNetworkDto SelectedNetwork { get; set; } = null;
    public static string UserAddress { get; set; } = "";


        public static TimeSpan CryptoWithdrawWaitTime = TimeSpan.FromMinutes(5);
    public static string DefaultLanguage { get; set; } = "English";
    public static string CurrentAccountNumber = "";
    public static CryptoTokenDto SelectedToken { get; set; } = null;
    public static AtmProfileDto Profile { get; set; } = null;

}
