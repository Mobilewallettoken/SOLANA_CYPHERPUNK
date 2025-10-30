using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace MobileWallet.Desktop.Helpers;

public class ButtonHelper
{
    public static void ToggleButton(object button)
    {
        try
        {
            if (button is Button btn)
            {
                btn.Dispatcher.Invoke(() =>
                {
                    btn.IsEnabled = !btn.IsEnabled;
                });
            }
        }
        catch (Exception e)
        {
                App.AppLogger.Error(e,e.Message);
            // ignored
        }
    }
}

public static class NavigationHelper
{
    public static void NavigateTo(this Window from, Window to)
    {
        to.Loaded += (s, e) => from.Close();
        to.Show();
    }
}
