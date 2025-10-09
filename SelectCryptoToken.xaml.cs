using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Client;

namespace MobileWallet.Desktop;

public partial class SelectCryptoToken : Window
{
    public SelectCryptoToken()
    {
        InitializeComponent();
        SetLanguage();
    }

    private void SetLanguage()
    {
        switch (Global.DefaultLanguage)
        {
            case "English":
                Back.Content = ResourceEnglish.Back;
                Cancel.Content = ResourceEnglish.Cancel;
                MyHeader.Text = ResourceEnglish.SelectCryptoToken;
                break;
            case "French":
                Back.Content = ResourceFrench.Back;
                Cancel.Content = ResourceFrench.Cancel;
                MyHeader.Text = ResourceFrench.SelectCryptoToken;
                break;
        }
    }

    private void BtnSelectCryptoToken_Click(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        var tokenDto = (CryptoTokenDto)btn.Tag;
        Global.SelectedToken = tokenDto;
        if (Global.IsDeposit)
        {
            SelectCryptoQuotation window = new SelectCryptoQuotation();
            window.Show();
        }
        else
        {
            if (Global.UseV2)
            {
                PleaseEnterAmountV2 note = new PleaseEnterAmountV2();
                note.Show();
            }
            else
            {
                PleaseEnterAmount note = new PleaseEnterAmount();
                note.Show();
            }
        }
        Close();
    }

    private async void SelectCryptoToken_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            App.StartTimer(this);
            _ = App.TrackAtmRealTime(
                new UpdateAtmRealTimeRequestModel() { CurrentScreen = nameof(SelectCryptoToken) }
            );
            App.ShowProcessingDialog();
            var cryptoList = await new CryptoClient(
                HttpClientSingleton.Instance
            ).Crypto_GetCryptoTokensAsync(
                Global.SelectedNetwork.Symbol,
                true,
                false,
                "",
                false,
                1,
                10,
                "Symbol"
            );
            List<CryptoTokenDto> list = cryptoList.Data.ToList();
            CryptoTokenItemControl.ItemsSource = list;
            App.HideProcessingDialog();
        }
        catch (Exception exception)
        {
            App.AppLogger.Error(exception, exception.Message);
            CustomMessageBox.Show("Unable to Fetch Networks");
        }
        finally
        {
            App.HideProcessingDialog();
        }
    }

    private void Button_Click_Back(object sender, RoutedEventArgs e)
    {
        SelectCryptoNetwork window = new SelectCryptoNetwork();
        window.Show();
        this.Close();
    }

    private void Button_Click_Cancel(object sender, RoutedEventArgs e)
    {
        WelcomeToMobileWallet window = new WelcomeToMobileWallet();
        window.Show();
        this.Close();
    }

    private void SelectCryptoToken_OnClosing(object? sender, CancelEventArgs e)
    {
        App.StopTimer();
    }
}
