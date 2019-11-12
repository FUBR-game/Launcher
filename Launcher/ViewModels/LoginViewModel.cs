using System;
using Avalonia;
using Avalonia.Controls;
using Launcher.Models;
using Launcher.Views;
using static Launcher.Lib.Authentication;

namespace Launcher.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public async void OnClickLogin()
        {
            var LoggedInUser = await Login();
            User.SetCurrentUser(LoggedInUser);
            startMainwindow();
        }

        public async void OnClickRegister()
        {
            var LoggedInUser = await Login();
            User.SetCurrentUser(LoggedInUser);
            startMainwindow();
        }

        private void startMainwindow()
        {
            var MainWindowDataContext = new MainWindowViewModel();

            var mainWindow = new MainWindow
            {
                DataContext = MainWindowDataContext
            };

            OnUserHasLoggedIn();
            Application.Current.Run(mainWindow);
        }

        public event EventHandler UserHasLoggedIn;

        protected virtual void OnUserHasLoggedIn()
        {
            UserHasLoggedIn?.Invoke(this, EventArgs.Empty);
        }
    }
}