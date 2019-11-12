using System.Reactive.Disposables;
using Launcher.Models;
using ReactiveUI;

namespace Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        private User _currentUser;
        public MainWindowViewModel()
        {
            Activator = new ViewModelActivator();
            this.WhenActivated(disposables =>
            {
                /* handle activation */
                Disposable
                    .Create(() => { /* handle deactivation */ })
                    .DisposeWith(disposables);
            });
            FriendsListViewModel = new FriendsListViewModel();
            CurrentUser = User.GetCurrentUser();
        }
        
        public FriendsListViewModel FriendsListViewModel { get; }

        public User CurrentUser
        {
            get => _currentUser;
            set => this.RaiseAndSetIfChanged(ref _currentUser, value);
        }
    }
}