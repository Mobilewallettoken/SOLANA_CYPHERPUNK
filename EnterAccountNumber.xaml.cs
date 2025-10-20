using System.ComponentModel;
using System.Text;
using System.Windows;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Client;
using MobileWallet.Desktop.Helpers;

namespace MobileWallet.Desktop
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class EnterAccountNumber : Window
    {
        public EnterAccountNumber()
        {
            InitializeComponent();
            Set_Language();
            txtAccountNumber.MaxLength = 15;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Back" });
            if (Global.IsCrypto)
            {
                SelectCryptoRange selectCashOut = new SelectCryptoRange();
                selectCashOut.Show();
            }
            else
            {
                SelectCashOut selectCashOut = new SelectCashOut();
                selectCashOut.Show();
            }
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAccountNumber.Text))
            {
                switch (Global.DefaultLanguage)
                {
                    case "English":
                        lblMsg.Content = ResourceEnglish.lblMsg;

                        break;
                    case "French":
                        lblMsg.Content = ResourceFrench.lblMsg;

                        break;
                }
                //lblMsg.Content = "Please Enter Account Number";
            }
            else
            {
                SecureOTPhasbeensent NewWindow = new SecureOTPhasbeensent();
                NewWindow.Show();
                this.Close();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Cancel" });
            WelcomeToMobileWallet mainWindow = new WelcomeToMobileWallet();
            mainWindow.Show();
            this.Close();
        }

        private void SetAccountNumber(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(value);
            string finalvalue = stringBuilder.ToString();
            if (Global.UseV2)
            {
                if (txtAccountNumber.Text.Length >= 15)
                {
                    return;
                }
            }
            else
            {
                if (txtAccountNumber.Text.Length >= 9)
                {
                    return;
                }
            }
            txtAccountNumber.Text += finalvalue;
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn1.Content.ToString());
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn2.Content.ToString());
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn3.Content.ToString());
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn4.Content.ToString());
        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn5.Content.ToString());
        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn6.Content.ToString());
        }

        private void btn7_Click(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn7.Content.ToString());
        }

        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn8.Content.ToString());
        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn9.Content.ToString());
        }

        private void btn0_Click(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn0.Content.ToString());
        }

        private void btnremovenumber_Click(object sender, RoutedEventArgs e) { }

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            txtAccountNumber.Text = string.Empty;
        }

        private void btndone_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtAccountNumber.Text))
            {
                int length = txtAccountNumber.Text.Length;
                string restnumbers = txtAccountNumber.Text.Substring(0, length - 1);
                txtAccountNumber.Text = restnumbers;
            }
        }

        private void Set_Language()
        {
            switch (Global.DefaultLanguage)
            {
                case "English":
                    Submit.Content = ResourceEnglish.Submit;
                    Back.Content = ResourceEnglish.Back;
                    Cancel.Content = ResourceEnglish.Cancel;
                    EnterAccountNo.Text = ResourceEnglish.EnterAccountNo;
                    btndone.Content = ResourceEnglish.btndone;
                    btnclear.Content = ResourceEnglish.btnclear;

                    break;
                case "French":
                    Submit.Content = ResourceFrench.Submit;
                    Back.Content = ResourceFrench.Back;
                    Cancel.Content = ResourceFrench.Cancel;
                    EnterAccountNo.Text = ResourceFrench.EnterAccountNo;
                    btndone.Content = ResourceFrench.btndone;
                    btnclear.Content = ResourceFrench.btnclear;
                    break;
            }
        }

        private async Task NavigateToScreen()
        {
            var transTWith = Application.Current.Properties["Withdraw"];
            var transTDepo = Application.Current.Properties["Deposit"];
            if (transTWith.ToString() == "Withdraw")
            {
                App.HideProcessingDialog();
                _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Submit" });
                var smsClient = new SmsClient(HttpClientSingleton.Instance);
                if (Global.UseOtp)
                {
                    var result = await smsClient.Sms_SendVerificationSmsAsync(
                        new API.SendVerificationSmsRequestModel()
                        {
                            PhoneNumber = Global.CurrentAccountNumber,
                            SessionId = TokenManager.SessionId,
                        }
                    );
                }
                // PleaseEnterAmount NewWindow = new PleaseEnterAmount();
                // NewWindow.Show();
                // this.Close();
                SecureOTPhasbeensent NewWindow = new SecureOTPhasbeensent();
                NewWindow.Show();
                Close();
            }
            else if (transTDepo.ToString() == "Deposit")
            {
                // PleaseEnterAmount NewWindow = new PleaseEnterAmount();
                // NewWindow.Show();
                // this.Close();
                App.HideProcessingDialog();
                _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Submit" });
                if (Global.UseV2)
                {
                    PleaseInsertNoteV2 NewWindow1 = new PleaseInsertNoteV2();
                    NewWindow1.Show();
                }
                else
                {
                    PleaseInsertNote NewWindow1 = new PleaseInsertNote();
                    NewWindow1.Show();
                }
                this.Close();
            }
        }

        private async Task NavigateToScreenCrypto()
        {
            if (Global.UseOtp)
            {
                var smsClient = new SmsClient(HttpClientSingleton.Instance);
                var result = await smsClient.Sms_SendVerificationSmsAsync(
                    new API.SendVerificationSmsRequestModel()
                    {
                        PhoneNumber = Global.CurrentAccountNumber,
                        SessionId = TokenManager.SessionId,
                    }
                );
            }
            SecureOTPhasbeensent NewWindow = new SecureOTPhasbeensent();
            NewWindow.Show();
            Close();
        }

        private async void Button_Click_Submit(object sender, RoutedEventArgs e)
        {
            try
            {
                ButtonHelper.ToggleButton(sender);
                App.ShowProcessingDialog();

                if (Global.UseV2)
                {
                    if (txtAccountNumber.Text.Length < 9 || txtAccountNumber.Text.Length > 15)
                    {
                        App.HideProcessingDialog();
                        CustomMessageBox.Show(Global.IsFrench? "Le numéro saisi n'est pas valide. Veuillez vous assurer qu'il commence par un 6 et comporte au moins 9 chiffres." : "The number you entered is not valid. Please ensure the number starts with a 6 and has at least 9 digits. ");

                        txtAccountNumber.Focus();
                        return;
                    }
                }
                else
                {
                    if (txtAccountNumber.Text.Length < 9 || txtAccountNumber.Text.Length > 9)
                    {
                        App.HideProcessingDialog();
                        CustomMessageBox.Show(Global.IsFrench? "Le numéro saisi n'est pas valide. Veuillez vous assurer qu'il commence par un 6 et comporte au moins 9 chiffres." : "The number you entered is not valid. Please ensure the number starts with a 6 and has at least 9 digits. ");
                        txtAccountNumber.Focus();
                        return;
                    }
                }

                string accountNumber = "";
                if (Global.IsCrypto)
                {

                        accountNumber = "237" + txtAccountNumber.Text;
                }
                _ = App.TrackAtmRealTime(
                    new UpdateAtmRealTimeRequestModel() { Action = "Submit", Data = accountNumber }
                );
                Application.Current.Properties["AcNum"] = accountNumber;
                Global.CurrentAccountNumber = accountNumber; // get accountNumber
                if (Global.IsCrypto)
                {
                    await NavigateToScreenCrypto();
                    return;
                }
                var getStatus = await ActiveCheck(accountNumber);
                Global.Username = getStatus.Data.Name;
                if (getStatus.Data.IsActive == false)
                {
                    App.HideProcessingDialog();
                    CustomMessageBox.Show(Global.IsFrench? "Le compte n'est pas actif, veuillez contacter votre opérateur !!":
                        "Account isn't active, Please contact to your Operator !!"
                    );
                    txtAccountNumber.Focus();
                }
                else
                {
                    try
                    {
                        var canDoTransaction = await CanDoTransaction(accountNumber);
                        if (!canDoTransaction)
                        {
                            CustomMessageBox.Show( Global.IsFrench? "Désolé, vous ne pouvez plus effectuer de transactions sur ce compte":"Sorry, you cannot do any more transactions on this account");
                            return;
                        }
                        Global.CurrentAccountNumber = accountNumber;

                        if (!Global.UseOtp)
                        {
                            App.HideProcessingDialog();
                            _ = App.TrackAtmRealTime(
                                new UpdateAtmRealTimeRequestModel()
                                {
                                    Action = "Submit",
                                    Data = accountNumber,
                                }
                            );
                            await NavigateToScreen();
                            // SecureOTPhasbeensent NewWindow = new SecureOTPhasbeensent();
                            // NewWindow.Show();
                            // Close();
                            // MessageBox.Show("Please enter secured otp sent on your registered no.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        // var smsClient = new SmsClient(HttpClientSingleton.Instance);
                        // var result = await smsClient.Sms_SendVerificationSmsAsync(
                        //     new API.SendVerificationSmsRequestModel()
                        //     {
                        //         PhoneNumber = accountNumber,
                        //         SessionId = TokenManager.SessionId
                        //     });
                        App.HideProcessingDialog();
                        if (true)
                        {
                            App.HideProcessingDialog();
                            _ = App.TrackAtmRealTime(
                                new UpdateAtmRealTimeRequestModel()
                                {
                                    Action = "Submit",
                                    Data = accountNumber,
                                }
                            );
                            await NavigateToScreen();
                            // SecureOTPhasbeensent NewWindow = new SecureOTPhasbeensent();
                            // NewWindow.Show();
                            // Close();
                        }
                        else
                        {
                            App.HideProcessingDialog();
                            _ = App.TrackAtmRealTime(
                                new UpdateAtmRealTimeRequestModel()
                                {
                                    Action = "",
                                    Data = "Error Sending OTP",
                                }
                            );
                            CustomMessageBox.Show( Global.IsFrench ? "Impossible d'envoyer des SMS à ce numéro de téléphone." :
                                "Unable to send sms to this phone number.",
                                "Alert"
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        App.AppLogger.Error(ex, ex.Message);
                        App.HideProcessingDialog();
                       CustomMessageBox.Show( Global.IsFrench ? "Impossible d'envoyer des SMS à ce numéro de téléphone." :
                                "Unable to send sms to this phone number.",
                                "Alert"
                            );
                    }
                }
            }
            catch (Exception exception)
            {
                App.AppLogger.Error(exception, exception.Message);
                App.HideProcessingDialog();
                CustomMessageBox.Show(Global.IsFrench ? "Une erreur s'est produite" : "Something went wrong!");
            }
            finally
            {
                ButtonHelper.ToggleButton(sender);
                App.HideProcessingDialog();
            }
        }

        public async Task<GetMtnUserInfoResponseModel> ActiveCheck(string number)
        {
            var client = new MtnClient(HttpClientSingleton.Instance);
            var result = await client.Mtn_GetUserInfoAsync(number);
            return result;
        }

        public async Task<bool> CanDoTransaction(string number)
        {
            var client = new AppTransactionClient(HttpClientSingleton.Instance);
            var result = await client.AppTransaction_CanDoTransactionAsync(
                new CanDoTransactionRequestModel() { PhoneNumber = number }
            );
            return result.Data;
        }

        private void OnWindowLoad(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                App.StartTimer(this);
                _ = App.TrackAtmRealTime(
                    new UpdateAtmRealTimeRequestModel()
                    {
                        CurrentScreen = nameof(EnterAccountNumber),
                    }
                );
            });
        }

        private void EnterAccountNumber_OnClosing(object? sender, CancelEventArgs e)
        {
            App.StopTimer();
        }
    }
}
