using System.Reactive.Disposables;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Launcher.Converters;
using Launcher.Models;
using ReactiveUI;

namespace Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
    {
        private bool _needsUpdate = false;

        public MainWindowViewModel()
        {
            Activator = new ViewModelActivator();
            this.WhenActivated(disposables =>
            {
                /* handle activation */
                Disposable
                    .Create(() =>
                    {
                        /* handle deactivation */
                    })
                    .DisposeWith(disposables);
            });
            FriendsListViewModel = new FriendsListViewModel();
        }

        public static User CurrentUser => User.GetCurrentUser();

        public FriendsListViewModel FriendsListViewModel { get; }

        public string CurrentUsername
        {
            get => CurrentUser.Username;
        }

        public UserStatus CurrentUserStatus
        {
            get => CurrentUser.UserStatus;
        }

        public Bitmap CurrentUserImage
        {
            get => CurrentUser.Image;
        }

        public ISolidColorBrush StatusColour => UserStatusToColourConverter.ConvertColour(CurrentUser.UserStatus);

        public bool NeedUpdating
        {
            get => _needsUpdate;
        }

        public bool CanPLay
        {
            get => !_needsUpdate;
        }

        public ViewModelActivator Activator { get; }
    }
}