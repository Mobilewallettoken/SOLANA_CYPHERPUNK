using System.ComponentModel;
using System.Windows;
using MobileWallet.Desktop.API;

namespace MobileWallet.Desktop;

public partial class SelectCryptoRange : Window
{
    public SelectCryptoRange()
    {
        InitializeComponent();
        SetLanguage();
    }

    private void SetLanguage()
    {
        if (Global.IsFrench)
        {
            MyHeader.Text = ResourceFrench.HowMuchDoYouWantToBuy;
            Back.Content = ResourceFrench.Back;
            Cancel.Content = ResourceFrench.Cancel;
            if (Global.IsDeposit)
            {
                BtnBuyAbove.Content = ResourceFrench.BuyAboveLimit;
                BtnBuyBelow.Content = ResourceFrench.BuyBelowLimit;
            }
            else
            {
                BtnBuyAbove.Content = ResourceFrench.SellAboveLimit;
                BtnBuyBelow.Content = ResourceFrench.SellBelowLimit;
            }
        }
        else
        {
            Back.Content = ResourceEnglish.Back;
            Cancel.Content = ResourceEnglish.Cancel;
            if (Global.IsDeposit)
            {
                MyHeader.Text = ResourceEnglish.HowMuchDoYouWantToBuy;
                if (Global.UseV2)
                {
                    BtnBuyAbove.Content = ResourceEnglish.BuyAboveLimitV2;
                    BtnBuyBelow.Content = ResourceEnglish.BuyBelowLimitV2;
                }
                else
                {
                    BtnBuyAbove.Content = ResourceEnglish.BuyAboveLimit;
                    BtnBuyBelow.Content = ResourceEnglish.BuyBelowLimit;
                }
            }
            else
            {
                MyHeader.Text = ResourceEnglish.HowMuchDoYouWantToSell;
                if (Global.UseV2)
                {
                    BtnBuyAbove.Content = ResourceEnglish.SellAboveLimitV2;
                    BtnBuyBelow.Content = ResourceEnglish.SellBelowLimitV2;
                }
                else
                {
                    BtnBuyAbove.Content = ResourceEnglish.SellAboveLimit;
                    BtnBuyBelow.Content = ResourceEnglish.SellBelowLimit;
                }
            }
        }
    }

    private void SelectCryptoRange_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            App.StartTimer(this);
            _ = App.TrackAtmRealTime(
                new UpdateAtmRealTimeRequestModel() { CurrentScreen = nameof(SelectCryptoRange) }
            );
        }
        catch (Exception exception)
        {
            App.AppLogger.Error(exception, exception.Message);
            App.HideProcessingDialog();
        }
    }

    private void SelectCryptoRange_OnClosing(object? sender, CancelEventArgs e)
    {
        App.StopTimer();
    }

    private void Button_Click_Above(object sender, RoutedEventArgs e)
    {
        Global.IsCrypto = true;
        Global.IsCryptoAbove = true;
        EnterAccountNumber window = new EnterAccountNumber();
        window.Show();
        Close();
    }

    private void Button_Click_Back(object sender, RoutedEventArgs e)
    {
        SelectCryptoCashInCashOut window = new SelectCryptoCashInCashOut();
        window.Show();
        Close();
    }

    private void Button_Click_Cancel(object sender, RoutedEventArgs e)
    {
        WelcomeToMobileWallet window = new WelcomeToMobileWallet();
        window.Show();
        Close();
    }

    private void Button_Click_Below(object sender, RoutedEventArgs e)
    {
        Global.IsCrypto = true;
        Global.IsCryptoAbove = false;
        EnterAccountNumber window = new EnterAccountNumber();
        window.Show();
        this.Close();
    }
}
