using Avalonia.Media;
using Avalonia.Media.Imaging;
using Launcher.Converters;
using Launcher.Models;
using ReactiveUI;

namespace Launcher.ViewModels
{
    public class FriendViewModel : ViewModelBase
    {
        private string _username = "";
        private UserStatus _status = UserStatus.Offline;
        private Bitmap _image;

        public FriendViewModel(string username, UserStatus userStatus, string imageName = "DefaultUserIcon.png")
        {
            _image = new Bitmap("Assets/50x50/" + imageName);
            _username = username;
            _status = userStatus;
        }
        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public UserStatus Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
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