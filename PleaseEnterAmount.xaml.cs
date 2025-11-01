using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Threading;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Atm;
using MobileWallet.Desktop.Client;
using MobileWallet.Desktop.Helpers;

namespace MobileWallet.Desktop
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class PleaseEnterAmount : Window
    {
        private DispatcherTimer dispatchTimer;


        private CancellationTokenSource cancellationTokenSource;

        private int previousTotalAmount = 0;

        public PleaseEnterAmount()
        {
            InitializeComponent();
            Set_Language();
            cancellationTokenSource = new CancellationTokenSource();
            //StartBackgroundTask();
            //Open the bill acceptor:
            // App.CashAcceptor.open_acceptor("COM3");
            // Add a delay of 2 seconds (2000 milliseconds) after opening the acceptor
            // Thread.Sleep(2000);
            //Starting a dispatch timer
            dispatchTimer = new System.Windows.Threading.DispatcherTimer();

            dispatchTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            dispatchTimer.Interval = TimeSpan.FromMinutes(2);
            dispatchTimer.Start();
        }        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatchTimer.Stop();

            dispatchTimer.Tick -= dispatcherTimer_Tick;
            WelcomeToMobileWallet welcomeToMobileWallet = new WelcomeToMobileWallet();
            dispatchTimer.Stop();

            this.Close();
        }
        public PleaseEnterAmount(int previousTotalAmount)
        {
            InitializeComponent();
            this.previousTotalAmount = previousTotalAmount;
            //StartBackgroundTask();
            //Open the bill acceptor:
            // string result = App.CashAcceptor.open_acceptor("COM3");
            // Add a delay of 2 seconds (2000 milliseconds) after opening the acceptor
            // Thread.Sleep(2000);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PlaceYourCardAlignWithTheCamera NewWindow = new PlaceYourCardAlignWithTheCamera();
            NewWindow.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Window10 NewWindow = new Window10();
            NewWindow.Show();
            this.Close();
        }

        private void xaf100_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "100";
        }

        private void xaf500_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "500";
        }

        private void xaf2000_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "5000";
        }

        private void xaf5000_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "10000";
        }

        private void SetAccountNumber(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(value);
            string finalvalue = stringBuilder.ToString();
            txtAmount.Text += finalvalue;
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

        private void btn0_Click_1(object sender, RoutedEventArgs e)
        {
            SetAccountNumber(btn0.Content.ToString());
        }

        //private void btn0_Click(object sender, RoutedEventArgs e)
        //{
        //    SetAccountNumber(btn0.Content.ToString());
        //}

        private void btnremovenumber_Click(object sender, RoutedEventArgs e)
        {
        }


        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = string.Empty;
        }

        private void btndone_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                int length = txtAmount.Text.Length;
                string restnumbers = txtAmount.Text.Substring(0, length - 1);
                txtAmount.Text = restnumbers;
            }
        }

        private void btnclear_Click_1(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "";
        }

        private void btndone_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    int length = txtAmount.Text.Length;
                    string restnumbers = txtAmount.Text.Substring(0, length - 1);
                    txtAmount.Text = restnumbers;
                }
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
            }
        }

        private async Task HandleCryptoFlowDemo(
            int amount,
            string accountNumber,
            List<CassetteDTO> cassetteDtos
        )
        {
            var bitCoinClient = new CryptoClient(HttpClientSingleton.Instance);
            var isBuy = Global.IsDeposit;
            var data = await bitCoinClient.Crypto_GetCryptoQuoteAsync(
                isBuy,
                amount,
                Global.SelectedToken.Address,
                Global.Currency
            );
            var totalAmountToSend = data.Quote;
            if (totalAmountToSend < 0)
            {
                CustomMessageBox.Show("Cannot process Low Amount");
                return;
            }


            var dialogResult = new SelectCryptoRateInfoDialog(data, amount, true).ShowDialog();
            if (dialogResult == true)
            {
                var result = await bitCoinClient.Crypto_CreateCryptoQuoteAsync(
                    new API.CreateCryptoQuoteRequestModel()
                    {
                        SessionId = TokenManager.SessionId,
                        Address = Global.SelectedToken.Address,
                        AmountIn = data.Quote,
                        AmountOut = amount,
                        AvgPrice = data.AvgPrice,
                        IsBuy = false,
                        UserAddress = Global.UserAddress,
                        PhoneNumber = Global.CurrentAccountNumber,
                        TotalLocal = data.TotalLocal,
                        TotalUsd = data.TotalUsd,
                        Currency = Global.Currency,
                    }
                );
                if (result != null)
                {
                    if (Global.UseV2)
                    {
                        WithdrawWaitV2 withdrawWait = new WithdrawWaitV2(
                            result.Data,
                            accountNumber,
                            amount,
                            data
                        );
                        withdrawWait.Show();
                    }
                    else
                    {
                        WithdrawWait withdrawWait = new WithdrawWait(
                            result.Data,
                            accountNumber,
                            cassetteDtos,
                            amount,
                            data
                        );
                        withdrawWait.Show();
                    }


                    Close();
                    App.HideProcessingDialog();
                }
            }
            else
            {
                App.HideProcessingDialog();
            }
        }

        private CreateWithdrawRequestResponseModel result;

              private async void Button_Click_Submit(object sender, RoutedEventArgs e)
        {
            try
            {
                ButtonHelper.ToggleButton(sender);
                var isValid = ReceiptPrinter.IsValid();
                if (isValid != 0)
                {
                    CustomMessageBox.Show(
                        "Receipt is not available at the moment. Please try again later"
                    );
                    return;
                }

                string accountNumber = Global.CurrentAccountNumber;
                var amount = int.Parse(txtAmount.Text);
                if (!ValidateAmount(amount))
                {
                    App.CashAcceptor.return_escrow_stack();
                    return;
                }
                var status = App.Dispenser.ReadCassetteStatus();
                
                var msg = "";
                if (!await WithdrawHelper.IsAvailableCurrency(amount, status))
                {
                    var notesAvailables =
                        status.Where(p => p.No > 0 && (p.Status == "No Issues" || p.Status == "Low Level"))
                            .Select(p => p.No == 1 ? 2000 : p.No == 2 ? 5000 : p.No == 3 ? 10000 : -1)
                            .ToList();
                    // var notesNotAvailable = status.Where(p => p.Status == "No Issues" || p.Status == "Low Level")
                    //             .Select( p => p.No)
                    //             .ToList();
                    msg = "Notes not available, Please enter amount in " + string.Join(",", notesAvailables) + " XAF increments";
                    CustomMessageBox.Show(msg);
                    return;
                }

                App.ShowProcessingDialog();
                if (Global.IsCrypto)
                {
                    await HandleCryptoFlowDemo(amount, accountNumber, status);
                }

            }
            catch (Exception exception)
            {
                App.AppLogger.Error(exception, exception.Message);
                App.HideProcessingDialog();
                CustomMessageBox.Show("Unable to Process Withdraw");
            }
            finally
            {
                ButtonHelper.ToggleButton(sender);
            }
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            App.CashAcceptor.return_escrow_stack();
            WelcomeToMobileWallet NewWindow = new WelcomeToMobileWallet();
            NewWindow.Show();
            Close();
        }

        private void Set_Language()
        {
            try
            {
                switch (Global.DefaultLanguage)
                {
                    case "English":
                        EnterAmount.Text = ResourceEnglish.EnterAmount;
                        Submit.Content = ResourceEnglish.Submit;
                        Cancel.Content = ResourceEnglish.Cancel;
                        btndone.Content = ResourceEnglish.btndone;
                        btnclear.Content = ResourceEnglish.btnclear;

                        break;
                    case "French":
                        EnterAmount.Text = ResourceFrench.EnterAmount;
                        Submit.Content = ResourceFrench.Submit;
                        Cancel.Content = ResourceFrench.Cancel;
                        btndone.Content = ResourceFrench.btndone;
                        btnclear.Content = ResourceFrench.btnclear;
                        break;
                }
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
            }
        }
        private bool ValidateDepositAmount()
        {
            
        }
        private void xaf10000_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "15000";
        }

        private void xaf25000_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "25000";
        }

        private void xaf50000_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "50000";
        }

        private void xaf100000_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "100000";
        }

        private void xaf150000_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "150000";
        }

        private void xaf200000_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "200000";
        }
        private void AnyButtonClickedEventHandler(object sender, RoutedEventArgs e)
        {
        }

        private void OnWindowLoad(object sender, RoutedEventArgs e)
        {
            _ = App.TrackAtmRealTime(
                new UpdateAtmRealTimeRequestModel() { CurrentScreen = nameof(PleaseEnterAmount) }
            );
        }

        private void HideAllOtherControls()
        {
        }





        private void OnWindowClose(object? sender, CancelEventArgs e)
        {
        }
        private bool ValidateAmount(int amount)
        {
            if (amount < 2000)
            {
                CustomMessageBox.Show("Cannot Withdraw less than XAF 2000");
                return false;
            }

            if (amount > 500000)
            {
                CustomMessageBox.Show("Cannot Withdraw more than XAF 500000");
                return false;
            }

            if (amount % 1000 != 0)
            {
                CustomMessageBox.Show("Amount should contain at least 3 zeros");
                return false;
            }
            var rem = amount + 0;
            rem = rem % 10000;
            rem = rem % 5000;
            rem %= 2000;
            if (rem > 0)
            {
                CustomMessageBox.Show("Notes not available. Please enter amounts in multiples of 2000, 5000 and 10000");
                return false;
            }

            return true;
        }
    }
}