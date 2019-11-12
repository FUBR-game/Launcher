using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia.Media;
using Launcher.Converters;
using Launcher.Models;
using ReactiveUI;

namespace Launcher.ViewModels
{
    public class FriendsListViewModel : ViewModelBase, IActivatableViewModel
    {
        private ObservableCollection<FriendViewModel> _onlineFriends = new ObservableCollection<FriendViewModel>();
        private ObservableCollection<FriendViewModel> _offlineFriends = new ObservableCollection<FriendViewModel>();

        private User _currentUser;

        public FriendsListViewModel()
        {
            Activator = new ViewModelActivator();
            this.WhenActivated(disposables =>
            {
                HandleActivation();
                /* handle activation */
                Disposable
                    .Create(HandleActivation)
                    .DisposeWith(disposables);
            });
        }

        private void HandleActivation()
        {
        }
    
        private void HandleDeactivation() { }


        public User CurrentUser
        {
            get => _currentUser;
            set => this.RaiseAndSetIfChanged(ref _currentUser, value);
        }

        public ObservableCollection<FriendViewModel> OfflineFriends
        {
            get => _offlineFriends;
            private set => this.RaiseAndSetIfChanged(ref _offlineFriends, value);
        }


        public ObservableCollection<FriendViewModel> OnlineFriends
        {
            get => _onlineFriends;
            private set => this.RaiseAndSetIfChanged(ref _onlineFriends, value);
        }

        public string Test
        {
            get { return "test"; }
        }

        public void FriendGoesOnline(FriendViewModel friend)
        {
            if (_offlineFriends.Contains(friend))
            {
                _offlineFriends.Remove(friend);
            }

            if (!_onlineFriends.Contains(friend))
            {
                _onlineFriends.Add(friend);
            }

            friend.ChangeStatus(UserStatus.Online);
            
            this.RaisePropertyChanged(nameof(OfflineFriends));
            this.RaisePropertyChanged(nameof(OnlineFriends));
        }

        public void FriendGoesOffline(FriendViewModel friend)
        {
            if (_onlineFriends.Contains(friend))
            {
                _onlineFriends.Remove(friend);
            }

            if (!_offlineFriends.Contains(friend))
            {
                _offlineFriends.Add(friend);
            }

            friend.ChangeStatus(UserStatus.Offline);

            this.RaisePropertyChanged(nameof(OnlineFriends));
            this.RaisePropertyChanged(nameof(OfflineFriends));
        }

        public ViewModelActivator Activator { get; }

        public ISolidColorBrush StatusColour
        {
            get => UserStatusToColourConverter.ConvertColour(CurrentUser.UserStatus);
        }
    }
}