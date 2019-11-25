using System.Buffers.Text;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using Launcher.Lib;
using Newtonsoft.Json;
using static System.Text.Encoding;

namespace Launcher.Models
{
    public class User
    {
        private static User _currentUser = new User();
        [JsonProperty("google_token")] private string _googleToken;
        [JsonProperty("email")] public string Email;
        [JsonProperty("game_currency")] public int GameCurrency;
        public Bitmap Image;
        [JsonProperty("premium_currency")] public int PremiumCurrency;
        [JsonProperty("id")] public int UserId;
        [JsonProperty("username")] public string Username;
        public UserStatus UserStatus = UserStatus.Online;

        private User()
        {
        }

        public string GetGoogleToken()
        {
            return _googleToken;
        }

        public static User UserFromJsonString(string jsonString)
        {
            var user = JsonConvert.DeserializeObject<User>(jsonString);

            user.LoadImage();

            return user;
        }

        public static List<User> UsersFromJsonString(string jsonString)
        {
            var users = JsonConvert.DeserializeObject<List<User>>(jsonString);
            foreach (var user in users)
            {
                user.LoadImage();
            }

            return users;
        }

        private void LoadImage()
        {
            var cache = new CacheHandler();
            Image = cache.GetUserIcon(this);
        }


        public static User UserFromBase64String(string base64String)
        {
            var encodedBytes = UTF8.GetBytes(base64String);
            var userJsonBytes = new byte[encodedBytes.Length];
            Base64.DecodeFromUtf8(encodedBytes, userJsonBytes, out var bytesConsumed, out var bytesWritten);

            var userJsonString = UTF8.GetString(userJsonBytes);
            return UserFromJsonString(userJsonString);
        }

        public static void SetCurrentUser(User user)
        {
            _currentUser = user;
        }

        public static User GetCurrentUser()
        {
            return _currentUser;
        }

        public static void changeUserStatus(UserStatus newStatus)
        {
            _currentUser.UserStatus = newStatus;
        }
    }
}