using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Client;
using MobileWallet.Desktop.Helpers;

namespace MobileWallet.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SelectMobileMoney : Window
    {
        public SelectMobileMoney()
        {
            InitializeComponent();
            Set_Language();
            BtnCrypto.Visibility = Visibility.Collapsed;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SelectLanguage NewWindow = new SelectLanguage();
            NewWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WelcomeToMobileWallet NewWindow = new WelcomeToMobileWallet();
            NewWindow.Show();
            this.Close();
        }

              private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Back" });
            _ = App.LogError("Back Button Pressed on Select Mobile Money", LogType.Back);
            SelectLanguage NewWindow = new SelectLanguage();
            NewWindow.Show();
            this.Close();
        }

        private void Set_Language()
        {
            SelectMoneyOperator.Inlines.Clear();
            switch (Global.DefaultLanguage)
            {
                case "English":
                    Back.Content = ResourceEnglish.Back;
                    OrangeMoneyContent.Text = ResourceEnglish.OrangeMoney;
                    SelectMoneyOperator.Inlines.Add(ResourceEnglish.SelectMoneyOperator);
                    CryptoMoneyContent.Text = ResourceEnglish.Crypto;
                    break;
                case "French":
                    Back.Content = ResourceFrench.Back;
                    OrangeMoneyContent.Text = ResourceFrench.OrangeMoney;
                    SelectMoneyOperator.Inlines.Add(ResourceFrench.SelectMoneyOperator);
                    CryptoMoneyContent.Text = ResourceEnglish.Crypto;
                    break;
            }
        }

        private void SelectMobileMoney_OnLoaded(object? sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(async () =>
            {
                try
                {
                    Helper.AdjustRowHeight(this, DynamicRow);
                    Global.IsCrypto = false;
                    App.StartTimer(this);
                    // App.ShowProcessingDialog();
                    _ = App.TrackAtmRealTime(
                        new UpdateAtmRealTimeRequestModel()
                        {
                            CurrentScreen = nameof(SelectMobileMoney),
                        }
                    );
                    var atmClient = new AtmClient(HttpClientSingleton.Instance);
                    var profile = await atmClient.Atm_GetAtmProfileAsync();
                    if (profile.Data != null)
                    {
                        Global.Profile = profile.Data;
                        if (profile.Data.CryptoEnabled)
                        {
                            BtnCrypto.Visibility = Visibility.Visible;
                        }
                    }
                    App.HideProcessingDialog();
                    if (profile != null && profile.Data != null)
                    {
                        if (!profile.Data.IsEnabled)
                        {
                            BtnMtn.Visibility = Visibility.Collapsed;
                            BtnOrange.Visibility = Visibility.Collapsed;
                            BtnCrypto.Visibility = Visibility.Collapsed;
                            SelectMoneyOperator.Inlines.Add(new LineBreak());
                            SelectMoneyOperator.Inlines.Add(
                                Global.IsFrench
                                    ? ResourceFrench.OutOfService
                                    : ResourceEnglish.OutOfService
                            );
                        }
                    }
                }
                catch (Exception exception)
                {
                    App.AppLogger.Error(exception, exception.Message);
                    _ = App.LogError(
                        "Error Occured: " + exception.Message,
                        error: exception.StackTrace
                    );
                }
                finally
                {
                    App.HideProcessingDialog();
                }
            });
        }

        private void Button_Click_Crypto(object sender, RoutedEventArgs e)
        {
            try
            {
                App.ShowProcessingDialog();
                _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Crypto" });
                SelectCryptoCashInCashOut NewWindow = new SelectCryptoCashInCashOut();
                NewWindow.Show();
                this.Close();
                App.HideProcessingDialog();
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
                App.HideProcessingDialog();
                CustomMessageBox.Show("Error Occured Please Try Again");
            }
            finally
            {
                App.HideProcessingDialog();
            }
        }

        private void SelectMobileMoney_OnClosing(object? sender, CancelEventArgs e)
        {
            App.StopTimer();
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            WelcomeToMobileWallet welcomeToMobileWallet = new WelcomeToMobileWallet();
            welcomeToMobileWallet.Show();
            Close();
        }
    }
}
