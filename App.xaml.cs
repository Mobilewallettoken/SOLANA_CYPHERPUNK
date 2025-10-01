using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Emgu.CV;
using ITL;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Atm;
using MobileWallet.Desktop.Client;
using MobileWallet.Desktop.MPostLib;
using NLog;
using Python.Runtime;

namespace MobileWallet.Desktop
{
    public class Helper
    {
        public static void AdjustRowHeight(
            Window window,
            RowDefinition rowDefinition,
            int height = 500,
            int star = 3
        )
        {
            // Check the window's current height and adjust the row height
            if (window.ActualWidth > window.ActualHeight) // Landscape
            {
                rowDefinition.Height = new GridLength(height, GridUnitType.Pixel); // Dynamic height for landscape
            }
            else
            {
                rowDefinition.Height = new GridLength(star, GridUnitType.Star); // Dynamic height for landscape
            }
        }


        public void CloseWindow(App app, Window? window)
        {
            app.Dispatcher.Invoke(() =>
            {
                WelcomeToMobileWallet NewWindow = new WelcomeToMobileWallet();
                NewWindow.Show();
                window?.Close();
            });
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static DispatcherTimer timer = new DispatcherTimer();
        private static Window? _currentWindow;
        public static App Instance;

        private static CashAcceptor? _cashAcceptor;
        public static void StartTimer(Window window)
        {
            _currentWindow = window;
            timer.Start();
        }

        public static void StopTimer()
        {
            Instance.Dispatcher.Invoke(() =>
            {
                _currentWindow = null;
                timer.Stop();
            });
        }

        private void TimerOnElapsed(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                new Helper().CloseWindow(this, _currentWindow);
                StopTimer();
            });
        }

        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            timer.Interval = TimeSpan.FromSeconds(60);
            timer.Tick += TimerOnElapsed;
            Instance = this;
        }
        private void App_DispatcherUnhandledException(
            object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e
        )
        {
            // Handle the exception
            if (e != null)
            {
                AppLogger.Error(e.Exception, e.Exception.Message);
                HandleException(e.Exception);
                e.Handled = true;
            }
            // Prevent default unhandled exception processing
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Handle the exception
            if (e.ExceptionObject is Exception ex)
            {
                AppLogger.Error(ex, ex.Message);
                HandleException(ex);
            }
        }

        private void HandleException(Exception ex)
        {
            // Log the exception
            // Display a friendly message to the user
            HideProcessingDialog();
            _ = LogError($"An unexpected error occurred: {ex.Message}");
            CustomMessageBox.ShowDialog(
                "An unexpected error occurred. Please contact support.",
                "Error"
            );
            // Optionally, shutdown the application
            // Application.Current.Shutdown();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            PythonEngine.Shutdown();
        }
    }
}
