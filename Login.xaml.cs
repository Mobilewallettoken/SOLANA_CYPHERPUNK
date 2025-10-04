using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ITL;
using ITL.Events;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Atm;
using MobileWallet.Desktop.Client;
using MobileWallet.Desktop.Helpers;

namespace MobileWallet.Desktop;

public partial class Login : CashDeviceWindow
{
    public Login()
    {
        InitializeComponent();
        if (Global.IsTest)
        {
            if (Global.UseV2)
            {
                TxtPin.Password = "Lahore123@";
                TxtUserName.Password = "demov2";
            }
            else
            {
                TxtPin.Password = "Lahore123@";
                TxtUserName.Password = "hbit";
            }


        }
        else
        {
            // TxtPin.Password = "Lahore123@";
            // TxtUserName.Password = "mwplc01";
            TxtPin.Password = "";
            TxtUserName.Password = "";

		}

    }

    private TextBox selectedTextBox;

    private void SetAccountNumber(string value)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(value);
        string finalvalue = stringBuilder.ToString();
        if (selectedTextBox != null)
            selectedTextBox.Text += finalvalue;
    }

    private void GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox txt)
        {
            selectedTextBox = txt;
        }
        App.OpenKeyBoard();
    }

    private async void BtnLogin_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            ButtonHelper.ToggleButton(sender);
            App.ShowProcessingDialog();
            await new AppAuthClient().LookUpBaseUrl();
            var result = await new AppAuthClient().Authenticate(
                TxtUserName.Password,
                TxtPin.Password
            );
            if (result)
            {
                if (Global.UseOtp)
                {
                    SecureOTPhasbeensent NewWindow = new SecureOTPhasbeensent(
                        TxtUserName.Password,
                        TxtPin.Password,
                        true
                    );
                    NewWindow.Show();
                    this.Close();
                }
                else
                {
                    await App.InitSignalR();
                    App.HideProcessingDialog();
                    WelcomeToMobileWallet window2 = new WelcomeToMobileWallet();
                    window2.Show();
                    this.Close();
                }
            }
            else
            {
                App.HideProcessingDialog();
                CustomMessageBox.ShowDialog("Invalid Credentials");
            }
        }
        catch (Exception exception)
        {
            App.AppLogger.Error(exception, exception.Message);
            App.HideProcessingDialog();
            CustomMessageBox.ShowDialog("Error has occured. Please try again");
        }
        finally
        {
            ButtonHelper.ToggleButton(sender);
            App.HideProcessingDialog();
        }
    }

    private async void BtnPrintDeposit_OnClick(object sender, RoutedEventArgs e)
    {
        // // 72afd69f-b96b-4012-a5c3-b13dc53310f0
        // var transactionClient = new MtnClient(HttpClientSingleton.Instance);
        // var response = (
        //     await transactionClient.Mtn_GetTransactionInfoAsync(
        //         "72afd69f-b96b-4012-a5c3-b13dc53310f0"
        //     )
        // ).Data;
        double cashIn = 5;
        string transactionId = "72afd69f-b96b-4012-a5c3-b13dc53310f0";
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string time = DateTime.Now.ToString("hh:mm tt");
        string currency = Global.Currency;
        string accountNo = "237672987483";
        bool isSuccess = ReceiptPrinter.PrintWithdrawReceipt(
            "demov2",
            "Gerald",
            cashIn,
            transactionId,
            date,
            time,
            currency,
            accountNo,
            new List<ReceiptItem>()
        );
    }

    private void BtnPrintWithdraw_OnClick(object sender, RoutedEventArgs e)
    {
        App.ShowProcessingDialog();
    }

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {
        App.CloseKeyBoard();
    }

    private async void Login_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Global.UseV2)
        {
            InitCashDevice();
            App.InitCashDevice();
        }
    }

    private void Login_OnClosing(object? sender, CancelEventArgs e)
    {
        UnInitCashDevice();
    }
}
