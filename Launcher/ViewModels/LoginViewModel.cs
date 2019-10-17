using Avalonia;
using Launcher.Views;
using static Launcher.Lib.Authentication;

namespace Launcher.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public static async void OnClickLogin()
        {
            await Login();
            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };

            mainWindow.Show();

            foreach (var window in Application.Current.Windows)
            {
                if (window.Tag == null || !window.Tag.Equals("LOGIN_WINDOW")) continue;
                window.Close();
                break;
            }
        }

        public static async void OnClickRegister()
        {
            await Login();
        }
    }
}