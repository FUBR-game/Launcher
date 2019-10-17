using Newtonsoft.Json;

namespace Launcher.Models
{
    internal class JsonUser
    {
        public int id;
        public string username;
        public string google_token;
        public string last_online;
        public int game_currency;
        public int premium_currency;
        public string created_at;
        public string updated_at;
    }
    public class User
    {
        public string Username;
        public int UserId;
        public int Game_Currency;
        public int PremiumCurrency;
        private User(){}

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
