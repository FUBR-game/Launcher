using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia.Threading;
using Launcher.Lib;
using Launcher.Models;
using ReactiveUI;

namespace Launcher.ViewModels
{
    public class FriendsListViewModel : ViewModelBase, IActivatableViewModel
    {
        private ObservableCollection<FriendViewModel> _offlineFriends = new ObservableCollection<FriendViewModel>();
        private ObservableCollection<FriendViewModel> _onlineFriends = new ObservableCollection<FriendViewModel>();
        private List<User> friends = new List<User>();

        public FriendsListViewModel()
        {
            Activator = new ViewModelActivator();
            this.WhenActivated(disposables =>
            {
                HandleActivation();
                /* handle activation */
                Disposable
                    .Create(HandleDeactivation)
                    .DisposeWith(disposables);
            });
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

        public ViewModelActivator Activator { get; }

        private void HandleActivation()
        {
        }

        private void HandleDeactivation()
        {
        }

        public void FriendGoesOnline(FriendViewModel friend, UserStatus userStatus)
        {
            if (_offlineFriends.Contains(friend))
            {
                _offlineFriends.Remove(friend);
            }

            if (!_onlineFriends.Contains(friend))
            {
                _onlineFriends.Add(friend);
            }

            friend.ChangeStatus(userStatus);
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
        }

        public void LoadFriends(List<User> friends)
        {
            this.friends = friends;
            var serverCom = ServerCommunicator.GetServerCommunicator();
            serverCom.FriendChangesStatus += ServerComOnFriendChangesStatus;
            OnlineFriends = new ObservableCollection<FriendViewModel>();
            foreach (var friend in friends)
            {
                serverCom.GetUserStatus(friend);
                friend.UserStatus = UserStatus.Offline;
                OfflineFriends.Add(new FriendViewModel(friend));
            }
        }

        private FriendViewModel FindFriend(int userId)
        {
            foreach (var offlineFriend in _offlineFriends)
            {
                if (offlineFriend.UserId == userId)
                {
                    return offlineFriend;
                }
            }

            foreach (var onlineFriend in _onlineFriends)
            {
                if (onlineFriend.UserId == userId)
                {
                    return onlineFriend;
                }
            }

            return null;
        }

        private void ServerComOnFriendChangesStatus(object sender, StatusChangeEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var friend = FindFriend(e.UserId);
                switch (e.UserStatus)
                {
                    case UserStatus.Online:
                        FriendGoesOnline(friend, e.UserStatus);
                        break;
                    case UserStatus.Away:
                        FriendGoesOnline(friend, e.UserStatus);
                        break;
                    case UserStatus.Busy:
                        FriendGoesOnline(friend, e.UserStatus);
                        break;
                    case UserStatus.Offline:
                        FriendGoesOffline(friend);
                        break;
                    case UserStatus.Invisible:
                        FriendGoesOffline(friend);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }
    }
}