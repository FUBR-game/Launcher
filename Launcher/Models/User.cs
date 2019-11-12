using System.Buffers.Text;
using Newtonsoft.Json;
using static System.Text.Encoding;

namespace Launcher.Models
{
    internal class JsonUser
    {
        public string created_at;
        public int game_currency;
        public string google_token;
        public int id;
        public string last_online;
        public int premium_currency;
        public string updated_at;
        public string username;
    }

    public class User
    {
        private static User currentUser = new User
        {
            Game_Currency = 0,
            PremiumCurrency = 0,
            googleToken = "104372892978312113975",
            UserId = 1,
            Username = "Aranna",
        };
        
        public int Game_Currency;
        private string googleToken;
        public int PremiumCurrency;
        public int UserId;
        public string Username;
        public UserStatus UserStatus = UserStatus.Online;
        public string Image = "avares://Launcher/Assets/50x50image.png";

        private User()
        {
        }

        public string GetGoogleToken()
        {
            return googleToken;
        }

        public static User UserFromJsonString(string jsonString)
        {
            var jsonUser = JsonConvert.DeserializeObject<JsonUser>(jsonString);

            return new User
            {
                Username = jsonUser.username,
                UserId = jsonUser.id,
                Game_Currency = jsonUser.game_currency,
                PremiumCurrency = jsonUser.premium_currency,
                googleToken = jsonUser.google_token
            };
        }

        public static User UserFromBase64String(string base64String)
        {
            var encodedBytes = UTF8.GetBytes(base64String);
            var userJsonBytes = new byte[encodedBytes.Length];
            Base64.DecodeFromUtf8(encodedBytes, userJsonBytes, out var bytesConsumed, out var bytesWritten);

            var userJsonString = UTF8.GetString(userJsonBytes);
            return UserFromJsonString(userJsonString);
        }

        public static string getGoogleTokenFromJsonString(string jsonString)
        {
            var jsonUser = JsonConvert.DeserializeObject<JsonUser>(jsonString);
            return jsonUser.google_token;
        }

        public static void setCurrentUser(User user)
        {
            currentUser = user;
        }

        public static User GetCurrentUser()
        {
            return currentUser;
        }
    }
}