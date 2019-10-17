using Newtonsoft.Json;

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
        public int Game_Currency;
        public int PremiumCurrency;
        public int UserId;
        public string Username;

        private User()
        {
        }

        public static User UserFromJsonString(string jsonString)
        {
            var jsonUser = JsonConvert.DeserializeObject<JsonUser>(jsonString);

            return new User
            {
                Username = jsonUser.username,
                UserId = jsonUser.id,
                Game_Currency = jsonUser.game_currency,
                PremiumCurrency = jsonUser.premium_currency
            };
        }
    }
}