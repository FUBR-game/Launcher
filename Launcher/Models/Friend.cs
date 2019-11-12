using System;
using System.Drawing.Text;
using Avalonia.Media.Imaging;

namespace Launcher.Models
{
    public class Friend
    {
        private Bitmap _image;
        private string _username;
        private UserStatus _userStatus;

        public Friend(string username, UserStatus userStatus, string imageName = "DefaultUserIcon.png")
        {
            _image = new Bitmap("Assets/50x50/" + imageName);
            _username = username;
            _userStatus = userStatus;
        }
    }
}