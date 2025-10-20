using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MobileWallet.Desktop.API;

namespace MobileWallet.Desktop;

public partial class SelectCryptoCashInCashOut : Window
{
    public SelectCryptoCashInCashOut()
    {
        InitializeComponent();
        Set_Language();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        SelectLanguage NewWindow = new SelectLanguage();
        NewWindow.Show();
        this.Close();
    }

    //private void Button_Click_1(object sender, RoutedEventArgs e)
    //{

    //    //Deposit.IsEnabled = true;
    //    Application.Current.Properties["Deposit"] = "Deposit";
    //    Application.Current.Properties["Withdraw"] = "null";
    //    EnterAccountNumber enterAccountNumber = new EnterAccountNumber();
    //    enterAccountNumber.Show();
    //    this.Close();
    //}

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        SelectMobileMoney mainWindow = new SelectMobileMoney();
        mainWindow.Show();
        this.Close();
    }

    private void Button_Click_Back(object sender, RoutedEventArgs e)
    {
        SelectMobileMoney mainWindow = new SelectMobileMoney();
        mainWindow.Show();
        this.Close();
        // PlaceYourCardAlignWithTheCamera placeYourCard = new PlaceYourCardAlignWithTheCamera();
        // placeYourCard.Show();
        // this.Close();
    }

    private void Button_Click_Cancel(object sender, RoutedEventArgs e)
    {
        WelcomeToMobileWallet NewWindow = new WelcomeToMobileWallet();
        NewWindow.Show();
        this.Close();
    }

    //private void Button_Click_3(object sender, RoutedEventArgs e)
    //{

    //}
    private void Set_Language()
    {
        switch (Global.DefaultLanguage)
        {
            case "English":
                Back.Content = ResourceEnglish.Back;
                Cancel.Content = ResourceEnglish.Cancel;
                BtnBuy.Content = ResourceEnglish.Buy;
                BtnWithdraw.Content = ResourceEnglish.Sell;
                BtnResume.Content = ResourceEnglish.Resume;
                MyHeader.Text = ResourceEnglish.WhatDoYouWantToDo;
                break;
            case "French":
                Back.Content = ResourceFrench.Back;
                Cancel.Content = ResourceFrench.Cancel;
                BtnBuy.Content = ResourceFrench.Buy;
                BtnResume.Content = ResourceFrench.Resume;
                BtnWithdraw.Content = ResourceFrench.Sell;
                MyHeader.Text = ResourceFrench.WhatDoYouWantToDo;
                break;
        }
    }

    private void Button_Click_Buy(object sender, RoutedEventArgs e)
    {
        Global.IsDeposit = true;
        SelectCryptoRange enterAccountNumber = new SelectCryptoRange();
        enterAccountNumber.Show();
        this.Close();
    }

    private void Button_Click_Withdrawl(object sender, RoutedEventArgs e)
    {
        Global.IsDeposit = false;
        SelectCryptoRange enterAccountNumber = new SelectCryptoRange();
        enterAccountNumber.Show();
        Close();
    }

    private void Button_Click_Resume(object sender, RoutedEventArgs e)
    {
        if (Global.UseV2)
        {
            PleaseEnterTransactionIdV2 enterTransactionId = new PleaseEnterTransactionIdV2();
            enterTransactionId.Show();
        }
        else
        {
            PleaseEnterTransactionId enterTransactionId = new PleaseEnterTransactionId();
            enterTransactionId.Show();
        }
        Close();
    }

    private void SelectCryptoCashInCashOut_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.Invoke(() =>
            {
                Helper.AdjustRowHeight(this, DynamicRow);
                App.StartTimer(this);
                _ = App.TrackAtmRealTime(
                    new UpdateAtmRealTimeRequestModel()
                    {
                        CurrentScreen = nameof(SelectCryptoCashInCashOut),
                    }
                );
                if (Global.Profile != null)
                {
                    if (!Global.Profile.DepositEnabled)
                    {
                        BtnDepositBorder.Visibility = Visibility.Hidden;
                    }
                    if (!Global.Profile.WithdrawEnabled)
                    {
                        BtnWithdrawBorder.Visibility = Visibility.Hidden;
                        BtnWithdrawResume.Visibility = Visibility.Hidden;
                    }
                }
            });
        }
        catch (Exception exception)
        {
            App.AppLogger.Error(exception, exception.Message);
            App.HideProcessingDialog();
        }
    }

    private void SelectCryptoCashInCashOut_OnClosing(object? sender, CancelEventArgs e)
    {
        App.StopTimer();
    }
}
