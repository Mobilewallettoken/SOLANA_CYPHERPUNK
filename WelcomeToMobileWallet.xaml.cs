using System.Windows;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Client;
using MobileWallet.Desktop.Helpers;

namespace MobileWallet.Desktop
{
    /// <summary>
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class WelcomeToMobileWallet : Window
    {
        public WelcomeToMobileWallet()
        {
            InitializeComponent();
            //InitializeComponent();
            //VideoControl.Volume = 100;
            VideoControl.Width = 700;
            VideoControl.Height = 700;
        }

        private string GetVideoFilePath()
        {
            string videoFilePath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "samplevedio.mp4"
            );
            return videoFilePath;
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            VideoControl.Play();
        }

        private void PauseClick(object sender, RoutedEventArgs e)
        {
            VideoControl.Pause();
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            VideoControl.Stop();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ButtonHelper.ToggleButton(sender);
                // App.ShowProcessingDialog();
                var sessionClient = new SessionClient(HttpClientSingleton.Instance);
                var sessionResponse = await sessionClient.Session_CreateSessionAsync();
                TokenManager.SessionId = sessionResponse.Data;
                _ = App.LogError("Creating Session", LogType.Start);
                App.HideProcessingDialog();
                _ = App.TrackAtmRealTime(
                    new UpdateAtmRealTimeRequestModel() { Action = "Getting Started" }
                );
                SelectLanguage window1 = new SelectLanguage();
                window1.Show();
                ButtonHelper.ToggleButton(sender);
                Close();
            }
            catch (Exception exception)
            {
                App.AppLogger.Error(exception, exception.Message);
                App.HideProcessingDialog();
                ButtonHelper.ToggleButton(sender);
                CustomMessageBox.ShowDialog( Global.IsFrench? "Une erreur s'est produite. Veuillez réessayer ultérieurement." : "Error Occured. Please try again Later");
            }
        }

        private void Set_Denomination(object sender, RoutedEventArgs e)
        {
            SetDenomination window2 = new SetDenomination();
            window2.Show();
            this.Close();
        }

        private void OnWindowLoad(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                VideoControl.Source = new Uri(GetVideoFilePath());
                Global.IsCrypto = false;
                Global.IsDeposit = false;
                App.DisposeAll();
                _ = App.TrackAtmRealTime(
                    new UpdateAtmRealTimeRequestModel()
                    {
                        CurrentScreen = nameof(WelcomeToMobileWallet),
                        Data = "",
                    }
                );
                VideoControl.Play();
            });
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            VideoControl.Position = TimeSpan.Zero;
            VideoControl.Play();
        }
    }
}
