using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Client;

namespace MobileWallet.Desktop
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SelectLanguage : Window
    {
        public object Globals { get; private set; }

        public SelectLanguage()
        {
            InitializeComponent();
            Set_Language();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectMobileMoney mainWindow = new SelectMobileMoney();
            mainWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WelcomeToMobileWallet mainWindow = new WelcomeToMobileWallet();
            mainWindow.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            English.Focus();
            Global.DefaultLanguage = "English";
            Set_Language();
            btnnext_Click(sender, e);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Global.DefaultLanguage = "French";
            Set_Language();
            btnnext_Click(sender, e);
            // App.ProcessingDialog.Set_Language();
        }

        private void Set_Language()
        {
            English.Content = ResourceEnglish.English + "/" + ResourceFrench.English;
            Back.Content = ResourceEnglish.Back + "/" + ResourceFrench.Back;
            Cancel.Content = ResourceEnglish.Cancel + "/" + ResourceFrench.Cancel;
            btnnext.Content = ResourceEnglish.Submit + "/" + ResourceFrench.Submit;
            french.Content = ResourceEnglish.French + "/" + ResourceFrench.French;
            SelectLaunguage.Text =
                ResourceEnglish.SelectLaunguage + "/" + ResourceFrench.SelectLaunguage;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) { }

        private void btnnext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // App.ShowProcessingDialog();
                var logClient = new AppLogClient(HttpClientSingleton.Instance);
                _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Next" });
                _ = App.LogError("Next Button Pressed on Select Mobile Money", LogType.Next);
            }
            catch (Exception exception)
            {
                App.AppLogger.Error(exception, exception.Message);
                App.HideProcessingDialog();
                return;
            }
            SelectMobileMoney mainWindow = new SelectMobileMoney();
            mainWindow.Show();
            Close();
            App.HideProcessingDialog();
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Back" });
                _ = App.LogError("Back Button Pressed on Select Language", LogType.Back);
                WelcomeToMobileWallet welcomeToMobileWallet = new WelcomeToMobileWallet();
                welcomeToMobileWallet.Show();
                this.Close();
            });
        }

        private void Button_Click_1_Cancel(object sender, RoutedEventArgs e)
        {
            try
            {
                App.ShowProcessingDialog();
                _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Cancel" });
                _ = App.LogError("Cancel Button Pressed on Select Language", LogType.Cancel);
                App.HideProcessingDialog();
            }
            catch (Exception exception)
            {
                App.AppLogger.Error(exception, exception.Message);
                App.HideProcessingDialog();
            }
            WelcomeToMobileWallet welcomeToMobileWallet = new WelcomeToMobileWallet();
            welcomeToMobileWallet.Show();
            this.Close();
        }

        private void OnGotFocusHandler(object sender, RoutedEventArgs e)
        {
            Button tb = e.Source as Button;
            tb.Background = Brushes.Cyan;
        }

        // Raised when Button losses focus.
        // Changes the color of the Button back to white.
        private void OnLostFocusHandler(object sender, RoutedEventArgs e)
        {
            Button tb = e.Source as Button;
            tb.Background = Brushes.Cyan;
        }

        private void SelectLanguage_OnLoaded(object sender, RoutedEventArgs e)
        {
            Helper.AdjustRowHeight(this, DynamicRow);
            App.StartTimer(this);
            _ = App.TrackAtmRealTime(
                new UpdateAtmRealTimeRequestModel() { CurrentScreen = nameof(SelectLaunguage) }
            );
        }

        private void SelectLanguage_OnClosing(object? sender, CancelEventArgs e)
        {
            App.StopTimer();
        }
    }
}
