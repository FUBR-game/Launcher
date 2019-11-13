using System.Buffers.Text;
using Avalonia.Media.Imaging;
using Launcher.Lib;
using Newtonsoft.Json;
using static System.Text.Encoding;

namespace Launcher.Models
{
    internal class JsonUser
    {
#pragma warning disable 649
        public string created_at;
        public int game_currency;
        public string google_token;
        public int id;
        public string last_online;
        public int premium_currency;
        public string updated_at;
        public string username;
        public string gravatar_icon;
        public string email;
#pragma warning restore 649
    }

    public class User
    {
        private static User _currentUser = new User();
        private string _googleToken;
        public string email;

        public int GameCurrency;
        public Bitmap Image;
        public int PremiumCurrency;
        public int UserId;
        public string Username;
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
            var jsonUser = JsonConvert.DeserializeObject<JsonUser>(jsonString);

            var user = new User
            {
                Username = jsonUser.username,
                UserId = jsonUser.id,
                GameCurrency = jsonUser.game_currency,
                PremiumCurrency = jsonUser.premium_currency,
                _googleToken = jsonUser.google_token,
                email = jsonUser.email
            };
            user.LoadImage();

            return user;
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

        public static string GetGoogleTokenFromJsonString(string jsonString)
        {
            var jsonUser = JsonConvert.DeserializeObject<JsonUser>(jsonString);
            return jsonUser.google_token;
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