using System.Windows;
using System.Windows.Controls;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Client;

namespace MobileWallet.Desktop;

public partial class SelectCryptoNetwork : Window
{
    public SelectCryptoNetwork()
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
                MyHeader.Text = ResourceEnglish.SelectCryptoNetwork;
                break;
            case "French":
                Back.Content = ResourceFrench.Back;
                Cancel.Content = ResourceFrench.Cancel;
                MyHeader.Text = ResourceFrench.SelectCryptoNetwork;
                break;
        }
    }

    private void Button_Click_Back(object sender, RoutedEventArgs e)
    {
        SelectMobileMoney window = new SelectMobileMoney();
        window.Show();
        Close();
    }

    private void Button_Click_Cancel(object sender, RoutedEventArgs e)
    {
        WelcomeToMobileWallet window = new WelcomeToMobileWallet();
        window.Show();
        Close();
    }

    private void BtnSelectCryptoNetwork_Click(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        var networkDto = (CryptoNetworkDto)btn.Tag;
        Global.SelectedNetwork = networkDto;
        SelectCryptoToken window = new SelectCryptoToken();
        window.Show();
        Close();
    }

    private async void SelectCryptoNetwork_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            App.StartTimer(this);
            _ = App.TrackAtmRealTime(
                new UpdateAtmRealTimeRequestModel() { CurrentScreen = nameof(SelectCryptoNetwork) }
            );
            GetCryptoNetworksResponseModel? cryptoList = await new CryptoClient(
                HttpClientSingleton.Instance
            ).Crypto_GetCryptoNetworksAsync(true, false, "", false, 1, 10, "Symbol");
            var list = cryptoList.Data.ToList();
            CryptoNetworkItemControl.ItemsSource = list;
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
}
