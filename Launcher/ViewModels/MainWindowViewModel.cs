using System.Reactive.Disposables;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Launcher.Converters;
using Launcher.Lib;
using Launcher.Models;
using ReactiveUI;

namespace Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
    {
        private ApiAccessor _apiAccessor = ApiAccessor.GetApiAccessor;
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
            GetFriendsList();
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

        private async void GetFriendsList()
        {
            FriendsListViewModel.LoadFriends(await _apiAccessor.GetFriends());
        }
    }
}