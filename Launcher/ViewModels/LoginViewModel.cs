using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Launcher.Views;

namespace Launcher.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public void OnClickLogin()
        {
            OpenBrowser(@"http://google.com/");
            var mainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel(),
            };
            
            mainWindow.Show();

            foreach (var window in Application.Current.Windows)
            {
                if (window.Tag == null || !window.Tag.Equals("LOGIN_WINDOW")) continue;
                window.Close();
                break;
            }
        }

        public void OnClickRegister()
        {
            OpenBrowser(@"http://google.com/");
        }

        public static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd",
                    $"/c start {url.Replace("&", "^&")}")); // Works ok on windows and escape need for cmd.exe
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url); // Works ok on linux
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url); // Not tested
            }
            else
            {
                //TODO catch all errors
            }
        }
    }
}