using System.Windows;
using MobileWallet.Desktop.API;

namespace MobileWallet.Desktop;

public partial class SelectCryptoRateInfoDialog : Window
{
    private GetCryptoQuoteResponseModel _data;
    private double _amount;

    public SelectCryptoRateInfoDialog(
        GetCryptoQuoteResponseModel data,
        double amount,
        bool isYesOrNo = false,
        string title = ""
    )
    {
        InitializeComponent();
        _data = data;
        _amount = amount;
        SetLanguage();
        SetValue();
        BtnNo.Visibility = BtnYes.Visibility = Visibility.Hidden;
        if (isYesOrNo)
        {
            BtnNo.Visibility = Visibility.Visible;
        }
        else
        {
            BtnYes.Visibility = Visibility.Visible;
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            MyHeader.Text = title;
        }
    }

    private void SetValue()
    {
        if (Global.UseV2)
        {
            TxtAmountValue.Text = "$" + _amount.ToString("N");
        }
        else
        {
            TxtAmountValue.Text = _amount.ToString("N") + " " + Global.Currency;
        }
        TxtMarketValue.Text = _data.Market;
        TxtFeeValue.Text = "$" + _data.GasFee.ToString("N");
        TxtSlippageValue.Text = _data.Slippage;
        TxtAveragePriceValue.Text =
            $"1 {Global.SelectedToken.Symbol} = $" + _data.AvgPrice.ToString("F4");
        // TxtTotalLocalValue.Text = "" + _data.TotalLocal + " XAF";
        TxtTotalUsdValue.Text = "$" + _data.TotalUsd;
        TxtQuoteValue.Text = Global.SelectedToken.Symbol + " " + _data.Quote.ToString("N6");
        TxtExchangeRateValue.Text = "1 USD = " + _data.ExchangeRate + " " + Global.Currency;
        // TxtPaymentMethodValue.Text = _data.PaymentMethod;
    }

    private void SetLanguage()
    {
        if (Global.IsFrench)
        {
            TxtAmountLabel.Text = ResourceFrench.Amount;
            TxtMarketLabel.Text = ResourceFrench.Market;
            TxtFeeLabel.Text = ResourceFrench.EstFee;
            TxtSlippageLabel.Text = ResourceFrench.EstSlippage;
            TxtAveragePriceLabel.Text = ResourceFrench.EstAveragePrice;
            // TxtTotalLocalLabel.Text = ResourceFrench.TotalLocal;
            TxtTotalUsdLabel.Text = ResourceFrench.TotalUsd;
            TxtExchangeRate.Text = ResourceFrench.ExchangeRate;
            if (Global.IsDeposit)
            {
                TxtQuoteLabel.Text =
                    Global.SelectedToken.Symbol + " " + ResourceFrench.QuoteDeposit;
            }
            else
            {
                TxtQuoteLabel.Text =
                    Global.SelectedToken.Symbol + " " + ResourceFrench.QuoteWithdraw;
            }
            // TxtPaymentMethodLabel.Text = ResourceFrench.PaymentMethod;
        }
        else
        {
            TxtAmountLabel.Text = ResourceEnglish.Amount;
            TxtMarketLabel.Text = ResourceEnglish.Market;
            TxtFeeLabel.Text = ResourceEnglish.EstFee;
            TxtSlippageLabel.Text = ResourceEnglish.EstSlippage;
            TxtAveragePriceLabel.Text = ResourceEnglish.EstAveragePrice;
            // TxtTotalLocalLabel.Text = ResourceEnglish.TotalLocal;
            TxtTotalUsdLabel.Text = ResourceEnglish.TotalUsd;
            TxtExchangeRate.Text = ResourceEnglish.ExchangeRate;
            if (Global.IsDeposit)
            {
                TxtQuoteLabel.Text =
                    Global.SelectedToken.Symbol + " " + ResourceEnglish.QuoteDeposit;
            }
            else
            {
                TxtQuoteLabel.Text =
                    Global.SelectedToken.Symbol + " " + ResourceEnglish.QuoteWithdraw;
            }
            // TxtPaymentMethodLabel.Text = ResourceEnglish.PaymentMethod;
        }
    }

    private void Yes_OnClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
        this.Close();
    }

    private void No_OnClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
    }
}
