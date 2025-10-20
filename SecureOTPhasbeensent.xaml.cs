using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Client;
using MobileWallet.Desktop.Helpers;
using Newtonsoft.Json;

namespace MobileWallet.Desktop
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class SecureOTPhasbeensent : Window
    {
        private readonly string _password;
        private readonly string _userName;
        private readonly bool _isLogin;

        public SecureOTPhasbeensent(
            string userName = "",
            string password = "",
            bool isLogin = false
        )
        {
            _userName = userName;
            _isLogin = isLogin;
            _password = password;
            InitializeComponent();
            Set_Language();
            if (isLogin)
            {
                ResendOTPBorder.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectLanguage NewWindow = new SelectLanguage();
            NewWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAccountNumber.Text))
            {
                switch (Global.DefaultLanguage)
                {
                    case "English":
                        lblMsgOTP.Content = ResourceEnglish.lblMsgOTP;

                        break;
                    case "French":
                        lblMsgOTP.Content = ResourceFrench.lblMsgOTP;

                        break;
                }
                //  lblMsgOTP.Content = "Please Enter OTP recieved on mobile";
            }
            else
            {


                    PleaseEnterAmount NewWindow = new PleaseEnterAmount();
                    NewWindow.Show();
                this.Close();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SelectMobileMoney mainWindow = new SelectMobileMoney();
            mainWindow.Show();
            this.Close();
        }

        private void SetAccountNumber(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(value);
            string finalvalue = stringBuilder.ToString();
            if (txtAccountNumber.Text.Length >= 6)
            {
                return;
            }

            txtAccountNumber.Text += finalvalue;
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

        private void btnremovenumber_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtAccountNumber.Text))
            {
                int length = txtAccountNumber.Text.Length;
                string restnumbers = txtAccountNumber.Text.Substring(0, length - 1);
                txtAccountNumber.Text = restnumbers;
            }
        }

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            txtAccountNumber.Text = string.Empty;
        }

        private void btn1_Click_1(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn1.Content.ToString());
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            if (_isLogin)
            {
                Login NewWindow = new Login();
                NewWindow.Show();
                this.Close();
            }
            else
            {
                _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Back" });
                EnterAccountNumber NewWindow = new EnterAccountNumber();
                NewWindow.Show();
                this.Close();
            }
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            if (_isLogin)
            {
                Login NewWindow = new Login();
                NewWindow.Show();
                this.Close();
            }
            else
            {
                _ = App.TrackAtmRealTime(new UpdateAtmRealTimeRequestModel() { Action = "Cancel" });
                WelcomeToMobileWallet NewWindow = new WelcomeToMobileWallet();
                NewWindow.Show();
                this.Close();
            }
        }

        private async void Button_Click_Resendotp(object sender, RoutedEventArgs e)
        {
            try
            {
                #region TimerToResendOtp

                App.ShowProcessingDialog();
                System.Timers.Timer time = new System.Timers.Timer();
                var btn = (Button)sender;
                btn.IsEnabled = false; //Disable button.
                var fooTimer = new System.Timers.Timer(5000); //Exceute after 5000 milliseconds
                time.Elapsed += (fooTimer_s, fooTimer_e) =>
                {
                    //It has to be dispatched because of thread crossing if you are using WPF.
                    Dispatcher.Invoke(() =>
                    {
                        btn.IsEnabled = true; //Bring button back to life by enabling it.
                        fooTimer.Dispose();
                    });
                };
                fooTimer.Start();

                #endregion

                #region ResendOtp

                var accountNum = Global.CurrentAccountNumber;
                var smsClient = new SmsClient(HttpClientSingleton.Instance);
                _ = App.TrackAtmRealTime(
                    new UpdateAtmRealTimeRequestModel() { Action = "Resend OTP" }
                );
                var result = await smsClient.Sms_SendVerificationSmsAsync(
                    new API.SendVerificationSmsRequestModel()
                    {
                        PhoneNumber = accountNum,
                        SessionId = TokenManager.SessionId,
                    }
                );
                if (result != null)
                {
                    txtAccountNumber.Text = "";
                    App.HideProcessingDialog();
                    CustomMessageBox.Show(
                        Global.IsFrench? "Veuillez saisir le mot de passe à usage unique sécurisé (OTP) envoyé sur votre numéro enregistré.":
                        "Please enter secured otp sent on your registered no.",
                        "Alert"
                    );
                }

                #endregion
            }
            catch (Exception exception)
            {
                App.AppLogger.Error(exception, exception.Message);
                App.HideProcessingDialog();
                CustomMessageBox.Show(
                    Global.IsFrench? "Une erreur s'est produite" :
                    "Something went wrong!", "Alert");
            }
            finally
            {
                App.HideProcessingDialog();
            }
        }

        private async void Button_Click_Submit(object sender, RoutedEventArgs e)
        {
            VerifyOtpResponseModel result;
            try
            {
                ButtonHelper.ToggleButton(sender);
                var transTDepo = Application.Current.Properties["Deposit"];
                var transTWith = Application.Current.Properties["Withdraw"];
                if (txtAccountNumber.Text.Length > 6 || txtAccountNumber.Text.Length < 6)
                {
                    CustomMessageBox.ShowDialog( Global.IsFrench? "Veuillez saisir un mot de passe (OTP) à 6 chiffres !!":"Please enter 6 digit otp !!");
                    txtAccountNumber.Focus();
                }
                else
                {
                    App.ShowProcessingDialog();
                    if (_isLogin)
                    {
                        var authResult = await new AppAuthClient().Authenticate(
                            _userName,
                            _password,
                            txtAccountNumber.Text
                        );
                        if (authResult)
                        {
                            App.HideProcessingDialog();
                            WelcomeToMobileWallet window2 = new WelcomeToMobileWallet();
                            window2.Show();
                            this.Close();
                        }
                        else
                        {
                            App.HideProcessingDialog();
                            CustomMessageBox.ShowDialog( Global.IsFrench? "Identifiants invalides" : "Invalid Credentials");
                        }

                        return;
                    }

                    // API.OtpVerificationStatusCheckResponse result;
                    if (Global.UseOtp)
                    {
                        var smsClient = new SmsClient(HttpClientSingleton.Instance);
                        result = await smsClient.Sms_VerifyOtpAsync(
                            new API.VerifyOtpRequestModel()
                            {
                                Otp = txtAccountNumber.Text,
                                PhoneNumber = Global.CurrentAccountNumber,
                                SessionId = TokenManager.SessionId,
                            }
                        );
                    }
                    else
                    {
                        result = new VerifyOtpResponseModel();
                    }

                    if (result != null)
                    {
                        if (true)
                        {
                            if (Global.IsCrypto)
                            {
                                App.HideProcessingDialog();
                                if (Global.IsCryptoAbove)
                                {
                                    SelectYourID window = new SelectYourID();
                                    window.Show();
                                    this.Close();
                                }
                                else
                                {
                                    SelectCryptoNetwork window = new SelectCryptoNetwork();
                                    window.Show();
                                    this.Close();
                                }
                            }
                            else if (transTWith.ToString() == "Withdraw")
                            {
                                App.HideProcessingDialog();
                                _ = App.TrackAtmRealTime(
                                    new UpdateAtmRealTimeRequestModel() { Action = "Submit" }
                                );
                                if (Global.UseV2)
                                {
                                    PleaseEnterAmountV2 NewWindow = new PleaseEnterAmountV2();
                                    NewWindow.Show();
                                }
                                else
                                {
                                    PleaseEnterAmount NewWindow = new PleaseEnterAmount();
                                    NewWindow.Show();
                                }
                                Close();
                            }
                            else if (transTDepo.ToString() == "Deposit")
                            {
                                // PleaseEnterAmount NewWindow = new PleaseEnterAmount();
                                // NewWindow.Show();
                                // this.Close();
                                App.HideProcessingDialog();
                                _ = App.TrackAtmRealTime(
                                    new UpdateAtmRealTimeRequestModel() { Action = "Submit" }
                                );
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
                        else
                        {
                            App.HideProcessingDialog();
                            CustomMessageBox.Show(
                                Global.IsFrench ? "Échec de la validation du mot de passe personnel (OTP) ! Veuillez saisir le mot de passe personnel correct." : 
                                "Otp validation failed! Please enter the correct Otp.",
                                "Alert",
                                MessageBoxButton.OK
                            );
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                App.AppLogger.Error(exception, exception.Message);
                App.HideProcessingDialog();
                CustomMessageBox.Show(
                    Global.IsFrench ? "Échec de la validation du mot de passe personnel (OTP) ! Veuillez saisir le mot de passe personnel correct." : 
                    "Otp validation failed! Please enter the correct Otp.",
                    "Alert",
                    MessageBoxButton.OK
                );
            }
            finally
            {
                ButtonHelper.ToggleButton(sender);
                App.HideProcessingDialog();
            }
        }

        private void Set_Language()
        {
            switch (Global.DefaultLanguage)
            {
                case "English":
                    {
                        Submit.Content = ResourceEnglish.Submit;
                        Back.Content = ResourceEnglish.Back;
                        Cancel.Content = ResourceEnglish.Cancel;
                        ResendOTP.Content = ResourceEnglish.ResendOTP;
                        SecureOTPSend.Text = ResourceEnglish.SecureOTPSend;
                        btndone.Content = ResourceEnglish.btndone;
                        btnclear.Content = ResourceEnglish.btnclear;
                        lblMsgOTP.Content = ResourceEnglish.lblMsgOTP;
                        if (!_isLogin)
                        {
                            SecureOTPSend.Text = "";
                            var text = (
                                Global.IsCrypto
                                    ? ResourceEnglish.SecureOTPSendWithPhone
                                    : ResourceEnglish.SecureOTPSendWithName
                            )
                                .Replace("{Name}", Global.Username)
                                .Replace("{Phone}", Global.CurrentAccountNumber);
                            var split = text.Split("\\n");
                            SecureOTPSend.Inlines.Clear();
                            for (var index = 0; index < split.Length; index++)
                            {
                                var s = split[index];
                                SecureOTPSend.Inlines.Add(s);
                                if (index != split.Length - 1)
                                {
                                    SecureOTPSend.Inlines.Add(new LineBreak());
                                }
                            }
                        }
                    }
                    break;
                case "French":
                    {
                        Submit.Content = ResourceFrench.Submit;
                        Back.Content = ResourceFrench.Back;
                        Cancel.Content = ResourceFrench.Cancel;
                        ResendOTP.Content = ResourceFrench.ResendOTP;
                        SecureOTPSend.Text = ResourceFrench.SecureOTPSend;
                        btndone.Content = ResourceFrench.btndone;
                        btnclear.Content = ResourceFrench.btnclear;
                        lblMsgOTP.Content = ResourceFrench.lblMsgOTP;
                        if (!_isLogin)
                        {
                            SecureOTPSend.Text = "";
                            var text = (
                                Global.IsCrypto
                                    ? ResourceFrench.SecureOTPSendWithPhone
                                    : ResourceFrench.SecureOTPSendWithName
                            )
                                .Replace("{Name}", Global.Username)
                                .Replace("{Phone}", Global.CurrentAccountNumber);
                            var split = text.Split("\\n");
                            SecureOTPSend.Inlines.Clear();
                            for (var index = 0; index < split.Length; index++)
                            {
                                var s = split[index];
                                SecureOTPSend.Inlines.Add(s);
                                if (index != split.Length - 1)
                                {
                                    SecureOTPSend.Inlines.Add(new LineBreak());
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            _ = App.TrackAtmRealTime(
                new UpdateAtmRealTimeRequestModel() { CurrentScreen = nameof(SecureOTPhasbeensent) }
            );
            if (Global.IsTest)
            {
                txtAccountNumber.Text = "000000";
            }
        }

        private void Btndone_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtAccountNumber.Text))
            {
                int length = txtAccountNumber.Text.Length;
                string restnumbers = txtAccountNumber.Text.Substring(0, length - 1);
                txtAccountNumber.Text = restnumbers;
            }
        }
    }
}
