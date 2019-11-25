using Avalonia.Media;
using Avalonia.Media.Imaging;
using Launcher.Converters;
using Launcher.Models;
using ReactiveUI;

namespace Launcher.ViewModels
{
    public class FriendViewModel : ViewModelBase
    {
        private Bitmap _image;
        private UserStatus _status = UserStatus.Offline;
        private string _username = "";
        public int UserId;


        public FriendViewModel(string username, UserStatus userStatus, int userId,
            string imageName = "DefaultUserIcon.png")
        {
            _image = new Bitmap("Assets/50x50/" + imageName);
            _username = username;
            _status = userStatus;
            UserId = userId;
        }

        public FriendViewModel(User user)
        {
            UserId = user.UserId;
            _image = user.Image;
            _status = user.UserStatus;
            _username = user.Username;
        }

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public UserStatus Status
        {
            get => _status == UserStatus.Invisible ? UserStatus.Offline : _status;
            set
            {
                this.RaiseAndSetIfChanged(ref _status, value);
                this.RaisePropertyChanged(nameof(StatusColour));
            }
        }

        public Bitmap Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        public ISolidColorBrush StatusColour
        {
            get => UserStatusToColourConverter.ConvertColour(_status);
        }

        public void ChangeStatus(UserStatus newUserStatus)
        {
            Status = newUserStatus;
        }
    }
}