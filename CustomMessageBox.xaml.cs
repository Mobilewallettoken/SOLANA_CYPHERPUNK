using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MobileWallet.Desktop;

public partial class CustomMessageBox : Window
{
    public CustomMessageBox()
    {
        InitializeComponent();
        HideAllButton();
    }

    public static bool? ShowDialog(string message, string title = "", MessageBoxButton buttons = MessageBoxButton.OK)
    {
        var dialog = new CustomMessageBox();
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.TxtTitle.Text = title;
        dialog.TxtContent.Inlines.Clear();
        var ses = message.Split('\n');
        for (var index = 0; index < ses.Length; index++)
        {
            var se = ses[index];
            dialog.TxtContent.Inlines.Add(se);
            if (index != ses.Length - 1)
            {
                dialog.TxtContent.Inlines.Add(new LineBreak());
            }
        }
        if (Global.IsFrench)
        {
            dialog.BtnYesContent.Content = "Oui";
            dialog.BtnNoContent.Content = "Non";
            dialog.BtnOkContent.Content = "Ok";
        }
        else
        {
            dialog.BtnYesContent.Content = "Yes";
            dialog.BtnNoContent.Content = "No";
            dialog.BtnOkContent.Content = "OK";
        }
        switch (buttons)
        {
            case MessageBoxButton.OK:
                dialog.BtnYes.Visibility = Visibility.Visible;
                break;
            case MessageBoxButton.OKCancel:
                dialog.BtnNo.Visibility = Visibility.Visible;
                break;
            case MessageBoxButton.YesNo:
                dialog.BtnNo.Visibility = Visibility.Visible;
                break;
        }
        return dialog.ShowDialog();
    }
    public static bool? Show(string message, string title = "", MessageBoxButton buttons = MessageBoxButton.OK)
    {
        return ShowDialog(message, title, buttons);
    }

    public void HideAllButton()
    {
        BtnYes.Visibility = Visibility.Hidden;
        BtnNo.Visibility = Visibility.Hidden;
    }
    
    private void Yes_OnClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
        Close();
    }
    
    private void No_OnClick(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        Close();
    }
}