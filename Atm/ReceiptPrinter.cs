using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using MobileWallet.Desktop.Extensions;
using QRCoder;
using ZXing;
using Size = System.Windows.Size;

namespace MobileWallet.Desktop.Atm;

public class ReceiptPrinter
{
    private const string DllName = @"AppLibs\Msprintsdk.dll";

    [DllImport("kernel32.dll", EntryPoint = "GetSystemDefaultLCID")]
    public static extern int GetSystemDefaultLCID();

    [DllImport(DllName, EntryPoint = "SetInit", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetInit();

    [DllImport(DllName, EntryPoint = "SetUsbportauto", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetUsbportauto(); //Only used if printer is connected by Usb

    [DllImport(DllName, EntryPoint = "SetClean", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetClean();

    [DllImport(DllName, EntryPoint = "SetClose", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetClose();

    [DllImport(DllName, EntryPoint = "SetAlignment", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetAlignment(int iAlignment);

    [DllImport(DllName, EntryPoint = "SetAlignmentLeftRight", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetAlignmentLeftRight(int iAlignment);

    [DllImport(DllName, EntryPoint = "SetBold", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetBold(int iBold);

    [DllImport(DllName, EntryPoint = "SetCommandmode", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetCommandmode(int iMode);

    [DllImport(DllName, EntryPoint = "SetLinespace", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetLinespace(int iLinespace);

    [DllImport(DllName, EntryPoint = "SetPrintport", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetPrintport(StringBuilder strPort, int iBaudrate);

    [DllImport(DllName, EntryPoint = "PrintString", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintString(StringBuilder strData, int iImme);

    /// <summary>
    /// Prints in Line 45 Characters in row
    /// </summary>
    /// <param name="strData"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    public static int PrintStringLocal(string strData, int i = 0)
    {
        if (Global.UsePrinter)
        {
            return PrintString(new StringBuilder(strData), i);
        }
        if (i == 0)
        {
            Console.WriteLine(strData);
        }
        else
        {
            Console.Write(strData);
        }
        return 0;
    }

    [DllImport(DllName, EntryPoint = "PrintFeedline", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintFeedline(int iLine);

    [DllImport(DllName, EntryPoint = "PrintSelfcheck", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintSelfcheck();

    [DllImport(DllName, EntryPoint = "GetStatus", CharSet = CharSet.Ansi)]
    public static extern unsafe int GetStatus();

    [DllImport(DllName, EntryPoint = "GetStatusspecial", CharSet = CharSet.Ansi)]
    public static extern unsafe int GetStatusspecial();

    [DllImport(DllName, EntryPoint = "PrintCutpaper", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintCutpaper(int iMode);

    [DllImport(DllName, EntryPoint = "SetSizetext", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetSizetext(int iHeight, int iWidth);

    [DllImport(DllName, EntryPoint = "SetSizechinese", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetSizechinese(
        int iHeight,
        int iWidth,
        int iUnderline,
        int iChinesetype
    );

    [DllImport(DllName, EntryPoint = "SetItalic", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetItalic(int iItalic);

    [DllImport(DllName, EntryPoint = "PrintDiskbmpfile", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintDiskbmpfile(StringBuilder strData);

    [DllImport(DllName, EntryPoint = "PrintDiskimgfile", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintDiskimgfile(StringBuilder strData);

    [DllImport(DllName, EntryPoint = "PrintQrcode", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintQrcode(
        StringBuilder strData,
        int iLmargin,
        int iMside,
        int iRound
    );

    [DllImport(DllName, EntryPoint = "PrintRemainQR", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintRemainQR();

    [DllImport(DllName, EntryPoint = "SetLeftmargin", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetLeftmargin(int iLmargin);

    [DllImport(DllName, EntryPoint = "GetProductinformation", CharSet = CharSet.Ansi)]
    public static extern unsafe int GetProductinformation(int Fstype, byte[] FIDdata, int iFidlen);

    [DllImport(DllName, EntryPoint = "PrintTransmit", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintTransmit(byte[] strCmd, int iLength);

    [DllImport(DllName, EntryPoint = "GetTransmit", CharSet = CharSet.Ansi)]
    public static extern unsafe int GetTransmit(
        byte[] strCmd,
        int iLength,
        byte[] bRecv,
        int iRelen
    );

    [DllImport(DllName, EntryPoint = "PrintFeedDot", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintFeedDot(int Lnumber);

    [DllImport(DllName, EntryPoint = "PrintChargeRow", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintChargeRow();

    [DllImport(DllName, EntryPoint = "SetSpacechar", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetSpacechar(int iSpace);

    [DllImport(DllName, EntryPoint = "SetSizechar", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetSizechar(
        int iHeight,
        int iWidth,
        int iUnderline,
        int iAsciitype
    );

    [DllImport(DllName, EntryPoint = "SetRotate", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetRotate(int iRotate);

    [DllImport(DllName, EntryPoint = "SetDirection", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetDirection(int iDirection);

    [DllImport(DllName, EntryPoint = "SetWhitemodel", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetWhitemodel(int iWhite);

    [DllImport(DllName, EntryPoint = "SetUnderline", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetUnderline(int underline);

    [DllImport(DllName, EntryPoint = "PrintNextHT", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintNextHT();

    [DllImport(DllName, EntryPoint = "SetHTseat", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetHTseat(byte[] bHTseat, int iLength);

    [DllImport(DllName, EntryPoint = "SetCodepage", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetCodepage(int country, int CPnumber);

    [DllImport(DllName, EntryPoint = "Print1Dbar", CharSet = CharSet.Ansi)]
    public static extern unsafe int Print1Dbar(
        int iWidth,
        int iHeight,
        int iHrisize,
        int iHriseat,
        int iCodetype,
        StringBuilder strData
    );

    [DllImport(DllName, EntryPoint = "SetPagemode", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetPagemode(int iMode, int Xrange, int Yrange);

    [DllImport(DllName, EntryPoint = "SetPagestartposition", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetPagestartposition(int Xdot, int Ydot);

    [DllImport(DllName, EntryPoint = "SetPagedirection", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetPagedirection(int iDirection);

    [DllImport(DllName, EntryPoint = "PrintPagedata", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintPagedata();

    [DllImport(DllName, EntryPoint = "SetReadZKmode", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetReadZKmode(int mode);

    [DllImport(DllName, EntryPoint = "SetNvbmp", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetNvbmp(int iNums, StringBuilder strPath);

    [DllImport(DllName, EntryPoint = "PrintNvbmp", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintNvbmp(int iNvindex, int iMode);

    [DllImport(DllName, EntryPoint = "SetPrintIDorName", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetPrintIDorName(StringBuilder strIDorNAME);

    [DllImport(DllName, EntryPoint = "GetPrintIDorName", CharSet = CharSet.Ansi)]
    public static extern unsafe int GetPrintIDorName(StringBuilder strIDorNAME);

    [DllImport(DllName, EntryPoint = "PrintMarkcutpaper", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintMarkcutpaper(int iMode);

    [DllImport(DllName, EntryPoint = "SetTraceLog", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetTraceLog(int iLog);

    [DllImport(DllName, EntryPoint = "PrintPdf417", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintPdf417(
        int iDotwidth,
        int iDotheight,
        int iDatarows,
        int iDatacolumns,
        StringBuilder strData
    );

    //旋转模式
    [DllImport(DllName, EntryPoint = "SetRotation_Intomode", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetRotation_Intomode();

    [DllImport(DllName, EntryPoint = "SetRotation_Leftspace", CharSet = CharSet.Ansi)]
    public static extern unsafe int SetRotation_Leftspace(int iLeftspace);

    [DllImport(DllName, EntryPoint = "PrintRotation_Sendcode", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintRotation_Sendcode(
        int leftspace,
        int iWidth,
        int iHeight,
        int iCodetype,
        StringBuilder strData
    );

    [DllImport(DllName, EntryPoint = "PrintRotation_Sendtext", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintRotation_Sendtext(StringBuilder strData, int iImme);

    [DllImport(DllName, EntryPoint = "PrintRotation_Changeline", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintRotation_Changeline();

    [DllImport(DllName, EntryPoint = "PrintRotation_Data", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintRotation_Data();

    [DllImport(DllName, EntryPoint = "PrintPDF_CCCB", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintPDF_CCCB(StringBuilder strData);

    [DllImport(DllName, EntryPoint = "PrintDataMatrix", CharSet = CharSet.Ansi)]
    public static extern unsafe int PrintDataMatrix(StringBuilder strData, int iSize);

    private static int characterLength = 48;

    private static string WithSpace(
        string content,
        int width = 45,
        bool isCenter = false,
        bool isLeft = false,
        bool isRight = false
    )
    {
        if (content.Length < width)
        {
            var remaining = width - content.Length;
            if (isLeft)
            {
                return $"{content}{GetSpace(remaining)}";
            }
            if (isRight)
            {
                return $"{GetSpace(remaining)}{content}";
            }
            if (isCenter)
            {
                if (remaining % 2 == 1)
                {
                    remaining++;
                }
                return $"{GetSpace(remaining / 2)}{content}{GetSpace(remaining / 2)}";
            }
        }
        return content;
    }

    private static string GetSpace(int a)
    {
        if (a == 0)
        {
            return "";
        }
        var s = "";
        for (var i = 0; i < a; i++)
        {
            s += " ";
        }
        return s;
    }

    private static void SetLangToFrench()
    {
        var result = SetCodepage(1, 2);
    }

    public static bool PrintDepositReceipt(
        string branchCode = "AKWA0001",
        string name = "你好世界 Lukong Mobile wallet",
        double cashIn = 10000,
        string transactionId = "123456789",
        string date = "",
        string time = "",
        string currency = "XAF",
        string accountNo = "237674481877",
        List<ReceiptItem>? item = null
    )
    {
        if (accountNo.StartsWith("237"))
        {
            accountNo = accountNo[3..];
        }
        try
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                date = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrWhiteSpace(time))
            {
                time = DateTime.Now.ToString("hh:mm tt");
            }
            if (item == null)
            {
                item = new List<ReceiptItem>()
                {
                    new ReceiptItem()
                    {
                        Amount = 70000,
                        Item = 10000,
                        Quantity = 7,
                    },
                    new ReceiptItem()
                    {
                        Amount = 20000,
                        Item = 5000,
                        Quantity = 4,
                    },
                    new ReceiptItem()
                    {
                        Amount = 10000,
                        Item = 2000,
                        Quantity = 5,
                    },
                    new ReceiptItem()
                    {
                        Amount = 1000,
                        Item = 200,
                        Quantity = 5,
                    },
                    new ReceiptItem()
                    {
                        Amount = 100,
                        Item = 20,
                        Quantity = 5,
                    },
                    new ReceiptItem()
                    {
                        Amount = 10,
                        Item = 2,
                        Quantity = 5,
                    },
                };
            }

            if (Global.UsePrinter)
            {
                var r = SetUsbportauto();
                if (r == 1)
                {
                    MessageBox.Show("Unable to set Port ");
                    App.AppLogger.Error("No suitable USB port found. Check if Printer is connected via USB");
                    return false;
                }
                App.AppLogger.Debug("Usb Port Chosen succesfully by the sdk, moving to initialization");
                r = SetInit();
                if (r == 1)
                {
                    MessageBox.Show(
                        "The Printer is offline at this moment, we apologize for the inconvenience"
                    );
                    return false;
                }
                App.AppLogger.Debug("Printer Initialized succesfully");
                SetClean();
                var status = GetStatus();
                if (status > 0)
                {
                    // MessageBox.Show(status.ToString());
                    SetClose();
                    return false;
                }
            }

            if (Global.UsePrinter)
            {
                SetBold(1);
                SetAlignment(1);
            }
            SetAlignment(1);
            var image = PrintDiskbmpfile(new StringBuilder(GetImagePath()));
            PrintStringLocal("Transaction Receipt / ", 1);
            if (Global.UsePrinter)
            {
                SetLangToFrench();
            }
            PrintStringLocal("Recu de Transaction");
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal($"ATM{GetSpace(7)}{branchCode}{GetSpace(10)}Deposit / ", 1);
            if (Global.UsePrinter)
            {
                SetLangToFrench();
            }
            PrintStringLocal("Depot");
            if (Global.UsePrinter)
            {
                SetClean();
                SetLangToFrench();
            }
            PrintStringLocal($"Account Number/Numero de Compte{GetSpace(4)}{accountNo}");
            PrintStringLocal($"Name / Nom{GetSpace(33 - name.Length)}{name}{GetSpace(5)}");
            PrintStringLocal(
                $"{WithSpace("Cash In", 25, isLeft: true)}{GetSpace(4) + currency + " "}{WithSpace(cashIn.ToString("N"), 12, isRight: true)}"
            );

            if (item.Any())
            {
                PrintStringLocal(
                    $"{WithSpace("Lose Currency", 20, isCenter: true)}{WithSpace("QTY", 5, isCenter: true)}{WithSpace("SUM", 19, isRight: true)}"
                );
                foreach (var receiptItem in item)
                {
                    var money = $"{receiptItem.Item:N}";
                    PrintStringLocal(
                        $"{GetSpace(4)}{WithSpace(currency + " " + money, 16, isLeft: true)}{WithSpace(receiptItem.Quantity.ToString(), 5, isCenter: true)}{GetSpace(4) + currency + " "}{WithSpace(receiptItem.Amount.ToString("N"), 12, isRight: true)}"
                    );
                }
                PrintStringLocal($"Total Deposit/", 1);
                if (Global.UsePrinter)
                {
                    SetLangToFrench();
                }
                PrintStringLocal("Depot Total", 1);
                var c = (item?.Sum(p => p.Amount) ?? 0).ToString("N");
                PrintStringLocal(
                    $"{GetSpace(4) + currency + " "}{WithSpace(c, 12, isRight: true)}"
                );
            }

            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal("");
            PrintStringLocal("Deposit Availability");
            PrintStringLocal("The full amount of your deposit is included");
            PrintStringLocal("in your Available balance");
            PrintStringLocal("");
            PrintStringLocal($"Transaction ID{GetSpace(1)}");
            PrintStringLocal($"{WithSpace(transactionId, isLeft: true)}");
            PrintStringLocal($"Date{GetSpace(10)}{date}{GetSpace(8)}Time {time}");
            PrintStringLocal("");
            PrintStringLocal("With MoMo, stand a chance to receive up to");
            PrintStringLocal("25000F simply by making a payment of at least");
            PrintStringLocal("100000F at any ATM Location");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            if (Global.UsePrinter)
            {
                PrintCutpaper(0);
                SetClose();
            }
        }
        catch (Exception e)
        {
            App.AppLogger.Error(e, e.Message);
            MessageBox.Show(e.Message);
            return false;
        }

        return true;
    }

    public static bool PrintDepositReceiptCrypto(
        string branchCode = "AKWA0001",
        string name = "Isaac Lukong Mobile wallet",
        double cashIn = 10000,
        string transactionId = "f1938b48-6089-438d-974a-fcaeeda4d54a",
        string date = "",
        string time = "",
        string currency = "XAF",
        string accountNo = "237674481877",
        string purchasedAmount = "0.072132132135",
        string tokenSymbol = "BTC",
        string gasFee = "0.00001211124",
        string pricePerToken = "10,000.123123145",
        string walletAddress = "ABCDEFGHI",
        string userWalletAddress = "USER_WALLET",
        string txHash = "HASH",
        List<ReceiptItem>? item = null,
        string averagePrice = "1 SOL = $115.52",
        string exchangeRate = "1 USD = 620.89 XAF",
        string totalUsd = "$10.00",
        string status = "CONFIRMED"
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                date = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrWhiteSpace(time))
            {
                time = DateTime.Now.ToString("hh:mm tt");
            }
            if (Global.UsePrinter)
            {
                var r = SetUsbportauto();
                if (r == 1)
                {
                    MessageBox.Show("Unable to set Port ");
                    return false;
                }
                r = SetInit();
                if (r == 1)
                {
                    MessageBox.Show("Unable to Init ");
                    return false;
                }
            }
            PrintStringLocal("=============================================");
            if (Global.UsePrinter)
            {
                SetClean();
                SetAlignment(1);
                string imagePath = GetCryptoLogoPath();
                var image = PrintDiskbmpfile(new StringBuilder(imagePath));
                PrintStringLocal("");
                SetClean();
                SetBold(1);
            }
            PrintStringLocal(WithSpace("CRYPTO DEPOSIT RECEIPT", isCenter: true));
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal("=============================================");
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal($"ATM{WithSpace(branchCode, 42, isRight: true)}");
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal($"ID{WithSpace(transactionId, 43, isRight: true)}");
            //Date          2025-04-14  Time 11:58 AM
            PrintStringLocal($"Date{GetSpace(10)}{date}{GetSpace(8)}Time {time}");
            // PrintStringLocal(($"Numéro de Compte"), 0);
            PrintStringLocal("---------------------------------------------");
            PrintStringLocal($"{WithSpace("Withdrawal Information", isCenter: true)}");
            PrintStringLocal("---------------------------------------------");
            // Withdrawal Information
            if (string.IsNullOrEmpty(name))
            {
                PrintStringLocal($"Phone{WithSpace(accountNo, 40, isRight: true)}");
            }
            else
            {
                PrintStringLocal($"Name{WithSpace(name, 41, isRight: true)}");
            }
            PrintStringLocal($"Crypto will be sent to the following address");
            if (Global.UsePrinter)
            {
                SetClean();
                SetAlignment(1);
                string imagePath = GetQrCodePath(userWalletAddress);
                var image = PrintDiskbmpfile(new StringBuilder(imagePath));
                SetClean();
            }
            PrintStringLocal("Wallet");
            PrintStringLocal(userWalletAddress);
            PrintStringLocal("");
            PrintStringLocal("---------------------------------------------");
            PrintStringLocal($"{WithSpace("Transaction Details", isCenter: true)}");
            PrintStringLocal("---------------------------------------------");
            PrintStringLocal($"Currency" + WithSpace(currency, 37, isRight: true));
            PrintStringLocal($"Average Price" + WithSpace(averagePrice, 32, isRight: true));
            PrintStringLocal($"Slippage" + WithSpace("2.5%", 37, isRight: true));
            var amountWithCurrency = pricePerToken + " " + currency;
            amountWithCurrency = cashIn.ToString("F2") + " " + currency;
            PrintStringLocal(
                $"Amount Deposited" + WithSpace(amountWithCurrency, 29, isRight: true)
            );
            PrintStringLocal($"Transaction Fee" + WithSpace("3.00 USD", 30, isRight: true));
            var col = $"Total {tokenSymbol} Withdrawn";
            amountWithCurrency = purchasedAmount + " " + tokenSymbol;
            PrintStringLocal(
                $"{col}" + WithSpace(amountWithCurrency, 45 - col.Length, isRight: true)
            );
            // PrintStringLocal($"Exchange Rate" + WithSpace(exchangeRate, 32, isRight: true));
            PrintStringLocal($"Total USD" + WithSpace(totalUsd, 36, isRight: true));
            PrintStringLocal($"Status" + WithSpace(status, 39, isRight: true));
            // amountWithCurrency = gasFee + " " + tokenSymbol;
            // PrintStringLocal(
            //     new StringBuilder(
            //         "Gas Fee:" + GetSpace(37 - amountWithCurrency.Length) + amountWithCurrency
            //     )
            // );
            // amountWithCurrency = cashIn + " " + currency;
            // PrintStringLocal(
            //     new StringBuilder(
            //         $"Withdrawn:" + GetSpace(35 - amountWithCurrency.Length) + amountWithCurrency
            //     )
            // );
            // amountWithCurrency = purchasedAmount + " " + tokenSymbol;
            // PrintStringLocal(
            //     new StringBuilder(
            //         "Sold:" + GetSpace(40 - amountWithCurrency.Length) + amountWithCurrency
            //     )
            // );
            // amountWithCurrency = "Ending with (" + walletAddress[^5..] + ")";
            // PrintStringLocal(
            //     "Wallet Address:" + GetSpace(30 - amountWithCurrency.Length) + amountWithCurrency
            // );
            if (!string.IsNullOrWhiteSpace(userWalletAddress))
            {
                amountWithCurrency = "Ending with (" + userWalletAddress[^5..] + ")";
                PrintStringLocal(
                    ("User Address" + GetSpace(33 - amountWithCurrency.Length) + amountWithCurrency)
                );
            }

            if (!string.IsNullOrWhiteSpace(txHash))
            {
                PrintStringLocal("TxHash");
                PrintStringLocal(txHash);
            }

            PrintStringLocal("=============================================");
            PrintStringLocal(WithSpace("THANK YOU FOR USING OUR SERVICE", isCenter: true));
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal("=============================================");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            if (Global.UsePrinter)
            {
                PrintCutpaper(0);
                SetClose();
            }
        }
        catch (Exception e)
        {
            App.AppLogger.Error(e, e.Message);
            MessageBox.Show(e.Message);
            return false;
        }

        return true;
    }

    public static int IsValid()
    {
        var log_time = DateTime.Now;
        if (!Global.UsePrinter)
        {
            return 0;
        }
        var r = SetUsbportauto();
        if (r > 0)
        {
            App.AppLogger.Warn($"{log_time} : Printer USB port not set");
            return r;
        }
        App.AppLogger.Debug($"{log_time} : Printer USB port set succesfully");
        r = SetInit();
        if (r > 0)
        {
            App.AppLogger.Info($"{log_time} : Printer initialization failure");
            return r;
        }
        App.AppLogger.Debug($"{log_time} : Printer initialization successful");
        SetClean();
        var status = GetStatus();
        if (status > 0)
        {
            switch (status)
            {
                case 1:
                    App.AppLogger.Warn($"{log_time} : Printer is offline or no power");
                    break;
                case 2:
                    App.AppLogger.Warn($"{log_time} : Printer called unmatched library");
                    break;
                case 3:
                    App.AppLogger.Warn(
                        $"{log_time} : Current printer can’t support special function"
                    );
                    break;
                case 4:
                    App.AppLogger.Warn($"{log_time} : Printer doesn’t load paper to presenter");
                    break;
                case 5:
                    App.AppLogger.Warn($"{log_time} : Paper is blocked in printer bezel");
                    break;
                case 6:
                    App.AppLogger.Warn("{log_time} : Paper jams in printer mechanism");
                    break;
                case 7:
                    App.AppLogger.Warn(
                        $"{log_time} : Unfinished ticket is dragged by outside force"
                    );
                    break;
                case 8:
                    App.AppLogger.Warn(
                        $"{log_time} : There is ticket held on printer bezel (This is detected by ticket taken sensor)"
                    );
                    break;
                default:
                    App.AppLogger.Warn($"Unrecognized error with status code: {status}");
                    break;
            }
            SetClose();
            return status;
        }
        SetClose();
        return r;
    }

    private static string GetImagePath()
    {
        string currentPath = Directory.GetCurrentDirectory();
        string imagePath = Path.Combine(currentPath, "images", "400x200.bmp");
        return imagePath;
    }

    private static string GetQrCodePath(string data)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new QRCode(qrCodeData);

        // Generate QR as Bitmap (say 200x200)
        int qrSize = 200; // square QR size
        Bitmap qrBitmap = ConvertTo1BitGrayscale(qrCode.GetGraphic(6)); // Adjust pixel-per-module to control size
        string currentPath = Directory.GetCurrentDirectory();
        string imagePath = Path.Combine(currentPath, "images", "qrcode.bmp");
        qrBitmap.Save(imagePath, ImageFormat.Bmp);
        return imagePath;
    }

    private static Bitmap ConvertTo1BitGrayscale(Bitmap source)
    {
        int width = source.Width;
        int height = source.Height;
        Bitmap bwBmp = new Bitmap(width, height, PixelFormat.Format1bppIndexed);

        BitmapData data = bwBmp.LockBits(
            new Rectangle(0, 0, width, height),
            ImageLockMode.WriteOnly,
            PixelFormat.Format1bppIndexed
        );

        int stride = data.Stride;
        IntPtr scan0 = data.Scan0;
        byte[] rawData = new byte[stride * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = source.GetPixel(x, y);
                byte gray = (byte)(
                    (pixelColor.R * 0.3) + (pixelColor.G * 0.59) + (pixelColor.B * 0.11)
                );
                bool isWhite = gray >= 128;

                int byteIndex = y * stride + (x >> 3);
                int bitIndex = 7 - (x & 0x7);

                if (isWhite)
                {
                    rawData[byteIndex] |= (byte)(1 << bitIndex); // set bit
                }
                // else leave bit as 0 (black)
            }
        }

        System.Runtime.InteropServices.Marshal.Copy(rawData, 0, scan0, rawData.Length);
        bwBmp.UnlockBits(data);

        return bwBmp;
    }

    private static string GetCryptoLogoPath()
    {
        string currentPath = Directory.GetCurrentDirectory();
        string imagePath = Path.Combine(currentPath, "images", "mwt-logo.bmp");
        return imagePath;
    }

    public static bool PrintSampleQrCode()
    {
        var r = SetUsbportauto();
        if (r == 1)
        {
            MessageBox.Show("Unable to set Port ");
            return false;
        }
        r = SetInit();
        if (r == 1)
        {
            MessageBox.Show("Unable to Init ");
            return false;
        }
        SetClean();
        SetAlignment(1);
        r = PrintQrcode(new StringBuilder("Test"), 0, 0, 00);
        r = PrintRemainQR();
        r = PrintCutpaper(0);
        SetClose();
        return true;
    }

    public static bool PrintWithdrawReceipt(
        string branchCode = "AKWA0001",
        string name = "Isaac Lukong Mobile wallet",
        double cashIn = 10000,
        string transactionId = "f1938b48-6089-438d-974a-fcaeeda4d54a",
        string date = "",
        string time = "",
        string currency = "XAF",
        string accountNo = "237674481877",
        List<ReceiptItem>? item = null
    )
    {
        try
        {
            if (accountNo.StartsWith("237"))
            {
                accountNo = accountNo[3..];
            }
            if (string.IsNullOrWhiteSpace(date))
            {
                date = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrWhiteSpace(time))
            {
                time = DateTime.Now.ToString("hh:mm tt");
            }
            if (item == null)
            {
                item = new List<ReceiptItem>()
                {
                    new ReceiptItem()
                    {
                        Amount = 70000,
                        Item = 10000,
                        Quantity = 7,
                    },
                    new ReceiptItem()
                    {
                        Amount = 20000,
                        Item = 5000,
                        Quantity = 4,
                    },
                    new ReceiptItem()
                    {
                        Amount = 10000,
                        Item = 2000,
                        Quantity = 5,
                    },
                    new ReceiptItem()
                    {
                        Amount = 1000,
                        Item = 200,
                        Quantity = 5,
                    },
                    new ReceiptItem()
                    {
                        Amount = 100,
                        Item = 20,
                        Quantity = 5,
                    },
                    new ReceiptItem()
                    {
                        Amount = 10,
                        Item = 2,
                        Quantity = 5,
                    },
                };
            }

            if (Global.UsePrinter)
            {
                var r = SetUsbportauto();
                if (r == 1)
                {
                    MessageBox.Show("Unable to set Port ");
                    return false;
                }
                r = SetInit();
                if (r == 1)
                {
                    MessageBox.Show("Unable to Init ");
                    return false;
                }
                SetClean();
                SetAlignment(1);
            }
            SetAlignment(1);
            var image = PrintDiskbmpfile(new StringBuilder(GetImagePath()));
            PrintStringLocal("MOMO from MTN", 1);
            SetClean();
            SetBold(1);
            SetAlignment(1);
            PrintStringLocal("Transaction Receipt / ", 1);
            if (Global.UsePrinter)
            {
                SetLangToFrench();
            }
            PrintStringLocal("Recu de Transaction");
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal($"ATM{GetSpace(7)}{branchCode}{GetSpace(7)}Withdrawal / Retrait");
            if (Global.UsePrinter)
            {
                SetLangToFrench();
            }
            PrintStringLocal($"Account Number/Numero de Compte{GetSpace(4)}{accountNo}");
            if (Global.UsePrinter)
            {
                SetClean();
            }
            // PrintStringLocal(($"Numéro de Compte"), 0);
            PrintStringLocal($"Name / Nom{GetSpace(33 - name.Length)}{name}{GetSpace(5)}");
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal(
                $"{WithSpace("Cash Out", 25, isLeft: true)}{GetSpace(4) + currency + " "}{WithSpace(cashIn.ToString("N"), 12, isRight: true)}"
            );

            if (item != null && item.Any())
            {
                PrintStringLocal(
                    $"{WithSpace("Lose Currency", 20, isCenter: true)}{WithSpace("QTY", 5, isCenter: true)}{WithSpace("SUM", 19, isRight: true)}"
                );
                foreach (var receiptItem in item)
                {
                    var money = $"{receiptItem.Item:N}";
                    PrintStringLocal(
                        $"{GetSpace(4)}{currency} {WithSpace(money, 12, isRight: true)}{WithSpace(receiptItem.Quantity.ToString(), 5, isCenter: true)}{GetSpace(4) + currency + " "}{WithSpace(receiptItem.Amount.ToString("N"), 12, isRight: true)}"
                    );
                }
                PrintStringLocal($"Total Withdraw/", 1);
                if (Global.UsePrinter)
                {
                    SetLangToFrench();
                }
                PrintStringLocal("Retrait total", 1);
                if (Global.UsePrinter)
                {
                    SetClean();
                }
                // PrintString(
                // new StringBuilder($"{GetSpace(6)}{currency}{GetSpace(6)}{(item?.Sum(p => p.Amount) ?? 0):N}"), 0);
                var c = (item?.Sum(p => p.Amount) ?? 0).ToString("N");
                PrintStringLocal(
                    $"{GetSpace(1) + currency + " "}{WithSpace(c, 12, isRight: true)}"
                );
            }

            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal("");
            PrintStringLocal("Withdrawal Availability");
            PrintStringLocal("TThe full amount of your withdrawal is ready");
            PrintStringLocal("right away and is reflected in your balance");
            PrintStringLocal("");
            PrintStringLocal($"Transaction ID{GetSpace(1)}");
            PrintStringLocal($"{WithSpace(transactionId, isLeft: true)}");
            PrintStringLocal($"Date{GetSpace(10)}{date}{GetSpace(2)}Time {time}");
            PrintStringLocal("");
            PrintStringLocal("With MoMo, stand a chance to receive up to");
            PrintStringLocal("25000F simply by making a payment of at least");
            PrintStringLocal("100000F at any ATM Location");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            if (Global.UsePrinter)
            {
                PrintCutpaper(0);
                SetClose();
            }
        }
        catch (Exception e)
        {
            App.AppLogger.Error(e, e.Message);
            MessageBox.Show(e.Message);
            return false;
        }

        return true;
    }

    public static bool PrintWithdrawReceiptCrypto(
        string branchCode = "AKWA0001",
        string name = "Isaac Lukong Mobile wallet",
        double cashIn = 10000,
        string transactionId = "f1938b48-6089-438d-974a-fcaeeda4d54a",
        string date = "",
        string time = "",
        string currency = "XAF",
        string accountNo = "237674481877",
        string purchasedAmount = "0.072132132135",
        string tokenSymbol = "BTC",
        string gasFee = "0.00001211124",
        string pricePerToken = "10,000.123123145",
        string walletAddress = "ABCDEFGHI",
        string userWalletAddress = "USER_WALLET",
        string txHash = "HASH",
        List<ReceiptItem>? item = null,
        string averagePrice = "1 SOL = $115.52",
        string exchangeRate = "1 USD = 620.89 XAF",
        string totalUsd = "$10.00",
        string status = "CONFIRMED",
        string externalTransactionId = "TEST_EXTERNAL"
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                date = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrWhiteSpace(time))
            {
                time = DateTime.Now.ToString("hh:mm tt");
            }
            if (Global.UsePrinter)
            {
                var r = SetUsbportauto();
                if (r == 1)
                {
                    MessageBox.Show("Unable to set Port ");
                    return false;
                }
                r = SetInit();
                if (r == 1)
                {
                    MessageBox.Show("Unable to Init ");
                    return false;
                }
            }
            PrintStringLocal("=============================================");
            if (Global.UsePrinter)
            {
                SetClean();
                SetAlignment(1);
                string imagePath = GetCryptoLogoPath();
                var image = PrintDiskbmpfile(new StringBuilder(imagePath));
                PrintStringLocal("");
                SetClean();
                SetBold(1);
            }
            PrintStringLocal(WithSpace("CRYPTO WITHDRAWAL RECEIPT", isCenter: true));
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal("=============================================");
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal($"ATM{WithSpace(branchCode, 42, isRight: true)}");
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal($"ID{WithSpace(transactionId, 43, isRight: true)}");
            //Date          2025-04-14  Time 11:58 AM
            PrintStringLocal($"Date{GetSpace(10)}{date}{GetSpace(8)}Time {time}");
            // PrintStringLocal(($"Numéro de Compte"), 0);
            PrintStringLocal("---------------------------------------------");
            PrintStringLocal($"{WithSpace("Withdrawal Information", isCenter: true)}");
            PrintStringLocal("---------------------------------------------");
            // Withdrawal Information
            if (string.IsNullOrEmpty(name))
            {
                PrintStringLocal($"Phone{WithSpace(accountNo, 40, isRight: true)}");
            }
            else
            {
                PrintStringLocal($"Name{WithSpace(name, 41, isRight: true)}");
            }
            PrintStringLocal($"Send Crypto to the following address");
            if (Global.UsePrinter)
            {
                SetClean();
                SetAlignment(1);
                string imagePath = GetQrCodePath(walletAddress);
                var image = PrintDiskbmpfile(new StringBuilder(imagePath));
                SetClean();
            }
            PrintStringLocal("");
            PrintStringLocal("Wallet");
            PrintStringLocal(walletAddress);
            PrintStringLocal("");
            PrintStringLocal($"Scan the following code to resume");
            if (Global.UsePrinter)
            {
                SetClean();
                SetAlignment(1);
                string imagePath = GetQrCodePath(externalTransactionId);
                var image = PrintDiskbmpfile(new StringBuilder(imagePath));
                SetClean();
            }
            PrintStringLocal("");
            PrintStringLocal("---------------------------------------------");
            PrintStringLocal($"{WithSpace("Transaction Details", isCenter: true)}");
            PrintStringLocal("---------------------------------------------");
            PrintStringLocal($"Currency" + WithSpace(currency, 37, isRight: true));
            PrintStringLocal($"Average Price" + WithSpace(averagePrice, 32, isRight: true));
            PrintStringLocal($"Slippage" + WithSpace("2.5%", 37, isRight: true));
            var amountWithCurrency = cashIn + " " + currency;
            PrintStringLocal(
                $"Amount Withdrawn" + WithSpace(amountWithCurrency, 29, isRight: true)
            );
            PrintStringLocal($"Transaction Fee" + WithSpace("3.00 USD", 30, isRight: true));
            var col = $"Total {tokenSymbol} Withdrawn";
            amountWithCurrency = purchasedAmount + " " + tokenSymbol;
            PrintStringLocal(
                $"{col}" + WithSpace(amountWithCurrency, 45 - col.Length, isRight: true)
            );
            // PrintStringLocal($"Exchange Rate" + WithSpace(exchangeRate, 32, isRight: true));
            PrintStringLocal($"Total USD" + WithSpace(totalUsd, 36, isRight: true));
            PrintStringLocal($"Status" + WithSpace(status, 39, isRight: true));
            // amountWithCurrency = gasFee + " " + tokenSymbol;
            // PrintStringLocal(
            //     new StringBuilder(
            //         "Gas Fee:" + GetSpace(37 - amountWithCurrency.Length) + amountWithCurrency
            //     )
            // );
            // amountWithCurrency = cashIn + " " + currency;
            // PrintStringLocal(
            //     new StringBuilder(
            //         $"Withdrawn:" + GetSpace(35 - amountWithCurrency.Length) + amountWithCurrency
            //     )
            // );
            // amountWithCurrency = purchasedAmount + " " + tokenSymbol;
            // PrintStringLocal(
            //     new StringBuilder(
            //         "Sold:" + GetSpace(40 - amountWithCurrency.Length) + amountWithCurrency
            //     )
            // );
            // amountWithCurrency = "Ending with (" + walletAddress[^5..] + ")";
            // PrintStringLocal(
            //     "Wallet Address:" + GetSpace(30 - amountWithCurrency.Length) + amountWithCurrency
            // );
            if (!string.IsNullOrWhiteSpace(userWalletAddress))
            {
                amountWithCurrency = "Ending with (" + userWalletAddress[^5..] + ")";
                PrintStringLocal(
                    ("User Address" + GetSpace(33 - amountWithCurrency.Length) + amountWithCurrency)
                );
            }
            if (!string.IsNullOrWhiteSpace(txHash))
            {
                PrintStringLocal("TxHash");
                PrintStringLocal(txHash);
            }
            PrintStringLocal("=============================================");
            PrintStringLocal(
                WithSpace("THANK YOU FOR USING OUR SERVICE", width: 45, isCenter: true)
            );
            if (Global.UsePrinter)
            {
                SetClean();
            }
            PrintStringLocal("=============================================");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            PrintStringLocal("");
            if (Global.UsePrinter)
            {
                PrintCutpaper(0);
                SetClose();
            }
        }
        catch (Exception e)
        {
            App.AppLogger.Error(e, e.Message);
            MessageBox.Show(e.Message);
            return false;
        }

        return true;
    }
}
